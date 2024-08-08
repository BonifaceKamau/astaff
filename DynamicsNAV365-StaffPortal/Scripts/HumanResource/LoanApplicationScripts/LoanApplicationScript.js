function InitializeLoanApplicationScript()
{
    $("#LoanProductType").select2({
        placeholder: $("#LoanProductTypeLbl").text(),
        allowClear: true
    });
}