// icon picker options
var iconPickerOptions = { searchText: "Buscar...", labelHeader: "{0}/{1}" };
// sortable list options
var sortableListOptions = {
    placeholderCss: { 'background-color': "#cccccc" }
};
var editor = new MenuEditor('myEditor',
    {
        listOptions: sortableListOptions,
        iconPicker: iconPickerOptions,
        maxLevel: 2 // (Optional) Default is -1 (no level limit)
        // Valid levels are from [0, 1, 2, 3,...N]
    });
editor.setForm($('#frmEdit'));
editor.setUpdateButton($('#btnUpdate'));
//Calling the update method
$("#btnUpdate").click(function () {
    editor.update();
});
// Calling the add method
$('#btnAdd').click(function () {
    editor.add();
});

var str = editor.getString();
$("#myTextarea").text(str);

$('#btnOutput').on('click', function () {
    var str = editor.getString();
    $("#out").text(str);
});

$('#btnSave').on('click', function () {
    var $this = $(this);
    $this.addClass("disabled");

    var str = editor.getString();
    var formData = new FormData();
    formData.append("str", str);

    $.ajax({
        url: "/admin/menu/setStringJson",
        type: "POST",
        processData: false,
        contentType: false,
        data: formData,
        success: function (data) {
            $this.removeClass("disabled");
            if (data == "success") {
                alert("Basarılı")
            }
        },
        error: function () {
            $this.removeClass("disabled");
            alert("HATA!")
        }
    });

});
/* ====================================== */

