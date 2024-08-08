//Initialize claims/refund scripts
function InitializeClaimsRefundScripts() {
    //On change events
    AddOnChangeEvents();

	//Add dropdown search
	AddClaimsRefundDropDownListSearch();
	AddImprestSurrenderDropDownListSearch();
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
	$("#UnsurrenderedImprest").change(function () {
		ValidateImprestSurrenderLines($(this).val());
		LoadClaimsRefundLines($("#No").val());
	});
}

function AddImprestSurrenderDropDownListSearch() {
	$("#ReceiptNo").select2({
		placeholder: $("#ReceiptNoLbl").text(),
		allowClear: true
	});

	$("#ImprestSurrenderCode").select2({
		placeholder: $("#ImprestSurrenderCodeLbl").text(),
		allowClear: true
	});

	$("#UnsurrenderedImprest").select2({
		placeholder: $("#UnsurrenderedImprestLbl").text(),
		allowClear: true
	});
}

//Validate imprest surrender lines
function ValidateImprestSurrenderLines(UnsurrenderedImprest) {
	var DocumentNo = $("#No").val();
	console.log(DocumentNo)
	$.ajax({
		url: AJAXUrls.ValidateImprestSurrenderLines,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo, UnsurrenderedImprest: UnsurrenderedImprest },
		cache: false,
		success: function (result) {
			if (result.success == false) {
				Swal.fire('Warning', result.message, 'warning');
			} else {
				LoadClaimsRefundLines(DocumentNo);
			}
		},
		error: function (xhr, errorType, exception) {
			Swal.fire('Warning', xhr.responseText, 'warning');
		}
	});
}

//Load imprest surrender lines
function LoadImprestSurrenderLines(DocumentNo) {
	console.log(DocumentNo);
	$.ajax({
		url: AJAXUrls.GetImprestSurrenderLinesAjax,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (imprestSurrenderLines) {
			var rows = "";
			$.each(imprestSurrenderLines, function (i, imprestSurrenderLine) {
				rows += "<tr>";
				//rows += "<td>" + imprestSurrenderLine.ImprestSurrenderCode + "</td>";
				rows += "<td>" + imprestSurrenderLine.LineSurrenderDescription + "</td>";
				rows += "<td>" + imprestSurrenderLine.LineAmount.toLocaleString() + "</td>";
				rows += "<td>" + imprestSurrenderLine.LineActualAmount.toLocaleString() + "</td>";
				rows += "<td>" + imprestSurrenderLine.ReceiptNo + "</td>";
				rows += "<td>" + imprestSurrenderLine.ReceiptAmount.toLocaleString() + "</td>";
				rows += "<td>" + imprestSurrenderLine.Dimension1 + "</td>";
				rows += "<td>" + imprestSurrenderLine.Dimension2 + "</td>";
				rows += "<td>" + imprestSurrenderLine.Dimension3 + "</td>";
				rows += "<td>" + imprestSurrenderLine.Dimension4 + "</td>";
				rows += "<td>" + imprestSurrenderLine.Dimension5 + "</td>";
				rows += "<td>" + imprestSurrenderLine.Dimension6 + "</td>";
				rows += "<td>" + imprestSurrenderLine.Dimension7 + "</td>";
				//rows += '<td><a href="javascript:void(0);" onclick="return EditImprestSurrenderLine(' + imprestSurrenderLine.LineNo + ',\'' + imprestSurrenderLine.DocumentNo + '\');">Edit</a> | <a href="#" onclick="DeleteImprestSurrenderLine(' + imprestSurrenderLine.LineNo + ',\'' + imprestSurrenderLine.DocumentNo + '\')">Delete</a></td>';
				rows += '<td><a href="javascript:void(0);" onclick="return EditImprestSurrenderLine(' + imprestSurrenderLine.LineNo + ',\'' + imprestSurrenderLine.DocumentNo + '\');"><i class="fa fa-edit" aria-hidden="true">Edit</i></a></td>';
				rows += "</tr>";
				$("#ClaimsRefundLineTbl tbody").html(rows);
			});
			$("#AjaxLoader").css("display", "none");
			$("#ClaimsRefundLineTbl").css("display", "block");
			console.log(rows)
		},
		error: OnError
	});
}

