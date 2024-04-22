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
        } else if (fileValue.length === 0) {
            $("#url_checked").prop("disabled", false)
        }
    })
    $("#url").on("input", () => {
        let urlValue = $("#url").val().trim()
        if (urlValue.length > 0) {
            $("#url_checked").prop("disabled", true)
        } else if (urlValue.length === 0) {
            $("#url_checked").prop("disabled", false)
        }
    })

    $("#formUrl").on("input", () => {
        let form_url = $("#formUrl").val().trim()
        console.log(form_url)
        if (form_url.length > 0) {
            $("#submit").prop("disabled", false)
        } else {
            $("#submit").prop("disabled", true)
        }
    })
})