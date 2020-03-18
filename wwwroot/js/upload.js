
function initializeFileUpload() {
    if (typeof (window["FileReader"]) == "undefined") {
        return;
    }

    var fileUploadArea = $("#file-upload");

    if (fileUploadArea.length == 0) {
        return;
    }

    fileUploadArea[0].ondragover = function () {
        fileUploadArea.addClass("upload-area-dragging");
        return false;
    };

    fileUploadArea[0].ondragleave = function () {
        fileUploadArea.removeClass("upload-area-dragging");
        return false;
    };

    fileUploadArea[0].ondrop = function (event) {
        fileUploadArea.removeClass("upload-area-dragging");

        var formData = new FormData();

        for (var i = 0; i != event.dataTransfer.files.length; i++) {
            formData.append("files", event.dataTransfer.files[i]);
        }

        $.ajax(
            {
                url: "/upload",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    alert("Files Uploaded!");
                }
            }
        );

        return false;
    }
}

$(document).ready(
    function () {
        initializeFileUpload();
    }
);