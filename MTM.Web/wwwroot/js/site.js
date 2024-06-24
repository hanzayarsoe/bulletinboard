dayjs.extend(dayjs_plugin_relativeTime);

DateFormatChange();
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
}

// For Password Hide/Show 
    $('.eye-icon').on('click', function () {
        var input = $($(this).attr('data-toggle'));
        var type = input.attr('type') === 'password' ? 'text' : 'password';
        input.attr('type', type);
        $(this).text(type === 'password' ? '👁️' : '🙈');
    });

// For Moment Usage

// For SwalAlert
function showAlert(type, message, callback) {
    Swal.fire({
        icon: type,
        title: message
    }).then(function () {
        if (typeof callback === 'function') {
            callback();
        }
    });
}

$(function () {
    $('#uploadForm').on('submit', function (event) {
        event.preventDefault();
        var formData = new FormData(this);
        $.ajax({
            url: `/User/Upload`,
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    showAlert('success', response.message, function () {
                        window.location.reload();
                    });
                } else {
                    showAlert('error', response.message);
                }
            },
            error: function () {
                showAlert('error', 'An error occurred while processing the request.');
            }
        });
    });
});