//Add dropdown list
function AddClaimsRefundDropDownListSearch() { 

	$("#TransactionType").select2({
		placeholder: $("#TransactionTypeLbl").text(),
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
}

//Get claims/refund amount
function GetClaimsRefundAmount(DocumentNo) {
	var claimsRefundAmount = 0;
	$.ajax({
		url: AJAXUrls.GetClaimsRefundAmount,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			claimsRefundAmount = result.Amount.toLocaleString();
			$("#Amount").val(claimsRefundAmount);
		},
		error: OnError
	});
}

//Load claims/refund lines
function LoadClaimsRefundLines(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetClaimsRefundLines,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (claimsRefundLines) {
			console.log(claimsRefundLines)
            var rows = "";
            $.each(claimsRefundLines, function (i, claimsRefundLine) {
                rows += "<tr>";
                rows += "<td>" + claimsRefundLine.TransactionType + "</td>";
                rows += "<td>" + claimsRefundLine.LineAmount.toLocaleString() + "</td>";
				rows += "<td>" + claimsRefundLine.ActualSpent.toLocaleString() + "</td>";
                rows += "<td>" + claimsRefundLine.LineDescription + "</td>";
                rows += "<td>" + claimsRefundLine.Dimension1 + "</td>";
                rows += "<td>" + claimsRefundLine.Dimension2 + "</td>";
                rows += "<td>" + claimsRefundLine.Dimension3 + "</td>";
                rows += "<td>" + claimsRefundLine.Dimension4 + "</td>";
                rows += "<td>" + claimsRefundLine.Dimension5 + "</td>";
                rows += "<td>" + claimsRefundLine.Dimension6 + "</td>";
                rows += "<td>" + claimsRefundLine.Dimension7 + "</td>";
                rows += '<td><a href="#" onclick="return EditClaimsRefundLine(' + claimsRefundLine.LineNo + ',\'' + claimsRefundLine.DocumentNo + '\');">Edit</a> | <a href="#" onclick="DeleteClaimsRefundLine(' + claimsRefundLine.LineNo + ',\'' + claimsRefundLine.DocumentNo + '\')">Delete</a></td>';
                rows += "</tr>";
            });
            $("#ClaimsRefundLineTbl tbody").empty().append(rows);
            $("#ClaimsRefundAjaxLoader").css("display", "none");
            $("#ClaimsRefundLineTbl").css("display", "block");
        },
        error: function (xhr, status, thrownError) {
            var rows = "<tr>";
            rows += "<td class='text-danger text-center' colspan='8'>Unable to load the claims/refund lines.</td>";
            rows += "</tr>";
            $("#ClaimsRefundLineTbl tbody").empty().append(rows);
            $("#ClaimsRefundAjaxLoader").css("display", "none");
            $("#ClaimsRefundLineTbl").css("display", "block");
        }
    });
}


//Load claims/refund lines
function ViewClaimsRefundLines(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetClaimsRefundLines,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (claimsRefundLines) {
			var rows = "";
			$.each(claimsRefundLines, function (i, claimsRefundLine) {
				rows += "<tr>";
				rows += "<td>" + claimsRefundLine.TransactionType + "</td>";
				rows += "<td>" + claimsRefundLine.LineAmount.toLocaleString() + "</td>";
				rows += "<td>" + claimsRefundLine.ActualSpent.toLocaleString() + "</td>";
				rows += "<td>" + claimsRefundLine.LineDescription + "</td>";
				rows += "<td>" + claimsRefundLine.Dimension1 + "</td>";
				rows += "<td>" + claimsRefundLine.Dimension2 + "</td>";
				rows += "<td>" + claimsRefundLine.Dimension3 + "</td>";
				rows += "<td>" + claimsRefundLine.Dimension4 + "</td>";
				rows += "<td>" + claimsRefundLine.Dimension5 + "</td>";
				rows += "<td>" + claimsRefundLine.Dimension6 + "</td>";
				rows += "<td>" + claimsRefundLine.Dimension7 + "</td>";
				rows += '<td><a href="#" onclick="return ViewClaimsRefundLine(' + claimsRefundLine.LineNo + ',\'' + claimsRefundLine.DocumentNo + '\');">View</a> </td>';
				rows += "</tr>";
				$("#ClaimsRefundLineTbl tbody").html(rows);
			});
			$("#ClaimsRefundAjaxLoader").css("display", "none");
			$("#ClaimsRefundLineTbl").css("display", "block");
		},
		error: function (xhr, status, thrownError) {
			rows += "<tr>";
			rows += "<td class='text-danger text-center' colspan='8'>Unable to load the claims/refund lines.</td>"; 
			rows += "</tr>";
			$("#ClaimsRefundLineTbl tbody").html(rows);
			$("#ClaimsRefundAjaxLoader").css("display", "none");
			$("#ClaimsRefundLineTbl").css("display", "block");
		}
	});
}

