//Initialize Emlpoyee Appraisal Scripts
function InitializeEmlpoyeeAppraisalScripts() {

	//Add OnChange Events
	AddOnChangeEvents();

	//Add dropdown search
	AddEmployeeAppraisalDropDownListSearch();
}

//Add OnChange Events
function AddOnChangeEvents() {

	//Organizational Appraisal Codes
	$("#AppraisalObjective").change(function () {
		LoadOrganizationActivityCodes($(this).val());
		GetObjectiveWeight($(this).val());
	});

	//Departmental Appraisal Codes
	$("#OrganizationActivityCode").change(function () {
		LoadDepartmentalAppraisalCodes($(this).val());
	});

}

//Add Dropdown List
function AddEmployeeAppraisalDropDownListSearch() {

	$("#AppraisalPeriod").select2({
		placeholder: $("#AppraisalPeriodLbl").text(),
		allowClear: true
	});

	$("#CalendarPeriod").select2({
		placeholder: $("#CalendarPeriodLbl").text(),
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

	$("#AppraisalScoreType").select2({
		placeholder: $("#AppraisalScoreTypeLbl").text(),
		allowClear: true
	});

	$("#ParameterType").select2({
		placeholder: $("#ParameterTypeLbl").text(),
		allowClear: true
	});

	$("#BUM").select2({
		placeholder: $("#BUMLbl").text(),
		allowClear: true
	});
}

//Get Objective Weight
function GetObjectiveWeight(GlobalAppraisalObjective) {
	var objectiveWeight = 0;
	$.ajax({
		url: AJAXUrls.GetObjectiveWeight,
		type: "GET",
		dataType: "json",
		data: { GlobalAppraisalObjective: GlobalAppraisalObjective },
		cache: false,
		success: function (result) {
			objectiveWeight = result.ObjectiveWeight.toLocaleString();
			$("#ObjectiveWeight").val(objectiveWeight);
		},
		error: OnError
	});
}

//Load organizational activity Codes
function LoadOrganizationActivityCodes(GlobalObjective) {
	var options = "";
	options += "<option>";
	options += "";
	options += "</option>";

	$.ajax({
		url: AJAXUrls.GetOrganizationActivityCodes,
		type: "GET",
		dataType: "json",
		data: { GlobalObjective: GlobalObjective },
		cache: false,
		success: function (organizationalActivityCodes) {
			$.each(organizationalActivityCodes, function (i, organizationalActivityCode) {
				options += "<option value='" + organizationalActivityCode.Activity_Code + "'>";
				options += organizationalActivityCode.Activity_Description;
				options += "</option>";
			});
			$("#OrganizationActivityCode").html(options);
		},
		error: OnError
	});
}

//Load Departmental Appraisal Codes
function LoadDepartmentalAppraisalCodes(OrganizationalAppraisalCode) {
	var options = "";
	options += "<option>";
	options += "";
	options += "</option>";

	$.ajax({
		url: AJAXUrls.GetDepartmentalAppraisalCodes,
		type: "GET",
		dataType: "json",
		data: { OrganizationalAppraisalCode: OrganizationalAppraisalCode },
		cache: false,
		success: function (departmentalAppraisalCodes) {
			$.each(departmentalAppraisalCodes, function (i, departmentalAppraisalCode) {
				options += "<option value='" + departmentalAppraisalCode.Activity_Code + "'>";
				options += departmentalAppraisalCode.Activity_Description;
				options += "</option>";
			});
			$("#DepartmentActivityCode").html(options);
		},
		error: OnError
	});
}

//Edit Employee Appraisal Line
function EditEmployeeAppraisalLine(DocumentNo, ActivityCode, AppraisalPeriod, AppraisalObjective, OrganizationActivityCode, DepartmentActivityCode) {

	$.ajax({
		url: AJAXUrls.GetEmployeeAppraisalLine,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo, ActivityCode: ActivityCode, AppraisalPeriod: AppraisalPeriod, AppraisalObjective: AppraisalObjective, OrganizationActivityCode: OrganizationActivityCode, DepartmentActivityCode: DepartmentActivityCode },
		cache: false,
		success: function (result) {

			$('#DocumentNo').val(DocumentNo);
			$('#AppraisalPeriod').val(result.AppraisalPeriod);
			$('#AppraisalObjective').val(AppraisalObjective);
			$('#ActivityCode').val(ActivityCode);
			$('#ActivityWeight').val(result.ActivityWeight);
			$('#TargetValue').val(result.TargetValue);
			$('#BUM').val(result.BUM);
			$('#OrganizationActivityCode').val(OrganizationActivityCode);
			$('#DepartmentActivityCode').val(DepartmentActivityCode);
			$('#ActivityDescription').val(result.ActivityDescription);

			$("#EmployeeAppraisalLineModal").modal("show");
			$("#CreateEmployeeAppraisalLineBtn").hide();
			$("#ModifyEmployeeAppraisalLineBtn").show();
		},

		error: OnError
	});

	return false;
}

//View Employee Appraisal Line
function ViewEmployeeAppraisalLine(DocumentNo, AppraisalObjective, OrganizationActivityCode, DepartmentActivityCode, ActivityCode, AppraisalPeriod) {

	//Add EmployeeAppraisal DropDown List Search
	AddEmployeeAppraisalDropDownListSearch();

	$.ajax({
		url: AJAXUrls.GetEmployeeAppraisalLine,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo, AppraisalObjective: AppraisalObjective, OrganizationActivityCode: OrganizationActivityCode, DepartmentActivityCode: DepartmentActivityCode, ActivityCode: ActivityCode, AppraisalPeriod: AppraisalPeriod },
		cache: false,
		success: function (result) {
			$('#DocumentNo').val(DocumentNo);
			$('#AppraisalPeriod').val(result.AppraisalPeriod);
			$('#AppraisalObjective').val(AppraisalObjective);
			$('#ActivityCode').val(ActivityCode);
			$('#ActivityWeight').val(result.ActivityWeight);
			$('#TargetValue').val(result.TargetValue);
			$('#BUM').val(result.BUM);
			$('#OrganizationActivityCode').val(OrganizationActivityCode);
			$('#DepartmentActivityCode').val(DepartmentActivityCode);
			$('#ActivityDescription').val(result.ActivityDescription);

			$("#EmployeeAppraisalLineModal").modal("show");
		},

		error: OnError
	});

	return false;
}

