function InitializeLeaveApplication() {
    var today = new Date();
    var minDate = new Date(today);
    minDate.setDate(today.getDate() + 2);

    $("#LeaveStartDate").datepicker({ 
        dateFormat: "dd-mm-y",
        changeMonth: true,
        changeYear: true,
        minDate: minDate,
        onSelect: function(dateText) {
            validateSelectedDate(dateText);
        }
    });

    AddLeaveApplicationDropDownListSearch();
    AddOnChangeEvents();
}

function AddLeaveApplicationDropDownListSearch() {
    $("#SubstituteEmployeeNo, #LeaveType, #duration").each(function() {
        $(this).select2({
            placeholder: $(`#${this.id}Lbl`).text(),
            allowClear: true
        });
    });
}

function validateSelectedDate(dateText) {
    var selectedDateStr = dateText;
    var availableleavetyp = $("#LeaveType").val();
    
    var today = new Date();
    today.setHours(0, 0, 0, 0);
    
    var minDate = new Date(today);
    minDate.setDate(today.getDate() + (availableleavetyp === 'SICK' ? 1 : 2));
    
    var selectedDateParts = selectedDateStr.split('-');
    var selectedDateObj = new Date(20 + selectedDateParts[2], selectedDateParts[1] - 1, selectedDateParts[0]);
    selectedDateObj.setHours(0, 0, 0, 0);

    if (selectedDateObj >= minDate) {
        $("#LeaveStartDateError").text("");
        updateDates();
    } else {
        var errorMessage = availableleavetyp === 'SICK' 
            ? "Please select a date that is at least one day from today for sick leave." 
            : "Please select a date that is at least two days from today.";
        $("#LeaveStartDateError").text(errorMessage);
        $("#LeaveStartDate").datepicker("setDate", minDate);
    }
}

function AddOnChangeEvents() {
    $("#LeaveStartDate").change(function() {
        validateSelectedDate($(this).val());
    });

    $("#DaysApplied").blur(function() {
        var daysApplied = parseInt($(this).val());
        var leaveBalance = parseInt($("#LeaveBalance").val());
        var leaveType = $("#LeaveType").val();

        if (leaveType === "") {
            $("#DaysAppliedError").text("Kindly, select leave type first");
        } else if (leaveType === "ANNUAL" && daysApplied > leaveBalance) {
            $("#DaysAppliedError").text(`Your current leave balance is ${leaveBalance}. The applied leave days requested cannot be more than leave balance.`);
        } else {
            $("#DaysAppliedError").text("");
            updateDates();
        }
    });

    $("#LeaveType").change(function() {
        validateSelectedDate($("#LeaveStartDate").val());
    });
}

function selectedformat(date) {
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var year = date.getFullYear();
    return `${day < 10 ? '0' + day : day}-${month < 10 ? '0' + month : month}-${year.toString().substr(-2)}`;
}

function ValidateLeaveApplication() {
    var isValid = true;
    var daysApplied = parseInt($("#DaysApplied").val());
    var leaveType = $("#LeaveType").val();

    if (daysApplied <= 0) {
        $("#DaysAppliedError").text("Applied Days cannot be less than or equal to zero.");
        isValid = false;
    } else {
        $("#DaysAppliedError").text("");
    }

    return isValid;
}

// Assuming this function exists elsewhere in your code
function updateDates() {
    // Implementation of updateDates
}










//insert leave application document
function InsertLeaveApplicationDocuments(LeaveType) { 
	var LeaveApplicationNo = $("#No").val();
	$.ajax({
		url: AJAXUrls.InsertLeaveApplicationDocuments,
		type: "GET",
		dataType: "json",
		data: { LeaveApplicationNo: LeaveApplicationNo, LeaveType: LeaveType },
		cache: false,
		success: function (result) {
			LoadLeaveApplicationDocuments(LeaveApplicationNo);
		},
		error: function (errormessage) {
			//alert(" " + EmployeeLoan + "  exists for employee no. " + employeeNo + ", finalize on this loan application before creating a new one.");
		}
	});
}

//get employee splitted leave balances
function LoadEmployeeSplittedLeaveBalanaces(LeaveType) {
	$.ajax({
		url: AJAXUrls.getEmployeeLeaveSplittedBalance,
		type: "GET",
		dataType: "json",
		data: { LeaveType: LeaveType },
		cache: false,
		success: function (employeeSplittedLeaves) {
			var rows = "";
			$.each(employeeSplittedLeaves, function (i, employeeSplittedLeave) {
				rows += "<tr>";
				rows += "<td>" + employeeSplittedLeave.DaysEntitlement + "</td>";
				rows += "<td>" + employeeSplittedLeave.DaysEarned + "</td>";
				rows += "<td>" + employeeSplittedLeave.DaysTakenToDate + "</td>";
				rows += "<td>" + employeeSplittedLeave.AnnualLeaveDaysCarriedForward + "</td>";
				rows += "<td>" + employeeSplittedLeave.DaysAvailable + "</td>";
				rows += "<td>" + employeeSplittedLeave.CurrentLeavePeriod + "</td>";
				rows += "</tr>";

				$("#EmployeeLeaveBalanceTbl tbody").html(rows);
			});

			$("#EmployeeLeaveBalanceTbl").css("display", "block");

		}
	});
}

