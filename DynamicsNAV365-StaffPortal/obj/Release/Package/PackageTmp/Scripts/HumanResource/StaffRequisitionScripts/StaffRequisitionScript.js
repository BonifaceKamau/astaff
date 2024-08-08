function InitializeStaffRequisitionScripts() {
	var dateToday = new Date(0);

	$("#ExpectedReportingDate").datepicker({
		dateFormat: "dd/mm/yy",
		changeMonth: true,
		changeYear: true,
		minDate: dateToday
	});

	$("#RequisitionType").select2({
		placeholder: $("#RequisitionTypeLbl").text(),
		allowClear: true
	});

	$("#AppointmentType").select2({
		placeholder: $("#AppointmentTypeLbl").text(),
		allowClear: true
	});

	$("#PositionType").select2({
		placeholder: $("#PositionTypeLbl").text(),
		allowClear: true
	});

	$("#JobNo").select2({
		placeholder: $("#JobNoLbl").text(),
		allowClear: true
	});
}

//Load JD Documents
function LoadJobDescriptionDocuments(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetJobDescriptionDocuments,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (jdDocuments) {
			var rows = "";
			$.each(jdDocuments, function (i, jdDocument) {
				rows += "<tr>";
				rows += "<td>" + jdDocument.DocumentDescription + "</td>";
				rows += '<td><a href="#" onclick="return EditJobDescriptionDocument(\'' + jdDocument.LineNo + '\',\'' + DocumentNo + '\',\'' + jdDocument.DocumentCode + '\');"><i class="fa fa-paperclip" aria-hidden="true">Attach Detailed JD</i></a></td>';
				rows += "</tr>";
			});

			$("#JobDescriptionAjaxLoader").css("display", "none");
			$("#JobDescriptionDocumentTbl tbody").html(rows);
			$("#JobDescriptionDocumentTbl").css("display", "block");
		}
	});
}

//Load JD View
function LoadJobDescriptionDocumentsView(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetJobDescriptionDocuments,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (imprestUploadedDocuments) {
			var rows = "";
			$.each(imprestUploadedDocuments, function (i, imprestUploadedDocument) {
				rows += "<tr>";
				rows += "<td>" + imprestUploadedDocument.DocumentDescription + "</td>";
				rows += '<td><a href="#" onclick="return ViewImprestDocument(\'' + DocumentNo + '\',\'' + imprestUploadedDocument.DocumentCode + '\');"><i class="" aria-hidden="true">View</i></a></td>';
				rows += "</tr>";
			});

			$("#JobDescriptionAjaxLoader").css("display", "none");
			$("#JobDescriptionDocumentTbl tbody").html(rows);
			$("#JobDescriptionDocumentTbl").css("display", "block");
		}
	});
}

//Edit JD document
function EditJobDescriptionDocument(LineNo, DocumentNo, DocumentCode) {

	ResetJobDescriptionDocumentModal();

	$.ajax({
		url: AJAXUrls.GetJobDescriptionDocument,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo, DocumentCode: DocumentCode },
		cache: false,
		success: function (applicationDocument) {
			$("#DocumentNo").val(applicationDocument.DocumentNo);
			$("#DocumentCode").val(applicationDocument.DocumentCode);
			$("#DocumentDescription").val(applicationDocument.DocumentDescription);
			$('#errorMessage').hide();

			$("#JobDescriptionDocumentModal").modal("show");
			$("#UploadJDBtn").show();
		}
	});
	return false;
}

//Reset JD document Line
function ResetJobDescriptionDocumentModal() {
	$("#JobDescriptionDocumentModal").val("");
	//Ladda.stopAll();
}

//Upload JD
function UploadDetailedJobDescription() {
	var DocumentNo = $("#DocumentNo").val();
	var DocumentCode = $("#DocumentCode").val();
	var DocumentDescription = $("#DocumentDescription").val();

	var filebase = $("#JobDescriptionDocumentFile").get(0);
	var files = filebase.files;

	var form = $('ImprestDocumentForm')[0];
	var frmData = new FormData();

	frmData.append("DocumentNo", DocumentNo);
	frmData.append("DocumentCode", DocumentCode);
	frmData.append("DocumentDescription", DocumentDescription);

	frmData.append(files[0].name, files[0]);

	//Block UI
	$.blockUI();

	$.ajax({
		url: AJAXUrls.UploadDetailedJobDescription,
		type: "POST",
		data: frmData,
		dataType: 'json',
		contentType: false,
		processData: false,
		enctype: "multipart/form-data",
		async: true,
		cache: false,
		success: function (result) {
			//$('#txtMessage').html(result.message);
			if (result.success) {
				$('#JobDescriptionDocumentModal').modal('hide');
				$('#errorMessage').hide();
				LoadJobDescriptionDocuments(DocumentNo); 
				$.unblockUI();
			} else {
				$('#errorMessage').html(result.message);
				$('#errorMessage').show();
				$.unblockUI();
			}

			Ladda.stopAll();
		}

	});
}

