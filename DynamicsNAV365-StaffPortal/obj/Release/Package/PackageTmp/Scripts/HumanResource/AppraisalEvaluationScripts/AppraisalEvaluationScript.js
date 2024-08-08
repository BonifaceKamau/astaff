//Initialize Appraisal Evaluation Scripts
function InitializeAppraisalEvaluationScripts() {
	//OnChange Events
	AddOnChangeEvents();

	//Add Appraisal Evaluation Select List
	AddAppraisalEvaluationDropdownList();
}

//Add Appraisal Evaluation SelectList
function AddAppraisalEvaluationDropdownList() {

	$("#EvaluationStage").select2({
		placeholder: $("#EvaluationStageLbl").text(),
		allowClear: true
	});

	$("#AppraisalObjective").select2({
		placeholder: $("#AppraisalObjectiveLbl").text(),
		allowClear: true
	});

	$("#OrganizationActivityCode").select2({
		placeholder: $("#OrganizationActivityCodeLbl").text(),
		allowClear: true
	});

	$("#DepartmentActivityCode").select2({
		placeholder: $("#DepartmentActivityCodeLbl").text(),
		allowClear: true
	});

	$("#BUM").select2({
		placeholder: $("#BUMLbl").text(),
		allowClear: true
	});
}

//Add OnChange Events
function AddOnChangeEvents() {
	$("#ApprovedAppraisal").change(function () {
		ValidateAppraisalEvaluationLines($(this).val());
	});
}

//Validate Appraisal Evaluation Lines
function ValidateAppraisalEvaluationLines(AppraisalNo) {

	var DocumentNo = $("#No").val();

	$.ajax({
		url: AJAXUrls.InsertAppraisalEvaluationLines,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo, AppraisalNo: AppraisalNo },
		cache: false,
		success: function (result) {
			LoadAppraisalEvaluationLines(DocumentNo);
		},
		error: OnError
	});
}

//Load Appraisal Evaluation Lines
function LoadAppraisalEvaluationLines(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetAppraisalEvaluationLines,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (employeeAppraisalEvaluationLines) {
			var rows = "";
			$.each(employeeAppraisalEvaluationLines, function (i, employeeAppraisalEvaluationLine) {
				rows += "<tr>";
				rows += "<td>" + employeeAppraisalEvaluationLine.AppraisalNo + "</td>";
				rows += "<td>" + employeeAppraisalEvaluationLine.AppraisalPeriod + "</td>";
				rows += "<td>" + employeeAppraisalEvaluationLine.AppraisalObjective + "</td>";
				rows += "<td>" + employeeAppraisalEvaluationLine.ActivityCode + "</td>";
				rows += "<td>" + employeeAppraisalEvaluationLine.ActivityDescription + "</td>";
				rows += "<td>" + employeeAppraisalEvaluationLine.TargetValue + "</td>";
				rows += "<td>" + employeeAppraisalEvaluationLine.ActualValue + "</td>";
				rows += '<td><a href="#" onclick="return EditAppraisalEvaluationLine(' + employeeAppraisalEvaluationLine.LineNo + ',\'' + DocumentNo + '\');"><strong>Edit</strong></a> | <a href="#" onclick="DeleteAppraisalEvaluationLine(' + employeeAppraisalEvaluationLine.LineNo + ',\'' + DocumentNo + '\')"><strong>Delete</strong></a></td>';
				rows += "</tr>";

				$("#AppraisalEvaluationLineTbl tbody").html(rows);
			});

			$("#AjaxLoader").css("display", "none");
			$("#AppraisalEvaluationLineTbl").css("display", "block");

		},
		error: OnError
	});
}

