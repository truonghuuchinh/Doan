
const connection = new signalR.HubConnectionBuilder().withUrl("/MessageChat").build();
connection.start().then(function () {

}).catch(function (err) {
   
});
connection.on("receiveMessage", function (message) {
    if (message.watched) {
        $(".delete-" + message.id).remove();
        if (senderIds == message.senderId)
             alertSuccess("Đã xóa");
    } else addMessageToChat(message);
   
});

connection.onclose(function (e) {
    connection.start();
});

function sendMessageToHub(messages) {
    connection.invoke("SendMessage",messages).catch(function (err) {
        
    });;
}

