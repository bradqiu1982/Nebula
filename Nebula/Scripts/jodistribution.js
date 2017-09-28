var JoDistribution = function () {
    var show = function (container, id) {
        var mychart = new Highcharts.chart(container, {
            chart: {
                type: 'column',
                backgroundColor: "#001D2E",
                borderColor: "#001D2E"
            },
            colors: [
                '#FFC000', '#FF0000', '#0070C0',
                '#FF3399', '#FA9604', '#00B050',
                '#7030A0',
            ],
            title: {
                text: 'JO Distribution',
                style: {
                    color: '#afafaf',
                    fontSize: '28px'
                }
            },
            xAxis: {
                type: 'category',
                labels: {
                    rotation: -45,
                    style: {
                        fontSize: '12px'
                    }
                },
                gridLineWidth: 0,
                tickWidth: 0
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'Module Amount'
                },
                style: {
                    fontSize: '12px'
                },
                gridLineWidth: 0
            },
            legend: {
                enabled: false
            },
            tooltip: {
                pointFormat: 'Module: <b>{point.y}</b>'
            },
            plotOptions: {
                series: {
                    borderWidth: 0,
                    colorByPoint: true
                }
            },
            exporting: {
                enabled: false
            },
            series: [{
                name: 'Module',
                dataLabels: {
                    enabled: true,
                    rotation: -90,
                    align: 'right',
                    format: '{point.y:.1f}',
                    y: 10,
                    style: {
                        fontSize: '12px'
                    }
                }
            }]
        });

        $.post('/BRTrace/JoDistributionData',
        {
            JONum: $('#' + id).val()
        }, function (output) {
            for (var idx = 0; idx < output.length; idx++) {
                mychart.series[0].addPoint({ name: output[idx].Station, y: output[idx].Amount });
            }
        });
    }

    return {
        init: function (container, id) {
            show(container, id);
        }
    };
}();