//Create Employee Appraisal Line
function CreateEmployeeAppraisalLine() {

	var documentNo = $("#DocumentNo").val();

	var validLine = ValidateEmployeeAppraisalLine();
	if (validLine == false) {
		return false;
	}

	var employeeAppraisalObj = {
		DocumentNo: documentNo,
		AppraisalObjective: $("#AppraisalObjective").val(),
		AppraisalPeriod: $("#AppraisalPeriod").val(),
		OrganizationActivityCode: $("#OrganizationActivityCode").val(),
		DepartmentActivityCode: $("#DepartmentActivityCode").val(),
		ActivityWeight: $("#ActivityWeight").val(),
		ActivityDescription: $("#ActivityDescription").val(),
		TargetValue: $("#TargetValue").val(),
		AppraisalScoreType: $("#AppraisalScoreType").val(),
		ParameterType: $("#ParameterType").val(),
		BUM: $("#BUM").val()
	};

	$.ajax({
		url: AJAXUrls.CreateEmployeeAppraisalLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(employeeAppraisalObj),
		cache: false,
		success: function (result) {
			LoadEmployeeAppraisalLines(documentNo);
			$('#EmployeeAppraisalLineModal').modal('hide');
			ClearEmployeeAppraisalLineModal();
		},
		error: OnError
	});
}

//Modify Employee Appraisal Line
function ModifyEmployeeAppraisalLine() {

	var documentNo = $("#DocumentNo").val();

	var validLine = ValidateEmployeeAppraisalLine();
	if (validLine == false) {
		return false;
	}

	var employeeAppraisalObj = {
		DocumentNo: documentNo,
		AppraisalObjective: $("#AppraisalObjective").val(),
		AppraisalPeriod: $("#AppraisalPeriod").val(),
		OrganizationActivityCode: $("#OrganizationActivityCode").val(),
		DepartmentActivityCode: $("#DepartmentActivityCode").val(),
		ActivityDescription: $("#ActivityDescription").val(),
		ActivityWeight: $("#ActivityWeight").val(),
		TargetValue: $("#TargetValue").val(),
		AppraisalScoreType: $("#AppraisalScoreType").val(),
		ParameterType: $("#ParameterType").val(),
		BUM: $("#BUM").val()
	};

	$.ajax({
		url: AJAXUrls.ModifyEmployeeAppraisalLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(employeeAppraisalObj),
		cache: false,
		success: function (result) {
			LoadEmployeeAppraisalLines(documentNo);
			$("#EmployeeAppraisalLineModal").modal('hide');
			ClearEmployeeAppraisalLineModal();
		},
		error: OnError
	});
}

