using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace FastResampler
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] argv)
        {
            Console.Clear();
            if (argv.Length == 1)
            {
                Utils.tempTitle2 = argv[0];
                Console.Title = Utils.tempTitle;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                try
                {
                    Application.Run(new Form1());
                }
                catch (Exception e)
                {
                    Utils.log("error: " + string.Format("{0}, {1}", e.Message, e.StackTrace));
                }
            }
            else
            {
                EventWaitHandle _mainWaitHandle = new AutoResetEvent(false);
                Console.Title = Utils.tempTitle;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = Application.ExecutablePath;
                process.StartInfo.WorkingDirectory = System.Environment.CurrentDirectory;
                process.StartInfo.Arguments = string.Format("\"{0}\"", Utils.tempTitle);
                process.Start();
                Console.ReadLine();
            }
        }
    }
}
