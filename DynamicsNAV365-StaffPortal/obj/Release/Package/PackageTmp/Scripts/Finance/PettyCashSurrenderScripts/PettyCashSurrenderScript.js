//Initialize petty cash surrender scripts
function InitializePettyCashSurrenderScripts() {
	//Add dropdown search
    AddPettyCashSurrenderDropDownListSearch();

    AddOnChangeEvents();
}
function AddOnChangeEvents() {
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
    $('#requesttype').change(function () {
        loadpettycashaccounts($(this).val());
    });

    $("#UnsurrenderedPettyCash").change(function () {
        var headerNo = $("#No").val();
        ValidateImprestSurrenderLines($(this).val());
        LoadPettyCashSurrenderLines(headerNo);
    });
}

function AddPettyCashSurrenderDropDownListSearch() {
	$("#UnsurrenderedPettyCash").select2({
	    placeholder: $("#UnsurrenderedPettyCashLbl").text(),
	    allowClear: true
	});

	$("#ReceiptNo").select2({
		placeholder: $("#ReceiptNoLbl").text(),
		allowClear: true
	});


	$("#PettyCashTransactionType").select2({
		placeholder: $("#PettyCashTransactionTypeLbl").text(),
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
	$("#requesttype").select2({
	    placeholder: $("#requesttypeLbl").text(),
	    allowClear: true
	});
}

//Validate imprest surrender lines
function ValidateImprestSurrenderLines(UnsurrenderedImprest) {
    var DocumentNo = $("#No").val();
    $.ajax({
        url: AJAXUrls.ValidatePettyCashSurrenderLines,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo, UnsurrenderedImprest: UnsurrenderedImprest },
        cache: false,
        success: function (result) {
            if (result.success == false) {
                Swal.fire('Warning', result.message, 'warning');
            } else {
                LoadPettyCashSurrenderLines(DocumentNo);
            }
            
        },
        error: function () {
            Swal.fire('Warning', result.message, 'warning');
        }
    });
}

//Get petty cash surrender amount
function GetPettyCashSurrenderAmount(DocumentNo) {
	var pettyCashSurrenderAmount = 0;
	$.ajax({
		url: AJAXUrls.GetPettyCashSurrenderAmount,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			pettyCashSurrenderAmount = result.Amount.toLocaleString();
			$("#Amount").val(pettyCashSurrenderAmount);
		},
		error: OnError
	});
}

//Load Petty Cash surrender lines
function LoadPettyCashSurrenderLines(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetPettyCashSurrenderLines,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (pettyCashSurrenderLines) {
			var rows = "";
			$.each(pettyCashSurrenderLines, function (i, pettyCashSurrenderLine) {
				rows += "<tr>";
				rows += "<td>" + pettyCashSurrenderLine.AccountNo + "</td>";
				//rows += "<td>" + pettyCashSurrenderLine.AccountName + "</td>";
				rows += "<td>" + pettyCashSurrenderLine.LineDescription + "</td>";
				rows += "<td>" + pettyCashSurrenderLine.Dimension1 + "</td>";
				rows += "<td>" + pettyCashSurrenderLine.Dimension2 + "</td>";
				//rows += "<td>" + pettyCashSurrenderLine.Dimension3 + "</td>";
				//rows += "<td>" + pettyCashSurrenderLine.Dimension4 + "</td>";
				//rows += "<td>" + pettyCashSurrenderLine.Dimension5 + "</td>";
				//rows += "<td>" + pettyCashSurrenderLine.Dimension6 + "</td>";
				//rows += "<td>" + pettyCashSurrenderLine.Dimension7 + "</td>";
				rows += "<td>" + pettyCashSurrenderLine.LineAmount.toLocaleString() + "</td>";
				rows += "<td>" + pettyCashSurrenderLine.LineActualAmount.toLocaleString() + "</td>";
				if (pettyCashSurrenderLine.Status == "Open" || pettyCashSurrenderLine.Status == "Pending") {
				    rows += '<td><a href="#" onclick="return EditPettyCashSurrenderLine(' + pettyCashSurrenderLine.LineNo + ',\'' + pettyCashSurrenderLine.DocumentNo + '\');">Edit</a> | <a href="#" onclick="DeletePettyCashSurrenderLine(' + pettyCashSurrenderLine.LineNo + ',\'' + pettyCashSurrenderLine.DocumentNo + '\')">Delete</a></td>';
				}
				rows += "</tr>";
				$("#PettyCashSurrenderLineTbl tbody").html(rows);
			});
			//$("#PettyCashSurrenderAjaxLoader").css("display", "none");
			$("#PettyCashSurrenderLineTbl").css("display", "block");
		},
		error: function (xhr, status, thrownError) {
			rows += "<tr>";
			rows += "<td class='text-danger text-center' colspan='8'>Unable to load the petty cash surrender lines.</td>";
			rows += "</tr>";
			$("#PettyCashSurrenderLineTbl tbody").html(rows);
		//	$("#PettyCashSurrenderAjaxLoader").css("display", "none");
			$("#PettyCashSurrenderLineTbl").css("display", "block");
		}
	});
}

