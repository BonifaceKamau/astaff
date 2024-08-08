function InitializeTrainingRequest() {
    var dateToday = '01/01/2020';

    $("#FromDate").datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        minDate: dateToday
    });
    $("#ToDate").datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        minDate: dateToday
    });

    AddTrainingDropDownListSearch();
}

function AddTrainingDropDownListSearch() {
    $("#TrainingNeed").select2({
        placeholder: $("#TrainingNeedLbl").text(),
        allowClear: true
    });

    $("#Year").select2({
        placeholder: $("#YearLbl").text(),
        allowClear: true
    });

}

function LoadTrainingDocuments(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetTrainingApplicationDocuments,
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

                rows += '<td><a href="#" onclick="return EditTrainingDocument(\'' + DocumentNo + '\',\'' + trainingUploadedDocument.DocumentCode + '\');"><i class="fa fa-paperclip" aria-hidden="true">Attach</i></a></td>';
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

//Load training Documents View
function LoadTrainingDocumentsView(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetTrainingApplicationDocuments,
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
                rows += '<td><a href="#" onclick="return ViewTrainingDocument(\'' + DocumentNo + '\',\'' + trainingUploadedDocument.DocumentCode + '\');"><i class="" aria-hidden="true">View</i></a></td>';
                rows += "</tr>";
            });

            $("#AjaxLoader").css("display", "none");
            $("#ApplicationDocumentsTbl tbody").html(rows);
            $("#ApplicationDocumentsTbl").css("display", "block");
        },
        error: function (xhr, status, error) {

        }
    });
}
   
//Reset training document Line
function ResetTrainingDocumentModal() {
    $("#ApplicationDocumentFile").val("");
    //Ladda.stopAll();
}

//Upload training document
function UploadApplicationTrainingDocument() {
    var DocumentNo = $("#ApplicationNo").val();
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
        url: AJAXUrls.UploadTrainingApplicationDocument,
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
                LoadTrainingDocuments(DocumentNo);
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

function EditTrainingDocument(DocumentNo, DocumentCode) {

    //Clear link path
    ResetTrainingApplicationDocumentModal();

    $.ajax({
        url: AJAXUrls.GetTrainingApplicationDocument,
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

function ResetTrainingApplicationDocumentModal() {
    $("#ApplicationDocumentFile").val("");
    //Ladda.stopAll();
}

