
/*-----------------Javscript of DetailVideo--------------------*/
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
dongs.addEventListener("click", function () {
    container.classList.toggle("hien");
    content.style.display = "block";
    flags++;
});
let dem = 0;
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
    });
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
/*const floatField = document.querySelector('#contents');
const floatContainer = document.querySelector('.floatContainer');

floatField.addEventListener('focus', () => {
   
    floatContainer.classList.add('active');
});
floatField.addEventListener('blur', () => {
   
    floatContainer.classList.remove('active');
});*/
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

/*-------------End-----------------*/
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

