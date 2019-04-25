var Modal = function (action) {
    $.ajax({
        url: action,
        method: 'GET',
        success: function (result) {
            if (result["result"] == "modal") {
                $(".modal-content").html(result["message"]);
                $("#modal").modal({ backdrop: 'static' });
            }
            if (result["result"] == "error") {
                DisplayMessage(result["message"]);
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
            $('#modal').modal('hide');

            if (result["message"] !== undefined && result["signature"] !== undefined) {
                DisplayMessage(result["message"], signature);
            }

            if (tag !== "" && result["html"] !== undefined) {
                $("#" + tag).html(result["html"]);
            }
        }
    });
}

var VerifyDelete = function (submitControl, action, tag = "", message = "Are you sure you want to delete this item?") {
    $("#" + submitControl).on("click", function (e) {
        if (confirm(message)) {
            $.ajax({
                url: action,
                method: 'POST',
                success: function (result) {
                    $('#modal').modal('hide');

                    if (result["message"] !== undefined && result["signature"] !== undefined) {
                        DisplayMessage(result["message"], signature);
                    }

                    if (tag !== "" && result["html"] !== undefined) {
                        $("#" + tag).html(result["html"]);
                    }
                }
            });
        }
    });
}