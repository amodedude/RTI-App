$(window).load(function () {
    var CustomerType = $('#CustomerType')[0].value;
    var CustomerId = $('#CustomerId')[0].value;
    var HasTrainDetails = $('#HasTrainDetails')[0].value;
    var autocomplete = true;

    if (CustomerId != "" && CustomerType == "True" && HasTrainDetails == "Yes") {

        $("#GetSystemConfig").addClass('disabled');

        var SaltSplitStatusFlag = false;
        var ThroughputChartStatusFlag = false;

        var append = '<a id="PredictiveDiv" class="graphclicklink" href="/PredictiveSystem/PredictiveSystemPerformance"><span class="glyphicon glyphicon-zoom-in" aria-hidden="true"></span> Open detailed view</a><div class="graph_click"></div>'
        $.ajax({
            type: "GET",
            url: '/PredictiveSystem/PlotSaltSplitChart',
            data: { IsDashboard: true },
            success: function (data) { 
                $("#SaltSplitPlot").empty();
                
                $("#SaltSplitPlot").html(data);

                SaltSplitStatusFlag = true;

                if (SaltSplitStatusFlag == true && ThroughputChartStatusFlag == true) {
                    $("#GetSystemConfig").removeClass('disabled');
                }
            },
            error: function (xhr) {
                window.location.href = "/ClientDatabase/Errorview";
            }
        });
        LoadConductivityCharts(HasTrainDetails);
        return false;
    }

    else if (CustomerId != "" && CustomerType == "True") {
        if (HasTrainDetails == "Check") {
            $.ajax({
                type: "GET",
                url: '/ClientDatabase/GetSystemSettings',
                success: function (data) {
                    $("#SettingsType").empty();
                    $("#SettingsType").html(data);
                    $('#slideout').toggleClass('on');
                    $('#SystemOrTrain')[0].selectedIndex = "0";
                },
                error: function (xhr) {
                    window.location.href = "/ClientDatabase/Errorview";
                }
            });


        }
        else if (HasTrainDetails == "No") {
            $.ajax({

                type: "GET",
                url: '/ClientDatabase/TrainSettings',
                success: function (data) {
                    $("#SettingsType").empty();
                    $("#SettingsType").html(data);
                    $('#slideout').toggleClass('on');
                    $('#SystemOrTrain')[0].selectedIndex = "1";
                },
                error: function (xhr) {
                    window.location.href = "/ClientDatabase/Errorview";
                }
            });
            LoadConductivityCharts(HasTrainDetails);
            return false;
        }

    }
    else if (CustomerId == "") {
        $.ajax({
            type: "GET",
            url: '/Customer/Create',
            success: function (data) {
                $(".EditableDiv").empty();
                $(".EditableDiv").html(data);
                $('#changeCustomer').show();

            },
            error: function (xhr) {
                window.location.href = "/ClientDatabase/Errorview";
            }
        });
        $('#changeCustomer').modal('show');
    }

    else if (CustomerId != "" && CustomerType == "False") {

        $("#GetSystemConfig").addClass('disabled');

        var SaltSplitStatusFlag = false;
        var ThroughputChartStatusFlag = false;

        if (HasTrainDetails == "Yes") {
            //LoadConductivityCharts();
            //var append = '<a id="PredictiveDiv" class="graphclicklink" href="/PredictiveSystem/PredictiveSystemPerformance"><span class="glyphicon glyphicon-zoom-in" aria-hidden="true"></span> Open detailed view</a><div class="graph_click"></div>'
            $.ajax({
                type: "GET",
                url: '/PredictiveSystem/PlotSaltSplitChart',
                data: { IsDashboard: true },
                success: function (data) {
                    $("#SaltSplitPlot").empty();
                    $("#graph_container2").append(append);
                    $("#SaltSplitPlot").html(data);

                    SaltSplitStatusFlag = true;

                    if (SaltSplitStatusFlag == true && ThroughputChartStatusFlag == true) {
                        $("#GetSystemConfig").removeClass('disabled');
                    }
                },
                error: function (xhr) {
                    window.location.href = "/ClientDatabase/Errorview";
                }
            });
            LoadConductivityCharts(HasTrainDetails);
            return false;
        }
        else if (HasTrainDetails == "No") {
            $.ajax({
                type: "GET",
                url: '/ClientDatabase/TrainSettings',
                success: function (data) {
                    $("#SettingsType").empty();
                    $("#SettingsType").html(data);
                    $('#slideout').toggleClass('on');
                    $('#SystemOrTrain')[0].selectedIndex = "1";
                },
                error: function (xhr) {
                    window.location.href = "/ClientDatabase/Errorview";
                }
            });
            LoadConductivityCharts(HasTrainDetails);
            $("#GetSystemConfig").removeClass('disabled');
        }
    }

});

