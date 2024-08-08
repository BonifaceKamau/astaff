//Initialize purchase requisitions
function InitializePurchaseRequisition() {
    var dateToday = '01-01-21';
    $("#RequestedReceiptDate").datepicker({
        dateFormat: "dd-mm-y",
        changeMonth: true,
        changeYear: true,
        minDate: dateToday
    });

    AddOnChangeEvents();
    //End onchange events

    //Add dropdown search
    AddPurchaseRequisitionDropDownListSearch();
    //End add dropdown search
    GetProcurementPlan();
    //GetDimension1();
}

function AddPurchaseRequisitionDropDownListSearch() {
    $("#CurrencyCode").select2({
        placeholder: $("#CurrencyCodeLbl").text(),
        allowClear: true
    });
    $("#ProcurementPlanItemLns").select2({
        placeholder: $("#ProcurementPlanItemLnsLbl").text(),
        allowClear: true
    });
    $("#ProcurementPlanLns").select2({
        placeholder: $("#ProcurementPlanLnsLbl").text(),
        allowClear: true
    });
    $("#Dimension1").select2({
        placeholder: $("#Dimension1Lbl").text(),
        allowClear: true
    });
    $("#Dimension2").select2({
        placeholder: $("#Dimension2Lbl").text(),
        allowClear: true
    });
    $("#Dimension3").select2({
        placeholder: $("#Dimension3Lbl").text(),
        allowClear: true
    });
    $("#Dimension4").select2({
        placeholder: $("#Dimension4Lbl").text(),
        allowClear: true
    });
    $("#Dimension5").select2({
        placeholder: $("#Dimension5Lbl").text(),
        allowClear: true
    });
    $("#Dimension6").select2({
        placeholder: $("#Dimension6Lbl").text(),
        allowClear: true
    });
    $("#Dimension7").select2({
        placeholder: $("#Dimension7Lbl").text(),
        allowClear: true
    });

    //$("#ResponsibilityCenter").select2({
    //    placeholder: $("#ResponsibilityCenterLbl").text(),
    //    allowClear: true
    //});

    $("#RequisitionType").select2({
        placeholder: $("#RequisitionTypeLbl").text(),
        allowClear: true
    });

    $("#RequisitionCode").select2({
        placeholder: $("#RequisitionCodeLbl").text(),
        allowClear: true
    });


    //$("#LineLocationCode").select2({
    //    placeholder: $("#LineLocationCodeLbl").text(),
    //    allowClear: true
    //});

    //$("#LocationCode").select2({
    //	placeholder: $("#LocationCodeLbl").text(),
    //	allowClear: true
    //});

    //$("#ItemNo").select2({
    //	placeholder: $("#ItemNoLbl").text(),
    //	allowClear: true
    //});

    $("#UOM").select2({
    	placeholder: $("#UOMLbl").text(),
    	allowClear: true
    });
}  

//Onchange events
function AddOnChangeEvents() {
    $('#RequisitionType').change(function () {
        GetPurchaseRequisitionCodes($(this).val());
       // GetPurchaseRequisitionItems($(this).val());
       // GetPurchaseRequisitionFixedAssets($(this).val());

        GetPurchaseRequisitionUOMs($(this).val());
    });

    $('#Dimension1').change(function () {
        loaddimension2($(this).val());
    });
    $('#Dimension2').change(function () {
        loaddimension3($(this).val());
    });
    $('#Dimension3').change(function () {
        loaddimension4($(this).val());
    });
    $('#Dimension4').change(function () {
        loaddimension5($(this).val());
    });
    $('#Dimension5').change(function () {
        loaddimension6($(this).val());
    });
    $('#Dimension6').change(function () {
        loaddimension7($(this).val());
    });
    $('#ProcurementPlan').change(function () {
        loadprocplanitems($(this).val());
    });
    $('#ProcurementPlanLns').change(function () {
        loadprocplanitemslns($(this).val());
    });
    $('#SharedType').change(function () {
        var shared = $("#SharedType").val();

        if (shared == "Shared") {
            $("#shared").css("display", "block");
        } else {
            $("#shared").css("display", "none");
        }
    });
    
}


