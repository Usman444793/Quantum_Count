window.submitPost = (url, data) => {
    try {
        const form = document.createElement('form');
        form.method = 'post';
        form.action = url;
        for (const key in data) {
            if (!Object.prototype.hasOwnProperty.call(data, key)) continue;
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = key;
            input.value = data[key] ?? '';
            form.appendChild(input);
        }
        document.body.appendChild(form);
        form.submit();
    } catch (e) {
        console.error('submitPost error', e);
    }
};
document.addEventListener('click', function (e) {
    try {
        var avatar = e.target.closest('.avatar-button');
        if (avatar) {
            var pop = document.querySelector('.account-popover');
            if (pop) {
                pop.classList.toggle('visible');
                pop.classList.toggle('hidden');
            }
            return;
        }
        var popEl = document.querySelector('.account-popover');
        if (popEl && !e.target.closest('.account-popover')) {
            popEl.classList.remove('visible');
            popEl.classList.add('hidden');
        }
    } catch (err) {
        console.error('popover toggle error', err);
    }
});