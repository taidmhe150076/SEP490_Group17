$(() => {
    $("#submit").prop("disabled", true)
    $("#checked_label").on("click", (event) => {
        var isChecked = $("#url_checked").prop("checked")
        if (isChecked) {
            $("#url-control").addClass("d-none");
            $("#key-control").removeClass("d-none");
            $("#checked_label").text("File input")
        } else {
            $("#key-control").addClass("d-none");
            $("#url-control").removeClass("d-none");
            $("#checked_label").text("Google Sheet URL")
        }
    })
    $("#key").on("change", () => {
        let fileValue = $("#key").val().trim()
        var isChecked = $("#url_checked")
        console.log(fileValue)
        if (fileValue.length > 0) {
            $("#url_checked").prop("disabled", true)
            $("#submit").prop("disabled", false)
        } else if (fileValue.length === 0) {
            $("#url_checked").prop("disabled", false)
            $("#submit").prop("disabled", true)
        }
    })
    $("#url").on("input", () => {
        let urlValue = $("#url").val().trim();
        console.log(urlValue)
        if (urlValue.length > 0) {
            $("#url_checked").prop("disabled", true)
            $("#submit").prop("disabled", false)
        } else if (urlValue.length === 0) {
            $("#url_checked").prop("disabled", false)
            $("#submit").prop("disabled", true)
        }
    })
})