//Load Employee Appraisal Lines
function LoadEmployeeAppraisalLines(DocumentNo) {

	$.ajax({
		url: AJAXUrls.GetEmployeeAppraisalLines,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (appraisalLines) {
			var rows = "";
			$.each(appraisalLines, function (i, result) {
				rows += "<tr>";
				rows += "<td>" + result.AppraisalPeriod + "</td>";
				rows += "<td>" + result.AppraisalObjective + "</td>";
				rows += "<td>" + result.OrganizationActivityCode + "</td>";
				rows += "<td>" + result.DepartmentActivityCode + "</td>";
				rows += "<td>" + result.ActivityDescription + "</td>";
				rows += "<td>" + result.ObjectiveWeight.toLocaleString() + "</td>";
				rows += "<td>" + result.ActivityWeight.toLocaleString() + "</td>";
				rows += "<td>" + result.TargetValue.toLocaleString() + "</td>";
				rows += "<td>" + result.AppraisalScoreType.toLocaleString() + "</td>";
				rows += "<td>" + result.ParameterType.toLocaleString() + "</td>";
				rows += "<td>" + result.BUM + "</td>";
				rows += '<td><a href="#" onclick="return EditEmployeeAppraisalLine(\'' + DocumentNo + '\',\'' + result.ActivityCode + '\',\'' + result.AppraisalPeriod + '\',\'' + result.AppraisalObjective + '\',\'' + result.OrganizationActivityCode + '\',\'' + result.DepartmentActivityCode + '\');"><i class="fa fa-pencil" style="color:blue" aria-hidden="true"><strong>Edit</strong></i></a> | <a href="#" onclick="return DeleteEmployeeAppraisalLine(\'' + DocumentNo + '\',\'' + result.ActivityCode + '\',\'' + result.AppraisalPeriod + '\',\'' + result.AppraisalObjective + '\',\'' + result.OrganizationActivityCode + '\',\'' + result.DepartmentActivityCode + '\');"><i class="fa fa-trash-o" style="color:red" aria-hidden="true"><strong>Delete</strong></i></a>';
				rows += "</tr>";

				$("#EmployeeAppraisalLineTbl tbody").html(rows);
			});

			$("#AjaxLoader").css("display", "none");
			$("#EmployeeAppraisalLineTbl").css("display", "block");

		},
		error: OnError
	});
}

//Load Employee Appraisal Lines View
function LoadEmployeeAppraisalLinesView(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetEmployeeAppraisalLines,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (appraisalLines) {
			var rows = "";
			$.each(appraisalLines, function (i, result) {
				rows += "<tr>";
				rows += "<td>" + result.AppraisalPeriod + "</td>";
				rows += "<td>" + result.AppraisalObjective + "</td>";
				rows += "<td>" + result.OrganizationActivityCode + "</td>";
				rows += "<td>" + result.DepartmentActivityCode + "</td>";
				rows += "<td>" + result.ActivityDescription + "</td>";
				rows += "<td>" + result.ObjectiveWeight.toLocaleString() + "</td>";
				rows += "<td>" + result.ActivityWeight.toLocaleString() + "</td>";
				rows += "<td>" + result.TargetValue.toLocaleString() + "</td>";
				rows += "<td>" + result.AppraisalScoreType.toLocaleString() + "</td>";
				rows += "<td>" + result.ParameterType.toLocaleString() + "</td>";
				rows += "<td>" + result.BUM + "</td>";
				rows += '<td><a href="#" onclick="return ViewEmployeeAppraisalLine(\'' + DocumentNo + '\',\'' + result.AppraisalObjective + '\',\'' + result.OrganizationalAppraisalCode + '\',\'' + result.DepartmentAppraisalCode + '\',\'' + result.ActivityCode + '\',\'' + result.AppraisalPeriod + '\');"><i class="fa fa-update" style="color:blue" aria-hidden="true"><strong>View</strong></i></a>';
				rows += "</tr>";

				$("#EmployeeAppraisalLineTbl tbody").html(rows);
			});

			$("#AjaxLoader").css("display", "none");
			$("#EmployeeAppraisalLineTbl").css("display", "block");

		},
		error: OnError
	});
}

//Validate Employee Appraisal Appraisal
function ValidateEmployeeAppraisalHeader() {
	var isValid = true;
	if ($('#DocumentNo').val().trim() == "") {
		$('#DocumentNo').css('border-color', 'Red');
		isValid = false;
	}
	else {
		$('#DocumentNo').css('border-color', 'lightgrey');
	}


	if ($("#CalendarPeriod").val().trim() == "") {
		$("#CalendarPeriodError").text("Calendar Period. Required");
		isValid = false;
	}
	else {
		$("#CalendarPeriodError").text("");
	}

	//Clear Employee line modal
	ClearEmployeeAppraisalLineModal();

	return isValid;

}

