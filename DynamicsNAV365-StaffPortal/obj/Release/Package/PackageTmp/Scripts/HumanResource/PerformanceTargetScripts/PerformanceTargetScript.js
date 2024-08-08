//Initialize performance target scripts
function InitializePerformanceTargetScript() {

	LoadGlobalAppraisalObjective();
	LoadJobPerformanceTargets();
	LoadBusinessCoreValuesAndCompetencies();

	var headerNo = $("#No").val();
	LoadJobPerformanceTargetLinesView(headerNo);
}

//Load global appraisal objectives
function LoadGlobalAppraisalObjective() {
	$.ajax({
		url: AJAXUrls.GetGlobalAppraisalPerspectives,
		type: "GET",
		dataType: "json",
	//	data: { HeaderNo: HeaderNo },
		cache: false,
		success: function (globalAppraisalPerspectives) {
			var rows = "";
			$.each(globalAppraisalPerspectives, function (i, globalAppraisalPerspective) {
				rows += "<tr>";
				rows += "<td>" + globalAppraisalPerspective.PerspectiveType + "</td>";
				rows += "<td>" + globalAppraisalPerspective.Code + "</td>";
				rows += "<td>" + globalAppraisalPerspective.Description + "</td>";
				rows += "</tr>";

				$("#GlobalAppraisalObjectiveTbl tbody").html(rows);
			});

			$("#GlobalAppraisalObjectiveTbl").css("display", "block");
		}
	});
}

//Load business core values and competencies
function LoadBusinessCoreValuesAndCompetencies() {
	$.ajax({
		url: AJAXUrls.GetBusinessCoreValuesAndCompetencies,
		type: "GET",
		dataType: "json",
		//	data: { HeaderNo: HeaderNo },
		cache: false,
		success: function (businessCoreValuesAndCompetencies) {
			var rows = "";
			$.each(businessCoreValuesAndCompetencies, function (i, businessCoreValuesAndCompetency) {
				rows += "<tr>";
				rows += "<td>" + businessCoreValuesAndCompetency.Category + "</td>";
				/*rows += "<td>" + businessCoreValuesAndCompetency.Code + "</td>";*/
				rows += "<td>" + businessCoreValuesAndCompetency.Description + "</td>";
				rows += "<td>" + businessCoreValuesAndCompetency.Description + "</td>";
				rows += "</tr>";

				$("#BusinessCoreValuesAndCompetenciesTbl tbody").html(rows);
			});

			$("#BusinessCoreValuesAndCompetenciesTbl").css("display", "block");
		}
	});
}

//Load job performance targets
function LoadJobPerformanceTargets() {
	$.ajax({
		url: AJAXUrls.GetJobPerformanceTargets,
		type: "GET",
		dataType: "json",
		//data: { HeaderNo: HeaderNo },
		cache: false,
		success: function (jobPerformanceTargets) {
			var rows = "";
			$.each(jobPerformanceTargets, function (i, jobPerformanceTarget) {
				rows += "<tr>";
				rows += "<td>" + jobPerformanceTarget.AppraisalPeriod + "</td>";
				rows += "<td>" + jobPerformanceTarget.ObjectiveCode + "</td>";
				rows += "<td>" + jobPerformanceTarget.ObjectiveDescription + "</td>";
				rows += "<td>" + jobPerformanceTarget.PerspectiveType + "</td>";
				rows += "<td>" + jobPerformanceTarget.DepartmentName + "</td>";
				rows += "<td>" + jobPerformanceTarget.PerspectiveDescription + "</td>";
				rows += "</tr>";

				$("#JobPerformanceTargetTbl tbody").html(rows);
			});

			$("#JobPerformanceTargetTbl").css("display", "block");
		}
	});
}

