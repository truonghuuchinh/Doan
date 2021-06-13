

//----------------Xử lý báo cáo video vipham------------
var idVideoReport = 0;
function reportVideo(id) {
    idVideoReport = id;
    $("#reportvideo").modal("show");
}
$("#confirm_report").click(function () {
    var userId = $("#idUser").val();
    var inputReport = $("#valueReport").val();
    if (inputReport == '') $("#errorReport").text("Vui lòng nhập nội dung");
    else {
        $("#reportvideo").modal("hide");
        var datas = {
            "UserId": userId,
            "Content": inputReport,
            "VideoId": idvideo
        };
        $.post("/ReportVideo/Create", { "data": JSON.stringify(datas) }, function (respone) {
            if (respone == "Success") {
                toast('Báo cáo thành công', '/Client', {
                    type: 'success',
                    animation: 'zoom',
                    position: 'bottom-right'
                });
                $("valueReport").val('');
            }
        });
    }
});
//----------------Kết thúc------------------------------
//----------------Xử lý add video into playList----------
//---Variable global
var idvideo = 0;
function addItem(id) {
    var data = {
        "PlayListId": id,
        "VideoId": idvideo
    };
   
    if ($(".chooseList-" + id).prop("checked") == false) {
        $.post("/DetailVideo/Create", { "data": JSON.stringify  (data) }, function (respone) {
            if (respone != "Error") {
                toast('Đã thêm', '/Client', {
                    type: 'success',
                    animation: 'zoom',
                    position: 'bottom-left'
                });
            }
        });
    } else {
        $.post("/DetailVideo/Delete", { "data": JSON.stringify(data) }, function (respone) {
            if (respone != "Error") {
                toast('Đã xóa', '/Client', {
                    type: 'warn',
                    animation: 'zoom',
                    position: 'bottom-left'
                });
            }
        });
    
    }
    
}
$(".add-playlist").click(function () {
    $(this).css("display", "none");
    $(".create_playlist").css("display", "block");
});
$(".close").click(function () {
    $(".add-playlist").css("display", "block");
    $(".create_playlist").css("display", "none");
    document.querySelectorAll(".chooseList").forEach(item => {
        item.checked = false;
    });
});
function showPlayList(id) {
   
        $("#modalPlayList").modal('show');
  
}
function addPlayList(id) {
    idvideo = id;
    $("#movepage_detail-" + id).click(function (e) {
        e.preventDefault();
        $(this).unbind(e);
    });
    if ($("#idUser").val() == '0') {
        cuteAlert({
            type: "question",
            title: "Đăng nhập",
            message: "Vui lòng đăng nhập để thực hiện chức năng",
            confirmText: "Đăng nhập",
            cancelText: "Hủy",
            srcImg: "/Client"
        });
    }
    else {
        $.get("/DetailVideo/ListId/?id=" + id, function (respone) {
            if (respone != 'OK') {
                respone = JSON.parse(respone);
                document.querySelectorAll(".chooseList").forEach(item => {
                    for (var i of respone) {
                        if (i == item.getAttribute("data-id"))
                            item.checked = true;
                    }
                });
            }
        });
        if ($(".playlist-" + id).css('display') == 'none') {
            $("#addplaylist-" + id).css("display", "block");
            $(".playlist-" + id).css("display", "block");
        } else {
            $("#addplaylist-" + id).css("display", "");
            $(".playlist-" + id).css("display", "");
        }
    }
}
LoadEventMoseLeave();
function LoadEventMoseLeave() {
    document.querySelectorAll(".container22").forEach(item => {
        item.addEventListener('mouseleave', function () {
            document.querySelectorAll(".playlist_option").forEach(item => {
                var itemNone = window.getComputedStyle(item).getPropertyValue("display");
                if (itemNone == 'block') {
                    item.style.display = "";
                }
            });
            document.querySelectorAll(".add_playlist").forEach(item => {
                var itemNone = window.getComputedStyle(item).getPropertyValue("display");
                if (itemNone == 'block') {
                    item.style.display = "";
                }
            });

        });
    });
}

