var BR = function(){
	var show = function(){
        pic_timeout();
        $('body').on('mouseenter', '.f-picture-hide', function(){
            $('.footer-pic').removeClass('f-picture-hide').addClass('f-picture');
            pic_timeout();
        });

        $('body').on('click', '.pages', function () {
            var page = $(this).attr("data-data");
            window.location.href = '/BRTrace/Home?p=' + page;
        })


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
                window.location.href = '/BRTrace/SearchKeyWord?SearchWords=' + keyword;
            }
        })

        $('body').on('keydown', '#keywords', function (e) {
            var keyword = $('#keywords').val();
            keyword = keyword.replace("'", "");
            if (e.keyCode === 13 && keyword) {
                window.location.href = '/BRTrace/SearchKeyWord?SearchWords=' + keyword;
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

        $('body').on('click', '.br-breadcrumb', function () {
            window.location.href = '/BRTrace/BRInfo?BRNum=' + $(this).html();
        })
    }

    var brlist = function () {
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
                     $('#br_detail').attr('href', '/BRTrace/BRInfo?BRNum=' + br_no + '&SearchWords=' + $('#keywords').val());
                    $('.br-list-right').attr('style', 'display: block;');
                 }
             })
        })

        $('body').on('click', '.pages', function () {
            var page = $(this).attr("data-data");
            window.location.href = '/BRTrace/SearchKeyWord?SearchWords=' + $('#keywords').val() + '&p=' + page;
        })
    }

    var brjolist = function () {
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
                    $('#jo_yd').html(output.pnyd);
                    $('#jo_type').html(output.jotype);
                    $('#jo_stat').html(output.jostat);
                    $('#jo_date').html(output.jodate);
                    $('#jo_wip').html(output.jowip);
                    $('#jo_planner').html(output.joplanner);
                    $('#jo_detail').attr('href', '/BRTrace/JOInfo?JONum=' + jo_no);
                    $('.jo-info-right').attr('style', 'display: block;');
                }
            })
        })

        $('body').on('click', '.label-color', function () {
            var page = $(".current-page").html();
            window.location.href = '/BRTrace/BRInfo?BRNum=' + $(this).html()
                + '&SearchWords=' + $('#keywords').val() + '&p=' + page;
        })
        
        $('body').on('click', '.pages', function () {
            var page = $(this).attr("data-data");
            var sub_page = ($(".sub-current-page").html() == undefined) ? 1 : $(".sub-current-page").html();
            window.location.href = '/BRTrace/BRInfo?BRNum=' + $("#br_num").html() + '&SearchWords='
                + $('#keywords').val() + '&p=' + page + '&sp=' + sub_page;
        })

        $('body').on('click', '.sub-pages', function () {
            var sub_page = $(this).attr("data-data");
            var page = ($(".current-page").html() == undefined) ? 1 : $(".current-page").html();
            window.location.href = '/BRTrace/BRInfo?BRNum=' + $("#br_num").html() + '&SearchWords='
                + $('#keywords').val() + '&p=' + page + '&sp=' + sub_page;
        })
    }

    var jolist = function(){
        //jo tr click event
        $('body').on('click', '.cus-table > tbody > tr', function () {
            $('.cus-table > tbody > tr').each(function(){
                $(this).attr('style', 'background-color: transparent;');
            })
            $(this).attr('style', 'background-color: #07151E;');
            var jo_no = $(this).children().eq(0).html();
            $.post('/BRTrace/BRJOData', {
                jo_no: jo_no
            }, function (output) {
                if (output.success) {
                    $('#jo_pn').html(output.pn);
                    $('#jo_yd').html(output.pnyd);
                    $('#jo_type').html(output.jotype);
                    $('#jo_stat').html(output.jostat);
                    $('#jo_date').html(output.jodate);
                    $('#jo_wip').html(output.jowip);
                    $('#jo_planner').html(output.joplanner);
                    $('#jo_detail').attr('href', '/BRTrace/JOInfo?JONum=' + jo_no);
                    $('.jo-list-right').attr('style', 'display: block;');
                }
            })
        })

        $('body').on('click', '.pages', function () {
            var page = $(this).attr("data-data");
            window.location.href = '/BRTrace/SearchKeyWord?SearchWords=' + $('#keywords').val() + '&p=' + page;
        })
    }
    var joinfo = function () {
        $('body').on('click', '.jo-tranparent', function () {
            window.location.href = '/BRTrace/JODetail?BRNum=' + $('#br_num').html() + '&JONum=' + $('#jo_num').html() + '&Step=' + this.id.split('-')[1];
        })
    }
    var jodetail = function () {
        var jo_step_class = ['schedule', 'pro-line', 'pqe-oqc', 'warehouse'];
        var jo_step_html = ['Schedule', 'Product Line', 'PQE/OQC', 'Ware House'];

        $(function () {
            jo_step_skip($('#jo_step').val());
        })

        $('body').on('click', '.schedule, .pro-line, .pqe-oqc, .warehouse', function () {
            jo_step_skip(this.id.split('-')[1]);
        })

        var jo_step_skip = function (step) {
            var jo_class = jo_step_class[step - 1];
            $('.current-page').html(jo_step_html[step - 1]);
            $('.current-page').removeClass().addClass('current-page ' + jo_class + '-page');
            $('.op-tag').removeClass().addClass('op-tag op-tag-' + jo_class);
            $('html,body').animate({
                scrollTop: $('.jo-' + jo_class).offset().top
            });
        }
        //loading more
        $(window).scroll(function () {
            if ($(window).scrollTop() == $(document).height() - $(window).height()) {
                //alert($(document).height() - $(window).height());
            }
        });
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
        brjolist: function () {
            brjolist();
            common();
        },
        jolist: function () {
            jolist();
            common();
        },
        jodetail: function () {
            jodetail();
            common();
        }, 
        joinfo: function () {
            joinfo();
            common();
        }
    };
}();