//Load job performance target lines view
function LoadJobPerformanceTargetLinesView(HeaderNo) {
	$.ajax({
		url: AJAXUrls.GetEmployeePerformanceTargetLines,
		type: "GET",
		dataType: "json",
		data: { HeaderNo: HeaderNo },
		cache: false,
		success: function (employeePerformanceTargetLines) {
			var rows = "";
			$.each(employeePerformanceTargetLines, function (i, employeePerformanceTargetLine) {
				rows += "<tr>";
				rows += "<td>" + employeePerformanceTargetLine.AgreedPerformanceTargets + "</td>";
				rows += "<td>" + employeePerformanceTargetLine.KeyPerformanceIndicator + "</td>";
				rows += "<td>" + employeePerformanceTargetLine.KeyResultAreasOutput + "</td>";
				rows += "</tr>";

				$("#EmployeePerformanceTargetLineTbl tbody").html(rows);
			});

			$("#EmployeePerformanceTargetLineTbl").css("display", "block");
		}
	});
}

//Load target lines
function LoadEmployeePerformanceTargetLines(HeaderNo) {  
	$.ajax({
		url: AJAXUrls.GetEmployeePerformanceTargetLines,
		type: "GET",
		dataType: "json",
		data: { HeaderNo: HeaderNo },
		cache: false,
		success: function (employeePerformanceTargetLines) {
			var rows = "";
			$.each(employeePerformanceTargetLines, function (i, employeePerformanceTargetLine) {
				rows += "<tr>";
				rows += "<td>" + employeePerformanceTargetLine.AgreedPerformanceTargets + "</td>";
				rows += "<td>" + employeePerformanceTargetLine.KeyPerformanceIndicator + "</td>";
				rows += '<td><a href="#" onclick="return EditEmployeePerformanceTargetLine(' + employeePerformanceTargetLine.LineNo + ',\'' + employeePerformanceTargetLine.AppraisalNo + '\');">Edit</a> | <a href="#" onclick="DeleteEmployeePerformanceTargetLine(' + employeePerformanceTargetLine.LineNo + ',\'' + employeePerformanceTargetLine.AppraisalNo + '\')">Delete</a></td>';
				rows += "</tr>";

				$("#EmployeePerformanceTargetLineTbl tbody").html(rows);
			});

			$("#EmployeePerformanceTargetLineTbl").css("display", "block");
		}
	});
}

//Edit line
function EditEmployeePerformanceTargetLine(LineNo, HeaderNo) {
	$.ajax({
		url: AJAXUrls.GetEmployeePerformanceTargetByLineNo,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, HeaderNo: HeaderNo },
		cache: false,
		success: function (result) {
			$('#LineNo').val(result.LineNo);
			$('#AppraisalNo').val(result.AppraisalNo);
			$('#AgreedPerformanceTargets').val(result.AgreedPerformanceTargets);
			$('#KeyPerformanceIndicator').val(result.KeyPerformanceIndicator);
			$('#KeyResultAreasOutput').val(result.KeyResultAreasOutput);

			$("#EmployeePerformanceTargetLineModal").modal("show");
			$("#CreateTargetLineBtn").hide();
			$("#ModifyTargetLineBtn").show();

		}
	});
	return false;
}

//Load target lines view
function LoadEmployeePerformanceTargetLinesView(HeaderNo) {
	$.ajax({
		url: AJAXUrls.GetEmployeePerformanceTargetLines,
		type: "GET",
		dataType: "json",
		data: { HeaderNo: HeaderNo },
		cache: false,
		success: function (employeePerformanceTargetLines) {
			var rows = "";
			$.each(employeePerformanceTargetLines, function (i, employeePerformanceTargetLine) {
				rows += "<tr>";
				rows += "<td>" + employeePerformanceTargetLine.AgreedPerformanceTargets + "</td>";
				rows += "<td>" + employeePerformanceTargetLine.KeyPerformanceIndicator + "</td>";
				rows += '<td><a href="#" onclick="return EditEmployeePerformanceTargetLine(' + employeePerformanceTargetLine.LineNo + ',\'' + employeePerformanceTargetLine.AppraisalNo + '\');">Edit</a> | <a href="#" onclick="DeleteEmployeePerformanceTargetLine(' + employeePerformanceTargetLine.LineNo + ',\'' + employeePerformanceTargetLine.AppraisalNo + '\')">Delete</a></td>';
				rows += "</tr>";

				$("#EmployeePerformanceTargetLineTbl tbody").html(rows);
			});

			$("#EmployeePerformanceTargetLineTbl").css("display", "block");
		}
	});
}
 
