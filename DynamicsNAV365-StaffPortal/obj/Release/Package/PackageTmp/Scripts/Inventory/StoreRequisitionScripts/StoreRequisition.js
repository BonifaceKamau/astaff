//Initialize store requisitions
function InitializeStoreRequisition() {
    var dateToday = '01-01-21';
    $("#RequiredDate").datepicker({
        dateFormat: "dd-mm-y",
        changeMonth: true,
        changeYear: true,
        minDate: dateToday
    });

    var itemNo = "";
    var uom = "";
    var locationCode = "";

    //Onchange events
    $("#ItemNo").change(function () {
        $("#Inventory").val(0);
        $("#Quantity").val(0);
        $("#QuantityError").text("");
        GetAvailableInventory($(this).val());
    });

    //$("#UOM").change(function () {
    //	$("#Inventory").val(0);
    //	$("#Inventory").css("background-color", "White");
    //	$("#Quantity").val(0);
    //	$("#QuantityError").text("");

    //    itemNo = $("#ItemNo").val().trim();
    //    uom = $(this).val().trim();
    //    if (itemNo != "") {
    //        if ($("#LineLocationCode").val().trim() != "") {
    //            locationCode = $("#LineLocationCode").val().trim();
    //     //       GetAvailableInventory(itemNo, uom, locationCode);
    //        }
    //    } else {
    //        $("#ItemNoError").text("Item No. cannot be empty.");
    //        $("#UOM").val("");
    //    }

    //});

    //$("#LineLocationCode").change(function () {
    //	$("#Inventory").val(0);
    //	$("#Inventory").css("background-color", "White");
    //	$("#Quantity").val(0);
    //	$("#QuantityError").text("");

    //    itemNo = $("#ItemNo").val().trim();
    //    uom = $("#UOM").val().trim();
    //    locationCode = $(this).val().trim();
    //    if ((itemNo != "") && (uom != "") && (locationCode != "")) {
    //        GetAvailableInventory(itemNo, uom, locationCode);
    //    } else {
    //        if (itemNo == "") {
    //            $("#ItemNoError").text("Item No. cannot be empty.");
    //        }
    //        if (uom == "") {
    //            $("#UOMError").text("UOM cannot be empty.");
    //        }
    //        if (locationCode == "") {
    //            $("#LineLocationCodeError").text("Location Code cannot be empty.");
    //        }
    //    }       
    //});
    //End onchange events

    //Add dropdown search
    AddStoreRequisitionDropDownListSearch();
    //End add dropdown search
}
//AddStoreRequisitionDropDownListSearch
function AddStoreRequisitionDropDownListSearch() {
    //$("#LocationCode").select2({
    //    placeholder: $("#LocationCodeLbl").text(),
    //    allowClear: true
    //});

    $("#ItemNo").select2({
        placeholder: $("#ItemNoLbl").text(),
        allowClear: true
    });

    $("#UOM").select2({
        placeholder: $("#UOMLbl").text(),
        allowClear: true
    });
    $("#LineGlobalDimension1Code").select2({
        placeholder: $("#Dimension1Lbl").text(),
        allowClear: true
    });
    $("#LineGlobalDimension2Code").select2({
        placeholder: $("#Dimension2Lbl").text(),
        allowClear: true
    });
    $("#LineShortcutDimension3Code").select2({
        placeholder: $("#Dimension3Lbl").text(),
        allowClear: true
    });
    $("#LineLocationCode").select2({
        placeholder: $("#LineLocationCodeLbl").text(),
        allowClear: true
    });
    if ($("#Quantity").is("select")) {
        $("#Quantity").select2({
            placeholder: $("#QuantityLbl").text(),
            allowClear: true
        });
    }
    if ($("#Inventory").is("select")) {
        $("#Inventory").select2({
            placeholder: $("#InventoryLbl").text(),
            allowClear: true
        });
    }
}
function AddOnChangeEvents() {

    $('#LineGlobalDimension1Code').change(function () {
        loaddimension2($(this).val());
    });
    $('#LineGlobalDimension2Code').change(function () {
        loaddimension3($(this).val());
    });
    $('#LineShortcutDimension3Code').change(function () {
        loaddimension4($(this).val());
    });
    $('#LineShortcutDimension4Code').change(function () {
        loaddimension5($(this).val());
    });
    $('#LineShortcutDimension5Code').change(function () {
        loaddimension6($(this).val());
    });
    $('#LineShortcutDimension7Code').change(function () {
        loaddimension8($(this).val());
    });



}
//Load store requisition lines
function LoadStoreRequisitionLines(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetStoreRequisitionLines,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (storeRequisitionLines) {
            var rows = "";

            $.each(storeRequisitionLines, function (i, storeRequisitionLine) {
                $("#AddReqLine").css("display", "block");
                rows += "<tr>";
                rows += "<td>" + storeRequisitionLine.ItemNo + "</td>";
                rows += "<td>" + storeRequisitionLine.ItemDescription + "</td>";
                rows += "<td>" + storeRequisitionLine.LineLocationCode + "</td>";
                rows += "<td>" + storeRequisitionLine.UOM + "</td>";
                /*rows += "<td>" + storeRequisitionLine.Inventory + "</td>";*/
                rows += "<td>" + storeRequisitionLine.Quantity + "</td>";
                //rows += "<td>" + storeRequisitionLine.LineGlobalDimension1Code + "</td>";
                //rows += "<td>" + storeRequisitionLine.LineGlobalDimension2Code + "</td>";
                //rows += "<td>" + storeRequisitionLine.LineShortcutDimension3Code + "</td>";
                //rows += "<td>" + storeRequisitionLine.LineShortcutDimension4Code + "</td>";
                /*rows += "<td>" + storeRequisitionLine.LineShortcutDimension5Code + "</td>";
                rows += "<td>" + storeRequisitionLine.LineShortcutDimension6Code + "</td>";
                rows += "<td>" + storeRequisitionLine.LineShortcutDimension7Code + "</td>";
                rows += "<td>" + storeRequisitionLine.LineShortcutDimension8Code + "</td>";*/
                rows += "<td><a href='#' onclick='return EditStoreRequisitionLine(" + storeRequisitionLine.LineNo + ",\"" + storeRequisitionLine.DocumentNo + "\");'>Edit</a> | <a href='#' onclick='DeleteStoreRequisitionLine(" + storeRequisitionLine.LineNo + ",\"" + storeRequisitionLine.DocumentNo + "\")'>Delete</a></td>";
                rows += "</tr>";
            });

            // remove existing table rows
            $("#StoreRequisitionLineTbl tbody tr").remove();

            // add new table rows
            $("#StoreRequisitionLineTbl tbody").append(rows);

            $("#AjaxStoreLineLoader").css("display", "none");
            $("#StoreRequisitionLineTbl").css("display", "block");
        },
        error: OnError
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
//LoadStoreRequisitionLinesView
function LoadStoreRequisitionLinesView(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetStoreRequisitionLines,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (storeRequisitionLines) {
            var rows = "";
            $.each(storeRequisitionLines, function (i, storeRequisitionLine) {
                rows += "<tr>";
                rows += "<td>" + storeRequisitionLine.ItemNo + "</td>";
                rows += "<td>" + storeRequisitionLine.ItemDescription + "</td>";
                rows += "<td>" + storeRequisitionLine.LineLocationCode + "</td>";
                rows += "<td>" + storeRequisitionLine.UOM + "</td>";
                rows += "<td>" + storeRequisitionLine.Inventory + "</td>";
                rows += "<td>" + storeRequisitionLine.Quantity + "</td>";
               // rows += "<td>" + storeRequisitionLine.LineLocationCode + "</td>";
                rows += "<td>" + storeRequisitionLine.LineGlobalDimension1Code + "</td>";
                rows += "<td>" + storeRequisitionLine.LineGlobalDimension2Code + "</td>";
                //rows += "<td>" + storeRequisitionLine.LineShortcutDimension3Code + "</td>";
                //rows += "<td>" + storeRequisitionLine.LineShortcutDimension4Code + "</td>";
                //rows += "<td>" + storeRequisitionLine.LineShortcutDimension5Code + "</td>";
                //rows += "<td>" + storeRequisitionLine.LineShortcutDimension6Code + "</td>";
                //rows += "<td>" + storeRequisitionLine.LineShortcutDimension7Code + "</td>";
                //rows += "<td>" + storeRequisitionLine.LineShortcutDimension8Code + "</td>";
                rows += "<td><a href='javascript:void(0);' onclick='return ViewStoreRequisitionLine(' + storeRequisitionLine.LineNo + ',\'" + storeRequisitionLine.DocumentNo + "\')';>View</a></td>";
                rows += "</tr>";
                $("#StoreRequisitionLineTbl tbody").html(rows);
            });
            $("#ViewStoreRequisitionLineAjaxLoader").css("display", "none");
            $("#StoreRequisitionLineTbl").css("display", "block");
        },
        error: OnError
    });
}
//Create store requisition line   
function CreateStoreRequisitionLine() {
    var documentNo = $("#No").val();

    var validLine = ValidateStoreRequisitionLine();
    if (validLine == false) {
        return false;
    }
    console.log($("#Inventory").val())

    var StoreRequisitionLineObj = {
        DocumentNo: documentNo,
        ItemNo: $("#ItemNo").val(),
        ItemDescription: $("#ItemDescription").val(),
        UOM: $("#UOM").val(),
        LineLocationCode: $("#LineLocationCode").val(),
        Inventory: $("#Inventory").val(),
        Quantity: $("#Quantity").val(),
        LineGlobalDimension1Code: $("#LineGlobalDimension1Code").val(),
        LineGlobalDimension2Code: $("#LineGlobalDimension2Code").val(),
        LineShortcutDimension3Code: $("#LineShortcutDimension3Code").val(),
        LineShortcutDimension4Code: $("#LineShortcutDimension4Code").val(),
        LineShortcutDimension5Code: $("#LineShortcutDimension5Code").val(),
        LineShortcutDimension6Code: $("#LineShortcutDimension6Code").val(),
        LineShortcutDimension7Code: $("#LineShortcutDimension7Code").val(),
        LineShortcutDimension8Code: $("#LineShortcutDimension8Code").val()
    };
    console.log(StoreRequisitionLineObj)

    $.ajax({
        url: AJAXUrls.CreateStoreRequisitionLine,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(StoreRequisitionLineObj),
        cache: false,
        success: function (result) {
            if (result.success) {
                LoadStoreRequisitionLines(documentNo);
                $("#StoreRequisitionLineModal").modal("hide");
            }else {
                Swal.fire('Error', result.message, 'error');
            }
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}
//Edit store requisition line
function EditStoreRequisitionLine(LineNo, DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetStoreRequisitionLineByLineNo,
        type: "GET",
        dataType: "json",
        data: { LineNo: LineNo, DocumentNo: DocumentNo },
        cache: false,
        success: function (result) {
            console.log(result)
            $("#LineNo").val(result.LineNo);
            $("#DocumentNo").val(result.DocumentNo);
            $("#ItemNo").val(result.ItemNo).trigger("change.select2");
            $("#ItemDescription").val(result.ItemDescription);
            $("#UOM").val(result.UOM).trigger("change.select2");
            $("#LineLocationCode").val(result.LineLocationCode).trigger("change.select2");
            $("#Inventory").val(result.Inventory);
            $("#Quantity").val(result.Quantity);
            console.log(result)
            //Dimension1
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
                    $("#LineGlobalDimension1Code").html(options);
                    $("#LineGlobalDimension1Code option[value='" + result.LineGlobalDimension1Code + "']").prop("selected", true);
                },
                error: OnError
            });
            //Dimension2
            var options1 = "";
            options1 += "<option>";
            options1 += "";
            options1 += "</option>";

            $.ajax({
                url: AJAXUrls.GetGlobalDimension2Codes,
                type: "GET",
                dataType: "json",
                data: { dimension1: result.LineGlobalDimension1Code },
                cache: false,
                success: function (Dimnesions2) {

                    var rows = "";
                    $.each(Dimnesions2.DropDownData.ListOfddlData, function (i, Dimnesions) {
                        options1 += "<option value='" + Dimnesions.Value + "'>";
                        options1 += Dimnesions.Value;
                        options1 += "</option>";
                    });
                    $("#LineGlobalDimension2Code").html(options1);
                    $("#LineGlobalDimension2Code option[value='" + result.LineGlobalDimension2Code + "']").prop("selected", true);
                },
                error: OnError
            });


            //Dimension3
            var options2 = "";
            options2 += "<option>";
            options2 += "";
            options2 += "</option>";

            $.ajax({
                url: AJAXUrls.GetGlobalDimension3Codes,
                type: "GET",
                dataType: "json",
                data: { dimension2: result.LineGlobalDimension2Code },
                cache: false,
                success: function (Dimnesions2) {

                    var rows = "";
                    $.each(Dimnesions2.DropDownData.ListOfddlData, function (i, Dimnesions) {
                        options2 += "<option value='" + Dimnesions.Value + "'>";
                        options2 += Dimnesions.Value;
                        options2 += "</option>";
                    });
                    $("#LineShortcutDimension3Code").html(options2);
                    $("#LineShortcutDimension3Code option[value='" + result.LineShortcutDimension3Code + "']").prop("selected", true);
                },
                error: OnError
            });

            //Dimension4
            var options3 = "";
            options3 += "<option>";
            options3 += "";
            options3 += "</option>";

            $.ajax({
                url: AJAXUrls.GetGlobalDimension4Codes,
                type: "GET",
                dataType: "json",
                data: { dimension3: result.LineGlobalDimension3Code },
                cache: false,
                success: function (Dimnesions3) {

                    var rows = "";
                    $.each(Dimnesions3.DropDownData.ListOfddlData, function (i, Dimnesions) {
                        options3 += "<option value='" + Dimnesions.Value + "'>";
                        options3 += Dimnesions.Value;
                        options3 += "</option>";
                    });
                    $("#LineShortcutDimension4Code").html(options3);
                    $("#LineShortcutDimension4Code option[value='" + result.LineShortcutDimension4Code + "']").prop("selected", true);
                },
                error: OnError
            });
            //Dimension5
            var options4 = "";
            options4 += "<option>";
            options4 += "";
            options4 += "</option>";

            $.ajax({
                url: AJAXUrls.GetGlobalDimension5Codes,
                type: "GET",
                dataType: "json",
                data: { dimension4: result.LineGlobalDimension4Code },
                cache: false,
                success: function (Dimnesions4) {

                    var rows = "";
                    $.each(Dimnesions4.DropDownData.ListOfddlData, function (i, Dimnesions) {
                        options4 += "<option value='" + Dimnesions.Value + "'>";
                        options4 += Dimnesions.Value;
                        options4 += "</option>";
                    });
                    $("#LineShortcutDimension5Code").html(options4);
                    $("#LineShortcutDimension5Code option[value='" + result.LineShortcutDimension5Code + "']").prop("selected", true);
                },
                error: OnError
            });
            //Dimension6
            var options5 = "";
            options5 += "<option>";
            options5 += "";
            options5 += "</option>";

            $.ajax({
                url: AJAXUrls.GetGlobalDimension6Codes,
                type: "GET",
                dataType: "json",
                data: { dimension5: result.LineGlobalDimension5Code },
                cache: false,
                success: function (Dimnesions5) {

                    var rows = "";
                    $.each(Dimnesions5.DropDownData.ListOfddlData, function (i, Dimnesions) {
                        options5 += "<option value='" + Dimnesions.Value + "'>";
                        options5 += Dimnesions.Value;
                        options5 += "</option>";
                    });
                    $("#LineShortcutDimension6Code").html(options5);
                    $("#LineShortcutDimension6Code option[value='" + result.LineShortcutDimension6Code + "']").prop("selected", true);
                },
                error: OnError
            });
            //Dimension7
            var options6 = "";
            options6 += "<option>";
            options6 += "";
            options6 += "</option>";

            $.ajax({
                url: AJAXUrls.GetGlobalDimension7Codes,
                type: "GET",
                dataType: "json",
                data: { dimension6: result.LineGlobalDimension6Code },
                cache: false,
                success: function (Dimnesions6) {

                    var rows = "";
                    $.each(Dimnesions6.DropDownData.ListOfddlData, function (i, Dimnesions) {
                        options6 += "<option value='" + Dimnesions.Value + "'>";
                        options6 += Dimnesions.Value;
                        options6 += "</option>";
                    });
                    $("#LineShortcutDimension7Code").html(options6);
                    $("#LineShortcutDimension7Code option[value='" + result.LineShortcutDimension7Code + "']").prop("selected", true);
                },
                error: OnError
            });
            //Dimension8
            var options7 = "";
            options7 += "<option>";
            options7 += "";
            options7 += "</option>";

            $.ajax({
                url: AJAXUrls.GetGlobalDimension8Codes,
                type: "GET",
                dataType: "json",
                data: { dimension7: result.LineGlobalDimension7Code },
                cache: false,
                success: function (Dimnesions7) {

                    var rows = "";
                    $.each(Dimnesions7.DropDownData.ListOfddlData, function (i, Dimnesions) {
                        options7 += "<option value='" + Dimnesions.Value + "'>";
                        options7 += Dimnesions.Value;
                        options7 += "</option>";
                    });
                    $("#LineShortcutDimension8Code").html(options7);
                    $("#LineShortcutDimension8Code option[value='" + result.LineShortcutDimension8Code + "']").prop("selected", true);
                },
                error: OnError
            });
            //$("#LineGlobalDimension1Code").val(result.LineGlobalDimension1Code);
            //$("#LineGlobalDimension2Code").val(result.LineGlobalDimension2Code).trigger("change.select2");
            //$("#LineShortcutDimension3Code").val(result.LineShortcutDimension3Code).trigger("change.select2");
            //$("#LineShortcutDimension4Code").val(result.LineShortcutDimension4Code).trigger("change.select2");
            //$("#LineShortcutDimension5Code").val(result.LineShortcutDimension5Code).trigger("change.select2");
            //$("#LineShortcutDimension6Code").val(result.LineShortcutDimension6Code).trigger("change.select2");
            //$("#LineShortcutDimension7Code").val(result.LineShortcutDimension7Code).trigger("change.select2");
            //$("#LineShortcutDimension8Code").val(result.LineShortcutDimension8Code).trigger("change.select2");

            $("#StoreRequisitionLineModal").modal("show");
            $("#CreateStoreRequisitionLineBtn").hide();
            $("#ModifyStoreRequisitionLineBtn").show();
        },
        error: OnError
    });
    return false;
}
//Modify store requisition line
function ModifyStoreRequisitionLine() {
    var documentNo = $("#No").val();

    var validLine = ValidateStoreRequisitionLine();
    if (validLine == false) {
        return false;
    }
    var storeRequisitionLineObj = {
        LineNo: $("#LineNo").val(),
        DocumentNo: documentNo,
        ItemNo: $("#ItemNo").val(),
        ItemDescription: $("#ItemDescription").val(),
        UOM: $("#UOM").val(),
        LineLocationCode: $("#LineLocationCode").val(),
        Inventory: $("#Inventory").val(),
        Quantity: $("#Quantity").val(),
        LineGlobalDimension1Code: $("#LineGlobalDimension1Code").val(),
        LineGlobalDimension2Code: $("#LineGlobalDimension2Code").val(),
        LineShortcutDimension3Code: $("#LineShortcutDimension3Code").val(),
        LineShortcutDimension4Code: $("#LineShortcutDimension4Code").val(),
        LineShortcutDimension5Code: $("#LineShortcutDimension5Code").val(),
        LineShortcutDimension6Code: $("#LineShortcutDimension6Code").val(),
        LineShortcutDimension7Code: $("#LineShortcutDimension7Code").val(),
        LineShortcutDimension8Code: $("#LineShortcutDimension8Code").val()
    };
    $.ajax({
        url: AJAXUrls.ModifyStoreRequisitionLine,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(storeRequisitionLineObj),
        cache: false,
        success: function (result) {
            if (result.success) {
                LoadStoreRequisitionLines(documentNo);
                $("#StoreRequisitionLineModal").modal("hide");

                ClearStoreRequisitionLineModal();
            }else {
                Swal.fire('Error', result.message, 'error');
            }
        },
        error: OnError
    });
}
//View store requisition line
function ViewStoreRequisitionLine(LineNo, DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetStoreRequisitionLineByLineNo,
        type: "GET",
        dataType: "json",
        data: { LineNo: LineNo, DocumentNo: DocumentNo },
        cache: false,
        success: function (result) {
            console.log(result)
            $("#LineNo").val(result.LineNo);
            $("#DocumentNo").val(result.DocumentNo);
            $("#ItemNo").val(result.ItemNo);
            $("#ItemDescription").val(result.ItemDescription);
            $("#LineLocationCode").val(result.LineLocationCode);
            $("#UOM").val(result.UOM);
            $("#Inventory").val(result.Inventory);
            $("#Quantity").val(result.Quantity);
            $("#LineGlobalDimension1Code").val(result.LineGlobalDimension1Code);
            $("#LineGlobalDimension2Code").val(result.LineGlobalDimension2Code);
            console.log(result.Quantity)
            //$("#LineShortcutDimension3Code").val(result.LineShortcutDimension3Code);
            //$("#LineShortcutDimension4Code").val(result.LineShortcutDimension4Code);
            //$("#LineShortcutDimension5Code").val(result.LineShortcutDimension5Code);
            //$("#LineShortcutDimension6Code").val(result.LineShortcutDimension6Code);
            //$("#LineShortcutDimension7Code").val(result.LineShortcutDimension7Code);
            //$("#LineShortcutDimension8Code").val(result.LineShortcutDimension8Code);

            $('#StoreRequisitionLineModal').modal('show');
        },
        error: OnError
    });
    return false;
}
//Delete store requisition line
function DeleteStoreRequisitionLine(LineNo, DocumentNo) {
    var ans = confirm("Are you sure you want to delete this Line?");
    if (ans) {
        $.ajax({
            url: AJAXUrls.DeleteStoreRequisitionLine,
            type: "POST",
            dataType: "json",
            data: { LineNo: LineNo, DocumentNo: DocumentNo },
            cache: false,
            success: function (result) {
                LoadStoreRequisitionLines(DocumentNo);
            },
            error: function (errormessage) {
                alert("Error");
            }
        });
    }
}
//Get available inventory
function GetAvailableInventory(ItemNo) {
    var availableInventory = 0;
    $.ajax({
        url: AJAXUrls.GetAvailableInventory,
        type: "GET",
        dataType: "json",
        data: { ItemNo: ItemNo },
        cache: false,
        success: function (result) {
            availableInventory = result.AvailableInventory.toLocaleString();
            $("#Inventory").val(availableInventory);
        },
        error: OnError
    });

    //Onblur Quality
    $("#Quantity").blur(function () {
        //Validate Quantity Requested
        $.ajax({
            url: AJAXUrls.ValidateQuantityRequested,
            type: "GET",
            dataType: "json",
            data: { ItemNo: ItemNo, Quantity: $(this).val() },
            cache: false,
            success: function (result) {
                if (result.success === false) {
                    $("#QuantityError").text(result.message);
                    isValid = false;
                    return isValid;
                }

                else {
                    $("#QuantityError").text("");
                }
            }
        });

    });
}
//Validate Quantity Requested
function ValidateQuantityRequested(ItemNo) {
    //Onblur Quality
    $("#Quantity").blur(function () {

        //Validate Quantity Requested
        $.ajax({
            url: AJAXUrls.ValidateQuantityRequested,
            type: "GET",
            dataType: "json",
            data: { ItemNo: ItemNo },
            cache: false,
            success: function (result) {
                if (result.success === false) {
                    $("#QuantityError").text(result.message);
                    isValid = false;
                    return isValid;
                }

                else {
                    $("#QuantityError").text("");
                }
            }
        });

    });
}
//Get store requisition amount
function GetStoreRequisitionAmount(DocumentNo) {
    var storeRequisitionAmount = 0;
    $.ajax({
        url: AJAXUrls.GetStoreRequisitionAmount,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (result) {
            storeRequisitionAmount = result.Amount.toLocaleString();
            $("#Amount").val(storeRequisitionAmount);
        },
        error: OnError
    });
}
//ValidateStoreRequisition
function ValidateStoreRequisition() {
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
    //Clear store requisition line modal
    ClearStoreRequisitionLineModal();

    return isValid;
}
//ValidateStoreRequisitionLine
function ValidateStoreRequisitionLine() {
    var isValid = true;
    var label = "";
    if ($("#ItemNo").val().trim() == "") {
        $("#ItemNoError").text("Item No. cannot be empty.");
        isValid = false;
    }
    else {
        $("#ItemNoError").text("");
    }

    //if ($("#LineLocationCode").val().trim() == "") {
    //    $("#LineLocationCodeError").text("Location Code cannot be empty.");
    //    isValid = false;
    //}
    //else {
    //    $("#LineLocationCodeError").text("");
    //}

    //if ($("#UOM").val().trim() == "") {
    //    $("#UOMError").text("UOM cannot be empty.");
    //    isValid = false;
    //}
    //else {
    //    $("#UOMError").text("");
    //}

    if ($.isNumeric($("#Quantity").val())) {
        $("#QuantityError").text("");
    } else {
        ("#QuantityError").text("Quantity must be numeric.");
        isValid = false;
    }


    //if (($("#Quantity").val() <= 0) || ($("#Quantity").val().trim() == "")) {
    //    $("#QuantityError").text("Quantity cannot be less or equal to zero.");
    //    isValid = false;
    //}
    //else {
    //    $("#QuantityError").text("");
    //}

    //if ($("#Quantity").val() > $("#Inventory").val()) {
    //    $("#QuantityError").text("The quantity requested cannot be more than the available inventory. ");
    //    isValid = false;
    //}else{
    //    $("#QuantityError").text("");
    //}

    //if ($("#LineGlobalDimension2Code").val().trim() == "") {
    //    label = $("#LineGlobalDimension2CodeLbl").text();
    //    $("#LineGlobalDimension2CodeError").text(label + " cannot be empty.");
    //    isValid = false;
    //}
    //else {
    //    $("#LineGlobalDimension2CodeError").text("");
    //}

    //if ($("#LineShortcutDimension3Code").val().trim() == "") {
    //    label = $("#LineShortcutDimension3CodeLbl").text();
    //    $("#LineShortcutDimension3CodeError").text(label + " cannot be empty.");
    //    isValid = false;
    //}
    //else {
    //    $("#LineShortcutDimension3CodeError").text("");
    //}

    //if ($("#LineShortcutDimension4Code").val().trim() == "") {
    //    label = $("#LineShortcutDimension4CodeLbl").text();
    //    $("#LineShortcutDimension4CodeError").text(label + " cannot be empty.");
    //    isValid = false;
    //}
    //else {
    //    $("#LineShortcutDimension4CodeError").text("");
    //}

    //if ($("#LineShortcutDimension5Code").val().trim() == "") {
    //    label = $("#LineShortcutDimension5CodeLbl").text();
    //    $("#LineShortcutDimension5CodeError").text(label + " cannot be empty.");
    //    isValid = false;
    //}
    //else {
    //    $("#LineShortcutDimension5CodeError").text("");
    //}

    //if ($("#LineShortcutDimension6Code").val().trim() == "") {
    //    label = $("#LineShortcutDimension6CodeLbl").text();
    //    $("#LineShortcutDimension6CodeError").text(label + " cannot be empty.");
    //    isValid = false;
    //}
    //else {
    //    $("#LineShortcutDimension6CodeError").text("");
    //}
    return isValid;
}
//Clear store requisition line modal
function ClearStoreRequisitionLineModal() {
    $("#LineNo").val(0);
    $("#DocumentNo").val("");
    $("#ItemNo").val("").trigger("change.select2");
    $("#ItemDescription").val("").trigger("change");
    $("#LineLocationCode").val("").trigger("change");
    $("#UOM").val("");
    $("#Inventory").val(0);
    $("#Quantity").val(0);
    $("#LineGlobalDimension1Code").val("").trigger("change");
    $("#LineGlobalDimension2Code").val("").trigger("change");
    $("#LineShortcutDimension3Code").val("").trigger("change");
    $("#LineShortcutDimension4Code").val("").trigger("change");
    $("#LineShortcutDimension5Code").val("").trigger("change");
    $("#LineShortcutDimension6Code").val("").trigger("change");
    $("#LineShortcutDimension7Code").val("").trigger("change");
    $("#LineShortcutDimension8Code").val("").trigger("change");

    $("#CreateStoreRequisitionLineBtn").show();
    $("#ModifyStoreRequisitionLineBtn").hide();

    $("#ItemNoError").text("");
    $("#ItemDescriptionError").text("");
    $("#LineLocationCodeError").text("");
    $("#UOMError").text("");
    $("#InventoryError").text("");
    $("#QuantityError").text("");
    $("#LineGlobalDimension1CodeError").text("");
    $("#LineGlobalDimension2CodeError").text("");
    $("#LineShortcutDimension3CodeError").text("");
    $("#LineShortcutDimension4CodeError").text("");
    $("#LineShortcutDimension5CodeError").text("");
    $("#LineShortcutDimension6CodeError").text("");
    $("#LineShortcutDimension7CodeError").text("");
    $("#LineShortcutDimension8CodeError").text("");
}

