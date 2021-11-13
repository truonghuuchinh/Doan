

//-------------------Variable global-------------
var idPlaylist = 0;
var idvideo = 0;
var userId = $("#UserId").val();
var videoId = $("#VideoId").val();
var emailUserAuthenticated = $("#email_authenticated").val();
var currentPage = 2;
var idVideoReport = 0;
var arrayNoLoaded = [];

//Xử lý loading
function imgLoading() {
    $(".img-loading").css("display", "block");
}

//----------------Xử lý Capture Webcame--------------
//Webcam Properties settings
Webcam.set({
    width: 360,
    height: 260,
    image_format: 'png',
    jpeg_quality: 100
});
function captureAvartarWebcam() {
    Webcam.attach('#idWebcam');
}
//capture camera
$("#btnCapture").click(function () {
    Webcam.snap(function (data_uri) {
        $("#imgCapture")[0].src = data_uri;
    });
    Webcam.reset();
});
$("#btnCaptureReset").click(function () {
    Webcam.attach('#idWebcam');
});
$("#btnUpdateCapture").click(function () {
    Webcam.upload($("#imgCapture")[0].src, '/Video/UpdateAvartar', emailUserAuthenticated);
    document.location.href = window.location.href;
});


//----------------End--------------------------------
//----------------Xử lý tạo danh sách phát------------
$(".btnPlaylist").click(function () {
    var inputname = $("#namePlayList").val();
    var data = {
        "Name": inputname,
        "UserId": $("#idUser").val()
    };
    $.post("/PlayListVideo/Create", { request: data }, function (respone) {
        respone = JSON.parse(respone);
        if (respone != null) {
            $(".list__playlist").prepend(`
                <div class="listChoose">
                    <input type="checkbox" id="chooseList-${respone.Id}" data-id="${respone.Id}" class="chooseList chooseList-${respone.Id}" />
                    <label class="hover" onclick="addItem(${respone.Id})" for="chooseList-${respone.Id}"><span>${respone.Name}</span></label>
                </div>
            `);
        }
    });
    $("#namePlayList").val('');
    $(".create_playlist").css("display", "");
    $(".add-playlist").css("display", "");
});
$("#namePlayList").keyup(function () {
    if ($(this).val() == '')
        $(".btnPlaylist").prop("disabled", true);
    else $(".btnPlaylist").prop("disabled", false);
});
//----------------Kết thúc---------------------------
//----------------Xử lý cập nhật avartar---------------
$(".repair_avartar").click(function () {
    $("#chooseUpdateAvartar").modal('show');
});
$(".btnUpdateAvartar").click(function () {
    $("#chooseUpdateAvartar").modal('hide');
    $("#upload_avartar").modal('show');
    $("#updateImgAvartar").val('');
    $("#erroravartar").text('');
});
$(".btnCaptureAvartar").click(function () {
    captureAvartarWebcam();
    $("#chooseUpdateAvartar").modal('hide');
    $("#updateAvartarCapture").modal('show');
});
$("#updateImgAvartar").change(function (event) {
    $("#erroravartar").text('');
    var imgReview = document.querySelector("#img_review__avartar");
    if (event.target.files[0] != undefined) {
        imgReview.src = URL.createObjectURL(event.target.files[0]);
        imgReview.onload = function () {
            URL.revokeObjectURL(imgReview.src);
        }
        fileTempAvartar = event.target.files;
    } else {
        this.files = fileTempAvartar;
    }
});
$("#fUploadAvartar").submit(function (e) {
    $("#emailUser").val($("#email_authenticated").val());
    if (!$("#img_review__avartar")[0].src.includes("default_image.png")) return;
    $("#erroravartar").text('(Vui lòng chọn ảnh)');
    e.preventDefault();
});
//----------------Kết thúc-----------------------------
//----------------Xử lý show item in playlist-----------
$("#timkiem__video").keyup(function () {
    loadDataList(idPlaylist, '<h4>Không có video bạn tìm......................................</h4>', $(this).val());
});
function deleteInPlayList(id) {
    if (id != 0) {
        var data = {
            "PlayListId": idPlaylist,
            "VideoId": id
        };
        $.post("/DetailVideo/Delete", { data: JSON.stringify(data) }, function (respone) {
            if (respone == "Success") {
                $(".bodyContent-" + id).remove();
                $(".removeHr-" + id).remove();
                var countItem1 = parseInt($(".item2__count-" + idPlaylist).text()) - 1;
                var countItem2 = parseInt($(".modifier__td-" + idPlaylist).text()) - 1;
                if (countItem1 == 0) {
                    $(".table__playlist__item1-" + idPlaylist).prop('src', '/Client/img/no_playlist.PNG');
                }
                $(".item2__count-" + idPlaylist).text(countItem1);
                $(".modifier__td-" + idPlaylist).text(countItem2);
                alertSuccess("Đã xóa");
            }
        });
    }
}
function showAllPlayList(id) {
    idPlaylist = id;
    if (id != 0) {
        loadDataList(id, `<h4>Chưa có video trong danh sách phát</h4>`, '');
    }
    $("#modalShowPlayList").modal('show');
    $("#timkiem__video").val('');
    setTimeout(function () {
        $(".searchAllList").focus();
    }, 500);
}
function loadDataList(id, message, search) {
    $.get("/Video/GetAllPlayList/?id=" + id + "&nameSearch=" + search, function (respone) {
        if (respone != 'null') {

            respone = JSON.parse(respone);
            var getdata = '';
            $.each(respone, function (index, item) {
                getdata += `
                            <div class="bodyContent bodyContent-${item.Id}">
                                <div class="bodyContent__left hover">
                                    <a href="/Home/DetailVideo/${item.Id}"><img src="/Client/imgPoster/${item.PosterImg}" alt="Alternate Text" /></a>
                                </div>
                                <div class="bodyContent__center">
                                    <div class="bodyContent__center--name">${item.Name}</div>
                                    <div class="bodyContent__center--mota">${item.Description}</div>
                                    <div class="bodyContent__center--kenh">${item.FirtsName} ${item.LastName} </div>
                                </div>
                                <div class="bodyContent__createdate">
                                    <div class="bodyContent__createdate--noidung">${item.CreateDate.split(" ")[0]}</div>
                                </div>
                                <div class="bodyContent__right">
                                   <a target="blank" class="a__playlist" href="/Home/DetailVideo/${item.Id}"><i style="color:#5252fc;" class="far fa-play-circle hover" title="Phát video"></i></a>
                                    <i style="color: #e90505;" class="fas fa-trash hover" onclick="deleteInPlayList(${item.Id})" title="Xóa khỏi danh sách"></i>
                                </div>
                            </div>
                        <hr class="removeHr-${item.Id}" />
                            `;
            });
            $(".tbodyOfPlayList").html(getdata);
        } else {
            $(".tbodyOfPlayList").html(message);
        }
    });
}
//---------------End--------------------------------
//----------------Xử lý load giây-----------------------
function loadDurationTime() {
    var array_video = new Array();
    var array_textDuration = new Array();
    array_video = Array.from(document.querySelectorAll(".nextSecond"));
    array_textDuration = Array.from(document.querySelectorAll(".time__duration"));
    excuteDuration(array_video, array_textDuration);
    if (arrayNoLoaded.length > 0) {
        setTimeout(function () {
            excuteDuration(array_video, array_textDuration);
        }, 5000);
    }

}
function excuteDuration(array_video, array_textDuration) {

    for (var i = 0; i < array_video.length; i++) {
        var tam = Math.floor(array_video[i].duration);
        var miniute = Math.floor(array_video[i].duration / 60);
        var seccond = 0;
        for (var j = 0; j < miniute; j++) {
            tam -= 60;
        }
        seccond = Math.abs(tam);
        if (Math.abs(tam) < 10)
            seccond = "0" + Math.abs(tam).toString();
        if (array_video[i].readyState >= 3)
            array_textDuration[i].innerHTML = miniute + ":" + seccond;
        else arrayNoLoaded.push(array_video[i]);
    }
}