//Academic Requirements
function CreateJobAcademicRequirement() {

    var academicRequirements = {
        DocumentNo: $("#No").val(),
        Code: $("#Code").val(),
        Description1: $("#Description1").val()
    };

    $.ajax({
        url: AJAXUrls.CreateJobAcademicQualification,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(academicRequirements),
        cache: false,
        success: function (result) {
            if (result.success) {
                $('#errorMessage').hide();
                $('#successMessage').show();
                LoadJobAcademicRequirements($("#No").val());
                $("#JobAcademicRequirementsModal").modal("hide");
            } else {
                $('#errorMessage').html(result.message);
                $('#errorMessage').show();
                $('#successMessage').hide();
            }
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}

function ModifyJobAcademicRequirement() {

    var academicRequirements = {
        LineNo: $("#LineNo").val(),
        DocumentNo: $("#No").val(),
        Code: $("#Code").val(),
        Description1: $("#Description1").val()
    };

    $.ajax({
        url: AJAXUrls.ModifyJobAcademicQualification,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(academicRequirements),
        cache: false,
        success: function (result) {
            if (result.success) {
                $('#errorMessage').hide();
                $('#successMessage').show();
                LoadJobAcademicRequirements($("#No").val());
                $("#JobAcademicRequirementsModal").modal("hide");
            } else {
                $('#errorMessage').html(result.message);
                $('#errorMessage').show();
                $('#successMessage').hide();
            }
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}

function ClearJobAcademicRequirementsModal() {
    $("#LineNo").val(0);
    $("#DocumentNo").val("");
    $("#Code").val("").trigger("change");
    $("#Description1").val("");

    $("#CreateJobAcademicBtn").show();
    $("#ModifyJobAcademicBtn").hide();

    $("#CodeError").text("");
    $("#Description1Error").text("");
}

function DeleteJobAcademicRequirement(LineNo, DocumentNo) {
    var ans = confirm("Are you sure you want to delete this Line?");
    if (ans) {
        $.ajax({
            url: AJAXUrls.DeleteJobAcademicQualification,
            type: "POST",
            dataType: "json",
            data: { LineNo: LineNo, DocumentNo: DocumentNo },
            cache: false,
            success: function (result) {
                LoadJobAcademicRequirements(DocumentNo);
            },
            error: function (errormessage) {
                //alert(errormessage.responseText);
                alert("Error");
            }
        });
    }
}

function EditJobAcademicRequirement(LineNo, DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetJobAcademicRequirements,
        type: "GET",
        dataType: "json",
        data: { LineNo: LineNo, DocumentNo: DocumentNo },
        cache: false,
        success: function (result) {
            $('#LineNo').val(result.LineNo);
            $('#DocumentNo').val(result.DocumentNo);
            $('#Code').val(result.Code).trigger("change");
            $('#Description1').val(result.Description1).trigger("change");

            $("#JobAcademicRequirementsModal").modal("show");
            $("#CreateJobAcademicBtn").hide();
            $("#ModifyJobAcademicBtn").show();
        },
        error: OnError
    });
    return false;
}

function LoadJobAcademicRequirements(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetJobAcademicRequirements,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (jAcademicQualifications) {
            var rows = "";
            $.each(jAcademicQualifications, function (i, jAcademicQualification) {
                rows += "<tr>";
                rows += "<td>" + jAcademicQualification.Code + "</td>";
                rows += "<td>" + jAcademicQualification.Description1 + "</td>";
                rows += '<td><a href="#" onclick="return EditJobAcademicRequirement(' + jAcademicQualification.LineNo + ',\'' + jAcademicQualification.DocumentNo + '\');">Edit</a> | <a href="#" onclick="DeleteJobAcademicRequirement(' + jAcademicQualification.LineNo + ',\'' + jAcademicQualification.DocumentNo + '\')">Delete</a></td>';
                rows += "</tr>";
                $("#AcademicRequirementsTbl tbody").html(rows);
            });
            $("#AcademicRequirementsTbl").css("display", "block");
        },
        error: function (xhr, status, thrownError) {
            rows += "<tr>";
            rows += "<td class='text-danger text-center' colspan='8'>Unable to load the academic requirements lines.</td>";
            rows += "</tr>";
            $("#AcademicRequirementsTbl tbody").html(rows);
            $("#AcademicRequirementsTbl").css("display", "block");
        }
    });
}

//Professional Qualifications
function CreateJobProfessionalQualification() {

    var professionalQualifications = {
        DocumentNo: $("#No").val(),
        Code: $("#Code").val(),
        Description2: $("#Description2").val()
    };

    $.ajax({
        url: AJAXUrls.CreateJobProfessionalQualification,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(professionalQualifications),
        cache: false,
        success: function (result) {
            if (result.success) {
                $('#errorMessage').hide();
                $('#successMessage').show();
                LoadJobProfessionalBodiesRequirements($("#No").val());
                $("#JobProfessionalQualificationsModal").modal("hide");
            } else {
                $('#errorMessage').html(result.message);
                $('#errorMessage').show();
                $('#successMessage').hide();
            }
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}

function ModifyJobProfessionalQualification() {

    var professionalQualifications = {
        LineNo: $("#LineNo").val(),
        DocumentNo: $("#No").val(),
        Code: $("#Code").val(),
        Description2: $("#Description2").val()
    };

    $.ajax({
        url: AJAXUrls.ModifyJobProfessionalQualification,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(professionalQualifications),
        cache: false,
        success: function (result) {
            if (result.success) {
                $('#errorMessage').hide();
                $('#successMessage').show();
                LoadJobProfessionalBodiesRequirements($("#No").val());
                $("#JobProfessionalQualificationsModal").modal("hide");
            } else {
                $('#errorMessage').html(result.message);
                $('#errorMessage').show();
                $('#successMessage').hide();
            }
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}

function ClearJobProfessionalQualificationsModal() {
    $("#LineNo").val(0);
    $("#DocumentNo").val("");
    $("#Code").val("").trigger("change");
    $("#Description2").val("");

    $("#CreateJobQualificationBtn").show();
    $("#ModifyJobQualificationBtn").hide();

    $("#CodeError").text("");
    $("#Description2Error").text("");
} 

function DeleteJobProfessionalQualification(LineNo, DocumentNo) {
    var ans = confirm("Are you sure you want to delete this Line?");
    if (ans) {
        $.ajax({
            url: AJAXUrls.DeleteJobProfessionalQualification,
            type: "POST",
            dataType: "json",
            data: { LineNo: LineNo, DocumentNo: DocumentNo },
            cache: false,
            success: function (result) {
                LoadJobProfessionalBodiesRequirements(DocumentNo);
            },
            error: function (errormessage) {
                //alert(errormessage.responseText);
                alert("Error");
            }
        });
    }
}

function EditJobProfessionalQualification(LineNo, DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetJobProfessionalQualification,
        type: "GET",
        dataType: "json",
        data: { LineNo: LineNo, DocumentNo: DocumentNo },
        cache: false,
        success: function (result) {
            $('#LineNo').val(result.LineNo);
            $('#DocumentNo').val(result.DocumentNo);
            $('#Code').val(result.Code).trigger("change");
            $('#Description2').val(result.Description2).trigger("change");

            $("#JobProfessionalQualificationsModal").modal("show");
            $("#CreateJobQualificationBtn").hide();
            $("#ModifyJobQualificationBtn").show();
        },
        error: OnError
    });
    return false;
}

function LoadJobProfessionalQualifications(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetJobProfessionalQualifications,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (professionalQualificationRequirements) {
            var rows = "";
            $.each(professionalQualificationRequirements, function (i, professionalQualificationRequirement) {
                rows += "<tr>";
                rows += "<td>" + professionalQualificationRequirement.Code + "</td>";
                rows += "<td>" + professionalQualificationRequirement.Description2 + "</td>";
                rows += '<td><a href="#" onclick="return EditJobProfessionalQualification(' + professionalQualificationRequirement.LineNo + ',\'' + professionalQualificationRequirement.DocumentNo + '\');">Edit</a> | <a href="#" onclick="DeleteJobProfessionalQualification(' + professionalQualificationRequirement.LineNo + ',\'' + professionalQualificationRequirement.DocumentNo + '\')">Delete</a></td>';
                rows += "</tr>";
                $("#ProfessionalQualificationsTbl tbody").html(rows);
            });
            $("#ProfessionalQualificationsTbl").css("display", "block");
        },
        error: function (xhr, status, thrownError) {
            rows += "<tr>";
            rows += "<td class='text-danger text-center' colspan='8'>Unable to load the academic requirements lines.</td>";
            rows += "</tr>";
            $("#ProfessionalQualificationsTbl tbody").html(rows);
            $("#ProfessionalQualificationsTbl").css("display", "block");
        }
    });
}

//Professional Bodies

function CreateJobProfessionalBody() {

    var professionalBodies = {
        DocumentNo: $("#No").val(),
        Code: $("#Code").val(),
        Description3: $("#Description3").val()
    };

    $.ajax({
        url: AJAXUrls.CreateJobProfessionalBody,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(professionalBodies),
        cache: false,
        success: function (result) {
            if (result.success) {
                $('#errorMessage').hide();
                $('#successMessage').show();
                LoadJobProfessionalBodiesRequirements($("#No").val());
                $("#JobProfessionalBodiesModal").modal("hide");
            } else {
                $('#errorMessage').html(result.message);
                $('#errorMessage').show();
                $('#successMessage').hide();
            }
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}

function ModifyJobProfessionalBody() {

    var professionalBodies = {
        LineNo: $("#LineNo").val(),
        DocumentNo: $("#No").val(),
        Code: $("#Code").val(),
        Description3: $("#Description3").val()
    };

    $.ajax({
        url: AJAXUrls.ModifyJobProfessionalBody,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(professionalBodies),
        cache: false,
        success: function (result) {
            if (result.success) {
                $('#errorMessage').hide();
                $('#successMessage').show();
                LoadJobProfessionalBodiesRequirements($("#No").val());
                $("#JobProfessionalBodiesModal").modal("hide");
            } else {
                $('#errorMessage').html(result.message);
                $('#errorMessage').show();
                $('#successMessage').hide();
            }
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}

function ClearJobProfessionalBodiesModal() {
    $("#LineNo").val(0);
    $("#DocumentNo").val("");
    $("#Code").val("").trigger("change");
    $("#Description3").val("");

    $("#CreateJobProfessionalBtn").show();
    $("#ModifyJobProfessionalBtn").hide();

    $("#CodeError").text("");
    $("#Description3Error").text("");
}

function DeleteJobProfessionalBody(LineNo, DocumentNo) {
    var ans = confirm("Are you sure you want to delete this Line?");
    if (ans) {
        $.ajax({
            url: AJAXUrls.DeleteJobProfessionalBody,
            type: "POST",
            dataType: "json",
            data: { LineNo: LineNo, DocumentNo: DocumentNo },
            cache: false,
            success: function (result) {
                LoadJobProfessionalBodiesRequirements(DocumentNo);
            },
            error: function (errormessage) {
                //alert(errormessage.responseText);
                alert("Error");
            }
        });
    }
}

function EditJobProfessionalBody(LineNo, DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetJobProfessionalBodiesRequirement,
        type: "GET",
        dataType: "json",
        data: { LineNo: LineNo, DocumentNo: DocumentNo },
        cache: false,
        success: function (result) {
            $('#LineNo').val(result.LineNo);
            $('#DocumentNo').val(result.DocumentNo);
            $('#Code').val(result.Code).trigger("change");
            $('#Description3').val(result.Description3);

            $("#JobProfessionalBodiesModal").modal("show");
            $("#CreateJobProfessionalBtn").hide();
            $("#ModifyJobProfessionalBtn").show();
        },
        error: OnError
    });
    return false;
}

function LoadJobProfessionalBodiesRequirements(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetJobProfessionalBodiesRequirements,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (professionalBodiesRequirements) {
            var rows = "";
            $.each(professionalBodiesRequirements, function (i, professionalBodiesRequirement) {
                rows += "<tr>";
                rows += "<td>" + professionalBodiesRequirement.Description3 + "</td>";
                rows += '<td><a href="#" onclick="return EditJobProfessionalBody(' + professionalBodiesRequirement.LineNo + ',\'' + professionalBodiesRequirement.DocumentNo + '\');">Edit</a> | <a href="#" onclick="DeleteJobProfessionalBody(' + professionalBodiesRequirement.LineNo + ',\'' + professionalBodiesRequirement.DocumentNo + '\')">Delete</a></td>';
                rows += "</tr>";
                $("#ProfessionalBodiesTbl tbody").html(rows);
            });
            $("#ProfessionalBodiesTbl").css("display", "block");
        },
        error: function (xhr, status, thrownError) {
            rows += "<tr>";
            rows += "<td class='text-danger text-center' colspan='8'>Unable to load the academic requirements lines.</td>";
            rows += "</tr>";
            $("#ProfessionalBodiesTbl tbody").html(rows);
            $("#ProfessionalBodiesTbl").css("display", "block");
        }
    });
}

//Job Requirement Responsibilities
function CreateJobResponsibilities() {

    var jobResponsibilities = {
        DocumentNo: $("#No").val(),
        Code: $("#Code").val(),
        Description7: $("#Description7").val()
    };

    $.ajax({
        url: AJAXUrls.CreateJobRequirementResponsibility,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(jobResponsibilities),
        cache: false,
        success: function (result) {
            if (result.success) {
                $('#errorMessage').hide();
                $('#successMessage').show();
                LoadJobResponsibilities($("#No").val());
                $("#JobReponsibilitiesModal").modal("hide");
            } else {
                $('#errorMessage').html(result.message);
                $('#errorMessage').show();
                $('#successMessage').hide();
            }
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}

function ModifyJobResponsibilities() {

    var jobResponsibilities = {
        LineNo: $("#LineNo").val(),
        DocumentNo: $("#No").val(),
        Code: $("#Code").val(),
        Description7: $("#Description7").val()
    };

    $.ajax({
        url: AJAXUrls.ModifyJobRequirementResponsibility,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(jobResponsibilities),
        cache: false,
        success: function (result) {
            if (result.success) {
                $('#errorMessage').hide();
                $('#successMessage').show();
                LoadJobResponsibilities($("#No").val());
                $("#JobResponsibilityModal").modal("hide");
            } else {
                $('#errorMessage').html(result.message);
                $('#errorMessage').show();
                $('#successMessage').hide();
            }
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}

function ClearJobReponsibilitiesModal() {
    $("#LineNo").val(0);
    $("#DocumentNo").val("");
    $("#Code").val("").trigger("change");
    $("#Description7").val("");

    $("#CreateResponsibilityBtn").show();
    $("#ModifyResponsibilityBtn").hide();

    $("#CodeError").text("");
    $("#DescriptionError").text("");
}

function DeleteJobResponsibility(LineNo, DocumentNo) {
    var ans = confirm("Are you sure you want to delete this Line?");
    if (ans) {
        $.ajax({
            url: AJAXUrls.DeleteJobRequirementResponsibility,
            type: "POST",
            dataType: "json",
            data: { LineNo: LineNo, DocumentNo: DocumentNo },
            cache: false,
            success: function (result) {
                LoadJobResponsibilities(DocumentNo);
            },
            error: function (errormessage) {
                //alert(errormessage.responseText);
                alert("Error");
            }
        });
    }
}

function EditJobResponsibility(LineNo, DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetJobResponsibilityByLineNo,
        type: "GET",
        dataType: "json",
        data: { LineNo: LineNo, DocumentNo: DocumentNo },
        cache: false,
        success: function (result) {
            $('#LineNo').val(result.LineNo);
            $('#DocumentNo').val(result.DocumentNo);
            $('#Code').val(result.Code).trigger("change");
            $('#Description7').val(result.Description7).trigger("change");

            $("#JobResponsibilityModal").modal("show");
            $("#CreateResponsibilityBtn").hide();
            $("#ModifyResponsibilityBtn").show();
        },
        error: OnError
    });
    return false;
}

function LoadJobResponsibilities(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetJobResponsibilities,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (jobResponsibilities) {
            var rows = "";
            $.each(jobResponsibilities, function (i, jobResponsibility) {
                rows += "<tr>";
                rows += "<td>" + jobResponsibility.Description7 + "</td>";
                rows += '<td><a href="#" onclick="return EditJobResponsibility(' + jobResponsibility.LineNo + ',\'' + jobResponsibility.DocumentNo + '\');">Edit</a> | <a href="#" onclick="DeleteJobResponsibility(' + jobResponsibility.LineNo + ',\'' + jobResponsibility.DocumentNo + '\')">Delete</a></td>';
                rows += "</tr>";
                $("#JobResponsibilitiesTbl tbody").html(rows);
            });
            $("#JobResponsibilitiesTbl").css("display", "block");
        },
        error: function (xhr, status, thrownError) {
            rows += "<tr>";
            rows += "<td class='text-danger text-center' colspan='8'>Unable to load the academic requirements lines.</td>";
            rows += "</tr>";
            $("#JobResponsibilitiesTbl tbody").html(rows);
            $("#JobResponsibilitiesTbl").css("display", "block");
        }
    });
}

//Chapter 6 Requirements

function CreateChapter6Requirement() {

    var chapter6Requirments = {
        DocumentNo: $("#No").val(),
        Code: $("#Code").val(),
        Description: $("#Description").val()
    };

    $.ajax({
        url: AJAXUrls.CreateChapter6Requirement,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(chapter6Requirments),
        cache: false,
        success: function (result) {
            if (result.success) {
                $('#errorMessage').hide();
                $('#successMessage').show();
                LoadJobChapter6Requirements($("#No").val());
                $("#Chapter6RequirementsModal").modal("hide");
            } else {
                $('#errorMessage').html(result.message);
                $('#errorMessage').show();
                $('#successMessage').hide();
            }
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}

function ModifyChapter6Requirement() {

    var chapter6Requirments = {
        LineNo: $("#LineNo").val(),
        DocumentNo: $("#No").val(),
        Code: $("#Code").val(),
        Description5: $("#Description5").val()
    };

    $.ajax({
        url: AJAXUrls.ModifyChapter6Requirement,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(chapter6Requirments),
        cache: false,
        success: function (result) {
            if (result.success) {
                $('#errorMessage').hide();
                $('#successMessage').show();
                LoadJobChapter6Requirements($("#No").val());
                $("#Chapter6RequirementsModal").modal("hide");
            } else {
                $('#errorMessage').html(result.message);
                $('#errorMessage').show();
                $('#successMessage').hide();
            }
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}

function ClearChapter6RequirementsModal() {
    $("#LineNo").val(0);
    $("#DocumentNo").val("");
    $("#Code").val("").trigger("change");
    $("#Description5").val("");

    $("#CreateChapter6Btn").show();
    $("#ModifyChapter6Btn").hide();

    $("#CodeError").text("");
    $("#DescriptionError").text("");
}

function DeleteChapter6Requirement(LineNo, DocumentNo) {
    var ans = confirm("Are you sure you want to delete this Line?");
    if (ans) {
        $.ajax({
            url: AJAXUrls.DeleteChapter6Requirement,
            type: "POST",
            dataType: "json",
            data: { LineNo: LineNo, DocumentNo: DocumentNo },
            cache: false,
            success: function (result) {
                LoadJobChapter6Requirements(DocumentNo);
            },
            error: function (errormessage) {
                //alert(errormessage.responseText);
                alert("Error");
            }
        });
    }
}

function EditChapter6Requirement(LineNo, DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetJobChapter6RequirementByLineNo,
        type: "GET",
        dataType: "json",
        data: { LineNo: LineNo, DocumentNo: DocumentNo },
        cache: false,
        success: function (result) {
            $('#LineNo').val(result.LineNo);
            $('#DocumentNo').val(result.DocumentNo);
            $('#Code').val(result.Code).trigger("change");
            $('#Description5').val(result.Description5).trigger("change");

            $("#Chapter6RequirementsModal").modal("show");
            $("#CreateChapter6Btn").hide();
            $("#ModifyChapter6Btn").show();
        },
        error: OnError
    });
    return false;
}

function LoadJobChapter6Requirements(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetJobChapter6Requirements,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (chapter6Requirements) {
            var rows = "";
            $.each(chapter6Requirements, function (i, chapter6Requirement) {
                rows += "<tr>";
                rows += "<td>" + chapter6Requirement.Code + "</td>";
                rows += "<td>" + chapter6Requirement.Description5 + "</td>";
                rows += '<td><a href="#" onclick="return EditChapter6Requirement(' + chapter6Requirement.LineNo + ',\'' + chapter6Requirement.DocumentNo + '\');">Edit</a> | <a href="#" onclick="DeleteChapter6Requirement(' + chapter6Requirement.LineNo + ',\'' + chapter6Requirement.DocumentNo + '\')">Delete</a></td>';
                rows += "</tr>";
                $("#Chapter6RequirementsTbl tbody").html(rows);
            });
            $("#Chapter6RequirementsTbl").css("display", "block");
        },
        error: function (xhr, status, thrownError) {
            rows += "<tr>";
            rows += "<td class='text-danger text-center' colspan='8'>Unable to load the academic requirements lines.</td>";
            rows += "</tr>";
            $("#Chapter6RequirementsTbl tbody").html(rows);
            $("#Chapter6RequirementsTbl").css("display", "block");
        }
    });
}

//Experience
function CreateJobExperience() {

    var jobExperience = {
        DocumentNo: $("#No").val(),
        Code: $("#Code").val(),
        Years: $("#Years").val(),
        Description4: $("#Description4").val()
    };

    $.ajax({
        url: AJAXUrls.CreateJobExperienceRequirement,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(jobExperience),
        cache: false,
        success: function (result) {
            if (result.success) {
                $('#errorMessage').hide();
                $('#successMessage').show();
                LoadJobExperienceRequirements($("#No").val());
                $("#JobExperienceModal").modal("hide");
            } else {
                $('#errorMessage').html(result.message);
                $('#errorMessage').show();
                $('#successMessage').hide();
            }
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}

function ModifyJobExperience() {

    var jobExperience = {
        LineNo: $("#LineNo").val(),
        DocumentNo: $("#No").val(),
        Code: $("#Code").val(),
        Years: $("#Years").val(),
        Description4: $("#Description4").val()
    };

    $.ajax({
        url: AJAXUrls.ModifyJobExperienceRequirement,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(jobExperience),
        cache: false,
        success: function (result) {
            if (result.success) {
                $('#errorMessage').hide();
                $('#successMessage').show();
                LoadJobExperienceRequirements($("#No").val());
                $("#JobExperienceModal").modal("hide");
            } else {
                $('#errorMessage').html(result.message);
                $('#errorMessage').show();
                $('#successMessage').hide();
            }
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}

function ClearJobExperienceModal() {
    $("#LineNo").val(0);
    $("#DocumentNo").val("");
    $("#Years").val("");
    $("#Code").val("").trigger("change");
    $("#Description4").val("");

    $("#CreateExperienceBtn").show();
    $("#ModifyExperienceBtn").hide();

    $("#CodeError").text("");
    $("#DescriptionError").text("");
}

function DeleteJobExperience(LineNo, DocumentNo) {
    var ans = confirm("Are you sure you want to delete this Line?");
    if (ans) {
        $.ajax({
            url: AJAXUrls.DeleteJobExperienceRequirement,
            type: "POST",
            dataType: "json",
            data: { LineNo: LineNo, DocumentNo: DocumentNo },
            cache: false,
            success: function (result) {
                LoadJobExperienceRequirements(DocumentNo);
            },
            error: function (errormessage) {
                //alert(errormessage.responseText);
                alert("Error");
            }
        });
    }
}

function EditJobExperience(LineNo, DocumentNo) { 
    $.ajax({
        url: AJAXUrls.GetJobExperienceRequirementByLineNo,
        type: "GET",
        dataType: "json",
        data: { LineNo: LineNo, DocumentNo: DocumentNo },
        cache: false,
        success: function (result) {
            $('#LineNo').val(result.LineNo);
            $('#DocumentNo').val(result.DocumentNo);
            $('#Code').val(result.Code).trigger("change");
            $('#Years').val(result.Years);
            $('#Description4').val(result.Description6).trigger("change");

            $("#JobExperienceModal").modal("show");
            $("#CreateExperienceBtn").hide();
            $("#ModifyExperienceBtn").show();
        },
        error: OnError
    });
    return false;
}

function LoadJobExperienceRequirements(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetJobExperienceRequirements,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (jobExperiences) {
            var rows = "";
            $.each(jobExperiences, function (i, jobExperiences) {
                rows += "<tr>";
                rows += "<td>" + jobExperiences.Description4 + "</td>";
                rows += "<td>" + jobExperiences.Years + "</td>";
                rows += '<td><a href="#" onclick="return EditJobExperience(' + jobExperiences.LineNo + ',\'' + jobExperiences.DocumentNo + '\');">Edit</a> | <a href="#" onclick="DeleteJobExperience(' + jobExperiences.LineNo + ',\'' + jobExperiences.DocumentNo + '\')">Delete</a></td>';
                rows += "</tr>";
                $("#JobExperienceTbl tbody").html(rows);
            });
            $("#JobExperienceTbl").css("display", "block");
        },
        error: function (xhr, status, thrownError) {
            rows += "<tr>";
            rows += "<td class='text-danger text-center' colspan='8'>Unable to load the academic requirements lines.</td>";
            rows += "</tr>";
            $("#JobExperienceTbl tbody").html(rows);
            $("#JobExperienceTbl").css("display", "block");
        }
    });
}

//Other
function CreateOtherRequirements() {

    var otherRequirements = {
        DocumentNo: $("#No").val(),
        Code: $("#Code").val(),
        Description6: $("#Description6").val()
    };

    $.ajax({
        url: AJAXUrls.CreateJobOtherRequirement,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(otherRequirements),
        cache: false,
        success: function (result) {
            if (result.success) {
                $('#errorMessage').hide();
                $('#successMessage').show();
                LoadOtherRequirements($("#No").val());
                $("#OtherRequirementsModal").modal("hide");
            } else {
                $('#errorMessage').html(result.message);
                $('#errorMessage').show();
                $('#successMessage').hide();
            }
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}

function ModifyOtherRequirements() {

    var otherRequirements = {
        LineNo: $("#LineNo").val(),
        DocumentNo: $("#No").val(),
        Code: $("#Code").val(),
        Description6: $("#Description6").val()
    };

    $.ajax({
        url: AJAXUrls.ModifyJobOtherRequirement,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(otherRequirements),
        cache: false,
        success: function (result) {
            if (result.success) {
                $('#errorMessage').hide();
                $('#successMessage').show();
                LoadOtherRequirements($("#No").val());
                $("#OtherRequirementsModal").modal("hide");
            } else {
                $('#errorMessage').html(result.message);
                $('#errorMessage').show();
                $('#successMessage').hide();
            }
        },
        error: function (xhr, errorType, exception) {
            alert(xhr.responseText);
        }
    });
}

function ClearOtherRequirementsModal() {
    $("#LineNo").val(0);
    $("#DocumentNo").val("");
    $("#Code").val("").trigger("change");
    $("#Description7").val("");

    $("#CreateOtherBtn").show();
    $("#ModifyOtherBtn").hide();

    $("#CodeError").text("");
    $("#Description7Error").text("");
}

function DeleteOtherRequirements(LineNo, DocumentNo) {
    var ans = confirm("Are you sure you want to delete this Line?");
    if (ans) {
        $.ajax({
            url: AJAXUrls.DeleteJobOtherRequirement,
            type: "POST",
            dataType: "json",
            data: { LineNo: LineNo, DocumentNo: DocumentNo },
            cache: false,
            success: function (result) {
                LoadOtherRequirements(DocumentNo);
            },
            error: function (errormessage) {
                //alert(errormessage.responseText);
                alert("Error");
            }
        });
    }
}

function EditOtherRequirements(LineNo, DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetJobChapter6RequirementByLineNo,
        type: "GET",
        dataType: "json",
        data: { LineNo: LineNo, DocumentNo: DocumentNo },
        cache: false,
        success: function (result) {
            $('#LineNo').val(result.LineNo);
            $('#DocumentNo').val(result.DocumentNo);
            $('#Code').val(result.Code).trigger("change");
            $('#Description7').val(result.Description7).trigger("change");

            $("#Chapter6RequirementsModal").modal("show");
            $("#CreateChapter6Btn").hide();
            $("#ModifyChapter6Btn").show();
        },
        error: OnError
    });
    return false;
}

function LoadOtherRequirements(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetOtherRequirements,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (otherRequirements) {
            var rows = "";
            $.each(otherRequirements, function (i, otherRequirement) {
                rows += "<tr>";
                rows += "<td>" + otherRequirement.Description6 + "</td>";
                rows += '<td><a href="#" onclick="return EditOtherRequirements(' + otherRequirement.LineNo + ',\'' + otherRequirement.DocumentNo + '\');">Edit</a> | <a href="#" onclick="DeleteOtherRequirements(' + otherRequirement.LineNo + ',\'' + otherRequirement.DocumentNo + '\')">Delete</a></td>';
                rows += "</tr>";
                $("#OtherRequirementsTbl tbody").html(rows);
            });
            $("#OtherRequirementsTbl").css("display", "block");
        },
        error: function (xhr, status, thrownError) {
            rows += "<tr>";
            rows += "<td class='text-danger text-center' colspan='8'>Unable to load the academic requirements lines.</td>";
            rows += "</tr>";
            $("#OtherRequirementsTbl tbody").html(rows);
            $("#OtherRequirementsTbl").css("display", "block");
        }
    });
}