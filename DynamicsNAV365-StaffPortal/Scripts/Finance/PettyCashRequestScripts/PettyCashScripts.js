//Initialize petty cash request
function InitializePettyCashRequest() {
	var dateToday = new Date(0);
	$("#DateFrom").datepicker({
		dateFormat: "dd-mm-y",
		changeMonth: true,
		changeYear: true,
		minDate: dateToday
	});

	$("#DateTo").datepicker({
		dateFormat: "dd-mm-y",
		changeMonth: true,
		changeYear: true,
		minDate: dateToday
	});

	//Add dropdown search
	AddPettyCashDropDownListSearch();
    //End add dropdown search

	AddOnChangeEvents();

	loadpettycashaccounts($('#requesttype').val());
}

function AddOnChangeEvents() {
    $('#Dimension1').change(function () {
        loaddimension2($(this).val());
    });
    $('#Dimension2').change(function () {
        loaddimension3($(this).val());
    });
    $('#Dimension3').change(function () {
        loaddimension4($(this).val());
    });
    $('#Dimension4').change(function () {
        loaddimension5($(this).val());
    });
    $('#Dimension5').change(function () {
        loaddimension6($(this).val());
    });
    $('#Dimension6').change(function () {
        loaddimension7($(this).val());
    });
    $('#requesttype').change(function () {
        loadpettycashaccounts($(this).val());
    });
    
}

function AddPettyCashDropDownListSearch() {
	$("#ImprestCode").select2({
		placeholder: $("#ImprestCodeLbl").text(),
		allowClear: true
	});

	$("#Dimension1").select2({
	    placeholder: $("#Dimension1Lbl").text(),
	    allowClear: true
	});
	$("#Dimension2").select2({
	    placeholder: $("#Dimension2Lbl").text(),
	    allowClear: true
	});
	$("#Dimension3").select2({
	    placeholder: $("#Dimension3Lbl").text(),
	    allowClear: true
	});
	$("#Dimension4").select2({
	    placeholder: $("#Dimension4Lbl").text(),
	    allowClear: true
	});
	$("#Dimension5").select2({
	    placeholder: $("#Dimension5Lbl").text(),
	    allowClear: true
	});
	$("#Dimension6").select2({
	    placeholder: $("#Dimension6Lbl").text(),
	    allowClear: true
	});
	$("#Dimension7").select2({
	    placeholder: $("#Dimension7Lbl").text(),
	    allowClear: true
	});
	$("#requesttype").select2({
	    placeholder: $("#requesttypeLbl").text(),
	    allowClear: true
	});
	
}

//Get petty cash amount
function GetPettyCashAmount(DocumentNo) {
	var pettyCashAmount = 0;
	$.ajax({
		url: AJAXUrls.GetPettyCashAmount,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			pettyCashAmount = result.Amount.toLocaleString();
			$("#Amount").val(pettyCashAmount);
		},
		error: OnError
	});
}

var GenerateReport = function (DocNo) {
    $.ajax({
        async: true,
        type: "POST",
        datatype: "json",
        contentType: "application/json; charset = utf-8",
        processData: false,
        data: JSON.stringify({ DocNo: DocNo }),

        url: '/PettyCash/GenerateReport',
        success: function (data) {
            window.open(data.message, '_blank').focus();
            //window.alert(data.message);
            //window.location.reload();
        },
        error: function () {
            window.alert(data.message);
        }
    });
};

