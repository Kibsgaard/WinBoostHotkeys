using System;
using System.Threading;
using System.Windows.Forms;
using WinBoostHotkeys.Resources;

namespace WinBoostHotkeys
{
    internal static class Program
    {
        private static Mutex? _mutex;
        private const string MutexName = "WinBoostHotkeys_SingleInstance";

        [STAThread]
        static void Main()
        {
            // Single-instance check
            bool createdNew;
            _mutex = new Mutex(true, MutexName, out createdNew);

            if (!createdNew)
            {
                // Another instance is already running
                MessageBox.Show(
                    Strings.MessageAlreadyRunning,
                    Strings.AppName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            try
            {
                Application.Run(new MainForm());
            }
            finally
            {
                _mutex?.ReleaseMutex();
                _mutex?.Dispose();
            }
        }
    }
}
