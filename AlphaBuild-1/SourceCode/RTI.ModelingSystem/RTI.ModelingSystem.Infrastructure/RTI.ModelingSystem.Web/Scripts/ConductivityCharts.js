$(document).ready(function () {
    var Source1Id = $("#Source1Id")[0].value;
    var Source2Id = $("#Source2Id")[0].value;
    $(function () {
        $.ajax({
            type: 'GET',
            url: '/Conductivity/ConductivityPlot',
            data: { USGSID: Source1Id },
            success: function (jsonData) {
                var Data = new Array();
                for (var i = 0 ; i < jsonData.length ; i++) {
                    var parsedDate1 = new Date(parseInt(jsonData[0].Key.substr(6, 13)));
                    var jsDate1 = new Date(parsedDate1);
                    var ConductivityData = new Object();
                    ConductivityData = [parseInt(jsonData[i].Key.substr(6, 13)), jsonData[i].Value];
                    Data.push(ConductivityData);
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
                    xAxis: {
                        type: 'datetime',
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
                        crosshairs: true,
                        pointFormat: '<b>{series.name}: {point.y:,.2f}</b><br>'
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
                    var parsedDate1 = new Date(parseInt(jsonData[0].Key.substr(6, 13)));
                    var jsDate1 = new Date(parsedDate1);
                    var ConductivityData = new Object();
                    ConductivityData = [parseInt(jsonData[i].Key.substr(6, 13)), jsonData[i].Value];
                    Data.push(ConductivityData);
                }
                    $('#Conductivity_Source_2').empty();
                    if (jsonData.length > 0) {
                        var parsedDate2 = new Date(parseInt(jsonData[0].Key.substr(6, 13)));
                        var jsDate2 = new Date(parsedDate2);
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
                            xAxis: {
                                type: 'datetime',
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
                                crosshairs: true,
                                pointFormat: '<b>{series.name}: {point.y:,.2f}</b><br>'
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
                                data: Data
                            }]
                        });
                    }
                    else {
                        $('#Conductivity_Source_2').html('<div class="nodataimg" ></div>');
                    }
                }
            });
        });
    }
    $(function () {
        var now = new Date();
        now.setHours(0, 0, 0, 0);;
        now.setMinutes(0);

        var plus1mo = new Date();
        plus1mo.setMonth((now.getMonth() + 1));
        plus1mo.setHours(0, 0, 0, 0);
        plus1mo.setMinutes(0);

        var plus3mo = new Date();
        plus3mo.setMonth((now.getMonth() + 3));
        plus3mo.setHours(0, 0, 0, 0);
        plus3mo.setMinutes(0);

        var plus6mo = new Date();
        plus6mo.setMonth((now.getMonth() + 6));
        plus6mo.setHours(0, 0, 0, 0);
        plus6mo.setMinutes(0);

        var utc_timestamp_today = Date.UTC(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
        var utc_timestamp_1moFromNow = Date.UTC(plus1mo.getFullYear(), plus1mo.getMonth(), plus1mo.getDate(), 0, 0, 0, 0);
        var utc_timestamp_3moFromNow = Date.UTC(plus3mo.getFullYear(), plus3mo.getMonth(), plus3mo.getDate(), 0, 0, 0, 0);
        var utc_timestamp_6moFromNow = Date.UTC(plus6mo.getFullYear(), plus6mo.getMonth(), plus6mo.getDate(), 0, 0, 0, 0);

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
                        buttons: [{
                            type: 'month',
                            count: 1,
                            text: '1m'
                        }, {
                            type: 'month',
                            count: 3,
                            text: '3m'
                        }, {
                            type: 'month',
                            count: 6,
                            text: '6m'
                        }, {
                            type: 'all',
                            text: 'All'
                        }],
                        selected: 1
                    },
                    chart: {
                        type: 'spline',
                        zoomType: 'x',
                        width: 630,
                        height: 300
                    },
                    xAxis: {
                        type: 'datetime',
                        tickInterval: 24 * 3600 * 1000 * 21,
                        min: utc_timestamp_today,
                        max: utc_timestamp_3moFromNow,
                        title: {
                            text: 'Date'
                        },
                        events: {
                            afterSetExtremes: function (e) {
                                if (e.trigger == "rangeSelectorButton" && e.rangeSelectorButton.text == "1m") {
                                    setTimeout(function () {

                                        // Find the index of the forecast chart (needed to find the correct index when there are TWO sources!)
                                        var numOfCharts = Highcharts.charts.length;
                                        var chartIndex; 
                                        for (var i = 0; i < numOfCharts; i++) {
                                            if (Highcharts.charts[i].series[0].name == "WorstCase") {
                                                chartIndex = i;
                                                break;
                                            }
                                        }
                                        // Set the proper timespan                                         
                                        Highcharts.charts[chartIndex].xAxis[0].setExtremes(utc_timestamp_today, utc_timestamp_1moFromNow);
                                        
                                    }, 1);
                                }
                                else if (e.trigger == "rangeSelectorButton" && e.rangeSelectorButton.text == "3m") {
                                    setTimeout(function () {
                                        // Find the index of the forecast chart (needed to find the correct index when there are TWO sources!)
                                        var numOfCharts = Highcharts.charts.length;
                                        var chartIndex;
                                        for (var i = 0; i < numOfCharts; i++) {
                                            if (Highcharts.charts[i].series[0].name == "WorstCase") {
                                                chartIndex = i;
                                                break;
                                            }
                                        }
                                        // Set the proper timespan  
                                        Highcharts.charts[chartIndex].xAxis[0].setExtremes(utc_timestamp_today, utc_timestamp_3moFromNow);
                                        
                                    }, 1)
                                }
                                else if (e.trigger == "rangeSelectorButton" && e.rangeSelectorButton.text == "6m") {
                                    setTimeout(function () {
                                        
                                        // Find the index of the forecast chart (needed to find the correct index when there are TWO sources!)
                                        var numOfCharts = Highcharts.charts.length;
                                        var chartIndex;
                                        for (var i = 0; i < numOfCharts; i++) {
                                            if (Highcharts.charts[i].series[0].name == "WorstCase") {
                                                chartIndex = i;
                                                break;
                                            }
                                        }
                                        // Set the proper timespan  
                                        Highcharts.charts[chartIndex].xAxis[0].setExtremes(utc_timestamp_today, utc_timestamp_6moFromNow)
                                        
                                    }, 1);
                                }
                            }
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
                        name: 'WorstCase',
                        pointInterval: 24 * 3600 * 1000,
                        pointStart: utc_timestamp_today,
                        data: WorstCase,
                        color: '#FF0000'
                    },{
                        name: 'Expected',
                        pointInterval: 24 * 3600 * 1000,
                        pointStart: utc_timestamp_today,
                        data: BestCase
                    }]
                });
            }
        });
    });
})