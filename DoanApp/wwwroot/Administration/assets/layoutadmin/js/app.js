//-----------------------Back end-------------------------//


//-----variable Global------
var currentPage = 2;
function loadVideoReport() {

    if ($(".tr-shadow").length != 0 && $(".tr-shadow") != undefined && $(".clickVideo").prop("id") != '0') {
        var list = document.querySelectorAll(".tr-shadow");
        console.log(list);
        for (var value of list) {
            if (value.id.split('-')[1] == document.querySelector(".clickVideo").id) {
                $(".clickVideo").get(0).click();
                break;
            }
            else $(document).scrollTop($(document).height());
        }
    }
   
}
function loadSecondVideo() {
    var listVideo = Array.from(document.querySelectorAll(".secondVideo"));
    if (listVideo != null && listVideo != undefined) {
        for (var i = 0; i < listVideo.length; i++) {
            listVideo[i].currentTime = 10;

        }
    }
}
//---------------------Phần biều đồ Home-------------------//
                    //=========>Line chart<===========//


var line__Chart = document.getElementById("lineChart");
if (line__Chart) {
        var dataUserCreate = [];
        var dataVideoCreate = [];
        $.get("/Administration/Home/GetCreateDate", function (respone) {
            respone = JSON.parse(respone);
            if (respone.length == 0) {
                for (var i = 1; i <= 12; i++) {
                    dataUserCreate.push(0);
                }
            } else {
                for (var i = 1; i <= 12; i++) {
                    var month = 0;
                    for (var j of respone) {
                        if (i == j.Month)
                            month = j.Count;
                    }
                    dataUserCreate.push(month);
                }
            }
           
        });
        $.get("/Administration/Home/GetVideoCreate", function (respone) {
            respone = JSON.parse(respone);
            if (respone.length == 0) {
                for (var i = 1; i <= 12; i++) {
                    dataVideoCreate.push(0);
                }
            } else {
                for (var i = 1; i <= 12; i++) {
                    var month = 0;
                    for (var j of respone) {
                        if (i == j.Month)
                            month = j.Count;
                    }
                    dataVideoCreate.push(month);
                }
            }
        });
       
    }
setTimeout(function () {
    var ctx = document.getElementById("lineChart");
  
    if (ctx) {
        ctx.height = 150;
        var lineChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: ["Jan", "Feb", "Mar", "Apr", "May", "June", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
                defaultFontFamily: "Poppins",
                datasets: [
                    {
                        label: "User đăng ký",
                        borderColor: "rgba(7,255,49,.09)",
                        borderWidth: "1",
                        backgroundColor: "rgba(7,255,49,0.5)",
                        data: dataUserCreate
                    },
                    {
                        label: "User đăng video",
                        borderColor: "rgba(0, 123, 255, 0.9)",
                        borderWidth: "1",
                        backgroundColor: "rgba(0, 123, 255, 0.5)",
                        pointHighlightStroke: "rgba(26,179,148,1)",
                        data: dataVideoCreate
                    }
                ]
            },
            options: {
                legend: {
                    position: 'top',
                    labels: {
                        fontFamily: 'Poppins'
                    }

                },
                responsive: true,
                tooltips: {
                    mode: 'index',
                    intersect: false
                },
                hover: {
                    mode: 'nearest',
                    intersect: true
                },
                scales: {
                    xAxes: [{
                        ticks: {
                            fontFamily: "Poppins"

                        }
                    }],
                    yAxes: [{
                        ticks: {
                            beginAtZero: true,
                            fontFamily: "Poppins"
                        }
                    }]
                }

            }
        });
    }
    
}, 1000);

                     //=========>Kết thúc<============//

                     //=========> Polar chart <=======//
try {

   
    var Polar__chart = document.getElementById("polarChart");
    var dataCategoryHome = [];
    var lableCategoryHome = [];
    if (Polar__chart) {
        $.get("/Administration/Home/GetCategory", function (respone) {
            respone = JSON.parse(respone);
            var countLenght = 0;
            for (var i of respone) {
                lableCategoryHome.push(i.Name);
                countLenght += i.Count;
            }
            for (var i of respone) {
                dataCategoryHome.push(((i.Count / countLenght) * 100).toFixed(2));
            }
            if (lableCategoryHome.length == 0) {
                lableCategoryHome.push("Chưa có thể loại");
                dataCategoryHome.push(100);
            }
            Polar__chart.height = 200;
            var popar_chart = new Chart(Polar__chart, {
                type: 'polarArea',
                data: {
                    datasets: [{
                        data: dataCategoryHome,
                        backgroundColor: [
                            "rgba(0, 123, 255,0.9)",
                            "rgba(14,255,22,0.8)",
                            "rgba(227,221,30,0.8)",
                            "rgba(250,8,220,0.8)",
                            "rgba(250,8,75,0.8)"
                        ]

                    }],
                    labels: lableCategoryHome
                },
                options: {
                    legend: {
                        position: 'top',
                        labels: {
                            fontFamily: 'Poppins'
                        }

                    },
                    responsive: true
                }
            });

        });
    }
   

} catch (error) {
    console.log(error);
}

                        //========>Kết thúc<===========//
                     //===========>Single Chart<======//
