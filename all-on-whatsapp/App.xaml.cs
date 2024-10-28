using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace all_on_whatsapp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private async Task ActivateExistingWindowAsync()
        {
            // 获取当前运行的进程
            var currentProcess = Process.GetCurrentProcess();

            // 查找所有相同进程名的进程
            var runningProcess = Process.GetProcessesByName(currentProcess.ProcessName)
                .FirstOrDefault(p => p.Id != currentProcess.Id);  // 排除当前进程

            if (runningProcess != null && runningProcess.MainWindowHandle != IntPtr.Zero)
            {
                // 等待目标进程的主窗口可供交互
                runningProcess.WaitForInputIdle(1000); // 可根据需要调整等待时间

                // 将已有的窗口设置为前台窗口
                SetForegroundWindow(runningProcess.MainWindowHandle);
            }
            // 避免警告，使用 Task.CompletedTask
            await Task.CompletedTask;
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            // 检查是否已有同名进程在运行
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                await ActivateExistingWindowAsync(); // 尝试激活现有实例
                Shutdown();  // 关闭当前进程
                return;
            }

            // 设置 DPI 感知
            SetProcessDPIAware();
            base.OnStartup(e);

            // 通过 WebView2 环境来管理缓存
            string baseCachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebView2", "Cache");

            // WebView2 缓存设置
            try
            {
                CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions();
                var environment = await CoreWebView2Environment.CreateAsync(null, baseCachePath, options);
                // 在需要的地方使用 environment
            }
            catch (Exception ex)
            {
                // 处理 WebView2 环境创建的异常
                Logger.Error($"WebView2 初始化失败: {ex.Message}");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // WebView2 不需要像 CefSharp 那样手动关闭
            base.OnExit(e);
            // 释放全局资源
            Application.Current.Shutdown();
        }
    }
}
