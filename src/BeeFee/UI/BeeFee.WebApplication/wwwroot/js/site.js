function initIndexPage(filters, listcontainerselector, allLoaded) {
    if (typeof filters == "undefined" || filters == null || filters.length === 0)
        return;
    var page = 2;
    var size = 9;
    allLoaded = typeof allLoaded != "boolean" ? false : allLoaded;
    var loading = false;
    var loadEvents = function (filter) {
        if (allLoaded || loading)
            return;
        loading = true;
        var categories = [];
        $.each($(filter.categoriesselector + ":checked"), function (index, inp) { categories.push($(inp).val()) });
        Events.loadEvents($(filter.searchtextselector).val(), $(filter.startdateselector).val(),
            $(filter.enddateselector).val(),
            $(filter.cityselector).val(),
            categories,
            page,
            size,
            $(filter.maxpriceselector).val()).done(function (result) {
                allLoaded = result.allLoaded;
                if (result.html.length > 0) {
                    $(listcontainerselector).append(result.html);
                } else
                    $(listcontainerselector).append("Событий не найдено");
                page++;
            }).fail(function () { }).always(function () { loading = false; });
    };
    $(window).scroll(function () {
        if (($(window).scrollTop() + $(window).height()) == $(document).height()) {
            loadEvents(filters[0]);
        }
    });
}
function initEventPage(nameselector, phoneselector, emailselector) {
    $(phoneselector).mask("+7 (999) 999-9999", { placeholder: "+7 (___) ___-____" });
    $(nameselector).trigger("change");
    $(emailselector).trigger("change");
}

function initCreateOrUpdateEventPage() {

    $('[data-step-nav="next"]').on('click', function (e) {
        e.preventDefault();
        var current = $('.create--step.is-shown');

        var validator = current.find("form").data('validator');
        validator.settings.ignore = "";

        current.find("form").submit();
    });

    if ($(".html-editor").length > 0) {
        CKEDITOR.replace($(".html-editor")[0]);
        $(".html-editor").fadeIn("slow");
    }
    var $file = $('#File');
    $file.fileupload({
        url: $file.data("imageserverurl") + '/api/home',
        dataType: 'json',
        type: "POST",
        formData: { setting: 'event', companyName: $file.data("companyurl"), eventName: $file.data("eventurl") },
        done: function (e, data) {
            var $inp = $('#File');
            $('#errorUploadImage').remove();
            if (data.result.error != null) {
                $inp.parent().before("<span id='#errorUploadImage' class=error>" + data.result.error + "</span>");
                return;
            }
            $inp.prev().prev().remove();
            $inp.parent().prepend("<img src='" + $file.data("imageserverurl") + "/min/" + $file.data("companyurl") + "/" + $file.data("eventurl") + "/368x190/" + data.result.path + "' />");
            $("#Cover").val(data.result.path);
        },
    }).prop('disabled', !$.support.fileInput)
        .parent().find(".create--file-b").addClass($.support.fileInput ? undefined : 'disabled');


    $('[data-range="start"]').datetimepicker({
        onChangeDateTime: function (dp, $input) {
            showDate($input.closest('.create--field-di'), dp);
        }
    });

    $('[data-range="end"]').datetimepicker({
        onChangeDateTime: function (dp, $input) {
            showDate($input.closest('.create--field-di'), dp);
        }
    });

    if ($('[data-range="start"]').length > 0 && $('[data-range="start"]').val().length > 0)
        showDate($('[data-range="start"]').closest('.create--field-di'), $('[data-range="start"]').data("xdsoft_datetimepicker").getValue());

    if ($('[data-range="end"]').length > 0 && $('[data-range="end"]').val().length > 0)
        showDate($('[data-range="end"]').closest('.create--field-di'), $('[data-range="end"]').data("xdsoft_datetimepicker").getValue());

    function showDate(el, d) {
        var date = '';
        var time = '';

        date += d.getDate() + '/';
        date += (d.getMonth() + 1) + '/';
        date += d.getFullYear();

        time += (d.getHours() <= 9) ? ('0' + d.getHours()) : (d.getHours()) + ':';
        time += (d.getMinutes() <= 9) ? ('0' + d.getMinutes()) : (d.getMinutes());

        el.find('span.is-s1').html(date);
        el.find('span.is-s2').html(time);
    }

    var maps = $('.create--map-in');

    var process = function (maps) {
        if (typeof ymaps != 'undefined') {
            ymaps.ready(function () {
                maps.each(function () {
                    var map = new ymaps.Map(this, {
                        center: [55.751244, 37.618423],
                        zoom: 10
                    });
                });
            });
        }
    }

    if (maps.length > 0)
        $.getScript('https://api-maps.yandex.ru/2.1/?lang=ru_RU', function () {
            process(maps);
        });


}

