$(document).ready(function () {
    var Source1Id = $("#Source1Id")[0].value;
    var Source2Id = $("#Source2Id")[0].value;
    $(function () {
        $.ajax({
            type: 'GET',
            url: '/Conductivity/ConductivityPlot',
            data: { USGSID: Source1Id },
            success: function (jsonData) {
                //if (!jsonData.length > 0)
                //{
                //    $('#Conductivity_Source_1_nodata').display = 'block';
                //}
                var Data = new Array();
                for (var i = 0 ; i < jsonData.length ; i++) {
                    var ConductivityData = new Object();
                    ConductivityData = jsonData[i];
                    Data.push(ConductivityData.Value);
                }
                $('#Conductivity_Source_1').empty();
                $('#Conductivity_Source_1').highcharts(
                    'StockChart', {

                        rangeSelector: {
                            selected: 5
                        },

                    
                    chart: {
                        type: 'spline',
                        zoomType: 'x',
                        width: 630,
                        height: 300
                    },
                    //title: {
                    //    text: 'Conductivty Chart',
                    //    //x: -20 //center
                    //},
                    xAxis: {
                        type: 'datetime',
                        tickInterval: 24 * 3600 * 1000 * 90,
                        title: {
                            text: 'Date'
                        }
                    },                  
                    yAxis: {
                        title: {
                            text: 'Conductivity'
                        }
                    },
                    credits: {
                        enabled: false
                    },
                    tooltip: {
                        shared: true,
                        crosshairs: true
                    },
                    plotOptions: {
                        series: {
                            cursor: 'pointer',
                            point: {
                                events: {
                                    click: function (e) {
                                        
                                    }
                                }
                            },
                            marker: {
                                enabled: false,
                                lineWidth: 1
                            }
                        }
                    },
                    legend: {
                        enabled: true,
                        layout: 'vertical',
                        borderWidth: 1
                    },
                    series: [{
                        name: 'Conductivity',
                        pointInterval: 24 * 3600 * 1000,
                        pointStart: Date.UTC(2012, 01, 01, 0, 0, 0, 0),
                        data: Data
                    }]
                });
            }
        });
    });

    if (Source2Id != "") {
        $(function () {
            $.ajax({
                type: 'GET',
                url: '/Conductivity/ConductivityPlot',
                data: { USGSID: Source2Id },
                success: function (jsonData) {
                    var Data = new Array();
                    for (var i = 0 ; i < jsonData.length ; i++) {
                        var ConductivityData = new Object();
                        ConductivityData = jsonData[i];
                        Data.push(ConductivityData.Value);
                    }
                    $('#Conductivity_Source_2').empty();
                    $('#Conductivity_Source_2').highcharts('StockChart', {

                        rangeSelector: {
                            selected: 5
                        },
                        chart: {
                            type: 'spline',
                            zoomType: 'x',
                            width: 630,
                            height: 300
                        },
                        //title: {
                        //    text: 'Conductivty Chart',
                        //    //x: -20 //center
                        //},
                        xAxis: {
                            type: 'datetime',
                            tickInterval: 24 * 3600 * 1000 * 90,
                            title: {
                                text: 'Date'
                            }
                        },
                        yAxis: {
                            title: {
                                text: 'Conductivity'
                            }
                        },
                        credits: {
                            enabled: false
                        },
                        tooltip: {
                            shared: true,
                            crosshairs: true
                        },
                        plotOptions: {
                            series: {
                                cursor: 'pointer',
                                point: {
                                    events: {
                                        click: function (e) {
                                            
                                        }
                                    }
                                },
                                marker: {
                                    enabled: false,
                                    lineWidth: 1
                                }
                            }
                        },
                        legend: {
                            enabled: true,
                            layout: 'vertical',
                            borderWidth: 1
                        },
                        series: [{
                            name: 'Conductivity',
                            pointInterval: 24 * 3600 * 1000,
                            pointStart: Date.UTC(2012, 01, 01, 0, 0, 0, 0),
                            data: Data
                        }]
                    });
                }
            });
        });
    }
    $(function () {
        var now = new Date();
        var utc_timestamp = Date.UTC(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
        
        $.ajax({
            type: 'GET',
            url: '/Conductivity/ForecastPlot',
            data: { USGSID: Source1Id },
            success: function (jsonData) {
                var BestCase = new Array();
                var WorstCase = new Array();
                for (var i = 0 ; i < jsonData.AverageForecastData.length ; i++) {
                    var BestData = new Object();
                    var WorstData = new Object();
                    BestData = jsonData.AverageForecastData[i];
                    WorstData = jsonData.MaximumForecastData[i];
                    BestCase.push(BestData.cond);
                    WorstCase.push(WorstData.cond)
                }
                $('#Forecast_Source_1').empty();
                $('#Forecast_Source_1').highcharts('StockChart', {

                    rangeSelector: {
                        selected: 1,
                    },
                    chart: {
                        type: 'spline',
                        zoomType: 'x',
                        width: 630,
                        height: 300
                    },
                    //title: {
                    //    text: 'Forecast Chart',
                    //    //x: -20 //center
                    //},
                    xAxis: {
                        type: 'datetime',
                        tickInterval: 24 * 3600 * 1000 * 21,
                        title: {
                            text: 'Date'
                        }
                    },
                    yAxis: {
                        title: {
                            text: 'Conductivity'
                        }
                    },
                    credits: {
                        enabled: false
                    },
                    tooltip: {
                        shared: true,
                        crosshairs: true
                    },
                    plotOptions: {
                        series: {
                            cursor: 'pointer',
                            point: {
                                events: {
                                    click: function (e) {
                                        
                                    }
                                }
                            },
                            marker: {
                                enabled:false,
                                lineWidth: 1
                            }
                        }
                    },
                    legend: {
                        enabled: true,
                        layout: 'horizontal',
                        borderWidth: 1
                    },
                    series: [{
                        name: 'Expected',
                        pointInterval: 24 * 3600 * 1000,
                        pointStart: utc_timestamp,
                        data: BestCase
                    }, {
                        name: 'WorstCase',
                        pointInterval: 24 * 3600 * 1000,
                        pointStart: utc_timestamp,
                        data: WorstCase,
                        color: '#FF0000'
                    }]
                });
            }
        });
    });












    //$.ajax({
    //    type: 'GET',
    //    url: '/Conductivity/ConductivityPlot',
    //    data: { USGSID: Source1Id },
    //    contentType: 'application/json; charset=utf-8',
    //    dataType: 'json',
    //    success: function (jsonData) {
    //        var arr = new Array();
    //        for (var i = 0 ; i < jsonData.length ; i++) {
    //            arr.push(Object.keys(jsonData[i]).map(
    //                function (argMap) {
    //                    return (jsonData[i])[argMap]
    //                })
    //                );
    //        }
            
    //            $('#Conductivity_Source_1').highcharts({
    //                chart: {
    //                    type: 'spline'
    //                },
    //                xAxis: {
    //                    type: 'datetime'
    //                },
    //                yAxis: {
    //                    title: {
    //                        text: 'Conductivity'
    //                    },
    //                    labels: {
    //                        formatter: function () {
    //                            return this.value;
    //                        }
    //                    },
    //                    lineWidth: 2
    //                },
    //                legend: {
    //                    enabled: false
    //                },
    //                tooltip: {
    //                    headerFormat: '<b>{series.name}</b><br/>',
    //                    pointFormat: '{point.x} km: {point.y}°C'
    //                },
    //                plotOptions: {
    //                    spline: {
    //                        marker: {
    //                            enable: false
    //                        }
    //                    }
    //                },
    //                series: [{
    //                    name: 'Conductivity',
    //                    data: arr
    //                }]
    //            });


    //    },
    //    error: function () {
    //        alert('Error: ' + xhr.statusText);
    //    }
    //});
    

})