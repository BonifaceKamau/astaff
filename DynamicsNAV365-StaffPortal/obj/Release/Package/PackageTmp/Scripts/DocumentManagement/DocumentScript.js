function LoadDocumentScripts() {
	$("#DocumentType").select2({
		placeholder: $("#DocumentTypeLbl").text(),
		allowClear: true
	});
}