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

        $('body').on('click', '#brclosenav', function () {
            window.location.href = '/BRTrace/DefaultBRList?p=1&Status=CLOSE';
        })

        $('body').on('click', '#brkickoffnav', function () {
            window.location.href = '/BRTrace/DefaultBRList?p=1&Status=KICKOFF';
        })

        $('body').on('click', '#joclosenav', function () {
            window.location.href = '/BRTrace/DefaultJOList?p=1&Status=CLOSE';
        })

        $('body').on('click', '#oqmjonav', function () {
            window.location.href = '/BRTrace/OQMJOList';
        })

        $('body').on('click', '#oqmjoclosenav', function () {
            window.location.href = '/BRTrace/OQMJOList?p=1&Status=CLOSE';
        })

        $('body').on('click', '#reportnav', function () {
            window.location.href = '/BRTrace/BRReport?PM=&Weeks=1';
        })

        $('body').on('click', '#erpcomponav', function () {
            window.location.href = '/BRTrace/ERPComponent';
        })
        
        $('body').on('click', '.br-breadcrumb', function () {
            window.location.href = '/BRTrace/BRInfo?BRNum=' + $(this).html();
        })

        $('body').on('click', '.current-breadcrumb', function () {
            window.location.href = '/BRTrace/JOInfo?JONum=' + $('#jo_num1').val();
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
                     $('#br_pjkey').html(output.PJKey);
                     $('#br_orignator').html(output.Originator);
                     $('#br_date').html(output.OriginalDate);
                     $('#br_desc').html(output.Description);
                     $('#br_stat').html(output.Status);

                     $('#br_sqty').html(output.StartQTY);
                     $('#br_tcost').html(output.TotalCost);
                     $('#br_scrap').html(output.ScrapQTY);
                     $('#br_revenue').html(output.SaleRevenue);
                     $('#br_phase').html(output.ProductPhase);

                     $('#br_detail').attr('href', '/BRTrace/BRInfo?BRNum=' + br_no + '&SearchWords=' + $('#keywords').val());
                    $('.br-list-right').attr('style', 'display: block;');
                 }
             })
        })

        $('body').on('click', '.pages', function () {
            var page = $(this).attr("data-data");
            if ($('#keywords').val()) {
                window.location.href = '/BRTrace/SearchKeyWord?SearchWords=' + $('#keywords').val() + '&p=' + page;
            }
            else {
                var withstatus = $('#withstatus').val();
                if (withstatus) {
                    if (withstatus === "CLOSE" || withstatus === "KICKOFF") {
                        window.location.href = '/BRTrace/DefaultBRList?p=' + page + '&Status=' + withstatus;
                    }
                    else {
                        window.location.href = '/BRTrace/DefaultBRList?p=' + page;
                    }
                }
                else {
                    window.location.href = '/BRTrace/DefaultBRList';
                }
            }

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
                    $('#jo_pjkey').html(output.jopjkey);
                    $('#jo_pn').html(output.pn.substring(0, 31));
                    if (output.jopjkey) {
                        $('#jo_yd').html(output.pnyd + '&nbsp;' + "<a href='http://wuxinpi.china.ads.finisar.com/Project/ProjectYieldMain?ProjectKey=" + output.jopjkey + "' target='_blank'>" + "<img src='../Content/images/loading.png' height='6' width='24'/>" + "</a>");
                    }
                    else {
                        $('#jo_yd').html(output.pnyd);
                    }

                    $('#jo_type').html(output.jotype);
                    $('#jo_stat').html(output.jostat);
                    $('#jo_date').html(output.jodate);
                    $('#jo_wip').html(output.jowip);
                    $('#jo_store').html(output.jostore);
                    $('#jo_planner').html(output.joplanner);
                    $('#jo_detail').attr('href', '/BRTrace/JOInfo?JONum=' + jo_no);
                    $('#jo_compo').attr('href','/BRTrace/JOComponent?JONum='+jo_no);
                    $('#jo_compo').attr('target', '_blank');
                    $('#jo_ship').attr('href', '/BRTrace/JOShipping?JONum=' + jo_no);
                    $('#jo_ship').attr('target','_blank');
                    $('.jo-info-right').attr('style', 'display: block;');
                }
            })
        })

        $('body').on('click', '.label-color', function () {
            var page = $(".current-page").html();
            if (page) {
            window.location.href = '/BRTrace/BRInfo?BRNum=' + $(this).html()
                + '&SearchWords=' + $('#keywords').val() + '&p=' + page;
            } else {
                window.location.href = '/BRTrace/BRInfo?BRNum=' + $(this).html()
                    + '&SearchWords=' + $('#keywords').val() + '&p=1';
            }
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
                    $('#jo_pjkey').html(output.jopjkey);
                    $('#jo_pn').html(output.pn.substring(0, 31));
                    if (output.jopjkey)
                    {
                        $('#jo_yd').html(output.pnyd + '&nbsp;' + "<a href='http://wuxinpi.china.ads.finisar.com/Project/ProjectYieldMain?ProjectKey=" + output.jopjkey + "' target='_blank'>" + "<img src='../Content/images/loading.png' height='6' width='24'/>" + "</a>");
                    }
                    else
                    {
                        $('#jo_yd').html(output.pnyd);
                    }

                    $('#jo_type').html(output.jotype);
                    $('#jo_stat').html(output.jostat);
                    $('#jo_date').html(output.jodate);
                    $('#jo_wip').html(output.jowip);
                    $('#jo_store').html(output.jostore);
                    $('#jo_planner').html(output.joplanner);
                    $('#jo_detail').attr('href', '/BRTrace/JOInfo?JONum=' + jo_no);
                    $('#jo_compo').attr('href', '/BRTrace/JOComponent?JONum=' + jo_no);
                    $('#jo_compo').attr('target', '_blank');
                    $('#jo_ship').attr('href', '/BRTrace/JOShipping?JONum=' + jo_no);
                    $('#jo_ship').attr('target', '_blank');
                    $('.jo-list-right').attr('style', 'display: block;');
                }
            })
        })

        $('body').on('click', '#downloadoqm', function () {
            var myurl = '/BRTrace/ExportAllOQMJO';
            window.open(myurl, '_blank');
        })
        
        $('body').on('click', '.pages', function () {
            var page = $(this).attr("data-data");
            if ($('#keywords').val()) {
                window.location.href = '/BRTrace/SearchKeyWord?SearchWords=' + $('#keywords').val() + '&p=' + page;
            }
            else {
                var withstatus = $('#withstatus').val();
                var withoqm = $('withoqm').val();
                if (withoqm) {
                    if (withstatus) {
                        window.location.href = '/BRTrace/OQMJOList?p=' + page + '&Status=' + withstatus;
                    }
                    else {
                        window.location.href = '/BRTrace/OQMJOList';
                    }
                }
                else {
                    if (withstatus) {
                        window.location.href = '/BRTrace/DefaultJOList?p=' + page + '&Status=' + withstatus;
                    }
                    else {
                        window.location.href = '/BRTrace/DefaultJOList';
                    }
                }


            }
        })
    }

    var joinfo = function () {
        $('body').on('click', '.jo-tranparent', function () {
            window.location.href = '/BRTrace/JODetail?BRNum=' + $('#br_num').html() + '&JONum=' + $('#jo_num').html() + '&Step=' + this.id.split('-')[1];
        })
    }

    var jodetail = function () {
        var jo_step_class = ['schedule', 'pro-line', 'pqe-oqc', 'warehouse'];
        var jo_step_html = ['Schedule', 'JO Distribution', 'Failure Distribution', 'TO BE'];

        $(function () {
            jo_step_skip($('#jo_step').val());
        })

        $('body').on('click', '.schedule, .pro-line, .pqe-oqc, .warehouse', function () {
            jo_step_skip(this.id.split('-')[1]);
            if (!$('#popup-import').hasClass('hide')) {
                $('#popup-import').addClass('hide');
            }
        })

        var jo_step_skip = function (step) {
            $('#jo_step').val(step);
            var jo_class = jo_step_class[step - 1];
            $('.jo-current-page').html(jo_step_html[step - 1]);
            $('.jo-current-page').removeClass().addClass('jo-current-page ' + jo_class + '-page');
            $('.op-tag').removeClass().addClass('op-tag op-tag-' + jo_class);
            if (step >= 2) {
                if ($('#jodistribute').html() == false) {
                    $("#jodistribute").attr('style', 'height: 600px');
                    JoDistribution.init('jodistribute', 'jo_num');
                }
            }
            if (step >= 3) {
                if ($('#pnerrdistribute').html() == false) {
                    $("#pnerrdistribute").attr('style', 'height: 600px');
                    pnerrordistribution.init('pnerrdistribute', 'jo_num','pjkey');
                }
            }
            $('html,body').animate({
                scrollTop: (step == 1) ? 0 : ($('.jo-' + jo_class).offset().top - 150)
            });
            window.history.pushState(null, null, window.location.href.split('Step')[0] + 'Step=' + step);
        }
        //loading more
        $(window).scroll(function () {
            if (parseInt($(window).scrollTop() + 0.5) == ($(document).height() - $(window).height())) {
                var step = parseInt($('#jo_step').val());
                if (step < jo_step_class.length) {
                    jo_step_skip(step + 1);
                }
            }
        });

        $('body').on('click', '.jo-import', function () {
            $('.popup').attr('style', 'height:' + parseInt($('.jo-schedule').height() + 40) + 'px');
            $('#popup-import').removeClass("hide");
            $('body').addClass("popup-open");
        })

        $('body').on('click', '#popup_cancel', function () {
            $('#popup-import').addClass('hide');
            $('body').removeClass("popup-open");
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
        },
        common: function () {
            common();
        }
    };
}();