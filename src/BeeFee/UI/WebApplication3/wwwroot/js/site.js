
function initEventEdit() {
    if ($(".html-editor").length > 0)
        CKEDITOR.replace($(".html-editor")[0]);
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

	var $file = $('#File');
	$file.change(function () {
		var $this = $(this);
		var fd = new FormData;
		fd.append('file', $this.prop('files')[0]);
		fd.append('companyName', $file.data("companyurl"));
		fd.append('eventName', $file.data("eventurl"));
		$.ajax({
			url: $file.data("imageserverurl") + '/api/home',
			contentType: false,
			processData: false,
			dataType: 'json',
			method: "POST",
			data: fd,
			success: function (data) {
				$('#errorUploadImage').remove();
				if (data.result.error != null) {
					$inp.parent().before("<span id='#errorUploadImage' class=error>" + data.result.error + "</span>");
					return;
				}
				$this.prev().prev().remove();
				$this.parent().prepend("<img src='" + $file.data("imageserverurl") + "/min/" + $file.data("companyurl") + "/" + $file.data("eventurl") + "/368x190/" + data.result.path + "' />");
				$("#Cover").val(data.result.path);
			}
		});
	});
	
}

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
	
	// ----------

    var $file = $('input[data-type="fileupload"]');
    $file.change(function () {
        var $this = $(this);
        var fd = new FormData;
        fd.append('file', $this.prop('files')[0]);
        fd.append('companyName', $file.data("companyurl"));
        fd.append('eventName', $file.data("eventurl"));
        $.ajax({
            url: $file.data("imageserverurl") + '/api/home',
            contentType: false,
            processData: false,
            dataType: 'json',
            method: "POST",
            data: fd,
            success: function (data) {
                $('#errorUploadImage').remove();
                if (data.result.error != null) {
                    $inp.parent().before("<span id='#errorUploadImage' class=error>" + data.result.error + "</span>");
                    return;
                }
                $this.prev().prev().remove();
                $this.parent().prepend("<img src='" + $file.data("imageserverurl") + "/min/" + $file.data("companyurl") + "/" + $file.data("eventurl") + "/368x190/" + data.result.path + "' />");
                $("#Cover").val(data.result.path);
            }
        });
    });

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

	$.each($('.input-field input[type="text"]'), function (i, inp) {
		if ($(inp).val().length > 0)
			$(inp).trigger("change");
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