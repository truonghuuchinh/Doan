//---------------Process chat application----------//
var senderIds = 0;
var receiverIds = 0;
var countChange = 0;

//Xử lý loading
function imgLoading() {
    $(".img-loading").css("display", "block");
}
//------------------------Xử lý Update and Delete Message---------//
function deleteMessage(id) {
    var ms = new Message();
    ms.id = id;
    ms.watched = true;
    ms.receiverId = receiverIds;
    ms.senderId = senderIds;
    $.post("/Message/Delete", { id: id }, function (respone) {
        if (respone == "Success") {
            sendMessageToHub(ms);
        }
    });
}
/*function updateMessage(id) {

}*/
//------------------------Kết thúc--------------------------------//
//------------------------Xử lý cập nhật đếm tin nhắn--------------//
var user__Logins = $("#user_authenticated").val();
if (user__Logins != '') {

    $.get("/Message/GetNotifyMessage/?id=" + user__Logins, function (respone) {
        if (respone != 0) {
            $(".message__count").text(respone);
            $(".message__count").addClass("displayCount");
        }
    });
}

//-------------------------------Kết thúc--------------------------//

function contactUser(senderid, receiverid) {
    var model = document.getElementById("leave_the_check_avarta");
    if(model != null || model!= undefined){
        $("#leave_the_check_avarta").modal('hide');
    }
    loadMessage(senderid, receiverid);
    $(".chatApplication").addClass("displayMessage");
}
$(".chatApplication__title").click(function () {
    if (!$(".au-message-list").hasClass("displayMessage")) {
        $.get("/Message/MessageList_Partial/?id=" + $("#user_authenticated").val(), function (respone) {
            if (respone != null) {
                $(".au-message-list").html(respone);
            }
        });
    }
})

