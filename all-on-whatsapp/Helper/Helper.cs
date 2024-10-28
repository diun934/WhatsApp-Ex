using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using HandyControl.Controls;
using HandyControl.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Threading;

//全局辅助方法

namespace all_on_whatsapp
{
    public class WindowHelper
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        private delegate bool EnumChildProc(IntPtr hwnd, IntPtr lParam);

        /// <summary>
        /// 获取指定窗口的所有子窗口句柄，根据子窗口的类名进行过滤。
        /// </summary>
        /// <param name="parentHandle">父窗口句柄</param>
        /// <param name="className">子窗口的类名</param>
        /// <returns>符合条件的子窗口句柄列表</returns>
        public static List<IntPtr> GetChildWindowsWithClass(IntPtr parentHandle)
        {
            string targetClassName = "#32770";
            //Debug.WriteLine($"Parent Window Handle: {parentHandle}");
            List<IntPtr> childWindows = new List<IntPtr>();

            EnumChildWindows(parentHandle, (hwnd, lParam) =>
            {
                StringBuilder classNameBuilder = new StringBuilder(256);
                int classNameLength = GetClassName(hwnd, classNameBuilder, classNameBuilder.Capacity);

                if (classNameLength > 0)
                {
                    string currentClassName = classNameBuilder.ToString();
                    //Debug.WriteLine($"Found child window: Handle = {hwnd}, Class Name = {currentClassName}");

                    // 比较子窗口的类名
                    if (currentClassName.Equals(targetClassName, StringComparison.Ordinal))
                    {
                        childWindows.Add(hwnd);
                    }
                }
                else
                {
                    // Debug.WriteLine($"Failed to get class name for window handle: {hwnd}");
                }

                return true; // 继续枚举下一个子窗口
            }, IntPtr.Zero);

            return childWindows;
        }

