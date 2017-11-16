var pnerrordistribution = function () {
    var show = function (container, id, pjkey) {
        var temppjkey = $('#' + pjkey).val();
        var mychart = new Highcharts.chart(container, {
            title: {
                text: 'PN Failure Pareto',
                style: {
                    color: '#afafaf',
                    fontSize: '28px'
                }
            },
            colors: [
                '#33CCFF', '#FF0000', '#0070C0',
                '#FF3399', '#FA9604', '#00B050',
                '#6E28BC',
            ],
            chart:
            {
                alignTicks: false,
                backgroundColor: "#001D2E",
                borderColor: "#001D2E"
            },
            xAxis: {
                type: 'category',
                crosshair: true,
                labels: {
                    rotation: -45,
                    style: {
                        fontSize: '12px'
                    }
                },
                gridLineWidth: 0,
                tickWidth: 0
            },
            plotOptions: {
                column: {
                    pointPadding: 0.2,
                    borderWidth: 0
                },
                series: {
                    allowPointSelect: true,
                    point:{
                            events: {
                            click: function (event) {
                                if (temppjkey)
                                {
                                    var myurl = 'http://wuxinpi.china.ads.finisar.com/Project/UpdateProjectError2?ProjectKey=' + temppjkey + '&ErrorCode=' + this.category;
                                    window.open(myurl, '_blank');
                                }
                            }
                        }
                    }

                    //events: {
                    //    click: function (event) {
                    //        if (temppjkey)
                    //        {
                    //            var myurl = 'http://wuxinpi.china.ads.finisar.com/Project/UpdateProjectError2?ProjectKey=' + temppjkey + '&ErrorCode=' + event.point.name;
                    //            window.open(myurl, '_blank');
                    //        }
                    //    }
                    //}
                }
            },
            yAxis: [{
                title: {
                    text: 'Amount'
                },
                min: 0,
                style: {
                    fontSize: '12px'
                },
                gridLineWidth: 0
            }, {
                title: {
                    text: 'Percent'
                },
                min: 0,
                max: 100,
                gridLineWidth: 0,
                tickPixelInterval: 36,
                opposite: true
            }],
            exporting: {
                enabled: false
            },
            series: [{
                type: 'column',
                name: 'Amount',
                yAxis:0
            }, 
            {
                type: 'scatter',
                name: 'Percent',
                color: '#FF0066',
                yAxis:1,
                tooltip: {
                    headerFormat: '<b>{point.key}</b><br/>',
                    pointFormat: '{point.y:.2f} %'
                }
            },
            {
                type: 'line',
                name: 'Summary Percent',
                marker: {
                    lineWidth: 2,
                    lineColor: Highcharts.getOptions().colors[3],
                    fillColor: 'green'
                },
                yAxis:1,
                tooltip: {
                    headerFormat: '<b>{point.key}</b><br/>',
                    pointFormat: '{point.y:.2f} %'
                }
            }]
        });

        $.post('/BRTrace/PNErrorDistribution',
        {
            JONum: $('#' + id).val()
        }, function (output) {
            for (var idx = 0; idx < output.length; idx++) {
                mychart.series[0].addPoint({ name: output[idx].Failure, y: output[idx].Amount });
                mychart.series[1].addPoint({ name: output[idx].Failure, y: output[idx].ABPercent });
                mychart.series[2].addPoint({ name: output[idx].Failure, y: output[idx].PPercent });
            }
        });
    }

    return {
        init: function (container, id, pjkey) {
            show(container, id, pjkey);
        }
    };
}();