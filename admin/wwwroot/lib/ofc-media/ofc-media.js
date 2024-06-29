$('.quality_slider').bootstrapSlider();

/*--------------------------- start of normal file upload -------------------------*/
$(document).ready(function () {

	$(".file_upload__box").find("input[name='fileText']");
	$.each($(".file_upload__box"), function (i, v) {
		const $this = $(this);
		if ($this.find("input[name='fileText']").val() == "") {
			$this.find("input[name='fileText']").val("untitled");
		}
	});
});

$(document).on("click", ".media_modal__btn", function () {
	var mediaMmodal = $(this).closest(".file_upload__inbox").find(".media_modal");
	var mediaId = mediaMmodal.find(".file_upload__mediaid").val();
	if (mediaId != 0 && mediaId != null) {
		console.log("Medya bulundu.");
	}
	else {
		mediaMmodal.find(".file_upload__form").css("display", "block");
		console.log("Medya bulunamadı.");
	}
	mediaMmodal.modal("show");
});

/*---------------------------
	 FILE UPLOAD
* -------------------------*/
$(document).on("change", "input[name='fileUpload']", function () {
	// FormData ile yüklenen dosya kontrollera gönderiliyor.
	// Kontrollerdan dönen media type html içerisine append ediliyor.
	var formData = new FormData();
	var fileUpload = $(this)[0].files[0];
	formData.append('fileUpload', fileUpload); // Append the file to the FormData object
	var $this = $(this);
	$this.closest(".file_upload__form").find(".overlay").addClass("d-flex").removeClass("d-none");//Show the overlay
	var paramMediaId = $this.data("id");
	$.ajax({
		url: "/admin/media/insert",
		dataType: 'json',
		type: 'POST',
		data: formData,
		processData: false,  // tell jQuery not to process the data
		contentType: false,  // tell jQuery not to set contentType
		success: function (result) {
			const file = result;
			if (file) {
				$this.closest(".file_upload__box").find(".file_upload__mediaid").val(result.id);
				$(".mediaIdForCategory").val(result.id);
				const reader = new FileReader();
				reader.onload = function (e) {
					const fileHtml = "<div class='file_upload__item'><a href='javascript:;' data-mediaid='" + result.id + "' \
					data-id='"+ paramMediaId + "' class='bg-danger text-white btn btn-icon btn_remove__file'>\
					<i class='fa fa-times'></i></a>" +
						"<img class='mw-100' src='" + result.path + "' />" + result.title + "</div>";
					$this.closest(".file_upload__form").hide("250"); //Hide the form 
					$this.closest('.file_upload__box').find('.file_upload__content').append(fileHtml);
				};
				const blob = new Blob([file], { type: file.type });
				reader.readAsDataURL(blob);
			} else {
				preview.src = '';
			}
		},
		error: function (jqXHR) {
		},
		complete: function (jqXHR, status) {
			$this.closest(".file_upload__form").find(".overlay").removeClass("d-flex").addClass("d-none");
		}
	});

});

$(document).on('click', '.btn_remove__file', function (evt) {
	// Media Id formData ile gönderilecek.
	// Media silindikten sonra modalda kalan inputların valuesu yeniden resim eklenme durumunda değişiklik kabul edebilmesi için sıfırlanıyor.
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
			if (obj.closest(".file_upload__box").find(".btn_remove__file").length == 1) {
				obj.closest(".file_upload__box").find(".file_upload__form").show("250"); //Show form for uploading.
			}
			obj.closest(".file_upload__box").find(".file_upload__mediaid").val("0"); //Remove media id

			obj.closest('.file_upload__box').find('input[name="paramImage' + id + '"]').val("");
			obj.closest('.file_upload__box').find('input[name="paramVideo' + id + '"]').val("");
			obj.closest('.file_upload__box').find("input[name='fileUpload']").val("");
			obj.closest('.file_upload__item').remove();
		}
	});
});

function ShowHideItem(item, isShow) {
	if (isShow == true) {
		item.removeClass("d-none");
	}
	else {
		item.addClass("d-none");
	}
}