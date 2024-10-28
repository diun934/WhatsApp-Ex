using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Wpf;

namespace all_on_whatsapp
{
    //设置窗口类型模型
    public enum SetWindowMode
    {
        Add, Edit, Extensions, Set
    }
    public class StatusModel
    {
        public string Text { get; set; } = string.Empty;
        public string PhotoSource { get; set; } = string.Empty;
    }

    //动态任务模型
    public class StatusTask
    {
        public string? PhotoSource { get; set; } = string.Empty;
        public string TextContent { get; set; } = string.Empty;
        public bool IsGraphicMode { get; set; } = false;
        public List<string> SenderList { get; set; } = new List<string>();

    }

    //拓展插件（js脚本）模型
    public class Extension
    {
        public string Name { get; set; } = string.Empty;
        public string IconResource { get; set; } = "pack://application:,,,/Resource/Extension.png";
        public string NickName { get; set; } = string.Empty;
        public string ResourceName { get; set; } = string.Empty;
        public bool IsEnable { get; set; } = true;
        public string Description { get; set; } = "没有更多描述";

    }

    //项目（不同网站）类型模型
    public class ItemType
    {
        public int Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;
    }

    //PinUser（置顶对象）按钮模型
    public class PinUserModel
    {
        public string Tag { get; set; } = string.Empty;
        public string ImageSource { get; set; } = string.Empty; // 图片资源路径
        public string ToolTip { get; set; } = string.Empty; // 提示
        public string Name { get; set; } = string.Empty; // 用户名
    }

    //系统通知枚举类
    public enum AppNotificationLevel
    {
        Info, Error, Warning, Debug
    }

    //系统通知模型
    public class AppNotificationViewModel
    {
        private AppNotificationLevel _level;

        public AppNotificationLevel Level
        {
            get => _level;
            set
            {
                _level = value;
                UpdateColor();
            }
        }

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Time { get; set; } = DateTime.Now.ToString("yy/M/dd h:mm tt", System.Globalization.CultureInfo.InvariantCulture).ToLower();
        public string Sender { get; set; } = string.Empty;
        public Brush Color { get; private set; } = Brushes.DodgerBlue; // 默认颜色

        private void UpdateColor()
        {
            switch (Level)
            {
                case AppNotificationLevel.Info:
                    Color = Brushes.DodgerBlue;
                    break;
                case AppNotificationLevel.Warning:
                    Color = Brushes.Orange;
                    break;
                case AppNotificationLevel.Error:
                    Color = Brushes.Red;
                    break;
                default:
                    Color = Brushes.Gray;
                    break;
            }
        }
    }

    //网页通知模型
    public class BrowserNotificationData
    {
        public string Title { get; set; } = string.Empty;
        public OptionsModel Options { get; set; } = new OptionsModel();
        public class OptionsModel
        {
            public string Tag { get; set; } = string.Empty;
            public bool Renotify { get; set; }
            public string Dir { get; set; } = string.Empty;
            public string Lang { get; set; } = string.Empty;
            public bool Silent { get; set; }
            public string Body { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
        }
    }

    //信息条模型
    public class InfoBarContent : INotifyPropertyChanged
    {
        private bool isPin = false;
        private bool isEnable = true;
        private bool isAudio = true;
        private bool isNotify = true;
        private string _tag = " - ";
        private string _userName = " - ";
        private string _systemName = " - ";
        private int _numberOfOrders;
        private double _amountOfOrders;
        private string _level = " - ";
        private string _remarks = " - ";
        private string _avatarUrl = "pack://application:,,,/Resource/Brand/whatsapp.png";
        private Visibility isObjectOnly = Visibility.Collapsed;
        private Visibility disableIndication = Visibility.Collapsed;
        public Visibility IsObjectOnly
        {
            get => isObjectOnly;
            set
            {
                if (isObjectOnly != value)
                {
                    isObjectOnly = value;
                    OnPropertyChanged(nameof(IsObjectOnly));
                }
            }
        }
        public Visibility DisableIndication
        {
            get => disableIndication;
            set
            {
                if (disableIndication != value)
                {
                    disableIndication = value;
                    OnPropertyChanged(nameof(DisableIndication));
                }
            }
        }

