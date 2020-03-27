
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