//Load Petty Cash lines
function LoadPettyCashLines(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetPettyCashLinesAjax,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (imprestLines) {
		    var rows = "";
		    $("#AddPettyCashLine").css("display", "block");
			$.each(imprestLines, function (i, imprestLine) {
				rows += "<tr>";
				rows += "<td>" + imprestLine.AccountType + "</td>";
				rows += "<td>" + imprestLine.AccountNo + "</td>";
				rows += "<td>" + imprestLine.impresttransaction + "</td>";
				rows += "<td>" + imprestLine.AccountName + "</td>";
				rows += "<td>" + imprestLine.LineDescription + "</td>";
				rows += "<td>" + imprestLine.LineAmount.toLocaleString() + "</td>";
				rows += "<td>" + imprestLine.Dimension1 + "</td>";
				//rows += "<td>" + imprestLine.Dimension2 + "</td>";
				//rows += "<td>" + imprestLine.Dimension3 + "</td>";
				//rows += "<td>" + imprestLine.Dimension4 + "</td>";
				//rows += "<td>" + imprestLine.Dimension5 + "</td>";
				//rows += "<td>" + imprestLine.Dimension6 + "</td>";
				//rows += "<td>" + imprestLine.Dimension7 + "</td>";
				if (imprestLine.Status == "Open" || imprestLine.Status == "Pending") {
				    rows += '<td><a href="#" onclick="return EditPettyCashLine(' + imprestLine.LineNo + ',\'' + imprestLine.DocumentNo + '\');">Edit</a> | <a href="#" onclick="DeletePettyCashLine(' + imprestLine.LineNo + ',\'' + imprestLine.DocumentNo + '\')">Delete</a></td>';
				} else {
				    $("#AddPettyCashLine").css("display", "none");
				}

				
				rows += "</tr>";
				$("#PettyCashLineTbl tbody").html(rows);
			});
			$("#PettyCashAjaxLoader").css("display", "none");
			$("#PettyCashLineTbl").css("display", "block");
		},
		error: function (xhr, status, thrownError) {
			rows += "<tr>";
			rows += "<td class='text-danger text-center' colspan='8'>Unable to load the imprest lines.</td>";
			rows += "</tr>";
			$("#PettyCashLineTbl tbody").html(rows);
			$("#PettyCashAjaxLoader").css("display", "none");
			$("#PettyCashLineTbl").css("display", "block");
		}
	});
}

function ViewPettyCashLines(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetPettyCashLinesAjax,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (imprestLines) {
			var rows = "";
			$.each(imprestLines, function (i, imprestLine) {
				rows += "<tr>";
				rows += "<td>" + imprestLine.AccountNo + "</td>";
				rows += "<td>" + imprestLine.AccountName + "</td>";
				rows += "<td>" + imprestLine.LineDescription + "</td>";
				rows += "<td>" + imprestLine.LineAmount.toLocaleString() + "</td>";
				rows += '<td><a href="#" onclick="return ViewPettyCashLine(' + imprestLine.LineNo + ',\'' + imprestLine.DocumentNo + '\');">View</a> </td>';
				rows += "</tr>";
				$("#PettyCashLineTbl tbody").html(rows);
			});
			$("#PettyCashAjaxLoader").css("display", "none");
			$("#PettyCashLineTbl").css("display", "block");
		},
		error: function (xhr, status, thrownError) {
			rows += "<tr>";
			rows += "<td class='text-danger text-center' colspan='8'>Unable to load the imprest lines.</td>";
			rows += "</tr>";
			$("#PettyCashLineTbl tbody").html(rows);
			$("#PettyCashAjaxLoader").css("display", "none");
			$("#PettyCashLineTbl").css("display", "block");
		}
	});
}

//Create Petty Cash line   
function CreatePettyCashLine() {

	var documentNo = $("#No").val();

	var validLine = ValidatePettyCashLine();
	if (validLine == false) {
		return false;
	}

	var PettyCashLineObj = {
		DocumentNo: documentNo,
		ImprestCode: $("#ImprestCode").val(),
		LineDescription: $("#LineDescription").val(),
		impresttransaction: $("#impresttransaction").val(),
		LineAmount: $("#LineAmount").val(),
		LineGlobalDimension1Code: $("#Dimension1").val(),
		LineGlobalDimension2Code: $("#Dimension2").val(),
		//LineShortcutDimension3Code: $("#Dimension3").val(),
		//LineShortcutDimension4Code: $("#Dimension4").val(),
		//LineShortcutDimension5Code: $("#Dimension5").val(),
		//LineShortcutDimension6Code: $("#Dimension6").val(),
		//LineShortcutDimension7Code: $("#Dimension7").val()
	};

	$.ajax({
		url: AJAXUrls.CreatePettyCashLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(PettyCashLineObj),
		cache: false,
		success: function (result) {
		    if (result.success) {
		        LoadPettyCashLines(documentNo);
		        GetPettyCashAmount(documentNo);
		        $("#PettyCashLineModal").modal("hide");
		    } else {
		        Swal.fire({
		            title: result.message,
		            //text: 'Do you want to continue',
		            icon: 'error',
		            confirmButtonText: 'Ok'
		        })

		    }
			
		},
		error: function (xhr, errorType, exception) {
			alert(xhr.responseText);
		}
	});
}

