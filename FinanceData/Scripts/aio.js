var data_array; /* хранит данные с результатами запроса */
var current_page = 1; /* текущая страница */
var items_per_page; /* элементов на страницу */
var pages_count; /* количество страниц */
var in_ajax = false; /* состояние ajax */

/* включение-выключение ajax-анимации */
function ajax_status(a)
{
    in_ajax = a;
    if (a)
        $("#div_for_table").html("Загрузка данных...<img src='/images/ajax-loader.gif'/>");
}

/* перерасчет количества страниц */
function reinit_pages() {
    /* забираем данные с селекта */
    items_per_page = parseInt($("#items_per_page :selected").val());
    current_page = 1;
    /* если есть данные, перерисовываем таблицу */
    if (data_array !== undefined) {
        pages_count = items_per_page == 0 ? 1 : Math.ceil((data_array.length - 1) / items_per_page);
        draw_table();
    }
}

/* отрисовка таблицы */
function draw_table() {
    var html = "";

    /* отображаем количество страниц и текущую, элементы навигации по страницам */
    html += "<div class='pagination'>";

        /* влево */
        html += "<div class='left'>"
        var temp = "";
        if (current_page > 1)
            html += "<a href='javascript:void(null);' class='btn nav_arrow' id='prev_arrow'>&larr; Назад</a>";
        else
            html += "<a href='javascript:void(null);' class='btn disabled'>&larr; Назад</a>";

        html += "</div>"

        /* вправо */
        html += "<div class='right'>"
        if (current_page < pages_count)
            html += "<a href='javascript:void(null);' class='btn nav_arrow' id='next_arrow'>Вперед &rarr;</a>";
        else
            html += "<a href='javascript:void(null);' class='btn disabled'>Вперед &rarr;</a>";
        html += "</div>"

        /* центр */
        html += "<div class='center'>"
        html += "Страница ";
        if (pages_count > 1) {
            html += "<select id='select_page'>";
            for (var i = 1; i <= pages_count; i++)
                html += "<option value='" + i + "'" + (i == current_page ? "selected" : "") + ">" + i + "</option>";
            html += "</select>";
        }
        else
            html += current_page;
        html += " из " + pages_count;
        html += "</div>"

    html += "</div>";

    
    html += "<table>";

    /* заголовки таблицы */
    html += "<tr>";
    for (var Num in data_array[0])
        html += "<th>" + data_array[0][Num] + "</th>";
    html += "</tr>";

    //вычисляем нужное кол-во записей и отступ в соответствии со страницой
    var items_from = ((current_page - 1) * items_per_page) + 1;
    var items_to = (data_array.length < items_from + items_per_page || items_per_page == 0) ? data_array.length : items_from + items_per_page;

    for (var Key = items_from; Key < items_to; Key++) {
        html += "<tr>";
        for (var Num in data_array[Key])
            html += "<td>" + data_array[Key][Num] + "</td>";
        html += "</tr>";
    }

    html += "</table>";
    $("#div_for_table").html(html);
}

/* Вывод сообщения об ошибке */
function draw_error(error) {
    var html = "";
    html += "<div class='warn_red'>" + error + "</div>";
    $("#div_for_table").html(html);
}

$(function () {

    /* выбор страницы из выпадающего списка */
    $(document).on("change", "#select_page", function (event) {
        var page = parseInt($("#select_page :selected").val());
        if (page > 0 && page <= pages_count) {
            current_page = page;
            draw_table();
        }
    });

    /* нажатие стрелок вперед-назад */
    $(document).on("click", ".nav_arrow", function (event) {
        $(this).attr("id") == "next_arrow" ? current_page++ : current_page--;
        draw_table();
    });

    /* по нажатию кнопки делаем ajax-запрос на сервер */
    $("#get_data").click(function () {
        if ($("#quote").val() == "") {
            draw_error("Необходимо заполнить поле с кодом");
            return false;
        }
        var datefrom = ($("#DateFrom").datepicker("getDate") / 1000) + 86400;
        var dateto = $("#DateTo").datepicker("getDate") / 1000 + 86400;

        /* включаем состояние ожидания запроса */
        ajax_status(true);
        $.getJSON('/Main/GetData/', { provider: $("#data_provider").val(), quote: $("#quote").val(), datefrom: datefrom, dateto: dateto }, function (data) {
            /* выключаем состояние ожидания запроса */
            ajax_status(false);
            if (data.Status == "OK") {
                data_array = data.Result;
                reinit_pages();
            }
            else {
                draw_error(data.Error);
            }
        })
        /* при ошибке выключаем анимацию ajax и выводим сообщение */
        .error(function () {
            ajax_status(false);
            draw_error("Произошла ошибка отправки данных через ajax");
        });
    });

    /* изменение количества элементов на страницу */
    $("#items_per_page").change(function () {
        reinit_pages();
    });

    $("#DateFrom").datepicker({
        /* по умолчанию начальная дата: текущая-год) */
        defaultDate: -365,
        changeMonth: true,
        dateFormat: "d MM, yy",
        changeMonth: true,
        changeYear: true,
        // numberOfMonths: 3,
        onClose: function (selectedDate) {
            $("#DateTo").datepicker("option", "minDate", selectedDate);
        }
    });

    $("#DateTo").datepicker({
        defaultDate: new Date(),
        changeMonth: true,
        dateFormat: "d MM, yy",
        changeMonth: true,
        changeYear: true,
        maxDate: new Date(),
        // numberOfMonths: 3,
        onClose: function (selectedDate) {
            $("#DateFrom").datepicker("option", "maxDate", selectedDate);
        }
    });

    $("#DateFrom").datepicker("setDate", -365);
    $("#DateTo").datepicker("setDate", new Date());
});