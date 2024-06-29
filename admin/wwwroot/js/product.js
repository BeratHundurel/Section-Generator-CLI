$('#cat-select').change(function () {
    var categoryId = parseInt($(this).val());
    console.log(typeof categoryId);
    if (categoryId) {
        console.log("if'egirdi");
        console.log(typeof categoryId);

        $.ajax({
            url: '/admin/product/GetSubCategories', 
            type: 'POST',
            //processData: false,
            //contentType: false,
            data: { categoryId: categoryId }, 
            success: function (response) {
                
                var subcategories = $("#subcat-select");
                subcategories.empty(); 
                $.each(response, function (index, item) {
                    subcategories.append($('<option></option>').attr('value', item.value).text(item.text));
                });
            },
            error: function (response) {
                console.log("Alt kategoriler getirilirken bir hata oluştu.");
            }
        });
    } else {

        $("#subcat-select").empty();
    }
});
$('#mancat-select').change(function () {
    // Erkek kategorisi seçildiğinde
    if ($(this).val() !== "") {
        // CategoryId ve SubCategoryId değerlerini null yap
        $('#cat-select').val("");
        $('#subcat-select').val("");
    }
});