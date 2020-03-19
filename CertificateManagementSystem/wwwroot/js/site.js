// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {

    $('#inputClientName').select2({
        ajax: {
            url: '/Document/GetAutocompleteData',
            data: { dataType: 'clientName' },
            processResults: function (data) {
                return {
                    results: data
                };
            }
        },
        tags: true
    });

    $('#inputExploitationPlace').select2({
        ajax: {
            url: '/Document/GetAutocompleteData',
            data: { dataType: 'exploitationPlace' },
            processResults: function (data) {
                return {
                    results: data
                };
            }
        },
        tags: true
    });

    $('#inputDeviceName').select2({
        ajax: {
            url: '/Document/GetAutocompleteData',
            data: { dataType: 'deviceName' },
            processResults: function (data) {
                return {
                    results: data
                };
            }
        },
        tags: true
    });

    $('#inputDeviceType').select2({
        ajax: {
            url: '/Document/GetAutocompleteData',
            data: { dataType: 'deviceType' },
            processResults: function (data) {
                return {
                    results: data
                };
            }
        },
        tags: true
    });
});


function SetClient() {
    let contractNumber = $('#inputContractNumber').val();
    let year = 2020;
    $.getJSON("/Document/GetClient", { contractNumber: contractNumber, year: year }, function (data) {
        let name = data.name;
        let place = data.exploitationPlace;
        $('#inputClientName').val(name);
        $('#inputExploitationPlace').val(place);
    });


}