console.log('Ready to inject SubstituteMessage.js');
function selectAllTextInElement(element) {
    const range = document.createRange();
    range.selectNodeContents(element);

    const selection = window.getSelection();
    selection.removeAllRanges();  // 清除任何之前的选择
    selection.addRange(range);    // 选中输入框中的所有内容
}

// 定义一个发送消息的函数，接受参数 messageContent
function substituteMessage(messageContent) {
    var messageBox = document.querySelector('div[contenteditable="true"][data-tab="10"]');
    if (messageBox) {
        // 让元素获得焦点
        messageBox.focus();

        // 手动全选现有文本
        selectAllTextInElement(messageBox);

        // 创建一个 ClipboardEvent 模拟粘贴
        const dataTransfer = new DataTransfer();
        dataTransfer.setData('text', messageContent);
        const event = new ClipboardEvent('paste', {
            clipboardData: dataTransfer,
            bubbles: true,
            cancelable: true
        });

        // 触发粘贴事件
        messageBox.dispatchEvent(event);
    } 
}

console.log('Complete the injection to SubstituteMessage.js');
