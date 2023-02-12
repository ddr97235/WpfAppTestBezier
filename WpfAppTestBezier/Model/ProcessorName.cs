using Microsoft.Win32;

namespace WpfAppTestBezier.Model
{
    class ProcessorName
    {
        private static string GetProcessorName()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0\");
            return key?.GetValue("ProcessorNameString")?.ToString() ?? "Not Found";
        }
        public string CPUName => GetProcessorName();
    }
}
