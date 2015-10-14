

jQuery(document).ready(function () {

    $(document).off("click").on("click", "#updatePerformanceModel", function () {
        var resinLifeExpectancy = $('#resinLifeExpectancy').text().substr(0, $('#resinLifeExpectancy').text().indexOf(" "));
        var avgResinAge = $('#avgResinAge').text();
        var newResinSaltSplit = $('#newResinSaltSplit').text();
        var regenEffectiveness = $('#regenEffectiveness').text().replace('%', '');
        var maxDegradation = $('#maxDegradation').text().replace('%', '');
        var rticleaningThreshold = $('#RTIcleaningThreshold').text().replace('kg/cu ft', '');
        var resinReplacementLevel = $('#resinReplacementLevel').text().replace('kg/cu ft', '');
        var sourcePredictibilty = $('#sourcePredictibilty').text().replace('%', '');
        var noOfIterations = $('#noOfIterations').text();
        var standardDeviationInterval = $('#standardDeviationInterval').text();
        var replaceResin = $('#replaceResin').is(":checked");
        var calcMethod = $('#calcMethod :selected').val();
        var trainId = $('#trainList').val();
        var rticleaningeffectiveness = $('#RTICleaningEffectiveness').text().replace('%', '');
        var jsonData = {
            ResinLifeExpectancy: resinLifeExpectancy, AvgResinAge: avgResinAge, NewResinSaltSplit: newResinSaltSplit, RegenEffectiveness: regenEffectiveness,
            MaxDegradation: maxDegradation, RticleaningThreshold: rticleaningThreshold, ResinReplacementLevel: resinReplacementLevel, SourcePredictibilty: sourcePredictibilty,
            NoOfIterations: noOfIterations, StandardDeviationInterval: standardDeviationInterval, ReplaceResin: replaceResin, CalculationMethod: calcMethod, TrainId: trainId
        };

        jsonData = JSON.stringify(jsonData);
        $(".loader_xs").css('display', 'block');
        $.ajax({
            type: "GET",
            data: {
                numWeeks: resinLifeExpectancy, AvgResinage: avgResinAge,
                startingSS: newResinSaltSplit, maxDegSS: maxDegradation,
                SelectedTrain: trainId, CleaningEffectiveness: rticleaningThreshold, CleaningEffectiveness: rticleaningeffectiveness
            },
            url: '/PredictiveSystem/PlotSaltSplitChart',
            success: function (data) {
                $('#SaltSplitChart').empty();
                $('#SaltSplitChart').html(data);
            },
            error: function (xhr) {
                window.location.href = "/ClientDatabase/Errorview";
            }
        });

        $.ajax({
            type: "GET",
            data: {
                startingSS: newResinSaltSplit, resinLifeExpectancy: resinLifeExpectancy, simulationconfidence: sourcePredictibilty, num_simulation_iterations: noOfIterations,
                simMethod: calcMethod, stdDev_threshold: standardDeviationInterval, resinAge: avgResinAge, MaxDegradation: maxDegradation, Replacement_Level: resinReplacementLevel,
                RTIcleaning_Level: rticleaningThreshold, ReGen_effectivness: regenEffectiveness, SelectedTrain: trainId, DontReplaceResin: replaceResin, CleaningEffectiveness: rticleaningeffectiveness
            },
            url: '/PredictiveSystem/ThroughputChart',
            success: function (data) {
                $("#ThroghPutChart").empty();
                $("#ThroghPutChart").html(data);
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
                        window.location.href = "/ClientDatabase/Errorview";
                        $(".loader_xs").css('display', 'none');
                    }
                });
            },
            error: function (xhr) {
                window.location.href = "/ClientDatabase/Errorview";
            }
        });
    })

    var lifeExpectancy = $('#LifeExpectancy').val();
    var ResinAge = $('#ResinAge').val();
    if (parseInt(ResinAge) > parseInt(lifeExpectancy))
        {
        ResinAge = lifeExpectancy;
        }

    $("#resinLifeExpectancySlider").slider({
        range: "min",
        min: 0,
        max: 520,
        step: 52,
        slide: function (event, ui) {
            $("#resinLifeExpectancy").text(ui.value);
            if ($("#resinLifeExpectancy").text() == "0")
                $("#resinLifeExpectancy").text($("#resinLifeExpectancy").text() + " (0yr)");
            else if ($("#resinLifeExpectancy").text() == "52")
                $("#resinLifeExpectancy").text($("#resinLifeExpectancy").text() + " (1yr)");
            else if ($("#resinLifeExpectancy").text() == "104")
                $("#resinLifeExpectancy").text($("#resinLifeExpectancy").text() + " (2yrs)");
            else if ($("#resinLifeExpectancy").text() == "156")
                $("#resinLifeExpectancy").text($("#resinLifeExpectancy").text() + " (3yrs)");
            else if ($("#resinLifeExpectancy").text() == "208")
                $("#resinLifeExpectancy").text($("#resinLifeExpectancy").text() + " (4yrs)");
            else if ($("#resinLifeExpectancy").text() == "260")
                $("#resinLifeExpectancy").text($("#resinLifeExpectancy").text() + " (5yrs)");
            else if ($("#resinLifeExpectancy").text() == "312")
                $("#resinLifeExpectancy").text($("#resinLifeExpectancy").text() + " (6yrs)");
            else if ($("#resinLifeExpectancy").text() == "364")
                $("#resinLifeExpectancy").text($("#resinLifeExpectancy").text() + " (7yrs)");
            else if ($("#resinLifeExpectancy").text() == "416")
                $("#resinLifeExpectancy").text($("#resinLifeExpectancy").text() + " (8yrs)");
            else if ($("#resinLifeExpectancy").text() == "468")
                $("#resinLifeExpectancy").text($("#resinLifeExpectancy").text() + " (9yrs)");
            else if ($("#resinLifeExpectancy").text() == "520")
                $("#resinLifeExpectancy").text($("#resinLifeExpectancy").text() + " (10yrs)");
            if (ui.value < $('#avgResinAge').text()) {
                $("#avgResinAge").text(ui.value);
                $("#avgResinAgeSlider").slider("value", ui.value);
            }
        }
    });
    $("#resinLifeExpectancySlider").slider("value", lifeExpectancy);
    $("#resinLifeExpectancy").text($("#resinLifeExpectancySlider").slider("value") + " ("+ lifeExpectancy/52 +"yrs)");

    $("#avgResinAgeSlider").slider({
        range: "min",
        min: 0,
        max: 520,
        slide: function (event, ui) {
            if (ui.value <= $("#resinLifeExpectancySlider").slider("value"))
                $("#avgResinAge").text(ui.value);
            else {
                $("#avgResinAge").text($("#resinLifeExpectancySlider").slider("value"));
                return false;
            }
        }
    });
    $("#avgResinAgeSlider").slider("value", ResinAge);
    $("#avgResinAge").text($("#avgResinAgeSlider").slider("value"));

    $("#newResinSaltSplitSlider").slider({
        range: "min",
        value: 25,
        min: 5,
        max: 30,
        slide: function (event, ui) {
            $("#newResinSaltSplit").text(ui.value);
        }
    });
    $("#newResinSaltSplit").text($("#newResinSaltSplitSlider").slider("value"));

    $("#regenEffectivenessSlider").slider({
        range: "min",
        value: 99.75,
        min: 0,
        max: 100,
        step: .25,
        slide: function (event, ui) {
            $("#regenEffectiveness").text(ui.value + "%");
        }
    });
    $("#regenEffectiveness").text($("#regenEffectivenessSlider").slider("value") + "%");

    $("#maxDegradationSlider").slider({
        range: "min",
        value: 62,
        min: 0,
        max: 100,
        step: .25,
        slide: function (event, ui) {
            $("#maxDegradation").text(ui.value + "%");
        }
    });
    $("#maxDegradation").text($("#maxDegradationSlider").slider("value") + "%");

    $("#RTIcleaningThresholdSlider").slider({
        range: "min",
        value: 17,
        min: 0,
        max: 30,
        step: .25,
        slide: function (event, ui) {
            $("#RTIcleaningThreshold").text(ui.value + "kg/cu ft");
        }
    });
    $("#RTIcleaningThreshold").text($("#RTIcleaningThresholdSlider").slider("value") + "kg/cu ft");

    $("#resinReplacementLevelSlider").slider({
        range: "min",
        value: 10,
        min: 0,
        max: 30,
        step: .25,
        slide: function (event, ui) {
            $("#resinReplacementLevel").text(ui.value + "kg/cu ft");
        }
    });
    $("#resinReplacementLevel").text($("#resinReplacementLevelSlider").slider("value") + "kg/cu ft");

    $("#sourcePredictibiltySlider").slider({
        range: "min",
        value: 95,
        min: 0,
        max: 100,
        step: .25,
        slide: function (event, ui) {
            $("#sourcePredictibilty").text(ui.value + "%");
        }
    });
    $("#sourcePredictibilty").text($("#sourcePredictibiltySlider").slider("value") + "%");

    $("#noOfIterationsSlider").slider({
        range: "min",
        value: 100,
        min: 1,
        max: 10000,
        slide: function (event, ui) {
            $("#noOfIterations").text(ui.value);
        }
    });
    $("#noOfIterations").text($("#noOfIterationsSlider").slider("value"));

    $("#standardDeviationIntervalSlider").slider({
        range: "min",
        value: 2,
        min: 1,
        max: 4,
        slide: function (event, ui) {
            $("#standardDeviationInterval").text(ui.value);
        }
    });
    $("#standardDeviationInterval").text($("#standardDeviationIntervalSlider").slider("value"));

    $("#RTICleaningEffectivenessSlider").slider({
        range: "min",
        value: 28,
        min: 0,
        max: 100,
        slide: function (event, ui) {
            $("#RTICleaningEffectiveness").text(ui.value + "%");
        }
    });
    $("#RTICleaningEffectiveness").text($("#RTICleaningEffectivenessSlider").slider("value") + "%");

    $("#trainList").change(function () {
        var trainId = $('#trainList').val();

        $.ajax({
            type: "GET",
            url: '/PredictiveSystem/GetPerformanceSettings',
            data: { SelectedTrain: trainId },
            success: function (data) {
                $("#performSettings").empty();
                $("#performSettings").html(data);
            },
            error: function (xhr) {
                window.location.href = "/ClientDatabase/Errorview";
            }
        });

    }
    );
});

