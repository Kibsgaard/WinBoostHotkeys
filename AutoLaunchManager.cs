using Microsoft.Win32.TaskScheduler;

namespace WinBoostHotkeys
{
    public static class AutoLaunchManager
    {
        private const string TaskName = "WinBoostHotkeys_AutoLaunch";

        public static void SetAutoLaunch(bool enable)
        {
            using (var ts = TaskService.Instance)
            {
                if (enable)
                {
                    TaskDefinition td = ts.NewTask();
                    td.RegistrationInfo.Description = "Auto-launch WinBoostHotkeys on login";
                    td.Principal.RunLevel = TaskRunLevel.Highest;
                    td.Settings.DisallowStartIfOnBatteries = false;
                    td.Settings.StopIfGoingOnBatteries = false;
                    td.Settings.ExecutionTimeLimit = TimeSpan.Zero; // Allow running indefinitely

                    td.Triggers.Add(new LogonTrigger());

                    string exePath = Application.ExecutablePath;
                    td.Actions.Add(new ExecAction(exePath));

                    // Register the task in the root folder
                    ts.RootFolder.RegisterTaskDefinition(TaskName, td);
                }
                else
                {
                    // Remove the task if it exists
                    var task = ts.FindTask(TaskName);
                    if (task != null)
                    {
                        ts.RootFolder.DeleteTask(TaskName);
                    }
                }
            }
        }

        public static bool IsAutoLaunchEnabled()
        {
            using (var ts = TaskService.Instance)
            {
                var task = ts.FindTask(TaskName);
                return task != null;
            }
        }
    }
}