//function eventsPageInit(filters, listcontainerselector) {
//    if (typeof filters == "undefined" || filters == null || filters.length === 0)
//        return;
//    //searchtextselector, cityselector, enddateselector, chkconcertselector, chkexhibitionselector, chkexcursionselector, maxpriceselector, btnloadselector, listcontainerselector) {
//    var page = 0;
//    var size = 10;
//    var allLoaded = false;
//    var loading = false;
//    var loadEvents = function (filter) {
//        if (allLoaded || loading)
//            return;
//        loading = true;
//        //var categories = [];
//        //$(".class-category").each(function () {
//        //    if ($(this).prop("checked"))
//        //        categories.push($(this).val());
//        //});
//        var loadingContainer = $("<div class='col-xs-4 col-md-12'>Загружаю события...</div>");
//        if (page == 0)
//            $(listcontainerselector).html("");
//        $(listcontainerselector).append(loadingContainer);

//        Events.loadEvents($(filter.searchtextselector).val(),
//            //$("#startDate").val(),
//            $(filter.enddateselector).val(),
//            $(filter.cityselector).val(),
//            $(filter.chkconcertselector).prop("checked"),
//            $(filter.chkexhibitionselector).prop("checked"),
//            $(filter.chkexcursionselector).prop("checked"),
//            //$("#chkAllCategories").prop("checked") ? [] : categories,
//            [],
//            page,
//            size,
//            $(filter.maxpriceselector).val()).done(function (events) {
//                if (events.length < size)
//                    allLoaded = true;
//                if (events.length > 0) {
//                    $.each(events,
//                        function (i, item) {
//                            $(listcontainerselector).append("<div class=\"col-xs-4 col-md-4\">"+
//                                "<div class=\"grid--item\">"+
//                                "<a class=\"item\" href=\"/event/"+item.page.Url+"\"><span class=\"item--poster\" style=\"background-image: url(" + item.url + ");\"></span>" +
//                                "<span class=\"item--date\">" + item.page.date + "</span></span><span class=\"item--title\">" +
//                                item.page.caption+"</span ></a ></div ></div >");
//                                //"<tr><td><a target='_blank' href='/event/event/" + item.url + "'>" +
//                                //item.page.caption +
//                                //"</a></td><td>" +
//                                //item.page.date +
//                                //"</td><td>" +
//                                //item.page.cover +
//                                //"</td></tr>");
//                        });
//                } else
//                    $(listcontainerselector).html("<div class='col-xs-4 col-md-12'>Событий не найдено</div>");
//                page++;
//                loadingContainer.remove();
//            }).fail(function () { $(listcontainerselector).html("<div class='col-xs-4 col-md-12'>Событий не найдено</div>"); }).always(function () { loading = false; });
//    };
//    $.each(filters, function (i, filter) {
//        $(filter.btnloadselector).click(function () {
//            allLoaded = false;
//            page = 0;
//            $(listcontainerselector).html("");
//            loadEvents(filter);
//        });
//    });
//    //$(btnloadselector).click(function () {
//    //    allLoaded = false;
//    //    page = 0;
//    //    $(listcontainerselector).html("");
//    //    loadEvents();
//    //});
//    //$('#startDate').datetimepicker({
//    //    format: "DD.MM.YYYY HH:mm"
//    //});
//    //$('#endDate').datetimepicker({
//    //    format: "DD.MM.YYYY HH:mm"
//    //});
//    //$("#chkAllCategories").change(function () {
//    //    if (!$('.class-category').prop('checked'))
//    //        $("#chkAllCategories").prop("checked", true);
//    //    else
//    //        if ($(this).prop("checked")) {
//    //            $('.class-category').prop('checked', true);
//    //        }
//    //});
//    //$(".class-category").change(function () {
//    //    if (!$('.class-category').prop('checked'))
//    //        $("#chkAllCategories").prop("checked", true);
//    //    else
//    //        if (!$(this).prop("checked")) {
//    //            $("#chkAllCategories").prop("checked", false);
//    //        }
//    //});
//    $(filters[0].btnloadselector).click();
//    $(window).scroll(function () {
//        if ($(window).scrollTop() + $(window).height() == $(document).height()) {
//            loadEvents();
//        }
//    });
//}

