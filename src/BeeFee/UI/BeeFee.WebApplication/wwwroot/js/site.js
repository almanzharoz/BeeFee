﻿function initIndexPage(filters, listcontainerselector, allLoaded) {
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
                }
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
function getDoc(frame) {
    var doc = null;

    // IE8 cascading access check
    try {
        if (frame.contentWindow) {
            doc = frame.contentWindow.document;
        }
    } catch (err) {
    }

    if (doc) { // successful getting content
        return doc;
    }

    try { // simply checking may throw in ie8 under ssl or mismatched protocol
        doc = frame.contentDocument ? frame.contentDocument : frame.document;
    } catch (err) {
        // last attempt
        doc = frame.document;
    }
    return doc;
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

    $('.select_in').on('click', function () {
        var self = $(this), select = self.closest('.select'), option_list = select.find('.select_list');

        if (option_list.is(':visible')) {
            option_list.slideUp(200);
            select.removeClass('is-opened');
            self.find('.select_arrow').removeClass('is-active');
        } else {
            if ($('.select .select_list:visible').length) {
                $('.select .select_list:visible').hide();
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
        if ($('.select .select_list:visible').length && !$(e.target).closest('.select').length) {
            $('.select').removeClass('is-opened');
            $('.select .select_list').slideUp(200);
            $('.select .select_arrow').removeClass('is-active');
        }
    });

    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            $('.select').removeClass('is-opened');
            $('.select .select_list').slideUp(200);
        }
    });

    // ----------
    var showedSection = null;
    var fakeForm = null;
    var submitEventHandler = function (e) {

        var formObj = $(this);
        var formURL = formObj.attr("action");

        if (window.FormData !== undefined)  // for HTML5 browsers
        {

            var formData = new FormData(this);
            $.ajax({
                url: formURL,
                type: 'POST',
                data: formData,
                mimeType: "multipart/form-data",
                contentType: false,
                cache: false,
                processData: false,
                success: function (data, textStatus, jqXHR) {
                    showedSection.find(".create-container").html(data);
                    fakeForm.remove();
                    showedSection.removeClass('is-saved').addClass('is-shown');
                },
                error: function (jqXHR, textStatus, errorThrown) {

                }
            });
            e.preventDefault();
            //e.unbind();
        }
        else  //for olden browsers
        {
            //generate a random id
            var iframeId = 'unique' + (new Date().getTime());

            //create an empty iframe
            var iframe = $('<iframe src="javascript:false;" name="' + iframeId + '" />');

            //hide it
            iframe.hide();

            //set form target to iframe
            formObj.attr('target', iframeId);

            //Add iframe to body
            iframe.appendTo('body');
            iframe.load(function (e) {
                var doc = getDoc(iframe[0]);
                var docRoot = doc.body ? doc.body : doc.documentElement;
                var data = docRoot.innerHTML;
                //data is returned from server.
                showedSection.find(".create-container").html(data);
                fakeForm.remove();
                setTimeout(function () {
                    iframe.remove();
                    showedSection.removeClass('is-saved').addClass('is-shown');
                },
                    1);

            });

        }
    }
    var loadEventSection = function (url) {
        fakeForm = $("#event-form").clone();
        fakeForm.hide();
        fakeForm.attr("action", url);
        fakeForm.appendTo('body');
        fakeForm.submit(submitEventHandler);
        fakeForm.submit();
    }

    $('[data-step-nav="next"]').on('click', function (e) {
        e.preventDefault();
        var current = $('.create--step.is-shown');

        current.removeClass('is-shown').addClass('is-saved');
        //showedSection = current.next();
        //loadEventSection(showedSection.data("url"));
        current.next().removeClass('is-saved').addClass('is-shown');
    });

    $('[data-step-nav="edit"]').on('click', function (e) {
        e.preventDefault();
        var current = $(this).closest('.create--step');

        $('.create--step.is-shown').removeClass('is-shown');
        //showedSection = current;
        //loadEventSection(showedSection.data("url"));
        current.removeClass('is-saved').addClass('is-shown');
    });

    if ($('.wysiwyg-editor').length > 0) {
        $('.wysiwyg-editor').each(function () {
            var $inpHtml = $(this).prev();
            $(this).trumbowyg({
                lang: 'ru'
            }).on('tbwblur', function () {
                $inpHtml.val($(this).trumbowyg("html"));
            });
            $(this).trumbowyg('html', $inpHtml.val());
        });
    }

    $('.create--file-i').on('change', function () {
        $(this).parent().find('.create--file-b').html($(this).val().split('\\').pop());
    });

    $.datetimepicker.setLocale('ru');

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

    $('.js-date').datetimepicker({
        timepicker: false,
        format: 'd.m.Y'
    });

    // --------

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

    $('[data-checkboxes="total"]').on('change', function () {
        $(this).closest('.input-field--c, .vertical-field--c').find('input').prop('checked', $(this).prop('checked'));
    });

    $('.input-field--c input, .vertical-field--c input').on('change', function () {
        var checked = true;
        var parent = $(this).closest('.input-field--c, .vertical-field--c');
        var total = parent.find('[data-checkboxes="total"]');

        parent.find('input').not(total).each(function () {
            if ($(this).prop('checked') == false) {
                checked = false;

                return;
            }
        });

        total.prop('checked', checked);
    });

});