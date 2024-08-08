function LoadDimensionValues() {
    //Load dimension Codes
    $("#GlobalDimension1Code").change(function () {
        var globalDimension1Code = "";
        LoadGlobalDimension2Values(globalDimension1Code);
        LoadShortcutDimension3Values(globalDimension1Code);
        LoadShortcutDimension4Values(globalDimension1Code);
        LoadShortcutDimension5Values(globalDimension1Code);
        LoadShortcutDimension6Values(globalDimension1Code);
    });

    //Add dropdown search
    AddDimensionsDropDownListSearch()
    //End add dropdown search
}

//Load global dimension 1 values
function LoadGlobalDimension1Values() {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetGlobalDimension1Codes,
        type: 'GET',
        dataType: 'json',
        success: function (dimension1Values) {         
            $.each(dimension1Values, function (i, dimension1Value) {
                options += "<option value='" + dimension1Value.Code + "'>";
                options += dimension1Value.Name;
                options += "</option>";
            });
            $("#GlobalDimension1Code").html(options);
            $("#LineGlobalDimension1Code").html(options);
        },
        error: function (xhr, status, thrownError) {
            var label = $("#lblGlobalDimension1Code").text();
            options += "<option>";
            options += "Unable to load "+label+" values";
            options += "</option>";
            $("#GlobalDimension1Code").html(options);
            $("#LineGlobalDimension1Code").html(options);
        }
    });
    
}

//Load global dimension 2 values
function LoadGlobalDimension2Values(GlobalDimension1Code) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetGlobalDimension2Codes,
        type: 'GET',
        dataType: 'json',
        data: { GlobalDimension1Code: GlobalDimension1Code },       
        success: function (dimension2Values) {
            $.each(dimension2Values, function (i, dimension2Value) {
                options += "<option value='" + dimension2Value.Code + "'>";
                options += dimension2Value.Name;
                options += "</option>";
            });
            $("#GlobalDimension2Code").html(options);
            $("#LineGlobalDimension2Code").html(options);
        },
        error: function (xhr, status, thrownError) {
            var label = $('#lblGlobalDimension2Code').text();
            options += "<option>";
            options += "Unable to load " + label + " values";
            options += "</option>";
            $("#GlobalDimension2Code").html(options);
            $("#LineGlobalDimension2Code").html(options);
        }
    }); 
}

//Load shortcut dimension 3 values
function LoadShortcutDimension3Values(GlobalDimension1Code) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetShortcutDimension3Codes,
        type: 'GET',
        dataType: 'json',
        data: { GlobalDimension1Code: GlobalDimension1Code },      
        success: function (dimension3Values) {
            $.each(dimension3Values, function (i, dimension3Value) {
                options += "<option value='" + dimension3Value.Code + "'>";
                options += dimension3Value.Name;
                options += "</option>";
            });
            $("#ShortcutDimension3Code").html(options);
            $("#LineShortcutDimension3Code").html(options);
        },
        error: function (xhr, status, thrownError) {
            var label = $("#lblShortcutDimension3Code").text();
            options += "<option>";
            options += "Unable to load " + label + " values";
            options += "</option>";
            $("#ShortcutDimension3Code").html(options);
            $("#LineShortcutDimension3Code").html(options);
        }
    });
}

//Load shortcut dimension 4 values
function LoadShortcutDimension4Values(GlobalDimension1Code) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetShortcutDimension4Codes,
        type: 'GET',
        dataType: 'json',
        data: { GlobalDimension1Code: GlobalDimension1Code },       
        success: function (dimension4Values) {
            $.each(dimension4Values, function (i, dimension4Value) {
                options += "<option value='" + dimension4Value.Code + "'>";
                options += dimension4Value.Name;
                options += "</option>";
            });
            $("#ShortcutDimension4Code").html(options);
            $("#LineShortcutDimension4Code").html(options);
        },
        error: function (xhr, status, thrownError) {
            var label = $("#lblShortcutDimension4Code").text();
            options += "<option>";
            options += "Unable to load " + label + " values";
            options += "</option>";
            $("#ShortcutDimension4Code").html(options);
            $("#LineShortcutDimension4Code").html(options);
        }
    });
}