//Load Petty Cash surrender lines view
function ViewPettyCashSurrenderLines(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetPettyCashSurrenderLines,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (pettyCashSurrenderLines) {
			var rows = "";
			$.each(pettyCashSurrenderLines, function (i, pettyCashSurrenderLine) {
				rows += "<tr>";
				rows += "<td>" + pettyCashSurrenderLine.AccountNo + "</td>";
				rows += "<td>" + pettyCashSurrenderLine.AccountName + "</td>";
				rows += "<td>" + pettyCashSurrenderLine.LineDescription + "</td>";
				rows += "<td>" + pettyCashSurrenderLine.LineAmount.toLocaleString() + "</td>";
				rows += '<td><a href="#" onclick="return ViewPettyCashSurrenderLine(' + pettyCashSurrenderLine.LineNo + ',\'' + pettyCashSurrenderLine.DocumentNo + '\');">View</a> </td>';
				rows += "</tr>";
				$("#PettyCashSurrenderLineTbl tbody").html(rows);
			});
		//	$("#PettyCashSurrenderAjaxLoader").css("display", "none");
			$("#PettyCashSurrenderLineTbl").css("display", "block");
		},
		error: function (xhr, status, thrownError) {
			rows += "<tr>";
			rows += "<td class='text-danger text-center' colspan='8'>Unable to load the petty cash surrender lines.</td>";
			rows += "</tr>";
			$("#PettyCashSurrenderLineTbl tbody").html(rows);
		//	$("#PettyCashSurrenderAjaxLoader").css("display", "none");
			$("#PettyCashSurrenderLineTbl").css("display", "block");
		}
	});
}

//Create Petty Cash surrender line   
function CreatePettyCashSurrenderLine() {

	var documentNo = $("#No").val();

	var validLine = ValidatePettyCashSurrenderLine();
	if (validLine == false) {
		return false;
	}

	var PettyCashSurrenderLineObj = {
		DocumentNo: documentNo,
		PettyCashTransactionType: $("#PettyCashTransactionType").val(),
		ReceiptNo: $("#ReceiptNo").val(),
		ImprestCode: $("#ImprestCode").val(),
		LineDescription: $("#LineDescription").val(),
		LineAmount: $("#LineAmount").val(),
		LineGlobalDimension1Code: $("#Dimension1").val(),
		LineGlobalDimension2Code: $("#Dimension2").val(),
		LineShortcutDimension3Code: $("#Dimension3").val(),
		LineShortcutDimension4Code: $("#Dimension4").val(),
		LineShortcutDimension5Code: $("#Dimension5").val(),
		LineShortcutDimension6Code: $("#Dimension6").val(),
		LineShortcutDimension7Code: $("#Dimension7").val()
	};

	$.ajax({
		url: AJAXUrls.CreatePettyCashSurrenderLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(PettyCashSurrenderLineObj),
		cache: false,
		success: function (result) {
			LoadPettyCashSurrenderLines(documentNo);
			GetPettyCashSurrenderAmount(documentNo);
			$("#PettyCashSurrenderLineModal").modal("hide");
		},
		error: function (xhr, errorType, exception) {
			alert(xhr.responseText);
		}
	});
}