//Load Store Requisition Document
function LoadStoreRequisitionDocuments(DocumentNo) {
    $.ajax({
        url: AJAXUrls.LoadStoreRequisitionDocuments,
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

                rows += '<td><a href="#" onclick="return EditStoreRequisitionDocument(\'' + DocumentNo + '\',\'' + result.DocumentCode + '\');"><i class="fa fa-paperclip" aria-hidden="true"></i></a></td>';
                rows += "</tr>";
            });
            $("#StoresApplicationDocumentsTbl tbody").html(rows);

            $("#AjaxStoreDocumentLoader").css("display", "none");

            $("#StoresApplicationDocumentsTbl").css("display", "block");
        },
        error: function (xhr, status, error) {

        }
    });
}

//Load store Requisition Document View
function LoadStoreRequisitionDocumentsView(DocumentNo) {
    $.ajax({
        url: AJAXUrls.LoadStoreRequisitionDocuments,
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
            $("#StoresApplicationDocumentsTbl tbody").html(rows);

            $("#AjaxStoreDocumentLoader").css("display", "none");

            $("#StoresApplicationDocumentsTbl").css("display", "block");
        },
        error: function (xhr, status, error) {

        }
    });
}

//Edit store Requisition Document
function EditStoreRequisitionDocument(DocumentNo, DocumentCode) {

    //Clear link path
    ResetStoreRequisitionDocumentModal();

    $.ajax({
        url: AJAXUrls.GetStoreRequisitionDocumentByLineNo,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo, DocumentCode: DocumentCode },
        cache: false,
        success: function (result) {
            console.log(result)
            $("#DocumentNo").val(result.DocumentNo);
            $("#DocumentCode").val(result.DocumentCode);
            $("#DocumentDescription").val(result.DocumentDescription);
            $('#errorMessage').hide();
            $("#ApplicationDocumentModal").modal("show");
            $("#UploadApplicationDocumentBtn").show();
        },
        error: function (xhr, status, error) {
            console.log(xhr)
        }
    });
    return false;
}

