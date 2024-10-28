function continuePictureUpload(textContent) {
    setTimeout(() => {
        //定位到输入框
        const editableDiv = document.querySelector('div[contenteditable="true"]');
        if (editableDiv) {
            editableDiv.focus();

            //模拟文本到剪贴板
            const dataTransfer = new DataTransfer();
            dataTransfer.setData('text', textContent);

            //触发剪贴板粘贴事件
            const pasteEvent = new ClipboardEvent('paste', { clipboardData: dataTransfer, bubbles: true, cancelable: true });
            editableDiv.dispatchEvent(pasteEvent);
        }
    }, 500);

    setTimeout(() => {
        const sendButton = document.querySelector('div[aria-label="发送"],div[aria-label="Send"]');
        if (sendButton) {
            sendButton.click();
            setTimeout(() => document.querySelector('[data-icon="chats-outline"]')?.click(), 1000);
        }
        window.chrome.webview.postMessage("Success:Text status sent");//回调信息
    }, 1000);
}


//Send dynamic function implementation
async function sendStatus(statusType, textContent, imageResource = null) {
    const clickAndWait = async (selector, delay = 1000) => {
        return new Promise(resolve => {
            setTimeout(() => {
                const element = document.querySelector(selector);
                if (element) {
                    element.click();
                }
                resolve(); // 确保 Promise 在点击后解决
            }, delay);
        });
    };
    try {
        await clickAndWait('[data-icon="status-outline"]', 500);
        await clickAndWait('div[aria-label="Add Status"]');

        let element;
        if (statusType === 0) {
            element = document.querySelector('[data-icon="media-multiple"]');
            if (element) {
                element.click();
                window.chrome.webview.postMessage("Sucess:" + imageResource + "|" + textContent);
            }
        } else {
            element = document.querySelector('[data-icon="media-editor-drawing"]');
            if (element) {
                element.click();
                setTimeout(() => {
                    //定位到输入框
                    const editableDiv = document.querySelector('div[contenteditable="true"]');
                    if (editableDiv) {
                        editableDiv.focus();

                        //模拟文本到剪贴板
                        const dataTransfer = new DataTransfer();
                        dataTransfer.setData('text', textContent);

                        //触发剪贴板粘贴事件
                        const pasteEvent = new ClipboardEvent('paste', { clipboardData: dataTransfer, bubbles: true, cancelable: true });
                        editableDiv.dispatchEvent(pasteEvent);
                    }
                }, 500);

                setTimeout(() => {
                    const sendButton = document.querySelector('div[aria-label="发送"],div[aria-label="Send"]');
                    if (sendButton) {
                        sendButton.click();
                        setTimeout(() => document.querySelector('[data-icon="chats-outline"]')?.click(), 1000);
                    }
                    window.chrome.webview.postMessage("Success:Text status sent");//回调信息
                }, 1000);
            }
        }
    } catch (error) {
        window.chrome.webview.postMessage("Error:" + error.message);
    }
}