//load 2nd dimension
function loaddimension2(dimension1)
{
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetGlobalDimension2Codes,
        type: "GET",
        dataType: "json",
        data: { dimension1: dimension1 },
        cache: false,
        success: function (Dimnesions2) {
            var rows = "";
            $.each(Dimnesions2.DropDownData.ListOfddlData, function (i, Dimnesions) {
                options += "<option value='" + Dimnesions.Value + "'>";
                options += Dimnesions.Value;
                options += "</option>";
            });
            $("#Dimension2").html(options);
        },
        error: OnError
    });
}
//load 3rd dimension
function loaddimension3(dimension2) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetGlobalDimension3Codes,
        type: "GET",
        dataType: "json",
        data: { dimension2: dimension2 },
        cache: false,
        success: function (Dimnesions3) {
            var rows = "";
            $.each(Dimnesions3.DropDownData.ListOfddlData, function (i, Dimnesions) {
                options += "<option value='" + Dimnesions.Value + "'>";
                options += Dimnesions.Value;
                options += "</option>";
            });
            $("#Dimension3").html(options);
        },
        error: OnError
    });
}
//load 4th dimension
function loaddimension4(dimension3) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetGlobalDimension4Codes,
        type: "GET",
        dataType: "json",
        data: { dimension3: dimension3 },
        cache: false,
        success: function (Dimnesions4) {
            var rows = "";
            $.each(Dimnesions4.DropDownData.ListOfddlData, function (i, Dimnesions) {
                options += "<option value='" + Dimnesions.Value + "'>";
                options += Dimnesions.Value;
                options += "</option>";
            });
            $("#Dimension4").html(options);
        },
        error: OnError
    });
}
//load 5th dimension
function loaddimension5(dimension4) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetGlobalDimension5Codes,
        type: "GET",
        dataType: "json",
        data: { dimension4: dimension4 },
        cache: false,
        success: function (Dimnesions5) {
            var rows = "";
            $.each(Dimnesions5.DropDownData.ListOfddlData, function (i, Dimnesions) {
                options += "<option value='" + Dimnesions.Value + "'>";
                options += Dimnesions.Value;
                options += "</option>";
            });
            $("#Dimension5").html(options);
        },
        error: OnError
    });
}
//load 6th dimension
function loaddimension6(dimension5) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetGlobalDimension6Codes,
        type: "GET",
        dataType: "json",
        data: { dimension5: dimension5 },
        cache: false,
        success: function (Dimnesions6) {
            var rows = "";
            $.each(Dimnesions6.DropDownData.ListOfddlData, function (i, Dimnesions) {
                options += "<option value='" + Dimnesions.Value + "'>";
                options += Dimnesions.Value;
                options += "</option>";
            });
            $("#Dimension6").html(options);
        },
        error: OnError
    });
}
//load 7th dimension
function loaddimension7(dimension6) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetGlobalDimension7Codes,
        type: "GET",
        dataType: "json",
        data: { dimension6: dimension6 },
        cache: false,
        success: function (Dimnesions7) {
            var rows = "";
            $.each(Dimnesions7.DropDownData.ListOfddlData, function (i, Dimnesions) {
                options += "<option value='" + Dimnesions.Value + "'>";
                options += Dimnesions.Value;
                options += "</option>";
            });
            $("#Dimension7").html(options);
        },
        error: OnError
    });
}
function loadprocplanitems(procurementplan) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";
    var PurchaseRequisitionLineObj = {
        LineGlobalDimension1Code: $("#Dimension1").val(),
        LineGlobalDimension2Code: $("#Dimension2").val(),
        LineShortcutDimension3Code: $("#Dimension3").val(),
        LineShortcutDimension4Code: $("#Dimension4").val(),
        LineShortcutDimension5Code: $("#Dimension5").val(),
        LineShortcutDimension6Code: $("#Dimension6").val(),
        LineShortcutDimension7Code: $("#Dimension7").val(),
        Quantity: $("#Quantity").val(),
        UnitCost: $("#UnitCost").val(),
        ProcurementPlan: $("#ProcurementPlan").val(),
        ProcurementPlanItem: $("#ProcurementPlanItem").val(),
    };


    $.ajax({
        url: AJAXUrls.GetProcPlanItems,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(PurchaseRequisitionLineObj),
        cache: false,
        success: function (result) {
            var rows = "";
            ProcurementPlanItems = result;
            $.each(ProcurementPlanItems.DropDownData.ListOfddlData, function (i, ProcurementPlanItem) {
                options += "<option value='" + ProcurementPlanItem.Text + "'>";
                options += ProcurementPlanItem.Value;
                options += "</option>";
            });
            $("#ProcurementPlanItem").html(options);
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}
function loadprocplanitemslns(procurementplan) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";
    var PurchaseRequisitionLineObj = {
        LineGlobalDimension1Code: $("#Dimension1").val(),
        LineGlobalDimension2Code: $("#Dimension2").val(),
        LineShortcutDimension3Code: $("#Dimension3").val(),
        LineShortcutDimension4Code: $("#Dimension4").val(),
        LineShortcutDimension5Code: $("#Dimension5").val(),
        LineShortcutDimension6Code: $("#Dimension6").val(),
        LineShortcutDimension7Code: $("#Dimension7").val(),
        Quantity: $("#Quantity").val(),
        UnitCost: $("#UnitCost").val(),
        ProcurementPlan: $("#ProcurementPlanLns").val(),
        ProcurementPlanItem: $("#ProcurementPlanItemLns").val(),
    };
    

    $.ajax({
        url: AJAXUrls.GetProcPlanItems,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(PurchaseRequisitionLineObj),
        cache: false,
        success: function (result) {
            var rows = "";
            ProcurementPlanItems = result;
            $.each(ProcurementPlanItems.DropDownData.ListOfddlData, function (i, ProcurementPlanItem) {
                options += "<option value='" + ProcurementPlanItem.Text + "'>";
                options += ProcurementPlanItem.Value;
                options += "</option>";
            });
            $("#ProcurementPlanItemLns").html(options);
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}
    //Load item UOMs lines
    function GetItemUOMs(ItemNo) {
        var options = "";
        options += "<option>";
        options += "";
        options += "</option>";

        $.ajax({
            url: AJAXUrls.GetItemUOMs,
            type: "GET",
            dataType: "json",
            data: { ItemNo: ItemNo },
            cache: false,
            success: function (itemUOMs) {
                var rows = "";
                $.each(itemUOMs, function (i, itemUOM) {
                    options += "<option value='" + itemUOM.Code + "'>";
                    options += itemUOM.Code;
                    options += "</option>";
                });
                $("#UOM").html(options);
            },
            error: OnError
        });
    }

    //Load purchase requisition lines
    function LoadPurchaseRequisitionLines(DocumentNo) {
        $.ajax({
            url: AJAXUrls.GetPurchaseRequisitionLines,
            type: "GET",
            dataType: "json",
            data: { DocumentNo: DocumentNo },
            cache: false,
            success: function (purchaseRequisitionLines) {
                var rows = "";
                $("#AddReqLine").css("display", "block");
                $.each(purchaseRequisitionLines, function (i, purchaseRequisitionLine) {
                    rows += "<tr>";
                    rows += "<td>" + purchaseRequisitionLine.ProcurementPlan + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.ProcurementPlanItem + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.RequisitionType + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.No + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.LineDescription + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.UOM + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.Quantity.toLocaleString() + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.UnitCost.toLocaleString() + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.TotalLineCost.toLocaleString() + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.Dimension1 + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.Dimension2 + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.Dimension3 + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.Dimension4 + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.Dimension5 + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.Dimension6 + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.Dimension7 + "</td>";
                    if (purchaseRequisitionLine.Status == "Open" || purchaseRequisitionLine.Status == "Pending") {
                        rows += '<td><a href="#" onclick="return EditPurchaseRequisitionLine(' + purchaseRequisitionLine.LineNo + ',\'' + purchaseRequisitionLine.DocumentNo + '\');">Edit</a> | <a href="#" onclick="DeletePurchaseRequisitionLine(' + purchaseRequisitionLine.LineNo + ',\'' + purchaseRequisitionLine.DocumentNo + '\')">Delete</a></td>';
                    } else {
                        $("#AddReqLine").css("display", "none");
                    }

                    rows += "</tr>";
                    $("#PurchaseRequisitionLineTbl tbody").html(rows);
                });
                $("#AjaxLoader").css("display", "none");
                $("#PurchaseRequisitionLineTbl").css("display", "block");
            },
            error: OnError
        });
    }

    function GetPurchaseRequisitionCodes(RequisitionType) {
        if (RequisitionType == "Item") {
            GetProcurementPlanItems();
        }
        else if (RequisitionType == "Fixed Asset")
        {
            GetProcurementPlanFixedAssets();
        }
        else
        {
            GetProcurementPlanServices();
        }
    }

    //Items
    function GetProcurementPlanItems() {
        var options = "";
        options += "<option>";
        options += "";
        options += "</option>";

        $.ajax({
            url: AJAXUrls.GetProcurementPlanItems,
            type: "GET",
            dataType: "json",
            // data: { RequisitionType: RequisitionType },
            cache: false,
            success: function (itemCodes) {
                var rows = "";
                $.each(itemCodes, function (i, itemCode) {
                    options += "<option value='" + itemCode.Code + "'>";
                    options += itemCode.Description;
                    options += "</option>";
                });
                $("#RequisitionCode").html(options);
            },
            error: OnError
        });
    }

    function GetProcurementPlan() {
        var options = "";
        options += "<option>";
        options += "";
        options += "</option>";

        $.ajax({
            url: AJAXUrls.GetProcurementPlan,
            type: "GET",
            dataType: "json",
            // data: { RequisitionType: RequisitionType },
            cache: false,
            success: function (itemCodes) {
                var rows = "";
                $.each(itemCodes, function (i, itemCode) {
                    options += "<option value='" + itemCode.Code + "'>";
                    options += itemCode.Description;
                    options += "</option>";
                });
                $("#ProcurementPlan").html(options);
                $("#ProcurementPlanLns").html(options);
            },
            error: OnError
        });
    }
    function GetDimension1() {
        var options = "";
        options += "<option>";
        options += "";
        options += "</option>";

        $.ajax({
            url: AJAXUrls.GetGlobalDimension1Codes,
            type: "GET",
            dataType: "json",
            // data: { RequisitionType: RequisitionType },
            cache: false,
            success: function (itemCodes) {
                var rows = "";
                $.each(itemCodes, function (i, itemCode) {
                    options += "<option value='" + itemCode.Code + "'>";
                    options += itemCode.Name;
                    options += "</option>";
                });
                $("#Dimension1").html(options);
            },
            error: OnError
        });
    }

    //Fixed Assets
    function GetProcurementPlanFixedAssets() {
        var options = "";
        options += "<option>";
        options += "";
        options += "</option>";

        $.ajax({
            url: AJAXUrls.GetProcurementPlanFixedAssets,
            type: "GET",
            dataType: "json",
            // data: { RequisitionType: RequisitionType },
            cache: false,
            success: function (fixedAssetCodes) {
                var rows = "";
                $.each(fixedAssetCodes, function (i, fixedAssetCode) {
                    options += "<option value='" + fixedAssetCode.Code + "'>";
                    options += fixedAssetCode.Description;
                    options += "</option>";
                });
                $("#RequisitionCode").html(options);
            },
            error: OnError
        });
    }

    //Services
    function GetProcurementPlanServices() {
        var options = "";
        options += "<option>";
        options += "";
        options += "</option>";

        $.ajax({
            url: AJAXUrls.GetProcurementPlanServices,
            type: "GET",
            dataType: "json",
            // data: { RequisitionType: RequisitionType },
            cache: false,
            success: function (serviceCodes) {
                var rows = "";
                $.each(serviceCodes, function (i, serviceCode) {
                    options += "<option value='" + serviceCode.Code + "'>";
                    options += serviceCode.Description;
                    options += "</option>";
                });
                $("#RequisitionCode").html(options);
            },
            error: OnError
        });
    }

    //Purchase Requisition UOMs
    function GetPurchaseRequisitionUOMs(RequisitionType) {
        var options = "";
        options += "<option>";
        options += "";
        options += "</option>";

        $.ajax({
            url: AJAXUrls.GetPurchaseRequisitionUOMs,
            type: "GET",
            dataType: "json",
            data: { RequisitionType: RequisitionType },
            cache: false,
            success: function (purchaseRequisitionUOMs) {
                var rows = "";
                $.each(purchaseRequisitionUOMs, function (i, purchaseRequisitionUOM) {
                    options += "<option value='" + purchaseRequisitionUOM.Code + "'>";
                    options += purchaseRequisitionUOM.Description;
                    options += "</option>";
                });
                $("#UOM").html(options);
            },
            error: OnError
        });
    }

    //Items
    function GetPurchaseRequisitionItems(RequisitionType) {
        var options = "";
        options += "<option>";
        options += "";
        options += "</option>";

        $.ajax({
            url: AJAXUrls.GetPurchaseRequisitionItems,
            type: "GET",
            dataType: "json",
            data: { RequisitionType: RequisitionType },
            cache: false,
            success: function (items) {
                var rows = "";
                $.each(items, function (i, item) {
                    options += "<option value='" + item.No + "'>";
                    options += item.Description;
                    options += "</option>";
                });
                $("#RequisitionCode").html(options);
            },
            error: OnError
        });
    }

    //Fixed assets
    function GetPurchaseRequisitionFixedAssets(RequisitionType) {
        var options = "";
        options += "<option>";
        options += "";
        options += "</option>";

        $.ajax({
            url: AJAXUrls.GetPurchaseRequisitionFixedAssets,
            type: "GET",
            dataType: "json",
            data: { RequisitionType: RequisitionType },
            cache: false,
            success: function (fixedAssets) {
                var rows = "";
                $.each(fixedAssets, function (i, fixedAsset) {
                    options += "<option value='" + fixedAsset.No + "'>";
                    options += fixedAsset.Description;
                    options += "</option>";
                });
                $("#RequisitionCode").html(options);
            },
            error: OnError
        });
    }

    function LoadPurchaseRequisitionLinesView(DocumentNo) {
        $.ajax({
            url: AJAXUrls.GetPurchaseRequisitionLines,
            type: "GET",
            dataType: "json",
            data: { DocumentNo: DocumentNo },
            cache: false,
            success: function (purchaseRequisitionLines) {
                var rows = "";
                $.each(purchaseRequisitionLines, function (i, purchaseRequisitionLine) {
                    rows += "<tr>";
                    rows += "<td>" + purchaseRequisitionLine.RequisitionType + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.LineGlobalDimension1Code + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.LineGlobalDimension2Code + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.LineShortcutDimension3Code + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.RequisitionCode + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.LineDescription + "</td>";
                    //rows += "<td>" + purchaseRequisitionLine.LineLocationCode + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.Quantity.toLocaleString() + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.UOM + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.UnitCost.toLocaleString() + "</td>";
                    rows += "<td>" + purchaseRequisitionLine.TotalLineCost.toLocaleString() + "</td>";
                    rows += "<td><a href='javascript:void(0);' onclick='return ViewPurchaseRequisitionLine(" + purchaseRequisitionLine.LineNo + ",\'" + purchaseRequisitionLine.DocumentNo + "\')';>View</a></td>";
                    rows += "</tr>";
                    $("#PurchaseRequisitionLineTbl tbody").html(rows);
                });
                $("#ViewPurchaseRequisitionLineAjaxLoader").css("display", "none");
                $("#PurchaseRequisitionLineTbl").css("display", "block");
            },
            error: OnError
        });
    }

    //Create purchase requisition line   
    function CreatePurchaseRequisitionLine() {
        var documentNo = $("#No").val();

        var validLine = ValidatePurchaseRequisitionLine();
        if (validLine == false) {
            return false;
        }

        var PurchaseRequisitionLineObj = {
            DocumentNo: documentNo,
            LineGlobalDimension1Code: $("#Dimension1").val(),
            LineGlobalDimension2Code: $("#Dimension2").val(),
            LineShortcutDimension3Code: $("#Dimension3").val(),
            LineShortcutDimension4Code: $("#Dimension4").val(),
            LineShortcutDimension5Code: $("#Dimension5").val(),
            LineShortcutDimension6Code: $("#Dimension6").val(),
            LineShortcutDimension7Code: $("#Dimension7").val(),
            Quantity: $("#Quantity").val(),
            UnitCost: $("#UnitCost").val(),
            ProcurementPlan: $("#ProcurementPlanLns").val(),
            ProcurementPlanItem: $("#ProcurementPlanItemLns").val(),
            LineDescription: $("#LineDescription").val(),
        };

        $.ajax({
            url: AJAXUrls.CreatePurchaseRequisitionLine,
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(PurchaseRequisitionLineObj),
            cache: false,
            success: function (result) {
                if (result.success == true) {
                    LoadPurchaseRequisitionLines(documentNo);
                    $("#PurchaseRequisitionLineModal").modal("hide");
                } else {
                    Swal.fire('Warning', result.message, 'warning');
                }
            },
            error: function (xhr, errorType, exception) {
                Swal.fire('Warning', result, 'warning');
            }
        });
    }

    //Edit purchase requisition line
    function EditPurchaseRequisitionLine(LineNo, DocumentNo) {
        $.ajax({
            url: AJAXUrls.GetPurchaseRequisitionLineByLineNo,
            type: "GET",
            dataType: "json",
            data: { LineNo: LineNo, DocumentNo: DocumentNo },
            cache: false,
            success: function (result) {
                console.log(result);
                $("#LineNo").val(result.LineNo);
                $("#DocumentNo").val(result.DocumentNo);
                $("#RequisitionType").val(result.RequisitionType).trigger("change.select2");
                $("#RequisitionCode").val(result.RequisitionCode).trigger("change.select2");
                $("#LineDescription").val(result.LineDescription);
                $("#LineLocationCode").val(result.LineLocationCode).trigger("change.select2");
                $("#Quantity").val(result.Quantity);
                $("#UOM").val(result.UOM);
                $("#UnitCost").val(result.UnitCost);
                $("#TotalLineCost").val(result.TotalLineCost);          
                $("#Dimension1").val(result.Dimension1);
                $("#Dimension2").val(result.Dimension2).trigger("change.select2");
                $("#Dimension3").val(result.Dimension3).trigger("change.select2");
                $("#Dimension4").val(result.Dimension4).trigger("change.select2");
                $("#Dimension5").val(result.Dimension5).trigger("change.select2");
                $("#Dimension6").val(result.Dimension6).trigger("change.select2");
                $("#Dimension7").val(result.Dimension7).trigger("change.select2");
                //$("#LineShortcutDimension8Code").val(result.LineShortcutDimension8Code).trigger("change.select2");

                $("#PurchaseRequisitionLineModal").modal("show");
                $("#CreatePurchaseRequisitionLineBtn").hide();
                $("#ModifyPurchaseRequisitionLineBtn").show();
                $("#shared1").css("display", "none");
                $("#shared2").css("display", "none");
                $("#shared3").css("display", "none");
                
            },
            error: OnError
        });
        return false;
    }

    //Modify purchase requisition line
    function ModifyPurchaseRequisitionLine() {
        var documentNo = $("#No").val();

        //var validLine = ValidatePurchaseRequisitionLine();
        //if (validLine == false) {
        //    return false;
        //}
        var PurchaseRequisitionLineObj = {
            LineNo: $("#LineNo").val(),
            DocumentNo: documentNo,
            
            Quantity: $("#Quantity").val(),
            UnitCost: $("#UnitCost").val(),
            LineDescription: $("#LineDescription").val(),
        };
        $.ajax({
            url: AJAXUrls.ModifyPurchaseRequisitionLine,
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(PurchaseRequisitionLineObj),
            cache: false,
            success: function (result) {
                LoadPurchaseRequisitionLines(documentNo);
                $("#PurchaseRequisitionLineModal").modal("hide");

                ClearPurchaseRequisitionLineModal();
            },
            error: OnError
        });
    }

    //View purchase requisition line
    function ViewPurchaseRequisitionLine(LineNo, DocumentNo) {
        $.ajax({
            url: AJAXUrls.GetPurchaseRequisitionLineByLineNo,
            type: "GET",
            dataType: "json",
            data: { LineNo: LineNo, DocumentNo: DocumentNo },
            cache: false,
            success: function (result) {
                $("#LineNo").val(result.LineNo);
                $("#DocumentNo").val(result.DocumentNo);
                $("#RequisitionType").val(result.RequisitionType);
                $("#RequisitionCode").val(result.RequisitionCode);
                $("#LineDescription").val(result.LineDescription);
                $("#LineLocationCode").val(result.LineLocationCode);
                $("#Quantity").val(result.Quantity);
                $("#UOM").val(result.UOM);
                $("#UnitCost").val(result.UnitCost);
                $("#TotalLineCost").val(result.TotalLineCost);
                $("#LineGlobalDimension1Code").val(result.LineGlobalDimension1Code);
                $("#LineGlobalDimension2Code").val(result.LineGlobalDimension2Code);
                $("#LineShortcutDimension3Code").val(result.LineShortcutDimension3Code);
                $("#LineShortcutDimension4Code").val(result.LineShortcutDimension4Code);
                $("#LineShortcutDimension5Code").val(result.LineShortcutDimension5Code);
                $("#LineShortcutDimension6Code").val(result.LineShortcutDimension6Code);
                $("#LineShortcutDimension7Code").val(result.LineShortcutDimension7Code);
                $("#LineShortcutDimension8Code").val(result.LineShortcutDimension8Code);

                $('#PurchaseRequisitionLineModal').modal('show');
            },
            error: OnError
        });
        return false;
    }

    //Delete purchase requisition line
    function DeletePurchaseRequisitionLine(LineNo, DocumentNo) {
        var ans = confirm("Are you sure you want to delete this Line?");
        if (ans) {
            $.ajax({
                url: AJAXUrls.DeletePurchaseRequisitionLine,
                type: "POST",
                dataType: "json",
                data: { LineNo: LineNo, DocumentNo: DocumentNo },
                cache: false,
                success: function (result) {
                    LoadPurchaseRequisitionLines(DocumentNo);
                    //window.location.reload();
                    Swal.fire('Success', 'Line Deleted successfully', 'success');
                    
                },
                error: function (errormessage) {
                    alert("Error");
                }
            });
        }
    }

    //Get purchase requisition amount
    function GetPurchaseRequisitionAmount(DocumentNo) {
        var purchaseRequisitionAmount = 0;
        $.ajax({
            url: AJAXUrls.GetPurchaseRequisitionAmount,
            type: "GET",
            dataType: "json",
            data: { DocumentNo: DocumentNo },
            cache: false,
            success: function (result) {
                purchaseRequisitionAmount = result.Amount.toLocaleString();
                $("#Amount").val(purchaseRequisitionAmount);
            },
            error: OnError
        });
    }

    function ValidatePurchaseRequisition() {
        var isValid = true;
        if ($('#No').val().trim() == "") {
            $('#No').css('border-color', 'Red');
            isValid = false;
        }
        else {
            $('#No').css('border-color', 'lightgrey');
        }

        if ($('#GlobalDimension1Code').val().trim() == "") {
            $("#GlobalDimension1CodeError").show();
            isValid = false;
        }
        else {
            $('#errorGlobalDimension1Code').hide();
        }
        //Clear purchase requisition line modal
        ClearPurchaseRequisitionLineModal();

        return isValid;
    }

    function ValidatePurchaseRequisitionLine() {
        var isValid = true;
        var label = "";
        //if ($("#Dimension1").val().trim() == "") {
        //    $("#Dimension1Error").text("Dimension 1 cannot be empty.");
        //    isValid = false;
        //}
        //else {
        //    $("#Dimension1Error").text("");
        //}

        //if ($("#Dimension2").val().trim() == "") {
        //    $("#Dimension2Error").text("Dimension 2 cannot be empty.");
        //    isValid = false;
        //}
        //else {
        //    $("#Dimension2Error").text("");
        //}

       

        if ($.isNumeric($("#Quantity").val())) {
            $("#QuantityError").text("");
        } else {
            ("#QuantityError").text("Quantity must be numeric.");
            isValid = false;
        }

        if (($("#Quantity").val() <= 0) || ($("#Quantity").val().trim() == "")) {
            $("#QuantityError").text("Quantity cannot be less or equal to zero.");
            isValid = false;
        }
        else {
            $("#QuantityError").text("");
        }

        if ($.isNumeric($("#UnitCost").val())) {
            $("#UnitCostError").text("");
        } else {
            $("#UnitCostError").text("Unit cost must be numeric.");
            isValid = false;
        }

        if (($("#UnitCost").val() <= 0) || ($("#UnitCost").val().trim() == "")) {
            $("#UnitCostError").text("Unit cost cannot be less or equal to zero.");
            isValid = false;
        }
        else {
            $("#UnitCostError").text("");
        }

        return isValid;
    }

    //Clear purchase requisition line modal
    function ClearPurchaseRequisitionLineModal() {
        $("#LineNo").val(0);
        $("#DocumentNo").val("");
        $("#RequisitionType").val("").trigger("change.select2");
        $("#RequisitionCode").val("").trigger("change");
        $("#LineDescription").val("").trigger("change");
        $("#LineLocationCode").val("").trigger("change");
        $("#Quantity").val(0);
        $("#UOM").val("");
        $("#UnitCost").val(0);
        $("#TotalLineCost").val(0);
        $("#LineGlobalDimension1Code").val("").trigger("change");
        $("#LineGlobalDimension2Code").val("").trigger("change");
        $("#LineShortcutDimension3Code").val("").trigger("change");
        $("#LineShortcutDimension4Code").val("").trigger("change");
        $("#LineShortcutDimension5Code").val("").trigger("change");
        $("#LineShortcutDimension6Code").val("").trigger("change");
        $("#LineShortcutDimension7Code").val("").trigger("change");
        $("#LineShortcutDimension8Code").val("").trigger("change");


        $("#CreatePurchaseRequisitionLineBtn").show();
        $("#ModifyPurchaseRequisitionLineBtn").hide();

        $("#shared1").css("display", "block");
        $("#shared2").css("display", "block");
        $("#shared3").css("display", "block");

        $("#RequisitionTypeError").text("");
        $("#RequisitionCodeError").text("");
        $("#LineDescriptionError").text("");
        $("#LineLocationCodeError").text("");    
        $("#QuantityError").text("");
        $("#UOMError").text("");
        $("#UnitCostError").text("");
        $("#TotalLineCostError").text("");
        $("#LineGlobalDimension1CodeError").text("");
        $("#LineGlobalDimension2CodeError").text("");
        $("#LineShortcutDimension3CodeError").text("");
        $("#LineShortcutDimension4CodeError").text("");
        $("#LineShortcutDimension5CodeError").text("");
        $("#LineShortcutDimension6CodeError").text("");
        $("#LineShortcutDimension7CodeError").text("");
        $("#LineShortcutDimension8CodeError").text("");
    }

    //Load Purchase Requisition Document
    function LoadPurchaseRequisitionDocuments(DocumentNo) {
        var DocNo = DocumentNo;
        var Status = $("#Status").val();
        $.ajax({
            async: true,
            type: "POST",
            datatype: "json",
            contentType: "application/json; charset = utf-8",
            processData: false,
            data: JSON.stringify({ DocNo: DocNo, TableID: 51525398, Status: Status }),
            url: "/PurchaseRequisition/DocumentAttachments",
            success: function (data) {
                $("#divAttachDocs").html(data);
            },
            error: function () {
                Swal.fire("There is some problem to process your request. Please try after some time");
            }
        });
    }
    var DeleteAttachment = function(tbl, No, Id)
    {
        $.ajaxSetup({ cache: false });
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.value) {
                $.ajax({
                    cache: false,
                    url: '/PurchaseRequisition/DeleteAttachedDocument',
                    datatype: "json",
                    type: "POST",
                    data: JSON.stringify({ DocNo: No, DocID: Id, tblID: tbl }),
                    contentType: "application/json; charset = utf-8",
                    success: function (data) {
                        if (data.success == true) {
                            LoadPurchaseRequisitionDocuments(No);
                            Swal.fire('Success', 'File Deleted Successfully', 'success');
                        }
                        else {
                            Swal.fire('Warning', data.message, 'warning');
                        }
                    },
                    error: function (err) {
                        Swal.fire('Warning', err, 'warning');
                    }
                });
            }
            else {
                Swal.fire('Cancelled', 'Attachment File has not been deleted', 'error');
            }
        });
    }

    var ViewAttachment = function (tbl, No, Id, fName, ext) {
        $.ajaxSetup({ cache: false });
        $.ajax({
            cache: false,
            url: '/PurchaseRequisition/DocumentAttachmentview',
            type: "POST",
            datatype: "json",
            cache: false,
            contentType: "application/json; charset = utf-8",
            processData: false,
            data: JSON.stringify({ tblID: tbl, No: No, ID: Id, fileName: fName, ext: ext }),
            success: function (data) {
                if (data.success == true) {
                    if (data.view == true) {
                        var viewer = $("#modalAttachmentBody");
                        if (ext == "pdf") {
                            PDFObject.embed("data:application/" + ext + ";base64," + data.message + "", viewer);
                        }
                        if (ext == "png" || ext == "jpg") {
                            PDFObject.embed("data:image/" + ext + ";base64," + data.message + "", viewer);
                        }
                        $("#myModalAttachment").modal("show");
                    }
                    else {
                        window.location = '/PurchaseRequisition/AttachmentDownload?fileName=' + data.message;
                        Swal.fire('Success', 'Document Downloaded successfully', 'success');
                    }
                }
                else {
                    Swal.fire('Warning', data.message, 'warning');
                }
            },
            error: function (err) {
                Swal.fire('Warning', err, 'warning');
            }
        });
    };
    function AddAttachment() {
        $("#ApplicationDocumentModal").modal("show");
    }
    //Load Purchase Requisition Document View
    function LoadPurchaseRequisitionDocumentsView(DocumentNo) {
        $.ajax({
            url: AJAXUrls.LoadPurchaseRequisitionDocuments,
            type: "GET",
            dataType: "json",
            data: { DocumentNo: DocumentNo },
            cache: false,
            success: function (results) {
                var rows = "";
                $.each(results, function (i, result) {
                    rows += "<tr>";
                    rows += "<td>" + result.DocumentDescription + "</td>";
                    if (result.DocumentAttached) {
                        rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' checked disabled>" + "</td>";
                    } else {
                        rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' disabled>" + "</td>";
                    }
                    rows += "</tr>";
                });
                $("#ApplicationDocumentsTbl tbody").html(rows);

                $("#AjaxLoader").css("display", "none");

                $("#ApplicationDocumentsTbl").css("display", "block");
            },
            error: function (xhr, status, error) {

            }
        });
    }

    //Edit Purchase Requisition Document
    function EditPurchaseRequisitionDocument(DocumentNo, DocumentCode) {

        //Clear link path
        ResetPurchaseRequisitionDocumentModal();

        $.ajax({
            url: AJAXUrls.GetPurchaseRequisitionDocumentByLineNo,
            type: "GET",
            dataType: "json",
            data: { DocumentNo: DocumentNo, DocumentCode: DocumentCode },
            cache: false,
            success: function (result) {
                $("#DocumentNo").val(result.DocumentNo);
                $("#DocumentCode").val(result.DocumentCode);
                $("#DocumentDescription").val(result.DocumentDescription);
                $('#errorMessage').hide();
                $("#ApplicationDocumentModal").modal("show");
                $("#UploadPurchaseRequisitionDocumentBtn").show();
            },
            error: function (xhr, status, error) {

            }
        });
        return false;
    }

    //Upload Purchase Requisition Document
    function UploadPurchaseRequsitionDocuments() {

        //Assign values to this variables
        var DocumentNo = $("#No").val();
        var DocumentCode = $("#DocumentCode").val();
        var DocumentDescription = $("#DocumentDescription").val();


        var filebase = $("#ApplicationDocumentFile").get(0);
        var files = filebase.files;

        var form = $('ApplicationDocumentForm')[0];
        var frmData = new FormData();

        frmData.append("DocumentNo", DocumentNo);
        frmData.append("DocumentCode", DocumentCode);
        frmData.append("DocumentDescription", DocumentDescription);

        frmData.append(files[0].name, files[0]);

        //Block UI
        $.blockUI();

        $.ajax({
            url: AJAXUrls.UploadPurchaseRequsitionAttachments,
            type: "POST",
            data: frmData,
            dataType: 'json',
            contentType: false,
            processData: false,
            enctype: "multipart/form-data",
            async: true,
            cache: false,
            success: function (result) {
                //$('#txtMessage').html(result.message);
                if (result.success) {
                    $('#ApplicationDocumentModal').modal('hide');
                    $('#errorMessage').hide();
                    LoadPurchaseRequisitionDocuments(DocumentNo);
                    $.unblockUI();
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: result.message
                    })
                } else {
                    $('#errorMessage').html(result.message);
                    $('#errorMessage').show();
                    $.unblockUI();
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: result.message
                    })
                }
                Ladda.stopAll();
            },
            error: function (err) {
                $('#ApplicationDocumentModal').modal('show');
                $('#errorMessage').html(err.statusText);
                $('#errorMessage').show();
                Ladda.stopAll();
            }
        });
    }

    //Reset Purchase Requisition Document Path
    function ResetPurchaseRequisitionDocumentModal() {
        $("#ApplicationDocumentFile").val("");
        //Ladda.stopAll();
    }

    //error
    function OnError(xhr, errorType, exception) {
        var responseText;
        $("#dialog").html("");
        try {
            responseText = jQuery.parseJSON(xhr.responseText);
            $("#dialog").append("<div><b>" + errorType + " " + exception + "</b></div>");
            $("#dialog").append("<div><u>Exception</u>:<br /><br />" + responseText.ExceptionType + "</div>");
            $("#dialog").append("<div><u>StackTrace</u>:<br /><br />" + responseText.StackTrace + "</div>");
            $("#dialog").append("<div><u>Message</u>:<br /><br />" + responseText.Message + "</div>");
            alert(responseText.Message);
        } catch (e) {
            responseText = xhr.responseText;
            $("#dialog").html(responseText);
        }
        $("#dialog").dialog({
            title: "jQuery Exception Details",
            width: 700,
            buttons: {
                Close: function () {
                    $(this).dialog('close');
                }
            }
        });
    }
