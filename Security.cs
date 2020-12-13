using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;

namespace ST
{
    class Security
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        static bool isDebuggerPresent = false;
        public static void Initialize()
        {
            DetectSandboxie();
            AntiDebug();
            AntiDnspy();
            DetectVM();
        }
        private static bool PinPublicKey(object sender, X509Certificate certificate,
                                 X509Chain chain, SslPolicyErrors sslPolicyErrors) //The SSL Pinning
        {
            if (certificate == null || chain == null)
                return false;

            if (sslPolicyErrors != SslPolicyErrors.None)
                return false;

            string pk = certificate.GetPublicKeyString(); //if you ever need to update just make it wirteout pk and then update it. (Console.WriteLien(pk);)
            return pk.Equals("04054F9BE28FAAF5B5F960DB2B3E5D743C9B6B8435B762E704F8E07797C12E2F7630EB7DE33A6EE5DDA3A814E40C05FE95822CDB4F732AA062F357BC3FBE0214E8"); //you may need to update this if discord changes it
        }
        private static void AntiDnspy()
        {
            IntPtr kernel32 = LoadLibrary("kernel32.dll");
            IntPtr GetProcessId = GetProcAddress(kernel32, "IsDebuggerPresent");
            byte[] data = new byte[1];
            Marshal.Copy(GetProcessId, data, 0, 1);
            if (data[0] == 0xE9)
            {
                MessageBox.Show("Common Debugger Detected!", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CleanUp();
                Destruct();
                return;
            }
            GetProcessId = GetProcAddress(kernel32, "CheckRemoteDebuggerPresent");
            data = new byte[1];
            Marshal.Copy(GetProcessId, data, 0, 1);
            if (data[0] == 0xE9)
            {
                MessageBox.Show("Common Debugger Detected!", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CleanUp();
                Destruct();
                return;
            }
        }
        private static void AntiDebug()
        {
            CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isDebuggerPresent);
            if (isDebuggerPresent)
            {
                MessageBox.Show("Common Debugger Detected!", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CleanUp();
                Destruct();
            }
        }
        private static void DetectSandboxie()
        {
            if (GetModuleHandle("SbieDll.dll").ToInt32() != 0)
            {
                MessageBox.Show("Sandboxie Detected!", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CleanUp();
                Destruct();
            }
            else
            {
            }
        }
        private static void DetectVM()
        {
            using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
            {
                using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
                {
                    foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
                    {
                        if ((managementBaseObject["Manufacturer"].ToString().ToLower() == "microsoft corporation" && managementBaseObject["Model"].ToString().ToUpperInvariant().Contains("VIRTUAL")) || managementBaseObject["Manufacturer"].ToString().ToLower().Contains("vmware") || managementBaseObject["Model"].ToString() == "VirtualBox")
                        {
                            MessageBox.Show("VM Detected!", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            CleanUp();
                            Destruct();
                        }
                    }
                }
            }
            foreach (ManagementBaseObject managementBaseObject2 in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController").Get())
            {
                if (managementBaseObject2.GetPropertyValue("Name").ToString().Contains("VMware") && managementBaseObject2.GetPropertyValue("Name").ToString().Contains("VBox"))
                {
                    MessageBox.Show("VM Detected!", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CleanUp();
                    Destruct();
                }
            }
        }
        public static void DownloadFile(string url, string path)
        {
            ProxyCheck(); //outdated, meant for before SSL pinning
            Thread.Sleep(1000);
            HttpWebRequest.DefaultWebProxy = new WebProxy(); //outdated, meant for before SSL pinning
            Thread.Sleep(1000);
            WebClient files = new WebClient();
            ServicePointManager.ServerCertificateValidationCallback = PinPublicKey;
            Thread.Sleep(1000);
            try
            {
                files.DownloadFile(url, path);
            }
            catch (Exception ex)
            {
                if ($"{ex}".Contains("The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel."))
                {
                    MessageBox.Show("The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel.", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CleanUp();
                    Destruct();
                }
                else
                {
                    MessageBox.Show($"{ex}", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CleanUp();
                    Destruct();
                }

            }
        }
        public static void CleanUp()
        {
            try
            {
                string[] files2 = Directory.GetFiles(@"C:\ProgramData\SpooferTemplate\");
                foreach (string file in files2)
                {
                    File.Delete(file);
                }
                Directory.Delete(@"C:\ProgramData\SpooferTemplate\");
            }
            catch
            {

            }
        }
        public static void CheckHash(string file, string original)
        {
            string sourceFileName = file;
            byte[] shaHash;
            using (var shaForStream = new SHA256Managed())
            using (Stream sourceFileStream = File.Open(sourceFileName, FileMode.Open))
            using (Stream sourceStream = new CryptoStream(sourceFileStream, shaForStream, CryptoStreamMode.Read))
            {
                while (sourceStream.ReadByte() != -1) ;
                shaHash = shaForStream.Hash;
            }
            string hash = Convert.ToBase64String(shaHash);
            if (hash == original)
            {
                
            }
            else
            {
                MessageBox.Show("File Integreity Check Failed! Possible File Tampering!", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CleanUp();
                Destruct();
            }
        }
        private static void Destruct() //for some reason, this doesn't seem to work for me, maybe it will work for you?
        {
            string app = System.AppDomain.CurrentDomain.FriendlyName;
            string AppPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString() + $@"\{app}";
            Process.Start("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 3000 > Nul & Del " + AppPath);
            Process.GetCurrentProcess().Kill();
        }
        private static void ProxyCheck() //I had this before SSL pinning
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            string ProxyEnabledOrNo = registry.GetValue("ProxyEnable").ToString();
            object ProxyServerValue = registry.GetValue("ProxyServer");
            if (ProxyEnabledOrNo == "1")
            {
                MessageBox.Show("Error! Network Debugger Detected! Closing Now!", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Destruct();
            }
        }
        
    }
}
