//Initialize imprest request
function InitializeImprestRequest() {
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

	//On change events
	AddOnChangeEvents();

	//Add dropdown search
	AddImprestRequestDropDownListSearch();
	//End add dropdown search
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

	//$("#TransactionType").change(function () {

	//	if ($(this).val() == "STAFF INTL TRAVEL" || $(this).val() == "BOARD INTL TRAVEL") {
	//		LoadSRCInternationalDestinations();
	//		LoadSalaryScales();
	//		GetEmployeeSalaryScale();
	//	}

	//	else {
	//		LoadSRCLocalDestinations();
	//		LoadSalaryScales();
	//		GetEmployeeSalaryScale();
	//	}

	//	LoadImprestElements($(this).val());
	//});
	//$("#ImprestCategory").change(function () {

	//	if ($(this).val() == "OTHER") {
	//		$("#LineAmount").css("background-color", "White");
	//		$("#LineAmount").removeAttr("disabled");

	//		$("#ImprestElement").css("background-color", "LightGray");
	//		$("#ImprestElement").attr("disabled", "disabled");
	//		$("#SRCDestination").css("background-color", "LightGray");
	//		$("#SRCDestination").attr("disabled", "disabled");
	//		$("#SalaryScale").css("background-color", "LightGray");
	//		$("#SalaryScale").attr("disabled", "disabled");

	//		GetEmployeeSalaryScale();
	//	}

	//	else if ($(this).val() == "PER DIEM") {

	//		$("#LineAmount").css("background-color", "LightGray");
	//		$("#LineAmount").attr("disabled", "disabled");

	//		$("#Days").css("background-color", "White");
	//		$("#Days").removeAttr("disabled");

	//		$("#ImprestElement").css("background-color", "White");
	//		$("#ImprestElement").removeAttr("disabled");

	//		$("#SRCDestination").css("background-color", "White");
	//		$("#SRCDestination").removeAttr("disabled");

	//		$("#SalaryScale").css("background-color", "White");
	//		$("#SalaryScale").removeAttr("disabled");

	//		GetEmployeeSalaryScale();
	//		LoadImprestElements($(this).val());
	//		GetImprestAmountPerCategory($("#TransactionType").val(), $(this).val(), $("#ImprestElement").val(), $("#SalaryScale").val(), $("#SRCDestination").val(), $("#Days").val());
	//	}

		//else if ($(this).val() == "TRANSPORT") {

		//	$("#ImprestElement").css("background-color", "LightGray");
		//	$("#ImprestElement").attr("disabled", "disabled");

		//	GetEmployeeSalaryScale();
		//	LoadImprestElements($(this).val());
		//}

		//else {
		//	$("#LineAmount").css("background-color", "LightGray");
		//	$("#LineAmount").attr("disabled", "disabled");

		//	$("#Days").css("background-color", "White");
		//	$("#Days").removeAttr("disabled");

		//	$("#ImprestElement").css("background-color", "White");
		//	$("#ImprestElement").removeAttr("disabled");

		//	$("#SRCDestination").css("background-color", "White");
		//	$("#SRCDestination").removeAttr("disabled");

		//	$("#SalaryScale").css("background-color", "White");
		//	$("#SalaryScale").removeAttr("disabled");
		//}

		//GetEmployeeSalaryScale();
		//GetImprestAmountPerCategory($("#TransactionType").val(), $(this).val(), $("#ImprestElement").val(), $("#SalaryScale").val(), $("#SRCDestination").val(), $("#Days").val());
		
	//});
	$("#SalaryScale").change(function () {
		var clusterDestination = $("#SRCDestination").val();
		//GetSRCAmountCopy($(this).val(), clusterDestination);
		GetImprestAmountPerCategory($("#TransactionType").val(), $("#ImprestCategory").val(), $("#ImprestElement").val(), $("#SalaryScale").val(), $("#SRCDestination").val(), $("#Days").val());
	});
	$("#ImprestElement").change(function () {
		//var clusterDestination = $("#SRCDestination").val();
		//GetSRCAmountCopy($(this).val(), clusterDestination);
		GetImprestAmountPerCategory($("#TransactionType").val(), $("#ImprestCategory").val(), $(this).val(), $("#SalaryScale").val(), $("#SRCDestination").val(), $("#Days").val());
	});
	$("#SRCDestination").change(function () {

		GetEmployeeSalaryScale();

		var salScale = $("#SalaryScale").val();
		//GetSRCAmountCopy(salScale, $(this).val());
		GetImprestAmountPerCategory($("#TransactionType").val(), $("#ImprestCategory").val(), $("#ImprestElement").val(), salScale, $(this).val(), $("#Days").val());
	});
	$("#Days").blur(function () {
		var lineAmount = 0;
		//Get SRC Amount
		$.ajax({
			url: AJAXUrls.GetImprestAmountPerCategory,
			//	url: AJAXUrls.GetSRCAmount,
			type: 'GET',
			//	data: { SRCScale: $("#SalaryScale").val(), Destination: $("#SRCDestination").val(), Qty: $(this).val() },
			data: { transactionType: $("#TransactionType").val(), imprestCategory: $("#ImprestCategory").val(), imprestElement: $("#ImprestElement").val(), kTNAJobGroup: $("#SalaryScale").val(), destination: $("#SRCDestination").val(), unit: $(this).val()},
			dataType: 'json',
			success: function (result) {
				lineAmount = result.LineAmount.toLocaleString();
				//$("#LineAmount").val(lineAmount);
			}
		});
	});
}

