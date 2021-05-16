function changeDisplayPassword(id) {
  let x = document.getElementById(id);
  let icon = document.getElementById(`eye-${id}`);
  if (x.type === "password") {
    x.type = "text";
    icon.src = "../assets/img/Eye.svg";
  } else {
    x.type = "password";
    icon.src = "../assets/img/Eye (closed).svg";
  }
}

function handleForgotPassword() {
  document.getElementsByClassName("form-forgot-password")[0].style.display =
    "none";
  document.getElementsByClassName("success-forgot-password")[0].style.display =
    "block";
}

function handleResetPassword() {
  if (document.getElementsByClassName("form-change-password").length) {
    document.getElementsByClassName("form-change-password")[0].style.display =
      "none";
  }
  if (document.getElementsByClassName("form-reset-password").length) {
    document.getElementsByClassName("form-reset-password")[0].style.display =
      "none";
  }
  document.getElementsByClassName("success-reset-password")[0].style.display =
    "block";
}

/*tấn nhật*/
function onopinion() {
  let button = document.getElementsByClassName("collapse");
  for (let i = 0; i < button.length; i++) {
    button[i].addEventListener("click", function () {
      this.classList.toggle("actives");
      let content = document.getElementsByClassName("content")[i];
      if (!content.style.maxHeight) {
        content.style.maxHeight = content.scrollHeight + "px";
      } else {
        content.style.maxHeight = null;
      }
    });
  }
}
function onopinionss() {
  let button = document.getElementsByClassName("collapsess");
  for (let i = 0; i < button.length; i++) {
    button[i].addEventListener("click", function () {
      this.classList.toggle("active");
      let content = document.getElementsByClassName("content")[i];
      if (!content.style.maxHeight) {
        content.style.maxHeight = content.scrollHeight + "px";
      } else {
        content.style.maxHeight = null;
      }
    });
  }
}

function onopenvote() {
  var x = document.getElementById("open_vote");
  let outvote = document.getElementById("out_vote");
  outvote.style.maxHeight = "200px";
  document.getElementById("reset_vote").reset();
}

$(document).on("click", "#data_confirm", function () {
  var myBookIds = $("#data_meting").val();
  $("#data_meting_outs").text(myBookIds);
});

$(document).ready(function () {
  $("#customCheckall").change(function () {
    $(".custom-control-input_users").prop("checked", $(this).prop("checked"));
  });
});
$(document).ready(function () {
  $("#customChecksecretary").change(function () {
    $(".custom-control-input_users_secretary").prop(
      "checked",
      $(this).prop("checked")
    );
  });
});
// Huỳnh thuần
$(document).on("change", ".fileUploadWrap input[type='file']", function () {
  if ($(this).val()) {
    var filename = $(this).val().split("\\");
    filename = filename[filename.length - 1];
    $(".uploadTitle").text(filename);
    document
      .getElementById("label__upload")
      .classList.add("label__uploaded-file");
    if ($(".close__upload-file").css("display") == "none") {
      $(".close__upload-file").css("display", "block");
    }
  }
});

function close__uploaded() {
  document
    .getElementById("label__upload")
    .classList.remove("label__uploaded-file");
  var html =
    "<img class='addImage' src='../assets/img/ic_add_24px.svg' alt=''>" +
    "Tải file chữ ký";
  $(".uploadTitle").html(html);
  if ($(".close__upload-file").css("display") == "block") {
    $(".close__upload-file").css("display", "none");
  }
}

var expanded = false;
function showMultiCheckbox() {
  var multicheckbox = document.getElementById("multicheckbox");
  if (!expanded) {
    multicheckbox.style.display = "block";
    expanded = true;
  } else {
    multicheckbox.style.display = "none";
    expanded = false;
  }
}

//Toggle
$(document).ready(function () {
  $(".result__block .container").on("click", function (event) {
    event.preventDefault();
    $(this).next().slideToggle();
  });
});

function myFunction(x) {
  var stt = x.id;
  let btn_img = document.getElementsByClassName("result__img-arrow");
  btn_img[stt].classList.toggle("result__img-arrow-active");
}
// end
$(document).ready(function () {
  let div_img = document.getElementsByClassName("part1__right-img");
  for (let i = 0; i < div_img.length; i++) {
    div_img[i].addEventListener("click", function () {
      let img = document.getElementsByClassName("img-largersign")[i];
      img.classList.toggle("img-largersign_active");
      let content = document.getElementsByClassName("frames-info-deliver")[i];
      if (content.style.display !== "block") {
        content.style.display = "block";
      } else {
        content.style.display = "none";
      }
    });
  }
  let content__part1_text = document.getElementsByClassName(
    "content__part1-text"
  );
  for (let i = 0; i < content__part1_text.length; i++) {
    content__part1_text[i].addEventListener("click", function () {
      let string_status = $(".part1_status" + [i]).text();
      if (string_status !== "Đã nhận") {
        $(".receive-project").css("display", "none");
        $(".div_pause-user").css("display", "flex");
      } else {
        $(".receive-project").css("display", "block");
        $(".div_pause-user").css("display", "none");
      }
    });
  }
});

