var DisplayMessage = function (message, signature) {
    if (signature === undefined)
        signature = "Fastjack";

    $("#message").html("<span class='message-body'>" + message + "</span><div class='message-signature admin-color'>-- " + signature + "</div>");
    $("#message").css("display", "inherit");

    setTimeout(function () {
        $("#message").fadeOut(1000, function () {
            $("#message").css("display", "none");
        });
    }, 3000);
}

var RenderPageNavigation = function (url, pageNumber, tag) {
    if (pageNumber === undefined || tag === undefined || url === undefined)
        return;

    var html = ' Page ' + (pageNumber + 1) + ' [<a href="' + url + '/' + (pageNumber + 1) + '">Next</a>]';

    if (pageNumber > 0)
        html = '[<a href="' + url + '/' + (pageNumber - 1) + '">Previous</a>] ' + html;

    $("#" + tag).html(html);
}