function AddImprestRequestDropDownListSearch() {

	$("#TransactionType").select2({
		placeholder: $("#TransactionTypeLbl").text(),
		allowClear: true
	});

	$("#ImprestCategory").select2({
		placeholder: $("#ImprestCategoryLbl").text(),
		allowClear: true
	});
	
	if ($("#GlobalDimension2Code").is("select")) {
		$("#GlobalDimension2Code").select2({
			placeholder: $("#GlobalDimension2CodeLbl").text(),
			allowClear: true
		});
	} 

	$("#SalaryScale").select2({
		placeholder: $("#SalaryScaleLbl").text(),
		allowClear: true
	});

	$("#SRCDestination").select2({
		placeholder: $("#SRCDestinationLbl").text(),
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
	
}

//load imprest elements
function LoadImprestElements(imprestCategory) {
	var options = "";
	options += "<option>";
	options += "";
	options += "</option>";

	$.ajax({
		url: AJAXUrls.GetImprestElements,
		type: "GET",
		dataType: "json",
		data: { imprestCategory: imprestCategory },
		cache: false,
		success: function (imprestElementList) {
			var rows = "";
			$.each(imprestElementList, function (i, imprestElementCode) {
				options += "<option value='" + imprestElementCode.Code + "'>";
				options += imprestElementCode.Description;
				options += "</option>";
			});
			$("#ImprestElement").html(options);
		}
	});
}

//load salary scales
function LoadSalaryScales() {
	var options = "";
	options += "<option>";
	options += "";
	options += "</option>";

	$.ajax({
		url: AJAXUrls.GetSalaryScales,
		type: "GET",
		dataType: "json",
		cache: false,
		success: function (srcList) {
			var rows = "";
			$.each(srcList, function (i, srcObj) {
				options += "<option value='" + srcObj.Code + "'>";
				options += srcObj.Name;
				options += "</option>";
			});
			$("#SalaryScale").html(options);
		}
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

        url: '/Imprest/GenerateReport',
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

//load SRC local destinations
function LoadSRCLocalDestinations() {
	var options = "";
	options += "<option>";
	options += "";
	options += "</option>";

	$.ajax({
		url: AJAXUrls.GetSRCLocalDestinations,
		type: "GET",
		dataType: "json",
		//data: { imprestCategory: imprestCategory },
		cache: false,
		success: function (srcInternationalDestinationList) {
			var rows = "";
			$.each(srcInternationalDestinationList, function (i, srcInternationalDestinationObj) {
				options += "<option value='" + srcInternationalDestinationObj.Code + "'>";
				options += srcInternationalDestinationObj.Name;
				options += "</option>";
			});
			$("#SRCDestination").html(options);
		}
	});
}

//load SRC international destinations
function LoadSRCInternationalDestinations() {
	var options = "";
	options += "<option>";
	options += "";
	options += "</option>";

	$.ajax({
		url: AJAXUrls.GetSRCInternationalDestinations,
		type: "GET",
		dataType: "json",
		//data: { imprestCategory: imprestCategory },
		cache: false,
		success: function (srcLocalDestinationList) {
			var rows = "";
			$.each(srcLocalDestinationList, function (i, srcLocalDestinationObj) {
				options += "<option value='" + srcLocalDestinationObj.Code + "'>";
				options += srcLocalDestinationObj.Name;
				options += "</option>";
			});
			$("#SRCDestination").html(options);
		}
	});
}

//Get employee SRC scale
function GetEmployeeSalaryScale() {
	$.ajax({
		url: AJAXUrls.GetEmployeeSalaryScale,
		type: "GET",
		dataType: "json",
		cache: false,
		success: function (result) {
			//$("#SalaryScale").val(result.SalaryScale);
			$('#SalaryScale').val(result.SalaryScale).trigger("change");
		}
	});
}

//Get imprest amount
function GetImprestAmount(DocumentNo) {
	var imprestAmount = 0;
	$.ajax({
		url: AJAXUrls.GetImprestAmount,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			imprestAmount = result.Amount.toLocaleString();
			$("#Amount").val(imprestAmount);
		},
		error: OnError
	});
}

//Get SRC amount
function GetSRCAmount(SRCScale,Destination) {
	var srcAmount = 0;
	$.ajax({
		url: AJAXUrls.GetSRCAmount,
		type: "GET",
		dataType: "json",
		data: { SRCScale: SRCScale, Destination: Destination, Qty: $("#Days").val() },
		cache: false,
		success: function (result) {
			srcAmount = result.LineAmount.toLocaleString();
			$("#LineAmount").val(srcAmount);
		},
		error: OnError
	});
}

//Get SRC amount copy
function GetSRCAmountCopy(salaryScale, clusterDestination) {
	var srcAmountCopy = 0;
	$.ajax({
		url: AJAXUrls.GetSRCAmountCopy,
		type: "GET",
		dataType: "json",
		data: { salaryScale: salaryScale, clusterDestination: clusterDestination},
		cache: false,
		success: function (result) {
			srcAmountCopy = result.LineAmount.toLocaleString();
			$("#LineAmount").val(srcAmountCopy);
		},
		error: OnError
	});
}

//Get imprest amount per category
function GetImprestAmountPerCategory(transactionType,imprestCategory,imprestElement,kTNAJobGroup,destination,unit) {
	var lineAmount = 0;
	$.ajax({
		url: AJAXUrls.GetImprestAmountPerCategory,
		type: "GET",
		dataType: "json",
		data: { transactionType: transactionType, imprestCategory: imprestCategory, imprestElement: imprestElement, kTNAJobGroup: kTNAJobGroup, destination: destination,unit:unit},
		cache: false,
		success: function (result) {
			lineAmount = result.LineAmount.toLocaleString();
			$("#LineAmount").val(lineAmount);
		},
		error: OnError
	});
}

//Load imprest lines
function LoadImprestLines(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetImprestLinesAjax,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (imprestLines) {
		    var rows = "";
			console.log(imprestLines)
		    $("#AddReqLine").css("display", "block");
			$.each(imprestLines, function (i, imprestLine) {
				rows += "<tr>";
				rows += "<td>" + imprestLine.TransactionType + "</td>";
				rows += "<td>" + imprestLine.Days + "</td>";
				/*rows += "<td>" + imprestLine.CurrencyCode + "</td>";*/
				rows += "<td>" + imprestLine.UnitPrice + "</td>";
				/*rows += "<td>" + imprestLine.UnitOfMeasure + "</td>";*/
				rows += "<td>" + imprestLine.LineAmount.toLocaleString() + "</td>";
				rows += "<td>" + imprestLine.LineDescription + "</td>";
				rows += "<td>" + imprestLine.Dimension1 + "</td>";
				rows += "<td>" + imprestLine.Dimension2 + "</td>";
				rows += "<td>" + imprestLine.Dimension3 + "</td>";
				rows += "<td>" + imprestLine.Dimension4 + "</td>";
				rows += "<td>" + imprestLine.Dimension5 + "</td>";
				rows += "<td>" + imprestLine.Dimension6 + "</td>";
				rows += "<td>" + imprestLine.Dimension7 + "</td>";
				if (imprestLine.Status == "Open" || imprestLine.Status == "Pending") {
				    rows += '<td><a href="#" onclick="return EditImprestLine(' + imprestLine.LineNo + ',\'' + imprestLine.DocumentNo + '\');">Edit</a> | <a href="#" onclick="DeleteImprestLine(' + imprestLine.LineNo + ',\'' + imprestLine.DocumentNo + '\')">Delete</a></td>';
				} else {
				    $("#AddReqLine").css("display", "none");
				}
				rows += "</tr>";
				$("#ImprestLineTbl tbody").html(rows);
			});
			$("#AjaxLoader").css("display", "none");
			$("#ImprestLineTbl").css("display", "block");
		},
		error: function (xhr, status, thrownError) {
			rows += "<tr>";
			rows += "<td class='text-danger text-center' colspan='8'>Unable to load the imprest lines.</td>";
			rows += "</tr>";
			$("#ImprestLineTbl tbody").html(rows);
			$("#AjaxLoader").css("display", "none");
			$("#ImprestLineTbl").css("display", "block");
		}
	});
}

function ViewImprestLines(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetImprestLinesAjax,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (imprestLines) {
			var rows = "";
			$.each(imprestLines, function (i, imprestLine) {
				rows += "<tr>";
				rows += "<td>" + imprestLine.TransactionType + "</td>";
				rows += "<td>" + imprestLine.Dimension1 + "</td>";
				rows += "<td>" + imprestLine.Dimension2 + "</td>";
				rows += "<td>" + imprestLine.Dimension3 + "</td>";
				rows += "<td>" + imprestLine.Dimension4 + "</td>";
				rows += "<td>" + imprestLine.Dimension5 + "</td>";
				rows += "<td>" + imprestLine.Dimension6 + "</td>";
				rows += "<td>" + imprestLine.Dimension7 + "</td>";
				rows += "<td>" + imprestLine.Days + "</td>";
				rows += "<td>" + imprestLine.LineAmount.toLocaleString() + "</td>";
				rows += "<td>" + imprestLine.LineDescription + "</td>";
				rows += '<td><a href="#" onclick="return ViewImprestLine(' + imprestLine.LineNo + ',\'' + imprestLine.DocumentNo + '\');">View</a> </td>';
				rows += "</tr>";
				$("#ImprestLineTbl tbody").html(rows);
			});
			$("#AjaxLoader").css("display", "none");
			$("#ImprestLineTbl").css("display", "block");
		},
		error: function (xhr, status, thrownError) {
			rows += "<tr>";
			rows += "<td class='text-danger text-center' colspan='8'>Unable to load the imprest lines.</td>";
			rows += "</tr>";
			$("#ImprestLineTbl tbody").html(rows);
			$("#AjaxLoader").css("display", "none");
			$("#ImprestLineTbl").css("display", "block");
		}
	});
}

