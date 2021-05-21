
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
function comment() {
    let h = document.getElementById("hello_coment");
    h.insertAdjacentHTML("afterend", `<div class="noidung_comment" id="hello_coment"><div class="all_counts_comment_left"><div class="customs_imgs_comment"><img class="details_lerfts icon-sidebar-items_video" alt="" src="~/Client/img/avatar.svg"></div></div><div class="all_counts_comment_right">
            <div class="khung_hang_1"><div class="ten_tac_gia">phạm nhật</div>
            <div class="thoi_gian_binh_luan">1 ssssay ago</div></div><div class="khung_hang_2"><div class="doan_thoi_gian_comment">2:03</div>
            <div class="noi_dung_binh_luan">Bắt đầu từ khoảnh khắc ấy chúng ta đã nhìn thấy 5 trái bóng được xoay cùng 1 lúc</div>
            </div><div class="khung_hang_3"><div class="like_binh_luan"></div><div class="so_luong_like">8</div><div class="disklike_binh_luan"></div><div class="so_luong_disklike">8</div><div class="phan_hoi_binh_luan-${dem}" onclick="oke(${dem}, ${true})" >REPLY</div></div>
            <div class="all_counts_comment_rep-${dem}" style="display: none;";><div class="all_counts_comment_rep_left"><div class="customs_imgs_rep_comment">
            <img class="details_lerfts icon-sidebar-items_video" alt="" src="~/Client/img/avatar.svg"></div></div><div class="all_counts_comment_rep_right"><input type="text" class="check_comments" placeholder="Bình Luận" type="text">
            <div class="text__comment_rep_right"><button class="check_rep_cancel" onclick="oke(${dem}, ${false})">CANCEL</button><button class="check_rep_comment" onclick="actions()">REPLY</button></div></div></div></div></div>`);
    dem++;
}
function oke(dem, type) {
    let contente = document.querySelector(`.all_counts_comment_rep-${dem}`);
    let a = document.querySelector(`.phan_hoi_binh_luan-${dem}`);
    if (type) {
        contente.style.display = "block";
    }
    else {
        contente.style.display = "none";
    }
}

/*-------------End-----------------*/
    let button = document.getElementsByClassName("collapses");
    for (let i=0; i<button.length;i++) {

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