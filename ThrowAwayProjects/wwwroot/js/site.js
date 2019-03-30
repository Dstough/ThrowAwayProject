$(function () {
    BindValidation = function (targetControls, submitControl, failureCriteria = [""]) {
        $("#" + submitControl).on("click", function (e) {
            for (i in targetControls) {
                for (j in failureCriteria) {
                    if ($("#" + targetControls[i]).val() == failureCriteria[j]) {
                        e.preventDefault();
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
        });
    }
});