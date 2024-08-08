//Load lines
function LoadCompleteAppraisalLines(HeaderNo) {
	$.ajax({
		url: AJAXUrls.GetCompleteAppraisalLines,
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
				rows += "<td>" + employeePerformanceTargetLine.SelfAssessment + "</td>";
				rows += "<td>" + employeePerformanceTargetLine.AppraiseeComments + "</td>";
				rows += "<td>" + employeePerformanceTargetLine.SupervisorAssessment + "</td>";
				rows += "<td>" + employeePerformanceTargetLine.SupervisorComments + "</td>";
				//rows += '<td><a href="#" onclick="return EditAppraiserEvaluationLine(' + employeePerformanceTargetLine.LineNo + ',\'' + employeePerformanceTargetLine.AppraisalNo + '\');">Edit</a>';
				rows += "</tr>";

				$("#CompleteAppraisalLineTbl tbody").html(rows);
			});

			$("#CompleteAppraisalLineTbl").css("display", "block");
		}
	});
}

//Edit line
function EditAppraiserEvaluationLine(LineNo, HeaderNo) {
	$.ajax({
		url: AJAXUrls.GetAppraiseeEvaluationByLineNo,
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
			$('#SelfScore').val(result.SelfScore);
			$('#AppraiseeComments').val(result.AppraiseeComments);
			$('#SupervisorsScore').val(result.SupervisorsScore);
			$('#SupervisorComments').val(result.SupervisorComments);

			$("#AppraiserEvaluationLineModal").modal("show");
			$("#CreateAppraiserEvaluationLineBtn").hide();
			$("#ModifyAppraiserEvaluationLineBtn").show();

		}
	});
	return false;
}

//Update
function ModifyAppraiserEvaluationLine() {

	var documentNo = $("#No").val();

	var validLine = ValidateAppraiserEvaluationLine();
	if (validLine == false) {
		return false;
	}

	var employeePerformanceTargetLineObj = {
		LineNo: $("#LineNo").val(),
		AppraisalNo: documentNo,
		SupervisorsScore: $("#SupervisorsScore").val(),
		SupervisorComments: $("#SupervisorComments").val()
	};

	$.ajax({
		url: AJAXUrls.ModifyAppraiserEvaluationLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(employeePerformanceTargetLineObj),
		cache: false,
		success: function (result) {
			LoadAppraiserEvaluationLines(documentNo);
			$("#AppraiserEvaluationLineModal").modal("hide");
		}
	});
}

//Validate
function ValidateAppraiserEvaluationLine() {
	var isValid = true;
	var label = "";

	if ($.isNumeric($("#SupervisorsScore").val())) {
		$("#SupervisorsScoreError").text("");
	} else {
		("#SupervisorsScoreError").text("Supervisors Score must be numeric.");
		isValid = false;
	}

	if (($("#SupervisorsScore").val() <= 0) || ($("#SupervisorsScore").val().trim() == "")) {
		$("#SupervisorsScoreError").text("Supervisors  Score cannot be less or equal to zero.");
		isValid = false;
	}
	else {
		$("#SupervisorsScoreError").text("");
	}

	if ($("#SupervisorComments").val().trim() == "") {
		$("#SupervisorCommentsError").text("Supervisor Comments cannot be empty.");
		isValid = false;
	}
	else {
		$("#SupervisorCommentsError").text("");
	}

	return isValid;
}

//Clear modal
function ClearAppraiserEvaluationLineModal() {

	$("#LineNo").val(0);
	$("#AppraisalNo").val("");
	$("#SupervisorsScore").val("");
	$("#SupervisorComments").val("");

	$("#CreateAppraiserEvaluationLineBtn").show();
	$("#ModifyAppraiserEvaluationLineBtn").hide();

	$("#SupervisorCommentsError").text("");
	$("#SupervisorsScoreError").text("");
}


