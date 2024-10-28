using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Collections.ObjectModel;

namespace all_on_whatsapp.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Visibility _isShowExpansionPanel = Visibility.Collapsed;
        private double _browserWidth = 900;
        private double _browserHeight = 1400;
        public Visibility IsShowExpansionPanel
        {
            get { return _isShowExpansionPanel; }
            set
            {
                _isShowExpansionPanel = value;
                OnPropertyChanged(nameof(IsShowExpansionPanel));
            }
        }
        public double Webview2BrowserHeight
        {
            get { return _browserHeight; }
            set
            {
                _browserHeight = value;
                OnPropertyChanged(nameof(Webview2BrowserHeight));
            }
        }
        public double Webview2BrowserWidth
        {
            get { return _browserWidth; }
            set
            {
                _browserWidth = value;
                OnPropertyChanged(nameof(Webview2BrowserWidth));
            }
        }
        //浏览器参数组集合
        private ObservableCollection<BrowserGroup> browserGroups = new ObservableCollection<BrowserGroup>();
        public ObservableCollection<BrowserGroup> BrowserGroups
        {
            get { return browserGroups; }
            set
            {
                browserGroups = value;
                OnPropertyChanged(nameof(BrowserGroups));
            }
        }
        private ObservableCollection<PinUserModel> _pinUserList = new ObservableCollection<PinUserModel>();
        public ObservableCollection<PinUserModel> PinUserList
        {
            get { return _pinUserList; }
            set
            {
                _pinUserList = value;
                OnPropertyChanged(nameof(PinUserList));
            }
        }

        private InfoBarContent _infoBarContent = new InfoBarContent();

        public InfoBarContent InfoBarContent
        {
            get => _infoBarContent;
            set => SetProperty(ref _infoBarContent, value);
        }

        public NewMessageList NewMessageList { get; set; } = new NewMessageList();

        private MyStyle myStyle = new MyStyle();
        public MyStyle MyStyle
        {
            get => myStyle;
            set => SetProperty(ref myStyle, value);
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
}