//Validate Employee Appraisal Appraisal Line
function ValidateEmployeeAppraisalLine() {
	var isValid = true;
	var label = "";

	if ($("#AppraisalObjective").val().trim() == "") {
		$("#AppraisalObjectiveError").text("Appraisal Objective. Required");
		isValid = false;
	}
	else {
		$("#AppraisalObjectiveError").text("");
	}

	if ($("#OrganizationActivityCode").val().trim() == "") {
		$("#OrganizationActivityCodeError").text("Organizational Activity Code. Required");
		isValid = false;
	}
	else {
		$("#OrganizationActivityCodeError").text("");
	}

	if ($("#DepartmentActivityCode").val().trim() == "") {
		$("#DepartmentActivityCodeError").text("Pull Activity From Organization Level.");
		isValid = false;
	}
	else {
		$("#DepartmentActivityCodeError").text("");
	}

	if ($("#AppraisalScoreType").val().trim() == "") {
		$("#AppraisalScoreTypeError").text("Appraisal Score Type cannot be empty");
		isValid = false;
	}
	else {
		$("#AppraisalScoreTypeError").text("");
	}

	if ($("#ParameterType").val().trim() == "") {
		$("#ParameterTypeError").text("Parameter Type cannot be empty");
		isValid = false;
	}
	else {
		$("#ParameterTypeError").text("");
	}

	if ($("#ActivityDescription").val().trim() == "") {
		$("#ActivityDescriptionError").text("Individual activity description cannot be empty");
		isValid = false;
	}
	else {
		$("#ActivityDescriptionError").text("");
	}

	if ($("#BUM").val().trim() == "") {
		$("#BUMError").text("Base Unit of Measure. Required");
		isValid = false;
	}
	else {
		$("#BUMError").text("");
	}

	if ($.isNumeric($("#ActivityWeight").val())) {
		$("#ActivityWeightError").text("");
	} else {
		("#ActivityWeightError").text("Activity Weight must be numeric.");
		isValid = false;
	}

	if (($("#ActivityWeight").val() <= 0) || ($("#ActivityWeight").val().trim() == "")) {
		$("#ActivityWeightError").text("Activity Weight cost cannot be less or equal to zero.");
		isValid = false;
	}
	else {
		$("#ActivityWeightError").text("");
	}

	return isValid;
}

//Delete employee appraisal line
function DeleteEmployeeAppraisalLine(DocumentNo, ActivityCode, AppraisalPeriod, AppraisalObjective, OrganizationActivityCode, DepartmentActivityCode) {
	var AppraisalNo = $("#DocumentNo").val();
	var ans = confirm("Are you sure you want to delete this Line?");
	if (ans) {
		$.ajax({
			url: AJAXUrls.DeleteEmployeeAppraisalLine,
			type: "POST",
			dataType: "json",
			data: { DocumentNo: DocumentNo, ActivityCode: ActivityCode, AppraisalPeriod: AppraisalPeriod, AppraisalObjective: AppraisalObjective, OrganizationActivityCode: OrganizationActivityCode, DepartmentActivityCode: DepartmentActivityCode },
			cache: false,
			success: function (result) {
				LoadEmployeeAppraisalLines(AppraisalNo);
			},
			error: function (errormessage) {
				alert("Error");
			}
		});
	}
}

//Clear Employee Appraisal appraisal line modal
function ClearEmployeeAppraisalLineModal() {
	$("#AppraisalPeriod").val("").trigger("change");
	$("#AppraisalObjective").val("").trigger("change.select2");
	$("#OrganizationActivityCode").val("").trigger("change");
	$("#DepartmentActivityCode").val("").trigger("change");
	$("#BUM").val("").trigger("change");
	$("#TargetValue").val(0.00);
	$("#ActivityWeight").val(0.00);
	$("#ActivityDescription").val("");
	$("#AppraisalScoreType").val("");
	$("#Parametertype").val("");

	

	$("#CreateEmployeeAppraisalLineBtn").show();
	$("#ModifyEmployeeAppraisalLineBtn").hide();

	$("#ActivityCodeError").text("");
	$("#AppraisalPeriodError").text("");
	$("#AppraisalObjectiveError").text("");
	$("#OrganizationActivityCodeError").text("");
	$("#DepartmentActivityCodeError").text("");
	$("#BUMError").text("");
	$("#TargetValueError").text("");
	$("#ActivityWeightError").text("");
	$("#ModeratedValueError").text("");
	$("#ActivityDescriptionError").text("");
	$("#AppraisalScoreTypeError").text("");
	$("#ParametertypeError").text("");
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