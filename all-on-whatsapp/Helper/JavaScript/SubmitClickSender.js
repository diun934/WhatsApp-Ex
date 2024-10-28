console.log('Ready to inject SubmitClickSender.js');
function SubmitClickSender(textContent) {
    const targetElement = document.querySelector(`span[title="${textContent}"]`);

    if (targetElement) {
        const mouseDownEvent = new MouseEvent('mousedown', { bubbles: true, cancelable: true });
        const mouseUpEvent = new MouseEvent('mouseup', { bubbles: true, cancelable: true });
        const clickEvent = new MouseEvent('click', { bubbles: true, cancelable: true });

        targetElement.dispatchEvent(mouseDownEvent);
        targetElement.dispatchEvent(mouseUpEvent);
        targetElement.dispatchEvent(clickEvent);

    } else {
        console.log('Element not found');
    }
}
console.log('Complete the injection of SubmitClickSender.js');