//---------------Kết thúc-------------------------

//----------------Xử lý kiềm tiếm Giọng nói--------------
    var searchForm = $("#formSearchVideo").get(0);
    var searchInput = $("#nameSearch").get(0);
    var i = $("#i_searchVoice").get(0);
    const SpeechRecognitions = window.SpeechRecognition || window.webkitSpeechRecognition;
    if (SpeechRecognitions) {
    const recognition = new webkitSpeechRecognition();
    recognition.lang = "vi";
    recognition.continuous = true;
    $(".search_Voice").click(function () {
        i.classList.contains("fa-microphone") ? (recognition.start()) : (recognition.stop());
    });
    recognition.addEventListener("start", function (event) {
        i.classList.remove("fa-microphone");
        i.classList.add("fa-microphone-slash")
        searchInput.focus();
    });
    recognition.addEventListener("end", function (event) {
        i.classList.remove("fa-microphone-slash");
        i.classList.add("fa-microphone");
        searchInput.focus();
    });
    recognition.onresult = function (event) {
    var transcript = event.results[event.resultIndex][0].transcript.toLowerCase().trim();
        if (transcript === "stop") {
            recognition.stop();
        } else {
            if (transcript === "go") {
                if (searchInput.value != '') searchForm.submit();
            } else {
                searchInput.value = transcript == "reset" ? '' : transcript;
            }
        }
    }

    } else {
        alert("Browser no support Voice Search!");
    }
//------------------Kết thúc-----------------------------

//----------------Xử lý chuyển động Video----------------
$(document).ready(function () {
    loadSecondVideo();
});

var flagSecond = 0;
function loadSecondVideo() {
    var listVideo = Array.from(document.querySelectorAll(".nextSecond"));
    //var listminiute = Array.from(document.querySelectorAll(".miniute_video"));
    for (var i = 0; i < listVideo.length; i++) {
        listVideo[i].currentTime = 10;
        /*var mininute = (listVideo[i].duration / 60).toFixed(2).split('.');
        console.log(mininute);*/
        //listminiute[i].textContent = mininute[0] + ":" + mininute[1];
    }
}
function viewBefore(id) {

    var nextTime = 30;
    var totalMiniute = document.querySelector(".nextSecond-" + id).duration;
  
    flagSecond = setInterval(function () {
        if (nextTime > parseInt(totalMiniute)) {
            nextTime = 30;
        }
        document.querySelector(".nextSecond-" + id).currentTime = nextTime;
        nextTime += 30;
    }, 600);
}
function clearBefore(id) {

    document.querySelector(".nextSecond-" + id).currentTime = 10;
    clearInterval(flagSecond);
}
//---------------------------End---------------------------

//-------------------Hàm convert View-----------------
function convertViewCount(view) {

    var viewConvert = "";
    if (view > 1000000) {
        view = (view / 1000000).toFixed(1);
        viewConvert = view.replace(".",",") + " Tr";
    }
    else {
        if (view > 1000) {
            view = (view / 1000).toFixed(1);
            viewConvert = view.toString() + " N";
        }
        else viewConvert = view.toString();
    }
    return viewConvert;
}
//------------------Kết thúc--------------------------



 //------------ Xử lý Like video---------
    //Global variable
    var userId = $("#UserId").val();
    var videoId = $("#VideoId").val();
    //end
    var userLike = $("#UserLike").val();
    var reaction = $("#Reaction").val();
    if (userId != undefined && userLike == userId) {
        if (reaction == "Like") {
            $(".likeVideo").attr("style", "color:blue;");
            $("#countLike").css("color","blue")
        } 
        if (reaction == "DisLike") {
            $(".dislikeVideo").attr("style", "color:blue;");
            $("#countDisLike").css("color", "blue")
        }
          
    }
