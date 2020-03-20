
// Write your JavaScript code.
$(function () {

    $('#inputClientName').select2({
        placeholder: "Укажите название заказчика...",
        allowClear: true,
        tags: true
    });

    $('#inputExploitationPlace').select2({
        placeholder: "Укажите место эксплуатации...",
        allowClear: true,
        tags: true
    });

    $('#inputDeviceName').select2({
        placeholder: "Укажите наименование СИ...",
        allowClear: true,
        tags: true
    });

    $('#inputDeviceType').select2({
        placeholder: "Укажите группу СИ...",
        allowClear: true,
        tags: true
    });

    $("select").on("select2:clear", function (evt) {
        $(this).on("select2:opening.cancelOpen", function (evt) {
            evt.preventDefault();
            $(this).off("select2:opening.cancelOpen");
        });
    });
});

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