//Clear modal
function ClearEmployeePerformanceTargetLineModal() {
	$("#LineNo").val(0);
	$("#AppraisalNo").val("");
	$("#AgreedPerformanceTargets").val("");
	$("#KeyPerformanceIndicator").val("");
	$("#KeyResultAreasOutput").val("");

	$("#CreateTargetLineBtn").show();
	$("#ModifyTargetLineBtn").hide();

	$("#AgreedPerformanceTargetsError").text("");
	$("#KeyPerformanceIndicatorError").text("");
	$("#KeyResultAreasOutputError").text("");
}

//Insert
function CreateEmployeePerformanceTargetLine() {

	var documentNo = $("#No").val();

	var validLine = ValidateEmployeePerformanceTargetLine();
	if (validLine == false) {
		return false;
	}

	var employeePerformanceTargetLineObj = {
		AppraisalNo: documentNo,
		AgreedPerformanceTargets: $("#AgreedPerformanceTargets").val(),
		KeyPerformanceIndicator: $("#KeyPerformanceIndicator").val(),
		KeyResultAreasOutput: $("#KeyResultAreasOutput").val()
	};

	$.ajax({
		url: AJAXUrls.CreateEmployeePerformanceTargetLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(employeePerformanceTargetLineObj),
		cache: false,
		success: function (result) {
			LoadEmployeePerformanceTargetLines(documentNo);
			$("#EmployeePerformanceTargetLineModal").modal("hide");
		}
	});
}

//Update
function ModifyEmployeePerformanceTargetLine() {

	var documentNo = $("#No").val();

	var validLine = ValidateEmployeePerformanceTargetLine();
	if (validLine == false) {
		return false;
	}

	var employeePerformanceTargetLineObj = {
		LineNo: $("#LineNo").val(),
		AppraisalNo: documentNo,
		AgreedPerformanceTargets: $("#AgreedPerformanceTargets").val(),
		KeyPerformanceIndicator: $("#KeyPerformanceIndicator").val(),
		KeyResultAreasOutput: $("#KeyResultAreasOutput").val()
	};

	$.ajax({
		url: AJAXUrls.ModifyEmployeePerformanceTargetLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(employeePerformanceTargetLineObj),
		cache: false,
		success: function (result) {
			LoadEmployeePerformanceTargetLines(documentNo);
			$("#EmployeePerformanceTargetLineModal").modal("hide");
		}
	});
}

//Validate
function ValidateEmployeePerformanceTargetLine() {
	var isValid = true;
	var label = "";

	if ($("#AgreedPerformanceTargets").val().trim() == "") {
		$("#AgreedPerformanceTargetsError").text("Agreed performance target line cannot be empty.");
		isValid = false;
	}
	else {
		$("#AgreedPerformanceTargetsError").text("");
	}

	if ($("#KeyPerformanceIndicator").val().trim() == "") {
		$("#KeyPerformanceIndicatorError").text("Key Performance Indicator line cannot be empty.");
		isValid = false;
	}
	else {
		$("#KeyPerformanceIndicatorError").text("");
	}

	return isValid;
}

//Delete 
function DeleteEmployeePerformanceTargetLine(LineNo, HeaderNo) {
	var ans = confirm("Are you sure you want to delete this Line?");
	if (ans) {
		$.ajax({
			url: AJAXUrls.DeleteEmployeePerformanceTargetLine,
			type: "POST",
			dataType: "json",
			data: { LineNo: LineNo, HeaderNo: HeaderNo },
			cache: false,
			success: function (result) {
				LoadEmployeePerformanceTargetLines(HeaderNo);
			}
		});
	}
}


