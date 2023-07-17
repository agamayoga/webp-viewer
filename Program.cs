using System;
using System.Windows.Forms;

namespace Agama
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            //Load libwebp.dll
            WebPDecoder.LoadLibrary();

            //Test libwebp.dll
            WebPDecoder.GetDecoderVersion();

            //Open the main window
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
