dayjs.extend(dayjs_plugin_relativeTime);
window.onload = function () {
    // Adjust the height of the textareas when the page loads
    document.querySelectorAll('.auto-resize-input').forEach(function (textarea) {
        autoResize(textarea);
    });
};

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

// auto resize input box and text area 
function autoResize(textarea) {
    textarea.style.height = 'auto';
    textarea.style.height = textarea.scrollHeight + 'px';
}
$('.auto-resize-input').on("input", function () {
    this.style.height = 'auto';
    this.style.height = this.scrollHeight + 'px';
})

// For Swal Alert For Delete
const showSwal = (title, text, icon, callback) => {
    Swal.fire({ title, text, icon }).then(callback);
};

let swalDelete = (url, deleteId, callback) => {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to recover this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then(result => {
        if (!result.isConfirmed) return;
        $.get(url, { id: deleteId })
            .done(result => {
                const success = result.responseType == 1;
                showSwal(success ? "Deleted!" : "Can't Delete!", result.responseMessage, success ? "success" : "error", () => callback(success));
            })
            .fail(err => {
                showSwal("Error!", "An error occurred while deleting.", "error", () => callback(false));
            });
    });
};

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
                console.log("Response Success", response.success);
                if (response.success) {
                    showSwal(response.message, "", 'success', function () {
                        window.location.reload();
                    })
                } else {
                    showSwal(response.message, "", 'error');
                }
            },
            error: function () {
                showSwal('An error occurred while processing the request.', "", 'error');
            }
        });
    });
    $('#export').on('click', function () {
        let category = $(this).data("id");
        window.location.href = `/${category}/Export`;
    });
});
