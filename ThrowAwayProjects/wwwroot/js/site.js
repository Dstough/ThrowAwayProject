$(function () {
    $(".fade-in").fadeIn();
});

var converter = new showdown.Converter({
    tables: true,
    strikethrough: true,
});

var DisplayMessage = function (message, signature) {
    if (signature === undefined)
        signature = "Fastjack";

    $("#message").html("<span class='message-body'>" + message + "</span><div class='message-signature' style='color:#ff0'>" + signature + "</div>");
    $("#message").css("display", "inherit");

    setTimeout(function () {
        $("#message").fadeOut(1000, function () {
            $("#message").css("display", "none");
        });
    }, 3000);
}

var RenderPageNavigation = function (url, pageNumber, recordCount, maxRecordCount, tag) {
    if (pageNumber === undefined || tag === undefined || url === undefined || recordCount === undefined)
        return;

    var html = '';

    if (pageNumber > 1)
        html += '<a href="' + url + '/' + (pageNumber - 1) + '">Previous</a> ';

    html += ' Page ' + (pageNumber);

    if (recordCount == maxRecordCount)
        html += ' <a href="' + url + '/' + (pageNumber + 1) + '">Next</a>';

    $("#" + tag).html(html);
}