function updateApprovedDates() {

    var leaveApprovedNo = $("#No").val();
    var EmployeeApprovedNo = $("#EmployeeNo").val();
    var leaveApprovedType = $("#LeaveType").val();
    var leaveApprovedStartDateStr = $("#LeaveStartDate").val();
    const DaysApproved = parseInt($("#DaysApproved").val());

    var leaveApprovedStartDate = selectedformat(new Date(leaveApprovedStartDateStr));

  //  console.log('Date fix:' + leaveApprovedStartDate);

    // AJAX request to update end date
    $.ajax({
        url: AJAXUrls.GetApprovedLeaveEndDate,
        type: 'GET',
        data: {

            EmployeeNo: EmployeeApprovedNo,
            LeaveType: leaveApprovedType,
            LeaveStartDate: leaveApprovedStartDate,
            DaysApplied: DaysApproved
        }, 
        success: function (endDate) {
            $("#LeaveEndDate").val(endDate);
        },
        error: function (xhr, status, error) {
            console.error('Error updating end date:', error);
        }
    });

    // AJAX request to update return date
    $.ajax({
        url: AJAXUrls.GetApprovedLeaveReturnDate,
        type: 'GET',
        data: {
            leaveNo: leaveApprovedNo,
            DaysApplied: DaysApproved,
            LeaveStartDate: leaveApprovedStartDate
        },
        success: function (returnDate) {
            $("#LeaveReturnDate").val(returnDate);
        },
        error: function (xhr, status, error) {
            console.error('Error updating return date:', error);
        }

    });
}
function updateDates() {

    var leaveNo = $("#No").val();
    var EmployeeNo = $("#EmployeeNo").val();
    var leaveType = $("#LeaveType").val();
    var leaveStartDate = $("#LeaveStartDate").val();
    var daysApplied = $("#DaysApplied").val();

    // AJAX request to update end date
    $.ajax({
        url: AJAXUrls.GetLeaveEndDate,
        type: 'GET',
        data: {

            EmployeeNo: EmployeeNo,
            LeaveType: leaveType,
            LeaveStartDate: leaveStartDate,
            DaysApplied: daysApplied
        },
        success: function (endDate) {
            $("#LeaveEndDate").val(endDate);
        },
        error: function (xhr, status, error) {
            console.error('Error updating end date:', error);
        }
    });

    // AJAX request to update return date
    $.ajax({
        url: AJAXUrls.GetLeaveReturnDate,
        type: 'GET',
        data: {
            leaveNo: leaveNo,
            DaysApplied: daysApplied,
            LeaveStartDate: leaveStartDate
        },
        success: function (returnDate) {
            $("#LeaveReturnDate").val(returnDate);
        },
        error: function (xhr, status, error) {
            console.error('Error updating return date:', error);
        }

    });
}


const updateType = () => {

    var LeaveAppNo = $("#No").val();
    var LeaveType = $("#LeaveType").val();

     //AJAX request to update end date
    $.ajax({
        url: '/LeaveApplication/GetStartDate',
        type: 'GET',
        data: {
            LeaveAppNo: LeaveAppNo,
            LeaveType: LeaveType,
        },
        success: function (StartDate) {
            $("#LeaveStartDate").val(StartDate);
        },
        error: function ( error) {
            console.error('Error updating start date:', error);
        }
    });

    // AJAX request to update return date
    $.ajax({
        url: '/LeaveApplication/GetDaysApplied' ,
        type: 'GET',
        data: {
            LeaveAppNo: LeaveAppNo,
            LeaveType: LeaveType,
        },
        success: function (Applied) {
            $("#DaysApplied").val(Applied);
        },
        error: function (xhr, status, error) {
            console.error('Error updating return date:', error);
        }

    });

};



async function checkAndSendAction(editUrl)
{
    const no = $('#No').val();
    var selectedSubstituteEmployeeNo = $('#SubstituteEmployeeNo').val();
    var reasonForLeave = $('#ReasonForLeave').val();
    const Leavedays = parseInt($("#DaysApplied").val());


    if ((Leavedays == 0) || (reasonForLeave == '') || (selectedSubstituteEmployeeNo == '')) {
        Swal.fire('There is a problem to process your request, either handover notes is null or you have 0 applied days! ');
    }
    else
    {
        console.log('apply seven: ' + no);

        try {
            const response = await fetch(editUrl, {
                method: "POST",
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ no: no})
            })

            if (!response.ok) {
                throw new Error('Network response was not okay' + response.statusText)
            }
          
            const responseData = await response.json()

            console.log("Success", responseData)
           // location.reload();

        } catch (error) {
            console.log("Error", error)
           // location.reload();
        }

    }

}


