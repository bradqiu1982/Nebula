var BR = function(){
	var show = function(){
        pic_timeout();
        $('body').on('mouseenter', '.f-picture-hide', function(){
            $('.footer-pic').removeClass('f-picture-hide').addClass('f-picture');
            pic_timeout();
        });
    }

    var pic_timeout = function(){
        setTimeout("$('.footer-pic').removeClass('f-picture').addClass('f-picture-hide')",3000);
    }
    return {
        init: function () {
            show();
        }
    };
}();