//Create imprest line   
function CreateImprestLine() {

    var documentNo = $("#No").val();
    var datefrom = $("#DateFrom").val();
    var dateTo = $("#DateTo").val();
    var headerdesc = $("#Description").val();
    var createpv = $("#CreatePV").val();
    if (headerdesc == "") {
        window.alert("Kindly add the header description first");
    }
    if (datefrom == "") {
        window.alert("Kindly add the from date first");
    }
    if (dateTo == "") {
        window.alert("Kindly add the to date first");
    }
	var validLine = ValidateImprestLine();
	if (validLine == false) {
		return false;
	}

	var ImprestLineObj = {
		DocumentNo: documentNo,
		TransactionType: $("#TransactionType").val(),
		LineDescription: $("#LineDescription").val(),
		Days: $("#Days").val(),
		LineAmount: $("#LineAmount").val(),
		LineAmountLCY: $("#LineAmountLCY").val(),
		CurrencyCode: $("#CurrencyCode").val(),
		UnitOfMeasure: $("#UnitOfMeasure").val(),
		UnitPrice:$("#UnitPrice").val(),
		LineGlobalDimension1Code: $("#Dimension1").val(),
		LineGlobalDimension2Code: $("#Dimension2").val(),
		LineShortcutDimension3Code: $("#Dimension3").val(),
		LineShortcutDimension4Code: $("#Dimension4").val(),
		LineShortcutDimension5Code: $("#Dimension5").val(),
		LineShortcutDimension6Code: $("#Dimension6").val(),
		LineShortcutDimension7Code: $("#Dimension7").val(),
	    StartDate: datefrom,
	    EndDate: dateTo,
	    HeaderDescripton: headerdesc
		//LineShortcutDimension8Code: $("#LineShortcutDimension8Code").val()
	};
	console.log(ImprestLineObj)

	$.ajax({
		url: AJAXUrls.CreateImprestLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(ImprestLineObj),
		cache: false,
		success: function (result) {
			if (result.success) {
				$('#errorMessage').hide();
				$('#successMessage').show();
				LoadImprestLines(documentNo);
				GetImprestAmount(documentNo);
				$("#ImprestLineModal").modal("hide");
			} else {
				$('#errorMessage').html(result.message);
				$('#errorMessage').show();
				$('#successMessage').hide();
				Swal.fire('Warning', result.message, 'warning');
			}
		},
		error: function (xhr, errorType, exception) {
			alert(xhr.responseText);
		}
	});
}