//Edit petty cash surrender line
function EditPettyCashSurrenderLine(LineNo, DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetPettyCashSurrenderLine,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			$('#LineNo').val(result.LineNo);
			$('#DocumentNo').val(result.DocumentNo);
			$('#ReceiptNo').val(result.ReceiptNo).trigger("change");
			$('#ImprestCode').val(result.ImprestCode).trigger("change");
			$('#PettyCashTransactionType').val(result.PettyCashTransactionType).trigger("change");
			$('#LineDescription').val(result.LineDescription).trigger("change");
			$('#LineAmount').val(result.LineAmount);
			$('#FromCity').val(result.FromCity).trigger("change");
			$('#ToCity').val(result.ToCity).trigger("change");
			$('#LineGlobalDimension1Code').val(result.LineGlobalDimension1Code).trigger("change");
			$('#LineGlobalDimension2Code').val(result.LineGlobalDimension2Code).trigger("change");
			$('#LineShortcutDimension3Code').val(result.LineShortcutDimension3Code).trigger("change");
			$('#LineShortcutDimension4Code').val(result.LineShortcutDimension4Code).trigger("change");
			$('#LineShortcutDimension5Code').val(result.LineShortcutDimension5Code).trigger("change");
			$('#LineShortcutDimension6Code').val(result.LineShortcutDimension6Code).trigger("change");
			$('#LineShortcutDimension7Code').val(result.LineShortcutDimension7Code).trigger("change");
			$('#LineShortcutDimension8Code').val(result.LineShortcutDimension8Code).trigger("change");

			$("#PettyCashSurrenderLineModal").modal("show");
			$("#CreatePettyCashSurrenderLineBtn").hide();
			$("#ModifyPettyCashSurrenderLineBtn").show();
		},
		error: OnError
	});
	return false;
}

//Modify petty cash surrender line
function ModifyPettyCashSurrenderLine() {
	var documentNo = $("#No").val();

	//var validLine = ValidatePettyCashSurrenderLine();
	//if (validLine == false) {
	//	return false;
	//}

	var PettyCashSurrenderLineObj = {
		LineNo: $("#LineNo").val(),
		DocumentNo: documentNo,
		ImprestCode: $("#ImprestCode").val(),
		PettyCashTransactionType: $("#PettyCashTransactionType").val(),
		ReceiptNo: $("#ReceiptNo").val(),
		LineDescription: $("#LineDescription").val(),
		LineAmount: $("#LineAmount").val(),
		LineActualAmount: $("#LineActualAmount").val(),
		//Dimension1: $("#Dimension1").val(),
		//Dimension2: $("#Dimension2").val(),
		//Dimension3: $("#Dimension3").val(),
		//Dimension4: $("#Dimension4").val(),
		//Dimension5: $("#Dimension5").val(),
		//Dimension6: $("#Dimension6").val(),
		//Dimension7: $("#Dimension7").val()
	};
	$.ajax({
		url: AJAXUrls.ModifyPettyCashSurrenderLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(PettyCashSurrenderLineObj),
		cache: false,
		success: function (result) {
			LoadPettyCashSurrenderLines(documentNo);
			GetPettyCashSurrenderAmount(documentNo);
			$("#PettyCashSurrenderLineModal").modal("hide");

			ClearPettyCashLineModal();
		},
		error: OnError
	});
}

//View petty cash surrender line
function ViewPettyCashSurrenderLine(LineNo, DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetPettyCashSurrenderLine,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			$("#LineNo").val(result.LineNo);
			$("#DocumentNo").val(result.DocumentNo);
			$("#PettyCashTransactionType").val(result.PettyCashTransactionType).trigger("change");
			$("#ImprestCode").val(result.ImprestCode).trigger("change");
			$("#LineDescription").val(result.LineDescription).trigger("change");
			$("#LineAmount").val(result.LineAmount.toLocaleString());
			$("#LineGlobalDimension1Code").val(result.LineGlobalDimension1Code).trigger("change");
			$("#LineGlobalDimension2Code").val(result.LineGlobalDimension2Code).trigger("change");
			$("#LineShortcutDimension3Code").val(result.LineShortcutDimension3Code).trigger("change");
			$("#LineShortcutDimension4Code").val(result.LineShortcutDimension4Code).trigger("change");
			$("#LineShortcutDimension5Code").val(result.LineShortcutDimension5Code).trigger("change");
			$("#LineShortcutDimension6Code").val(result.LineShortcutDimension6Code).trigger("change");
			$("#LineShortcutDimension7Code").val(result.LineShortcutDimension7Code).trigger("change");
			$("#LineShortcutDimension8Code").val(result.LineShortcutDimension8Code).trigger("change");

			$("#PettyCashSurrenderLineModal").modal('show');
		},
		error: OnError
	});
	return false;
}