//Edit petty cash line
function EditPettyCashLine(LineNo, DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetPettyCashLineByLineNo,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			$('#LineNo').val(result.LineNo);
			$('#DocumentNo').val(result.DocumentNo);
			$('#ImprestCode').val(result.ImprestCode).trigger("change");
			$('#LineDescription').val(result.LineDescription).trigger("change");
			$('#impresttransaction').val(result.impresttransaction).trigger("change");
			$('#LineAmount').val(result.LineAmount);
			$('#FromCity').val(result.FromCity).trigger("change");
			$('#ToCity').val(result.ToCity).trigger("change");
			$('#Dimension1').val(result.LineGlobalDimension1Code).trigger("change");
			$('#Dimension2').val(result.LineGlobalDimension2Code).trigger("change");
			//$('#LineShortcutDimension3Code').val(result.LineShortcutDimension3Code).trigger("change");
			//$('#LineShortcutDimension4Code').val(result.LineShortcutDimension4Code).trigger("change");
			//$('#LineShortcutDimension5Code').val(result.LineShortcutDimension5Code).trigger("change");
			//$('#LineShortcutDimension6Code').val(result.LineShortcutDimension6Code).trigger("change");
			//$('#LineShortcutDimension7Code').val(result.LineShortcutDimension7Code).trigger("change");
			//$('#LineShortcutDimension8Code').val(result.LineShortcutDimension8Code).trigger("change");

			$("#PettyCashLineModal").modal("show");
			$("#CreatePettyCashLineBtn").hide();
			$("#ModifyPettyCashLineBtn").show();
		},
		error: OnError
	});
	return false;
}

//Modify petty cash line
function ModifyPettyCashLine() {
	var documentNo = $("#No").val();

	var validLine = ValidatePettyCashLine();
	if (validLine == false) {
		Swal.fire('error', 'the form is not valid, please check the fields and input the correct value and try again', 'error');
		return false;
	}

	var PettyCashLineObj = {
	    LineNo: $("#LineNo").val(),
	    DocumentNo: documentNo,
	    ImprestCode: $("#ImprestCode").val(),
		LineDescription: $("#LineDescription").val(),
		impresttransaction: $("#impresttransaction").val(),
	    LineAmount: $("#LineAmount").val(),
	    Dimension1: $("#Dimension1").val(),
	    Dimension2: $("#Dimension2").val(),
	    //Dimension3: $("#Dimension3").val(),
	    //Dimension4: $("#Dimension4").val(),
	    //Dimension5: $("#Dimension5").val(),
	    //Dimension6: $("#Dimension6").val(),
	    //Dimension7: $("#Dimension7").val()
	}
	$.ajax({
		url: AJAXUrls.ModifyPettyCashLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(PettyCashLineObj),
		cache: false,
		success: function (result) {
			if (result.success) {
				LoadPettyCashLines(documentNo);
				GetPettyCashAmount(documentNo);
				$("#PettyCashLineModal").modal("hide");

				ClearPettyCashLineModal();
			}else {
				Swal.fire('error', result.message, 'error');
			}
		},
		error: OnError
	});
}

