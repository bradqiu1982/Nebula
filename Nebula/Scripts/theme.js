var Theme = function () {
    var show = function () {
        $('body').on('click', '.span-theme-setting', function () {
            $.post('/BRTrace/ChangeTheme',
            {
            }, function (output) {
                if (output.success) {
                    window.location.reload();
                }
            })
        })
    }
    return {
        init: function () {
            show();
        }
    };
}();