//Delete petty cash surrender line
function DeletePettyCashSurrenderLine(LineNo, DocumentNo) {
	var ans = confirm("Are you sure you want to delete this Line?");
	if (ans) {
		$.ajax({
			url: AJAXUrls.DeletePettyCashSurrenderLine,
			type: "POST",
			dataType: "json",
			data: { LineNo: LineNo, DocumentNo: DocumentNo },
			cache: false,
			success: function (result) {
				LoadPettyCashSurrenderLines(DocumentNo);
				GetPettyCashSurrenderAmount(DocumentNo);
			},
			error: function (errormessage) {
				alert("Error");
			}
		});
	}
}

function ValidatePettyCashSurrenderHeader() {

	//Clear petty cash surrender line modal
	ClearPettyCashSurrenderLineModal();

	var isValid = true;
	if ($('#No').val().trim() == "") {
		$('#No').css('border-color', 'Red');
		isValid = false;
	}
	else {
		$('#No').css('border-color', 'lightgrey');
	}

	//if ($('#GlobalDimension1Code').val().trim() == "") {
	//	$("#GlobalDimension1CodeError").show();
	//	isValid = false;
	//}
	//else {
	//	$('#errorGlobalDimension1Code').hide();
	//}

	return isValid;
}

function ValidatePettyCashSurrenderLine() {
	var isValid = true;
	var label = "";
	//if ($("#ReceiptNo").val().trim() == "") {
	//	$("#ReceiptNoError").text("Please select Receipt No.");
	//	isValid = false;
	//}
	//else {
	//	$("#ReceiptNoError").text("");
	//}

	if ($("#PettyCashTransactionType").val().trim() == "") {
		$("#PettyCashTransactionTypeError").text("PettyCash TransactionType required..");
		isValid = false;
	}
	else {
		$("#PettyCashTransactionTypeError").text("");
	}

	if ($("#LineDescription").val().trim() == "") {
		$("#LineDescriptionError").text("Petty Cash line description cannot be empty.");
		isValid = false;
	}
	else {
		$("#LineDescriptionError").text("");
	}

	//if ($.isNumeric($("#LineAmount").val())) {
	//	$("#LineAmountError").text("");
	//} else {
	//	("#LineAmountError").text("Petty Cash line amount must be numeric.");
	//	isValid = false;
	//}

	if (($("#LineAmount").val() <= 0) || ($("#LineAmount").val().trim() == "")) {
		$("#LineAmountError").text("Petty Cash line amount cannot be less or equal to zero.");
		isValid = false;
	}
	else {
		$("#LineAmountError").text("");
	}
	return isValid;
}

//Clear petty cash surrender line modal
function ClearPettyCashSurrenderLineModal() {
	$("#LineNo").val(0);
	$("#DocumentNo").val("");
	$("#PettyCashTransactionType").val("").trigger("change");
	$("#ReceiptNo").val("").trigger("change");
	$("#ImprestCode").val("").trigger("change");
	$("#LineDescription").val("");
	$("#LineAmount").val(0);
	$("#LineGlobalDimension1Code").val("").trigger("change");
	$("#LineGlobalDimension2Code").val("").trigger("change");
	$("#LineShortcutDimension3Code").val("").trigger("change");
	$("#LineShortcutDimension4Code").val("").trigger("change");
	$("#LineShortcutDimension5Code").val("").trigger("change");
	$("#LineShortcutDimension6Code").val("").trigger("change");
	$("#LineShortcutDimension7Code").val("").trigger("change");
	$("#LineShortcutDimension8Code").val("").trigger("change");

	$("#CreatePettyCashSurrenderLineBtn").show();
	$("#ModifyPettyCashSurrenderLineBtn").hide();

	
	$("#ReceiptNoError").text("");
	$("#ImprestCodeError").text("");
	$("#PettyCashTransactionTypeError").text("");
	$("#LineDescriptionError").text("");
	$("#LineAmountError").text("");
	$("#LineGlobalDimension1CodeError").text("");
	$("#LineGlobalDimension2CodeError").text("");
	$("#LineShortcutDimension3CodeError").text("");
	$("#LineShortcutDimension4CodeError").text("");
	$("#LineShortcutDimension5CodeError").text("");
	$("#LineShortcutDimension6CodeError").text("");
	$("#LineShortcutDimension7CodeError").text("");
	$("#LineShortcutDimension8CodeError").text("");
}