//View petty cash line
function ViewPettyCashLine(LineNo, DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetPettyCashLineByLineNo,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			$("#LineNo").val(result.LineNo);
			$("#DocumentNo").val(result.DocumentNo);
			$("#ImprestCode").val(result.ImprestCode).trigger("change");
			$("#LineDescription").val(result.LineDescription).trigger("change");
			$("#impresttransaction").val(result.impresttransaction).trigger("change");
			$("#LineAmount").val(result.LineAmount.toLocaleString());
			$("#LineGlobalDimension1Code").val(result.LineGlobalDimension1Code).trigger("change");
			$("#LineGlobalDimension2Code").val(result.LineGlobalDimension2Code).trigger("change");
			/*$("#LineShortcutDimension3Code").val(result.LineShortcutDimension3Code).trigger("change");
			$("#LineShortcutDimension4Code").val(result.LineShortcutDimension4Code).trigger("change");
			$("#LineShortcutDimension5Code").val(result.LineShortcutDimension5Code).trigger("change");
			$("#LineShortcutDimension6Code").val(result.LineShortcutDimension6Code).trigger("change");
			$("#LineShortcutDimension7Code").val(result.LineShortcutDimension7Code).trigger("change");
			$("#LineShortcutDimension8Code").val(result.LineShortcutDimension8Code).trigger("change");*/

			$("#PettyCashLineModal").modal('show');
		},
		error: OnError
	});
	return false;
}

//Delete petty cash line
function DeletePettyCashLine(LineNo, DocumentNo) {
	var ans = confirm("Are you sure you want to delete this Line?");
	if (ans) {
		$.ajax({
			url: AJAXUrls.DeletePettyCashLine,
			type: "POST",
			dataType: "json",
			data: { LineNo: LineNo, DocumentNo: DocumentNo },
			cache: false,
			success: function (result) {
				LoadPettyCashLines(DocumentNo);
				GetPettyCashAmount(DocumentNo);
			},
			error: function (errormessage) {
				alert("Error");
			}
		});
	}
}

function ValidatePettyCashHeader() {

	//Clear petty cash line modal
	ClearPettyCashLineModal();

	var isValid = true;
	if ($('#No').val().trim() == "") {
		$('#No').css('border-color', 'Red');
		isValid = false;
	}
	else {
		$('#No').css('border-color', 'lightgrey');
	}

	if ($('#GlobalDimension1Code').val().trim() == "") {
		$("#GlobalDimension1CodeError").show();
		isValid = false;
	}
	else {
		$('#errorGlobalDimension1Code').hide();
	}

	return isValid;
}

function ValidatePettyCashLine() {
	var isValid = true;
	var label = "";
	if ($("#ImprestCode").val().trim() == "") {
		$("#ImprestCodeError").text("Petty Cash code cannot be empty.");
		Swal.fire('error', 'Petty Cash code cannot be empty.', 'error');
		isValid = false;
		console.log("imprest code error")
	}
	else {
		$("#ImprestCodeError").text("");
	}

	if ($("#LineDescription").val().trim() == "") {
		$("#LineDescriptionError").text("Petty Cash line description cannot be empty.");
		Swal.fire('error', 'Petty Cash line description cannot be empty.', 'error');
		isValid = false;
		console.log("LineDescription code error")
	}
	else {
		$("#LineDescriptionError").text("");
	}

	if (!isNaN(Number($("#LineAmount").val()))) {
		$("#LineAmountError").text("");
	} else {
		$("#LineAmountError").text("Petty Cash line amount must be numeric.");
		Swal.fire('error', 'Petty Cash line amount must be numeric.', 'error');
		isValid = false;
		console.log("LineAmount numeric code error")
	}

	if (($("#LineAmount").val() <= 0) || ($("#LineAmount").val().trim() == "")) {
		$("#LineAmountError").text("Petty Cash line amount cannot be less or equal to zero.");
		Swal.fire('error', 'Petty Cash line amount cannot be less or equal to zero.', 'error');
		isValid = false;
		console.log("LineAmount code error")
	}
	else {
		$("#LineAmountError").text("");
	}
	return isValid;
}