        /// <summary>
        /// 获取窗口的标题文本。
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <returns>窗口标题文本</returns>
        public static string GetWindowTitle(IntPtr hwnd)
        {
            StringBuilder windowText = new StringBuilder(256);
            GetWindowText(hwnd, windowText, windowText.Capacity);
            return windowText.ToString();
        }
    }

    //模拟键盘
    public class KeyboardSimulator
    {
        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        const int KEYEVENTF_KEYDOWN = 0x0000;  // Key down flag
        const int KEYEVENTF_KEYUP = 0x0002;    // Key up flag
        const int INPUT_KEYBOARD = 1;          // Input type keyboard

        public static void SimulateKeyPress(ushort keyCode)
        {
            INPUT[] inputs = new INPUT[2];
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].u.ki = new KEYBDINPUT { wVk = keyCode, dwFlags = KEYEVENTF_KEYDOWN };
            inputs[1].type = INPUT_KEYBOARD;
            inputs[1].u.ki = new KEYBDINPUT { wVk = keyCode, dwFlags = KEYEVENTF_KEYUP };

            SendInput(2, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public static void SimulateKeyCombo(ushort keyModifier, ushort keyCode)
        {
            INPUT[] inputs = new INPUT[4];
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].u.ki = new KEYBDINPUT { wVk = keyModifier, dwFlags = KEYEVENTF_KEYDOWN };
            inputs[1].type = INPUT_KEYBOARD;
            inputs[1].u.ki = new KEYBDINPUT { wVk = keyCode, dwFlags = KEYEVENTF_KEYDOWN };
            inputs[2].type = INPUT_KEYBOARD;
            inputs[2].u.ki = new KEYBDINPUT { wVk = keyCode, dwFlags = KEYEVENTF_KEYUP };
            inputs[3].type = INPUT_KEYBOARD;
            inputs[3].u.ki = new KEYBDINPUT { wVk = keyModifier, dwFlags = KEYEVENTF_KEYUP };

            SendInput(4, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public static void SelectAll()
        {
            SimulateKeyCombo(0x11, 0x41); // Ctrl+A
        }

        public static void Copy()
        {
            SimulateKeyCombo(0x11, 0x43); // Ctrl+C
        }

        public static void Paste()
        {
            SimulateKeyCombo(0x11, 0x56); // Ctrl+V
        }
    }

    //剪贴板操作
    public class ClipboardAPI
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        public static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll")]
        public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("user32.dll")]
        public static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        public static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalFree(IntPtr hMem);

        private const uint CF_UNICODETEXT = 13;
        private const uint GHND = 0x0042;

        public static string? GetText()
        {
            if (!IsClipboardFormatAvailable(CF_UNICODETEXT))
                return null;

            if (!OpenClipboard(IntPtr.Zero))
                return null;

            IntPtr hGlobal = GetClipboardData(CF_UNICODETEXT);
            if (hGlobal == IntPtr.Zero)
            {
                CloseClipboard();
                return null;
            }

            IntPtr lpGlobal = GlobalLock(hGlobal);
            if (lpGlobal == IntPtr.Zero)
            {
                CloseClipboard();
                return null;
            }

            string clipboardText = Marshal.PtrToStringUni(lpGlobal)!;
            GlobalUnlock(hGlobal);
            CloseClipboard();
            return clipboardText;
        }

        public static void SetText(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (!OpenClipboard(IntPtr.Zero))
                return;

            EmptyClipboard();

            int bytes = (text.Length + 1) * 2; // 2 bytes per character for Unicode
            IntPtr hGlobal = GlobalAlloc(GHND, (UIntPtr)bytes);
            if (hGlobal == IntPtr.Zero)
            {
                CloseClipboard();
                return;
            }

            IntPtr lpGlobal = GlobalLock(hGlobal);
            if (lpGlobal != IntPtr.Zero)
            {
                Marshal.Copy(text.ToCharArray(), 0, lpGlobal, text.Length);
                Marshal.WriteInt16(lpGlobal, text.Length * 2, 0); // Null-terminate the string
                GlobalUnlock(hGlobal);
                SetClipboardData(CF_UNICODETEXT, hGlobal);
            }
            else
            {
                GlobalFree(hGlobal);
            }

            CloseClipboard();
        }
    }

    //吐司通知
    public static class AppNotice
    {
        public static void Show(string title, string contont, AppNotificationLevel level, int displayTimeInSeconds = 3, string sender = "System")
        {
            Notification.Show(new AppUserControl.AppNotification(new AppNotificationViewModel()
            {
                Title = title,
                Content = contont,
                Level = level,
                Sender = sender

            }, displayTimeInSeconds), ShowAnimation.Fade, true);
        }
    }

    //全局帮助类
    public static class Helper
    {
        //填充文件名
        public static async Task<bool> FillFileName(string filePath)
        {
            return await Task.Run(async () =>
            {
                // 获取当前应用程序的进程
                var currentProcess = Process.GetCurrentProcess();

                using (var automation = new UIA3Automation())
                {
                    // 附加到当前运行的应用程序
                    var app = FlaUI.Core.Application.Attach(currentProcess.Id);

                    // 获取应用程序的主窗口
                    var mainWindow = app.GetMainWindow(automation);

                    try
                    {
                        FlaUI.Core.AutomationElements.Window? fileDialog = null;

                        var stopwatch = Stopwatch.StartNew();

                        while (stopwatch.Elapsed < TimeSpan.FromSeconds(5))
                        {
                            fileDialog = mainWindow.ModalWindows.FirstOrDefault(w => w.ClassName == "#32770");
                            if (fileDialog != null) break;
                            await Task.Delay(500); // 暂停一段时间后重试
                        }

                        if (fileDialog != null)
                        {
                            // 查找文件路径输入框，并设置文件路径
                            var filePathBox = fileDialog.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit).And(cf.ByAutomationId("1148"))).AsTextBox();

                            if (filePathBox != null)
                            {
                                filePathBox.Text = filePath;

                                // 点击“打开”按钮
                                var openButton = fileDialog.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button).And(cf.ByAutomationId("1"))).AsButton();

                                if (openButton != null)
                                {
                                    openButton.Invoke();
                                    return true; // 成功操作
                                }
                            }

                            // 如果操作未成功，关闭文件对话框
                            fileDialog.Close();
                        }
                        return false; // 未找到文件对话框或控件
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"填充文件名时出错：{ex.Message}");

                        return false; // 操作失败
                    }
                }
            });
        }
        public static string ReadJs(string jsName)
        {
            // 假设JavaScript文件存放在项目的 "JavaScript" 文件夹下
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string jsFolderPath = Path.Combine(basePath, "Helper/JavaScript");
            string jsFilePath = Path.Combine(jsFolderPath, jsName);

            try
            {
                // 尝试读取文件内容
                string scriptContent = File.ReadAllText($"{jsFilePath}.js");
                return scriptContent;
            }
            catch (Exception ex)
            {
                // 处理文件不存在或其他读取错误的情况
                Logger.Error("Failed to read the JavaScript file: " + ex.Message);
                return string.Empty; // 或者根据需要返回适当的错误消息或错误代码
            }
        }
    }
}