//Load petty cashsurrender Documents
function LoadPettyCashSurrenderDocuments(DocumentNo) {
    var DocNo = DocumentNo;
    var Status = $("#Status").val();
    $.ajax({
        async: true,
        type: "POST",
        datatype: "json",
        contentType: "application/json; charset = utf-8",
        processData: false,
        data: JSON.stringify({ DocNo: DocNo, TableID: 51525003, Status: Status }),
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
                data: JSON.stringify({ DocNo: No, DocID: Id, tblID: 51525003 }),
                contentType: "application/json; charset = utf-8",
                success: function (data) {
                    if (data.success == true) {
                        LoadPettyCashSurrenderDocuments(No);
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

var GenerateReport = function (DocNo) {
    $.ajax({
        async: true,
        type: "POST",
        datatype: "json",
        contentType: "application/json; charset = utf-8",
        processData: false,
        data: JSON.stringify({ DocNo: DocNo }),

        url: '/PettyCashSurrender/GenerateReport',
        success: function (data) {
            window.open(data.message, '_blank').focus();
            //window.alert(data.message);
            //window.location.reload();
        },
        error: function () {
            window.alert(data.message);
        }
    });
};

//Load petty cash surrender Documents View
function LoadPettyCashSurrenderDocumentsView(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetPettyCashSurrenderDocuments,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (pettyCashSurrenderDocuments) {
			var rows = "";
			$.each(pettyCashSurrenderDocuments, function (i, pettyCashSurrenderDocument) {
				rows += "<tr>";
				rows += "<td>" + pettyCashSurrenderDocument.DocumentDescription + "</td>";
				//if (imprestUploadedDocument.DocumentAttached) {
				//	rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' checked disabled>" + "</td>";
				//} else {
				//	rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' disabled>" + "</td>";
				//}

				rows += '<td><a href="#" onclick="return ViewImprestDocument(\'' + DocumentNo + '\',\'' + pettyCashSurrenderDocument.DocumentCode + '\');"><i class="" aria-hidden="true">View</i></a></td>';
				rows += "</tr>";
			});

			$("#PettyCashDocumentsAjaxLoader").css("display", "none");
			$("#ApplicationDocumentsTbl tbody").html(rows);
			$("#ApplicationDocumentsTbl").css("display", "block");
		},
		error: function (xhr, status, error) {

		}
	});
}

//Edit petty cash surrender document
function EditPettyCashSurrenderDocument(LineNo, DocumentNo, DocumentCode) {

	ResetPettyCashSurrenderDocumentModal();

	$.ajax({
		url: AJAXUrls.GetPettyCashSurrenderDocumentLink,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo, DocumentCode: DocumentCode },
		cache: false,
		success: function (applicationDocument) {
			$("#DocumentNo").val(applicationDocument.DocumentNo);
			$("#DocumentCode").val(applicationDocument.DocumentCode);
			$("#DocumentDescription").val(applicationDocument.DocumentDescription);
			$('#errorMessage').hide();
			$("#ApplicationDocumentModal").modal("show");
			$("#UploadApplicationDocumentBtn").show();
		},
		error: function (xhr, status, error) {

		}
	});
	return false;
}

//Reset petty cash surrender document Line
function ResetPettyCashSurrenderDocumentModal() {
	$("#ApplicationDocumentFile").val("");
	//Ladda.stopAll();
}
function AddAttachment() {
    $("#ApplicationDocumentModal").modal("show");
}

//Upload petty cash surrender document
function UploadPettyCashSurrenderDocument() {
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
		url: AJAXUrls.UploadPettyCashSurrenderDocumentLink,
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
				LoadPettyCashSurrenderDocuments(DocumentNo);
				$.unblockUI();
			} else {
				$('#errorMessage').html(result.message);
				$('#errorMessage').show();
				$.unblockUI();
			}

			Ladda.stopAll();
		}

	});
}

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

//Error
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