//Funds Claim Scripts
function InitializeFundsClaimScripts() {
	//Add dropdown search
	AddDropDownListSearch();
}
//Dropdown List Search
function AddDropDownListSearch() {
	$("#ImprestCode").select2({
		placeholder: $("#ImprestCodeLbl").text(),
		allowClear: true
	});
}
//Get funds claim amount
function GetFundsClaimAmount(DocumentNo) {
	var fundsClaimAmount = 0;
	$.ajax({
		url: AJAXUrls.GetFundsClaimAmount,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			fundsClaimAmount = result.Amount.toLocaleString();
			$("#Amount").val(fundsClaimAmount);
		},
		error: OnError
	});
}
//Load funds claim  lines
function LoadFundsClaimLines(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetFundsClaimLines,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (fundsClaimLines) {
			var rows = "";
			$.each(fundsClaimLines, function (i, fundsClaimLine) {
				rows += "<tr>";
				rows += "<td>" + fundsClaimLine.ImprestCode + "</td>";
				rows += "<td>" + fundsClaimLine.LineDescription + "</td>";
				rows += "<td>" + fundsClaimLine.LineAmount.toLocaleString() + "</td>";
				rows += '<td><a href="javascript:void(0);" onclick="return EditFundsClaimLine(' + fundsClaimLine.LineNo + ',\'' + fundsClaimLine.DocumentNo + '\');"><i class="fa fa-edit" aria-hidden="true">Edit</i></a> | <a href="#" onclick="DeleteFundsClaimLine(' + fundsClaimLine.LineNo + ',\'' + fundsClaimLine.DocumentNo + '\')"><i style="color:red"><strong>Delete</strong></i></a></td>';
				rows += "</tr>";
				$("#FundsClaimLineTbl tbody").html(rows);
			});
			//$("#AjaxLoader").css("display", "none");
			$("#FundsClaimLineTbl").css("display", "block");
		},
		error: OnError
	});
}
//Load ifunds claim  lines View
function LoadFundsClaimLinesView(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetFundsClaimLines,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (fundsClaimLines) {
			var rows = "";
			$.each(fundsClaimLines, function (i, fundsClaimLine) {
				rows += "<tr>";
				rows += "<td>" + fundsClaimLine.ImprestCode + "</td>";
				rows += "<td>" + fundsClaimLine.LineDescription + "</td>";
				rows += "<td>" + fundsClaimLine.LineAmount.toLocaleString() + "</td>";
				rows += '<td><a href="javascript:void(0);" onclick="return ViewFundsClaimLine(' + fundsClaimLine.LineNo + ',\'' + fundsClaimLine.DocumentNo + '\');"><i class="fa fa-search" aria-hidden="true">View</i></a></td>';
				rows += "</tr>";
				$("#FundsClaimLineTbl tbody").html(rows);
			});
		//	$("#AjaxLoader").css("display", "none");
			$("#FundsClaimLineTbl").css("display", "block");
		},
		error: OnError
	});
}
//Create funds claim line   
function CreateFundsClaimLine() {
	var documentNo = $("#No").val();

	var validLine = ValidateFundsClaimLine(); 
	if (validLine == false) {
		return false;
	}

	var FundsClaimLineObj = {
		DocumentNo: documentNo,
		ImprestCode: $("#ImprestCode").val(),
		LineDescription: $("#LineDescription").val(),
		LineAmount: $("#LineAmount").val()
	};

	$.ajax({
		url: AJAXUrls.CreateFundsClaimLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(FundsClaimLineObj),
		cache: false,
		success: function (result) {
			LoadFundsClaimLines(documentNo);
			GetFundsClaimAmount(documentNo);
			$("#FundsClaimLineModal").modal("hide");
		},
		error: function (xhr, errorType, exception) {
			alert(xhr.responseText);
		}
	});
}
//Edit funds claim line
function EditFundsClaimLine(LineNo, DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetFundsClaimLineByLineNo,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			$('#LineNo').val(result.LineNo);
			$('#DocumentNo').val(result.DocumentNo);
			$('#ImprestCode').val(result.ImprestCode).trigger("change");
			$('#LineDescription').val(result.LineDescription);
			$('#LineAmount').val(result.LineAmount);

			$("#FundsClaimLineModal").modal("show");
			$("#CreateFundsClaimLineBtn").hide();
			$("#ModifyFundsClaimLineBtn").show();
		},
		error: OnError
	});
	return false;
}
//Modify funds claim line
function ModifyFundsClaimLine() {
	var documentNo = $("#No").val();

	var validLine = ValidateFundsClaimLine(); 
	if (validLine == false) {
		return false;
	}

	var FundsClaimLineObj = {
		LineNo: $("#LineNo").val(),
		DocumentNo: documentNo,
		ImprestCode: $("#ImprestCode").val(),
		LineDescription: $("#LineDescription").val(),
		LineAmount: $("#LineAmount").val()
	};
	$.ajax({
		url: AJAXUrls.ModifyFundsClaimLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(FundsClaimLineObj),
		cache: false,
		success: function (result) {
			GetFundsClaimAmount(documentNo);
			LoadFundsClaimLines(documentNo);
			$("#FundsClaimLineModal").modal("hide");
			ClearFundsClaimLineModal();
		},
		error: OnError
	});
}
//View funds claim  line
function ViewFundsClaimLine(LineNo, DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetImprestSurrenderLine,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			$('#LineNo').val(result.LineNo);
			$('#DocumentNo').val(result.DocumentNo);
			$('#ImprestSurrenderCode').val(result.ImprestSurrenderCode);
			$('#LineSurrenderDescription').val(result.LineSurrenderDescription);
			$('#LineAmount').val(result.LineAmount.toLocaleString());
			$('#LineActualAmount').val(result.LineAmount.toLocaleString());
			$('#LineGlobalDimension1Code').val(result.LineGlobalDimension1Code);
			$('#LineGlobalDimension2Code').val(result.LineGlobalDimension2Code);
			$('#LineShortcutDimension3Code').val(result.LineShortcutDimension3Code);
			$('#LineShortcutDimension4Code').val(result.LineShortcutDimension4Code);
			$('#LineShortcutDimension5Code').val(result.LineShortcutDimension5Code);
			$('#LineShortcutDimension6Code').val(result.LineShortcutDimension6Code);
			$('#LineShortcutDimension7Code').val(result.LineShortcutDimension7Code);
			$('#LineShortcutDimension8Code').val(result.LineShortcutDimension8Code);

			$('#ImprestLineModal').modal('show');
		},
		error: OnError
	});
	return false;
}
//Delete funds claim line
function DeleteFundsClaimLine(LineNo, DocumentNo) {
	var ans = confirm("Are you sure you want to delete this Line?");
	if (ans) {
		$.ajax({
			url: AJAXUrls.DeleteFundsClaimLine,
			type: "POST",
			dataType: "json",
			data: { LineNo: LineNo, DocumentNo: DocumentNo },
			cache: false,
			success: function (result) {
				LoadFundsClaimLines(DocumentNo);
				GetFundsClaimAmount(DocumentNo);
			},
			error: function (errormessage) {
				//alert(errormessage.responseText);
				alert("Error");
			}
		});
	}
}
//Validate funds claims line
function ValidateFundsClaimLine() {
	var isValid = true;
	var label = "";
	if ($("#ImprestCode").val().trim() == "") {
		$("#ImprestCodeError").text("Funds code cannot be empty.");
		isValid = false;
	}
	else {
		$("#ImprestCodeError").text("");
	}

	if ($("#LineDescription").val().trim() == "") {
		$("#LineDescriptionError").text("description cannot be empty.");
		isValid = false;
	}
	else {
		$("#LineDescriptionError").text("");
	}

	if ($.isNumeric($("#LineAmount").val())) {
		$("#LineAmountError").text("");
	} else {
		("#LineAmountError").text("amount must be numeric.");
		isValid = false;
	}

	if (($("#LineAmount").val() <= 0) || ($("#LineAmount").val().trim() == "")) {
		$("#LineAmountError").text("amount cannot be less or equal to zero.");
		isValid = false;
	}
	else {
		$("#LineAmountError").text("");
	}

	return isValid;
}
//Clear funds claim line modal
function ClearFundsClaimLineModal() {
	$("#LineNo").val(0);
	$("#DocumentNo").val("");
	$("#ImprestCode").val("").trigger("change");
	$("#LineDescription").val("");
	$("#LineAmount").val(0);

	$("#CreateFundsClaimLineBtn").show();
	$("#ModifyFundsClaimLineBtn").hide();

	$("#FundsClaimCodeError").text("");
	$("#LineDescriptionError").text("");
	$("#LineAmountError").text("");
}
//Load attached Documents
function LoadAttachedDocuments(DocumentNo) {
	$.ajax({
		url: AJAXUrls.LoadAttachedDocuments,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (UploadedDocuments) {
			var rows = "";
			$.each(UploadedDocuments, function (i, uploadedDocument) {
				rows += "<tr>";
				rows += "<td>" + uploadedDocument.DocumentDescription + "</td>";
				if (uploadedDocument.DocumentAttached) {
					rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' checked disabled>" + "</td>";
				} else {
					rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' disabled>" + "</td>";
				}

				rows += '<td><a href="#" onclick="return EditAttachedDocument(\'' + DocumentNo + '\',\'' + uploadedDocument.DocumentCode + '\');"><i class="fa fa-paperclip" aria-hidden="true">Attach</i></a></td>';
				rows += "</tr>";
			});

			$("#AjaxLoader").css("display", "none");
			$("#ApplicationDocumentsTbl tbody").html(rows);
			$("#ApplicationDocumentsTbl").css("display", "block");
		},
		error: function (xhr, status, error) {

		}
	});
}
//Load Uploaded Documents View
function LoadAttachedDocumentsView(DocumentNo) {
	$.ajax({
		url: AJAXUrls.LoadAttachedDocuments,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (UploadedDocuments) {
			var rows = "";
			$.each(UploadedDocuments, function (i, uploadedDocuments) {
				rows += "<tr>";
				rows += "<td>" + uploadedDocuments.DocumentDescription + "</td>";
				if (uploadedDocuments.DocumentAttached) {
					rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' checked disabled>" + "</td>";
				} else {
					rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' disabled>" + "</td>";
				}

				rows += '<td><a href="#" onclick="return ViewAttachedDocument(\'' + DocumentNo + '\',\'' + uploadedDocuments.DocumentCode + '\');"><i class="" aria-hidden="true">View</i></a></td>';
				rows += "</tr>";
			});

			$("#AjaxLoader").css("display", "none");
			$("#ApplicationDocumentsTbl tbody").html(rows);
			$("#ApplicationDocumentsTbl").css("display", "block");
		},
		error: function (xhr, status, error) {

		}
	});
}
//Edit attached document
function EditAttachedDocument(DocumentNo, DocumentCode) {

	ResetDocumentModal();

	$.ajax({
		url: AJAXUrls.GetAttachedDocument,
		type: "GET",
		dataType: "json",
		data: { No: DocumentNo, DocumentCode: DocumentCode },
		cache: false,
		success: function (applicationDocument) {
			$("#DocumentNo").val(applicationDocument.DocumentNo);
			$("#DocumentCode").val(applicationDocument.DocumentCode);
			$("#DocumentDescription").val(applicationDocument.DocumentDescription);
			$('#errorMessage').hide();
			$("#ApplicationDocumentModal").modal("show");
			$("#UploadBtn").show();
		},
		error: function (xhr, status, error) {

		}
	});
	return false;
}
//Reset document modal Line
function ResetDocumentModal() {
	$("#ApplicationDocumentFile").val("");
	Ladda.stopAll();
}
//Upload document
function UploadDocument() {
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
		url: AJAXUrls.UploadDocument,
		type: "POST",
		data: frmData,
		dataType: 'json',
		contentType: false,
		processData: false,
		enctype: "multipart/form-data",
		async: true,
		cache: false,
		success: function (result) {
			$('#txtMessage').html(result.message);
			if (result.success) {
				$('#ApplicationDocumentModal').modal('hide');
				$('#errorMessage').hide();
				LoadAttachedDocuments(DocumentNo);
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
