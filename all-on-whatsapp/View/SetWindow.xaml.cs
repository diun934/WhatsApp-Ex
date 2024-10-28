using all_on_whatsapp.ViewModel;
using HandyControl.Controls;
using HandyControl.Tools;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ScrollViewer = System.Windows.Controls.ScrollViewer;

namespace all_on_whatsapp
{
    /// <summary>
    /// SetWindowView.xaml 的交互逻辑
    /// </summary>
    public partial class SetWindow : System.Windows.Window
    {
        // 定义一个新增实例事件，用于通知主窗口
        public event EventHandler? AddButtonClicked;

        public SetWindowViewModel _viewModel;

        public SetWindow(SetWindowViewModel viewModel)
        {
            InitializeComponent();

            //初始化模型数据
            _viewModel = viewModel;

            DataContext = viewModel;
        }
        /// <summary>
        /// 窗口加载完毕后的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (_viewModel.WindowMode == SetWindowMode.Add)
            {
                //这不是由实例的上下文菜单发起的事件
                InstanceListBox.Visibility = Visibility.Collapsed;
                BrowserManagementTabItem.Header = "新增实例";
                _viewModel.Set_BrowMge.IsShowDeltelBtn = Visibility.Collapsed;
                _viewModel.Set_BrowMge.BtnText1 = "新增";
                _viewModel.Set_BrowMge.BtnText2 = "新增并退出";
            }
            else
            {
                InstanceListBox.Visibility = Visibility.Visible;
                _viewModel.Set_BrowMge.IsShowUpdate = Visibility.Collapsed;
                BrowserManagementTabItem.Header = "编辑实例";
                _viewModel.Set_BrowMge.BtnText1 = "更新";
                _viewModel.Set_BrowMge.BtnText2 = "更新并退出";

                // 获取 IconSelect 中的所有 RadioButton 组成的列表
                var radioButtons = IconSelect.Children.OfType<RadioButton>().ToList();
                var radioIndex = _viewModel.Set_BrowMge.BrowserGrop.ItemType;
                if (radioButtons.Count >= radioIndex)
                {
                    radioButtons[radioIndex].IsChecked = true;

                    //如果是文字类型的图标
                    if (_viewModel.Set_BrowMge.BrowserGrop.ButtonsModel.IconType == 0)
                    {
                        radioButtons[radioButtons.Count - 1].IsChecked = true;
                    }
                }
            }
            //根据窗口模式打开不同的子夹
            var tabIndexs = new List<(SetWindowMode mode, int index)> { (SetWindowMode.Add, 0), (SetWindowMode.Edit, 0), (SetWindowMode.Extensions, 1), (SetWindowMode.Set, 2), };
            int index = tabIndexs.FirstOrDefault(x => x.mode == _viewModel.WindowMode).index;

            //读入App设置配置项
            _viewModel.Set_Appset.Appsetings = await ConfigManager.LoadAppConfigAsync();