function CreateImprestLineForAdvance() {

	var documentNo = $("#No").val();
	var datefrom = $("#DateFrom").val();
	var dateTo = $("#DateTo").val();
	var headerdesc = $("#PurposeOfImprest").val();
	var createpv = $("#CreatePV").val();
	if (headerdesc == "") {
		window.alert("Kindly add the header description first");
	}
	var validLine = ValidateImprestLine();
	if (validLine == false) {
		return false;
	}

	var ImprestLineObj = {
		DocumentNo: documentNo,
		TransactionType: $("#TransactionType").val(),
		LineDescription: $("#LineDescription").val(),
		Days: $("#Days").val(),
		LineAmount: $("#LineAmount").val(),
		LineGlobalDimension1Code: $("#Dimension1").val(),
		LineGlobalDimension2Code: $("#Dimension2").val(),
		LineShortcutDimension3Code: $("#Dimension3").val(),
		LineShortcutDimension4Code: $("#Dimension4").val(),
		LineShortcutDimension5Code: $("#Dimension5").val(),
		LineShortcutDimension6Code: $("#Dimension6").val(),
		LineShortcutDimension7Code: $("#Dimension7").val(),
		StartDate: datefrom,
		EndDate: dateTo,
		HeaderDescripton: headerdesc
		//LineShortcutDimension8Code: $("#LineShortcutDimension8Code").val()
	};
	console.log(ImprestLineObj)

	$.ajax({
		url: '/Imprest/CreateImprestLineForAdvance',
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(ImprestLineObj),
		cache: false,
		success: function (result) {
			if (result.success) {
				$('#errorMessage').hide();
				$('#successMessage').show();
				LoadImprestLines(documentNo);
				GetImprestAmount(documentNo);
				$("#ImprestLineModal").modal("hide");
			} else {
				Swal.fire(result.message);
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

//Edit imprest line
function EditImprestLine(LineNo, DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetImprestLineByLineNo,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			console.log(result);
			$('#LineNo').val(result.LineNo);
			$('#DocumentNo').val(result.DocumentNo);
			$('#TransactionType').val(result.TransactionType).trigger("change");
			$('#ImprestCategory').val(result.ImprestCategory).trigger("change");
			$('#ImprestElement').val(result.ImprestElement).trigger("change");
			$('#SalaryScale').val(result.SalaryScale).trigger("change");
			$('#SRCDestination').val(result.SRCDestination).trigger("change");
			$("#Days").val(result.Days);
			$("#UnitPrice").val(result.UnitPrice);
			$('#LineDescription').val(result.LineDescription).trigger("change");
			$('#LineAmount').val(result.LineAmount);
			$('#LineGlobalDimension1Code').val(result.LineGlobalDimension1Code).trigger("change");
			$('#LineGlobalDimension2Code').val(result.LineGlobalDimension2Code).trigger("change");
			$('#LineShortcutDimension3Code').val(result.LineShortcutDimension3Code).trigger("change");
			$('#LineShortcutDimension4Code').val(result.LineShortcutDimension4Code).trigger("change");
			$('#LineShortcutDimension5Code').val(result.LineShortcutDimension5Code).trigger("change");
			$('#LineShortcutDimension6Code').val(result.LineShortcutDimension6Code).trigger("change");
			$('#LineShortcutDimension7Code').val(result.LineShortcutDimension7Code).trigger("change");
			$('#LineShortcutDimension8Code').val(result.LineShortcutDimension8Code).trigger("change");
			$('#CurrencyCode').val(result.CurrencyCode).trigger("change");
			$('#UnitOfMeasure').val(result.UnitOfMeasure).trigger("change");

			$("#ImprestLineModal").modal("show");
			$("#CreateImprestLineBtn").hide();
			$("#ModifyImprestLineBtn").show();
		},
		error: OnError
	});
	return false;
}

//Modify imprest line
function ModifyImprestLine(checkDate =true) {

    var documentNo = $("#No").val();
    var datefrom = $("#DateFrom").val();
    var dateTo = $("#DateTo").val();
    var headerdesc = $("#Description").val();
	console.log(checkDate)
    if (headerdesc == "") {
        window.alert("Kindly add the header description first");
    }
    if (datefrom == "" && checkDate == true) {
        window.alert("Kindly add the from date first");
    }
    if (dateTo == "" && checkDate == true) {
        window.alert("Kindly add the to date first");
    }
	var validLine = ValidateImprestLine();
	if (validLine == false) {
		return false;
	}

	var imprestLineObj = {
		LineNo: $("#LineNo").val(),
		DocumentNo: documentNo,
		TransactionType: $("#TransactionType").val(),
		LineDescription: $("#LineDescription").val(),
		Days: $("#Days").val(),
		CurrencyCode: $("#CurrencyCode").val(),
		UnitOfMeasure: $("#UnitOfMeasure").val(),
		UnitPrice:$("#UnitPrice").val(),
		LineAmount: $("#LineAmount").val(),
		LineGlobalDimension1Code: $("#Dimension1").val(),
		LineGlobalDimension2Code: $("#Dimension2").val(),
		LineShortcutDimension3Code: $("#Dimension3").val(),
		LineShortcutDimension4Code: $("#Dimension4").val(),
		LineShortcutDimension5Code: $("#Dimension5").val(),
		LineShortcutDimension6Code: $("#Dimension6").val(),
		LineShortcutDimension7Code: $("#Dimension7").val(),
		StartDate: datefrom,
		EndDate: dateTo,
		HeaderDescripton: headerdesc,
		checkDate: checkDate
	};
	$.ajax({
		url: AJAXUrls.ModifyImprestLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(imprestLineObj),
		cache: false,
		success: function (result) {
			if (result.success) {
				LoadImprestLines(documentNo);
				GetImprestAmount(documentNo);
				$("#ImprestLineModal").modal("hide");

				ClearImprestLineModal();
			}
			else
				Swal.fire('error', result.message, 'error');
		},
		error: OnError
	});
}

//View imprest line
function ViewImprestLine(LineNo, DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetImprestLineByLineNo,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			$("#LineNo").val(result.LineNo);
			$("#DocumentNo").val(result.DocumentNo);
			$("#TransactionType").val(result.TransactionType).trigger("change");
			$('#ImprestCategory').val(result.ImprestCategory).trigger("change");
			$('#ImprestElement').val(result.ImprestElement).trigger("change");
			$('#SalaryScale').val(result.SalaryScale).trigger("change");
			$('#SRCDestination').val(result.SRCDestination).trigger("change");
			$("#Days").val(result.Days);
			$("#UnitPrice").val(result.UnitPrice);
			$("#LineDescription").val(result.LineDescription).trigger("change");
			$("#LineAmount").val(result.LineAmount.toLocaleString());
			$("#LineGlobalDimension1Code").val(result.LineGlobalDimension1Code).trigger("change");
			$("#LineGlobalDimension2Code").val(result.LineGlobalDimension2Code).trigger("change");
			$("#LineShortcutDimension3Code").val(result.LineShortcutDimension3Code).trigger("change");
			$("#LineShortcutDimension4Code").val(result.LineShortcutDimension4Code).trigger("change");
			$("#LineShortcutDimension5Code").val(result.LineShortcutDimension5Code).trigger("change");
			$("#LineShortcutDimension6Code").val(result.LineShortcutDimension6Code).trigger("change");
			$("#LineShortcutDimension7Code").val(result.LineShortcutDimension7Code).trigger("change");
			$("#LineShortcutDimension8Code").val(result.LineShortcutDimension8Code).trigger("change");
			$("#CurrencyCode").val(result.CurrencyCode).trigger("change");
			$("#UnitofMeasure").val(result.UnitofMeasure).trigger("change");

			$("#ImprestLineModal").modal('show');
		},
		error: OnError
	});
	return false;
}

