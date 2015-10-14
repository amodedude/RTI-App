$(function () {
    $.ajax({
        url: 'PlotCostAnalyzerChart',
        type: "GET",
        dataType: "json",
        success: function (jsonData) {
            $.ajax({
                url: 'GetCumulativeSavings',
                type: 'GET',
                success: function (CumulativeSavingsData) {
                    $("#CumulativeSavings").empty();
                    $("#CumulativeSavings").html(CumulativeSavingsData);
                },
                error: function (xhr) {
                    window.location.href = "/ClientDatabase/Errorview";
                }
            });
            var arrWithOutClean = new Array();
            var arrWithClean = new Array();
            for (var i = 0 ; i < jsonData.length-1 ; i++) {
                var mWithOutClean = new Object();
                var mWithClean = new Object();
                mWithOutClean = jsonData[i];
                mWithClean = jsonData[i];
                arrWithOutClean.push(mWithOutClean.Item2);
                arrWithClean.push(mWithClean.Item3);
            }
            var XaxisStartingPoint = jsonData[0].Item1-1; 
            //alert(arr);
            $('#graph_CostAnalyzer').empty();
            $('#graph_CostAnalyzer').highcharts({
                chart: {
                    zoomType: 'x'
                },
                title: {
                    text: 'Cost Analyzer',
                    x: -20 //center
                },
                xAxis: {
                    title: {
                        text: 'Number of Weeks'
                    }

                },
                yAxis: {
                    title: {
                        text: 'Cost of Operations'
                    }
                },
                credits: {
                    enabled: false
                },
                tooltip: {
                    crosshairs: true,
                    formatter: function () {
                        return 'Week Number : ' + this.point.x + '<br>' + this.series.name + ' : ' + Highcharts.numberFormat(this.point.y, 2) + '<br>' + 'Click on chart to get cost analysis';
                    }

                },
                plotOptions: {
                    series: {
                        cursor: 'pointer',
                        point: {
                            events: {
                                click: function (e) {
                                    $("#CostAnalyzerResultsTable").fadeOut(300);
                                    $.ajax({
                                        url: 'GetResultsTable',
                                        type: "GET",
                                        data: { weekNumber: this.x },
                                        success: function (CostAnalyzerResultsData) {
                                            //alert(CostAnalyzerResultsData);
                                            $("#CostAnalyzerResultsTable").empty();
                                            $("#CostAnalyzerResultsTable").html(CostAnalyzerResultsData);
                                            //FADEOUT AND FADEIN           
                                            $("#CostAnalyzerResultsTable").fadeIn(400);
                                        }
                                    });
                                }
                            }
                        },
                        marker: {
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
                    pointStart: XaxisStartingPoint,
                    name: 'With RTI Cleaning',
                    data: arrWithClean
                },
                {
                    pointStart: XaxisStartingPoint,
                    name: 'Without RTI Cleaning',
                    data: arrWithOutClean
                }]
            });
        }
    });
});

$(document).on("click", "#updateCostModel", function () {

    var acidPrice = $('#AcidPrice').text().replace('$', '');
    var causticPrice = $('#CausticPrice').text().replace('$', '');
    var acidUsage = $('#AcidUsage').text();
    var causticUsage = $('#CausticUsage').text();
    var acidPercentage = $('#AcidPercentage').text().replace('%', '');
    var causticPercentage = $('#CausticPercentage').text().replace('%', '');
    var cationResin = $('#CationResin').text();
    var anionResin = $('#AnionResin').text();
    var cationCleaningPrice = $('#CationCleaningPrice').text().substr(0, $('#resinLifeExpectancy').text().indexOf(" "));
    var anionCleaningPrice = $('#AnionCleaningPrice').text().substr(0, $('#resinLifeExpectancy').text().indexOf(" "));
    var cationCleaningDiscount = $('#CationCleaningDiscount').text().replace('%', '');
    var anionCleaningDiscount = $('#AnionCleaningDiscount').text().replace('%', '');
    var cationReplacementPrice = $('#CationReplacementPrice').text().substr(0, $('#resinLifeExpectancy').text().indexOf(" "));
    var anionReplacementPrice = $('#AnionReplacementPrice').text().substr(0, $('#resinLifeExpectancy').text().indexOf(" "));

    //var jsonData = {
    //    ResinLifeExpectancy: resinLifeExpectancy, AvgResinAge: avgResinAge, NewResinSaltSplit: newResinSaltSplit, RegenEffectiveness: regenEffectiveness,
    //    MaxDegradation: maxDegradation, RticleaningThreshold: rticleaningThreshold, ResinReplacementLevel: resinReplacementLevel, SourcePredictibilty: sourcePredictibilty,
    //    NoOfIterations: noOfIterations, StandardDeviationInterval: standardDeviationInterval, ReplaceResin: replaceResin, CalculationMethod: calcMethod, TrainId: trainId
    //};

    //jsonData = JSON.stringify(jsonData);
    $.ajax({
        type: "GET",
        data: {
            acidPrice: acidPrice, causticPrice: causticPrice,
            acidUsage: acidUsage, causticUsage: causticUsage,
            cationResin: cationResin, anionResin: anionResin,
            loadOnSettingsUpdate: true
        },
        dataType: "json",
        url: '/CostAnalyzer/PlotCostAnalyzerChart',
        success: function (jsonData) {
            $.ajax({
                url: 'GetCumulativeSavings',
                type: 'GET',
                success: function (CumulativeSavingsData) {
                    $("#CumulativeSavings").empty();
                    $("#CumulativeSavings").html(CumulativeSavingsData);
                },
                error: function (xhr) {
                    window.location.href = "/ClientDatabase/Errorview";
                }
            });
            var arrWithOutClean = new Array();
            var arrWithClean = new Array();
            for (var i = 0 ; i < jsonData.length-1 ; i++) {
                var mWithOutClean = new Object();
                var mWithClean = new Object();
                mWithOutClean = jsonData[i];
                mWithClean = jsonData[i];
                arrWithOutClean.push(mWithOutClean.Item2);
                arrWithClean.push(mWithClean.Item3);
            }
            //alert(arr);
            var XaxisStartingPoint = jsonData[0].Item1-1;
            $('#graph_CostAnalyzer').empty();
            $('#graph_CostAnalyzer').highcharts({
                chart: {
                    zoomType: 'x'
                },
                title: {
                    text: 'Cost Analyzer',
                    x: -20 //center
                },
                xAxis: {
                    title: {
                        text: 'Number of Weeks'
                    }
                },
                yAxis: {
                    title: {
                        text: 'Cost of Operations'
                    }
                },
                credits: {
                    enabled: false
                },
                tooltip: {
                    crosshairs: true,
                    formatter: function () {
                        return 'Week Number : ' + this.point.x + '<br>' + this.series.name + ' : ' + Highcharts.numberFormat(this.point.y, 2) + '<br>' + 'Click on chart to get cost analysis';
                    }
                },
                plotOptions: {
                    series: {
                        cursor: 'pointer',
                        point: {
                            events: {
                                click: function (e) {

                                    $("#CostAnalyzerResultsTable").fadeOut(300);
                                    $.ajax({
                                        url: 'GetResultsTable',
                                        type: "GET",
                                        data: { weekNumber: this.x },
                                        success: function (CostAnalyzerResultsData) {
                                            //alert(CostAnalyzerResultsData);
                                            $("#CostAnalyzerResultsTable").empty();
                                            $("#CostAnalyzerResultsTable").html(CostAnalyzerResultsData);
                                            //FADEOUT AND FADEIN           
                                            $("#CostAnalyzerResultsTable").fadeIn(400);
                                        }
                                    });
                                }
                            }
                        },
                        marker: {
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
                    pointStart: XaxisStartingPoint,
                    name: 'With RTI Cleaning',
                    data: arrWithClean
                },
                {
                    pointStart: XaxisStartingPoint,
                    name: 'Without RTI Cleaning',
                    data: arrWithOutClean
                }]
            });
        }
    });
});