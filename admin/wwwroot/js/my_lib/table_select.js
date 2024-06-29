$(function () {
    $('.checkAll').click(function () {
        if (this.checked) {
            $(".checkboxes").prop("checked", true);
        } else {
            $(".checkboxes").prop("checked", false);
        }
    });

    $(".checkboxes").click(function () {
        var numberOfCheckboxes = $(".checkboxes").length;
        var numberOfCheckboxesChecked = $('.checkboxes:checked').length;
        if (numberOfCheckboxes == numberOfCheckboxesChecked) {
            $(".checkAll").prop("checked", true);
        } else {
            $(".checkAll").prop("checked", false);
        }
    });
});