try {

    var ctx = document.getElementById("singelBarChart");
    if (ctx) {
        
        $.get("/Administration/Home/GetTotalReport", function (respone) {
            respone = JSON.parse(respone);
            var dataSingle = [];
            respone.forEach(item => {
                dataSingle.push(item.Day);
            });
            ctx.height = 200;
            var myChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: ["Sun", "Mon", "Tu", "Wed", "Th", "Fri", "Sat"],
                    datasets: [
                        {
                            label: " Lượt Báo cáo",
                            data: dataSingle,
                            borderColor: "rgba(0, 123, 255, 0.9)",
                            borderWidth: "0",
                            backgroundColor: "rgba(0, 123, 255, 0.5)"
                        }
                    ]
                },
                options: {
                    legend: {
                        position: 'top',
                        labels: {
                            fontFamily: 'Poppins'
                        }

                    },
                    scales: {
                        xAxes: [{
                            ticks: {
                                fontFamily: "Poppins"

                            }
                        }],
                        yAxes: [{
                            ticks: {
                                beginAtZero: true,
                                fontFamily: "Poppins"
                            }
                        }]
                    }
                }
            });
        });
      
    }

} catch (error) {
    console.log(error);
}
                    //============>Kết thúc<==========//

//--------------------Kết thúc----------------------------//

//---------------------Phần biểu đồ User--------------------//

$(document).ready(function () {

    // Main Template Color
    var brandPrimary = '#33b35a';
    // ------------------------Xử lý lấy dữ liệu chủ đề từ server--------------//
    var idUserView = $("#user__Id").val();
    if (idUserView != '' && idUserView != undefined) {
        // -------------------------Line Chart----------------------------- //
        var dataVideo=[];
        $.get("/Administration/User/GetCreateDate/?id=" + idUserView, function (respone) {
            respone = JSON.parse(respone);
            if (respone.length == 0) {
                for (var i = 1; i <= 12; i++) {
                    dataVideo.push(0);
                }
            } else {
                for (var i = 1; i <= 12; i++) {
                  
                    var month = 0;
                    for (var j of respone) {
                        if (i == j.Month)
                            month = j.Count;
                    }
                    dataVideo.push(month);
                }
            }
            var myLineChart = new Chart($('#lineCahrt'), {
                type: 'line',
                options: {
                    legend: {
                        display: false
                    }
                },
                data: {
                    labels: ["Jan", "Feb", "Mar", "Apr", "May", "June", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
                    datasets: [
                        {
                            label: "Video",
                            fill: true,
                            lineTension: 0.3,
                            backgroundColor: "rgba(75,192,192,0.4)",
                            borderColor: "rgba(75,192,192,1)",
                            borderCapStyle: 'butt',
                            borderDash: [],
                            borderDashOffset: 0.0,
                            borderJoinStyle: 'miter',
                            borderWidth: 1,
                            pointBorderColor: "rgba(75,192,192,1)",
                            pointBackgroundColor: "#fff",
                            pointBorderWidth: 1,
                            pointHoverRadius: 5,
                            pointHoverBackgroundColor: "rgba(75,192,192,1)",
                            pointHoverBorderColor: "rgba(220,220,220,1)",
                            pointHoverBorderWidth: 2,
                            pointRadius: 1,
                            pointHitRadius: 10,
                            data: dataVideo,
                            spanGaps: false
                        }
                    ]
                }
            });
        });




        // ------------------------------Kết thúc------------------------- //
        // ------------------------ Pie Chart ---------------------------------//
        var labelCategory = [];
        var dataCategory = [];
        $.get("/Administration/User/GetCategory/?idUser=" + idUserView, function (respone) {
            respone = JSON.parse(respone);
            var countLenght = 0;
            for (var i of respone) {
                labelCategory.push(i.Name);
                countLenght += i.Count;
            }
            for (var i of respone) {
                dataCategory.push(((i.Count / countLenght) * 100).toFixed(2));
            }
            if (labelCategory.length == 0) {
                labelCategory.push("Kênh chưa có Video");
                dataCategory.push(100);
            }

            var myPieChart = new Chart($('#pieChart'), {
                type: 'doughnut',
                data: {
                    labels: labelCategory,
                    datasets: [
                        {
                            data: dataCategory,
                            borderWidth: [1, 1, 1, 1],
                            backgroundColor: [
                                brandPrimary,
                                "rgba(75,192,192,1)",
                                "#FFCE56"
                            ],
                            hoverBackgroundColor: [
                                brandPrimary,
                                "rgba(75,192,192,1)",
                                "#FFCE56"
                            ]
                        }]
                }
            });
        });
        //-----------------------------Kết thúc---------------------------------//
    }
});



//----------------------Kết thúc--------------------------//
$(".account-wrap").click(function () {
    $(".account-wrap").toggleClass("show-dropdown");
    if ($(".noti-wrap").hasClass("show-dropdown"))
        $(".noti-wrap").toggleClass("show-dropdown");
})
$(".noti-wrap").click(function () {
  
    $(".noti-wrap").toggleClass("show-dropdown");
    if ($(".account-wrap").hasClass("show-dropdown"))
    $(".account-wrap").toggleClass("show-dropdown");
})

document.querySelectorAll('.sidebar-submenu').forEach(e => {
    e.querySelector('.sidebar-menu-dropdown').onclick = (event) => {
        event.preventDefault()
        e.querySelector('.sidebar-menu-dropdown .dropdown-icon').classList.toggle('active')

        let dropdown_content = e.querySelector('.sidebar-menu-dropdown-content')
        let dropdown_content_lis = dropdown_content.querySelectorAll('li')

        let active_height = dropdown_content_lis[0].clientHeight * dropdown_content_lis.length

        dropdown_content.classList.toggle('active')

        dropdown_content.style.height = dropdown_content.classList.contains('active') ? active_height + 'px' : '0'
    }
})

// DARK MODE TOGGLE


let overlay = document.querySelector('.overlay')
let sidebar = document.querySelector('.sidebar')