//Upload store Requisition Document
function UploadStoreRequsitionDocuments() {

    //Assign values to this variables
    var DocumentNo = $("#DocumentNo").val();
    var DocumentCode = $("#DocumentCode").val();
    var DocumentDescription = $("#DocumentDescription").val();


    var filebase = $("#ApplicationDocumentFile").get(0);
    var files = filebase.files;

    var form = $('#ApplicationDocumentForm')[0];
    var frmData = new FormData();

    frmData.append("DocumentNo", DocumentNo);
    frmData.append("DocumentCode", DocumentCode);
    frmData.append("DocumentDescription", DocumentDescription);

    frmData.append(files[0].name, files[0]);

    //Block UI
    $.blockUI();

    $.ajax({
        url: AJAXUrls.UploadStoreRequisitionAttachments,
        type: "POST",
        data: frmData,
        dataType: 'json',
        contentType: false,
        processData: false,
        enctype: "multipart/form-data",
        async: true,
        cache: false,
        success: function (result) {
            //  $('#txtMessage').html(result.message);
            if (result.success) {
                $('#StoresApplicationDocumentModal').modal('hide');
                $('#errorMessage').hide();
                LoadStoreRequisitionDocuments(DocumentNo);
                $.unblockUI();
            } else {
                $('#errorMessage').html(result.message);
                $('#errorMessage').show();
                $.unblockUI();
            }
            Ladda.stopAll();
        },
        error: function (err) {
            $('#StoresApplicationDocumentModal').modal('show');
            $('#errorMessage').html(err.statusText);
            $('#errorMessage').show();
            Ladda.stopAll();
        }
    });
}