//Delete imprest line
function DeleteImprestLine(LineNo, DocumentNo) {
	var ans = confirm("Are you sure you want to delete this Line?");
	if (ans) {
		$.ajax({
			url: AJAXUrls.DeleteImprestLine,
			type: "POST",
			dataType: "json",
			data: { LineNo: LineNo, DocumentNo: DocumentNo },
			cache: false,
			success: function (result) {
				LoadImprestLines(DocumentNo);
				GetImprestAmount(DocumentNo);
			},
			error: function (errormessage) {
				//alert(errormessage.responseText);
				alert("Error");
			}
		});
	}
}

function ValidateImprestHeader() {

	//Clear imprest line modal
	ClearImprestLineModal();

	var isValid = true;
	if ($('#No').val().trim() == "") {
		$('#No').css('border-color', 'Red');
		isValid = false;
	}
	else {
		$('#No').css('border-color', 'lightgrey');
	}

	//if ($('#GlobalDimension1Code').val().trim() == "") {
	//	$("#GlobalDimension1CodeError").show();
	//	isValid = false;
	//}
	//else {
	//	$('#errorGlobalDimension1Code').hide();
	//}

	//if ($.isNumeric($("#Amount").val())) {
	//	$("#AmountError").text("");
	//} else {
	//	("#AmountError").text("Imprest amount must be numeric.");
	//	isValid = false;
	//}

	//if (($("#Amount").val() <= 0) || ($("#Amount").val().trim() == "")) {
	//	$("#AmountError").text("Imprest amount cannot be less or equal to zero.");
	//	isValid = false;
	//}
	//else {
	//	$("#AmountError").text("");
	//}

	return isValid;
}

function ValidateImprestLine() {
	var isValid = true;
	var label = "";
	if ($("#TransactionType").val().trim() == "") {
		$("#TransactionTypeError").text("Transaction Type cannot be empty.");
		isValid = false;
	}
	else {
		$("#TransactionTypeError").text("");
	}

	//if ($("#ImprestCategory").val().trim() == "") {
	//	$("#ImprestCategoryError").text("Imprest Category cannot be empty.");
	//	isValid = false;
	//}
	//else {
	//	$("#ImprestCategoryError").text("");
	//}

	//if ($("#ImprestElement").val().trim() == "") {
	//	$("#ImprestElementError").text("Imprest Element cannot be empty.");
	//	isValid = false;
	//}
	//else {
	//	$("#ImprestElementError").text("");
	//}

	//if ($("#Destination").val().trim() == "") {
	//	$("#DestinationError").text("Destination cannot be empty.");
	//	isValid = false;
	//}
	//else {
	//	$("#DestinationError").text("");
	//}

	if ($("#LineDescription").val().trim() == "") {
		$("#LineDescriptionError").text("Imprest line description cannot be empty.");
		isValid = false;
	}
	else {
		$("#LineDescriptionError").text("");
	}

	//if ($.isNumeric($("#LineAmount").val())) {
	//	$("#LineAmountError").text("");
	//} else {
	//	("#LineAmountError").text("Imprest line amount must be numeric.");
	//	isValid = false;
	//}
	console.log($("#LineAmount").val());
	if (($("#LineAmount").val() <= 0) || ($("#LineAmount").val().trim() == "")) {
		$("#LineAmountError").text("Imprest line amount cannot be less or equal to zero.");
		isValid = false;
	}
	else {
		$("#LineAmountError").text("");
	}

	//if ($.isNumeric($("#Days").val())) {
	//	$("#DaysError").text("");
	//} else {
	//	("#DaysError").text("Days must be numeric.");
	//	isValid = false;
	//}

	if (($("#Days").val() <= 0) || ($("#Days").val().trim() == "")) {
		$("#DaysError").text("Days cannot be less or equal to zero.");
		isValid = false;
	}
	else {
		$("#DaysError").text("");
	}

	//if ($("#LineGlobalDimension2Code").val().trim() == "") {
	//    label = $("#LineGlobalDimension2CodeLbl").text();
	//    $("#LineGlobalDimension2CodeError").text(label + " cannot be empty.");
	//    isValid = false;
	//}
	//else {
	//    $("#LineGlobalDimension2CodeError").text("");
	//}

	//if ($("#LineShortcutDimension3Code").val().trim() == "") {
	//    label = $("#LineShortcutDimension3CodeLbl").text();
	//    $("#LineShortcutDimension3CodeError").text(label + " cannot be empty.");
	//    isValid = false;
	//}
	//else {
	//    $("#LineShortcutDimension3CodeError").text("");
	//}

	//if ($("#LineShortcutDimension4Code").val().trim() == "") {
	//    label = $("#LineShortcutDimension4CodeLbl").text();
	//    $("#LineShortcutDimension4CodeError").text(label + " cannot be empty.");
	//    isValid = false;
	//}
	//else {
	//    $("#LineShortcutDimension4CodeError").text("");
	//}

	//if ($("#LineShortcutDimension5Code").val().trim() == "") {
	//    label = $("#LineShortcutDimension5CodeLbl").text();
	//    $("#LineShortcutDimension5CodeError").text(label + " cannot be empty.");
	//    isValid = false;
	//}
	//else {
	//    $("#LineShortcutDimension5CodeError").text("");
	//}

	//if ($("#LineShortcutDimension6Code").val().trim() == "") {
	//    label = $("#LineShortcutDimension6CodeLbl").text();
	//    $("#LineShortcutDimension6CodeError").text(label + " cannot be empty.");
	//    isValid = false;
	//}
	//else {
	//    $("#LineShortcutDimension6CodeError").text("");
	//}
	return isValid;
}

