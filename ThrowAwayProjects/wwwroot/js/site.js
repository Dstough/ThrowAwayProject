﻿BindModalValidation = function (formControl, targetControls, submitControl, submitCommand, failureCriteria = [""]) {
    $("#" + submitControl).on("click", function (e) {

        var pass = true;

        for (i in targetControls) {
            for (j in failureCriteria) {
                if ($("#" + targetControls[i]).val() == failureCriteria[j]) {
                    e.preventDefault();
                    AddRedGlow(targetControls[i])
                    pass = false;
                }
            }
        }

        if (pass) {
            $("#modal").fadeOut(300, function () {
                if (submitCommand !== undefined)
                    submitCommand();
                else
                    $("#" + formControl).submit();
            });
        }
    });
}

BindValidation = function (targetControls, submitControl, failureCriteria = [""]) {
    $("#" + submitControl).on("click", function (e) {

        for (i in targetControls) {
            for (j in failureCriteria) {
                if ($("#" + targetControls[i]).val() == failureCriteria[j]) {
                    e.preventDefault();
                    AddRedGlow(targetControls[i]);
                }
            }
        }
    });
}

var AddRedGlow = function (targetControl) {

    $("#" + targetControl).css("box-shadow", "0 0 10px red");

    $("#" + targetControl).on("focus", function () {
        $(this).css("box-shadow", "inset 0 0 7px #000");
    });

    $("#" + targetControl).on("mouseover", function () {
        $(this).css("box-shadow", "inset 0 0 7px #000");
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