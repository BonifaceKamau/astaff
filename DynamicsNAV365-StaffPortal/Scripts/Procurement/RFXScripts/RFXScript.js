//Load rfx lines
function LoadRFXLines(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetRFXLines,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (rfxLines) {
            var rows = "";
            $.each(rfxLines, function (i, rfxLine) {
                rows += "<tr>";
                rows += "<td>" + rfxLine.RequisitionNo + "</td>";
                rows += "<td>" + rfxLine.Type + "</td>";
                rows += "<td>" + rfxLine.Quantity.toLocaleString() + "</td>";
                rows += "<td>" + rfxLine.UOM + "</td>";
                rows += "<td>" + rfxLine.UnitPrice.toLocaleString() + "</td>";
                rows += "<td>" + rfxLine.Amount.toLocaleString() + "</td>";
                rows += "<td>" + rfxLine.ProcurementPlan + "</td>";
                rows += "</tr>";

                $("#RFXLineTbl tbody").html(rows);
            });

            $("#RFXLineTbl").css("display", "block");
        },
        error: OnError
    });
}
//Load BID Documents
function LoadBidDocuments(DocumentNo) {
    var DocNo = DocumentNo;
    var Status = "Pending Approval";
    $.ajax({
        async: true,
        type: "POST",
        datatype: "json",
        contentType: "application/json; charset = utf-8",
        processData: false,
        data: JSON.stringify({ DocNo: DocNo, TableID: 51073, Status: Status }),
        url: "/PurchaseRequisition/DocumentAttachments",
        success: function (data) {
            $("#divAttachDocs").html(data);
        },
        error: function () {
            Swal.fire("There is some problem to process your request. Please try after some time");
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
//Load rfx lines
function LoadBidAnalysisLines(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetBidAnalysisLines,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (rfxLines) {
            var rows = "";
            $.each(rfxLines, function (i, rfxLine) {
                rows += "<tr>";
                rows += "<td>" + rfxLine.ResponseID + "</td>"; 
                rows += "<td>" + rfxLine.RFQNo + "</td>";
                rows += "<td>" + rfxLine.VendorNo + "</td>";
                rows += "<td>" + rfxLine.VendorName + "</td>";
                rows += "<td>" + rfxLine.TotalQuotedAmount.toLocaleString() + "</td>";
                rows += "</tr>";

                $("#BidsLineTbl tbody").html(rows);
            });

            $("#BidsLineTbl").css("display", "block");
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