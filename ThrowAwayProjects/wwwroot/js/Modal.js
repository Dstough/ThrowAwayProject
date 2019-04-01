var Modal = function (action) {
    $.ajax({
        url: action,
        method: 'GET',
        success: function (result) {
            if (result["result"] == "modal") {
                $(".modal-content").html(result["message"]);
                $("#modal").modal({ backdrop: 'static' });
            }
        }
    });
}

var Submit = function (action, data, tag = "") {
    $.ajax({
        url: action,
        method: 'POST',
        data: data,
        success: function (result) {
            var signature = result["signature"];
            if (signature === undefined)
                signature = "-- Fastjack";
            $("#message").html(
                "<span class='message-body'>" + result["message"] + "</span>" +
                "<div class='message-signature admin-color'>" + signature + "</div>"
            );
            $("#message").css("display", "inherit");
            setTimeout(function () {
                $("#message").fadeOut(1000, function () {
                    $("#message").css("display", "none");
                });
            }, 3000);
            if (tag !== "" && result["html"] !== undefined) {
                $("#" + tag).html(result["html"]);
            }
        }
    });
}