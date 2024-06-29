$(".header-lang").on("click", function () {
    console.log("girdi");
    $this = $(this);
    var langId = $this.attr("data-langId");
    console.log("girdi", langId);

    $.ajax({
        type: "POST",
        url: "/Action/ChangeCookieLang",
        data: { LangId: langId },
        success: function (data) {
            window.location.href = "/" + data;
        },
        error: function (error) {
            console.log("error: " + error);
        },
    });
});