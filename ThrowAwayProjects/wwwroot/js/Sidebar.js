var FetchNewRandomPost = function (action, tag) {
    setInterval(
        function () {
            $("#" + tag).fadeOut(1000, function () {
                try {
                    $.ajax({
                        url: action,
                        method: 'GET',
                        success: function (result) {
                            $("#" + tag).attr('onclick', "location.href='" + result["url"] + "'")
                            $("#" + tag).html(
                                "<span class='message-body'>" + result["message"] + "</span>" +
                                "<div class='message-signature " + result["css"] + "'>-- " + result["signature"] + "</div>"
                            );
                        }
                    });
                }
                catch (err) {
                    DisplayMessage(err.message);
                }
                $("#" + tag).fadeIn(1000);
            });
        }, 10000
    );
}