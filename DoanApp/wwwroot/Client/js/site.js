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
            if (i.classList.contains("fa-microphone")) {
                recognition.start();
            } else {
                recognition.stop();
            }
        })
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
            var currentIndex = event.resultIndex;
            var transcript = event.results[currentIndex][0].transcript;
            if (transcript.toLowerCase().trim() === "stop") {
                recognition.stop();
            } else {
                if (!searchInput.value) {
                    searchInput.value = transcript;
                }
                else {
                    if (transcript.toLowerCase().trim() === "go") {
                        searchForm.submit();
                    }
                    else {
                        if (transcript.toLowerCase().trim() === "reset")
                            searchInput.value = '';
                        else {
                            searchInput.value = transcript;
                        }
                    }

                }
            }

        }
    } else {
        alert("Browser no support voice!")
    }
//------------------Kết thúc-----------------------------

//----------------Xử lý chuyển động Video----------------
loadSecondVideo();
var flagSecond = 0;
function loadSecondVideo() {
    var listVideo = Array.from(document.querySelectorAll(".nextSecond"));
  
    for (var i of listVideo) {
        i.currentTime = 10;
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

$(document).ready(function () {

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
 //---------------End-----------------
});
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
        alert("Vui lòng đăng nhập để thực hiện chức năng này");
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
                    $(count).css("color", "gray")
                } else {
                    if (revertReaction != " ") {
                        traiNguocPhanUng(revertReaction, countRevert);
                    }
                    $(count).text(respone);
                    $(element).attr("style", "color:blue;");
                    $(count).css("color", "blue")
                }
            } else {
                alert("Like không thành công");
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
        alert("Vui lòng đăng nhập để thực hiện chức năng này");
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
        alert("Vui lòng đăng nhập để thực hiện chức năng này");
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
                    $(count).css("color", "blue")
                }
            } else {
                alert("Like không thành công");
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
            } else alert("Bình luận không thành công vui lòng thử lại");
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

/*function searchSelect2(urls) {
    $(".search_Video").select2({
        placeholder: "Tìm kiếm...........",
        theme: "classic",
        allowClear: true
    });
    var dataUse = [];
    $.ajax({
        url: "/Home/ListJsonVideo",
        type: "GET",
        dataTye: "json"
    }).done(function (datas) {
        for (var item of datas) {
            dataUse.push(item.name);
        }
        console.log(dataUse);
        $(".search_Video").select2({
            placeholder: "Tìm kiếm...........",
            theme: "classic",
            allowClear: true,
            data:dataUse
        });
    });
   
  
}*/
$("#formSearchVideo").submit(function (event) {
    if ($("#nameSearch").val() != '') return;
    event.preventDefault();
});
