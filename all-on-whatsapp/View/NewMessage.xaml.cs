using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace all_on_whatsapp.View
{
    /// <summary>
    /// NewMessage.xaml 的交互逻辑
    /// </summary>
    public partial class NewMessage : Window
    {
        private static NewMessage instance;

        private NewMessage()
        {
            InitializeComponent();
        }

        public static NewMessage Instance
        {
            get
            {
                if (instance == null || !instance.IsLoaded)
                {
                    instance = new NewMessage();
                }
                return instance;
            }
        }

        // 确保窗口关闭时不被销毁
        protected override void OnClosing(CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true; // 阻止窗口关闭
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
          
        }
    }
}
