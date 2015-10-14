
$(document).ready(function () {
    var SaltSplitStatusFlag = false;
    var ThroughputChartStatusFlag = false;
    $(".loader_xs").css('display', 'block');
    $.ajax({
        type: "GET",
        url: '/PredictiveSystem/PlotSaltSplitChart',
        success: function (data) {
            $('#SaltSplitChart').empty();
            $('#SaltSplitChart').html(data);

            SaltSplitStatusFlag = true;

            if (SaltSplitStatusFlag == true && ThroughputChartStatusFlag == true) {
                $("#runCostAnalysis").removeClass('disabled');
            }

        },
        error: function (xhr) {
            alert('Error: ' + xhr.statusText);
        }
    });

    $.ajax({
        type: "GET",
        url: '/PredictiveSystem/ThroughputChart',
        success: function (data) {
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
                },
                error: function (xhr) {
                    alert('Error: ' + xhr.statusText);
                    $(".loader_xs").hide();
                }
            });
            if (SaltSplitStatusFlag == true && ThroughputChartStatusFlag == true) {
                $("#runCostAnalysis").removeClass('disabled');
            }

        },
        error: function (xhr) {
            alert('Error: ' + xhr.statusText);
        }
    });



    //$.ajax({
    //    url: '/PredictiveSystem/SystemConditions',
    //    type: 'GET',
    //    success: function (data) {
    //        $("#SystemConditions").html(data);
    //    },
    //    error: function (xhr) {
    //        alert('Error: ' + xhr.statusText);
    //    }
    //});

    $(document).on("click", "#GetPerformanceSettings", function (e) {
        $.ajax({
            type: "GET",
            url: '/PredictiveSystem/GetPerformanceSettings',
            success: function (data) {
                $("#performSettings").empty();
                $("#performSettings").html(data);
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });
    });
});
