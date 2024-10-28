console.log('Ready the injection to objectMonitor.js');

// 定义一个插入翻译按钮的函数
function insertTranslateButton() {
    // 查找 aria-label 为 "语音消息" 或 "Voice message" 的按钮
    var voiceMessageButton = document.querySelector('button[aria-label="语音消息"], button[aria-label="Voice message"]');

    if (voiceMessageButton) {
        // 检查是否已经存在翻译按钮，避免重复插入
        if (!document.querySelector('button[aria-label="翻译"]')) {
            // 调整父容器的宽度以适应更多按钮
            var buttonContainer = voiceMessageButton.parentNode;
            buttonContainer.style.width = 'auto'; // 设置宽度为自动，适应更多按钮

            // 创建新的翻译按钮
            var translateButton = document.createElement('button');
            translateButton.className = 'xfect85 x100vrsf x1vqgdyp x78zum5 xl56j7k x6s0dn4'; // 与现有按钮相同的类样式
            translateButton.setAttribute('aria-label', '翻译'); // 设置 aria-label
            translateButton.setAttribute('data-tab', '12'); // 设置 data-tab 属性

            // 设置按钮左右边距为 5px
            translateButton.style.marginRight = '5px';
            translateButton.style.marginLeft = '5px';

            // 插入 SVG 图标
            translateButton.innerHTML = `
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor" class="bi bi-translate" viewBox="0 0 16 16">
                  <path d="M4.545 6.714 4.11 8H3l1.862-5h1.284L8 8H6.833l-.435-1.286H4.545zm1.634-.736L5.5 3.956h-.049l-.679 2.022H6.18z"/>
                  <path d="M0 2a2 2 0 0 1 2-2h7a2 2 0 0 1 2 2v3h3a2 2 0 0 1 2 2v7a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2v-3H2a2 2 0 0 1-2-2V2zm2-1a1 1 0 0 0-1 1v7a1 1 0 0 0 1 1h7a1 1 0 0 0 1-1V2a1 1 0 0 0-1-1H2zm7.138 9.995c.193.301.402.583.63.846-.748.575-1.673 1.001-2.768 1.292.178.217.451.635.555.867 1.125-.359 2.08-.844 2.886-1.494.777.665 1.739 1.165 2.93 1.472.133-.254.414-.673.629-.89-1.125-.253-2.057-.694-2.82-1.284.681-.747 1.222-1.651 1.621-2.757H14V8h-3v1.047h.765c-.318.844-.74 1.546-1.272 2.13a6.066 6.066 0 0 1-.415-.492 1.988 1.988 0 0 1-.94.31z"/>
                </svg>
            `;

            // 将翻译按钮插入到语音消息按钮前面
            voiceMessageButton.parentNode.insertBefore(translateButton, voiceMessageButton);

            // 给翻译按钮添加点击事件，回传输入框内容到 C#
            translateButton.addEventListener('click', function () {
                // 获取当前输入框的内容
                var messageBox = document.querySelector('div[contenteditable="true"][data-tab="10"]');
                if (messageBox) {
                    var inputText = messageBox.innerText || messageBox.textContent;
                    // 使用 WebView2 将输入的文字发送给 C#
                    window.chrome.webview.postMessage("Translation:" + inputText);
                }
            });
        }
    }
}


// 创建 MutationObserver 来监听 DOM 变化，监控 "pane-side" 元素以及插入翻译按钮
var observer = new MutationObserver(function (mutationsList, observer) {
    for (var mutation of mutationsList) {
        if (mutation.type === 'childList') {
            var paneSideElement = document.getElementById("pane-side");
            // 监听 pane-side 元素的点击事件
            if (paneSideElement) {

                // 调用方法来初始化滚动事件监听器
                //initializeScrollListener();
                paneSideElement.addEventListener("click", function () {
                    //获取当前whatsapp实例选中的用户名称和头像
                    GetSelectorObject();

                });
                
                //移除获取windows版whatsapp
                var targetSpan = document.querySelector('span[data-icon="wa-square-icon"]');
                if (targetSpan) {
                    // 从 <span> 元素向上查找，定位到 <div role="button" tabindex="0" data-tab="4">
                    var targetButtonDiv = targetSpan.closest('div[role="button"][tabindex="0"][data-tab="4"]');

                    // 如果找到了目标 <div>，则删除它
                    if (targetButtonDiv) {
                        targetButtonDiv.remove();
                    }
                }
                //移除找到新顾客
                var newCustomer = document.querySelector('div._amik');
                if (newCustomer) {
                    // 如果找到了目标 <div>，则删除它
                    newCustomer.remove();
                }
            }
            // 插入翻译按钮
            insertTranslateButton();
        }
    }
});

// 监听整个文档的变化
observer.observe(document.body, { childList: true, subtree: true });

console.log('Complete the injection to objectMonitor.js');