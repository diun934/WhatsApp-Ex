using HandyControl.Tools.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace all_on_whatsapp.AppUserControl
{
    /// <summary>
    /// AppNotification.xaml 的交互逻辑
    /// </summary>
    public partial class AppNotification : UserControl
    {
        private DispatcherTimer timer;
        public AppNotification(AppNotificationViewModel viewModel, int displayTimeInSeconds)
        {
            InitializeComponent();
            this.DataContext = viewModel;

            if (displayTimeInSeconds > 0)  // 只有当显示时间大于0秒时，才初始化和启动计时器
            {
                // 初始化计时器
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(displayTimeInSeconds);
                timer.Tick += Timer_Tick!;

                // 注册Loaded事件以启动计时器
                this.Loaded += (s, e) => timer.Start();
            }

            // 注册点击事件以重置计时器（如果计时器已初始化）
            this.MouseLeftButtonDown += (s, e) =>
            {
                if (timer != null && timer.IsEnabled)
                {
                    timer.Stop();
                    timer.Start();  // 重置计时器
                }
            };
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();  // 停止计时器
            this.Hide();  // 执行销毁或隐藏控件的逻辑
        }

    }
}
