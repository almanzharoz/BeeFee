(function ($) {
	$.fn.fileUpload = function (options) {
		jQuery.support.cors = true;
		var $t = this;

		$t.change(function() {
			var input = $(options.for, $t.parents("form")[0]);
			var token = $("input[name='__RequestVerificationToken']", $t.parents("form")[0]).val();

			var fd = new FormData;
			fd.append('file', $(this).prop('files')[0]);
			fd.append('directory', options.dir);
			fd.append('token', token);
			$.ajax({
				url: options.url+"/api/home",
				contentType: false,
				processData: false,
				dataType: 'json',
				method: "POST",
				data: fd,
				success: function (data) {
					$t.next("span").remove();
					if (data.error != null) {
						$t.after("<span class=error>" + data.error + "</span>");
						return;
					}
					$t.next("img").remove();
					$t.after("<img src=" + options.url +"'"+data.path+"?ver=" + new Date().getTime() +"' />");
					input.val(data.path);
				}
			});
		});

		return this;
	};
}(jQuery));