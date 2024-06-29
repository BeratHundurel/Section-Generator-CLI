$(document).ready(function () {
    $(".summernote").summernote(summernoteSettings);

    // Toolbar extra buttons
    var btnFinish = $('<button class="btn_save"></button>').text('Kaydet').addClass('btn btn-success bg-success').on('click', function () {
    });
    // Step show event
    $("#smartwizard").on("showStep", function (e, anchorObject, stepNumber, stepDirection, stepPosition) {
        $("#prev-btn").removeClass('disabled');
        $(".btn_save").addClass('d-none');
        $("#next-btn").removeClass('disabled');
        if (stepPosition === 'first') {
            $("#prev-btn").addClass('disabled');
        } else if (stepPosition === 'last') {
            $(".btn_save").removeClass('d-none');
            $("#next-btn").addClass('disabled');
        } else {
            $("#prev-btn").removeClass('disabled');
            $("#next-btn").removeClass('disabled');
        }
    });
    // Smart Wizard
    $('#smartwizard').smartWizard({
        selected: 0,
        theme: 'dots',
        lang: { // Language variables for button
            next: 'İleri',
            previous: 'Geri',
        },
        keyboardSettings: {
            keyNavigation: false, // Enable/Disable keyboard navigation(left and right keys are used if enabled)
        },
        toolbarSettings: {
            toolbarExtraButtons: [btnFinish]
        },
        onFinish: function () {
            $("form").submit();
        }
    });

    $("#smartwizard").on("stepContent", function (e, anchorObject, stepIndex, stepDirection) {
        var sectionsHtml = $("#sectionsHtml");
        var sectionJsonValueInput = $(".sectionsJsonData");
        JsonSerialize(sectionsHtml, sectionJsonValueInput);

        var slidersHtml = $("#slidersHtml");
        var sliderJsonValueInput = $(".slidersJsonData");
        JsonSerialize(slidersHtml, sliderJsonValueInput);

        if (stepDirection == 'forward') {
            console.log(anchorObject)
        }

    });
});


$("body").delegate(".section_item__header", "click", function () {
    var $this = $(this);
    var thisSecItem = $this.closest(".section_item");
    thisSecItem.find(".section_item__content").toggleClass("show");
});


$(document).ready(function () {
    setTimeout(function () { $(".page_overlay").hide(); }, 500);
});

//Json serialize plugis is required.

function JsonSerialize(jsonHtml, jsonValueInput) {
    var jsonData = "";
    jsonHtml.children('div').each(function (i) {
        jsonData += $(this).find("input,textarea,select").serializeJSON() + ",";
    });
    jsonValueInput.val(jsonData);
}

$('.section_add__btn').on('click', function (evt) {
    var $this = $(this);
    evt.preventDefault();
    evt.stopPropagation();
    $.get($(this).data('url'), function (data) {
        var sectionsHtml = $this.closest(".section_tab_pane").find(".sections_html");
        sectionsHtml.append(data);
        sectionsHtml.find(".section_item .section_item__content").removeClass("show");
        sectionsHtml.find(".section_item .section_item__content:last").addClass("show");

        orderElements();
        orderSlidersElements();

        $this.closest(".section_tab_pane").find(".sectionsInfo").hide();
        //$('.summernote').summernote(summernoteSettings);
        //$('.search_select').selectpicker();
    });

});

$(document).on('click', '.section_item_remove', function (evt) {
    $(this).closest(".section_item").remove();
    orderElements();
    orderSlidersElements();
});

var removeLastChar = function (value, char) {

    if (!value || !value.length) { return; }

    var lastChar = value.slice(-1);
    if (lastChar == char) {
        value = value.slice(0, -1);
    }
    return value;
}

var splitImages = function (value, char) {

    if (!value || !value.length) { return; }

    if (value.indexOf(char) > 0)
        return removeLastChar(value, char).split(',');
    else
        return removeLastChar(value, char);
}

var removeLastComma = function (value) {
    return value.replace(/,\s*$/, "");
}

function changeTitle(obj) {
    setTimeout(function () {
        $(obj).closest('.section_item').find(".collapse_button_span").html($(obj).val());
    }, 250);
}

function orderElements() {
    $('#sectionsHtml').children('div').each(function () {
        $(this).find("input[name='order']").val($(this).index());
        $(this).attr("data-order", $(this).index());
    });
}

function orderSlidersElements() {
    $('#slidersHtml').children('div').each(function () {
        $(this).find("input[name='order']").val($(this).index());
        $(this).attr("data-order", $(this).index());
    });
}

function sortJSON(data, key, way) {
    return data.sort(function (a, b) {
        var x = a[key]; var y = b[key];
        if (way === 'asc') { return ((x < y) ? -1 : ((x > y) ? 1 : 0)); }
        if (way === 'desc') { return ((x > y) ? -1 : ((x < y) ? 1 : 0)); }
    });
}

jQuery.fn.sortDivs = function sortDivs() {
    $("> div", this[0]).sort(dec_sort).appendTo(this[0]);
    function dec_sort(a, b) { return ($(b).data("order")) < ($(a).data("order")) ? 1 : -1; }
}


function convertToSlug(Text) {
    return Text
        .toLowerCase()
        .replace(/[^\w ]+/g, '')
        .replace(/ +/g, '-');
}

var slug = function (str) {
    str = str.replace(/^\s+|\s+$/g, ''); // trim
    str = str.toLowerCase();

    // remove accents, swap ñ for n, etc
    var from = "ÁÄÂÀÃÅČÇĆĎÉĚËÈÊẼĔȆÍÌÎÏŇÑÓÖÒÔÕØŘŔŠŤÚŮÜÙÛÝŸŽáäâàãåčçćďéěëèêẽĕȇíìîïňñóöòôõøðřŕšťúůüùûýÿžþÞĐđßÆa·/_,:;";
    var to = "AAAAAACCCDEEEEEEEEIIIINNOOOOOORRSTUUUUUYYZaaaaaacccdeeeeeeeeiiiinnooooooorrstuuuuuyyzbBDdBAa------";
    for (var i = 0, l = from.length; i < l; i++) {
        str = str.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i));
    }

    str = str.replace(/[^a-z0-9 -]/g, '') // remove invalid chars
        .replace(/\s+/g, '-') // collapse whitespace and replace by -
        .replace(/-+/g, '-'); // collapse dashes

    return str;
};



var PortletDraggable = function () {

    return {
        init: function () {
            if (!jQuery().sortable) {
                return;
            }

            $(".sortable").sortable({
                connectWith: ".section_item",
                items: ".section_item",
                opacity: 0.8,
                handle: '.section_item__header',
                coneHelperSize: true,
                forcePlaceholderSize: true,
                tolerance: "pointer",
                helper: "clone",
                tolerance: "pointer",
                forcePlaceholderSize: !0,
                helper: "clone",
                revert: 250, // animation in milliseconds
                update: function (b, c) {
                    if (c.item.prev().hasClass("portlet-sortable-empty")) {
                        c.item.prev().before(c.item);
                    }

                    orderElements();
                }
            });
        }
    };
}();

jQuery(document).ready(function () {
    PortletDraggable.init();
});