class Message {
    constructor(id,senderId,receiverId,watched) {
        this.senderId = senderId;
        this.recevicerId = receiverId;
        this.id = id;
        this.watched = watched;
    }
}


var textInput = '';

function clearInput() {
    textInput = $("#contentMessage").val();
    $(".au-input--full").val('');
}
function sendMessage() {
    var message = new Message();
    message.senderId = senderIds;
    message.receiverId = receiverIds;
    sendMessageToHub(message);

}
function addMessageToChat(message) {
    if (senderIds != 0) {
        if (user__Logins == message.senderId) {
            $.get("/Message/Message_Partial/?senderId=" + senderIds + "&receiverId=" + receiverIds + "&flag=true", function (respone) {
                $(".au-chat__content").append(respone);
                $(".au-input-icon").prop('hidden', true);
            });
        } else {
            if (message.receiverId == senderIds && message.senderId == receiverIds) {
                RevertWatched(senderIds, receiverIds, true, true);
                $.get("/Message/Message_Partial/?senderId=" + senderIds + "&receiverId=" + receiverIds + "&flag=true", function (respone) {
                    $(".au-chat__content").append(respone);
                    $(".au-input-icon").prop('hidden', true);
                });
            } else {
                RevertWatched(message.receiverId, message.senderId, false, true);
            }
        }
       
    } else {
        RevertWatched(user__Logins, 0, false, true);
        $.get("/Message/Message_Partial/?senderId=" + senderIds + "&receiverId=" + receiverIds + "&flag=true", function (respone) {
            $(".au-chat__content").append(respone);
            $(".au-input-icon").prop('hidden', true);
        });
    }
        
}
function RevertWatched(sender, receiver, flag,checkParent,loadMessage=false) {
    var checkCounts = document.querySelector(".check-" + senderIds + "-" + receiverIds);
    $.post("/Message/UpdateWatched", { senderId: sender, receiverId: receiver, flag: flag }, function (respone) {
        countChange = respone;
        if (respone > 0 && !flag) {
            $(".message__count").addClass("displayCount");
            $(".message__count").text(respone);
            if (parseInt($(".message__count").text()) >= 10)
                $(".message__count").css("padding-left", "4px");
        }
        if (loadMessage) {
            var count = parseInt($(".message__count").text()) - respone;
            if (count <= 0) {
                count = 0;
                $(".message__count").removeClass("displayCount");
            }
            $(".message__count").text(count);
        } else $(".message__count").text(respone);
        if ((checkCounts != null || checkCounts != undefined) && !checkParent) {
            RevertWatched(senderIds, receiverIds, true,true);
            $(".message__count").text(0);
            $(".message__count").removeClass("displayCount");
        }
    });
}