//----------------Kết thúc----------------------------
//----------------Xử lý hiển  thị thêm bình luận--------
function displayComment(id) {
    var listChild = document.querySelector(".listchild-" + id).scrollHeight;
    var text = $(".listChild__text-" + id).text();
    var countChild = $(".countChildComment-" + id).val();
    if (text == 'Xem thêm ' + countChild) {
        $(".listchild-" + id).css('max-height', (listChild + 89 * parseInt(countChild)) + "px");
        $(".listChild__text-" + id).text("Ẩn đi " + countChild);
    } else {
        $(".listchild-" + id).css('max-height', '');
        $(".listChild__text-" + id).text("Xem thêm " + countChild);
    }
}
//----------------Kết thúc------------------------------
//----------------Xử lý chỉnh sửa Bình luận--------------
var userLoginSystem = $("#user_authenticated").val();
function showOptionComment(id) {
    $(".optionId-" + id).toggleClass("showoption");
}
function closeOption(id) {
    $(".optionId-" + id).removeClass("showoption");

}
function confirmRepairComment(id) {

    var data = {
        "Id": id,
        "Content": $(".repaircomment__input-" + id).val()
    };
    $.post("/Comment/UpdateContent", { request: data }, function (respone) {
        if (respone == "Success") {
            cancelRepairComment(id, false);
            $(".content__Comment-" + id).text($(".repaircomment__input-" + id).val());
            $(".inputContent-" + id).val($(".repaircomment__input-" + id).val());
            alertSuccess("Đã chỉnh sửa");
        }
    });
}
function cancelRepairComment(id, flag) {
    if (flag) {
        $(".repaircomment__input-" + id).val($(".inputContent-" + id).val());
    }
    $(".repaircomment-" + id).css('display', '');
    $(".content__Comment-" + id).css('display', '');
    $(".noidung_comment--img-" + id).css('display', '');
    $(".optionId-" + id).css('display', '');
    $(".optionId-" + id).removeClass("showoption");
    $(".repaircomment__primary-" + id).prop('disabled', false);
}
function changeContent(id) {
    var content = $(".content__Comment-" + id).text().trim();
    var input = $(".repaircomment__input-" + id).val().trim();

    if (input != content)
        $(".repaircomment__primary-" + id).prop('disabled', false);
    else {
        $(".repaircomment__primary-" + id).prop('disabled', true);
    }
}

