//Load purchase order lines
function LoadPurchaseOrderLines(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetPurchaseOrderLines,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (purchaseOrderLines) {
            var rows = "";
            $.each(purchaseOrderLines, function (i, purchaseOrderLine) {
                rows += "<tr>";
                rows += "<td>" + purchaseOrderLine.LineDescription + "</td>";
                rows += "<td>" + purchaseOrderLine.Qty.toLocaleString()  + "</td>";
                rows += "<td>" + purchaseOrderLine.QtyBase.toLocaleString() + "</td>";
                rows += "<td>" + purchaseOrderLine.UnitCost.toLocaleString() + "</td>";
                rows += "<td>" + purchaseOrderLine.LineAmount.toLocaleString() + "</td>";
                rows += "<td>" + purchaseOrderLine.LineVATAmount.toLocaleString() + "</td>";
                rows += "<td>" + purchaseOrderLine.LineAmountInclVAT.toLocaleString() + "</td>";
                rows += "<td>" + purchaseOrderLine.PlannedReceiptDate + "</td>";
                rows += "<td>" + purchaseOrderLine.ExpectedReceiptDate + "</td>";
                rows += "</tr>";
                $("#PurchaseOrderLineTbl tbody").html(rows);
            });

            $("#PurchaseOrderLineTbl").css("display", "block");
        },
        error: OnError
    });
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