        public bool IsPin
        {
            get => isPin;
            set
            {
                if (isPin != value)
                {
                    isPin = value;
                    OnPropertyChanged(nameof(IsPin));
                }
            }
        }
        public bool IsAudio
        {
            get => isAudio;
            set
            {
                if (isAudio != value)
                {
                    isAudio = value;
                    OnPropertyChanged(nameof(IsAudio));
                }
            }
        }
        public bool IsNotify
        {
            get => isNotify;
            set
            {
                if (isNotify != value)
                {
                    isNotify = value;
                    OnPropertyChanged(nameof(IsNotify));
                }
            }
        }
        public bool IsEnable
        {
            get => isEnable;
            set
            {
                if (isEnable != value)
                {
                    isEnable = value;
                    OnPropertyChanged(nameof(IsEnable));
                }
            }
        }
        public string Tag
        {
            get => _tag;
            set
            {
                if (_tag != value)
                {
                    _tag = value;
                    OnPropertyChanged(nameof(Tag));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string AvatarUrl
        {
            get => _avatarUrl;
            set
            {
                if (_avatarUrl != value)
                {
                    _avatarUrl = value;
                    OnPropertyChanged(nameof(AvatarUrl));
                }
            }
        }
        public string SystemName
        {
            get => _systemName;
            set
            {
                if (_systemName != value)
                {
                    _systemName = value;
                    OnPropertyChanged(nameof(SystemName));
                }
            }
        }
        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }

        public int NumberOfOrders
        {
            get => _numberOfOrders;
            set
            {
                if (_numberOfOrders != value)
                {
                    _numberOfOrders = value;
                    OnPropertyChanged(nameof(NumberOfOrders));
                }
            }
        }

        public double AmountOfOrders
        {
            get => _amountOfOrders;
            set
            {
                if (_amountOfOrders != value)
                {
                    _amountOfOrders = value;
                    OnPropertyChanged(nameof(AmountOfOrders));
                }
            }
        }

        public string Level
        {
            get => _level;
            set
            {
                if (_level != value)
                {
                    _level = value;
                    OnPropertyChanged(nameof(Level));
                }
            }
        }

        public string Remarks
        {
            get => _remarks;
            set
            {
                if (_remarks != value)
                {
                    _remarks = value;
                    OnPropertyChanged(nameof(Remarks));
                }
            }
        }
    }

    //导航按钮模型
    public class NavigationBtnViewModel : INotifyPropertyChanged
    {
        private int iconType = 1;
        private bool isUnread = true;
        private Visibility _hasUnreadMessages = Visibility.Collapsed;
        private int _unreadMessagesCount;
        //按钮图标
        private string iconSource = "pack://application:,,,/Resource/Brand/whatsapp.png";
        public string IconSource
        {
            get => iconSource;
            set
            {
                if (iconSource != value)
                {
                    iconSource = value;
                    OnPropertyChanged(nameof(IconSource));
                }
            }
        }

        public int IconType
        {
            get => iconType;
            set
            {
                if (iconType != value)
                {
                    iconType = value;
                    OnPropertyChanged(nameof(IconType));
                }
            }
        }

        public bool IsUnread
        {
            get => isUnread;
            set
            {
                if (isUnread != value)
                {
                    isUnread = value;
                    OnPropertyChanged(nameof(IsUnread));
                }
            }
        }


        public Visibility HasUnreadMessages
        {
            get => _hasUnreadMessages;
            private set
            {
                if (_hasUnreadMessages != value)
                {
                    _hasUnreadMessages = value;
                    OnPropertyChanged(nameof(HasUnreadMessages));
                }
            }
        }

        public int UnreadMessagesCount
        {
            get => _unreadMessagesCount;
            set
            {
                if (_unreadMessagesCount != value)
                {
                    _unreadMessagesCount = value;
                    OnPropertyChanged(nameof(UnreadMessagesCount));

                    // 根据消息数量控制 HasUnreadMessages
                    HasUnreadMessages = _unreadMessagesCount > 0 ? Visibility.Visible : Visibility.Collapsed;

                    // 通知 UI 显示数量
                    OnPropertyChanged(nameof(DisplayUnreadMessagesCount));
                }
            }
        }

        // 用于显示的未读消息数量，超过99显示 "99+"
        public string DisplayUnreadMessagesCount
        {
            get
            {
                return _unreadMessagesCount > 99 ? "!" : _unreadMessagesCount.ToString();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    //浏览器实例模型
    public class Webview2BrowserViewModel : INotifyPropertyChanged
    {
        private bool isLoaded = true;
        private double zoomFactor = 1;
        private bool isEnableProxy = false;
        private bool isAudio = true;
        private bool isNotify = true;
        private string cachePath = string.Empty;
        private string url = "https://web.whatsapp.com";
        private string ipAddress = "0.0.0.0";
        private string port = "1000";
        private string user = string.Empty;
        private string password = string.Empty;
        public bool IsLoaded
        {
            get => isLoaded;
            set
            {
                if (isLoaded != value)
                {
                    isLoaded = value;
                    OnPropertyChanged(nameof(IsLoaded));
                }
            }
        }


        public bool IsNotify
        {
            get => isNotify;
            set
            {
                if (isNotify != value)
                {
                    isNotify = value;
                    OnPropertyChanged(nameof(IsNotify));
                }
            }
        }

        public bool IsAudio
        {
            get => isAudio;
            set
            {
                if (isAudio != value)
                {
                    isAudio = value;
                    OnPropertyChanged(nameof(IsAudio));
                }
            }
        }
        public bool IsEnableProxy
        {
            get => isEnableProxy;
            set
            {
                if (isEnableProxy != value)
                {
                    isEnableProxy = value;
                    OnPropertyChanged(nameof(IsEnableProxy));
                }
            }
        }

        public string Password
        {
            get => password;
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }
        public double ZoomFactor
        {
            get => zoomFactor;
            set
            {
                if (zoomFactor != value)
                {
                    zoomFactor = value;
                    OnPropertyChanged(nameof(ZoomFactor));
                }
            }
        }
        public string User
        {
            get => user;
            set
            {
                if (user != value)
                {
                    user = value;
                    OnPropertyChanged(nameof(User));
                }
            }
        }
        public string Port
        {
            get => port;
            set
            {
                if (port != value)
                {
                    port = value;
                    OnPropertyChanged(nameof(Port));
                }
            }
        }
        public string IpAddress
        {
            get => ipAddress;
            set
            {
                if (ipAddress != value)
                {
                    ipAddress = value;
                    OnPropertyChanged(nameof(IpAddress));
                }
            }
        }

        public string Url
        {
            get => url;
            set
            {
                if (url != value)
                {
                    url = value;
                    OnPropertyChanged(nameof(Url));
                }
            }
        }
        public string CachePath
        {
            get => cachePath;
            set
            {
                if (cachePath != value)
                {
                    cachePath = value;
                    OnPropertyChanged(nameof(CachePath));
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    //浏览器参数模型
    public class BrowserGroup : INotifyPropertyChanged
    {
        //浏览器相关模型数据
        private NavigationBtnViewModel navigationBtnViewModel = new NavigationBtnViewModel();
        //导航按钮相关模型数据
        private Webview2BrowserViewModel webview2BrowserViewModel = new Webview2BrowserViewModel();

        //以下为公用参数
        private bool isChecked = false;
        private string tag = "WhatsApp";
        private bool isEnable = true;
        private int itemType = 0;
        private string createTime = DateTime.Now.ToString("yyyyMMddHHmmss");
        public Webview2BrowserViewModel BrowserModel
        {
            get => webview2BrowserViewModel;
            set
            {
                if (webview2BrowserViewModel != value)
                {
                    webview2BrowserViewModel = value;
                    OnPropertyChanged(nameof(BrowserModel));
                }
            }
        }

        public NavigationBtnViewModel ButtonsModel
        {
            get => navigationBtnViewModel;
            set
            {
                if (navigationBtnViewModel != value)
                {
                    navigationBtnViewModel = value;
                    OnPropertyChanged(nameof(ButtonsModel));
                }
            }
        }
        public bool IsEnable
        {
            get => isEnable;
            set
            {
                if (isEnable != value)
                {
                    isEnable = value;
                    OnPropertyChanged(nameof(IsEnable));
                }
            }
        }
        public string Tag
        {
            get => tag;
            set
            {
                if (tag != value)
                {
                    tag = value;
                    OnPropertyChanged(nameof(Tag));
                }
            }
        }
        public int ItemType
        {
            get => itemType;
            set
            {
                if (itemType != value)
                {
                    itemType = value;
                    OnPropertyChanged(nameof(ItemType));
                }
            }
        }
        public string CreateTime
        {
            get => createTime;
            set
            {
                if (createTime != value)
                {
                    createTime = value;
                    OnPropertyChanged(nameof(CreateTime));
                }
            }
        }
        private bool btnIsChecked;
        public bool BtnIsChecked
        {
            get => btnIsChecked;
            set
            {
                if (btnIsChecked != value)
                {
                    btnIsChecked = value;
                    OnPropertyChanged(nameof(BtnIsChecked));  // Notify that BtnIsChecked has changed.
                }
            }
        }
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    BtnIsChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //窗口信息模型
    public class WindowSettings
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
    }

    //新消息模型
    public class NewMessage
    {
        public string Avatar { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty;
        public string MessageContent { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string UnreadMessages { get; set; } = string.Empty;
        public WebView2 Browser { get; set; } = new WebView2();
    }
    public class NewMessageList : INotifyPropertyChanged
    {
        private ObservableCollection<NewMessage> _newMessages = new ObservableCollection<NewMessage>();

        public ObservableCollection<NewMessage> NewMessages
        {
            get { return _newMessages; }
            set
            {
                if (_newMessages != null)
                {
                    _newMessages.CollectionChanged -= OnNewMessagesCollectionChanged!;
                }

                _newMessages = value;
                if (_newMessages != null)
                {
                    _newMessages.CollectionChanged += OnNewMessagesCollectionChanged!;
                }

                OnPropertyChanged(nameof(NewMessages));
                UpdateNewMessagesCountDisplay();
            }
        }

        private string _newMessagesCountDisplay = string.Empty;
        public string NewMessagesCountDisplay
        {
            get { return _newMessagesCountDisplay; }
            private set
            {
                _newMessagesCountDisplay = value;
                OnPropertyChanged(nameof(NewMessagesCountDisplay));
                OnPropertyChanged(nameof(NewMessagesCountVisibility));
            }
        }

        public Visibility NewMessagesCountVisibility
        {
            get { return string.IsNullOrEmpty(_newMessagesCountDisplay) || _newMessagesCountDisplay == "0" ? Visibility.Collapsed : Visibility.Visible; }
        }

        public NewMessageList()
        {
            _newMessages.CollectionChanged += OnNewMessagesCollectionChanged!;
        }

        private void OnNewMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateNewMessagesCountDisplay();
        }

        private void UpdateNewMessagesCountDisplay()
        {
            int newMessagesCount = _newMessages.Count;
            NewMessagesCountDisplay = newMessagesCount > 99 ? "99+" : newMessagesCount.ToString();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //程序主题模型
    public class MyStyle : INotifyPropertyChanged
    {
        private string name = "Gray";
        private int mainNavBarPosition = 1;
        private double mainNavBarWidth = 50;
        private Brush mainNavBarColor = new SolidColorBrush(Color.FromRgb(128, 128, 128));
        private bool mainNavBarIconColor = true;
        private Brush instantiationIconColorBo = new SolidColorBrush(Color.FromRgb(128, 128, 128));
        private Brush instantiationIconColorBg = new SolidColorBrush(Colors.Transparent);
        private Brush instantiationIconColorFg = new SolidColorBrush(Colors.White);
        private Brush backupNavBarColor = new SolidColorBrush(Color.FromRgb(240, 242, 245));
        private double backupNavBarWidth = 50;
        private bool backupNavBarIconColor = true;
        private Visibility isEnableBackupNavBar = Visibility.Visible;
        private Visibility isEnableNote = Visibility.Visible;
        private Visibility isEnableAddBtn = Visibility.Visible;
        private Visibility isEnableNotificationBtn = Visibility.Visible;
        private Visibility isEnableSetBtn = Visibility.Visible;
        private Visibility isEnableExpansionBtn = Visibility.Collapsed;
        private Visibility isEnableContactBtn = Visibility.Visible;
        private Visibility isEnablePinNavBar = Visibility.Collapsed;
        private bool isEnableToolTip = true;

        public bool IsEnableToolTip
        {
            get => isEnableToolTip;
            set => SetProperty(ref isEnableToolTip, value);
        }
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public int MainNavBarPosition
        {
            get => mainNavBarPosition;
            set => SetProperty(ref mainNavBarPosition, value);
        }

        public double MainNavBarWidth
        {
            get => mainNavBarWidth;
            set => SetProperty(ref mainNavBarWidth, value);
        }

        public Brush MainNavBarColor
        {
            get => mainNavBarColor;
            set => SetProperty(ref mainNavBarColor, value);
        }

        public bool MainNavBarIconColor
        {
            get => mainNavBarIconColor;
            set => SetProperty(ref mainNavBarIconColor, value);
        }

        public Brush InstantiationIconColorBo
        {
            get => instantiationIconColorBo;
            set => SetProperty(ref instantiationIconColorBo, value);
        }

        public Brush InstantiationIconColorBg
        {
            get => instantiationIconColorBg;
            set => SetProperty(ref instantiationIconColorBg, value);
        }

        public Brush InstantiationIconColorFg
        {
            get => instantiationIconColorFg;
            set => SetProperty(ref instantiationIconColorFg, value);
        }

        public Brush BackupNavBarColor
        {
            get => backupNavBarColor;
            set => SetProperty(ref backupNavBarColor, value);
        }

        public double BackupNavBarWidth
        {
            get => backupNavBarWidth;
            set => SetProperty(ref backupNavBarWidth, value);
        }

        public bool BackupNavBarIconColor
        {
            get => backupNavBarIconColor;
            set => SetProperty(ref backupNavBarIconColor, value);
        }

        public Visibility IsEnableBackupNavBar
        {
            get => isEnableBackupNavBar;
            set => SetProperty(ref isEnableBackupNavBar, value);
        }
        public Visibility IsEnablePinNavBar
        {
            get => isEnablePinNavBar;
            set => SetProperty(ref isEnablePinNavBar, value);
        }
        public Visibility IsEnableNote
        {
            get => isEnableNote;
            set => SetProperty(ref isEnableBackupNavBar, value);
        }

        public Visibility IsEnableAddBtn
        {
            get => isEnableAddBtn;
            set => SetProperty(ref isEnableAddBtn, value);
        }

        public Visibility IsEnableNotificationBtn
        {
            get => isEnableNotificationBtn;
            set => SetProperty(ref isEnableNotificationBtn, value);
        }

        public Visibility IsEnableSetBtn
        {
            get => isEnableSetBtn;
            set => SetProperty(ref isEnableSetBtn, value);
        }

        public Visibility IsEnableExpansionBtn
        {
            get => isEnableExpansionBtn;
            set => SetProperty(ref isEnableExpansionBtn, value);
        }

        public Visibility IsEnableContactBtn
        {
            get => isEnableContactBtn;
            set => SetProperty(ref isEnableContactBtn, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null!)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    //App全局设置项
    public class Appsetings : INotifyPropertyChanged
    {
        private int language = 0;//语言
        private int style = 0;//主题
        private int navBarPosition = 0;//导航条位置
        private bool isDpi = true;//DPI支持
        private bool isGpu = false;//硬件加速
        private bool isTop = false;//窗口置顶
        private bool isExpansionIcon = false;//显示拓展图标
        private bool isAddIcon = true;//显示新增图标
        private bool isPinIcon = true;//显示置顶列表图标
        private bool isUpdate = true;//自动升级
        private bool isNumberAnalysis = false;//启用号码分析
        private bool isShowTaskBar = false;//显示在任务栏
        private bool isShowTray = false;//显示在托盘
        private bool isToastNotice = false;//吐司通知
        private string user = string.Empty;//用户名
        private string password = string.Empty;//密码
        private string key = string.Empty;//密钥
        private int navigationWidthMode { get; set; } = 0;
        public int NavigationWidthMode
        {
            get => navigationWidthMode;
            set
            {
                if (navigationWidthMode != value)
                {
                    navigationWidthMode = value;
                    OnPropertyChanged(nameof(NavigationWidthMode));
                }
            }
        }
        private int translateKey { get; set; } = 1;
        public int TranslateKey
        {
            get => translateKey;
            set
            {
                if (translateKey != value)
                {
                    translateKey = value;
                    OnPropertyChanged(nameof(TranslateKey));
                }
            }
        }
        // 属性实现
        public int Language
        {
            get => language;
            set
            {
                if (language != value)
                {
                    language = value;
                    OnPropertyChanged(nameof(Language));
                }
            }
        }

        public int Style
        {
            get => style;
            set
            {
                if (style != value)
                {
                    style = value;
                    OnPropertyChanged(nameof(Style));
                }
            }
        }

        public int NavBarPosition
        {
            get => navBarPosition;
            set
            {
                if (navBarPosition != value)
                {
                    navBarPosition = value;
                    OnPropertyChanged(nameof(NavBarPosition));
                }
            }
        }

        public bool IsDpi
        {
            get => isDpi;
            set
            {
                if (isDpi != value)
                {
                    isDpi = value;
                    OnPropertyChanged(nameof(IsDpi));
                }
            }
        }
        public bool IsToastNotice
        {
            get => isToastNotice;
            set
            {
                if (isToastNotice != value)
                {
                    isToastNotice = value;
                    OnPropertyChanged(nameof(IsToastNotice));
                }
            }
        }

        public bool IsGpu
        {
            get => isGpu;
            set
            {
                if (isGpu != value)
                {
                    isGpu = value;
                    OnPropertyChanged(nameof(IsGpu));
                }
            }
        }

        public bool IsTop
        {
            get => isTop;
            set
            {
                if (isTop != value)
                {
                    isTop = value;
                    OnPropertyChanged(nameof(IsTop));
                }
            }
        }

        public bool IsExpansionIcon
        {
            get => isExpansionIcon;
            set
            {
                if (isExpansionIcon != value)
                {
                    isExpansionIcon = value;
                    OnPropertyChanged(nameof(IsExpansionIcon));
                }
            }
        }

        public bool IsAddIcon
        {
            get => isAddIcon;
            set
            {
                if (isAddIcon != value)
                {
                    isAddIcon = value;
                    OnPropertyChanged(nameof(IsAddIcon));
                }
            }
        }

        public bool IsPinIcon
        {
            get => isPinIcon;
            set
            {
                if (isPinIcon != value)
                {
                    isPinIcon = value;
                    OnPropertyChanged(nameof(IsPinIcon));
                }
            }
        }

        public bool IsUpdate
        {
            get => isUpdate;
            set
            {
                if (isUpdate != value)
                {
                    isUpdate = value;
                    OnPropertyChanged(nameof(IsUpdate));
                }
            }
        }

        public bool IsNumberAnalysis
        {
            get => isNumberAnalysis;
            set
            {
                if (isNumberAnalysis != value)
                {
                    isNumberAnalysis = value;
                    OnPropertyChanged(nameof(IsNumberAnalysis));
                }
            }
        }

        public bool IsShowTaskBar
        {
            get => isShowTaskBar;
            set
            {
                if (isShowTaskBar != value)
                {
                    isShowTaskBar = value;
                    OnPropertyChanged(nameof(IsShowTaskBar));
                }
            }
        }

        public bool IsShowTray
        {
            get => isShowTray;
            set
            {
                if (isShowTray != value)
                {
                    isShowTray = value;
                    OnPropertyChanged(nameof(IsShowTray));
                }
            }
        }

        public string User
        {
            get => user;
            set
            {
                if (user != value)
                {
                    user = value;
                    OnPropertyChanged(nameof(User));
                }
            }
        }

        public string Password
        {
            get => password;
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public string Key
        {
            get => key;
            set
            {
                if (key != value)
                {
                    key = value;
                    OnPropertyChanged(nameof(Key));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
