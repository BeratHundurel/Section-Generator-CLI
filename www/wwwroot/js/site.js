$(".contact-form input").on("click", function (event) {
	event.stopPropagation();
});
$(".contact-form a").on("click", function (event) {
	event.stopPropagation();
});

$("body").delegate(".footer-button", "click", function () {
	console.log("footer mail");
	var mail = $(".footer-input").val();
	$(".mail-input-val").empty();
	$.ajax({
		url: "Main/SubscriptionsForm",
		type: "POST",
		data: { Email: mail },
		success: function (data) {
			if (data == "ok") {
				$(".mail-input-val").append("Başarıyla bültene abone oldunuz.").attr("style", "color:#000 !important; font-weight:400;");
				$(".footer-input").val("")
			}
			else if (data == "null") {
				$(".mail-input-val").append("Bu alan boş geçilemez.").attr("style", "color:red !important; font-weight:400;");
			}
			else if (data == "exist") {
				$(".mail-input-val").append("Bu mail zaten kullanılmış.").attr("style", "color:red !important; font-weight:400;");
			}
			else {
				$(".mail-input-val").append("Mesajınız gönderilemedi.").attr("style", "color:red !important; font-weight:400;");
			}
		},
		error: function (error) {
			console.log("error : ", error.statusText);
		}
	});
});

$(".nav-link-off").on("click", function () {
	$(".nested-dropdown-first").toggleClass("d-block");
})
$(".sub-1").on("click", function (event) {
	$(".dropdown-sub-first").toggleClass("d-block");
})
$(".sub-2").on("click", function (event) {
	$(".dropdown-sub-second").toggleClass("d-block");
})
$(".sub-3").on("click", function (event) {
	$(".dropdown-sub-third").toggleClass("d-block");
})


// Tüm mini görüntüleri seç
var miniImages = document.querySelectorAll('.product-detail-img-list img');

// Büyük görüntüyü seç
var bigImage = document.querySelector('.product-detail-img-box img');

// Her mini görüntüye olay dinleyici ekle
miniImages.forEach(function (miniImage) {
	miniImage.addEventListener('click', function () {
		var bigImageSrc = bigImage.src;
		var miniImageSrc = miniImage.src;
		bigImage.src = miniImageSrc
		miniImage.src = bigImageSrc;
	});
});