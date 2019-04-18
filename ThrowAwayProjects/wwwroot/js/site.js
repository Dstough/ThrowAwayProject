var DisplayMessage = function (message, signature) {
    if (signature === undefined)
        signature = "-- Fastjack";

    $("#message").html(
        "<hr/>" +
        "<span class='message-body'>" + message + "</span>" +
        "<div class='message-signature admin-color'>" + signature + "</div>"
    );

    $("#message").css("display", "inherit");

    setTimeout(function () {
        $("#message").fadeOut(1000, function () {
            $("#message").css("display", "none");
        });
    }, 3000);
}