//Clear imprest line modal
function ClearImprestLineModal() {
	$("#LineNo").val(0);
	$("#DocumentNo").val("");
	$("#TransactionType").val("").trigger("change");
	$("#ImprestCategory").val("").trigger("change");
	$("#ImprestElement").val("").trigger("change");
	$("#SRCDestination").val("").trigger("change");
	$("#SalaryScale").val("").trigger("change");
	$("#LineDescription").val("");
	$("#LineAmount").val(0);
	$("#FromCity").val("").trigger("change");
	$("#ToCity").val("").trigger("change");
	$("#Days").val("");
	$("#LineGlobalDimension1Code").val("").trigger("change");
	$("#LineGlobalDimension2Code").val("").trigger("change");
	$("#LineShortcutDimension3Code").val("").trigger("change");
	$("#LineShortcutDimension4Code").val("").trigger("change");
	$("#LineShortcutDimension5Code").val("").trigger("change");
	$("#LineShortcutDimension6Code").val("").trigger("change");
	$("#LineShortcutDimension7Code").val("").trigger("change");
	$("#LineShortcutDimension8Code").val("").trigger("change");

	$("#CreateImprestLineBtn").show();
	$("#ModifyImprestLineBtn").hide();

	$("#CreateFundsClaimLineBtn").show();
	$("#ModifyFundsClaimLineBtn").hide();

	$("#TransactionTypeError").text("");
	$("#ImprestCategoryError").text("");
	$("#ImprestElementError").text("");
	$("#SRCDestinationError").text("");
	$("#SalaryScaleError").text("");
	$("#LineDescriptionError").text("");
	$("#LineAmountError").text("");
	$("#DaysError").text("");
	$("#LineGlobalDimension1CodeError").text("");
	$("#LineGlobalDimension2CodeError").text("");
	$("#LineShortcutDimension3CodeError").text("");
	$("#LineShortcutDimension4CodeError").text("");
	$("#LineShortcutDimension5CodeError").text("");
	$("#LineShortcutDimension6CodeError").text("");
	$("#LineShortcutDimension7CodeError").text("");
	$("#LineShortcutDimension8CodeError").text("");
}

//Load Funds Claim lines
function LoadFundsClaimLines(DocumentNo) { 
	$.ajax({
		url: AJAXUrls.GetFundClaimLines,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (imprestLines) {
			var rows = "";
			$.each(imprestLines, function (i, imprestLine) {
				rows += "<tr>";
				rows += "<td>" + imprestLine.ImprestCode + "</td>";
				rows += "<td>" + imprestLine.LineDescription + "</td>";
				rows += "<td>" + imprestLine.LineAmount.toLocaleString() + "</td>";
				//rows += "<td>" + imprestLine.FromCity + "</td>";
				//rows += "<td>" + imprestLine.ToCity + "</td>";
				rows += '<td><a href="#" onclick="return EditFundsClaimLine(' + imprestLine.LineNo + ',\'' + imprestLine.DocumentNo + '\');">Edit</a> | <a href="#" onclick="DeleteFundsClaimLine(' + imprestLine.LineNo + ',\'' + imprestLine.DocumentNo + '\')">Delete</a></td>';
				rows += "</tr>";
				$("#FundsClaimLineTbl tbody").html(rows);
			});
			$("#AjaxLoader").css("display", "none");
			$("#FundsClaimLineTbl").css("display", "block");
		},
		error: function (xhr, status, thrownError) {
			rows += "<tr>";
			rows += "<td class='text-danger text-center' colspan='8'>Unable to load the funds claim lines.</td>";
			rows += "</tr>";
			$("#FundsClaimLineTbl tbody").html(rows);
			$("#AjaxLoader").css("display", "none");
			$("#FundsClaimLineTbl").css("display", "block");
		}
	});
}

//Load Funds Claim lines View
function LoadFundsClaimLinesView(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetFundClaimLines,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (imprestLines) {
			var rows = "";
			$.each(imprestLines, function (i, imprestLine) {
				rows += "<tr>";
				rows += "<td>" + imprestLine.ImprestCode + "</td>";
				rows += "<td>" + imprestLine.LineDescription + "</td>";
				rows += "<td>" + imprestLine.LineAmount.toLocaleString() + "</td>";
				//rows += "<td>" + imprestLine.FromCity + "</td>";
				//rows += "<td>" + imprestLine.ToCity + "</td>";
				rows += '<td><a href="#" onclick="return ViewFundsClaimLine(' + imprestLine.LineNo + ',\'' + imprestLine.DocumentNo + '\');">View</a> </td>';
				rows += "</tr>";
				$("#FundsClaimLineTbl tbody").html(rows);
			});
			$("#AjaxLoader").css("display", "none");
			$("#FundsClaimLineTbl").css("display", "block");
		},
		error: function (xhr, status, thrownError) {
			rows += "<tr>";
			rows += "<td class='text-danger text-center' colspan='8'>Unable to load the funds claim lines.</td>";
			rows += "</tr>";
			$("#FundsClaimLineTbl tbody").html(rows);
			$("#AjaxLoader").css("display", "none");
			$("#FundsClaimLineTbl").css("display", "block");
		}
	});
}

