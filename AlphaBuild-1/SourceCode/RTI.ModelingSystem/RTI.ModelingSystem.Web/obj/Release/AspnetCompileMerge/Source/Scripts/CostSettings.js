
jQuery(document).ready(function () {

    var acidPrice = $('#acidPrice').val();

    $("#AcidPriceSlider").slider({
        range: "min",
        value: 50,
        min: 0.01,
        max: 5.00,
        step: 0.01,
        slide: function (event, ui) {
            $("#AcidPrice").text("$" + ui.value);
            $('#acidPrice').val(ui.value);
        }
    });
    $("#AcidPriceSlider").slider("value", acidPrice);
    $("#AcidPrice").text("$" + $("#AcidPriceSlider").slider("value"));

    var causticPrice = $('#causticPrice').val();

    $("#CausticPriceSlider").slider({
        range: "min",
        value: 50,
        min: 0.01,
        max: 5.00,
        step: 0.01,
        slide: function (event, ui) {
            $("#CausticPrice").text("$" + ui.value);
            $('#causticPrice').val(ui.value);
        }
    });
    $("#CausticPriceSlider").slider("value", causticPrice);
    $("#CausticPrice").text("$" + $("#CausticPriceSlider").slider("value"));

    $("#AcidUsageSlider").slider({
        range: "min",
        value: 6,
        min: 1,
        max: 20,
        slide: function (event, ui) {
            $("#AcidUsage").text(ui.value);
        }
    });
    $("#AcidUsage").text($("#AcidUsageSlider").slider("value"));

    $("#CausticUsageSlider").slider({
        range: "min",
        value: 6,
        min: 1,
        max: 20,
        slide: function (event, ui) {
            $("#CausticUsage").text(ui.value);
        }
    });
    $("#CausticUsage").text($("#CausticUsageSlider").slider("value"));

    $("#AcidPercentageSlider").slider({
        range: "min",
        value: 100,
        min: 1,
        max: 100,
        slide: function (event, ui) {
            $("#AcidPercentage").text(ui.value + "%");
        }
    });
    $("#AcidPercentage").text($("#AcidPercentageSlider").slider("value") + "%");

    $("#CausticPercentageSlider").slider({
        range: "min",
        value: 100,
        min: 1,
        max: 100,
        slide: function (event, ui) {
            $("#CausticPercentage").text(ui.value + "%");
        }
    });
    $("#CausticPercentage").text($("#CausticPercentageSlider").slider("value") + "%");


    $("#CationResinSlider").slider({
        range: "min",
        value: 600,
        min: 1,
        max: 1500,
        slide: function (event, ui) {
            $("#CationResin").text(ui.value);
        }
    });
    $("#CationResin").text($("#CationResinSlider").slider("value"));

    $("#AnionResinSlider").slider({
        range: "min",
        value: 600,
        min: 1,
        max: 1500,
        slide: function (event, ui) {
            $("#AnionResin").text(ui.value);
        }
    });
    $("#AnionResin").text($("#AnionResinSlider").slider("value"));


    $("#CationCleaningPriceSlider").slider({
        range: "min",
        value: 32.00,
        min: 0.50,
        max: 90.00,
        step: 0.50,
        slide: function (event, ui) {
            $("#CationCleaningPrice").text(ui.value + " / cu ft");
        }
    });
    $("#CationCleaningPrice").text($("#CationCleaningPriceSlider").slider("value") + " / cu ft");

    $("#AnionCleaningPriceSlider").slider({
        range: "min",
        value: 52.00,
        min: 0.50,
        max: 70.00,
        step: 0.50,
        slide: function (event, ui) {
            $("#AnionCleaningPrice").text(ui.value + " / cu ft");
        }
    });
    $("#AnionCleaningPrice").text($("#AnionCleaningPriceSlider").slider("value") + " / cu ft");

    $("#CationCleaningDiscountSlider").slider({
        range: "min",
        value: 0,
        min: 0,
        max: 100,
        slide: function (event, ui) {
            $("#CationCleaningDiscount").text(ui.value + "%");
        }
    });
    $("#CationCleaningDiscount").text($("#CationCleaningDiscountSlider").slider("value") + "%");

    $("#AnionCleaningDiscountSlider").slider({
        range: "min",
        value: 0,
        min: 0,
        max: 100,
        slide: function (event, ui) {
            $("#AnionCleaningDiscount").text(ui.value + "%");
        }
    });
    $("#AnionCleaningDiscount").text($("#AnionCleaningDiscountSlider").slider("value") + "%");

    
    $("#CationReplacementPriceSlider").slider({
        range: "min",
        value: 90.00,
        min: 0.50,
        max: 300.00,
        step: 0.50,
        slide: function (event, ui) {
            $("#CationReplacementPrice").text(ui.value + " / cu ft");
        }
    });
    $("#CationReplacementPrice").text($("#CationReplacementPriceSlider").slider("value") + " / cu ft");

    $("#AnionReplacementPriceSlider").slider({
        range: "min",
        value: 70.00,
        min: 0.50,
        max: 300.00,
        step: 0.50,
        slide: function (event, ui) {
            $("#AnionReplacementPrice").text(ui.value + " / cu ft");
        }
    });
    $("#AnionReplacementPrice").text($("#AnionReplacementPriceSlider").slider("value") + " / cu ft");
});