//Clear petty cash line modal
function ClearPettyCashLineModal() {
	$("#LineNo").val(0);
	$("#DocumentNo").val("");
	$("#ImprestCode").val("").trigger("change");
	$("#LineDescription").val("");
	$("#LineAmount").val(0);
	$("#LineGlobalDimension1Code").val("").trigger("change");
	$("#LineGlobalDimension2Code").val("").trigger("change");
	$("#LineShortcutDimension3Code").val("").trigger("change");
	$("#LineShortcutDimension4Code").val("").trigger("change");
	$("#LineShortcutDimension5Code").val("").trigger("change");
	$("#LineShortcutDimension6Code").val("").trigger("change");
	$("#LineShortcutDimension7Code").val("").trigger("change");
	$("#LineShortcutDimension8Code").val("").trigger("change");

	$("#CreatePettyCashLineBtn").show();
	$("#ModifyPettyCashLineBtn").hide();

	$("#ImprestCodeError").text("");
	$("#LineDescriptionError").text("");
	$("#LineAmountError").text("");
	$("#LineGlobalDimension1CodeError").text("");
	$("#LineGlobalDimension2CodeError").text("");
	$("#LineShortcutDimension3CodeError").text("");
	$("#LineShortcutDimension4CodeError").text("");
	$("#LineShortcutDimension5CodeError").text("");
	$("#LineShortcutDimension6CodeError").text("");
	$("#LineShortcutDimension7CodeError").text("");
	$("#LineShortcutDimension8CodeError").text("");
}

//Load petty cash Documents
function LoadPettyCashDocuments(DocumentNo) {
    var DocNo = DocumentNo;
    var Status = $("#Status").val();
    $.ajax({
        async: true,
        type: "POST",
        datatype: "json",
        contentType: "application/json; charset = utf-8",
        processData: false,
        data: JSON.stringify({ DocNo: DocNo, TableID: 51525003, Status: Status }),
        url: "/PurchaseRequisition/DocumentAttachments",
        success: function (data) {
            $("#divAttachDocs").html(data);
        },
        error: function () {
            Swal.fire("There is some problem to process your request. Please try after some time");
        }
    });
}

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
                data: JSON.stringify({ DocNo: No, DocID: Id, tblID: 51525003 }),
                contentType: "application/json; charset = utf-8",
                success: function (data) {
                    if (data.success == true) {
                        LoadPettyCashDocuments(No);
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
function AddAttachment() {
    $("#ApplicationDocumentModal").modal("show");
}

//Load petty cash Documents View
function LoadPettyCashDocumentsView(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetPettyCashPortalDocuments,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (imprestUploadedDocuments) {
			var rows = "";
			$.each(imprestUploadedDocuments, function (i, imprestUploadedDocument) {
				rows += "<tr>";
				rows += "<td>" + imprestUploadedDocument.DocumentDescription + "</td>";
				if (imprestUploadedDocument.DocumentAttached) {
					rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' checked disabled>" + "</td>";
				} else {
					rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' disabled>" + "</td>";
				}

				rows += '<td><a href="#" onclick="return ViewImprestDocument(\'' + DocumentNo + '\',\'' + imprestUploadedDocument.DocumentCode + '\');"><i class="" aria-hidden="true">View</i></a></td>';
				rows += "</tr>";
			});

			$("#PettyCashDocumentsAjaxLoader").css("display", "none");
			$("#ApplicationDocumentsTbl tbody").html(rows);
			$("#ApplicationDocumentsTbl").css("display", "block");
		},
		error: function (xhr, status, error) {

		}
	});
}

//Edit petty cash document
function EditPettyCashDocument(LineNo, DocumentNo, DocumentCode) {

	ResetPettyCashDocumentModal();

	$.ajax({
		url: AJAXUrls.GetPettyCashPortalDocumentLink,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo, DocumentCode: DocumentCode },
		cache: false,
		success: function (applicationDocument) {
			$("#DocumentNo").val(applicationDocument.DocumentNo);
			$("#DocumentCode").val(applicationDocument.DocumentCode);
			$("#DocumentDescription").val(applicationDocument.DocumentDescription);
			$('#errorMessage').hide();
			$("#ApplicationDocumentModal").modal("show");
			$("#UploadApplicationDocumentBtn").show();
		},
		error: function (xhr, status, error) {

		}
	});
	return false;
}

//Reset petty cash document Line
function ResetPettyCashDocumentModal() {
	$("#ApplicationDocumentFile").val("");
	//Ladda.stopAll();
}