//Load shortcut dimension 5 values
function LoadShortcutDimension5Values(GlobalDimension1Code) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetShortcutDimension5Codes,
        type: 'GET',
        dataType: 'json',
        data: { GlobalDimension1Code: GlobalDimension1Code },        
        success: function (dimension5Values) {
            $.each(dimension5Values, function (i, dimension5Value) {
                options += "<option value='" + dimension5Value.Code + "'>";
                options += dimension5Value.Name;
                options += "</option>";
            });
            $("#ShortcutDimension5Code").html(options);
            $("#LineShortcutDimension5Code").html(options);
        },
        error: function (xhr, status, thrownError) {
            var label = $("#lblShortcutDimension5Code").text();
            options += "<option>";
            options += "Unable to load " + label + " values";
            options += "</option>";
            $("#ShortcutDimension5Code").html(options);
            $("#LineShortcutDimension5Code").html(options);
        }
    });
}

//Load shortcut dimension 6 values
function LoadShortcutDimension6Values(GlobalDimension1Code) {
    var options = "";
    options += "<option>";
    options += "";
    options += "</option>";

    $.ajax({
        url: AJAXUrls.GetShortcutDimension6Codes,
        type: 'GET',
        dataType: 'json',
        data: { GlobalDimension1Code: GlobalDimension1Code },       
        success: function (dimension6Values) {
            $.each(dimension6Values, function (i, dimension6Value) {
                options += "<option value='" + dimension6Value.Code + "'>";
                options += dimension6Value.Name;
                options += "</option>";
            });
            $("#ShortcutDimension6Code").html(options);
            $("#LineShortcutDimension6Code").html(options);
        },
        error: function (xhr, status, thrownError) {
            var label = $("#lblShortcutDimension6Code").text();
            options += "<option>";
            options += "Unable to load " + label + " values";
            options += "</option>";
            $("#ShortcutDimension6Code").html(options);
            $("#LineShortcutDimension6Code").html(options);
        }
    });   
}

//Add dropdown search
function AddDimensionsDropDownListSearch() {
    $("#GlobalDimension1Code").select2({
        placeholder: $("#GlobalDimension1CodeLbl").text(),
        allowClear: true
    });
    if ($("#GlobalDimension2Code").is("select")) {
        $("#GlobalDimension2Code").select2({
            placeholder: $("#GlobalDimension2CodeLbl").text(),
            allowClear: true
        });
    }
    $("#ShortcutDimension3Code").select2({
        placeholder: $("#ShortcutDimension3CodeLbl").text(),
        allowClear: true
    });
    $("#ShortcutDimension4Code").select2({
        placeholder: $("#ShortcutDimension4CodeLbl").text(),
        allowClear: true
    });
    $("#ShortcutDimension5Code").select2({
        placeholder: $("#ShortcutDimension5CodeLbl").text(),
        allowClear: true
    });
    $("#ShortcutDimension6Code").select2({
        placeholder: $("#ShortcutDimension6CodeLbl").text(),
        allowClear: true
    });
    $("#ShortcutDimension7Code").select2({
        placeholder: $("#ShortcutDimension7CodeLbl").text(),
        allowClear: true
    });
    $("#ShortcutDimension8Code").select2({
        placeholder: $("#ShortcutDimension8CodeLbl").text(),
        allowClear: true
    });

    $("#LineGlobalDimension1Code").select2({
        placeholder: $("#GlobalDimension1CodeLbl").text(),
        allowClear: true
    });
    $("#LineGlobalDimension2Code").select2({
        placeholder: $("#LineGlobalDimension2CodeLbl").text(),
        allowClear: true
    });
    $("#LineShortcutDimension3Code").select2({
        placeholder: $("#LineShortcutDimension3CodeLbl").text(),
        allowClear: true
    });
    $("#LineShortcutDimension4Code").select2({
        placeholder: $("#LineShortcutDimension4CodeLbl").text(),
        allowClear: true
    });
    $("#LineShortcutDimension5Code").select2({
        placeholder: $("#LineShortcutDimension5CodeLbl").text(),
        allowClear: true
    });
    $("#LineShortcutDimension6Code").select2({
        placeholder: $("#LineShortcutDimension6CodeLbl").text(),
        allowClear: true
    });
    $("#LineShortcutDimension7Code").select2({
        placeholder: $("#LineShortcutDimension7CodeLbl").text(),
        allowClear: true
    });
    $("#LineShortcutDimension8Code").select2({
        placeholder: $("#LineShortcutDimension8CodeLbl").text(),
        allowClear: true
    });
}

//Custom ajax onError callback
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