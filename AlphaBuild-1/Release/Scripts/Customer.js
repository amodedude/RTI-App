


jQuery(document).ready(function () {
    $(document).on("click", "#UpdateCustomer", function (event) {

        var CustomerName = $("#CustomerName")[0].value;
        var state = $("#State")[0].value;
        var City = $("#SelectedCity")[0].value;
        var stateIndex = $("#State")[0].selectedIndex;
        var CityIndex = $("#SelectedCity")[0].selectedIndex;

        if (!($('#CustomerName').prop('readonly')) && (CustomerName != "" || stateIndex > 0 || CityIndex > 0)) {
            $.ajax({
                type: "GET",
                url: '/Customer/CheckForDuplicate',
                data: { name: CustomerName, state: state, city: City },
                success: function (data) {
                    if (data == "True") {
                        $("#errortext").text("Customer Already exists");
                        return false;
                    }
                    else {
                        $("#createForm").submit();
                        return false;
                    }
                },
                error: function (xhr) {
                    window.location.href = "/ClientDatabase/Errorview";
                }
            });
        }
        else {
            $("#createForm").submit();

            return false;
        }
    })



    $(document).on("click", "#No", function (e) {
        //$('#No').click(function () {
        $(".modal_confirmation").modal('hide');
        $('#secondWSchk').prop("checked", true);
    })
    $(document).on("click", "#Yes", function (e) {
        //$('#Yes').click(function () {
        $(".modal_confirmation").modal('hide');
        $('.secondWatersource_view').hide();
    })

    $(document).on("click", "#GetCustomerDetails", function (e) {
        $.ajax({
            type: "GET",
            url: '/Customer/Create',
            success: function (data) {
                $(".EditableDiv").empty();
                $(".EditableDiv").html(data);

            },
            error: function (xhr) {
                window.location.href = "/ClientDatabase/Errorview";
            }

        });
        $('#changeCustomer').show();
    })

    $(document).on("click", '.btn_cancel_sysConfig', function (e) {
        $('#slideout').toggleClass('on');
    });

    $(document).on("click", "#GetSystemConfig", function (e) {
        $.ajax({
            type: "GET",
            url: '/ClientDatabase/GetSystemSettings',
            success: function (data) {
                $("#SettingsType").html(data);
                $('#slideout').toggleClass('on');
            },
            error: function (xhr) {
                window.location.href = "/ClientDatabase/Errorview";
            }
        });
    })



    //$(document).on("click", "#GetResinProductModalLookup", function (e) {
    //    $.ajax({
    //        type: "GET",
    //        url: '/ClientDatabase/GetResinLookupModal',
    //        success: function (data) {
    //            $("#resinLookup").html(data);
    //            $("#resinLookupModal").modal('show');
    //            //$('#slideout').toggleClass('on');
    //        },
    //        error: function (xhr) {
    //            window.location.href = "/ClientDatabase/Errorview";
    //        }
    //    });
    //});

    $(document).on("click", "#GetResinProductModalLookupCatBed1", function (e) {

        var bedinfo = "CatBed1";
        bedInfoValue = bedinfo;
        $.ajax({
            type: "GET",
            url: '/ClientDatabase/GetResinLookupModal',
            data: { bedInfo: bedinfo, productName: "", searchText: "" },
            success: function (data) {
                $("#resinLookup").html(data);
                $("#resinLookupModal").modal('show');
                //$('#slideout').toggleClass('on');
            },
            error: function (xhr) {
                window.location.href = "/ClientDatabase/Errorview";
            }
        });
    });

    $(document).on("click", "#GetResinProductModalLookupCatBed2", function (e) {

        var bedinfo = "CatBed2";
        bedInfoValue = bedinfo;
        //alert('im in cat bed 2');


        $.ajax({
            type: "GET",
            url: '/ClientDatabase/GetResinLookupModal',
            data: { bedInfo: bedinfo, productName: "", searchText: "" },
            success: function (data) {
                $("#resinLookup").html(data);
                $("#resinLookupModal").modal('show');
                //$('#slideout').toggleClass('on');
            },
            error: function (xhr) {
                window.location.href = "/ClientDatabase/Errorview";
            }
        });
    });

    $(document).on("click", "#GetResinProductModalLookupAnionBed1", function (e) {

        var bedinfo = "AnionBed1";
        bedInfoValue = bedinfo;
        //alert('im in Anion bed 1');


        $.ajax({
            type: "GET",
            url: '/ClientDatabase/GetResinLookupModal',
            data: { bedInfo: bedinfo, productName: "", searchText: "" },
            success: function (data) {
                $("#resinLookup").html(data);
                $("#resinLookupModal").modal('show');
                //$('#slideout').toggleClass('on');
            },
            error: function (xhr) {
                window.location.href = "/ClientDatabase/Errorview";
            }
        });
    });

    $(document).on("click", "#trainSettingsCancel,#trainSettingsClose", function (e) {
        window.location.href = "/ClientDatabase/Dashboard";
    });

    $(document).on("click", "#GetResinProductModalLookupAnionBed2", function (e) {

        var bedinfo = "AnionBed2";
        bedInfoValue = bedinfo;
        //alert('im in Anion bed 2');


        $.ajax({
            type: "GET",
            url: '/ClientDatabase/GetResinLookupModal',
            data: { bedInfo: bedinfo, productName: "", searchText: "" },
            success: function (data) {
                $("#resinLookup").html(data);
                $("#resinLookupModal").modal('show');
                //$('#slideout').toggleClass('on');
            },
            error: function (xhr) {
                window.location.href = "/ClientDatabase/Errorview";
            }
        });
    });



    $(document).on("change", "#Customer", function (e) {
        var id = $('#Customer')[0].value;
        if (id != "0") {
            $.ajax({
                type: "GET",
                url: '/Customer/Create',
                data: { customerId: id },
                success: function (data) {
                    $(".EditableDiv").empty();
                    $(".EditableDiv").html(data);
                },
                error: function (xhr) {
                    window.location.href = "/ClientDatabase/Errorview";
                }
            });
        }
        else {
            $.ajax({
                type: "GET",
                url: '/Customer/Create',
                data: { customerId: id },
                success: function (data) {
                    $(".EditableDiv").empty();
                    $(".EditableDiv").html(data);
                },
                error: function (xhr) {
                    window.location.href = "/ClientDatabase/Errorview";
                }
            });
        }
    });

    $(document).on("change", "#SystemOrTrain", function (e) {
        $("#loadingSystemOrTrain").show();
        $(".sysConfig_panel .panel-body").css("overflow", "hidden");
        var id = $('#SystemOrTrain')[0].value;
        if (id == "2") {
            $.ajax({
                type: "GET",
                url: '/ClientDatabase/TrainSettings',
                success: function (data) {
                    autocomplete = false;
                    $("#SettingsType").empty();
                    $("#SettingsType").html(data);
                    $('#SystemOrTrain')[0].selectedIndex = "1";
                    $("#loadingSystemOrTrain").hide();
                    $(".sysConfig_panel .panel-body").css("overflow", "auto");
                },
                error: function (xhr) {
                    window.location.href = "/ClientDatabase/Errorview";
                }
            });
        }
        else if (id == "1") {
            $.ajax({
                type: "GET",
                url: '/ClientDatabase/GetSystemSettings',
                success: function (data) {
                    $("#SettingsType").empty();
                    $("#SettingsType").html(data);
                    $('#SystemOrTrain')[0].selectedIndex = "0";
                    $("#loadingSystemOrTrain").hide();
                    $(".sysConfig_panel .panel-body").css("overflow", "auto");
                },
                error: function (xhr) {
                    window.location.href = "/ClientDatabase/Errorview";
                }
            });
        }
    });





    $(document).on("change", "#State", function (e) {
        var id = $('#State')[0].value;
        $.ajax({
            type: "GET",
            url: '/Customer/GetCityList',
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            data: { State: id },
            success: function (CityList) {
                $('#City').empty();
                $('<option value="">-- Select City --</option>').appendTo('#City')
                for (var i = 0 ; i < CityList.length ; i++) {
                    var div_data = '<option value=' + CityList[i].Value + '>' + CityList[i].Text + '</option>';
                    $(div_data).appendTo('#City');
                }
            },
            error: function (xhr) {
                window.location.href = "/ClientDatabase/Errorview";
            }
        });
    });


    $(document).on("change", "#City", function (e) {
        var c = $('#City')[0][$('#City')[0].selectedIndex].innerHTML;
        $('#SelectedCity')[0].value = '';
        $('#SelectedCity')[0].value = c;
    })

    $(document).on("click", "#GetCostSettings", function (e) {
        $.ajax({
            type: "GET",
            url: '/CostAnalyzer/GetCostSettings',
            success: function (data) {
                $("#costSettings").empty();
                $("#costSettings").html(data);
            },
            error: function (xhr) {
                window.location.href = "/ClientDatabase/Errorview";
            }
        });
    });



});