//Create claims/refund line
function CreateClaimsRefundLine() {
    $(document).ready(function () {
        var documentNo = $("#No").val();
        var test = $("#Dimension1").val();
        var validLine = ValidateClaimsRefundLine();
        if (validLine == false) {
            return false;
        }

        var ClaimsRefundLineObj = {
            DocumentNo: documentNo,

            TransactionType: $("#TransactionType").val(),
            LineDescription: $("#LineDescription").val(),
            LineAmount: $("#LineAmount").val(),
			ActualSpent: $("#ActualSpent").val(),
            Dimension1: $("#Dimension1").val(),
            Dimension2: $("#Dimension2").val(),
            Dimension3: $("#Dimension3").val(),
            Dimension4: $("#Dimension4").val(),
            Dimension5: $("#Dimension5").val(),
            Dimension6: $("#Dimension6").val(),
            Dimension7: $("#Dimension7").val(),

        };
       
        $.ajax({
            url: AJAXUrls.CreateClaimsRefundLine,
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(ClaimsRefundLineObj),
            cache: false,
            success: function (result) {
                if (result.success) {
                    $('#errorMessage').hide();
                    $('#successMessage').show();
                    LoadClaimsRefundLines(documentNo);
                    GetClaimsRefundAmount(documentNo);
                    $("#ClaimsRefundLineModal").modal("hide");
                } else {
					console.log(result.message)
                    $('#errorMessage').html(result.message);
                    $('#errorMessage').show();
                    $('#successMessage').hide();
					Swal.fire('Warning', result.message, 'warning');
                }
            },
            error: function (xhr, errorType, exception) {
                alert(xhr.responseText);
            }
        });
    });
}

function LoadPettyCashDocuments(DocumentNo) {
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


//Edit claims/refund line
function EditClaimsRefundLine(LineNo, DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetClaimsRefundLine,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			console.log(result.Dimension2)
			$('#LineNo').val(result.LineNo);
			$('#DocumentNo').val(result.DocumentNo);
			$('#TransactionType').val(result.TransactionType).trigger("change");
			$('#LineDescription').val(result.LineDescription).trigger("change");
			$('#LineAmount').val(result.LineAmount);
			$('#ActualSpent').val(result.ActualSpent);
			$('#Dimension1').val(result.Dimension1).trigger("change");
			$('#Dimension2').val(result.Dimension1).trigger("change select2");
			$('#Dimension3').val(result.Dimension3).trigger("change");
			$('#Dimension4').val(result.Dimension4).trigger("change");
			$('#Dimension5').val(result.Dimension5).trigger("change");
			$('#Dimension6').val(result.Dimension6).trigger("change");
			$('#Dimension7').val(result.Dimension7).trigger("change");
			

			$("#ClaimsRefundLineModal").modal("show");
			$("#CreateClaimsRefundLineBtn").hide();
			$("#ModifyClaimsRefundLineBtn").show();
		},
		error: OnError
	});
	return false;
}

//Modify claim/refund line
function ModifyClaimsRefundLine() {

	var documentNo = $("#No").val();

	var validLine = ValidateClaimsRefundLine();
	if (validLine == false) {
		return false;
	}

	var ClaimsRefundLineObj = {
		LineNo: $("#LineNo").val(),
		DocumentNo: documentNo,
		TransactionType: $("#TransactionType").val(),
		LineDescription: $("#LineDescription").val(),
		LineAmount: $("#LineAmount").val(),
		ActualSpent: $("#ActualSpent").val(),
		Dimension1: $("#Dimension1").val(),
		Dimension2: $("#Dimension2").val(),
		Dimension3: $("#Dimension3").val(),
		Dimension4: $("#Dimension4").val(),
		Dimension5: $("#Dimension5").val(),
		Dimension6: $("#Dimension6").val(),
		Dimension7: $("#Dimension7").val(),
		//Dimension8: $("#Dimension8").val()
	};
	console.log(ClaimsRefundLineObj)
	$.ajax({
		url: AJAXUrls.ModifyClaimsRefundLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(ClaimsRefundLineObj),
		cache: false,
		success: function (result) {
			if (result.success) {
				LoadClaimsRefundLines(documentNo);
				GetClaimsRefundAmount(documentNo);
				$("#ClaimsRefundLineModal").modal("hide");

				ClearClaimsRefundLineModal();
			}else {
				Swal.fire('Error', result.message, 'Error');
			}
		},
		error: OnError
	});
}

