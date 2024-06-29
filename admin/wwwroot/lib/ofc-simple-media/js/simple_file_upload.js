
$(document).on("change", ".simple_file__upload___input", function () {
    let $this = $(this);
    let id = $this.data("id");

    //Inclusive of the input
    let inclusiveOfTheInput = $this.closest(".simple_file__upload___inclusive");

    let fileTitle = inclusiveOfTheInput.find("input[name='fileTitle']").val();
    if (fileTitle == "" || fileTitle == null) {
        fileTitle = $("input[name='Title']").val();
    }
    if (fileTitle == "" || fileTitle == null) {
        inclusiveOfTheInput.find("input[name='fileTitle']").addClass("border border-danger").show();
        $this.val(null);
        return null;
    }
    else {
        inclusiveOfTheInput.find("input[name='fileTitle']").removeClass("border border-danger").hide();
        fileTitle = $("input[name='Title']").val();
    }

    let imageWidth = inclusiveOfTheInput.find("input[name='imageHeight']").val();
    let imageHeight = inclusiveOfTheInput.find("input[name='imageWidth']").val();
    let imageQuality = inclusiveOfTheInput.find("input[name='imageQuality']").val();
    let filePath = inclusiveOfTheInput.find("input[name='filePath']").val();

    let formData = new FormData();
    formData.append('FilePath', filePath);
    formData.append('MediaTitle', fileTitle);
    formData.append('MediaWidth', imageWidth);
    formData.append('MediaHeight', imageHeight);
    formData.append('MediaQuality', imageQuality);

    let fileUploadSimple = $this.get(0);
    let fileUploadSimpleFiles = fileUploadSimple.files;
    for (let i = 0; i < fileUploadSimpleFiles.length; i++) {
        formData.append(fileUploadSimpleFiles[i].name, fileUploadSimpleFiles[i]);
    }

    let simpleFileUploadOverlay = $this.closest(".input_group_simple__upload").find(".input_group_simple__upload__overlay");
    simpleFileUploadOverlay.show()//Show the overlay

    //Upload the file
    $.ajax({
        url: "/admin/media/insert",
        dataType: 'json',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,  // tell jQuery not to process the data
        success: function (result) {
            $.each(result, function (i, v) {
                let oldFileName = v.oldFileName;
                let filePathWithFileName = v.filePathWithFileName;
                let fileContentType = v.fileContentType;
                let mediaId = v.mediaId;
                let fileHtml = "";

                if (v.alertMessage == "success") {
                    inclusiveOfTheInput.find(".simple_file__upload_media_id").val(mediaId);

                    if (fileContentType.includes("image") == true) {
                        fileHtml = "<div class='simple_file_upload__item'><a href='javascript:;' data-mediaid='" + mediaId + "' \
                    data-id='" + id + "' class='bg-danger text-white btn btn-icon btn_remove_simple_uploaded_file'>\
                    <i class='fa fa-times'></i></a>" +
                            "<img class='mw-100' src='" + filePathWithFileName + "' />" + oldFileName + "</div>";
                    }
                    else if (fileContentType.includes("video")) {
                        fileHtml = "<div class='simple_file_upload__item'><a href='javascript:;' data-mediaid='" + mediaId + "' \
                    data-id='" + id + "' class='bg-danger text-white btn btn-icon btn_remove_simple_uploaded_file'>\
                    <i class='fa fa-times'></i></a>"
                            + "<video class='w-100' width='100%' height='auto' autoplay controls><source src='" + filePathWithFileName + "' type='video/mp4' ></video>" + oldFileName + "</div>";
                    }
                    else {
                        fileHtml = "<div class='simple_file_upload__item'><a href='javascript:;' data-mediaid='" + mediaId + "' \
                    data-id='" + id + "' class='bg-danger text-white btn btn-icon btn_remove_simple_uploaded_file'>\
                    <i class='fa fa-times'></i></a>"+ oldFileName + "</div>";
                    }
                    inclusiveOfTheInput.find('.simple_file_upload__content').append(fileHtml);
                }
                else if (v.alertMessage == "big-size") {
                    alert("Boyutu küçültün.");
                }
                else if (v.alertMessage == "external-image-uploaded") {
                    alert("Harici fotoğraf yüklediniz.");
                }
                else if (v.alertMessage == "no-data") {
                    alert("Veri bulunamadı");
                }
            });
        },
        error: function (jqXHR) {
        },
        complete: function (jqXHR, status) {
            inclusiveOfTheInput.find("input[name='fileTitle']").val(null);
            simpleFileUploadOverlay.hide();//Hide the overlay
        }
    });

});

$(document).on('click', '.btn_remove_simple_uploaded_file', function (evt) {
    evt.preventDefault();
    evt.stopPropagation();

    var obj = $(this);

    var mediaid = obj.data('mediaid');
    var id = obj.data('id');

    var formData = new FormData();
    formData.append("id", mediaid);

    $.ajax({
        url: "/admin/media/delete",
        type: "POST",
        processData: false,
        contentType: false,
        data: formData
    }).done(function (data) {
        if (data == "success") {
            obj.closest(".simple_file__upload___inclusive").find(".simple_file__upload_media_id").val(""); //Remove media id
            obj.closest('.simple_file_upload__item').remove();
        }
    });
});