function repairComment(id) {
    if (userLoginSystem == '') {
        cuteAlert({
            type: "question",
            title: "Đăng nhập",
            message: "Vui lòng đăng nhập để thực hiện chức năng",
            confirmText: "Đăng nhập",
            cancelText: "Hủy",
            srcImg: "/Client"
        });
    } else {
        $(".repaircomment-" + id).css('display', 'flex');
        $(".content__Comment-" + id).css('display', 'none');
        $(".noidung_comment--img-" + id).css('display', 'none');
        $(".optionId-" + id).css('display', 'none');
        $(".repaircomment__input-" + id).focus();
        $(".repaircomment__primary-" + id).prop('disabled', true);
    }
}
function deleteComment(Id, commentId) {

    if (userLoginSystem == '' || userLoginSystem == undefined) {
        cuteAlert({
            type: "question",
            title: "Đăng nhập",
            message: "Vui lòng đăng nhập để thực hiện chức năng",
            confirmText: "Đăng nhập",
            cancelText: "Hủy",
            srcImg: "/Client"
        });
    } else {
        $.post("/Comment/Delete", { id: Id }, function (respone) {
            if (respone != "Error") {

                for (var i of JSON.parse(respone)) {
                    $(".removeComment-" + i).remove();
                    var view = parseInt($(".count_comment").text()) - 1;
                    $(".count_comment").text(view.toString() + " Bình luận");
                    var viewChild = parseInt($(".listChild__text-" + commentId).text().split(" ")[2]) - 1;
                    $(".listChild__text-" + commentId).text("Ẩn đi " + viewChild);
                    $(".countChildComment-" + commentId).val(viewChild);
                }
                alertSuccess("Đã xóa");
            } else {
                alertSuccess("Xóa không thành công");
            }
        });
    }
}
//----------------Kết thúc-------------------------------
//----------------Xử lý báo cáo video vi phạm------------

function reportVideo(id) {
    idVideoReport = id;
    $("#reportvideo").modal("show");
    $("#valueReport").focus();
    $("#errorReport").text('');
    $("#valueReport").val('');
}
$("#confirm_report").click(function () {
    var userId = $("#idUser").val();
    var inputReport = $("#valueReport").val();
    if (inputReport == '') $("#errorReport").text("(*Vui lòng nhập nội dung*)");
    else {
        $("#reportvideo").modal("hide");
        var datas = {
            "UserId": userId,
            "Content": inputReport,
            "VideoId": idvideo
        };
        $.post("/ReportVideo/Create", { "data": JSON.stringify(datas) }, function (respone) {
            if (respone == "Success") {
                alertSuccess("Báo cáo thành công");
                $("#valueReport").val('');
            }
        });
    }
});
//----------------Kết thúc------------------------------
//----------------Xử lý add video into playList----------