            // 指定当前选中的子夹
            if (MainTabControl.Items.Count - 1 >= index)
            {
                MainTabControl.SelectedIndex = index;
            }
        }
        /// <summary>
        /// 创建新的实例
        /// </summary>
        private bool CreateBrowser()
        {
            var dictionary = new List<(int index, double value)> { (0, 1), (1, 1.2), (2, 1.5), (3, 1.8), (4, 2), };
            _viewModel.Set_BrowMge.BrowserGrop.BrowserModel.ZoomFactor = dictionary[ZoomFactorComboBox.SelectedIndex].value;
            _viewModel.Set_BrowMge.BrowserGrop.ItemType = InstanceTypeComboBox.SelectedIndex;

            // 如果输入的字符包含在 BrowserGroups 中，阻止保存
            if (_viewModel.Set_BrowMge.BrowserGroups.FirstOrDefault(c => c.Tag.Equals($"{_viewModel.Set_BrowMge.BrowserGrop.Tag}")) != null)
            {
                AppNotice.Show("警告", "已存在相同名称的实例，请重新输入！", AppNotificationLevel.Warning);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 保存/删除/编辑实例
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SetManagement_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string btnTag)
            {
                if (_viewModel.WindowMode == SetWindowMode.Add)//新增模式
                {
                    switch (btnTag)
                    {
                        case "m2"://连续创建浏览器实例
                            if (_viewModel.WindowMode == SetWindowMode.Add && CreateBrowser()) AddButtonClicked?.Invoke(this, EventArgs.Empty);
                            break;
                        case "m3"://新增实例并退出
                            if (CreateBrowser()) this.DialogResult = true; //关闭窗口
                            break;
                    }
                }
                else if (_viewModel.WindowMode == SetWindowMode.Edit)//编辑模式
                {
                    switch (btnTag)
                    {
                        case "m1"://删除实例
                            _viewModel.Set_BrowMge.BrowserGrop.CreateTime = string.Empty; //传递删除信号
                            this.DialogResult = true;//关闭窗口
                            break;
                        case "m3"://更新实例并退出
                            var x = _viewModel.Set_BrowMge.BrowserGrop.ButtonsModel.IconSource;
              
                            //传递更新信号
                            this.DialogResult = true;//关闭窗口
                            break;
                    }
                }
                else if(_viewModel.WindowMode == SetWindowMode.Set)
                {
                    if (btnTag == "RestoreDefault")
                    {
                        //app设置初始化
                        _viewModel.Set_Appset.Appsetings = new Appsetings();

                        Close();//退出程序
                    }
                    else
                    {
                        //更新系统配置
                        this.DialogResult = true;//关闭窗口
                    }
                    //保存配置文件
                    await ConfigManager.SaveAppConfigAsync(_viewModel.Set_Appset.Appsetings);
                }
            }
        }
        /// <summary>
        /// 图标样式更换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectIcon_Checked(object sender, RoutedEventArgs e)
        {
            var selectedRadioButton = sender as RadioButton;
            if (selectedRadioButton != null)
            {
                if (selectedRadioButton.Content is Ellipse ellipse && ellipse != null && ellipse.Fill is ImageBrush brush)
                {
                    _viewModel.Set_BrowMge.BrowserGrop.ButtonsModel.IconSource = brush.ImageSource.ToString();
                    _viewModel.Set_BrowMge.BrowserGrop.ButtonsModel.IconType = 1;//代表图片
                }
                else
                {
                    var text = selectedRadioButton.Content as TextBlock;
                    if (text != null)
                    {
                        _viewModel.Set_BrowMge.BrowserGrop.ButtonsModel.IconSource = text.Text;
                        _viewModel.Set_BrowMge.BrowserGrop.ButtonsModel.IconType = 0;//代表文字
                    }
                }
            }
        }
        /// <summary>
        /// 实例组合款切换事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InstanceTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                var newItem = e.AddedItems[0] as ItemType;
                if (newItem != null)
                {
                    var index = _viewModel.Set_BrowMge.BrowserGroups.Where(x => x.ItemType == newItem.Type).ToList().Count;
                    _viewModel.Set_BrowMge.BrowserGrop.Tag = $"{newItem.Name}{index + 1}";
                    _viewModel.Set_BrowMge.BrowserGrop.ButtonsModel.IconSource = $"{index + 1}";

                    List<RadioButton> radioButtons = IconSelect.Children.OfType<RadioButton>().ToList();
                    var radioButton = radioButtons.FirstOrDefault(x => x.Name == newItem.IconName);
                    if (radioButton != null)
                    {
                        radioButton.IsChecked = true;
                    }
                    //更改业务类型
                    _viewModel.Set_BrowMge.BrowserGrop.ItemType = newItem.Type;
                    _viewModel.Set_BrowMge.BrowserGrop.BrowserModel.Url = newItem.Url;
                }
            }
        }
        /// <summary>
        /// 编辑列表的实例切换事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowserSwitchRadioButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as RadioButton;
            if (btn != null)
            {
                var model = btn.DataContext as BrowserGroup;
                if (model == null) return;

                _viewModel.Set_BrowMge.BrowserGrop = model;
                if (model.ButtonsModel.IconType == 0)
                {
                    textIcon.IsChecked = true;
                }
                var dictionary = new List<(int index, double value)> { (0, 1), (1, 1.2), (2, 1.5), (3, 1.8), (4, 2), };

                // 查找与 model.BrowserModel.ZoomFactor 匹配的项
                int selectedIndex = dictionary.FindIndex(x => x.value == model.BrowserModel.ZoomFactor);

                // 如果找不到匹配项，FindIndex 返回 -1，默认选择第一个
                ZoomFactorComboBox.SelectedIndex = selectedIndex != -1 ? selectedIndex : 0;
            }
        }

    }
}