//View claims/refund line
function ViewClaimsRefundLine(LineNo, DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetClaimsRefundLine,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			$("#LineNo").val(result.LineNo);
			$("#DocumentNo").val(result.DocumentNo);
			$("#TransactionType").val(result.TransactionType).trigger("change");
			$("#LineDescription").val(result.LineDescription).trigger("change");
			$("#LineAmount").val(result.LineAmount.toLocaleString());
			$("#ActualSpent").val(result.ActualSpent.toLocaleString());
			$('#Dimension1').val(result.Dimension1).trigger("change");
			$('#Dimension2').val(result.Dimension1).trigger("change select2");
			$('#Dimension3').val(result.Dimension3).trigger("change");
			$('#Dimension4').val(result.Dimension4).trigger("change");
			$('#Dimension5').val(result.Dimension5).trigger("change");
			$('#Dimension6').val(result.Dimension6).trigger("change");
			$('#Dimension7').val(result.Dimension7).trigger("change");

			$("#ClaimsRefundLineModal").modal('show');
		},
		error: OnError
	});
	return false;
}

//Delete claims/refund line
function DeleteClaimsRefundLine(LineNo, DocumentNo) {
	var ans = confirm("Are you sure you want to delete this Line?");
	if (ans) {
		$.ajax({
			url: AJAXUrls.DeleteClaimsRefundLine,
			type: "POST",
			dataType: "json",
			data: { LineNo: LineNo, DocumentNo: DocumentNo },
			cache: false,
			success: function (result) {
				LoadClaimsRefundLines(DocumentNo);
				GetClaimsRefundAmount(DocumentNo); 
			},
			error: function (errormessage) {
				//alert(errormessage.responseText);
				alert("Error");
			}
		});
	}
}

//Validate claims/refund header
function ValidateClaimsRefundHeader() {

	//Clear claims/refund line modal
	ClearClaimsRefundLineModal();

	var isValid = true;
	if ($('#No').val().trim() == "") {
		$('#No').css('border-color', 'Red');
		isValid = false;
	}
	else {
		$('#No').css('border-color', 'lightgrey');
	}

	return isValid;
}

//Validate claims/refund line
function ValidateClaimsRefundLine() {
	var isValid = true;
	var label = "";
	if ($("#TransactionType").val().trim() == "") {
		$("#TransactionTypeError").text("Transaction Type cannot be empty.");
		isValid = false;
	}
	else {
		$("#TransactionTypeError").text("");
	}

	if ($("#LineDescription").val().trim() == "") {
		$("#LineDescriptionError").text("Imprest line description cannot be empty.");
		isValid = false;
	}
	else {
		$("#LineDescriptionError").text("");
	}

	if (($("#LineAmount").val() <= 0) || ($("#LineAmount").val().trim() == "")) {
		$("#LineAmountError").text("Imprest line amount cannot be less or equal to zero.");
		isValid = false;
	}
	else {
		$("#LineAmountError").text("");
	}

	return isValid;
}