//Reset store Requisition Document Path
function ResetStoreRequisitionDocumentModal() {
    $("#ApplicationDocumentFile").val("");
    //Ladda.stopAll();
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
var DeleteAttachment = function (tbl, No, Id) {
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
//on error
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
    //load 2nd dimension
    function loaddimension2(dimension1) {
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
                $("#LineGlobalDimension2Code").html(options);
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
                $("#LineShortcutDimension3Code").html(options);
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
                $("#LineShortcutDimension4Code").html(options);
            },
            error: OnError
        });
    }
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
                $("#LineShortcutDimension5Code").html(options);
            },
            error: OnError
        });
    }
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
                $("#LineShortcutDimension6Code").html(options);
            },
            error: OnError
        });
    }
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
                $("#LineShortcutDimension7Code").html(options);
            },
            error: OnError
        });
    }
    function loaddimension8(dimension7) {
        var options = "";
        options += "<option>";
        options += "";
        options += "</option>";

        $.ajax({
            url: AJAXUrls.GetGlobalDimension8Codes,
            type: "GET",
            dataType: "json",
            data: { dimension7: dimension7 },
            cache: false,
            success: function (Dimnesions8) {
                var rows = "";
                $.each(Dimnesions8.DropDownData.ListOfddlData, function (i, Dimnesions) {
                    options += "<option value='" + Dimnesions.Value + "'>";
                    options += Dimnesions.Value;
                    options += "</option>";
                });
                $("#LineShortcutDimension8Code").html(options);
            },
            error: OnError
        });
    }
}