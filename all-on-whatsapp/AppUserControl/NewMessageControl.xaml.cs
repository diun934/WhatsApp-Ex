using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace all_on_whatsapp
{
    /// <summary>
    /// NewMessageControl.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class NewMessageControl
    {
        public NewMessageControl()
        {
            InitializeComponent();
        }

        public NewMessage Message
        {
            get { return (NewMessage)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(NewMessage), typeof(NewMessageControl), new PropertyMetadata(null, OnMessageChanged));

        private static void OnMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as NewMessageControl;
            if (control != null)
            {
                control.DataContext = e.NewValue;
            }
        }
        private void ClearMessage(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = this;
            while (parent != null && !(parent is ItemsControl))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            if (parent != null)
            {
                var itemsControl = parent as ItemsControl;
                if (itemsControl != null)
                {
                    var collection = itemsControl.ItemsSource as ObservableCollection<NewMessage>;
                    if (collection != null)
                    {
                        collection.Remove(Message);
                    }
                }
            }
        }

        // 定义一个路由事件
        public static readonly RoutedEvent MessageClickedEvent = EventManager.RegisterRoutedEvent(
            "MessageClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NewMessageControl));

        // 事件包装器
        public event RoutedEventHandler MessageClicked
        {
            add { AddHandler(MessageClickedEvent, value); }
            remove { RemoveHandler(MessageClickedEvent, value); }
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 触发路由事件
            RaiseEvent(new RoutedEventArgs(MessageClickedEvent));
        }

    }
}
