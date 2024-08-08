//Load appraisee evaluation lines
function LoadAppraiseeEvaluationLines(HeaderNo) {
	$.ajax({
		url: AJAXUrls.GetAppraiseeEvaluationLines, 
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
				rows += '<td><a href="#" onclick="return EditAppraiseeEvaluationLine(' + employeePerformanceTargetLine.LineNo + ',\'' + employeePerformanceTargetLine.AppraisalNo + '\');">Edit</a>';
				rows += "</tr>";

				$("#AppraiseeEvaluationLineTbl tbody").html(rows);
			});

			$("#AppraiseeEvaluationLineTbl").css("display", "block");
		}
	});
}

//Edit line
function EditAppraiseeEvaluationLine(LineNo, HeaderNo) {
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
			$('#SelfAssessment').val(result.SelfAssessment);
			$('#AppraiseeComments').val(result.AppraiseeComments);
			$('#SupervisorsScore').val(result.SupervisorsScore);
			$('#SupervisorAssessment').val(result.SupervisorAssessment);
			$('#SupervisorComments').val(result.SupervisorComments);
			$('#AgreedAssesmentResults').val(result.AgreedAssesmentResults);
			$('#CommentsAfterReview').val(result.CommentsAfterReview);
			$('#EndYearAssessment').val(result.EndYearAssessment);
			$('#EndYearEvaluationComments').val(result.EndYearEvaluationComments);

			$("#AppraiseeEvaluationLineModal").modal("show");
			$("#CreateAppraiseeEvaluationLineBtn").hide();
			$("#ModifyAppraiseeEvaluationLineBtn").show();

		}
	});
	return false;
}

//Update
function ModifyAppraiseeEvaluationLine() {

	var documentNo = $("#No").val();

	var validLine = ValidateAppraiseeEvaluationLine();
	if (validLine == false) {
		return false;
	}

	var employeePerformanceTargetLineObj = {
		LineNo: $("#LineNo").val(),
		AppraisalNo: documentNo,
		SelfAssessment: $("#SelfAssessment").val(),
		AppraiseeComments: $("#AppraiseeComments").val()
	};

	$.ajax({
		url: AJAXUrls.ModifyAppraiseeEvaluationLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(employeePerformanceTargetLineObj),
		cache: false,
		success: function (result) {
			LoadAppraiseeEvaluationLines(documentNo);
			$("#AppraiseeEvaluationLineModal").modal("hide");
		}
	});
}

//Validate
function ValidateAppraiseeEvaluationLine() {
	var isValid = true;
	var label = "";

	if ($.isNumeric($("#SelfAssessment").val())) {
		$("#SelfAssessmentError").text("");
	} else {
		("#SelfAssessmentError").text("Self Assessment must be numeric.");
		isValid = false;
	}

	if (($("#SelfAssessment").val() <= 0) || ($("#SelfAssessment").val().trim() == "")) {
		$("#SelfAssessmentError").text("Self Assessment cannot be less or equal to zero.");
		isValid = false;
	}
	else {
		$("#SelfAssessmentError").text("");
	}

	if ($("#AppraiseeComments").val().trim() == "") {
		$("#AppraiseeCommentsError").text("Appraisee Comments cannot be empty."); 
		isValid = false;
	}
	else {
		$("#AppraiseeCommentsError").text("");
	}

	return isValid;
}

//Clear modal
function ClearAppraiseeEvaluationLineModal() {
	$("#LineNo").val(0);
	$("#AppraisalNo").val("");
	$("#SelfAssessment").val("");
	$("#AppraiseeComments").val("");

	$("#CreateAppraiseeEvaluationLineBtn").show();
	$("#ModifyAppraiseeEvaluationLineBtn").hide();

	$("#AppraiseeCommentsError").text("");
	$("#SelfAssessmentError").text("");
}


