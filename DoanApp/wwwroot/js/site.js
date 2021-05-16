function OpenPopUp(title,url) {
    $.get(url, function (respone) {
        $("#ModalTitle").text(title);
        $("#ModalContent").html(respone);
        $("#myModal").modal("show");
    });
}