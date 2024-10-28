console.log('Ready to inject ReceiveNotification.js');

// 保存原始的Notification构造函数
const OriginalNotification = Notification;

// 重写全局的Notification构造函数
Notification = function (title, options) {

    // 序列化title和options为一个JSON字符串
    const message = JSON.stringify({ title: title, options: options });

    window.chrome.webview.postMessage("Notification:" + message);  // 将内容发送回 WebView2

    // 调用原始的Notification构造函数（这里注释掉了，将不再显示通知）
    //return new OriginalNotification(title, options);
};
console.log('Complete the injection of ReceiveNotification.js');

// 确保新的Notification函数有原来的permission和requestPermission属性
Notification.permission = OriginalNotification.permission;
Notification.requestPermission = function () {
    return OriginalNotification.requestPermission.apply(OriginalNotification, arguments);
};