$("#contentMessage").keyup(function () {
    if ($(this).val() != '')
        $(".au-input-icon").prop("hidden", false);
    else $(".au-input-icon").prop("hidden", true);
});
function showTime(id) {
    $(".time-" + id).toggleClass("displayTime");
}
function loadMessage(senderId, receiverId) {
    senderIds = senderId;
    receiverIds = receiverId;
    RevertWatched(senderId, receiverId, true, false,true);
    $(".au-message-list").removeClass("displayMessage");
    $.get("/Message/Message_Partial/?senderId=" + senderId + "&receiverId=" + receiverId, function (respone) {
        if (respone != null) {
            $(".au-chat__content").html(respone);
            $(".au-input--full").focus();
            $("#ReceiverId").val(receiverId);
        }
    });
    countChange = 0;
}
$(".message").click(function () {
    $(".chatApplication").toggleClass("displayMessage");

});
$(".au-btn-plus").click(function () {
    $(".au-message-list").toggleClass("displayMessage");
});
//---------------End------------------------------//
function OpenPopUp2(title, url) {
    $.get(url, function (respone) {
        $("#mediumModalLabel").text(title);
        $(".contentAll").html(respone);
        $("#mediumModal").modal("show");
    });
}
function OpenPopUp(tieude,url) {
    $.get(url, function (respone) {
        $("#ModalTitle").text(tieude);
        $("#ModalContent").html(respone);
        $("#myModal").modal("show");
    });
}
function cuteAlert({
    type,
    title,
    message,
    buttonText = "OK",
    confirmText = "OK",
    cancelText = "Cancel",
    closeStyle, srcImg }) {
    return new Promise((resolve) => {
        setInterval(() => { }, 5000);
        const body = document.querySelector("body");
        let closeStyleTemplate = "alert-close";
        if (closeStyle === "circle") {
            closeStyleTemplate = "alert-close-circle";
        }

        let btnTemplate = `<button class="alert-button ${type}-bg ${type}-btn">${buttonText}</button> `;
        if (type === "question") {
            btnTemplate = `
      <div class="question-buttons">
        <a class="hover login" href="/Home/Login"><button class="confirm-button ${type}-bg ${type}-btn">${confirmText}</button></a>
        <button class="cancel-button error-bg error-btn">${cancelText}</button>
      </div>
      `;
        }

        const template = `
    <div class="alert-wrapper">
      <div class="alert-frame">
        <div class="alert-header ${type}-bg">
          <span class="${closeStyleTemplate}">X</span>
          <img class="alert-img" src="${srcImg}/img/info.svg" />
        </div>
        <div class="alert-body">
          <span class="alert-title">${title}</span>
          <span class="alert-message">${message}</span>
          ${btnTemplate}
        </div>
      </div>
    </div>
    `;

        body.insertAdjacentHTML("afterend", template);

        const alertWrapper = document.querySelector(".alert-wrapper");
        const alertFrame = document.querySelector(".alert-frame");
        const alertClose = document.querySelector(`.${closeStyleTemplate}`);

        if (type === "question") {
            const confirmButton = document.querySelector(".confirm-button");
            const cancelButton = document.querySelector(".cancel-button");

            confirmButton.addEventListener("click", () => {
                alertWrapper.remove();
                resolve("confirm");
            });

            cancelButton.addEventListener("click", () => {
                alertWrapper.remove();
                resolve();
            });
        } else {
            const alertButton = document.querySelector(".alert-button");

            alertButton.addEventListener("click", () => {
                alertWrapper.remove();
                resolve();
            });
        }

        alertClose.addEventListener("click", () => {
            alertWrapper.remove();
            resolve();
        });

        alertWrapper.addEventListener("click", () => {
            alertWrapper.remove();
            resolve();
        });

        alertFrame.addEventListener("click", (e) => {
            e.stopPropagation();
        });
    });
}
function toast(msg, srcImg, options = {}) {
    if (typeof window) {
        var container;
        var check = document.getElementById("listOfToasts");
        if (check) {
            container = check;
        } else {
            container = document.createElement("div");
            container.classList.add("toastifier__container");
            container.id = "listOfToasts";
            document.body.append(container);
        }
        var h = document.createElement("div");
        var icon = document.createElement("div");
        var message = document.createElement("div");
        const svg = (val) => {
            if (val === "error") {
                return `<img src="${srcImg}/img/toast_error.png" height="18px" width="18px"/>`;
            }
            if (val === "success") {
                return `<img src="${srcImg}/img/toast_success.png" height="18px" width="18px"/>`;
            }
            if (val === "info") {
                return `<img src="${srcImg}/img/toast_info.png" height="18px" width="18px"/>`;
            }
            if (val === "warn") {
                return `<img src="${srcImg}/img/toast_warn.png" height="18px" width="18px"/>`;
            }
        };
        if (options.showIcon === false) {
            icon.style.display = "none";
        } else {
            icon.innerHTML = `${svg(options.type || "success")}`;
        }

        message.innerText = `${msg}`;
        h.appendChild(icon);
        h.appendChild(message);
        icon.style.marginRight = "5px";
        h.classList.add("toastifier__alert");
        if (options.type) {
            h.classList.add(`toastifier__${options.type}`);
        } else {
            h.classList.add("toastifier__success");
        }
        if (options.shadow) {
            h.classList.add("toastifier__shadow");
        }
        if (options.position) {
            container.classList.add(`toastifier__${options.position}`);
        } else {
            container.classList.add("toastifier__top-center");
        }
        if (options.onClick) {
            h.addEventListener("click", () => {
                options.onClick();
            });
        }
        if (options.styleClass) {
            if (options.styleClass.background) {
                h.style.background = options.styleClass.background;
            }
            if (options.styleClass.text) {
                h.style.color = options.styleClass.text;
            }
            if (options.styleClass.border) {
                h.style.border = options.styleClass.border;
            }
        }

        let styles, styleExit;
        if (options.animation === "zoom") {
            styles = `
        0% {
            opacity: 0;
            -webkit-transform: scale3d(0.3, 0.3, 0.3);
            transform: scale3d(0.3, 0.3, 0.3);
        }
        50% {
            opacity: 1;
        }
        `;
            styleExit = `
      0% {
        opacity: 1;
      }
      50% {
        opacity: 0;
        -webkit-transform: scale3d(0.3, 0.3, 0.3);
        transform: scale3d(0.3, 0.3, 0.3);
      }
      to {
        opacity: 0;
      }
          `;
        } else if (options.animation === "flip") {
            styles = `
      0% {
        -webkit-transform: perspective(400px) rotateX(90deg);
        transform: perspective(400px) rotateX(90deg);
        -webkit-animation-timing-function: ease-in;
        animation-timing-function: ease-in;
        opacity: 0;
      }
      40% {
        -webkit-transform: perspective(400px) rotateX(-20deg);
        transform: perspective(400px) rotateX(-20deg);
        -webkit-animation-timing-function: ease-in;
        animation-timing-function: ease-in;
      }
      60% {
        -webkit-transform: perspective(400px) rotateX(10deg);
        transform: perspective(400px) rotateX(10deg);
        opacity: 1;
      }
      80% {
        -webkit-transform: perspective(400px) rotateX(-5deg);
        transform: perspective(400px) rotateX(-5deg);
      }
      to {
        -webkit-transform: perspective(400px);
        transform: perspective(400px);
      }
        `;
            styleExit = `
      0% {
        -webkit-transform: perspective(400px);
        transform: perspective(400px);
      }
      30% {
        -webkit-transform: perspective(400px) rotateX(-20deg);
        transform: perspective(400px) rotateX(-20deg);
        opacity: 1;
      }
      to {
        -webkit-transform: perspective(400px) rotateX(90deg);
        transform: perspective(400px) rotateX(90deg);
        opacity: 0;
      }
          `;
        } else {
            styles = `
      %,
      20%,
      40%,
      60%,
      80%,
      to {
        -webkit-animation-timing-function: cubic-bezier(0.215, 0.61, 0.355, 1);
        animation-timing-function: cubic-bezier(0.215, 0.61, 0.355, 1);
      }
      0% {
        opacity: 0;
        -webkit-transform: scale3d(0.3, 0.3, 0.3);
        transform: scale3d(0.3, 0.3, 0.3);
      }
      20% {
        -webkit-transform: scale3d(1.1, 1.1, 1.1);
        transform: scale3d(1.1, 1.1, 1.1);
      }
      40% {
        -webkit-transform: scale3d(0.9, 0.9, 0.9);
        transform: scale3d(0.9, 0.9, 0.9);
      }
      60% {
        opacity: 1;
        -webkit-transform: scale3d(1.03, 1.03, 1.03);
        transform: scale3d(1.03, 1.03, 1.03);
      }
      80% {
        -webkit-transform: scale3d(0.97, 0.97, 0.97);
        transform: scale3d(0.97, 0.97, 0.97);
      }
      to {
        opacity: 1;
        -webkit-transform: scaleX(1);
        transform: scaleX(1);
      }
        `;
            styleExit = `
      20% {
        -webkit-transform: scale3d(0.9, 0.9, 0.9);
        transform: scale3d(0.9, 0.9, 0.9);
      }
      50%,
      55% {
        opacity: 1;
        -webkit-transform: scale3d(1.1, 1.1, 1.1);
        transform: scale3d(1.1, 1.1, 1.1);
      }
      to {
        opacity: 0;
        -webkit-transform: scale3d(0.3, 0.3, 0.3);
        transform: scale3d(0.3, 0.3, 0.3);
      }
          `;
        }
        let styleSheet = null;
        if (!styleSheet) {
            styleSheet = document.createElement("style");
            styleSheet.type = "text/css";
            document.head.appendChild(styleSheet);

            styleSheet.sheet.insertRule(
                `@keyframes animated_entrance {${styles}}`,
                styleSheet.length
            );
            styleSheet.sheet.insertRule(
                `@keyframes animated_exit {${styleExit}}`,
                styleSheet.length
            );
        }
        const animation_time = (options.duration / 2 - 500) / 1000 || 1;
        h.style.animation = `animated_entrance ${animation_time}s`;
        const time1 = setTimeout(() => {
            h.style.animation = `animated_exit ${animation_time}s`;
        }, options.duration || 3000);
        const time2 = setTimeout(() => {
            h.remove();
            if (document.getElementsByClassName("toastifier__alert").length === 0) {
                container.remove();
            }
        }, options.duration + 1000 * animation_time || 4000);
        if (options.onhoverPause) {
            h.addEventListener("mouseover", () => {
                clearTimeout(time1);
                clearTimeout(time2);
            });
            if (options.onhoverPause) {
                h.addEventListener("mouseleave", () => {
                    h.style.animation = `animated_exit ${animation_time}s`;
                    setTimeout(() => {
                        h.style.display = "none";
                    }, 800);
                });
            }
        }
        container.appendChild(h);
    }
}
function alertSuccess(msg) {
    toast(msg, "/Client", {
        type: 'success',
        animation: 'zoom',
        position:'bottom-right'
    });
}
function alertWarn(msg) {
    toast(msg, "/Client", {
        type: 'warn',
        animation: 'zoom',
        position: 'bottom-right'
    });
}
function PaginationPage(url,parent) {
    $(window).scroll(function () {
        if ($(window).scrollTop().toFixed() == $(document).height() - $(window).height()) {
            $.get(url + "?name=" + $("#searchAll").val()+"&page=" + currentPage, function (respone) {
                $(parent).append(respone);
                loadVideoReport();
                loadSecondVideo();
            });
            currentPage++;
        }
    });
}

function searchAll(url) {
    
    $(".searchAll").click(function () {
        currentPage = 1;
        if ($("#searchAll").val() != '') {
            $.get(url +"?name=" + $("#searchAll").val()+"&page=" + currentPage, function (respone) {
                if (respone != undefined)
                    $("tbody").html(respone);
                currentPage++;
            });
        }
    });
}