//Clear claims/refund line modal
function ClearClaimsRefundLineModal() {
	$("#LineNo").val(0);
	$("#DocumentNo").val("");
	$("#TransactionType").val("").trigger("change");
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

	$("#CreateClaimsRefundLineBtn").show();
	$("#ModifyClaimsRefundLineBtn").hide();

	$("#CreateFundsClaimLineBtn").show();
	$("#ModifyFundsClaimLineBtn").hide();

	$("#TransactionTypeError").text("");
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

//Load Claims/Refund Documents
function LoadClaimsRefundDocuments(DocumentNo) {
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
	/*$.ajax({
		url: AJAXUrls.GetClaimsRefundPortalDocuments,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (claimsRefundDocuments) {
			var rows = "";
			$.each(claimsRefundDocuments, function (i, claimsRefundDocument) {
				rows += "<tr>";
				rows += "<td>" + claimsRefundDocument.DocumentDescription + "</td>";
				rows += '<td><a href="#" onclick="return EditClaimsRefundDocument(\'' + claimsRefundDocument.LineNo + '\',\'' + DocumentNo + '\',\'' + claimsRefundDocument.DocumentCode + '\');"><i class="fa fa-paperclip" aria-hidden="true">Attach Document</i></a></td>';
				rows += "</tr>";
			});

			$("#ClaimsRefundAjaxLoader").css("display", "none");
			$("#ClaimsRefundDocumentsTbl tbody").html(rows);
			$("#ClaimsRefundDocumentsTbl").css("display", "block");
		},
		error: function (xhr, status, error) {

		}
	});*/
}

function AddAttachment() {
	$("#ApplicationDocumentModal").modal("show");
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
						LoadClaimsRefundDocuments(No);
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

//Load Claims/Refund Documents View
function LoadClaimsRefundDocumentsView(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetImprestPortalDocuments,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (claimsRefundDocuments) {
			var rows = "";
			$.each(claimsRefundDocuments, function (i, claimsRefundDocument) {
				rows += "<tr>";
				rows += "<td>" + claimsRefundDocument.DocumentDescription + "</td>";
				rows += '<td><a href="#" onclick="return ViewClaimsRefundDocument(\'' + DocumentNo + '\',\'' + claimsRefundDocument.DocumentCode + '\');"><i class="" aria-hidden="true">View</i></a></td>';
				rows += "</tr>";
			});

			$("#ClaimsRefundjaxLoader").css("display", "none");
			$("#ClaimsRefundDocumentsTbl tbody").html(rows);
			$("#ClaimsRefundDocumentsTbl").css("display", "block");
		},
		error: function (xhr, status, error) {

		}
	});
}

//Edit claims/refund document
function EditClaimsRefundDocument(LineNo, DocumentNo, DocumentCode) {

	ResetClaimsRefundDocumentModal();

	$.ajax({
		url: AJAXUrls.GetClaimsRefundDocumentLink,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo, DocumentCode: DocumentCode },
		cache: false,
		success: function (applicationDocument) {
			$("#DocumentNo").val(applicationDocument.DocumentNo);
			$("#DocumentCode").val(applicationDocument.DocumentCode);
			$("#DocumentDescription").val(applicationDocument.DocumentDescription);
			$('#errorMessage').hide();
			$("#ClaimsRefundDocumentModal").modal("show");
			$("#UploadClaimsRefundDocumentBtn").show();
		},
		error: function (xhr, status, error) {

		}
	});
	return false;
}

//Reset claims/refund document Line
function ResetClaimsRefundDocumentModal() {
	$("#ClaimsRefundDocumentModal").val("");
}

//Upload claims/refund document
function UploadClaimsRefundDocument() {

	var DocumentNo = $("#DocumentNo").val();
	var DocumentCode = $("#DocumentCode").val();
	var DocumentDescription = $("#DocumentDescription").val();

	var filebase = $("#ApplicationDocumentFile").get(0);
	console.log($("#ApplicationDocumentFile").val())
	var files = filebase.files;

	var form = $('ClaimsRefundDocumentForm')[0];
	var frmData = new FormData();

	frmData.append("DocumentNo", DocumentNo);
	frmData.append("DocumentCode", DocumentCode);
	frmData.append("DocumentDescription", DocumentDescription);

	frmData.append(files[0].name, files[0]);

	//Block UI
	$.blockUI();

	$.ajax({
		url: AJAXUrls.UploadClaimsRefundDocumentLink,
		type: "POST",
		data: frmData,
		dataType: 'json',
		contentType: false,
		processData: false,
		enctype: "multipart/form-data",
		async: true,
		cache: false,
		success: function (result) {
			if (result.success) {
				$('#ClaimsRefundDocumentModal').modal('hide');
				$('#errorMessage').hide();
				LoadClaimsRefundDocuments(DocumentNo);
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

//On error
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
