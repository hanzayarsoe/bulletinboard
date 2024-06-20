dayjs.extend(dayjs_plugin_relativeTime);

DateFormatChange();
// For Password Hide/Show 
    $('.eye-icon').on('click', function () {
        var input = $($(this).attr('data-toggle'));
        var type = input.attr('type') === 'password' ? 'text' : 'password';
        input.attr('type', type);
        $(this).text(type === 'password' ? '👁️' : '🙈');
    });

// For Moment Usage
function DateFormatChange() {
    let dateSelector = $('.dayjs');
    let datetime = dateSelector.data("datetime");
    let type = dateSelector.data("type");
    switch (type) {
        case "dmy": dateSelector.text(dayjs(datetime).format("DD-MM-YYYY")); break;
        case "hourago": dateSelector.text(dayjs(datetime).fromNow()); break;
        case "day": dateSelector.text(dayjs(datetime).format('dddd')); break;
        case "time": dateSelector.text(dayjs(datetime).format('HH:mm')); break;
        default: dateSelector.text(dayjs(datetime).format("DD-MM-YYYY")); break;
    }
};