$("#Like").click(function () {
    if ($(".likeVideo").css("color") == "rgb(0, 0, 255)")
        reaction("DontLike", ".likeVideo", "#countLike"," "," ");
    else {
        if ($(".dislikeVideo").css("color") == "rgb(0, 0, 255)") {
            $("#countDisLike").css("color", "gray");
            $(".dislikeVideo").css("color", "gray");
            reaction("Like", ".likeVideo", "#countLike", "DisLike", "#countDisLike");
        } else {
            reaction("Like", ".likeVideo", "#countLike"," "," ");
        }
     
    }

});
$("#DisLike").click(function () {
    if ($(".dislikeVideo").css("color") == "rgb(0, 0, 255)")
        reaction("DontDisLike", ".dislikeVideo", "#countDisLike"," "," ");
    else {
        if ($(".likeVideo").css("color") == "rgb(0, 0, 255)") {
            $("#countLike").css("color", "gray");
            $(".likeVideo").css("color", "gray");
            reaction("DisLike", ".dislikeVideo", "#countDisLike", "Like", "#countLike");
        } else {
            reaction("DisLike", ".dislikeVideo", "#countDisLike"," "," ");
        }
    }
  

});
function traiNguocPhanUng(value, count) {
    var userId = $("#UserId").val();
    var videoId = $("#VideoId").val();
    var data = {
        "UserId": userId,
        "VideoId": videoId,
        "Reaction": value
    };
  
    $.post("/LikeVideo/getLikeNguocPhanUng", { "data": JSON.stringify(data) }, function (respone) {
        $(count).text(respone);
    });
}
function reaction(value,element,count,revertReaction,countRevert) {
    var userId = $("#UserId").val();
    var videoId = $("#VideoId").val();
    var data = {
        "UserId": userId,
        "VideoId": videoId,
        "Reaction": value
    };
    if (userId == undefined) {
        cuteAlert({
            type: "question",
            title: "Đăng nhập",
            message: "Vui lòng đăng nhập để thực hiện chức năng",
            confirmText: "Đăng nhập",
            cancelText: "Hủy",
            srcImg: "/Client"
        });
    } else {
        var JsonData = JSON.stringify(data);
        $.ajax({
            url: "/LikeVideo/Create",
            method: "POST",
            data: { likeDetail: JsonData }
        }).done(function (respone) {
            if (respone != "Error") {
                if (value == "DontDisLike" || value == "DontLike") {
                    $(count).text(respone);
                    $(element).attr("style", "color:gray;");
                    $(count).css("color", "gray");
                   
                   /* toast(' Đã bỏ thích', '/Client', {
                        type: 'warn',
                        animation: 'zoom',
                        position: 'top-right'
                    });*/
                } else {
                    if (revertReaction != " ") {
                        traiNguocPhanUng(revertReaction, countRevert);
                    }
                    $(count).text(respone);
                    $(element).attr("style", "color:blue;");
                    $(count).css("color", "blue");
                    
                   /* toast('Đã thích', '/Client', {
                        type: 'success',
                        animation: 'zoom',
                        position:'top-right'
                    });*/
                }
            } else {
               /* toast('Like không thành công', '/Client', {
                    type: 'error',
                    animation: 'zoom',
                    position: 'top-right'
                });*/
            }
        });
        
    }
}
$("#Share").click(function () {
    var userId = $("#UserId").val();
    var videoId = $("#VideoId").val();
    var data = {
        "UserId": userId,
        "VideoId": videoId,
        "Reaction": value
    };
    if (userId == undefined) {
        cuteAlert({
            type: "question",
            title: "Đăng nhập",
            message: "Vui lòng đăng nhập để thực hiện chức năng",
            confirmText: "Đăng nhập",
            cancelText: "Hủy",
            srcImg: "/Client"
        });
    }
});

//----------------------End-------------------------------

//-----------------------Xử lý like Comment-------------------

