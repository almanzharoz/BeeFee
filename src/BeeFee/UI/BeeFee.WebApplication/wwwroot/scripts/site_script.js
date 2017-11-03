$(function () {
    $(this).on("submit", ".ajax-form", function () {
        var $t = $(this);
        if ($t.validate().valid())
            $.post($t.attr("action"), $t.serialize(), function (data) {
                $($t.parents("#ajax-container, .modals--body")).html(data);
            });
        return false;
    });
    $(this).on("click", ".ajax-link", function () {
        $($(this).parents("#ajax-container, .modals--body")).load($(this).attr("href"));
        return false;
    });
});
function initLoginDialog() {
    $(this.document).on("submit", "#loginForm", function () {
        var $t = $(this);
        if ($t.validate().valid())
            $.post($t.attr("action"), $t.serialize(), function (data) {
                if (typeof (data.url) != "undefined")
                    window.location.assign(data.url);
                else
                    $($t.parent()).html(data);
            });
        return false;
    });
}
