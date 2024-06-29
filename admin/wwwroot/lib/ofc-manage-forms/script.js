
function populate(frm, data) {
    $.each(data, function (key, value) {
        key = capitalize(key)
        var ctrl = frm.find('[name=' + key + ']');
        switch (ctrl.prop("type")) {
            case "radio": case "checkbox":
                ctrl.each(function () {
                    if ($(this).attr('value') == value) $(this).attr("checked", value);
                });
                break;
            default:
                ctrl.val(value);
        }
        if (ctrl.hasClass("textarea")) {
            tinymce.activeEditor.setContent(value);
        }
    });
}

$("body").delegate(".manage_form__modal-btn", "click", function () {
    const $this = $(this);
    const getHref = $this.data("href");
    const modalId = $this.data("modalid");
    var modalClassName = ".manage_form__modal_0";
    if (modalId != null) {
        modalClassName = ".manage_form__modal_" + modalId;
    }
    const $modal = $(modalClassName);

    if (getHref != null) {
        $.ajax({
            type: "GET",
            url: getHref,
            success: function (data) {
                console.log(data)
                populate($modal.find("form"), data);
                $modal.modal("show");
            }
        });
    }
    else {
        $modal.modal("show");
    }
});

$(".manage_form__modal_save-btn").on("click", function () {
    var $this = $(this);
    $this.closest(".manage_form__modal").find("form").submit();
});

function capitalize(word) {
    return $.camelCase("-" + word);
}

$(".sortable_list").sortable({
    group: 'no-drop',
    handle: 'i.icon-move',
    onDragStart: function ($item, container, _super) {
        // Duplicate items of the no drop area
        if (!container.options.drop)
            $item.clone().insertAfter($item);
        _super($item, container);
    }
});