function likeComment(id) {
    if ($(".like-" + id).css("color") == "rgb(0, 0, 255)") {
        reactionComment("DontLike", ".like-" + id, ".countLike-" + id, " ", " ",id);
    } else {
        if ($(".dislike-" + id).css("color") == "rgb(0, 0, 255)") {
            $(".countDis-" + id).css("color", "gray");
            $(".dislike-" + id).css("color", "gray");
            reactionComment("Like", ".like-" + id, ".countLike-" + id, "DisLike", ".countDis-" + id,id);
        } else {
            reactionComment("Like", ".like-" + id, ".countLike-" + id, " ", " ",id);
        }
    }
}
function dislikeComment(id) {
    if ($(".dislike-" + id).css("color") == "rgb(0, 0, 255)") {
        reactionComment("DontDisLike", ".dislike-" + id, ".countDis-" + id, " ", " ",id);
    } else {
        if ($(".like-" + id).css("color") == "rgb(0, 0, 255)") {
            $(".countLike-" + id).css("color", "gray");
            $(".like-" + id).css("color", "gray");
            reactionComment("DisLike", ".dislike-" + id, ".countDis-" + id,"Like", ".countLike-" + id,id)
        } else {
            reactionComment("DisLike", ".dislike-" + id, ".countDis-" + id, " ", " ",id);
        }
    }
}
function reactionComment(reaction, element, count, reactionRevert, countRevert,id) {
    var userId = $("#UserId").val();
    var videoId = $("#VideoId").val();
    var commentId = $(".comment-" + id).val();
    
    var data = {
        "UserId": userId,
        "VideoId": videoId,
        "CommentId": commentId,
        "Reaction": reaction,
        "IdComment":id
    };
    
    if (userId == undefined) {
        cuteAlert({
            type: "question",
            title: "Đăng nhập",
            message: "Vui lòng đăng nhập để thực hiện chức năng",
            confirmText: "Đăng nhập",
            cancelText: "Hủy",
            srcImg: "/Client"
        });
    } else {
        var JsonData = JSON.stringify(data);
        $.ajax({
            url: "/LikeComment/Create",
            method: "POST",
            data: { "dataLike": JsonData }
        }).done(function (respone) {
            if (respone != "Error") {
                if (reaction == "DontDisLike" || reaction == "DontLike") {
                    $(count).text(respone);
                    $(element).attr("style", "color:gray;");
                    $(count).css("color", "gray")
                } else {
                    if (reactionRevert != " ") {
                        revertReactionComment(reactionRevert, countRevert,id);
                    }
                    $(count).text(respone);
                    $(element).attr("style", "color:blue;");
                    $(count).css("color", "blue");
                }
            } else {
              
            }
        });

    }
}
function revertReactionComment(value, count,id) {
    var userId = $("#UserId").val();
    var videoId = $("#VideoId").val();
    var commentId = $(".comment-" + id).val();
    var data = {
        "UserId": userId,
        "VideoId": videoId,
        "Reaction": value,
        "CommentId": commentId,
        "IdComment":id
    };

    $.post("/LikeComment/EditLike", { "dataLike": JSON.stringify(data) }, function (respone) {
        $(count).text(respone);
    });
}
//-------------------------End--------------------------------

//------------------------Xử lý Comment---------------------------

