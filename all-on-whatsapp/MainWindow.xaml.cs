using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using all_on_whatsapp.ViewModel;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;
using all_on_whatsapp.View;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using HandyControl.Controls;
using System;
using System.Reflection;




namespace all_on_whatsapp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        //监测双击ctrl键的私有变量和参数
        private DateTime lastCtrlPress = DateTime.MinValue;
        private bool ctrlPressedOnce = false;
        private const double DoublePressMaxMilliSeconds = 500;
        private string lastBrowserTag = string.Empty;

        //避免翻译时的模拟按键跟双击的标志位
        private bool ignoreCtrlDoubleClick = false;

        //浏览器实例列表
        private List<WebView2> browserList { get; } = new List<WebView2>();

        //app视图模型
        private MainWindowViewModel viewModel;

        //app设置模型
        Appsetings appsetings { get; set; } = new Appsetings();

        //数据库连接字符串
        private string connectionString = string.Empty;
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.Title = $"WhatsApp Business Ex - v{(Assembly.GetExecutingAssembly()).GetName().Version}";

            // 初始化 ViewModel
            viewModel = new MainWindowViewModel();

            // 设置 DataContext
            DataContext = viewModel;

            //窗口初始化事件
            WindowInitialization();

            this.Deactivated += MainWindow_Deactivated!;

            // 监听主窗口的 LocationChanged 事件
            this.LocationChanged += MainWindow_LocationChanged!;
        }
        /// <summary>
        /// 窗口初始化
        /// </summary>
        private async void WindowInitialization()
        {
            //设置窗口位置和大小
            double screenHeight = SystemParameters.WorkArea.Height;
            Width = 1200; Height = screenHeight; Left = 0; Top = 3;

            //初始化数据库连接字符串
            appsetings = await ConfigManager.LoadAppConfigAsync();
            connectionString = $"Server=154.23.179.240;Database={appsetings.User};User={appsetings.User};Password={appsetings.Password};";

            //初始化导航栏位置显示模式
            var NavigationBarMode = appsetings.NavigationWidthMode;
            if (NavigationBarMode == 0)
            {
                NavigationButtonsGroupB.Visibility = Visibility.Collapsed;
                NavigationButtonsGroupA.Visibility = Visibility.Visible;
            }
            else
            {
                NavigationButtonsGroupA.Visibility = Visibility.Collapsed;
                NavigationButtonsGroupB.Visibility = Visibility.Visible;
            }

            //初始化实例集合(从配置文件)
            viewModel.BrowserGroups = await ConfigManager.LoadBrowserConfigAsync();

            //初始化的时候未读消息归0,浏览器全部标记为未加载
            foreach (var browserGroup in viewModel.BrowserGroups)
            {
                browserGroup.ButtonsModel.UnreadMessagesCount = 0;
                browserGroup.BrowserModel.IsLoaded = false;
                browserGroup.IsChecked = false;
            }

            //创建浏览器实例并初始化
            await CreateWebView2BrowseAsync();

            //初始化置顶用户列表
            viewModel.PinUserList = await ConfigManager.LoadPinUserListAsync();
        }
        /// <summary>
        /// 禁用/启用浏览器实例
        /// </summary>
        /// <param name="browserGroup"></param>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public async void IsEnableBrowser(BrowserGroup browserGroup)
        {
            var browser = browserList.FirstOrDefault(x => x.Tag.Equals(browserGroup.Tag));
            if (browser == null) return;

            if (browserGroup.IsEnable)//用户启用该实例
            {
                //还未加载过的状态下禁用
                if (!browserGroup.BrowserModel.IsLoaded)
                {
                    //初始化实例
                    await InitializeBrowserAsync(browser, browserGroup);
                }
                else//加载过的状态下禁用
                {
                    //刷新实例
                    if (browser.CoreWebView2 != null) browser.Reload();
                }

                //显示这个浏览器并解除禁用，避免是被禁用后又启用的情况
                browser.Visibility = Visibility.Visible;
                browser.IsEnabled = true;

                //跳转到该浏览器
                await ToggleWebview2BrowserAsync(browserGroup.Tag);
            }
            else//用户禁用该实例
            {
                //隐藏此浏览器
                browser.Visibility = Visibility.Collapsed;
                //标记为禁用
                browserGroup.IsEnable = false;
                browser.IsEnabled = false;

                //启用禁用背景
                viewModel.InfoBarContent.DisableIndication = Visibility.Visible;
            }
        }
        /// <summary>
        /// 主窗口移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            if (NewMessageBox_V.IsOpen)
            {
                NewMessageBox_V.IsOpen = false;
            }
        }
        /// <summary>
        /// 窗口失去焦点时的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            // 应用失去焦点时隐藏Popup
            NewMessageBox_V.IsOpen = false;
        }
        /// <summary>
        /// 创建浏览器方法实现
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task CreateWebView2BrowseAsync(bool isNewBrowser = false)
        {
            // 是否完成首例初始化
            bool isFirstInitialization = false;

            // 1. 创建控件并加入到容器中
            int browserStartIndex = isNewBrowser ? viewModel.BrowserGroups.Count - 1 : 0;
            int browserEndIndex = viewModel.BrowserGroups.Count;

            for (int i = browserStartIndex; i < browserEndIndex; i++)
            {
                // 首先创建控件并加入WebView2Canvas中，并绑定 WebView2 的宽度和高度到 ViewModel 的属性
                var browser = new WebView2 { Tag = viewModel.BrowserGroups[i].Tag };
                BindingOperations.SetBinding(browser, FrameworkElement.WidthProperty, new Binding("Webview2BrowserWidth") { Source = viewModel, Mode = BindingMode.TwoWay });
                BindingOperations.SetBinding(browser, FrameworkElement.HeightProperty, new Binding("Webview2BrowserHeight") { Source = viewModel, Mode = BindingMode.TwoWay });

                // 设置浏览器初始背景颜色
                browser.DefaultBackgroundColor = System.Drawing.ColorTranslator.FromHtml("#F0F2F5");
                // 设置控件位置并加入父容器中
                Canvas.SetLeft(browser, -9999 * (i + 1)); // 设置位置
                WebView2Canvas.Children.Add(browser);

                // 将当前浏览器加入活动浏览器列表
                if (!browserList.Contains(browser)) browserList.Add(browser);
            }

            // 2. 依次初始化浏览器或手动初始化浏览器
            int initStartIndex = isNewBrowser ? viewModel.BrowserGroups.Count - 1 : 0;
            int initEndIndex = browserList.Count;

            for (int i = initStartIndex; i < initEndIndex; i++)
            {
                // 跳过已禁用的实例
                if (!viewModel.BrowserGroups[i].IsEnable) continue;

                // 一个接一个的初始化WebView2 实例
                if (await InitializeBrowserAsync(browserList[i], viewModel.BrowserGroups[i]))
                {
                    viewModel.BrowserGroups[i].BrowserModel.IsLoaded = true;
                    await Task.Delay(500);
                }

                if (!isFirstInitialization && viewModel.BrowserGroups[i].IsEnable)
                {
                    await ToggleWebview2BrowserAsync(viewModel.BrowserGroups[i].Tag);
                    // 为初始InfoBarContent Tag赋值
                    viewModel.InfoBarContent.Tag = viewModel.BrowserGroups[i].Tag;
                    isFirstInitialization = true;
                }
            }
        }
        /// <summary>
        /// 初始化浏览器实例实现
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="cachePath"></param>
        /// <param name="url"></param>
        private async Task<bool> InitializeBrowserAsync(WebView2 browser, BrowserGroup browserGroup)
        {
            try
            {
                //已经禁用的实例不加载
                if (!browserGroup.IsEnable) return false;

                // 完成缓存文件夹路径的设置
                var cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebView2", "Cache", browserGroup.BrowserModel.CachePath);

                //配置浏览器代理
                var options = new CoreWebView2EnvironmentOptions();
                if (browserGroup.BrowserModel.IsEnableProxy)
                {
                    // 包括用户名和密码的代理设置
                    options.AdditionalBrowserArguments = $"--proxy-server={browserGroup.BrowserModel.User}:{browserGroup.BrowserModel.Password}@{browserGroup.BrowserModel.IpAddress}:{browserGroup.BrowserModel.Port}";

                }

                // 创建 CoreWebView2Environment 并设置缓存路径
                var environment = await CoreWebView2Environment.CreateAsync(null, cachePath, options);

                // 使用环境初始化 WebView2 控件
                await browser.EnsureCoreWebView2Async(environment);

                // 设置浏览器地址
                browser.Source = new Uri(browserGroup.BrowserModel.Url);

                // 添加标志位，用于跟踪是否已经处理过通知权限
                bool isNotificationPermissionHandled = false;

                // 自动运行通知权限，以接收消息
                browser.CoreWebView2.PermissionRequested += (sender, args) =>
                {
                    if (args.PermissionKind == CoreWebView2PermissionKind.Notifications && !isNotificationPermissionHandled)
                    {
                        // 自动允许通知权限
                        args.State = CoreWebView2PermissionState.Allow;

                        // 刷新浏览器，仅第一次请求时执行
                        browser.Reload();

                        // 标记已处理，防止重复刷新
                        isNotificationPermissionHandled = true;
                    }
                };

                // 注册页面加载完成事件，加载成功时注入 JavaScript
                browser.CoreWebView2.NavigationCompleted += async (sender, args) =>
                {
                    if (args.IsSuccess && browser.CoreWebView2 != null && browserGroup.ItemType == 0)
                    {
                        try
                        {
                            //获取当前whatsapp实例选中的用户名称和头像
                            await browser.CoreWebView2.ExecuteScriptAsync(Helper.ReadJs("GetSelectorObject"));
                            //此脚本实现当切换对象的时候获取对象的名称
                            await browser.CoreWebView2.ExecuteScriptAsync(Helper.ReadJs("ObjectMonitoring"));
                            //拦截whatsapp通知
                            await browser.CoreWebView2.ExecuteScriptAsync(Helper.ReadJs("ReceiveNotification"));

                            //监测未读消息变化以及插入翻译按钮
                            await browser.CoreWebView2.ExecuteScriptAsync(Helper.ReadJs("MonitorUnread"));
                            //用来向whatsapp联系人提交消息到编辑框
                            await browser.CoreWebView2.ExecuteScriptAsync(Helper.ReadJs("SubstituteMessage"));
                            //根据名称点击whatsapp联系人
                            await browser.CoreWebView2.ExecuteScriptAsync(Helper.ReadJs("SubmitClickSender"));
                            //自动发送动态（图文或纯文字）
                            await browser.CoreWebView2.ExecuteScriptAsync(Helper.ReadJs("AutoSendStatus"));

                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Failed to execute script: " + ex.Message);
                        }
                    }
                };
                // 注册消息接收事件
                browser.CoreWebView2.WebMessageReceived += (sender, e) => OnWebMessageReceivedAsync(browser, sender, e);

                //标记该浏览器为已加载
                browserGroup.BrowserModel.IsLoaded = true;
                browserGroup.IsEnable = true;

                return true;  // 初始化成功，返回 true
            }
            catch (Exception ex)
            {
                Logger.Error($"初始化失败：{ex.Message}");
                return false;
            }

        }
        /// <summary>
        /// 处理浏览器发送回C#的消息
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async void OnWebMessageReceivedAsync(WebView2 browser, object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string content = e.TryGetWebMessageAsString();

            //消息为空，则早返回，不执行后续代码
            if (string.IsNullOrEmpty(content)) return;

            // 假设通知消息以 "Notification:" 开头
            if (content.StartsWith("Notification:"))
            {
                content = content.Replace("Notification:", ""); ;
                var notificationData = JsonConvert.DeserializeObject<BrowserNotificationData>(content);
                if (notificationData != null)
                {
                    await ProcessNewMessageAsync(notificationData, browser);
                }
            }
            else if (content.StartsWith("Click:"))
            {
                //Debug.WriteLine(content);
                content = content.Replace("Click:", "");

                if (content == "No selected object")
                {
                    viewModel.InfoBarContent.IsObjectOnly = Visibility.Collapsed;
                    return;
                }

                var contentParts = content.Split("|Avatar:");  // 将用户名和头像 URL 分离
                var userName = contentParts[0].Split('\n')[0].Trim();  // 取换行符之前的用户名
                var avatarUrl = contentParts.Length > 1 ? contentParts[1].Trim() : "";  // 头像URL
                avatarUrl = string.IsNullOrEmpty(avatarUrl) || avatarUrl.StartsWith("data:") ? "/Resource/DefaultAvatar.png" : avatarUrl;

                //已经置顶的对象与未置顶的对象背景色区分
                viewModel.InfoBarContent.IsPin = viewModel.PinUserList.FirstOrDefault(x => x.Name == userName) != null;
                // 查找与该用户相关的所有消息并移除
                var messagesToRemove = viewModel.NewMessageList.NewMessages.Where(m => m.Sender == userName).ToList();
                foreach (var message in messagesToRemove)
                {
                    viewModel.NewMessageList.NewMessages.Remove(message);
                }

                //导航条基本信息赋值
                viewModel.InfoBarContent.Tag = browser.Tag is string tag ? tag : string.Empty;
                viewModel.InfoBarContent.IsObjectOnly = Visibility.Visible;
                viewModel.InfoBarContent.UserName = userName;  // 设置用户名
                viewModel.InfoBarContent.AvatarUrl = avatarUrl;  // 设置头像URL
                viewModel.InfoBarContent.SystemName = "-";
                viewModel.InfoBarContent.NumberOfOrders = 0;
                viewModel.InfoBarContent.AmountOfOrders = 0;
                viewModel.InfoBarContent.Level = "-";

                //获取当前用户的数据库信息
                try
                {
                    using (var context = new AppDbContext(connectionString))
                    {
                        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == userName);

                        if (user != null)
                        {
                            var orders = await context.Orders.Where(x => x.SystemName == user.SystemName).ToListAsync();

                            viewModel.InfoBarContent.SystemName = user.UserName ?? "-";  // 设置用户名
                            viewModel.InfoBarContent.NumberOfOrders = orders.Count;
                            viewModel.InfoBarContent.AmountOfOrders = orders.Sum(x => x.BusinessAmount);
                            viewModel.InfoBarContent.Level = user.Level ?? "-";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"连接数据库配置出错: {ex.Message} | 堆栈跟踪: {ex.StackTrace}");
                    Debug.WriteLine(ex);
                }

            }
            else if (content.StartsWith("Unread:"))
            {
                //监控未读消息变化
                content = content.Replace("Unread:", "");

                int unreadCount;

                if (int.TryParse(content, out unreadCount))
                {
                    var browserGropModel = viewModel.BrowserGroups.FirstOrDefault(x => x.Tag == browser.Tag.ToString());
                    if (browserGropModel != null)
                    {
                        browserGropModel.ButtonsModel.UnreadMessagesCount = unreadCount;
                    }
                }
            }
            else if (content.StartsWith("Translation:"))
            {
                content = content.Replace("Translation:", "");
                try
                {
                    var q = YoudaoTranslator.Execute(content);
                    if (q != null)
                    {
                        // 使用 C# 的 JSON 序列化来确保文本内容被安全传递给 JavaScript
                        string escapedText = System.Text.Json.JsonSerializer.Serialize(q);

                        if (browser.CoreWebView2 != null)
                        {
                            // 通过 ExecuteScriptAsync 执行 JavaScript，传递处理过的文本
                            await browser.CoreWebView2.ExecuteScriptAsync($"substituteMessage({escapedText});");
                        }

                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"翻译出错：{ex.Message}");
                    AppNotice.Show("错误", "翻译时出现错误，请稍后重试！", AppNotificationLevel.Error);
                }

            }
        }

        /// <summary>
        /// 处理whatsapp联系人发来的新消息（通过网页通知）
        /// </summary>
        /// <param name="message"></param>
        private async Task ProcessNewMessageAsync(BrowserNotificationData messageData, WebView2 browser)
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    //如果当前浏览器实例已禁用通知，则停止向下执行代码
                    var entry = viewModel.BrowserGroups.FirstOrDefault(x => x.Tag == browser.Tag.ToString());
                    if (entry == null || !entry.BrowserModel.IsNotify) return;

                    // 统计当前联系人发送的消息总数
                    var messageCount = viewModel.NewMessageList.NewMessages.FirstOrDefault(m => m.Sender == messageData.Title)?.UnreadMessages;

                    // 查找是否已有相同发送者的消息
                    var existingMessage = viewModel.NewMessageList.NewMessages.FirstOrDefault(m => m.Sender == messageData.Title);
                    if (existingMessage != null)
                    {
                        // 如果找到已有消息，则移除
                        viewModel.NewMessageList.NewMessages.Remove(existingMessage);
                    }
                    Debug.WriteLine("11111");
                    // 添加新消息
                    viewModel.NewMessageList.NewMessages.Add(new NewMessage
                    {
                        Avatar = messageData.Options.Icon, // 头像
                        Sender = messageData.Title, // 信息发送人
                        MessageContent = messageData.Options.Body, // 内容
                        Time = DateTime.Now.ToString("HH:mm"), // 时间
                        Browser = browser, // 关联的浏览器对象
                        UnreadMessages = $"{int.Parse(messageCount ?? "0") + 1}"
                    });

                    //获取最新配置的配置项
                    appsetings = await ConfigManager.LoadAppConfigAsync();

                    if (appsetings.IsToastNotice)
                    {
                        AppNotice.Show("通知", $"{messageData.Title}{messageData.Options.Body}", AppNotificationLevel.Warning, 0, browser.Tag.ToString() ?? string.Empty);
                    }
                }
                catch (Exception ex)
                {
                    // 日志记录或其他错误处理
                    Logger.Error($"Error processing new message: {ex.Message}");
                }
            });
        }
        /// <summary>
        /// 消息中心的显示与隐藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwitchMsgCenterBtnClick(object sender, MouseButtonEventArgs e)
        {
            IsShowMessageBox();
        }
        /// <summary>
        /// 切换消息中心容器
        /// </summary>
        private void IsShowMessageBox()
        {
            // 显示或切换NewMessageBox_V的显示状态
            NewMessageBox_V.IsOpen = !NewMessageBox_V.IsOpen;
            // Debug.WriteLine(MessageBoxBorder.ActualHeight);
            //设置NewMessageBox_V出现的位置
            Point relativePoint = MessageBoxGrid.PointToScreen(new Point(0, 0));
            NewMessageBox_V.VerticalOffset = relativePoint.Y + 90;
            NewMessageBox_V.HorizontalOffset = relativePoint.X + 48;

        }
        /// <summary>
        /// 消息中心的消息点击消息事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnMessageClicked(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is NewMessageControl messageControl))
            {
                Logger.Error("MessageControl is null!");
                return;
            }

            var message = messageControl.Message;

            //先根据浏览器Id跳转到对应的浏览器
            if (message.Browser == null)
            {
                Logger.Error("Invalid Browser!");
                return;
            }

            //设置目标浏览器为可见，并将当前可见浏览器设置为不可见
            var tag = message.Browser.Tag.ToString();
            if (!string.IsNullOrEmpty(tag))
            {
                await ToggleWebview2BrowserAsync(tag, false);
            }

            // 从消息中心中移除这条消息
            viewModel.NewMessageList.NewMessages.Remove(message);

            // 隐藏消息中心视图
            IsShowMessageBox();

            if (message.Browser.CoreWebView2 != null)
            {
                // 注入JavaScript代码来点击对应的用户
                await message.Browser.CoreWebView2.ExecuteScriptAsync($"SubmitClickSender('{message.Sender}');");

                //获取用户信息
                await message.Browser.CoreWebView2.ExecuteScriptAsync($"GetSelectorObject(true);");
            }

        }
        /// <summary>
        /// 切换浏览器
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private async Task ToggleWebview2BrowserAsync(string tag, bool isNormalTrigger = true)
        {
            //获取targetBrowserGroup，currentBrowserGroup以及targetBrowser
            var currentBrowserGroup = viewModel.BrowserGroups.FirstOrDefault(x => x.IsChecked == true);
            var targetBrowserGroup = viewModel.BrowserGroups.FirstOrDefault(x => x.Tag.Equals(tag));
            var targetBrowser = browserList.FirstOrDefault(x => x.Tag.Equals(tag));
            //记录将变成上一个实例(也就是当前)的标签
            lastBrowserTag = currentBrowserGroup?.Tag ?? string.Empty;

            if (targetBrowserGroup != null && !isNormalTrigger)
            {
                var targetIndex = viewModel.BrowserGroups.IndexOf(targetBrowserGroup);

                double verticalPosition = NavigationButtonsGroupA.VerticalOffset;  // 获取垂直滚动条的位置

                NavigationButtonsGroupA.ScrollToVerticalOffset(30 * targetIndex);
            }

            if (targetBrowserGroup != null && targetBrowser != null)
            {
                // 把当前实例组都标记为未选中
                foreach (var item in viewModel.BrowserGroups)
                {
                    item.IsChecked = false;
                }

                // 标记 targetBrowserGroup 为选中
                targetBrowserGroup.IsChecked = true;
                //判断目标按钮是否在滚动区域可视范围内,如果不在，滚动到对应位置


                //将实例移动到可视范围
                double targetX = Canvas.GetLeft(targetBrowser);
                canvasTransform.X = -targetX;

                //InfoBarContent赋值更新和模型内容更新
                viewModel.InfoBarContent.DisableIndication = targetBrowserGroup.IsEnable ? Visibility.Collapsed : Visibility.Visible;
                viewModel.InfoBarContent = new InfoBarContent { Tag = tag, };

                // 执行脚本，切换到新浏览器时执行与当前会话对象的交互
                if (targetBrowser.CoreWebView2 != null)
                {
                    try
                    {
                        Grid.SetRow(targetBrowser, 0);
                        await targetBrowser.CoreWebView2.ExecuteScriptAsync("GetSelectorObject();");
                    }
                    catch (Exception ex)
                    {
                        //Debug.WriteLine($"Error executing script: {ex.Message}");
                        Logger.Error($"Error executing script: {ex.Message}");
                    }
                }
            }
        }
        /// <summary>
        /// 显示设置窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ShowSetWindowBtnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                switch (tag)
                {
                    case "Add":
                        await ShowSetWindowAsync(SetWindowMode.Add);
                        break;
                    case "Edit":
                        await ShowSetWindowAsync(SetWindowMode.Edit);
                        break;
                    case "Expansion":
                        await ShowSetWindowAsync(SetWindowMode.Extensions);
                        break;
                    default:
                        await ShowSetWindowAsync(SetWindowMode.Set);
                        break;
                }
            }
        }
        /// <summary>
        /// 根据参数显示设置窗口
        /// </summary>
        /// <param name="tag"></param>
        private async Task ShowSetWindowAsync(SetWindowMode mode, BrowserGroup? targetBrowserGroup = null)
        {
            // 深拷贝 BrowserGroups 集合，避免影响当前集合
            var browserGroups = JsonConvert.DeserializeObject<ObservableCollection<BrowserGroup>>(JsonConvert.SerializeObject(viewModel.BrowserGroups));
            if (browserGroups == null) return;

            //定义设置窗口对应的视图模型
            var setWindowViewModel = new SetWindowViewModel() { WindowMode = mode, };

            //设置实例管理页面需要的模型数据
            setWindowViewModel.Set_BrowMge = new Set_BrowserManageViewModel
            {
                BrowserGrop = targetBrowserGroup ?? new BrowserGroup { Tag = $"WhatsApp{viewModel.BrowserGroups.Where(x => x.ItemType == 0).ToList().Count + 1}", ButtonsModel = new NavigationBtnViewModel { IconSource = $"{viewModel.BrowserGroups.Where(x => x.ItemType == 0).ToList().Count + 1}" } },
                BrowserGroups = browserGroups,
            };

            // 设置Owner为当前窗口，并初始化窗口
            var setWindow = new SetWindow(setWindowViewModel)
            {
                //设置设置窗口尺寸及位置
                Width = 900,
                Height = 600,
                Left = (this.Width - 900) / 2,
                Top = (this.Height - 600) / 2,
                Owner = this,
            };

            // （创建或编辑连续浏览器实例）
            setWindow.AddButtonClicked += async (s, args) =>
            {
                await BrowserModelControlsAsync(setWindow._viewModel.Set_BrowMge.BrowserGrop);
            };

            // （创建或编辑单个浏览器实例）
            if (setWindow.ShowDialog() == true)
            {
                await BrowserModelControlsAsync(setWindow._viewModel.Set_BrowMge.BrowserGrop);
            }

            //新增、删除以及编辑实例
            async Task BrowserModelControlsAsync(BrowserGroup model)
            {
                if (mode == SetWindowMode.Add)//新增实例
                {
                    //设置浏览器缓存位置
                    var folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebView2", "Cache", $"{model.Tag}{DateTime.Now.ToString("yyyyMMddHHmmss")}");

                    // 检查文件夹是否存在，如果不存在则创建
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    model.BrowserModel.CachePath = folderPath;

                    //将新增的实例添加到集合中
                    viewModel.BrowserGroups.Add(setWindow._viewModel.Set_BrowMge.BrowserGrop);


                    //创建并初始化新的浏览器实例
                    await CreateWebView2BrowseAsync(true);

                }
                else if (mode == SetWindowMode.Edit)//编辑实例
                {
                    Debug.WriteLine(setWindow._viewModel.Set_BrowMge.BrowserGrop.ButtonsModel.IconSource);
                    //删除实例实现
                    if (string.IsNullOrEmpty(setWindow._viewModel.Set_BrowMge.BrowserGrop.CreateTime))
                    {
                        // 获取当前实例索引并计算上一实例的索引，确保索引有效
                        var currentIndex = viewModel.BrowserGroups.IndexOf(setWindow._viewModel.Set_BrowMge.BrowserGrop);
                        var previousIndex = Math.Max(currentIndex - 1, 0);

                        // 查找与当前实例对应的浏览器并安全地移除它
                        var browser = browserList.FirstOrDefault(x => x.Tag.Equals(setWindow._viewModel.Set_BrowMge.BrowserGrop.Tag));
                        browser?.Dispose();

                        // 从 BrowserGroups 中移除当前实例
                        if (currentIndex >= 0)
                        {
                            viewModel.BrowserGroups.Remove(setWindow._viewModel.Set_BrowMge.BrowserGrop);
                        }

                        // 切换到上一实例（如果存在）
                        if (viewModel.BrowserGroups.Count > 0)
                        {
                            await ToggleWebview2BrowserAsync(viewModel.BrowserGroups[previousIndex].Tag);
                        }
                    }
                    else//只是更新实例信息
                    {
                        //更新当前实例组中的内容
                        var indexModel = viewModel.BrowserGroups.FirstOrDefault(x => x.Tag.Equals(setWindow._viewModel.Set_BrowMge.BrowserGrop.Tag));
                        if (indexModel != null)
                        {
                            viewModel.BrowserGroups[viewModel.BrowserGroups.IndexOf(indexModel)] = setWindow._viewModel.Set_BrowMge.BrowserGrop;
                        }
                    }
                }
                else if (mode == SetWindowMode.Set)
                {
                    if (setWindowViewModel.Set_Appset.Appsetings.NavigationWidthMode == 0)
                    {
                        NavigationButtonsGroupA.Visibility = Visibility.Visible;
                        NavigationButtonsGroupB.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        NavigationButtonsGroupB.Visibility = Visibility.Visible;
                        NavigationButtonsGroupA.Visibility = Visibility.Collapsed;
                    }
                }

                //更新配置文件
                await ConfigManager.SaveBrowserConfigAsync(viewModel.BrowserGroups);
            }
        }
        /// <summary>
        /// 切换Pinbar是否显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsShowPinBarBtnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (viewModel.MyStyle.IsEnablePinNavBar == Visibility.Collapsed)
                {
                    viewModel.MyStyle.IsEnablePinNavBar = Visibility.Visible;
                }
                else
                {
                    viewModel.MyStyle.IsEnablePinNavBar = Visibility.Collapsed;
                }
            });
        }
        /// <summary>
        /// 将对象加入置顶列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PinObjectBtnClick(object sender, RoutedEventArgs e)
        {
            //获取当前被选中用户的信息
            var item = viewModel.PinUserList.FirstOrDefault(x => x.Name == viewModel.InfoBarContent.UserName);

            if (item == null)//未在置顶列表中
            {
                //将对象加入置顶列表
                viewModel.PinUserList.Add(new PinUserModel()
                {
                    ImageSource = viewModel.InfoBarContent.AvatarUrl ?? "pack://application:,,,/Resource/Brand/whatsapp.png",
                    Tag = viewModel.InfoBarContent.Tag,
                    ToolTip = $"{viewModel.InfoBarContent.UserName} - {viewModel.InfoBarContent.Tag}",
                    Name = viewModel.InfoBarContent.UserName
                });
                Debug.WriteLine(viewModel.InfoBarContent.AvatarUrl);

                //标记为待取消置顶
                viewModel.InfoBarContent.IsPin = true;
            }
            else//已在置顶列表中
            {
                viewModel.PinUserList.Remove(item);//移出置顶列表
                viewModel.InfoBarContent.IsPin = false;//标记为待置顶
            }

            //更新置顶用户列表
            await ConfigManager.SavePinUserListAsync(viewModel.PinUserList);
        }
        /// <summary>
        /// whatsApp自动发送动态实现
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StateManagementBtn_Click(object sender, RoutedEventArgs e)
        {
            //打开动态脚本设置窗口
            var sendStatusWindow = new AutoStatus(viewModel.BrowserGroups.Where(x => x.ItemType == 0).ToList())
            {
                Width = 900,
                Height = 600,
                Left = (this.Width - 900) / 2,
                Top = (this.Height - 600) / 2,
            };

            if (sendStatusWindow.ShowDialog() == true)//接到发送任务
            {
                if (!sendStatusWindow.statusTask.SenderList.Any())
                {
                    Logger.Error($"发送动态时任务列表为空！");
                    return;
                }

                var successList = new List<string>();
                var failedList = new List<string>();

                //判断发送的类型
                var statusType = sendStatusWindow.statusTask.IsGraphicMode ? 0 : 1;

                //定义任务列表变量
                var taskList = sendStatusWindow.statusTask.SenderList;
                foreach (var item in taskList)
                {
                    Debug.WriteLine($"实例:{item}");
                }
                //循环执行任务
                foreach (var tag in taskList)
                {
                    Debug.WriteLine($"实例2:{tag}");

                    //获取要发送的浏览器实例
                    var browser = browserList.FirstOrDefault(x => x.Tag.ToString() == tag);

                    //实例未准备好，不执行任务
                    if (browser == null || browser.CoreWebView2 == null)
                    {
                        Logger.Error($"发送纯文字动态前实例未准备好!");
                        continue;
                    }

                    if (statusType == 1)//纯文字动态
                    {
                        //执行发送纯文字动态的JS脚本
                        await browser.CoreWebView2.ExecuteScriptAsync($"sendStatus(1,'{sendStatusWindow.statusTask.TextContent}')");

                        //适当间隔
                        await Task.Delay(500);
                    }
                    else//发送图文动态
                    {
                        var result = await SendGraphicStatusAsync(browser, sendStatusWindow.statusTask.TextContent, sendStatusWindow.statusTask.PhotoSource);

                        if (result) { successList.Add(tag); } else { failedList.Add(tag); }

                    }
                }
                //AppNotice.Show("消息", $"共{successList.Count + failedList.Count}个任务执行完成，成功{successList.Count}个，失败{failedList.Count}个.", AppNotificationLevel.Debug);
            }
        }
        /// <summary>
        /// 发送图文动态实现
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="textContent"></param>
        /// <param name="photoSource"></param>
        /// <returns></returns>
        private async Task<bool> SendGraphicStatusAsync(WebView2 browser, string textContent, string? photoSource)
        {
            var tcs = new TaskCompletionSource<bool>();  // 用来控制任务完成的 TaskCompletionSource

            try
            {
                // 假设 ExecuteScriptWithResultAsync 是一个理想化的函数，实际可能需要调整
                var result = await browser.CoreWebView2.ExecuteScriptAsync($"sendStatus(0,'{textContent}','{photoSource}');");

                if (!string.IsNullOrEmpty(photoSource))
                {
                    if (await Helper.FillFileName(photoSource))
                    {
                        //继续填充文本并发送
                        var uploadResult = await browser.CoreWebView2.ExecuteScriptAsync($"continuePictureUpload('{textContent}');");
                        tcs.SetResult(true);  // 图片上传成功
                    }
                    else
                    {
                        tcs.SetResult(false);  // 文件填充失败
                    }
                }
                else
                {
                    tcs.SetResult(true);  // 没有图片需要上传，只发送文本
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex.Message}");
                tcs.SetResult(false);  // 发生异常，设置结果为失败
            }

            return await tcs.Task;  // 返回操作结果
        }
        /// <summary>
        /// 导航按钮的单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NavigationBtnClick(object sender, RoutedEventArgs e)
        {

            if (sender is RadioButton btn && btn.Tag is string btnTag)
            {
                //清空原本的模型条数据
                viewModel.InfoBarContent = new InfoBarContent();

                await ToggleWebview2BrowserAsync(btnTag);

                //导航条信息模型赋值
                var browserGroup = viewModel.BrowserGroups.FirstOrDefault(y => y.Tag.Equals(btnTag));
                if (browserGroup != null)
                {
                    viewModel.InfoBarContent.Tag = btnTag;
                    viewModel.InfoBarContent.IsAudio = browserGroup.BrowserModel.IsAudio;
                    viewModel.InfoBarContent.IsNotify = browserGroup.BrowserModel.IsNotify;
                    viewModel.InfoBarContent.IsEnable = browserGroup.IsEnable;
                }
            }
        }


        private void ClearMessageCenterBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.NewMessageList.NewMessages.Clear();
            NewMessageBox_V.IsOpen = false;
        }
        /// <summary>
        /// 代付按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpendEasyERPBtnClick(object sender, RoutedEventArgs e)
        {
            // 设置Owner为当前窗口，并初始化窗口
            var walletPaymentWindow = new WalletPayment()
            {
                //设置设置窗口尺寸及位置
                Width = 900,
                Height = 600,
                Left = (this.Width - 900) / 2,
                Top = (this.Height - 600) / 2,
                Owner = this,
            };
            walletPaymentWindow.Show();
        }
        /// <summary>
        /// 启用浏览器实例按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EnableBrowserClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string btnTag)
            {
                var targetBrowserGroup = viewModel.BrowserGroups.FirstOrDefault(x => x.Tag.Equals(btnTag));
                if (targetBrowserGroup != null)
                {
                    targetBrowserGroup.IsEnable = true;
                    viewModel.InfoBarContent.IsEnable = true;
                    IsEnableBrowser(targetBrowserGroup);
                    //更新配置文件
                    await ConfigManager.SaveBrowserConfigAsync(viewModel.BrowserGroups);
                }
            }
        }
        /// <summary>
        /// 上下文菜单点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ContextMenuTapClick(object sender, RoutedEventArgs e)
        {

            if (sender is MenuItem menuItem && menuItem.Name is string menuItemName && menuItem.Tag is string menuItemTag)
            {
                //获取与选项相关联的browser实例
                var browserGroup = viewModel.BrowserGroups.FirstOrDefault(x => x.Tag.Equals(menuItemTag));
                var browser = browserList.FirstOrDefault(x => x.Tag.Equals(menuItemTag));
                if (browserGroup == null || browser == null) return;
                // Debug.WriteLine($"{menuItemTag}:{menuItemName}");
                await ToggleWebview2BrowserAsync(browserGroup.Tag);
                switch (menuItemName)
                {
                    case "Set":
                        await ShowSetWindowAsync(SetWindowMode.Edit, browserGroup);
                        break;
                    case "Cache":
                        await browser.CoreWebView2.Profile.ClearBrowsingDataAsync();
                        AppNotice.Show("提示", $"浏览器（{browser.Tag}）缓存已清除", AppNotificationLevel.Info);
                        break;
                    case "Reload":
                        if (browser.CoreWebView2 != null) browser.Reload();
                        break;
                }
            }
            else if (sender is Button btn && btn.Tag is string btnTag)
            {
                if (btnTag == "Reload")
                {
                    var browser = browserList.FirstOrDefault(x => x.Tag.Equals(viewModel.InfoBarContent.Tag));
                    if (browser == null || browser.CoreWebView2 == null) return;
                    browser.Reload();
                }
            }
            else
            {
                var toggleButton = sender as ToggleButton;
                if (toggleButton != null && toggleButton.Name is string toggleButtonName && toggleButton.Tag is string toggleButtonTag)
                {
                    var browserGroup = viewModel.BrowserGroups.FirstOrDefault(x => x.Tag.Equals(toggleButtonTag));
                    var browser = browserList.FirstOrDefault(x => x.Tag.Equals(toggleButtonTag));
                    if (browserGroup == null || browser == null) return;
                    switch (toggleButtonName)
                    {
                        case "Enable":
                            IsEnableBrowser(browserGroup);
                            await ToggleWebview2BrowserAsync(browserGroup.Tag);
                            break;
                        case "Audio":
                            if (browser.CoreWebView2 != null) await browser.CoreWebView2.ExecuteScriptAsync(browserGroup.BrowserModel.IsAudio ? "unmuteAudio();" : "muteAudio();");
                            break;
                        case "Notify":
                            //if (browser.CoreWebView2 != null) await browser.CoreWebView2.ExecuteScriptAsync(browserGroup.BrowserModel.IsAudio ? "unmuteAudio();" : "muteAudio();");
                            break;
                    }
                    //更新配置文件
                    await ConfigManager.SaveBrowserConfigAsync(viewModel.BrowserGroups);
                }

            }
        }
        /// <summary>
        /// 控制上下文菜单出现的位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 获取触发右键点击的 RadioButton

            if (sender is RadioButton btn && btn.ContextMenu != null)
            {
                // 禁用默认的右键点击行为
                e.Handled = true;

                // 显示 ContextMenu
                btn.ContextMenu.PlacementTarget = btn;
                btn.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.RelativePoint; // 固定相对位置显示
                btn.ContextMenu.IsOpen = true;
                // 固定的偏移量，例如，菜单将显示在按钮的右侧下方
                btn.ContextMenu.HorizontalOffset = 35; // 控制水平偏移
                btn.ContextMenu.VerticalOffset = 0;   // 控制垂直偏移
            }
        }

        private void WebView2Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            viewModel.Webview2BrowserWidth = WebView2Canvas.ActualWidth;
            viewModel.Webview2BrowserHeight = WebView2Canvas.ActualHeight;
            if (viewModel.Webview2BrowserWidth > 1429)
            {
                viewModel.IsShowExpansionPanel = Visibility.Visible;
            }
            else
            {
                viewModel.IsShowExpansionPanel = Visibility.Collapsed;
            }
        }
        /// <summary>
        /// 全局快捷键响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {

            var setModel = await ConfigManager.LoadAppConfigAsync();
            var keys = new List<(int, Key)> { (0, Key.F1), (1, Key.F2), (2, Key.F3), (3, Key.F4), (4, Key.F5), (5, Key.F6), (6, Key.F7), (8, Key.F9), (9, Key.F10), (10, Key.F11), (11, Key.F12), };

            //有道翻译快捷键
            if (e.Key == keys.FirstOrDefault(x => x.Item1 == setModel.TranslateKey).Item2)
            {
                //标记为翻译进行时
                ignoreCtrlDoubleClick = true;

                // 先模拟Ctrl+A进行全选
                KeyboardSimulator.SelectAll();
                // 稍等片刻
                await Task.Delay(100);
                //Ctrl + C进行复制
                KeyboardSimulator.Copy();
                // 稍等片刻
                await Task.Delay(100);
                // 尝试获取剪贴板内容
                string clipboardText = ClipboardAPI.GetText() ?? string.Empty;
                //剪贴板内容为空，停止向下执行
                if (string.IsNullOrEmpty(clipboardText)) return;

                // 调用翻译服务进行翻译
                try
                {
                    var translatedText = YoudaoTranslator.Execute(clipboardText);
                    if (!string.IsNullOrEmpty(translatedText))
                    {
                        // 置剪贴板内容为翻译后的文本
                        ClipboardAPI.SetText(translatedText);

                        // 稍等片刻再进行粘贴操作 等待确保剪贴板已更新
                        await Task.Delay(100);

                        // 模拟Ctrl+V进行粘贴
                        KeyboardSimulator.Paste();
                    }
                }
                catch (Exception ex)
                {
                    // 处理翻译过程中可能发生的异常
                    Debug.WriteLine("Translation failed: " + ex.Message);
                    Logger.Error("Translation failed: " + ex.Message);
                }
                finally
                {
                    ignoreCtrlDoubleClick = false;
                }
            }

            //监测双击ctrl键事件
            if ((e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl) && !ignoreCtrlDoubleClick)
            {
                if (ctrlPressedOnce)
                {
                    TimeSpan span = DateTime.Now - lastCtrlPress;
                    if (span.TotalMilliseconds < DoublePressMaxMilliSeconds)
                    {

                        if (!string.IsNullOrEmpty(lastBrowserTag))
                        {
                            Debug.WriteLine(lastBrowserTag);
                            await ToggleWebview2BrowserAsync(lastBrowserTag);
                        }

                        ctrlPressedOnce = false;  // 重置状态
                    }
                    else
                    {
                        // 重置第一次按下的时间
                        lastCtrlPress = DateTime.Now;
                    }
                }
                else
                {
                    ctrlPressedOnce = true;
                    lastCtrlPress = DateTime.Now;
                }
            }
        }
        /// <summary>
        /// 置顶用户点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PinBtnSkipBtnClick(object sender, MouseButtonEventArgs e)
        {

            if (!(sender is Border border && border.Tag is string borderTag)) return;

            var model = viewModel.PinUserList.FirstOrDefault(x => x.ToolTip == borderTag);

            if (model != null)
            {
                //设置目标浏览器为可见，并将当前可见浏览器设置为不可见
                await ToggleWebview2BrowserAsync(model.Tag);

                // 注入JavaScript代码来点击对应的用户
                try
                {
                    var browser = browserList.FirstOrDefault(x => x.Tag.ToString() == model.Tag);
                    if (browser != null)
                    {
                        //标记为已选中
                        viewModel.InfoBarContent.IsPin = true;

                        //点击选中用户
                        await browser.CoreWebView2.ExecuteScriptAsync($"SubmitClickSender('{model.Name}');");

                        //获取用户信息
                        await browser.CoreWebView2.ExecuteScriptAsync($"GetSelectorObject(true);");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"PinBtnSkipBtnClick方法执行出错，{ex.Message}");
                }
            }
        }
        /// <summary>
        /// 移除置顶用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemovePinUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string btnTag)
            {
                var model = viewModel.PinUserList.FirstOrDefault(x => x.ToolTip == btnTag);
                viewModel.PinUserList.Remove(model);
            }
        }
    }
}