//Create Funds Claim line
function CreateFundsClaimLine() {

	var documentNo = $("#No").val();

	var validLine = ValidateFundsClaimLine();
	if (validLine == false) {
		return false;
	}

	var FundClaimLineObj = {
		DocumentNo: documentNo,
		ImprestCode: $("#ImprestCode").val(),
		LineDescription: $("#LineDescription").val(),
		LineAmount: $("#LineAmount").val(),
		FromCity: $("#FromCity").val(),
		ToCity: $("#ToCity").val(),
		LineGlobalDimension1Code: $("#LineGlobalDimension1Code").val(),
		LineGlobalDimension2Code: $("#LineGlobalDimension2Code").val(),
		LineShortcutDimension3Code: $("#LineShortcutDimension3Code").val(),
		LineShortcutDimension4Code: $("#LineShortcutDimension4Code").val(),
		LineShortcutDimension5Code: $("#LineShortcutDimension5Code").val(),
		LineShortcutDimension6Code: $("#LineShortcutDimension6Code").val(),
		LineShortcutDimension7Code: $("#LineShortcutDimension7Code").val(),
		LineShortcutDimension8Code: $("#LineShortcutDimension8Code").val()
	};

	$.ajax({
		url: AJAXUrls.CreateFundClaimLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(FundClaimLineObj),
		cache: false,
		success: function (result) {
			LoadFundsClaimLines(documentNo);
			GetImprestAmount(documentNo);
			$("#FundsClaimLineModal").modal("hide");
		},
		error: function (xhr, errorType, exception) {
			alert(xhr.responseText);
		}
	});
}

//Edit Funds Claim line
function EditFundsClaimLine(LineNo, DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetFundClaimLineByLineNo, 
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo },
		cache: false,
		success: function (result) {
			$('#LineNo').val(result.LineNo);
			$('#DocumentNo').val(result.DocumentNo);
			$('#ImprestCode').val(result.ImprestCode).trigger("change");
			$('#LineDescription').val(result.LineDescription).trigger("change");
			$('#LineAmount').val(result.LineAmount);
			$('#FromCity').val(result.FromCity).trigger("change");
			$('#ToCity').val(result.ToCity).trigger("change");
			$('#LineGlobalDimension1Code').val(result.LineGlobalDimension1Code).trigger("change");
			$('#LineGlobalDimension2Code').val(result.LineGlobalDimension2Code).trigger("change");
			$('#LineShortcutDimension3Code').val(result.LineShortcutDimension3Code).trigger("change");
			$('#LineShortcutDimension4Code').val(result.LineShortcutDimension4Code).trigger("change");
			$('#LineShortcutDimension5Code').val(result.LineShortcutDimension5Code).trigger("change");
			$('#LineShortcutDimension6Code').val(result.LineShortcutDimension6Code).trigger("change");
			$('#LineShortcutDimension7Code').val(result.LineShortcutDimension7Code).trigger("change");
			$('#LineShortcutDimension8Code').val(result.LineShortcutDimension8Code).trigger("change");

			$("#FundsClaimLineModal").modal("show");
			$("#CreateFundsClaimLineBtn").hide();
			$("#ModifyFundsClaimLineBtn").show();
		},
		error: OnError
	});
	return false;
}

//Modify Funds Claim line
function ModifyFundsClaimLine() {
	var documentNo = $("#No").val();

	var validLine = ValidateFundsClaimLine();
	if (validLine == false) {
		return false;
	}

	var FundClaimLineObj = {
		LineNo: $("#LineNo").val(),
		DocumentNo: documentNo,
		ImprestCode: $("#ImprestCode").val(),
		LineDescription: $("#LineDescription").val(),
		LineAmount: $("#LineAmount").val(),
		FromCity: $("#FromCity").val(),
		ToCity: $("#ToCity").val(),
		LineGlobalDimension1Code: $("#GlobalDimension1Code").val(),
		LineGlobalDimension2Code: $("#LineGlobalDimension2Code").val(),
		LineShortcutDimension3Code: $("#LineShortcutDimension3Code").val(),
		LineShortcutDimension4Code: $("#LineShortcutDimension4Code").val(),
		LineShortcutDimension5Code: $("#LineShortcutDimension5Code").val(),
		LineShortcutDimension6Code: $("#LineShortcutDimension6Code").val(),
		LineShortcutDimension7Code: $("#LineShortcutDimension7Code").val(),
		LineShortcutDimension8Code: $("#LineShortcutDimension8Code").val()
	};
	$.ajax({
		url: AJAXUrls.ModifyFundClaimLine,
		type: "POST",
		dataType: "json",
		contentType: "application/json",
		data: JSON.stringify(FundClaimLineObj),
		cache: false,
		success: function (result) {
			LoadFundsClaimLines(documentNo);
			GetImprestAmount(documentNo);
			$("#FundsClaimLineModal").modal("hide");

			ClearImprestLineModal();
		},
		error: OnError
	});
}

//Delete Funds Claim Line
function DeleteFundsClaimLine(LineNo, DocumentNo) {
	var ans = confirm("Are you sure you want to delete this Line?");
	if (ans) {
		$.ajax({
			url: AJAXUrls.DeleteFundClaimLine,
			type: "POST",
			dataType: "json",
			data: { LineNo: LineNo, DocumentNo: DocumentNo },
			cache: false,
			success: function (result) {
				LoadFundsClaimLines(DocumentNo);
				GetImprestAmount(DocumentNo);
			},
			error: function (errormessage) {
				alert("Error");
			}
		});
	}
}

//Validate Funds Claim Line
function ValidateFundsClaimLine() {
	var isValid = true;
	var label = "";
	if ($("#ImprestCode").val().trim() == "") {
		$("#ImprestCodeError").text("Imprest code cannot be empty.");
		isValid = false;
	}
	else {
		$("#ImprestCodeError").text("");
	}

	//if ($("#FromCity").val().trim() == "") {
	//	$("#FromCityError").text("Specify the location you are traveling from.");
	//	isValid = false;
	//}
	//else {
	//	$("#FromCityError").text("");
	//}

	//if ($("#ToCity").val().trim() == "") {
	//	$("#ToCityError").text("Specify the location you are traveling to.");
	//	isValid = false;
	//}
	//else {
	//	$("#ToCityError").text("");
	//}

	if ($("#LineDescription").val().trim() == "") {
		$("#LineDescriptionError").text("Imprest line description cannot be empty.");
		isValid = false;
	}
	else {
		$("#LineDescriptionError").text("");
	}

	if ($.isNumeric($("#LineAmount").val())) {
		$("#LineAmountError").text("");
	} else {
		("#LineAmountError").text("Imprest line amount must be numeric.");
		isValid = false;
	}

	if (($("#LineAmount").val() <= 0) || ($("#LineAmount").val().trim() == "")) {
		$("#LineAmountError").text("Imprest line amount cannot be less or equal to zero.");
		isValid = false;
	}
	else {
		$("#LineAmountError").text("");
	}
	return isValid;
}

