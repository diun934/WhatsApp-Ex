console.log('Ready to inject MonitorUnread.js');

// 获取 <title> 元素
var targetNode = document.querySelector('title');

// 检查元素是否存在
if (targetNode) {
    // 创建一个 MutationObserver 实例来监控 <title> 元素的变化
    var observer = new MutationObserver(function () {
        // 获取当前的标题文本
        var titleText = targetNode.textContent.trim();

        // 提取未读消息数 (在括号中)
        var unreadMatch = titleText.match(/\((\d+)\)/);
        var unreadCount = unreadMatch ? unreadMatch[1] : "0";  // 如果没有匹配到数字，默认未读消息为 0

        // 发送当前未读消息数
        window.chrome.webview.postMessage("Unread:" + unreadCount);
    });

    // 配置 MutationObserver 来监控文本变化
    observer.observe(targetNode, { characterData: true, childList: true, subtree: true });

    console.log('Monitoring initialized for the title.');
} 

console.log('Complete the injection to MonitorUnread.js');