//Load Appraisal Evaluation Lines View
function LoadAppraisalEvaluationLinesView(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetAppraisalEvaluationLines,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (employeeAppraisalEvaluationLines) {
			var rows = "";
			$.each(employeeAppraisalEvaluationLines, function (i, employeeAppraisalEvaluationLine) {
				rows += "<tr>";
				rows += "<td>" + employeeAppraisalEvaluationLine.AppraisalNo + "</td>";
				rows += "<td>" + employeeAppraisalEvaluationLine.AppraisalPeriod + "</td>";
				rows += "<td>" + employeeAppraisalEvaluationLine.AppraisalObjective + "</td>";
				rows += "<td>" + employeeAppraisalEvaluationLine.ActivityCode + "</td>";
				rows += "<td>" + employeeAppraisalEvaluationLine.ActivityDescription + "</td>";
				rows += "<td>" + employeeAppraisalEvaluationLine.TargetValue + "</td>";
				rows += "<td>" + employeeAppraisalEvaluationLine.ActualValue + "</td>";
				rows += '<td><a href="#" onclick="return ViewAppraisalEvaluationLine(' + employeeAppraisalEvaluationLine.LineNo + ',\'' + DocumentNo + '\');">View</a></td>';
				rows += "</tr>";

				$("#AppraisalEvaluationLineTbl tbody").html(rows);
			});

			$("#AjaxLoader").css("display", "none");
			$("#AppraisalEvaluationLineTbl").css("display", "block");

		},
		error: OnError
	});
}

//Edit Appraisal Evaluation Line
function EditAppraisalEvaluationLine(LineNo, DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetAppraisalEvaluationLine,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			$('#LineNo').val(result.LineNo);
			$('#LineNumber').val(result.LineNumber);
			$('#AppraisalNo').val(result.DocumentNo);
			$('#AppraisalPeriod').val(result.AppraisalPeriod).trigger("change");
			$('#AppraisalObjective').val(result.AppraisalObjective).trigger("change");
			$('#ActivityCode').val(result.ActivityCode);
			$('#OrganizationActivityCode').val(result.OrganizationActivityCode).trigger("change");
			$('#DepartmentActivityCode').val(result.DepartmentActivityCode).trigger("change");
			$('#ActivityDescription').val(result.ActivityDescription);
			$('#TargetValue').val(result.TargetValue);
			$('#ActualValue').val(result.ActualValue);
			$('#BUM').val(result.BUM);

			$("#AppraisalEvaluationLineModal").modal("show");
			$("#CreateAppraisalEvaluationLineBtn").hide();
			$("#ModifyAppraisalEvaluationLineBtn").show();
		},
		error: OnError
	});
	return false;
}

//Modify Appraisal Evaluation Line
function ModifyAppraisalEvaluationLine() {
	var documentNo = $("#No").val();

	var validLine = ValidateAppraisalEvaluationLine();
	if (validLine == false) {
		return false;
	}

	var appraisalEvaluationObj = {
		LineNo: $("#LineNo").val(),
		EvaluationNo: documentNo,
		ActualValue: $("#ActualValue").val()

	};

	$.ajax({
		url: AJAXUrls.ModifyAppraisalEvaluationLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(appraisalEvaluationObj),
		cache: false,
		success: function (result) {
			LoadAppraisalEvaluationLines(documentNo);
			$("#AppraisalEvaluationLineModal").modal("hide");
			ClearAppraisalEvaluationLineModal();
		},
		error: OnError
	});
}

//Validate Appraisal Evaluation Line
function ValidateAppraisalEvaluationLine() {
	var isValid = true;
	var label = "";

	if ($("#ActualValue").val().trim() == "") {
		$("#ActualValueError").text("ActualValue cannot be empty. You must enter a value");
		isValid = false;
	}
	else {
		$("#ActualValueError").text("");
	}


	if ($.isNumeric($("#ActualValue").val())) {
		$("#ActualValueError").text("");
	} else {
		("#ActualValueError").text("Actual Value must be numeric.");
		isValid = false;
	}

	if (($("#ActualValue").val() <= 0) || ($("#ActualValue").val().trim() == "")) {
		$("#ActualValueError").text("Actual Value cost cannot be less or equal to zero.");
		isValid = false;
	}
	else {
		$("#ActualValueError").text("");
	}

	return isValid;
}

//Clear Appraisal Evaluation line modal
function ClearAppraisalEvaluationLineModal() {

	$("#ActualValue").val(0.00);
	$("#ActualValueError").text("");

}

