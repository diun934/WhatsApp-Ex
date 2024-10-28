using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace all_on_whatsapp.View
{
    /// <summary>
    /// AutoStatus.xaml 的交互逻辑
    /// </summary>
    public partial class AutoStatus : Window
    {
        public StatusTask statusTask { get; set; }

        //储存checkbox和webview2组合
        Dictionary<CheckBox, string> TemporaryDictionary = new Dictionary<CheckBox, string>();
        public AutoStatus(List<BrowserGroup> browserGroup)
        {
            InitializeComponent();

            statusTask = new StatusTask();

            if (browserGroup.Any())
            {
                foreach (var item in browserGroup)
                {
                    var checkBox = new CheckBox
                    {
                        IsChecked = true,
                        Content = item.Tag,
                        Margin = new Thickness(5),
                        Width = 130,
                    };
                    //checkBox加入容器
                    CheckboxList.Children.Add(checkBox);
                    //加入临时字典
                    TemporaryDictionary.Add(checkBox, item.Tag);
                }
            }
        }

        private void ManageTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn && btn.Tag is string btnTag))
            {
                return;
            }
            if (btnTag == "Start")
            {
                if (string.IsNullOrEmpty(StatusTextContent.Text))
                {
                    AppNotice.Show("警告", "必须输入动态文字", AppNotificationLevel.Warning);
                    return;
                }

                var photoSource = StatusImageSelector.Uri?.ToString();

                statusTask = new StatusTask
                {
                    PhotoSource = photoSource,//准备发送的素材
                    TextContent = StatusTextContent.Text,
                    IsGraphicMode = !string.IsNullOrEmpty(photoSource),//判断发送的类型
                    SenderList = TemporaryDictionary.Where(x => x.Key.IsChecked == true).Select(x => x.Value).ToList()//获取需要发送的对象
                };
                this.DialogResult = true;
            }
            else if (btnTag == "Stop")//停止
            {

            }
            else if (btnTag == "Timing")//定时
            {

            }
            else//退出
            {
                Close();
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
