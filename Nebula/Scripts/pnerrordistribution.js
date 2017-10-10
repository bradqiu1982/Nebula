var pnerrordistribution = function () {
    var show = function (container, id) {
        var mychart = new Highcharts.chart(container, {
            title: {
                text: 'PN Failure Pareto'
            },
            chart:
            {
                alignTicks: false
            },
            xAxis: {
                type: 'category',
                crosshair: true
            },
            plotOptions: {
                column: {
                    pointPadding: 0.2,
                    borderWidth: 0
                }
            },
            yAxis: [{
                title: {
                    text: 'Amount'
                },
                min: 0
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
            series: [{
                type: 'column',
                name: 'Amount',
                yAxis:0
            }, 
            {
                type: 'scatter',
                name: 'Percent',
                color: 'rgba(223, 83, 83, .5)',
                yAxis:1,
                tooltip: {
                    headerFormat: '<b>{point.key}</b><br/>',
                    pointFormat: '{point.y} %'
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
                yAxis:1
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
        init: function (container, id) {
            show(container, id);
        }
    };
}();