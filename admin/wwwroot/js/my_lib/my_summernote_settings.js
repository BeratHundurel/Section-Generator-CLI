var summernoteSettings = {
    height: 250,
    placeholder: '...',
    colors: [["#000000", "#ffffff", "#1f3960", "#8a8d8f", "#FFDA00", "#7F4A20", "#E5F1FB", "#E81123", "#40568D", "#6CE26C", "#1D7C8C"]],
    toolbar: [
        ['style', ['style', 'bold', 'italic', 'underline']],
        ['color', ['color']],
        ['para', ['ul', 'ol', 'paragraph']],
        ['image', ['link', 'picture', 'video', 'table']],
        ['misc', ['undo', 'redo', 'codeview']]
    ],
    dialogsInBody: true,
    lang: 'tr-TR'
}

$(".summernote_textarea").summernote(summernoteSettings);
$(".summernote_textarea__required").summernote(summernoteSettings);

$('form').on('submit', function (e) {
    console.log($(".summernote_textarea__required").length)
    if ($(".summernote_textarea__required").length > 0) {
        if ($('.summernote_textarea__required').summernote('isEmpty')) {
            alert('Açıklama alanlarını doldurun.');
            // cancel submit
            e.preventDefault();
        }
        else {
            // do action
        }
    }
})