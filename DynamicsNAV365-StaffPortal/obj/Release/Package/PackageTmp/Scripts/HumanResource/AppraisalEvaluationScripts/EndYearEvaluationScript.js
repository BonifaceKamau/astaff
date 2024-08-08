//Load lines
function LoadEndYearEvaluationLines(HeaderNo) {
	$.ajax({
		url: AJAXUrls.GetEndYearEvaluationLines,
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
				rows += "<td>" + employeePerformanceTargetLine.AgreedAssesmentResults + "</td>";
				rows += "<td>" + employeePerformanceTargetLine.CommentsAfterReview + "</td>";
				rows += "<td>" + employeePerformanceTargetLine.EndYearAssessment + "</td>";
				rows += "<td>" + employeePerformanceTargetLine.EndYearEvaluationComments + "</td>";
				rows += '<td><a href="#" onclick="return EditEndYearEvaluationLine(' + employeePerformanceTargetLine.LineNo + ',\'' + employeePerformanceTargetLine.AppraisalNo + '\');">Edit</a>';
				rows += "</tr>";

				$("#EndYearEvaluationLineTbl tbody").html(rows);
			});

			$("#EndYearEvaluationLineTbl").css("display", "block");
		}
	});
}

//Edit line
function EditEndYearEvaluationLine(LineNo, HeaderNo) {
	$.ajax({
		url: AJAXUrls.GetEndYearEvaluationByLineNo,
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
			$('#SelfAssessment').val(result.SelfAssessment);
			$('#AppraiseeComments').val(result.AppraiseeComments);
			$('#SupervisorsScore').val(result.SupervisorsScore);
			$('#SupervisorAssessment').val(result.SupervisorAssessment);
			$('#SupervisorComments').val(result.SupervisorComments);
			$('#AgreedAssesmentResults').val(result.AgreedAssesmentResults);
			$('#CommentsAfterReview').val(result.CommentsAfterReview);
			$('#EndYearAssessment').val(result.EndYearAssessment);
			$('#EndYearEvaluationComments').val(result.EndYearEvaluationComments);

			$("#EndYearEvaluationLineModal").modal("show");
			$("#CreateEndYearEvaluationLineBtn").hide();
			$("#ModifyEndYearEvaluationLineBtn").show();

		}
	});
	return false;
}

//Update
function ModifyEndYearEvaluationLine() {

	var documentNo = $("#No").val();

	var validLine = ValidateEndYearEvaluationLine();
	if (validLine == false) {
		return false;
	}

	var employeePerformanceTargetLineObj = {
		LineNo: $("#LineNo").val(),
		AppraisalNo: documentNo,
		EndYearAssessment: $("#EndYearAssessment").val(),
		EndYearEvaluationComments: $("#EndYearEvaluationComments").val()

	};

	$.ajax({
		url: AJAXUrls.ModifyEndYearEvaluationLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(employeePerformanceTargetLineObj),
		cache: false,
		success: function (result) {
			LoadEndYearEvaluationLines(documentNo);
			$("#EndYearEvaluationLineModal").modal("hide");
		}
	});
}

//Validate
function ValidateEndYearEvaluationLine() {
	var isValid = true;
	var label = "";

	if ($.isNumeric($("#EndYearAssessment").val())) {
		$("#EndYearAssessmentError").text("");
	} else {
		("#EndYearAssessmentError").text("End year assessment must be numeric.");
		isValid = false;
	}

	if (($("#EndYearAssessment").val() <= 0) || ($("#EndYearAssessment").val().trim() == "")) {
		$("#EndYearAssessment").text("End year assessment cannot be less or equal to zero.");
		isValid = false;
	}
	else {
		$("#EndYearAssessmentError").text("");
	}

	if ($("#EndYearEvaluationComments").val().trim() == "") {
		$("#EndYearEvaluationCommentsError").text("Comments required!");
		isValid = false;
	}
	else {
		$("#EndYearEvaluationCommentsError").text("");
	}

	return isValid;
}

//Clear modal
function ClearEndYearEvaluationLineModal() {

	$("#LineNo").val(0);
	$("#AppraisalNo").val("");
	$("#EndYearAssessment").val("");
	$("#EndYearEvaluationComments").val("");

	$("#CreateEndYearEvaluationLineBtn").show();
	$("#ModifyEndYearEvaluationLineBtn").hide();

	$("#EndYearAssessmentError").text("");
	$("#EndYearEvaluationCommentsError").text("");
}