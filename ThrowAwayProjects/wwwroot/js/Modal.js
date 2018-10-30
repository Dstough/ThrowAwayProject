var Modal = function(action){
    $.ajax({
        url: action,
        method: 'GET',
        success: function(result){
            $(".modal-content").html(result["message"]);
            $("#modal").modal({backdrop:'static'});
        }
    });
}