//Load Imprest Documents
function LoadImprestDocuments(DocumentNo) {
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
//Load PV Documents
function LoadPVDocuments(DocumentNo) {
    var DocNo = DocumentNo;
    var Status = $("#Status").val();
    $.ajax({
        async: true,
        type: "POST",
        datatype: "json",
        contentType: "application/json; charset = utf-8",
        processData: false,
        data: JSON.stringify({ DocNo: DocNo, TableID: 51525000, Status: Status }),
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
                data: JSON.stringify({ DocNo: No, DocID: Id, tblID: tbl }),
                contentType: "application/json; charset = utf-8",
                success: function (data) {
                    if (data.success == true) {
                        LoadImprestDocuments(No);
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

//Load Imprest Documents View
function LoadImprestDocumentsView(DocumentNo) {
	$.ajax({
		url: AJAXUrls.GetImprestPortalDocuments,
		type: "GET",
		dataType: "json",
		data: { DocumentNo: DocumentNo },
		cache: false,
		success: function (imprestUploadedDocuments) {
			var rows = "";
			$.each(imprestUploadedDocuments, function (i, imprestUploadedDocument) {
				rows += "<tr>";
				rows += "<td>" + imprestUploadedDocument.DocumentDescription + "</td>";
				//if (imprestUploadedDocument.DocumentAttached) {
				//	rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' checked disabled>" + "</td>";
				//} else {
				//	rows += "<td>" + "<input type='checkbox' name='DocumentAttached' value='DocumentAttached' disabled>" + "</td>";
				//}

				rows += '<td><a href="#" onclick="return ViewImprestDocument(\'' + DocumentNo + '\',\'' + imprestUploadedDocument.DocumentCode + '\');"><i class="" aria-hidden="true">View</i></a></td>';
				rows += "</tr>";
			});

			$("#ImprestAjaxLoader").css("display", "none");
			$("#ImprestDocumentsTbl tbody").html(rows);
			$("#ImprestDocumentsTbl").css("display", "block");
		},
		error: function (xhr, status, error) {

		}
	});
}

//Edit imprest document
function EditImprestDocument(LineNo,DocumentNo, DocumentCode) {

	ResetImprestDocumentModal();

	$.ajax({
		url: AJAXUrls.GetImprestPortalDocumentLink,
		type: "GET",
		dataType: "json",
		data: { LineNo: LineNo, DocumentNo: DocumentNo, DocumentCode: DocumentCode },
		cache: false,
		success: function (applicationDocument) {
			$("#DocumentNo").val(applicationDocument.DocumentNo);
			$("#DocumentCode").val(applicationDocument.DocumentCode);
			$("#DocumentDescription").val(applicationDocument.DocumentDescription);
			$('#errorMessage').hide();
			$("#ImprestDocumentModal").modal("show");
			$("#UploadImprestDocumentBtn").show();
		},
		error: function (xhr, status, error) {

		}
	});
	return false;
}

//Reset imprest document Line
function ResetImprestDocumentModal() {
	$("#ImprestDocumentModal").val("");
	//Ladda.stopAll();
}

//Upload imprest document
function UploadImprestDocument() {
	var DocumentNo = $("#No").val();
	var DocumentCode = $("#DocumentCode").val();
	var DocumentDescription = $("#DocumentDescription").val();

	var filebase = $("#ImprestDocumentFile").get(0);
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
		url: AJAXUrls.UploadImprestDocumentLink,
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
				LoadImprestDocuments(DocumentNo);
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

//load 2nd dimension
function loaddimension2(dimension1) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetGlobalDimension2Codes,
        type: "GET",
        dataType: "json",
        data: { dimension1: dimension1 },
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

function ViewPVLines(DocumentNo) {
    $.ajax({
        url: AJAXUrls.GetPVLinesAjax,
        type: "GET",
        dataType: "json",
        data: { DocumentNo: DocumentNo },
        cache: false,
        success: function (imprestLines) {
            var rows = "";
            $.each(imprestLines, function (i, imprestLine) {
                rows += "<tr>";
                rows += "<td>" + imprestLine.AccountType + "</td>";
                rows += "<td>" + imprestLine.AccountNo + "</td>";
                rows += "<td>" + imprestLine.Dimension1 + "</td>";
                rows += "<td>" + imprestLine.Dimension2 + "</td>";
                rows += "<td>" + imprestLine.Dimension3 + "</td>";
                rows += "<td>" + imprestLine.Dimension4 + "</td>";
                rows += "<td>" + imprestLine.Dimension5 + "</td>";
                rows += "<td>" + imprestLine.Dimension6 + "</td>";
                rows += "<td>" + imprestLine.Dimension7 + "</td>";
                rows += "<td>" + imprestLine.LineAmount.toLocaleString() + "</td>";
                rows += "<td>" + imprestLine.LineDescription + "</td>";
                rows += "</tr>";
                $("#ImprestLineTbl tbody").html(rows);
            });
            $("#AjaxLoader").css("display", "none");
            $("#ImprestLineTbl").css("display", "block");
        },
        error: function (xhr, status, thrownError) {
            rows += "<tr>";
            rows += "<td class='text-danger text-center' colspan='8'>Unable to load the imprest lines.</td>";
            rows += "</tr>";
            $("#ImprestLineTbl tbody").html(rows);
            $("#AjaxLoader").css("display", "none");
            $("#ImprestLineTbl").css("display", "block");
        }
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