var topFixed, prevTop = 0, prevShowTop = 0;
function initFixedBox() {
    var container = $("div[data-fixed-wrap]>.container");
    if (container.length == 0) return;
    topFixed = container.offset().top + container.outerHeight();
    prevTop = $(this).scrollTop();
    prevShowTop = prevTop;
    $(window).scroll(function (event) {
        var y = $(this).scrollTop();
        //console.log(y, prevTop);
        if (y > prevTop || y < topFixed) {
            $(".fixed-box").removeClass('is-shown');
            //console.log("hide");
            prevTop = y;
            return;
        }
        if (y < prevTop) {
            if (y >= topFixed && y + 2 < prevTop) {
                if (!$(".fixed-box").hasClass('is-shown')) {
                    $(".fixed-box").addClass('is-shown').slideDown(200);
                    //console.log("show");
                }
            }
            prevTop = y;
        }
        if (!$(".fixed-box").hasClass('is-shown'))
            prevTop = y;
    });
}
$(document).ready(function () {

    $('a[href="#"]').on({
        click: function (e) {
            e.preventDefault();
        }
    });

    // -- Login Form

    initLoginDialog();
    // ----------
    initFixedBox();
    //var didScroll;
    //var lastScrollTop = 0;
    //var delta = 5;
    //var navbarHeight = $('.header').outerHeight();

    //$(window).on('scroll', function () {
    //    didScroll = true;
    //});

    //setInterval(function () {
    //    if (didScroll) {
    //        hasScrolled();
    //        didScroll = false;
    //    }
    //}, 100);

    //function hasScrolled() {
    //    var st = $(this).scrollTop();

    //    if (Math.abs(lastScrollTop - st) <= delta)
    //        return;

    //    if (st > lastScrollTop) {
    //        // Scroll Down
    //        $('.fixed-box').removeClass('is-shown');
    //    } else {
    //        // Scroll Up
    //        if (st + $(window).height() < $(document).height())
    //            $('.fixed-box').addClass('is-shown');
    //    }

    //    $('[data-fixed-wrap]').each(function () {
    //        var wrap = $(this);
    //        var box = $('[data-fixed="' + wrap.attr('data-fixed-wrap') + '"]');

    //        if ((wrap.offset().top + wrap.outerHeight()) >= box.offset().top) {
    //            box.slideUp(200);
    //        } else {
    //            box.slideDown(200);
    //        }
    //    });

    //    lastScrollTop = st;
    //}

    // ----------

    $('[data-goto="filter"]').on({
        click: function (e) {
            e.preventDefault();

            $('html, body').scrollTop($('.filter').offset().top - $('[data-fixed="header"]').outerHeight());

            $('[data-toggle="filter"]').addClass('is-active');
            $('.filter--body').slideDown(200, function () {
                if ($(this).is(':visible')) {
                    $(this).find('input[type="text"]').first().focus();
                }
            });
        }
    });

    // ----------

    $('.js-modals-close').on({
        click: function (e) {
            e.preventDefault();

            $('.modals, .modals--item').fadeOut(200);
        }
    });

    $('.js-modals-open').on({
        click: function (e) {
            e.preventDefault();

            var id = $(this).attr('modal-id');

            $('.modals').fadeIn(200);
            $('.modals--item#' + id).fadeIn(200, function () {
                $(this).find('input[type="text"]').first().focus();
            });
        }
    });

    // ----------

    $('[data-toggle="filter"]').on({
        click: function (e) {
            e.preventDefault();

            $('[data-toggle="filter"]').toggleClass('is-active');
            $('.filter--body').slideToggle(200, function () {
                if ($(this).is(':visible')) {
                    $(this).find('input[type="text"]').first().focus();
                }
            });
        }
    });

    // ----------

    $(document).on("focus", '.input-field', function () { $(this).addClass('with-focus'); });
    $(document).on("blur", '.input-field', function () { if ($('.input-field--i', this).val() === '') $(this).removeClass('with-focus'); });
    $(document).on("keyup", '.input-field',
        function () {
            var field = $(this);
            field.removeClass('with-error');
            if ($('.input-field--i', this).val() != '')
                field.addClass('with-value');
            else
                field.removeClass('with-value');
        });
    $(document).on("change", '.input-field',
        function () {
            var field = $(this);
            field.removeClass('with-error');
            if ($('.input-field--i', this).val() != '')
                field.addClass('with-value');
            else
                field.removeClass('with-value');
        });

    $('.vertical-field').each(function () {
        var field = $(this);
        var input = $(this).find('.vertical-field--i');

        input.on({
            focus: function () {
                field.addClass('with-focus');
            },

            blur: function () {
                field.removeClass('with-focus');
            },

            keyup: function () {
                field.removeClass('with-error');

                if ($(this).val() != '') {
                    field.addClass('with-value');
                } else {
                    field.removeClass('with-value');
                }
            },
        });
    });

    // ----------

    $('.select_in, .select_in_checkbox').on('click', function () {
        var self = $(this), select = self.closest('.select'), option_list = select.find('.select_list, .select_list_checkbox');

        if (option_list.is(':visible')) {
            option_list.slideUp(200);
            select.removeClass('is-opened');
            self.find('.select_arrow').removeClass('is-active');
        } else {
            if ($('.select .select_list:visible, .select .select_list_checkbox:visible').length) {
                $('.select .select_list:visible, .select .select_list_checkbox:visible').hide();
                $('.select .select_arrow').removeClass('is-active');
            }

            option_list.slideDown(200);
            select.addClass('is-opened');
            self.find('.arrow').addClass('is-active');
        }
    });

    $('.select_list li').on('click', function () {
        var self = $(this), title = self.closest('.select').find('.select_in .select_title'), option = self.html();

        title.html(option);
        self.closest('.select').find('input[type=hidden]').val(self.attr('data-value'));
        self.closest('.select_list').find('li').removeClass('is-active');
        self.addClass('is-active');
        self.closest('.select_list').slideUp(200);
        self.closest('.select').removeClass('is-opened');
        self.closest('.select').find('.select_arrow').removeClass('is-active');
    });

    $(document).on('click', function (e) {
        if ($('.select .select_list:visible, .select .select_list_checkbox:visible').length && !$(e.target).closest('.select').length) {
            $('.select').removeClass('is-opened');
            $('.select .select_list, .select .select_list_checkbox').slideUp(200);
            $('.select .select_arrow').removeClass('is-active');
        }
    });

    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            $('.select').removeClass('is-opened');
            $('.select .select_list, .select .select_list_checkbox').slideUp(200);
        }
    });

    // ----------

    $('.create--file-i').on('change', function () {
        $(this).parent().find('.create--file-b').html($(this).val().split('\\').pop());
    });

    $.datetimepicker.setLocale('ru');

    $('.js-date').datetimepicker({
        timepicker: false,
        format: 'd.m.Y'
    });

    // --------

    $('[data-checkboxes="total"]').on('change', function () {
        $(this).closest('.input-field--c, .vertical-field--c, .vertical-field').find('input').prop('checked', $(this).prop('checked'));
        updateTitle($(this).closest(".select"));
    });

    $('.input-field--c input, .vertical-field--c input, .vertical-field input[type="checkbox"]').on('change', function () {
        var checked = true;
        var parent = $(this).closest('.input-field--c, .vertical-field--c, .vertical-field');
        var total = parent.find('[data-checkboxes="total"]');

        parent.find('input[type="checkbox"]').not(total).each(function () {
            if ($(this).prop('checked') == false) {
                checked = false;
                return;
            }
        });

        total.prop('checked', checked);
    });

    function updateTitle($select) {
        var ar = [];
        $.each($select.find('.select_list_checkbox input[type="checkbox"]:checked'), function (i, chk) {
            var d = $(chk).data("checkboxes");
            if (d == undefined || d != "total")
                ar.push($(chk).next().text().trim());
        });
        if (ar.length > 0) {
            $select.find(".select_title div").text(ar.join(", "));
            $select.find(".select_placeholder").hide();
        } else {
            $select.find(".select_title div").text("");
            $select.find(".select_placeholder").show();
        }
    }

    $('.select_list_checkbox input[type="checkbox"]').on('change', function (event) {
        updateTitle($(this).closest(".select"));
    });

    $.each($('.select_list_checkbox'), function (i, list) {
        updateTitle($(list).closest(".select"));
    });
});