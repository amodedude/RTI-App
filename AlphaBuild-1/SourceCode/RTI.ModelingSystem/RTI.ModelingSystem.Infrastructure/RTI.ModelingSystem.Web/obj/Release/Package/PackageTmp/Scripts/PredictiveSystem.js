
$(document).ready(function () {
    var SaltSplitStatusFlag = false;
    var ThroughputChartStatusFlag = false;
    $("#trainDisplay").hide();
    $(".loader_xs").css('display', 'block');
    $.ajax({
        type: "GET",
        url: '/PredictiveSystem/PlotSaltSplitChart',
        success: function (data) {
            debugger;
            $('#SaltSplitChart').empty();
            $('#SaltSplitChart').html(data);
            
            SaltSplitStatusFlag = true;

            if (SaltSplitStatusFlag == true && ThroughputChartStatusFlag == true) {
                debugger;
                $("#runCostAnalysis").removeClass('disabled');
            }
        },
        error: function (xhr) {
            window.location.href = "/ClientDatabase/Errorview";
        }
    });

    $.ajax({
        type: "GET",
        url: '/PredictiveSystem/ThroughputChart',
        success: function (data) {
            debugger;
            $('#ThroghPutChart').empty();
            $('#ThroghPutChart').html(data);
            ThroughputChartStatusFlag = true;
            /*----------------- System Conditions ---------------*/
            $.ajax({
                url: '/PredictiveSystem/SystemConditions',
                type: 'GET',
                success: function (data) { 
                    $("#SystemConditions").empty();
                    $("#SystemConditions").html(data);
                    $(".loader_xs").css('display', 'none');
                    $("#trainDisplay").show();
                },
                error: function (xhr) {
                    window.location.href = "/ClientDatabase/Errorview";
                    $(".loader_xs").hide();                    
                }
            });
            if (SaltSplitStatusFlag == true && ThroughputChartStatusFlag == true) {
                $("#runCostAnalysis").removeClass('disabled');
            } 
        },
        error: function (xhr) {
            window.location.href = "/ClientDatabase/Errorview";
        }
    });

    $("#GetPerformanceSettings").on("click", function (e) {
        $.ajax({
            type: "GET",
            url: '/PredictiveSystem/GetPerformanceSettings',
            success: function (data) {
                $("#performSettings").empty();
                $("#performSettings").html(data);
                $("#performSettings").modal('show');
            },
            error: function (xhr) {
                window.location.href = "/ClientDatabase/Errorview";
            }
        });
    });
});
