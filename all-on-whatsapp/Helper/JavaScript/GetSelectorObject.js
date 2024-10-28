// 获取当前 WhatsApp 实例选中的用户名称和头像
let currentobject = null;

function GetSelectorObject(isskip = false) {
    // 先定位到 id="pane-side" 的元素
    const paneSideElement = document.getElementById('pane-side');

    // 确保找到了 pane-side 元素
    if (!paneSideElement) {
        //console.log('没有找到 id="pane-side" 的元素');
        return;
    }

    // 在 paneSideElement 内查找满足 aria-selected="true" 和 tabindex="0" 的元素
    const targetElement = paneSideElement.querySelector('div[aria-selected="true"][tabindex="0"]');

    // 检查是否找到了目标元素
    if (targetElement) {
        // 获取用户名（电话号码或联系人名称）
        const nameElement = targetElement.querySelector('span[dir="auto"]');
        const content = nameElement ? nameElement.textContent : 'Unknown User';

        // 如果选中的内容与之前相同，则不发送消息
        if (content === currentobject && isskip == false) {
            return; // 选择未变化，不发送消息
        }

        // 获取目标元素内的 img 标签（用于头像）
        const imgElement = targetElement.querySelector('img');
        let imgSrc = "";

        // 检查是否存在 img 标签
        if (imgElement) {
            imgSrc = imgElement.src; // 获取第一个 img 的 src 属性
        }
        console.log(content);
        // 将内容和图片信息发送回 WebView2
        window.chrome.webview.postMessage(`Click:${content}|Avatar:${imgSrc}`);

        // 记录当前用户
        currentobject = content;
    } else {
        // 返回一个没有内容的消息
        window.chrome.webview.postMessage("Click:No selected object");
    }
}
