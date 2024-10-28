using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace all_on_whatsapp.ViewModel
{
    //实例管理界面专用模型
    public class Set_BrowserManageViewModel : INotifyPropertyChanged
    {
        //可选项目类型
        private ObservableCollection<ItemType> itemTypes = new ObservableCollection<ItemType>
        {
            new ItemType{ Type=0,Name="WhatsApp",Url="https://web.whatsapp.com",IconName="whatsapp"},
            new ItemType{ Type=1,Name="FaceBook",Url="https://www.facebook.com/login",IconName="facebook"},
            new ItemType{ Type=2,Name="Messenger",Url="https://www.messenger.com/",IconName="messenger"},
            new ItemType{ Type=3,Name="Google Mail",Url="https://accounts.google.com/servicelogin?service=mail",IconName="googleMail"},
            new ItemType{ Type=4,Name="a163 Mail",Url="https://email.163.com/",IconName="a163Mail"},
            new ItemType{ Type=5,Name="OutLook Mail",Url="https://outlook.live.com/owa/",IconName="outlook"},
            new ItemType{ Type=6,Name="WeChat",Url="https://web1.wechat.com/",IconName="wechat"},
            new ItemType{ Type=7,Name="Line",Url="https://access.line.me/oauth2/v2.1/authorize?response_type=code&client_id=1563287493&redirect_uri=http://woodbox.main.jp/line_login/&state=woodbox&scope=profile",IconName="line"},
            new ItemType{ Type=8,Name="Telegram",Url="https://web.telegram.org/a/",IconName="teregram"},
        };
        public ObservableCollection<ItemType> ItemTypes
        {
            get => itemTypes;
            set
            {
                if (itemTypes != value)
                {
                    itemTypes = value;
                    OnPropertyChanged();
                }
            }
        }

        //实例参数
        private BrowserGroup browserGrop = new BrowserGroup();
        public BrowserGroup BrowserGrop
        {
            get => browserGrop;
            set
            {
                if (browserGrop != value)
                {
                    browserGrop = value;
                    OnPropertyChanged(nameof(BrowserGrop));
                }
            }
        }

        private ObservableCollection<BrowserGroup> browserGroups = new ObservableCollection<BrowserGroup>();
        public ObservableCollection<BrowserGroup> BrowserGroups
        {
            get => browserGroups;
            set
            {
                if (browserGroups != value)
                {
                    browserGroups = value;
                    OnPropertyChanged(nameof(BrowserGroups));
                }
            }
        }

        private Visibility isShowDeltelBtn { get; set; } = Visibility.Visible;
        public Visibility IsShowDeltelBtn
        {
            get => isShowDeltelBtn;
            set
            {
                if (isShowDeltelBtn != value)
                {
                    isShowDeltelBtn = value;
                    OnPropertyChanged(nameof(IsShowDeltelBtn));
                }
            }
        }
        private Visibility isShowUpdate { get; set; } = Visibility.Visible;
        public Visibility IsShowUpdate
        {
            get => isShowUpdate;
            set
            {
                if (isShowUpdate != value)
                {
                    isShowUpdate = value;
                    OnPropertyChanged(nameof(IsShowUpdate));
                }
            }
        }
        private string btnText1 { get; set; } = "新增";
        public string BtnText1
        {
            get => btnText1;
            set
            {
                if (btnText1 != value)
                {
                    btnText1 = value;
                    OnPropertyChanged(nameof(BtnText1));
                }
            }
        }
        private string btnText2 { get; set; } = "新增并退出";
        public string BtnText2
        {
            get => btnText2;
            set
            {
                if (btnText2 != value)
                {
                    btnText2 = value;
                    OnPropertyChanged(nameof(BtnText2));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //App设置界面模型
    public class Set_AppSettingViewModel : INotifyPropertyChanged
    {
        //可选语言列表
        public ObservableCollection<string> LanguageList { get; set; } = new ObservableCollection<string>
            {
                "简体中文(CN)"
            };

        //可选主题列表
        public ObservableCollection<string> StyleList { get; set; } = new ObservableCollection<string>
            {
                "高雅灰(Gray)"
            };

        //可选导航条位置
        public ObservableCollection<string> NavBarPositionList { get; set; } = new ObservableCollection<string>
            {
                "左(Left)"
            };

        //设置界面模型
        private Appsetings appsetings = new Appsetings();
        public Appsetings Appsetings
        {
            get => appsetings;
            set
            {
                if (appsetings != value)
                {
                    appsetings = value;
                    OnPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //插件管理界面模型
    public class Set_ExtensionsManageViewModel : INotifyPropertyChanged
    {
        //插件界面列表
        private ObservableCollection<Extension> extensions { get; set; } = new ObservableCollection<Extension>()
        {
            new Extension{Name="有道翻译",IconResource="/Resource/Extensions/youdao.png",Description="有道翻译插件支持多语言即时翻译，易于集成，优化网站和应用的用户体验。",NickName="有道翻译" },
            new Extension{Name="屏蔽EXE文件",IconResource="/Resource/Extensions/Defense.png",Description="为了防止误下载exe病毒文件，使用该脚本可以屏蔽exe文件的消息。",NickName="病毒防护" },
            new Extension{Name="WhatsApp自动点赞",IconResource="/Resource/Extensions/Like.png",Description="使用该脚本，将在App页面增加一个开关，用以开启/关闭WhatsApp自动点赞。",NickName="有道翻译" },
            new Extension{Name="ChatGpt",IconResource="/Resource/Extensions/ChatGPT.png",Description="截至2024年，全球最知名的AI工具，懂得都懂。",NickName="ChatGpt" },
            new Extension{Name="自动发送动态",IconResource="/Resource/Extensions/Status.png",Description="自动发送图文动态或纯文字动态。",NickName="自动发送动态" },
            new Extension{Name="屏蔽获取Windows版...",IconResource="/Resource/Extensions/DisableUpgrade.png",Description="屏蔽WhatsApp会话列表下方弹出的获取Windows版WhatsApp。",NickName="屏蔽Windows版本" },
            new Extension{Name="客户大数据分析",IconResource="/Resource/Extensions/BigData.png",Description="开启后，可以看到同行对该客户的标记情况。",NickName="客户大数据分析" },
            new Extension{Name="iTunes/Apple礼品卡查询",IconResource="/Resource/Extensions/Apple.png",Description="通过苹果官网查询苹果卡余额，暂时只能查询Us（美国）的卡片。",NickName="苹果查卡" },
            new Extension{Name="奈拉代付",IconResource="/Resource/Extensions/Naira.png",Description="在线代付Api，代付Naira。",NickName="奈拉代付" },
            new Extension{Name="USDT代付",IconResource="/Resource/Extensions/USDT.png",Description="在线代付Api，代付USDT。",NickName="USDT代付" },
            new Extension{Name="BTC代付",IconResource="/Resource/Extensions/BTC.png",Description="在线代付BTC，代付USDT。",NickName="BTC代付" },
        };
        public ObservableCollection<Extension> Extensions
        {
            get => extensions;
            set
            {
                if (extensions != value)
                {
                    extensions = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class SetWindowViewModel : INotifyPropertyChanged
    {
        //拓展插件界面
        private Set_ExtensionsManageViewModel set_Ex { get; set; } = new Set_ExtensionsManageViewModel();
        public Set_ExtensionsManageViewModel Set_Ex
        {
            get => set_Ex;
            set
            {
                if (set_Ex != value)
                {
                    set_Ex = value;
                    OnPropertyChanged(nameof(Set_Ex));
                }
            }
        }

        //App设置界面
        private Set_AppSettingViewModel set_Appset { get; set; } = new Set_AppSettingViewModel();
        public Set_AppSettingViewModel Set_Appset
        {
            get => set_Appset;
            set
            {
                if (set_Appset != value)
                {
                    set_Appset = value;
                    OnPropertyChanged(nameof(Set_Appset));
                }
            }
        }

        //实例管理界面
        private Set_BrowserManageViewModel set_BrowMge { get; set; } = new Set_BrowserManageViewModel();
        public Set_BrowserManageViewModel Set_BrowMge
        {
            get => set_BrowMge;
            set
            {
                if (set_BrowMge != value)
                {
                    set_BrowMge = value;
                    OnPropertyChanged(nameof(Set_BrowMge));
                }
            }
        }

        //窗口模式
        public SetWindowMode WindowMode { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
