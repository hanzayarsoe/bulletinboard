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

// For Delete Process - Uses - Post, User
let swalDelete = (url, deleteId, callback ) => {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to recover this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'GET',
                data: { id: deleteId },
                success: function (result) {
                    console.log("result", result)
                    if (result.responseType == 1) {
                        Swal.fire({
                            title: "Deleted!",
                            text: result.responseMessage,
                            icon: "success"
                        }).then(() => {
                            callback(true);
                        });
                    } else {
                        Swal.fire({
                            title: "Can't Deleted!",
                            text: result.responseMessage,
                            icon: "fail"
                        }).then(() => {
                            callback(false);
                        });
                    }
                },
                error: function (err) {
                    console.error('Error deleting user:', err);
                    Swal.fire({
                        title: "Error!",
                        text: "An error occurred while deleting.",
                        icon: "error"
                    }).then(() => {
                        callback(false);
                    });
                }
            });
        }
    });
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
// auto resize input box and text area 
$('.auto-resize-input').on("input",function () {
    this.style.height = 'auto';
    this.style.height = this.scrollHeight + 'px';
})

$(function () {
    $('#uploadForm').on('submit', function (event) {
        event.preventDefault();
        let category = $(this).data("id");
        var formData = new FormData(this);
        $.ajax({
            url: `/${category}/Upload`,
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
    $('#export').on('click', function () {
        let category = $(this).data("id");
        window.location.href = `/${category}/Export`;
    });
});
