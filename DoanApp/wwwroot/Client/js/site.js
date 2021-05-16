
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