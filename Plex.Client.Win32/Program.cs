using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Plex.Client.Win32
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (a, b) =>
            {

            };

            Application.Run(new Form1());
        }
    }
}