function addItem(id) {
    var data = {
        "PlayListId": id,
        "VideoId": idvideo
    };

    if ($(".chooseList-" + id).prop("checked") == false) {
        $.post("/DetailVideo/Create", { "data": JSON.stringify(data) }, function (respone) {
            if (respone == "Success") {
                alertSuccess("Đã thêm");
            } else {
                alertWarn("Có lỗi vui lòng thử lại!");
            }
        });
    } else {
        $.post("/DetailVideo/Delete", { "data": JSON.stringify(data) }, function (respone) {
            if (respone != "Error") {
                alertSuccess("Đã xóa");
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

function showPlayListInDetail(id) {
    addPlayList(id);
    $("#modalPlayList").modal('show');
}
function showPlayList(id) {
    $("#modalPlayList").modal('show');
}
function addPlayList(id) {
    idvideo = id;
    $("#movepage_detail-" + id).click(function (e) {
        e.preventDefault();
        $(this).unbind(e);
    });
    if ($("#idUser").val() == '0' || $("#idUser").val() == '' || $("#idUser").val() == undefined) {
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
    document.querySelectorAll(".parent").forEach(item => {
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
    document.querySelectorAll(".content_right--item").forEach(item => {
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
    document.querySelectorAll(".khung_thongbao_video").forEach(item => {
        item.addEventListener('mouseleave', function () {
            document.querySelectorAll(".an_thongbaonay_all").forEach(item => {

                if (item.classList.contains("displays")) {
                    item.classList.remove("displays");
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
            searchInput.value = transcript;
            setTimeout(function () {
                searchForm.submit();
            }, 1000);
        }
    }

} else {
    alert("Browser no support Voice Search!");
}
//------------------Kết thúc-----------------------------

//----------------Xử lý chuyển động Video----------------
$(document).ready(function () {
    setTimeout(function () {
        loadSecondVideo();
    }, 1000);
});

var flagSecond = 0;
function loadSecondVideo() {
    var listVideo = Array.from(document.querySelectorAll(".nextSecond"));
    for (var i = 0; i < listVideo.length; i++) {
        listVideo[i].currentTime = 10;

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
        viewConvert = view.replace(".", ",") + " Tr";
    }
    else {
        if (view > 1000) {
            view = (view / 1000).toFixed(1);
            if (view.split(".")[1] == '0') {
                view = view.split(".")[0];
            }
            viewConvert = view.toString() + " N";
        }
        else viewConvert = view.toString();
    }
    return viewConvert;
}
//------------------Kết thúc--------------------------



//------------ Xử lý Like video---------

var userLike = $("#UserLike").val();
var reaction = $("#Reaction").val();
if (userId != undefined && userLike == userId) {
    if (reaction == "Like") {
        $(".likeVideo").attr("style", "color:blue;");
        $("#countLike").css("color", "blue")
    }
    if (reaction == "DisLike") {
        $(".dislikeVideo").attr("style", "color:blue;");
        $("#countDisLike").css("color", "blue")
    }

}
$("#Like").click(function () {
    if ($(".likeVideo").css("color") == "rgb(0, 0, 255)")
        reactionLike("DontLike", ".likeVideo", "#countLike", " ", " ");
    else {
        if ($(".dislikeVideo").css("color") == "rgb(0, 0, 255)") {
            $("#countDisLike").css("color", "gray");
            $(".dislikeVideo").css("color", "gray");
            reactionLike("Like", ".likeVideo", "#countLike", "DisLike", "#countDisLike");
        } else {
            reactionLike("Like", ".likeVideo", "#countLike", " ", " ");
        }

    }

});
$("#DisLike").click(function () {
    if ($(".dislikeVideo").css("color") == "rgb(0, 0, 255)")
        reactionLike("DontDisLike", ".dislikeVideo", "#countDisLike", " ", " ");
    else {
        if ($(".likeVideo").css("color") == "rgb(0, 0, 255)") {
            $("#countLike").css("color", "gray");
            $(".likeVideo").css("color", "gray");
            reactionLike("DisLike", ".dislikeVideo", "#countDisLike", "Like", "#countLike");
        } else {
            reactionLike("DisLike", ".dislikeVideo", "#countDisLike", " ", " ");
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
function reactionLike(value, element, count, revertReaction, countRevert) {
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

                } else {
                    if (revertReaction != " ") {
                        traiNguocPhanUng(revertReaction, countRevert);
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
        reactionComment("DontLike", ".like-" + id, ".countLike-" + id, " ", " ", id);
    } else {
        if ($(".dislike-" + id).css("color") == "rgb(0, 0, 255)") {
            $(".countDis-" + id).css("color", "gray");
            $(".dislike-" + id).css("color", "gray");
            reactionComment("Like", ".like-" + id, ".countLike-" + id, "DisLike", ".countDis-" + id, id);
        } else {
            reactionComment("Like", ".like-" + id, ".countLike-" + id, " ", " ", id);
        }
    }
}
function dislikeComment(id) {
    if ($(".dislike-" + id).css("color") == "rgb(0, 0, 255)") {
        reactionComment("DontDisLike", ".dislike-" + id, ".countDis-" + id, " ", " ", id);
    } else {
        if ($(".like-" + id).css("color") == "rgb(0, 0, 255)") {
            $(".countLike-" + id).css("color", "gray");
            $(".like-" + id).css("color", "gray");
            reactionComment("DisLike", ".dislike-" + id, ".countDis-" + id, "Like", ".countLike-" + id, id)
        } else {
            reactionComment("DisLike", ".dislike-" + id, ".countDis-" + id, " ", " ", id);
        }
    }
}
function reactionComment(reaction, element, count, reactionRevert, countRevert, id) {
    var userId = $("#UserId").val();
    var videoId = $("#VideoId").val();
    var commentId = $(".comment-" + id).val();

    var data = {
        "UserId": userId,
        "VideoId": videoId,
        "CommentId": commentId,
        "Reaction": reaction,
        "IdComment": id
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
                        revertReactionComment(reactionRevert, countRevert, id);
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
function revertReactionComment(value, count, id) {
    var userId = $("#UserId").val();
    var videoId = $("#VideoId").val();
    var commentId = $(".comment-" + id).val();
    var data = {
        "UserId": userId,
        "VideoId": videoId,
        "Reaction": value,
        "CommentId": commentId,
        "IdComment": id
    };

    $.post("/LikeComment/EditLike", { "dataLike": JSON.stringify(data) }, function (respone) {
        $(count).text(respone);
    });
}
//-------------------------End--------------------------------

//------------------------Xử lý Comment---------------------------

function comment(checkComment, id, level, commentId) {
    var userId = '';
    var content = '';
    var videoId = '';
    var commentId = '';
    var replyFor = '';
    var replyForId = 0;
    if (id == ' ') {
        userId = $("#UserId").val();
        content = $("#Content").val();
        videoId = $("#VideoId").val();
        commentId = $("#CommentId").val();
    } else {
        var Id = id.toString();
        userId = $(".UserId-" + Id).val();
        content = $(".Content-" + Id).val();
        videoId = $(".VideoId-" + Id).val();
        commentId = $(".CommentId-" + Id).val();
        if (level == "childOne") {
            replyFor = "childOne";
        } else {
            replyFor = $(".label-" + Id).text();
            replyForId = $("#idChild-" + id).val();
        }
    }
    if (content != '') {
        var formData = {
            "UserId": userId,
            "Content": content,
            "VideoId": videoId,
            "CommentId": commentId,
            "ReplyFor": replyFor,
            "ReplyForId": replyForId
        };
        var JsonComment = JSON.stringify(formData);
        $.post("/Home/CreateComment_Partial", { "comments": JsonComment }, function (respone) {
            if (respone != "Error") {
                if (checkComment) {
                    if (document.querySelector(".listchild__display-" + commentId) != null)
                        document.querySelector(".listchild__display-" + commentId).insertAdjacentHTML("afterend", respone);
                    else {
                        document.querySelector(".all_counts_comment_rep-" + id).insertAdjacentHTML("afterend", `
                             <div class="listChild listchild-${id}">
                                <div class="listchild__display listchild__display-${id} hover" onclick="displayComment(${id})">
                                        <i class="fas fa-caret-down"></i> 
                                        <div class="listChild__text-${id}">Ẩn đi 1</div>
                                </div>
                            </div>
                        `);
                        $(".removeComment-" + id).append(`<input type="hidden" class="countChildComment-${id}" value="0" />`);
                        $(".listchild-" + id).css('max-height', "200px");
                        document.querySelector(".listchild__display-" + commentId).insertAdjacentHTML("afterend", respone);

                    }
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
                $(".text__comment_right").css("display", "");
                $(".count_comment").text(view.toString() + " Bình luận");
                $(".all_counts_comment_rep-" + id).addClass("displayNone");
                if (commentId != 0) {
                    var listChild = document.querySelector(".listchild-" + commentId).scrollHeight;
                    var count = parseInt($(".countChildComment-" + commentId).val()) + 1;
                    var maxheight = (listChild + 89 * parseInt(count)).toString() + "px";
                    $(".listchild-" + commentId).css('max-height', maxheight);
                    $(".listChild__text-" + commentId).text("Ẩn đi " + count);
                    $(".countChildComment-" + commentId).val(count);
                }
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
}
function showFrameBlur(id) {

    var floatContainer = document.querySelector('.floatContainer-' + id);
    floatContainer.classList.remove('tranfer');
    floatContainer.classList.remove('active');
}

function oke(id, type, child) {
    let contente = document.querySelector(`.all_counts_comment_rep-${id}`);
    if (type) {
        if (child == 'child') {
            var name = $(".TacGia-" + id).text();
            $(".label-" + id).text('@' + name);
        }
        contente.style.display = "block";
        $(`.all_counts_comment_rep-${id}`).removeClass("displayNone");
    }
    else {
        contente.style.display = "none";
        $(`.all_counts_comment_rep-${id}`).removeClass("displayNone");
    }
}

//---------------------End-------------------
//Phần menu
let menu = document.getElementsByClassName("click_menu")[0];
let container = document.getElementsByClassName("all_nav_menus_as")[0];
menu.addEventListener("click", function (event) {
    event.stopPropagation();
    container.classList.toggle("hien");
});

let button = document.getElementsByClassName("collapses");
for (let i = 0; i < button.length; i++) {

    button[i].addEventListener("click", function (event) {
        event.stopPropagation();
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

function Phantrang(link) {
    $('.site-layout').on('scroll', function () {
        let div = $(this).get(0);

        if (div.scrollTop + div.clientHeight >= div.scrollHeight) {
            $(".loading").addClass("displayBlock");
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
        setTimeout(function () {
            loadDurationTime();
        }, 1000);
        $(".loading").removeClass("displayBlock");
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
    if ($(".all_nav_menus_as").hasClass('hien'))
        $(".all_nav_menus_as").toggleClass('hien');
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
}, 3000);
function revertStatus(event, id) {

    event.stopPropagation();
    $.get("/Notification/UpdateStatus/?id=" + id, function (respone) {
        if (respone != "Error") {
            $(".item_notifi-" + id).remove();
            alertSuccess("Đã ẩn thông báo")
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
            alertSuccess("Đã tắt thông báo")
        }
    });
}
//-------------------------End----------------------------------
//-------------------------Xử lý Thêm  vào video đã xem---------
function addWatched(id) {
    var userId = $("#user_authenticated").val();

    if (id != 0 && id != null && userId != undefined) {
        var data = {
            "VideoId": id,
            "UserId": userId
        };
        $.ajax({
            url: '/VideoWatched/Create',
            data: {
                request: data
            },
            type: 'POST',
            dataType: 'html',
            contentType: 'application/x-www-form-urlencoded'
        });
    }
}
function removeWatched(link, id) {
    var userId = $("#user_authenticated").val();
    var data = {
        "VideoId": id,
        "UserId": userId
    };
    if (id != 0 && id != null && id != '') {
        $.post(link, { resquest: data }, function (respone) {
            if (respone != "Error") {
                $("#remove-" + id).remove();
                alertSuccess("Đã xóa");
            }
        });
    }
}
//-------------------------Kết thúc-----------------------------
//-----------------------Xử lý xóa khỏi danh sách yêu thích-----

//-----------------------Kết thúc-------------------------------