//Load Leave Application Document
function LoadLeaveApplicationDocuments(DocumentNo) {
    var DocNo = DocumentNo;
    var Status = $("#Status").val();
    $.ajax({
        async: true,
        type: "POST",
        datatype: "json",
        contentType: "application/json; charset = utf-8",
        processData: false,
        data: JSON.stringify({ DocNo: DocNo, TableID: 51525209, Status: Status }),
        url: "/PurchaseRequisition/DocumentAttachments",
        success: function (data) {
            $("#divAttachDocs").html(data);
        },
        error: function () {
            Swal.fire("There is some problem to process your request. Please try after some time");
        }
    });
}

var ViewAttachment = function (tbl, No, Id, fName, ext) {
    $.ajaxSetup({ cache: false });
    $.ajax({
        cache: false,
        url: '/PurchaseRequisition/DocumentAttachmentview',
        type: "POST",
        datatype: "json",
        cache: false,
        contentType: "application/json; charset = utf-8",
        processData: false,
        data: JSON.stringify({ tblID: tbl, No: No, ID: Id, fileName: fName, ext: ext }),
        success: function (data) {
            if (data.success == true) {
                if (data.view == true) {
                    var viewer = $("#modalAttachmentBody");
                    if (ext == "pdf") {
                        PDFObject.embed("data:application/" + ext + ";base64," + data.message + "", viewer);
                    }
                    if (ext == "png" || ext == "jpg") {
                        PDFObject.embed("data:image/" + ext + ";base64," + data.message + "", viewer);
                    }
                    $("#myModalAttachment").modal("show");
                }
                else {
                    window.location = '/PurchaseRequisition/AttachmentDownload?fileName=' + data.message;
                    Swal.fire('Success', 'Document Downloaded successfully', 'success');
                }
            }
            else {
                Swal.fire('Warning', data.message, 'warning');
            }
        },
        error: function (err) {
            Swal.fire('Warning', err, 'warning');
        }
    });
};
var DeleteAttachment = function (tbl, No, Id) {
    $.ajaxSetup({ cache: false });
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                cache: false,
                url: '/PurchaseRequisition/DeleteAttachedDocument',
                datatype: "json",
                type: "POST",
                data: JSON.stringify({ DocNo: No, DocID: Id, tblID: tbl }),
                contentType: "application/json; charset = utf-8",
                success: function (data) {
                    if (data.success == true) {
                        LoadLeaveApplicationDocuments(No);
                        Swal.fire('Success', 'File Deleted Successfully', 'success');
                    }
                    else {
                        Swal.fire('Warning', data.message, 'warning');
                    }
                },
                error: function (err) {
                    Swal.fire('Warning', err, 'warning');
                }
            });
        }
        else {
            Swal.fire('Cancelled', 'Attachment File has not been deleted', 'error');
        }
    });
}

function AddAttachment() {
    $("#ApplicationDocumentModal").modal("show");
}


//Load Leave Application Document View
function LoadLeaveApplicationDocumentsView(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetLeaveApplicationDocuments,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (results) {
			var rows = "";
			$.each(results, function (i, result) {
				rows += "<tr>";
				rows += "<td>" + result.DocumentDescription + "</td>";
				if (result.DocumentAttached) {
					rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' checked disabled>" + "</td>";
				} else {
					rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' disabled>" + "</td>";
				}
				rows += "</tr>";
			});
			$("#ApplicationDocumentsTbl tbody").html(rows);

			$("#AjaxLoader").css("display", "none");

			$("#ApplicationDocumentsTbl").css("display", "block");
		},
		error: function (xhr, status, error) {

		}
	});
}

//Edit Loan Application Document
function EditLeaveApplicationDocument(DocumentNo, DocumentCode) {

	//Clear link path
	ResetLeaveApplicationDocumentModal(); 

	$.ajax({
		url: AJAXUrls.GetLeaveApplicationDocumentByLineNo,
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
			$("#UploadLeaveApplicationDocumentBtn").show();
		},
		error: function (xhr, status, error) {

		}
	});
	return false;
}

//Upload Leave Application Document
function UploadLeaveApplicationDocument() {

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
		url: AJAXUrls.UploadLeaveApplicationDocument,
		type: "POST",
		data: frmData,
		dataType: 'json',
		contentType: false,
		processData: false,
		enctype: "multipart/form-data",
		async: true,
		cache: false,
		success: function (result) {
		//	$('#txtMessage').html(result.message);
			if (result.success) {
				$('#ApplicationDocumentModal').modal('hide');
				$('#errorMessage').hide();
				LoadLeaveApplicationDocuments(DocumentNo);
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

//Reset Leave Application Document Path
function ResetLeaveApplicationDocumentModal() {
	$("#ApplicationDocumentFile").val("");
	//Ladda.stopAll();
}