//Upload petty cash document
function UploadPettyCashDocument() {
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
		url: AJAXUrls.UploadPettyCashDocumentLink,
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
				$('#ApplicationDocumentModal').modal('hide');
				$('#errorMessage').hide();
				LoadPettyCashDocuments(DocumentNo);
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
function loadpettycashaccounts(item) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetPettyCashAccounts,
        type: "GET",
        dataType: "json",
        data: { item: item },
        cache: false,
        success: function (Dimnesions2) {
            var rows = "";
            $.each(Dimnesions2.DropDownData.ListOfddlData, function (i, Dimnesions) {
                options += "<option value='" + Dimnesions.Text + "'>";
                options += Dimnesions.Value;
                options += "</option>";
            });
            //$("#ImprestCode").html(options);
        },
        error: OnError
    });
}
//load 2nd dimension
function loaddimension2(dimension1) {
	var options = "";
	options += "<option value=''>";
	options += "Select Dimension 1";
	options += "</option>";

	$.ajax({
		url: AJAXUrls.GetGlobalDimension2Codes,
		type: "GET",
		dataType: "json",
		cache: false,
		success: function (Dimnesions2) {
			var rows = "";
			$.each(Dimnesions2.DropDownData.ListOfddlData, function (i, Dimnesions) {
				options += "<option value='" + Dimnesions.Value + "'>";
				options += Dimnesions.Value;
				options += "</option>";
			});
			$("#Dimension2").html(options);
		},
		error: OnError
	});
}
//load 3rd dimension
function loaddimension3(dimension2) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetGlobalDimension3Codes,
        type: "GET",
        dataType: "json",
        data: { dimension2: dimension2 },
        cache: false,
        success: function (Dimnesions3) {
            var rows = "";
            $.each(Dimnesions3.DropDownData.ListOfddlData, function (i, Dimnesions) {
                options += "<option value='" + Dimnesions.Value + "'>";
                options += Dimnesions.Value;
                options += "</option>";
            });
            $("#Dimension3").html(options);
        },
        error: OnError
    });
}
//load 4th dimension
function loaddimension4(dimension3) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetGlobalDimension4Codes,
        type: "GET",
        dataType: "json",
        data: { dimension3: dimension3 },
        cache: false,
        success: function (Dimnesions4) {
            var rows = "";
            $.each(Dimnesions4.DropDownData.ListOfddlData, function (i, Dimnesions) {
                options += "<option value='" + Dimnesions.Value + "'>";
                options += Dimnesions.Value;
                options += "</option>";
            });
            $("#Dimension4").html(options);
        },
        error: OnError
    });
}
//load 5th dimension
function loaddimension5(dimension4) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetGlobalDimension5Codes,
        type: "GET",
        dataType: "json",
        data: { dimension4: dimension4 },
        cache: false,
        success: function (Dimnesions5) {
            var rows = "";
            $.each(Dimnesions5.DropDownData.ListOfddlData, function (i, Dimnesions) {
                options += "<option value='" + Dimnesions.Value + "'>";
                options += Dimnesions.Value;
                options += "</option>";
            });
            $("#Dimension5").html(options);
        },
        error: OnError
    });
}
//load 6th dimension
function loaddimension6(dimension5) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetGlobalDimension6Codes,
        type: "GET",
        dataType: "json",
        data: { dimension5: dimension5 },
        cache: false,
        success: function (Dimnesions6) {
            var rows = "";
            $.each(Dimnesions6.DropDownData.ListOfddlData, function (i, Dimnesions) {
                options += "<option value='" + Dimnesions.Value + "'>";
                options += Dimnesions.Value;
                options += "</option>";
            });
            $("#Dimension6").html(options);
        },
        error: OnError
    });
}
//load 7th dimension
function loaddimension7(dimension6) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetGlobalDimension7Codes,
        type: "GET",
        dataType: "json",
        data: { dimension6: dimension6 },
        cache: false,
        success: function (Dimnesions7) {
            var rows = "";
            $.each(Dimnesions7.DropDownData.ListOfddlData, function (i, Dimnesions) {
                options += "<option value='" + Dimnesions.Value + "'>";
                options += Dimnesions.Value;
                options += "</option>";
            });
            $("#Dimension7").html(options);
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