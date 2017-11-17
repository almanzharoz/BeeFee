﻿function initLoginDialog() {
	$(this.document).on("submit", "#loginForm", function () {
		var $t = $(this);
		if ($t.validate().valid())
		$.post($t.attr("action"), $t.serialize(), data => {
			if (typeof (data.url) != "undefined")
				window.location.assign(data.url);
			else
				$($t.parent()).html(data);
		});
		return false;
	});
}

