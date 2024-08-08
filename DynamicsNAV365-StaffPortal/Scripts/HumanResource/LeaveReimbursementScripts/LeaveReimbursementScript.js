//Initialize Leave Reimbursement Scripts
function InitializeLeaveReimbursmentScripts() {
	var dateToday = new Date(0);
	$("#ActualReturnDate").datepicker({ //EvaluationStartDate
		dateFormat: "dd/mm/yy",
		changeMonth: true,
		changeYear: true,
		minDate: dateToday
	});

	//On change events
	OnChangeEvents();

	AddLeaveReimbursementDropDownListSearch();
}

//On Change Events
function OnChangeEvents() {
	$("#ApprovedLeaveApplication").change(function () {
		LoadApprovedLeave($(this).val());
	});
}

//Drop down list
function AddLeaveReimbursementDropDownListSearch() {
	$("#ApprovedLeaveApplication").select2({
		placeholder: $("#ApprovedLeaveApplicationLbl").text(),
		allowClear: true
	});
}

//Load Leave Application Details
function LoadApprovedLeave(ApprovedLeaveApplication) {
	$.ajax({
		url: AJAXUrls.GetLeaveApplicationDetails,
		type: "GET",
		dataType: "json",
		data: { ApprovedLeaveApplication: ApprovedLeaveApplication },
		cache: false,
		success: function (result) {
			$("#LeaveType").val(result.LeaveType);
			$("#LeaveType").css("background-color", "LightGray");
			$("#LeavePeriod").val(result.LeavePeriod);
			$("#LeavePeriod").css("background-color", "LightGray");
			$("#LeaveStartDate").val(result.LeaveStartDate);
			$("#LeaveStartDate").css("background-color", "LightGray");
			$("#DaysApplied").val(result.DaysApplied);
			$("#DaysApplied").css("background-color", "LightGray");
			$("#DaysApproved").val(result.DaysApproved);
			$("#DaysApproved").css("background-color", "LightGray");
			$("#LeaveEndDate").val(result.LeaveEndDate);
			$("#LeaveEndDate").css("background-color", "LightGray");
			$("#LeaveReturnDate").val(result.LeaveReturnDate);
			$("#LeaveReturnDate").css("background-color", "LightGray");
			$("#SubstituteNo").val(result.SubstituteNo);
			$("#SubstituteNo").css("background-color", "LightGray");
			$("#SubstituteName").val(result.SubstituteName);
			$("#SubstituteName").css("background-color", "LightGray");
			$("#ReasonForLeave").val(result.ReasonForLeave);
			$("#ReasonForLeave").css("background-color", "LightGray");
		},
		error: OnError
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