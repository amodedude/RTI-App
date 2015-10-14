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
                $("#graph_container2").append(append);
                $("#SaltSplitPlot").html(data);

                SaltSplitStatusFlag = true;

                if (SaltSplitStatusFlag == true && ThroughputChartStatusFlag == true) {
                    $("#GetSystemConfig").removeClass('disabled');
                }
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });
        $.ajax({
            type: "GET",
            url: '/PredictiveSystem/ThroughputChart',
            data: { IsDashboard: true },
            success: function (data) {
                $("#ThroghPutPlot").empty();
                $("#ThroghPutPlot").html(data);

                ThroughputChartStatusFlag = true;

                if (SaltSplitStatusFlag == true && ThroughputChartStatusFlag == true) {
                    $("#GetSystemConfig").removeClass('disabled');
                }
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });
        LoadConductivityCharts();
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
                    alert('Error: ' + xhr.statusText);
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
                    alert('Error: ' + xhr.statusText);
                }
            });


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
                alert('Error: ' + xhr.statusText);
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
            var append = '<a id="PredictiveDiv" class="graphclicklink" href="/PredictiveSystem/PredictiveSystemPerformance"><span class="glyphicon glyphicon-zoom-in" aria-hidden="true"></span> Open detailed view</a><div class="graph_click"></div>'
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
                    alert('Error: ' + xhr.statusText);
                }
            });
            $.ajax({
                type: "GET",
                url: '/PredictiveSystem/ThroughputChart',
                data: { IsDashboard: true },
                success: function (data) {
                    $("#ThroghPutPlot").empty();
                    $("#ThroghPutPlot").html(data);

                    ThroughputChartStatusFlag = true;

                    if (SaltSplitStatusFlag == true && ThroughputChartStatusFlag == true) {
                        $("#GetSystemConfig").removeClass('disabled');
                    }
                },
                error: function (xhr) {
                    alert('Error: ' + xhr.statusText);
                }
            });
            LoadConductivityCharts();
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
                    alert('Error: ' + xhr.statusText);
                }
            });
            LoadConductivityCharts();
            $("#GetSystemConfig").removeClass('disabled');
        }
    }

});

function LoadConductivityCharts() {
    var Source1Id = $("#Source1Id")[0].value;
    var Source2Id = $("#Source2Id")[0].value;
    var append = '<a id="ConductivityDiv" class="graphclicklink" href="/Conductivity/WaterConductivity"><span class="glyphicon glyphicon-zoom-in" aria-hidden="true"></span> Open detailed view</a><div class="graph_click"></div>'
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
                        $('#Conductivity_Source_1').empty();
                        $('#graph_container1').append(append);
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
                                    name: 'Conductivity',
                                    pointInterval: 24 * 3600 * 1000,
                                    pointStart: Date.UTC(2012, 01, 01, 0, 0, 0, 0),
                                    data: Data
                                }]
                            });
                    }

                    else {
                        $('#Conductivity_Source_1').empty();
                        $('#Conductivity_Source_1').html('<div class="nodataimg" ></div>');
                    }
                }
            });
        });


        if (Source1Id != "") {
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
                        if (jsonData.AverageForecastData.length > 0) {
                            $('#Forecast_Source_1').highcharts('StockChart', {

                                rangeSelector: {
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
