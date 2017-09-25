var BR = function(){
	var show = function(){
        pic_timeout();
        $('body').on('mouseenter', '.f-picture-hide', function(){
            $('.footer-pic').removeClass('f-picture-hide').addClass('f-picture');
            pic_timeout();
        });

       //$('body').on('click', '.nav-link', function(){
        //    $('.nav-link').removeClass('nav-active');
        //    $(this).addClass('nav-active');
        //})


        $('body').on('click', '.label-color', function () {
            window.location.href = '/BRTrace/BRInfo?BRNum=' + $(this).html() + '&SearchWords=';
        })

    }

    var pic_timeout = function(){
        setTimeout("$('.footer-pic').removeClass('f-picture').addClass('f-picture-hide')",3000);
    }

    var common = function () {
        $('body').on('click', '.logout', function () {
            window.location.href = '/NebulaUser/Logout';
        })

        //search event
        $('body').on('mouseenter', '.row-search', function(){
            $('.search').addClass('hide');
            $('.search-div').removeClass('hide');
        })

        $('body').on('mouseleave', '.row-search', function(){
            if($('#keywords').val() === ''){
                $('.search').removeClass('hide');
                $('.search-div').addClass('hide');
            }
        })

        $('body').on('click', '.search-icon', function () {
            var keyword = $('#keywords').val();
            keyword = keyword.replace("'", "");
            if (keyword) {
                window.location.href = '/BRTrace/SearchKeyWord?Keywords=' + keyword;
            }
        })

        $('body').on('keydown', '#keywords', function (e) {
            var keyword = $('#keywords').val();
            keyword = keyword.replace("'", "");
            if (e.keyCode === 13 && keyword) {
                window.location.href = '/BRTrace/SearchKeyWord?Keywords=' + keyword;
            }
        })

        $('body').on('click', '.logo', function () {
            window.location.href = '/BRTrace/Home';
        })

        $('body').on('click', '#brnav', function () {
            window.location.href = '/BRTrace/DefaultBRList';
        })

        $('body').on('click', '#jonav', function () {
            window.location.href = '/BRTrace/DefaultJOList';
        })
    }

    var brlist = function () {
        //$(".label-panel").attr( "data-simplebar","data-simplebar");
        //$(".label-panel").attr("data-simplebar-autohide","false");
        //tr click event
        $('body').on('click', '.cus-table > tbody > tr', function(){
            $('.cus-table > tbody > tr').each(function(){
                $(this).attr('style', 'background-color: transparent;');
            })
            $(this).attr('style', 'background-color: #07151E;');
            var br_no = $(this).children().eq(0).html();
            $.post('/BRTrace/BRAgileData', {
                 br_no: br_no
             }, function (output) {
                 if (output.success) {
                     $('#br_orignator').html(output.Originator);
                     $('#br_date').html(output.OriginalDate);
                     $('#br_desc').html(output.Description);
                     $('#br_stat').html(output.Status);
                     $('#br_detail').attr('href', '/BRTrace/BRInfo?BRNum=' + br_no + '&SearchWords=' + $('#h-searchkeyword').val());
                    $('.br-list-right').attr('style', 'display: block;');
                 }
             })
        })
    }

    var jolist = function () {
        //tr click event
        $('body').on('click', '.cus-table > tbody > tr', function () {
            $('.cus-table > tbody > tr').each(function () {
                $(this).attr('style', 'background-color: transparent;');
            })
            $(this).attr('style', 'background-color: #07151E;');
            var jo_no = $(this).children().eq(0).html();
            $.post('/BRTrace/BRJOData', {
                jo_no: jo_no
            }, function (output) {
                if (output.success) {
                    $('#jo_pn').html(output.pn);
                    $('#jo_type').html(output.jotype);
                    $('#jo_stat').html(output.jostat);
                    $('#jo_date').html(output.jodate);
                    $('#jo_wip').html(output.jowip);
                    $('#jo_planner').html(output.joplanner);
                    $('#jo_detail').attr('href', '/BRTrace/JOInfo?JONum=' + jo_no);
                    $('.br-list-right').attr('style', 'display: block;');
                    $('.jo-info-right').attr('style', 'display: block;');
                }
            })
        })

        $('body').on('click', '.label-color', function () {
            window.location.href = '/BRTrace/BRInfo?BRNum=' + $(this).html() + '&SearchWords=' + $('#h-searchkeyword').val();
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
        brlist: function(){
            brlist();
            common();
        },
        jolist: function () {
            jolist();
            common();
        },
        info: function(){
            info();
            common();
        },
        common: function () {
            common();
        }

    };
}();