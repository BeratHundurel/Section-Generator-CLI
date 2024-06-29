(function () {
    'use strict';
    window.addEventListener('load', function () {
        // Fetch all the forms we want to apply custom Bootstrap validation styles to
        var forms = document.getElementsByClassName('needs-validation');
        // Loop over them and prevent submission
        var validation = Array.prototype.filter.call(forms, function (form) {
            form.addEventListener('submit', function (event) {
                if (form.checkValidity() === false) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
        });
    }, false);
})();



/*---------------------------
     IS ROOT PAGE
 * -------------------------*/

$(document).on('click', '.checkbox_command__btn', function () {
    var $this = $(this);
    var dataUrl = $(this).data("url");
    var itemValue = $(this).data("value");
    var formData = new FormData();
    formData.append("id", $(this).data("id"));
    formData.append("value", itemValue);

    $.ajax({
        url: dataUrl,
        type: "POST",
        processData: false,
        contentType: false,
        data: formData
    }).done(function (data) {
        $('input[type=checkbox].checkbox_command__btn').prop('checked', false);
        $this.prop('checked', true);
    });
});

/*---------------------------
     IS ROOT PAGE
 * -------------------------*/

$(".isroot_command__span").on("click", function () {
    var ajaxUrl = $(this).data("url");
    var id = $(this).data("id");

    var formData = new FormData();
    formData.append("itemId", id);
    console.log(ajaxUrl);
    console.log(id);
    $.ajax({
        url: ajaxUrl,
        type: "POST",
        data: formData,
        dataType: "json",
        contentType: false,
        processData: false,
    }).done(function (data) {
        if (data == "ok") {
            swal("İyi iş!", "Sayfa ana sayfa olarak değiştirildi!", "success");
            setTimeout(
                function () {
                    location.reload();
                }, 1000);
        }
    }).fail(function () {
        swal("Tüh!", "Tekrar deneyiniz!", "error");
    });
});

function ChangePublishBtnStatus(status, button) {
    button.addClass("d-none");
    if (status == true) {
        //published
        button.parent().find(".deactivate_button").removeClass("d-none");
    }
    else {
        //paused
        button.parent().find(".activate_button").removeClass("d-none");
    }
}

const alertDisplayingTime = 500;

$(document).on('click', '.list_publish__btn', function () {
    var itemId = $(this).data("id");
    var ajaxUrl = $(this).data("url");
    var $this = $(this);

    swal("Aktiflik durumunu değiştireceksiniz. Emin misiniz?", {
        buttons: {
            cancel: "Geri Dön",
            catch: {
                text: "Yayınla",
                value: "catch",
                closeModal: false,
            },
            pause: {
                text: "Duraklat",
                value: "pause",
                closeModal: false,
            },
        },
    })
        .then((value) => {
            switch (value) {
                case "pause":
                    var formData = new FormData();
                    formData.append("itemId", itemId);
                    formData.append("enabled", false);

                    $.ajax({
                        url: ajaxUrl,
                        method: 'POST',
                        data: formData,
                        dataType: "json",
                        processData: false,
                        contentType: false,
                        success: function (data) {
                            if (data == "no") {
                                swal("Owww!", "Durum pasifleştirilemedi. Lütfen daha sonra tekrar deneyin!", "error");
                            }
                            else {
                                swal("Heey", "Durum pasifleştirildi.", "success");
                                setTimeout(function () {
                                    swal.close();
                                    ChangePublishBtnStatus(false, $this);
                                }, alertDisplayingTime);
                            }
                        },
                        error: function () {
                            swal("Owww!", "Durum pasifleştirilemedi. Lütfen daha sonra tekrar deneyin!", "error");
                        }
                    });
                    break;

                case "catch":

                    //Ajax
                    var formData = new FormData();
                    formData.append("itemId", itemId);
                    formData.append("enabled", true);

                    $.ajax({
                        url: ajaxUrl,
                        method: 'POST',
                        data: formData,
                        dataType: "json",
                        processData: false,
                        contentType: false,
                        success: function (data) {
                            if (data == "no") {
                                swal("Owww!", "Durum aktifleştirilemedi. Lütfen daha sonra tekrar deneyin!", "error");
                            }
                            else {
                                swal("Harika!", "Durum aktifleştirildi!", "success");
                                setTimeout(function () {
                                    swal.close();
                                    ChangePublishBtnStatus(true, $this);
                                }, alertDisplayingTime);
                            }
                        },
                        error: function () {
                            swal("Owww!", "Durum aktifleştirilemedi. Lütfen daha sonra tekrar deneyin!", "error");
                        }
                    });

                    break;

                default:
                    swal("Görüşürüz!");
                    setTimeout(function () { swal.close(); }, 2000);
            }
        });

});

$(".list_delete__btn").on("click", function () {
    const itemId = $(this).data("id");
    var ajaxUrl = $(this).data("url");
    var $this = $(this);
    const swalText = $(this).data("text") == null ? "Bu veriyi silmeniz ilişkili olan birçok veriyi de etkileyecektir. Verileri emin olmadığınız sürece silmeyiniz! Gerçekten silmek istediğinize emin misiniz?" : $(this).data("text");

    swal(swalText, {
        buttons: {
            cancel: "İptal Et",
            catch: {
                text: "Silme İşlemine Devam Et!",
                value: "catch",
                closeModal: false,
            }
        },
    })
        .then((value) => {
            switch (value) {
                case "catch":
                    //Other

                    if (confirm(swalText) == true) {
                        const formData = new FormData();
                        formData.append("id", itemId);
                        $.ajax({
                            url: ajaxUrl,
                            type: "POST",
                            processData: false,
                            contentType: false,
                            data: formData,
                            success: function (data) {
                                if (data == "success") {
                                    swal("Seçtiğiniz veri veritabanından silindi!", {
                                        icon: "success",
                                    });
                                    setTimeout(function () {
                                        swal.close();
                                        $this.closest("tr").remove();
                                        $this.closest(".media_box__container").remove();
                                    }, 250);
                                }
                                else {
                                    swal("Seçtiğiniz veri silinemedi!", {
                                        icon: "error",
                                    });
                                    setTimeout(function () { swal.close(); }, 250);
                                }

                            },
                            error: function () {
                                swal("Seçtiğiniz veri silinemedi!", {
                                    icon: "error",
                                });
                                setTimeout(function () { swal.close(); }, 250);
                            }
                        });
                    }

                    break;

                default:
                    swal("Görüşürüz!");
                    setTimeout(function () { swal.close(); }, 250);
            }
        });
});


/*PANEL SETTING*/

$("#WrapperToggled").on("click", function () {
    $(".wrapper").toggleClass("toggled");
})



$('#sidebarCollapse').on('click', function () {
    $('#sidebar').toggleClass('active');
    $(this).toggleClass('active');
});

var formData1 = new FormData();

$.ajax({
    url: "/admin/home/GetPanelChoiceCookie",
    type: "POST",
    processData: false,
    contentType: false,
    dataType: 'json',
    data: formData1
}).done(function (data) {
    if (data.lang != 0) {
        $("#LanguageCookie").val(data.lang);
    }
    else {
        console.log(data.lang);
        swal("Lütfen panelde değişiklik yapmak için dil ve web sitesi seçiniz");
        setTimeout(function () { window.location.href = "/admin/home/choice" }, 2000);
    }
});

$(".language_dropdown_item").on("click", function () {
    var formData = new FormData();
    formData.append("langId", $(this).data("id"));

    $.ajax({
        url: "/admin/home/SetPanelChoiceCookie",
        type: "POST",
        processData: false,
        contentType: false,
        dataType: 'json',
        data: formData
    }).done(function (data) {
        if (data == "success") {
            swal("Geçiş Yapıldı.");
            if (window.location.pathname.includes("manage") && window.location.search != null) {
                window.location.href = "/admin/home/welcome";
            }
            else {
                setTimeout(function () { window.location.reload(); }, 250);
            }
        }
    });
});
$(".website_dropdown_item").on("click", function () {
    var formData = new FormData();
    formData.append("langId", $("#LanguageCookie").children("option:selected").val());
    $.ajax({
        url: "/admin/home/SetPanelChoiceCookie",
        type: "POST",
        processData: false,
        contentType: false,
        dataType: 'json',
        data: formData
    }).done(function (data) {
        if (data == "success") {
            swal("Geçiş Yapıldı.");
            if (window.location.pathname.includes("manage") && window.location.search != null) {
                window.location.href = "/admin/home/welcome";
            }
            else {
                setTimeout(function () { window.location.reload(); }, 250);
            }
        }
        else {
            swal("Lütfen doğru veri seçiniz.");
            setTimeout(function () { window.location.reload(); }, 250);
        }
    });
});

//AdminAction List JS (Controller ve Servis yardımıyla filtreleniyor)
$(".filter-btn").click(function () {

    var name = $('.filter-select-name').val();
    var process = $('.filter-select-process').val();

    $.ajax({
        url: "FilterList",
        type: "POST",
        data: { name: name, process: process },
        success: function (response) {
            $(".filter-table").remove();
            $(".append-filter").append(response);
        },

    });

});
//AdminAction List JS

//AdminLogin List JS  (Controller ve Servis yardımıyla filtreleniyor)
$(".filter-btn").click(function () {
    var lang = $('.filter-select-web').val();
    var web = $('.filter-select-lang').val();

    $.ajax({
        url: "FilterList",
        type: "POST",
        data: { lang: lang, web: web },
        success: function (response) {
            $(".filter-table").remove();
            $(".append-filter").append(response);
        },

    });


});
//AdminLogin List JS



//Javascript ile Filtreleme (İsteğe göre veriler javascriptde filtreleniyor)
//$(".filter-option").click(function () {
//    var letter = $(this).attr("data-title");
//    if (letter === 'ALL') {
//        $('tr').show();
//    }
//    else {
//        $('.table-item').each(function (rowIdx, tr) {
//            $(this).hide().find('td').each(function (idx, td) {
//                if (idx === 0 || idx === 1) {
//                    var check = $(this).text();
//                    if (check && check.indexOf(letter) == 0) {
//                        $(tr).show();
//                    }
//                }
//            });

//        });
//    }
//});


//sag taraftaki formu acan buton



