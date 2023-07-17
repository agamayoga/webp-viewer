using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Agama
{
    public class WebPDecoder
    {
        //Cross-thread protection: lock object
        private static object locker = new object();

        //List of loaded libraries
        private static Dictionary<string, IntPtr> loaded = new Dictionary<string, IntPtr>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Loads external library from file.
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        protected static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, uint dwFlags);

        /// <summary>
        /// Gets the libwebp.dll version.
        /// </summary>
        [DllImportAttribute("libwebp", EntryPoint = "WebPGetDecoderVersion", CallingConvention = CallingConvention.Cdecl)]
        protected static extern int WebPGetDecoderVersion();

        /// <summary>
        /// Decodes WebP image from a buffer.
        /// </summary>
        [DllImportAttribute("libwebp", EntryPoint = "WebPDecodeBGRAInto", CallingConvention = CallingConvention.Cdecl)]
        protected static extern IntPtr WebPDecodeBGRAInto([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);

        /// <summary>
        /// Get basic header information: width, height. Also used for validation.
        /// </summary>
        [DllImportAttribute("libwebp", EntryPoint = "WebPGetInfo", CallingConvention = CallingConvention.Cdecl)]
        protected static extern int WebPGetInfo([InAttribute()] IntPtr data, UIntPtr data_size, ref int width, ref int height);

        /// <summary>
        /// Constructor
        /// </summary>
        public WebPDecoder()
        {
        }

        /// <summary>
        /// Gets WebP image info. Also to validate if data is in correct WebP format.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public unsafe WebPImageInfo GetImageInfo(byte[] data, long length)
        {
            fixed (byte* dataptr = data)
            {
                return GetImageInfo((IntPtr)dataptr, length);
            }
        }

        protected WebPImageInfo GetImageInfo(IntPtr data, long length)
        {
            //Validate header and determine size
            int w = 0, h = 0;
            if (WebPDecoder.WebPGetInfo(data, (UIntPtr)length, ref w, ref h) == 0)
            {
                throw new Exception("Invalid WebP header detected");
            }

            return new WebPImageInfo() { Width = w, Height = h };
        }

        /// <summary>
        /// Decodes WebP image as System.Drawing.Bitmap object.
        /// </summary>
        /// <param name="data">Raw WebP file data as byte array.</param>
        /// <param name="length">Length of byte array.</param>
        /// <returns></returns>
        public unsafe Bitmap DecodeFromBytes(byte[] data, long length)
        {
            fixed (byte* dataptr = data)
            {
                return DecodeFromPointer((IntPtr)dataptr, length);
            }
        }

        protected Bitmap DecodeFromPointer(IntPtr data, long length)
        {
            //Validate header and determine size
            int w = 0, h = 0;
            if (WebPDecoder.WebPGetInfo(data, (UIntPtr)length, ref w, ref h) == 0)
            {
                throw new Exception("Invalid WebP header detected");
            }

            bool success = false;
            Bitmap b = null;
            BitmapData bd = null;
            try
            {
                //Allocate canvas
                b = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                //Lock surface for writing
                bd = b.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                //Decode to surface
                IntPtr result = WebPDecoder.WebPDecodeBGRAInto(data, (UIntPtr)length, bd.Scan0, (UIntPtr)(bd.Stride * bd.Height), bd.Stride);
                if (bd.Scan0 != result)
                {
                    throw new Exception("Failed to decode WebP image with error " + (long)result);
                }

                success = true;
            }
            finally
            {
                //Unlock surface
                if (bd != null && b != null)
                {
                    b.UnlockBits(bd);
                }

                //Dispose of bitmap if anything went wrong
                if (!success && b != null)
                {
                    b.Dispose();
                }
            }
            return b;
        }

        /// <summary>
        /// Gets the libwebp.dll version.
        /// </summary>
        /// <returns></returns>
        public static string GetDecoderVersion()
        {
            uint v = (uint)WebPDecoder.WebPGetDecoderVersion();
            var revision = v % 256;
            var minor = (v >> 8) % 256;
            var major = (v >> 16) % 256;
            return major + "." + minor + "." + revision;
        }

        /// <summary>
        /// Calls system function to load an assembly.
        /// </summary>
        protected static IntPtr LoadLibraryFromPath(string fullPath)
        {
            const uint LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100;
            const uint LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800;

            var moduleHandle = LoadLibraryEx(fullPath, IntPtr.Zero, LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR | LOAD_LIBRARY_SEARCH_SYSTEM32);
            if (moduleHandle == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return moduleHandle;
        }

        /// <summary>
        /// Loads libwebp.dll from "x86" or "x64" folder, depending on the executing architecture.
        /// </summary>
        public static void LoadLibrary()
        {
            string name = "libwebp.dll";
            string arch = (IntPtr.Size == 8) ? "x64" : "x86";

            var current = Assembly.GetExecutingAssembly();

            //Directories to search for
            var searchFolders = new string[]
            {
                Path.GetDirectoryName(current.Location),
                Path.GetDirectoryName(new Uri(current.CodeBase).LocalPath)
            };

            string searched = "";
            foreach (string folder in searchFolders)
            {
                //Build absolute path to the .dll file
                string basePath = Path.Combine(folder, arch);
                string fullPath = Path.Combine(basePath, name);
                string absPath = Path.GetFullPath(fullPath); //Canonicalize
                searched = searched + "\"" + fullPath + "\", ";

                if (File.Exists(fullPath))
                {
                    lock (locker)
                    {
                        if (loaded.ContainsKey(absPath))
                        {
                            //Already loaded
                            return;
                        }

                        //Load now
                        IntPtr handle = LoadLibraryFromPath(absPath);
                        if (handle != IntPtr.Zero)
                        {
                            loaded.Add(absPath, handle);
                            return;
                        }

                        throw new FileNotFoundException("Failed to load " + name);
                    }

                }
            }

            throw new FileNotFoundException("Failed to locate " + name + " as " + searched.TrimEnd(' ', ','));
        }
    }

    public struct WebPImageInfo
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}