using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace WinBoostHotkeys
{
    public class NetworkMonitor : IDisposable
    {
        private string? _lastWifiSsid;
        private bool _lastEthernetConnected;
        private bool _isMonitoring;

        public event EventHandler<NetworkChangedEventArgs>? NetworkChanged;

        public void StartMonitoring()
        {
            if (_isMonitoring) return;

            _isMonitoring = true;
            _lastWifiSsid = GetCurrentWifiSsid();
            _lastEthernetConnected = IsEthernetConnected();

            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
        }

        public void StopMonitoring()
        {
            if (!_isMonitoring) return;

            _isMonitoring = false;
            NetworkChange.NetworkAddressChanged -= NetworkChange_NetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged -= NetworkChange_NetworkAvailabilityChanged;
        }

        private void NetworkChange_NetworkAddressChanged(object? sender, EventArgs e)
        {
            // Delay to allow network to stabilize
            Task.Delay(1000).ContinueWith(_ => CheckNetworkChanges());
        }

        private void NetworkChange_NetworkAvailabilityChanged(object? sender, NetworkAvailabilityEventArgs e)
        {
            Task.Delay(1000).ContinueWith(_ => CheckNetworkChanges());
        }

        private void CheckNetworkChanges()
        {
            string? currentWifiSsid = GetCurrentWifiSsid();
            bool currentEthernetConnected = IsEthernetConnected();

            bool wifiChanged = currentWifiSsid != _lastWifiSsid;
            bool ethernetChanged = currentEthernetConnected != _lastEthernetConnected;

            if (wifiChanged || ethernetChanged)
            {
                _lastWifiSsid = currentWifiSsid;
                _lastEthernetConnected = currentEthernetConnected;

                NetworkChanged?.Invoke(this, new NetworkChangedEventArgs
                {
                    WifiSsid = currentWifiSsid,
                    IsEthernetConnected = currentEthernetConnected
                });
            }
        }

        public string? GetCurrentWifiSsid()
        {
            return GetWifiSsidFromNetsh();
        }

        private string? GetWifiSsidFromNetsh()
        {
            try
            {
                var processStartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "wlan show interfaces",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = System.Diagnostics.Process.Start(processStartInfo);
                if (process == null) return null;

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Parse SSID from output: "SSID                   : NetworkName"
                var lines = output.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Trim().StartsWith("SSID", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = line.Split(':');
                        if (parts.Length > 1)
                        {
                            string ssid = parts[1].Trim();
                            if (!string.IsNullOrEmpty(ssid) && ssid != "none")
                            {
                                return ssid;
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        public bool IsEthernetConnected()
        {
            try
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                return interfaces.Any(ni =>
                    ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    ni.OperationalStatus == OperationalStatus.Up &&
                    ni.GetIPProperties().UnicastAddresses.Any());
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            StopMonitoring();
        }
    }

    public class NetworkChangedEventArgs : EventArgs
    {
        public string? WifiSsid { get; set; }
        public bool IsEthernetConnected { get; set; }
    }
}
