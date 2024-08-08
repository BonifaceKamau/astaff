//Load lines
function LoadModeratedAppraisalLines(HeaderNo) {
	$.ajax({
		url: AJAXUrls.GetModeratedAppraisalLines,
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
				rows += '<td><a href="#" onclick="return EditModeratedAppraisalLine(' + employeePerformanceTargetLine.LineNo + ',\'' + employeePerformanceTargetLine.AppraisalNo + '\');">Edit</a>';
				rows += "</tr>";

				$("#ModeratedAppraisalLineTbl tbody").html(rows);
			});

			$("#ModeratedAppraisalLineTbl").css("display", "block");
		}
	});
}

//Edit line
function EditModeratedAppraisalLine(LineNo, HeaderNo) {
	$.ajax({
		url: AJAXUrls.GetModeratedAppraisalByLineNo,
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

			$("#ModeratedAppraisalLineModal").modal("show");
			$("#CreateModeratedAppraisalLineBtn").hide();
			$("#ModifyModeratedAppraisalLineBtn").show();

		}
	});
	return false;
}

//Update
function ModifyModeratedAppraisalLine() {

	var documentNo = $("#No").val();

	var validLine = ValidateModeratedAppraisalLine();
	if (validLine == false) {
		return false;
	}

	var employeePerformanceTargetLineObj = {
		LineNo: $("#LineNo").val(),
		AppraisalNo: documentNo,
		AgreedAssesmentResults: $("#AgreedAssesmentResults").val(),
		CommentsAfterReview: $("#CommentsAfterReview").val()
	};

	$.ajax({
		url: AJAXUrls.ModifyModeratedAppraisalLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(employeePerformanceTargetLineObj),
		cache: false,
		success: function (result) {
			LoadModeratedAppraisalLines(documentNo);
			$("#ModeratedAppraisalLineModal").modal("hide");
		}
	});
}

//Validate
function ValidateModeratedAppraisalLine() {
	var isValid = true;
	var label = "";

	if ($.isNumeric($("#AgreedAssesmentResults").val())) {
		$("#AgreedAssesmentResultsError").text("");
	} else {
		("#AgreedAssesmentResultsError").text("Agreed assesment results must be numeric.");
		isValid = false;
	}

	if (($("#AgreedAssesmentResults").val() <= 0) || ($("#AgreedAssesmentResults").val().trim() == "")) {
		$("#AgreedAssesmentResults").text("Agreed assesment results cannot be less or equal to zero.");
		isValid = false;
	}
	else {
		$("#AgreedAssesmentResultsError").text("");
	}

	if ($("#CommentsAfterReview").val().trim() == "") {
		$("#CommentsAfterReviewError").text("Please give comments based on what you have discussed.");
		isValid = false;
	}
	else {
		$("#CommentsAfterReviewError").text("");
	}

	return isValid;
}

//Clear modal
function ClearModeratedAppraisalLineModal() {

	$("#LineNo").val(0);
	$("#AppraisalNo").val("");
	$("#AgreedAssesmentResults").val("");
	$("#CommentsAfterReview").val("");

	$("#CreateModeratedAppraisalLineBtn").show();
	$("#ModifyModeratedAppraisalLineBtn").hide();

	$("#CommentsAfterReviewError").text("");
	$("#AgreedAssesmentResultsError").text("");
}