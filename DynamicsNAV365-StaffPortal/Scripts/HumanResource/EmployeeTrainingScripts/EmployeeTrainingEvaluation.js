function InitializeTrainingEvaluationScripts() {
  /*  var dateToday = '01/01/2020';

    $("#TrainingStartDate").datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        minDate: dateToday
    });

    $("#TrainingEndDate").datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        minDate: dateToday
    });*/

    AddTrainingEvaluationDropDownListSearch();
    AddOnChangeEvents();
}
function AddOnChangeEvents() {
    $("#ApplicationNo").change(function () {
        GetAttendedTrainingApplicationDetails($(this).val());
    });

}
function AddTrainingEvaluationDropDownListSearch() {
   
    $("#CalenderYear").select2({
        placeholder: $("#YearLbl").text(),
        allowClear: true
    });

    $("#ApplicationNo").select2({
        placeholder: $("#ApplicationNoLbl").text(),
        allowClear: true
    });

    $("#TrainingObjective").select2({
        placeholder: $("#TrainingObjectiveLbl").text(),
        allowClear: true
    });

    $("#ParticipationEncouraged").select2({
        placeholder: $("#ParticipationEncouragedLbl").text(),
        allowClear: true
    });

    $("#TopicsCovered").select2({
        placeholder: $("#TopicsCoveredLbl").text(),
        allowClear: true
    });

    $("#ContentOrganised").select2({
        placeholder: $("#ContentOrganisedLbl").text(),
        allowClear: true
    });

    $("#MaterialDistributed").select2({
        placeholder: $("#MaterialDistributedLbl").text(),
        allowClear: true
    });

    $("#TrainingExperience").select2({
        placeholder: $("#TrainingExperienceLbl").text(),
        allowClear: true
    });

    $("#TrainerKnowledgeable").select2({
        placeholder: $("#TrainerKnowledgeableLbl").text(),
        allowClear: true
    });

    $("#ObjectiveMet").select2({
        placeholder: $("#ObjectiveMetLbl").text(),
        allowClear: true
    });

    $("#Rate").select2({
        placeholder: $("#RateLbl").text(),
        allowClear: true
    });

    $("#TrainerWellPrepared").select2({
        placeholder: $("#TrainerWellPreparedLbl").text(),
        allowClear: true
    });
}
function LoadTrainingEvaluationDocuments(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetTrainingEvaluationApplicationDocuments,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (trainingUploadedDocuments) {
            var rows = "";
            $.each(trainingUploadedDocuments, function (i, trainingUploadedDocument) {
                rows += "<tr>";
                rows += "<td>" + trainingUploadedDocument.DocumentDescription + "</td>";
                if (trainingUploadedDocument.DocumentAttached) {
                    rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' checked disabled>" + "</td>";
                } else {
                    rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' disabled>" + "</td>";
                }

                rows += '<td><a href="#" onclick="return EditTrainingEvaluationDocument(\'' + DocumentNo + '\',\'' + trainingUploadedDocument.DocumentCode + '\');"><i class="fa fa-paperclip" aria-hidden="true">Attach</i></a></td>';
                rows += "</tr>";

                $("#ApplicationDocumentsTbl tbody").html(rows);
            });

            $("#AjaxLoader").css("display", "none");
            $("#ApplicationDocumentsTbl").css("display", "block");
        },
        error: function (xhr, status, error) {

        }
    });
}
function LoadTrainingEvaluationDocumentsView(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetTrainingEvaluationApplicationDocuments,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (trainingUploadedDocuments) {
            var rows = "";
            $.each(trainingUploadedDocuments, function (i, trainingUploadedDocument) {
                rows += "<tr>";
                rows += "<td>" + trainingUploadedDocument.DocumentDescription + "</td>";
                if (trainingUploadedDocument.DocumentAttached) {
                    rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' checked disabled>" + "</td>";
                } else {
                    rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' disabled>" + "</td>";
                }
                rows += '<td><a href="#" onclick="return ViewTrainingNeedDocument(\'' + DocumentNo + '\',\'' + trainingUploadedDocument.DocumentCode + '\');"><i class="" aria-hidden="true">View</i></a></td>';
                rows += "</tr>";

                $("#TrainingEvaluationDocumentsTbl tbody").html(rows);
            });

            $("#AjaxLoader").css("display", "none");
            $("#TrainingEvaluationDocumentsTbl tbody").html(rows);
            $("#TrainingEvaluationDocumentsTbl").css("display", "block");
        },
        error: function (xhr, status, error) {

        }
    });
}
function ResetTrainingEvaluationDocumentModal() {
    $("#ApplicationDocumentFile").val("");
    //Ladda.stopAll();
}
function UploadApplicationTrainingEvaluationDocument() {
    var DocumentNo = $("#TrainingEvaluationNo").val();
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
        url: AJAXUrls.UploadTrainingEvaluationApplicationDocument,
        type: "POST",
        data: frmData,
        dataType: 'json',
        contentType: false,
        processData: false,
        enctype: "multipart/form-data",
        async: true,
        cache: false,
        success: function (result) {
            $('#txtMessage').html(result.message);
            if (result.success) {
                $('#ApplicationDocumentModal').modal('hide');
                $('#errorMessage').hide();
                LoadTrainingEvaluationDocuments(DocumentNo);
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
function EditTrainingEvaluationDocument(DocumentNo, DocumentCode) {

    //Clear link path
    ResetTrainingEvaluationApplicationDocumentModal();

    $.ajax({
        url: AJAXUrls.GetTrainingEvaluationApplicationDocument,
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
            //$("#UploadLeaveApplicationDocumentBtn").show();
        },
        error: function (xhr, status, error) {

        }
    });
    return false;
}
function ResetTrainingEvaluationApplicationDocumentModal() {
    $("#ApplicationDocumentFile").val("");
    //Ladda.stopAll();
}
//Load Training Applications
function GetAttendedTrainingApplicationDetails(ApplicationNo) {
    $.ajax({
        url: AJAXUrls.GetAttendedTrainingApplicationDetails,
        type: "GET",
        dataType: "json",
        data: { ApplicationNo: ApplicationNo },
        cache: false,
        success: function (TrainingApplications) {
            $.each(TrainingApplications, function (i, TrainingApplication) {
                $("#CalenderYear").val(TrainingApplication.CalenderYear);
                $("#CalenderYear").css("background-color", "LightGray");
                $("#DevelopmentNeed").val(TrainingApplication.DevelopmentNeed);
                $("#DevelopmentNeed").css("background-color", "LightGray");
                $("#TrainingProvider").val(TrainingApplication.TrainingProvider);
                $("#TrainingProvider").css("background-color", "LightGray");
                $("#TrainingLocation").val(TrainingApplication.TrainingLocation);
                $("#TrainingLocation").css("background-color", "LightGray");
                $("#Objectives").val(TrainingApplication.Objectives);
                $("#Objectives").css("background-color", "LightGray");
                $("#StartDate").val(TrainingApplication.TrainingStartDate);
                $("#StartDate").css("background-color", "LightGray");
                $("#EndDate").val(TrainingApplication.TrainingEndDate);
                $("#EndDate").css("background-color", "LightGray");
            });

        },
        error: OnError
    });
}
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