//Initialize salary advance scripts
function InitializeSalaryAdvanceScripts() {
}

//Get salary advance amount
function GetSalaryAdvanceAmount(DocumentNo) {
	var salaryAdvanceAmount = 0;
	$.ajax({
		url: AJAXUrls.GetSalaryAdvanceAmount,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			salaryAdvanceAmount = result.Amount.toLocaleString();
			$("#Amount").val(salaryAdvanceAmount);
		},
		error: OnError
	});
}

//Load salary advance lines
function LoadSalaryAdvanceLines(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetSalaryAdvanceLines,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (salaryAdvanceLines) {
			var rows = "";
			$.each(salaryAdvanceLines, function (i, salaryAdvanceLine) {
				rows += "<tr>";
				rows += "<td>" + salaryAdvanceLine.LineAmount.toLocaleString() + "</td>";
				rows += '<td><a href="#" onclick="return EditSalaryAdvanceLine(' + salaryAdvanceLine.LineNo + ',\'' + salaryAdvanceLine.DocumentNo + '\');">Edit</a> | <a href="#" onclick="DeleteSalaryAdvanceLine(' + salaryAdvanceLine.LineNo + ',\'' + salaryAdvanceLine.DocumentNo + '\')">Delete</a></td>';
				rows += "</tr>";
				$("#SalaryAdvanceLineTbl tbody").html(rows);
			});
			$("#AjaxLoader").css("display", "none");
			$("#SalaryAdvanceLineTbl").css("display", "block");
		},
		error: function (xhr, status, thrownError) {
			rows += "<tr>";
			rows += "<td class='text-danger text-center' colspan='8'>Unable to load the Salary Advance lines.</td>";
			rows += "</tr>";
			$("#SalaryAdvanceLineTbl tbody").html(rows);
			$("#AjaxLoader").css("display", "none");
			$("#SalaryAdvanceTbl").css("display", "block");
		}
	});
}

function ViewSalaryAdvanceLines(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetSalaryAdvanceLines,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (salaryAdvanceLines) {
			var rows = "";
			$.each(salaryAdvanceLines, function (i, salaryAdvanceLine) {
				rows += "<tr>";
				rows += "<td>" + salaryAdvanceLine.LineAmount.toLocaleString() + "</td>";
				rows += '<td><a href="#" onclick="return ViewImprestLine(' + salaryAdvanceLine.LineNo + ',\'' + salaryAdvanceLine.DocumentNo + '\');">View</a> </td>';
				rows += "</tr>";
				$("#SalaryAdvanceLineTbl tbody").html(rows);
			});
			$("#AjaxLoader").css("display", "none");
			$("#SalaryAdvanceLineTbl").css("display", "block");
		},
		error: function (xhr, status, thrownError) {
			rows += "<tr>";
			rows += "<td class='text-danger text-center' colspan='8'>Unable to load the Salary Advance lines.</td>";
			rows += "</tr>";
			$("#SalaryAdvanceLineTbl tbody").html(rows);
			$("#AjaxLoader").css("display", "none");
			$("#SalaryAdvanceLineTbl").css("display", "block");
		}
	});
}

//Create salary advance line   
function CreateSalaryAdvanceLine() {

	var documentNo = $("#No").val();

	var validLine = ValidateSalaryAdvanceLine();
	if (validLine == false) {
		return false;
	}

	var SalaryAdvanceLineObj = {
		DocumentNo: documentNo,
		LineAmount: $("#LineAmount").val()
	};

	$.ajax({
		url: AJAXUrls.CreateSalaryAdvanceLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(SalaryAdvanceLineObj),
		cache: false,
		success: function (result) {
			LoadSalaryAdvanceLines(documentNo);
			GetSalaryAdvanceAmount(documentNo);
			$("#SalaryAdvanceLineModal").modal("hide");
		},
		error: function (xhr, errorType, exception) {
			alert(xhr.responseText);
		}
	});
}

//Edit salary advance line
function EditSalaryAdvanceLine(LineNo, DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetSalaryAdvanceLine,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			$('#LineNo').val(result.LineNo);
			$('#DocumentNo').val(result.DocumentNo);
			$('#LineAmount').val(result.LineAmount);

			$("#SalaryAdvanceLineModal").modal("show");
			$("#CreateSalaryAdvanceLineBtn").hide();
			$("#ModifySalaryAdvanceLineBtn").show();
		},
		error: OnError
	});
	return false;
}

//Modify salary advance line
function ModifySalaryAdvanceLine() {
	var documentNo = $("#No").val();

	var validLine = ValidateSalaryAdvanceLine();
	if (validLine == false) {
		return false;
	}

	var SalaryAdvanceLineObj = {
		LineNo: $("#LineNo").val(),
		DocumentNo: documentNo,
		LineAmount: $("#LineAmount").val()
	};
	$.ajax({
		url: AJAXUrls.ModifySalaryAdvanceLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(SalaryAdvanceLineObj),
		cache: false,
		success: function (result) {
			LoadSalaryAdvanceLines(documentNo);
			GetSalaryAdvanceAmount(documentNo);
			$("#SalaryAdvanceLineModal").modal("hide");

			ClearSalaryAdvanceLineModal();
		},
		error: OnError
	});
}

//View salary advance line
function ViewSalaryAdvanceLine(LineNo, DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetSalaryAdvanceLine,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			$("#LineNo").val(result.LineNo);
			$("#LineAmount").val(result.LineAmount.toLocaleString());
			
			$("#SalaryAdvanceLineModal").modal('show');
		},
		error: OnError
	});
	return false;
}

//Delete salary advance line
function DeleteSalaryAdvanceLine(LineNo, DocumentNo) {
	var ans = confirm("Are you sure you want to delete this Line?");
	if (ans) {
		$.ajax({
			url: AJAXUrls.DeleteSalaryAdvanceLine,
			type: "POST",
			dataType: "json",
			data: { LineNo: LineNo, DocumentNo: DocumentNo },
			cache: false,
			success: function (result) {
				LoadSalaryAdvanceLines(DocumentNo);
				GetSalaryAdvanceAmount(DocumentNo);
			},
			error: function (errormessage) {
				//alert(errormessage.responseText);
				alert("Error");
			}
		});
	}
}

function ValidateSalaryAdvanceLine() {
	var isValid = true;
	var label = "";

	if ($.isNumeric($("#LineAmount").val())) {
		$("#LineAmountError").text("");
	} else {
		("#LineAmountError").text("Imprest line amount must be numeric.");
		isValid = false;
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

//Clear salary advance line modal
function ClearSalaryAdvanceLineModal() {
	$("#LineNo").val(0);
	$("#DocumentNo").val("");
	$("#LineAmount").val(0);

	$("#CreateSalaryAdvanceLineBtn").show();
	$("#ModifySalaryAdvanceLineBtn").hide();

	$("#LineAmountError").text("");
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