﻿using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace common.libs
{
    /// <summary>
    /// 代理系统设置
    /// </summary>
    public sealed class ProxySystemSetting
    {
        /// <summary>
        /// windows 下， “应用级”也就是直接运行exe时，可以直接修改当前用户的注册表
        /// 而作为 windows service时，无法修改，需要模拟登录当前用户，才能修改，而模拟登录还需要SE_TCB_NAME权限，并不能保证一定修改成功，这就增加了复杂性。
        /// 所以，直接获取 HKEY_USERS 下的所有key，直接修改所有用户的注册表，反而更简单
        /// </summary>
        /// <param name="url"></param>
        private static void WindowsSet(string url)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string[] names = GetWindowsCurrentIds();
                    foreach (var item in names)
                    {
                        try
                        {
                            RegistryKey reg = Registry.Users.OpenSubKey($"{item}\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                            reg.SetValue("AutoConfigURL", url);
                            reg.Close();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    FlushOs();
                }
            }
            catch (Exception)
            {
            }
        }
        private static void WindowsClear()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string[] names = GetWindowsCurrentIds();
                    foreach (var item in names)
                    {
                        try
                        {
                            RegistryKey reg = Registry.Users.OpenSubKey($"{item}\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                            reg.DeleteValue("AutoConfigURL");
                            reg.Close();
                        }
                        catch (Exception)
                        {
                        }
                    }

                    FlushOs();
                }
            }
            catch (Exception)
            {
            }
        }

        private static void MacExcute(string command)
        {
            Process proc = new();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                proc.StartInfo.FileName = "cmd.exe";
            }
            else
            {
                proc.StartInfo.FileName = "/bin/bash";
            }
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.Verb = "runas";
            proc.Start();

            proc.StandardInput.WriteLine(command);
            proc.StandardInput.AutoFlush = true;
            proc.StandardInput.WriteLine("exit");
            proc.StandardOutput.ReadToEnd();
            proc.StandardError.ReadToEnd();
            proc.WaitForExit();
            proc.Close();
            proc.Dispose();
        }
        private static void MacSet(string url)
        {
            MacExcute($"networksetup -setautoproxyurl ethernet {url}");
            MacExcute($"networksetup -setautoproxyurl Wi-Fi {url}");
        }
        private static void MacClear()
        {
            MacExcute($"networksetup -setautoproxystate ethernet off");
            MacExcute($"networksetup -setautoproxystate Wi-Fi off");
        }
        public static void Set(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsSet(url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                MacSet(url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {

            }
        }
        public static void Clear()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsClear();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                MacClear();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {

            }
        }


        #region windows

        private static string[] GetWindowsCurrentIds()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Registry.Users.GetSubKeyNames().Where(c => c.Length > 10 && c.Contains("Classes") == false).ToArray();
            }
            return Array.Empty<string>();
        }
        [DllImport("wininet.dll")]
        static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        const int INTERNET_OPTION_REFRESH = 37;
        private static void FlushOs()
        {
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }
        #endregion
    }
}