$(document).ready(function () {
  let btn_pause = document.getElementsByClassName("btn-pause");
  for (let i = 0; i < btn_pause.length; i++) {
    btn_pause[i].addEventListener("click", function () {
      $("#confirm-pause").click(function () {
        let context_part = document.getElementsByClassName("part1_right-text")[
          i
        ];
        let div_pause = document.getElementsByClassName("div-row__pause")[i];
        context_part.textContent = "Tạm hoãn";
        context_part.style.color = "#FF0000";
        div_pause.style.display = "none";
      });
    });
  }
});

function showListOptionDocumnet() {
  let container = document.getElementsByClassName("col__list-option");
  for (let i = 0; i < container.length; i++) {
    container[i].addEventListener("click", function () {
      let img = document.getElementsByClassName("option-down__img")[i];
      img.classList.toggle("option-down__img-active");
      let content = document.getElementsByClassName("row-list-option")[i];
      if (!content.style.maxHeight) {
        content.style.maxHeight = content.scrollHeight + 3 + "px";
      } else {
        content.style.maxHeight = null;
      }
    });
  }
}

function contentIsShown() {
  document.getElementsByClassName("two-submit")[0].style.display = "none";
  document.getElementsByClassName("view-content")[0].style.display = "block";
}

function showListLocationManager() {
  console.log(1);
  let container = document.getElementsByClassName("col-evaluate");
  for (let i = 0; i < container.length; i++) {
    container[i].addEventListener("click", function () {
      let content = document.getElementsByClassName("list-manager")[i];
      if (content.style.display !== "flex") {
        content.style.display = "flex";
      } else {
        content.style.display = "none";
      }
    });
  }
}

$(document).ready(function () {
  var string_check = "";
  var cut_string = "";
  $('input[name="option__checkbox[]"]').click(function () {
    $('input[name="option__checkbox[]"]:checked').each(function () {
      string_check += $(this).val() + ", ";
      cut_string = string_check.substring(0, string_check.length - 2);
    });
    if (cut_string !== "") {
      $("#take-data").val(cut_string);
    } else {
      $("#take-data").val("Tùy chọn tài liệu");
    }
    string_check = "";
    cut_string = "";
  });
});

// docment secrectary
// add note docment secrectary
$().ready(function () {
  $("#btn__add-note").click(function () {
    $(".add__content-text").fadeIn(300);
  });
  $(".close-note").click(function () {
    $(".add__content-text").hide(200);
  });
});

$(document).ready(function () {
  let square_arrow = document.getElementsByClassName("square-arrow");
  for (let i = 0; i < square_arrow.length; i++) {
    square_arrow[i].addEventListener("click", function () {
      let img = document.getElementsByClassName("img_arrow_blue")[i];
      img.classList.toggle("img_arrow_blue-active");
      let content = document.getElementsByClassName("info-users")[i];
      if (content.style.display !== "block") {
        content.style.display = "block";
      } else {
        content.style.display = "none";
      }
    });
  }
  let p_tilte = document.getElementsByClassName("p_tilte");
  for (let i = 0; i < p_tilte.length; i++) {
    p_tilte[i].addEventListener("click", function () {
      let string_status = $(".js_status" + [i]).text();
      if (string_status !== "Đã nhận") {
        $(".div__taking-mission").css("display", "none");
        $(".div_pause-user").css("display", "flex");
      } else {
        $(".div__taking-mission").css("display", "flex");
        $(".div_pause-user").css("display", "none");
      }
    });
  }
});

$().ready(function () {
  $("document").ready(function () {
    $(".tab-slider--body").hide();
    $(".tab-slider--body:first").show();
  });
  // end add note docment secrectary

  // toggle document docment secrectary
  $().ready(function () {
    $("document").ready(function () {
      $(".document__col-right").hide();
      $(".document__col-right:first").show();
    });

    $(".document__content-doc-left").click(function () {
      $(".document__col-right").hide();
      var activeTab = $(this).attr("rel");
      $("#" + activeTab).fadeIn();
    });
  });
  // end toggle document docment secrectary
  // toggle tag docment secrectary
  $(".toggle-tabs__title span").click(function () {
    $(".tab-slider--body").hide();
    var activeTab = $(this).attr("rel");
    $("#" + activeTab).fadeIn();
    $(".toggle-tabs__title span").removeClass("internal-active-toggle-red");
    $(this).addClass("internal-active-toggle-red");
  });
});
// end toggle tag docment secrectary

// drop down document secrectary
$().ready(function () {
  let button = document.getElementsByClassName("btn-dropdown");
  for (let i = 0; i < button.length; i++) {
    button[i].addEventListener("click", function () {
      let content = document.getElementsByClassName("dropdown-content")[i];
      if (!content.style.maxHeight) {
        content.style.maxHeight = content.scrollHeight + "px";
      } else {
        content.style.maxHeight = null;
      }
    });
  }
  $(".rotate").click(function () {
    $(this).toggleClass("down");
  });
});
// end drop down docment secrectary
// docment secrectary

// enuciate
$().ready(function () {
  $(".expression-info__item").click(function () {
    $(".expression-info__item").removeClass("expression-info__highlights");
    $(this).addClass("expression-info__highlights");
  });
});
// end enuciate

// voting opinion
function genderChanged(obj) {
  var value = obj.value;
  var content = document.getElementById("modal-content").innerHTML;
  if (value != "") {
    $("#modal-content-child:nth-child(1)").remove();
    $("#row-panel").after(content);
  }
}
// end voting opinion

$(document).ready(function () {
  if(/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)){
    window.location.replace("../error.html");
  }
})