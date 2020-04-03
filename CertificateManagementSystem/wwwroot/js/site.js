
// Write your JavaScript code.

function SetClient() {
    let year = $('#inputYear').val();
    let contractNumber = $('#inputContractNumber').val();

    $.getJSON('/Document/SetClient', {
            contractNumber: contractNumber,
            year: year
        },
        function (result) {
            $('#inputClientName').val(result.name).trigger('change');
            $('#inputExploitationPlace').val(result.exploitationPlace).trigger('change');
    });
}

function LoadFile() {

    let file = $('#newDocumentFileInput').get(0);
    let files = file.files;
    let formData = new FormData();
    formData.append('uploadedFile', files[0]);

    $.ajax({
        url: '/Document/UploadFile',
        data: formData,
        contentType: false,
        processData: false,
        type: 'post',
        success: function (result) {
            $('.pdfHolder').attr('src', result);
        }
    });
}


