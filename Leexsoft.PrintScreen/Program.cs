using log4net.Config;
using System;
using System.Windows.Forms;

namespace Leexsoft.PrintScreen
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            XmlConfigurator.Configure();
            Application.Run(new FormBase());            
        }
    }
}