//View Appraisal Evaluation Line
function ViewAppraisalEvaluationLine(LineNo, DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetAppraisalEvaluationLine,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			$('#LineNo').val(result.LineNo);
			$('#AppraisalNo').val(result.DocumentNo);
			$('#AppraisalPeriod').val(result.AppraisalPeriod).trigger("change");
			$('#AppraisalObjective').val(result.AppraisalObjective).trigger("change");
			$('#ActivityCode').val(result.ActivityCode);
			$('#OrganizationActivityCode').val(result.OrganizationActivityCode).trigger("change");
			$('#DepartmentActivityCode').val(result.DepartmentActivityCode).trigger("change");
			$('#ActivityDescription').val(result.ActivityDescription);
			$('#TargetValue').val(result.TargetValue);
			$('#ActualValue').val(result.ActualValue);
			$('#BUM').val(result.BUM);

			$("#AppraisalEvaluationLineModal").modal("show");
			$("#CreateAppraisalEvaluationLineBtn").hide();
			$("#ModifyAppraisalEvaluationLineBtn").show();
		},
		error: OnError
	});
	return false;
}

//Delete Appraisal Evaluation Line
function DeleteAppraisalEvaluationLine(LineNo, DocumentNo) {
	var ans = confirm("Are you sure you want to delete this Line?");
	if (ans) {
		$.ajax({
			url: AJAXUrls.DeleteAppraisalEvaluationLine,
			type: "POST",
			dataType: "json",
			data: { LineNo: LineNo, DocumentNo: DocumentNo },
			cache: false,
			success: function (result) {
				LoadAppraisalEvaluationLines(DocumentNo);
			},
			error: function (errormessage) {
				//alert(errormessage.responseText);
				alert("Error");
			}
		});
	}
}

//Load Appraisal Evaluation Document
function LoadAppraisalEvaluationDocuments(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetAppraisalEvaluationDocuments,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (portalDocuments) {
			var rows = "";
			$.each(portalDocuments, function (i, portalDocument) {
				rows += "<tr>";
				rows += "<td>" + portalDocument.DocumentDescription + "</td>";
				if (portalDocument.DocumentAttached) {
					rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' checked disabled>" + "</td>";
				} else {
					rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' disabled>" + "</td>";
				}

				rows += '<td><a href="#" onclick="return EditAppraisalEvaluationDocument(\'' + DocumentNo + '\',\'' + portalDocument.DocumentCode + '\');"><i class="fa fa-paperclip" aria-hidden="true"></i></a></td>';
				rows += "</tr>";
			});
			$("#ApplicationDocumentsTbl tbody").html(rows);

			$("#AppraisalEvaluationAjaxLoader").css("display", "none");

			$("#ApplicationDocumentsTbl").css("display", "block");
		},
		error: function (xhr, status, error) {

		}
	});
}

//Load Appraisal Evaluation Document View
function LoadAppraisalEvaluationDocumentsView(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetAppraisalEvaluationDocumentsView,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (appraisalEvaluationDocuments) {
			var rows = "";
			$.each(appraisalEvaluationDocuments, function (i, appraisalEvaluationDocument) {
				rows += "<tr>";
				rows += "<td>" + appraisalEvaluationDocument.DocumentDescription + "</td>";
				if (appraisalEvaluationDocument.DocumentAttached) {
					rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' checked disabled>" + "</td>";
				} else {
					rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' disabled>" + "</td>";
				}
				rows += "</tr>";
				$("#ApplicationDocumentsTbl tbody").html(rows);
			});
			$("#AppraisalEvaluationAjaxLoader").css("display", "none");

			$("#ApplicationDocumentsTbl").css("display", "block");
		},
		error: function (xhr, status, error) {

		}
	});
}

//Edit Appraisal Evaluation Document
function EditAppraisalEvaluationDocument(DocumentNo, DocumentCode) {

	ResetAppraisalEvaluationDocumentModal();

	$.ajax({
		url: AJAXUrls.GetAppraisalEvaluationDocument,
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
			$("#UploadAppraisalEvaluationDocumentBtn").show();
		},
		error: function (xhr, status, error) {

		}
	});
	return false;
}

//Upload Appraisal Evaluation Document
function UploadAppraisalEvaluationDocuments() {

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
		url: AJAXUrls.UploadAppraisalEvaluationDocument,
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
				LoadAppraisalEvaluationDocuments(DocumentNo);
				$.unblockUI();
			} else {
				$('#errorMessage').html(result.message);
				$('#errorMessage').show();
				$.unblockUI();
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

//Reset Appraisal Evaluation Document Path
function ResetAppraisalEvaluationDocumentModal() {
	$("#ApplicationDocumentFile").val("");
}

//OnError
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