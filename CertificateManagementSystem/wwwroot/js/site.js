// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
jQuery.ready(function () {

});


function SetClient() {
    let contractNumber = $('#inputContractNumber').val();
    let year = 2020;
    $.getJSON("/Document/GetClient", { contractNumber: contractNumber, year: year }, function (data) {
        let name = data.name;
        let place = data.exploitationPlace;
        $('#inputClient').val(name);
        $('#inputExploitationPlace').val(place);
    });

    
}