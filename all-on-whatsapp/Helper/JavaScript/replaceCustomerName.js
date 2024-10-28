// 定义目标文字和替换文字的集合
const replacements = [
    { target: "洪兴社团", replacement: "替换文字1" },
    { target: "+86 135 3803 0330", replacement: "替换文字2" },
    { target: "+853 6202 6785", replacement: "替换文字3" }
];

// 用于替换目标文本的函数(指定用户更换为数据库中的名字)
function replaceCustomerName() {
    for (const { target, replacement } of replacements) {
        // 使用 CSS 选择器找到符合条件的 <span> 元素
        const targetSpans = document.querySelectorAll('span[title="' + target + '"]');
        targetSpans.forEach(targetSpan => {
            var originalName = targetSpan.getAttribute('title');
            targetSpan.textContent = replacement + "(" + originalName + ")";
            // 设置 <span> 元素的文本颜色为蓝色
            targetSpan.style.color = 'gray';
        });
    }
}
function initializeScrollListener() {
    // 获取列表框元素
    const paneSide = document.getElementById('pane-side');

    if (!paneSide) {
        console.warn('未找到元素 pane-side');
        return; // 如果元素未找到，则退出函数
    }

    // 定义一个变量来存储定时器
    let isScrolling;

    // 添加滚动事件监听器
    paneSide.addEventListener('scroll', () => {
        console.log('开始滚动'); // 触发滚动时的操作

        // 如果用户还在滚动，清除之前的定时器
        clearTimeout(isScrolling);

        // 设置一个新的定时器，在滚动结束后 200 毫秒触发
        isScrolling = setTimeout(() => {
            console.log('滚动结束'); // 滚动结束后的操作
            replaceCustomerName();
        }, 200);
    });
}


