var JO = function(){
    var show = function(){
       
    }

    var common = function(){
        //search event
        $('body').on('mouseenter', '.row-search', function(){
            $('.search').addClass('hide');
            $('.search-div').removeClass('hide');
        })

        $('body').on('mouseleave', '.row-search', function(){
            if($('#keywords').val() == ''){
                $('.search').removeClass('hide');
                $('.search-div').addClass('hide');
            }
        })

        $('body').on('click', '.nav-link', function(){
            $('.nav-link').removeClass('nav-active');
            $(this).addClass('nav-active');
        })
    }

    return {
        init: function () {
            show();
            common();
        }
    };
}();