function comment(checkComment, id,level) {
    var userId = '';
    var content ='';
    var videoId ='';
    var commentId = '';
    var replyFor = '';
    if (id == ' ') {
        userId = $("#UserId").val();
        content = $("#Content").val();
        videoId = $("#VideoId").val();
        commentId = $("#CommentId").val();
    } else {
        var Id = id.toString();
        userId = $(".UserId-" + Id).val();
        content = $(".Content-"+Id).val();
        videoId = $(".VideoId-"+Id).val();
        commentId = $(".CommentId-" + Id).val();
        if (level == "childOne") {
            replyFor = "childOne";
        } else {
            replyFor = $(".label-" + Id).text();
        }
    }
    if (content != '') {
        var formData = {
            "UserId": userId,
            "Content": content,
            "VideoId": videoId,
            "CommentId": commentId,
            "ReplyFor": replyFor
        };
        var JsonComment = JSON.stringify(formData);
        $.post("/Home/CreateComment_Partial", { "comments": JsonComment }, function (respone) {
            if (respone != "Error") {
                if (checkComment) {

                    document.querySelector(".all_counts_comment_rep-" + commentId).insertAdjacentHTML("afterend", respone)
                } else {
                    var helloComment = document.getElementById("hello_coment");
                    if (helloComment == null) {
                        document.querySelector(".all_counts_comment").insertAdjacentHTML("afterend", respone);
                    }
                    else {
                        $("#Content").val('');
                        document.getElementById("hello_coment").insertAdjacentHTML("beforeBegin", respone);
                    }
                }
                var view = parseInt($(".count_comment").text()) + 1;
                $(".count_comment").text(view.toString() + " Bình luận");
            } else {
                cuteAlert({
                    type: "error",
                    title: "Có lỗi",
                    message: "Vui lòng thử lại",
                    confirmText: "Ok",
                    cancelText: "Hủy",
                    srcImg: "/Client"
                });
            }
            $(".Content-" + Id).val('');
            $("#Content").val('');
        });
    }

}
function showFrameActive(id) {
    var floatContainer = document.querySelector('.floatContainer-' + id);
    floatContainer.classList.add('tranfer');
    floatContainer.classList.add('active');
    console.log(id);
}
function showFrameBlur(id) {
    
    var floatContainer = document.querySelector('.floatContainer-' + id);
    floatContainer.classList.remove('tranfer');
    floatContainer.classList.remove('active');
    console.log(id);
}

function oke(id, type,child) {
    let contente = document.querySelector(`.all_counts_comment_rep-${id}`);
    if (type) {
        if (child == 'child') {
            var name = $(".TacGia-" + id).text();
            $(".label-" + id).text('@' + name);
        }
        contente.style.display = "block";
        
    }
    else {
        contente.style.display = "none";
    }
}

//---------------------End-------------------
//Phần menu
var flags = 0;
let menu = document.getElementsByClassName("click_menu")[0];
let container = document.getElementsByClassName("all_nav_menus_as")[0];
let dongs = document.getElementsByClassName("dong")[0];
let content = document.getElementsByClassName("aa")[0];
menu.addEventListener("click", function () {
    if (flags % 2 == 0) {
        container.classList.toggle("hien");
        content.style.display = "none";
    } else {
        container.classList.toggle("hien");
        content.style.display = "block";
    }
    flags++;
});

let button = document.getElementsByClassName("collapses");
for (let i = 0; i < button.length; i++) {

    button[i].addEventListener("click", function () {

        if (button[i].classList.contains("active")) {
            button[i].innerHTML = "Xem thêm";

        }
        else {
            button[i].innerHTML = "Ẩn bớt";
        }
        this.classList.toggle("active");
        let content = document.getElementsByClassName("xohangall")[i];
        if (!content.style.maxHeight) {
            content.style.maxHeight = content.scrollHeight + "px";
        }
        else {
            content.style.maxHeight = null;
        }
    })
}
//---------------End-----------------------
/*-----------------phần ẩn hiện mô tả--------------*/
let but = document.getElementsByClassName("funtions_hien");
for (let i = 0; i < but.length; i++) {

    but[i].addEventListener("click", function () {

        if (but[i].classList.contains("activets")) {
            but[i].innerHTML = "HIỂN THỊ THÊM";

        }
        else {
            but[i].innerHTML = "ẨN BỚT";
        }
        this.classList.toggle("activets");
        let content = document.getElementsByClassName("funtions_comment")[i];
        if (!content.style.maxHeight) {
            content.style.maxHeight = content.scrollHeight + "px";
        }
        else {
            content.style.maxHeight = null;
        }
    })
}

//----------------------------End-------------------------------
//-------------------------Xử lý tìm kiếm----------------------

