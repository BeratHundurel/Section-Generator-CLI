$(document).ready(function () {
	const form = document.getElementById('contact-layout');
	const submitButton = document.getElementById('contact-form-btn');

	if (submitButton) {
		submitButton.addEventListener('click', function (event) {
			if (!form.checkValidity()) {
				event.preventDefault()
				event.stopPropagation()
			}
			else {
				form.dispatchEvent(new Event('submit'));
			}
			form.classList.add('was-validated');
		}, false)
	}
})

$(document).ready(function () {
	const form = document.getElementById('contactForm');
	const submitButton = document.getElementById('contact-btn');

	if (submitButton) {
		submitButton.addEventListener('click', function (event) {
			if (!form.checkValidity()) {
				event.preventDefault()
				event.stopPropagation()
			}
			else {
				form.dispatchEvent(new Event('submit'));
			}
			form.classList.add('was-validated');
		}, false)
	}
})

$("body").delegate(".send-msg-button", "click", function () {
	$(".form-info").empty();
	var langId = $("#langId").val();

	const formValid = $("#send-msg");
	formValid.validate({
		errorPlacement: function (error, element) {
			if ($(element).attr("type") == "checkbox") {
				$(element).siblings().addClass("error-label");
				$('#labelId').addClass("error-label");
			}
			else if ($(element) == "checkbox") {
				$(element).siblings().addClass("error-label");
				$('#labelId').addClass("error-label");
			}
			$(element).attr("style", "border:2px solid red !important");
		},
		rules: {
			Name: {
				required: true,
			},
			PhoneNumber: {
				required: true,
			},
			Email: {
				required: true,
				email: true,
			},

		},
		success: function (error, element) {
			if ($(element).attr("type") == "checkbox") {
				$(element).siblings().removeClass("error-label");
				$('#labelId').removeClass("error-label");
			}
			else if ($(element).attr("type") == "checkbox") {
				$(element).siblings().addClass("error-label");
				$('#labelId').addClass("error-label");
			}
			$(element).attr("style", "border:2px solid lightgreen !important");
		},
		messages: {
		},
	});

	if (formValid.valid() == false) {
		return false;
	}

	var formData = new FormData();
	formData.append("Name", $("#form-name").val());
	formData.append("PhoneNumber", $("#form-phone-number").val());
	formData.append("Email", $("#form-email").val());
	formData.append("Message", $("#form-message").val());

	$.ajax({
		url: "Action/SendMessage",
		type: "POST",
		data: formData,
		contentType: false,
		processData: false,
		success: function (data) {
			if (data == "ok") {
				$("#form-name").val("");
				$("#form-email").val("");
				$("#form-phone-number").val("");
				$("#form-message").val("");
				$(".form-info").empty();
				langId == 1 ? $(".form-info").append("Mesajınız başarıyla gönderildi.") : $(".form-info").append("We received your message successfully.");
			}
			else {
				$(".form-info").empty();
				langId == 1 ? $(".form-info").append("Mesajınız gönderilirken sıkıntı oluştu.") : $(".form-info").append("An error occured.");
			}
		},
		error: function (error) {
			console.log("error : ", error.statusText);
		},
	});

});