function LoadConductivityCharts(HasTrainDetails) {
    var Source1Id = $("#Source1Id")[0].value;
    var Source2Id = $("#Source2Id")[0].value;
    var append1 = '<a id="ConductivityDiv" class="graphclicklink" href="/Conductivity/WaterConductivity"><span class="glyphicon glyphicon-zoom-in" aria-hidden="true"></span> Open detailed view</a><div class="graph_click"></div>'
    var append2 = '<a id="PredictiveDiv" class="graphclicklink" href="/PredictiveSystem/PredictiveSystemPerformance"><span class="glyphicon glyphicon-zoom-in" aria-hidden="true"></span> Open detailed view</a><div class="graph_click"></div>'
    if (Source1Id != "") {
        $(function () {
            $.ajax({
                type: 'GET',
                url: '/Conductivity/ConductivityPlot',
                data: { USGSID: Source1Id },
                success: function (jsonData) {
                    var Data = new Array();
                    for (var i = 0 ; i < jsonData.length ; i++) {
                        var ConductivityData = new Object();
                        ConductivityData = jsonData[i];
                        Data.push(ConductivityData.Value);
                    }
                    if (jsonData.length > 0) {
                        var parsedDate = new Date(parseInt(jsonData[0].Key.substr(6,13)));
                        var jsDate = new Date(parsedDate);
                        $('#Conductivity_Source_1').empty();
                        $('#graph_container1').append(append1);
                        $('#Conductivity_Source_1').highcharts(
                            'StockChart', {
                                rangeSelector: {
                                    selected: 5
                                },
                                chart: {
                                    type: 'spline',
                                    zoomType: 'x',
                                    width: 480,
                                    height: 260
                                },
                                //title: {
                                //    text: 'Conductivty Chart',
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
                                    crosshairs: true,
                                    enabled: false
                                },
                                legend: {
                                    enabled: true,
                                    layout: 'horizontal',
                                    borderWidth: 1
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
                                series: [{
                                    name: 'Conductivity',
                                    pointInterval: 24 * 3600 * 1000,
                                    pointStart: Date.UTC(jsDate.getUTCFullYear(), jsDate.getUTCMonth(), jsDate.getUTCDay(), 0, 0, 0, 0),
                                    data: Data
                                }]
                            });

                        /*Load throughput chart only if water source has conductivity data*/
                        if (HasTrainDetails == "Yes") {
                            $.ajax({
                                type: "GET",
                                url: '/PredictiveSystem/ThroughputChart',
                                data: { IsDashboard: true },
                                success: function (data) {
                                    $("#ThroghPutPlot").empty();
                                    $("#ThroghPutPlot").html(data);
                                    $("#graph_container2").append(append2);
                                    //ThroughputChartStatusFlag = true; 
                                    $("#GetSystemConfig").removeClass('disabled');

                                },
                                error: function (xhr) {
                                    window.location.href = "/ClientDatabase/Errorview";
                                }
                            });
                        }
                    }

                    else {
                        $('#Conductivity_Source_1').empty();
                        $("#GetSystemConfig").removeClass('disabled');
                        $('#Conductivity_Source_1').html('<div class="nodataimg" ></div>');
                        $("#ThroghPutPlot").html('<h5>Throughput Forecast</h5><div class="nodataimg" ></div>');
                    }
                }
            });
        });


        if (Source1Id != "") {
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
                        if (jsonData.AverageForecastData.length > 0) {
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
                                    width: 480,
                                    height: 260
                                },
                                //title: {
                                //    text: 'Forecast Chart',
                                //},
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
                                                    Highcharts.charts[1].xAxis[0].setExtremes(utc_timestamp_today, utc_timestamp_1moFromNow)
                                                }, 1);
                                            }
                                            else if (e.trigger == "rangeSelectorButton" && e.rangeSelectorButton.text == "3m") {
                                                setTimeout(function () {
                                                    Highcharts.charts[1].xAxis[0].setExtremes(utc_timestamp_today, utc_timestamp_3moFromNow)
                                                }, 1);
                                            }
                                            else if (e.trigger == "rangeSelectorButton" && e.rangeSelectorButton.text == "6m") {
                                                setTimeout(function () {
                                                    Highcharts.charts[1].xAxis[0].setExtremes(utc_timestamp_today, utc_timestamp_6moFromNow)
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
                                legend: {
                                    enabled: true,
                                    layout: 'horizontal',
                                    borderWidth: 1
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
                                series: [{
                                    name: 'WorstCase',
                                    pointInterval: 24 * 3600 * 1000,
                                    pointStart: utc_timestamp_today,
                                    data: WorstCase,
                                    color: '#FF0000'
                                }, {
                                    name: 'Expected',
                                    pointInterval: 24 * 3600 * 1000,
                                    pointStart: utc_timestamp_today,
                                    data: BestCase
                                }]
                            });
                        }                   
                        else
                        {
                            $('#Forecast_Source_1').html('<div class="nodataimg" ></div>');
                        }
                   }
                });
            });
        }

    }
}