$("#formSearchVideo").submit(function (event) {
    if ($("#nameSearch").val() != '') return;
    event.preventDefault();
});
//-----Variable globale-----
var currentPage = 2;
function Phantrang(link) {
    $('.site-layout').on('scroll', function () {
        let div = $(this).get(0);

        if (div.scrollTop + div.clientHeight >= div.scrollHeight) {
            loadData(link);

            currentPage += 1;
        }
    });
}
function loadData(link) {
    $.get(link + currentPage, function (respone) {
        $('.site-layout').append(respone);
        loadSecondVideo();
        LoadEventMoseLeave();
    });
}
//---------------------Xử lý Notification----------------------------

var checkCount = parseInt($(".soluongtb").text());

var slTb = $('.soluongtb').text();
if (parseInt(slTb) <= 0) $(".thongbao_do").css('display', 'none');
var idNotifi = 0;
function revertWatched(arrayId) {
    $.ajax({
        type: "POST",
        url: "/Notification/UpdateWatched",
        data: {
            IdNotifi: arrayId
        },
        datatype: "html",
        contentType: 'application/x-www-form-urlencoded'
    });
}
$(".notification").click(function (event) {
    checkCount = 0;
    var arrayIdNotifi = [];
    document.querySelectorAll(".getIdNotifi").forEach(item => {
        arrayIdNotifi.push(parseInt(item.value));
    });
    revertWatched(arrayIdNotifi);
    event.stopPropagation();
    $(".soluongtb").text('0');
    $(".thongbao_do").css('display', 'none');
    var checknoneNotifi = document.querySelector('.option_notifi-' + idNotifi);
    if (checknoneNotifi != null && checknoneNotifi.classList.contains('displays')) {
        document.querySelector('.option_notifi-' + idNotifi).classList.remove('displays');
    }
    document.querySelector(".thongbao_all").classList.toggle('displays');
    console.log(idNotifi);
});

$(".thongbao_all").click(function (e) {
    e.stopPropagation();
    if ($('.option_notifi-' + idNotifi).hasClass('displays'))
     $('.option_notifi-' + idNotifi).toggleClass('displays');
});
$(document).click(function () {
    if ($(".displays").css('display') == 'block') {
        document.querySelector(".thongbao_all").classList.toggle('displays');
        if ($('.option_notifi-' + idNotifi).hasClass('displays'))
            $('.option_notifi-' + idNotifi).toggleClass('displays');
    }
});

function optionNotifi(event, id) {
    idNotifi = id;
    $('.option_notifi-' + id).toggleClass('displays');
    event.stopPropagation();
}

var realTimeNotification = setInterval(function () {
    if ($("#user_authenticated").val() != '' && $("#user_authenticated").val() != null) {
        $.get("/Notification/GetCountNotifi", function (responeCount) {
            if (parseInt(responeCount) > parseInt(checkCount)) {
                $.get("/Notification/Notification_Partial", function (respone) {
                    var userId = $('#user_authenticated').val();
                    $(".content_notifi-" + userId).html(respone);
                    $(".soluongtb").text(responeCount);
                    $(".thongbao_do").css('display', 'block');
                });
            }
            checkCount = parseInt(responeCount);
           
        });
    }
}, 5000);
function revertStatus(event, id) {
   
    event.stopPropagation();
    $.get("/Notification/UpdateStatus/?id=" + id, function (respone) {
        if (respone != "Error") {
            $(".item_notifi-" + id).remove();
            toast('Đã xóa thông báo', '/Client', {
                type: 'success',
                animation: 'zoom',
                position: 'bottom-right'
            });
        }
    });
}
function revertFollow(event, id) {
    
    var data = {
        "FromUserId": $("#user_authenticated").val(),
        "ToUserId": $(".userid-" + id).val()
    };
    $.ajax({
        type: "POST",
        url: "/Notification/UpdateFollow",
        data: {
            follow: data
        },
        datatype: "html",
        contentType: 'application/x-www-form-urlencoded'
    }).done(function (respone) {
        if (respone != "Error") {
            toast('Đã tắt thông báo', '/Client', {
                type: 'success',
                animation: 'zoom',
                position: 'bottom-right'
            });
        }
    });
}
//-------------------------End----------------------------------
