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

        //$('body').on('click', '.nav-link', function(){
        //    $('.nav-link').removeClass('nav-active');
        //    $(this).addClass('nav-active');
        //})

        $('body').on('click', '#brnav', function () {
            $('.nav-link').removeClass('nav-active');
            $(this).addClass('nav-active');
            $('.br_panel').removeClass('hide');
            $('.jo_panel').addClass('hide');
        })

        $('body').on('click', '#jonav', function(){
            $('.nav-link').removeClass('nav-active');
            $(this).addClass('nav-active');
            $('.jo_panel').removeClass('hide');
            $('.br_panel').addClass('hide');
        })

        $('body').on('click', '.label-color', function () {
            alert($(this).html());
        })

        $('body').on('click', '.search-icon', function () {
            if ($('#keywords').val())
            {
                alert($('#keywords').val());
            }
        })

        $('body').on('keydown', '#keywords', function (e) {
            if (e.which == 13 && $('#keywords').val()) {
                alert($('#keywords').val());
            }
        })
    }

    var list = function(){
        //tr click event
        $('body').on('click', '.cus-table > tbody > tr', function(){
            $('.cus-table > tbody > tr').each(function(){
                $(this).attr('style', 'background-color: transparent;');
            })
            $(this).attr('style', 'background-color: #07151E;');
            // var br_no = $(this).children().eq(0).html();
            // $.post('/', {
            //     br_no: br_no
            // }, function(output){
            //     $('.br-list-right').attr('style', 'display: block;');
            // })
            //for test
            $('.br-list-right').attr('style', 'display: block;');
        })
    }

    var info = function(){
        //jo tr click event
        $('body').on('click', '.cus-table > tbody > tr', function(){
            $('.cus-table > tbody > tr').each(function(){
                $(this).attr('style', 'background-color: transparent;');
            })
            $(this).attr('style', 'background-color: #07151E;');
            // var jr_no = $(this).children().eq(0).html();
            // $.post('/', {
            //     jr_no: jr_no
            // }, function(output){
            //     $('.jo-info-right').attr('style', 'display: block;');
            // })
            //for test
            $('.jo-info-right').attr('style', 'display: block;');
        })
    }
    return {
        init: function () {
            show();
            common();
        },
        list: function(){
            list();
            common();
        },
        info: function(){
            info();
            common();
        }
    };
}();