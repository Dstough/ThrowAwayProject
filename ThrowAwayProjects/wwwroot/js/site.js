BindValidation = function (formControl, targetControls, submitControl, failureCriteria = [""]) {
    $("#" + submitControl).on("click", function (e) {
        var pass = true;
        for (i in targetControls) {
            for (j in failureCriteria) {
                if ($("#" + targetControls[i]).val() == failureCriteria[j]) {
                    e.preventDefault();
                    pass = false;
                    $("#" + targetControls[i]).css("box-shadow", "0 0 10px red");
                    $("#" + targetControls[i]).on("focus", function () {
                        $(this).css("box-shadow", "inset 0 0 7px #000");
                    });
                    $("#" + targetControls[i]).on("mouseover", function () {
                        $(this).css("box-shadow", "inset 0 0 7px #000");
                    });
                }
            }
        }
        if(pass){
            $("#modal").fadeOut(300, function(){
                $("#" + formControl).submit();
            });
        }
    });
}

var DisplayMessage = function (message, signature) {
    if (signature === undefined)
        signature = "-- Fastjack";

    $("#message").html(
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

$(function () {
    setTimeout(function () {
        $("#errorMessage").fadeOut(1000, function () {
            $("#errorMessage").css("display", "none");
        });
    }, 3000);
});