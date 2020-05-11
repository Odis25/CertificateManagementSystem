
// Write your JavaScript code.
$(document).ready(function () {

    // Смена иконок при раскрытии ветви дерева документов
    $('#treeView .collapsable span.folder').on('click', function (e) {
        let hitarea = $(this).prev();
        $(this).toggleClass('opened');
        hitarea.toggleClass('expandable-hitarea');
    });

    $('#treeView .hitarea').on('click', function (e) {
        $(this).next().click();
    });

    // Автозакрытие всплывающих оповещений
    window.setTimeout(function () {
        $('.alerts-container #inner-alert').fadeOut(3500, function () {
            $(this).remove();
        });
    }, 7000);

    document.querySelector('.search-bar input').addEventListener('click', function () {
        let div = document.querySelector('.search-bar .search-bar-filter');
        div.classList.toggle('active');
        //if (div.style.display == 'block') {
        //    div.style.display = 'none'
        //}
        //else {
        //    div.style.display = 'block'
        //}
        
    })
});

function OpenDocument(id) {
    $.get('/Document/Details', { id },
        function (result) {
            document.querySelector('.modal .modal-dialog').innerHTML = result;
            $('#document-preview-modal').modal('show');
        });
}


// Загрузка документов при выборе договора
function LoadDocuments(contractId) {
    $.get("/Document/LoadDocuments", { contractId: contractId },
        function (result) {
            $('#data-holder').html(result);
            $('#documents-table').DataTable({
                paging: false,
                searching: false,
                info: false,
                scrollY: '66.5vh'
            });
        });
}

function SetClient() {
    //let year = $('#inputYear').val();
    //let contractNumber = $('#inputContractNumber').val();

    //$.getJSON('/Document/GetClient', {
    //        contractNumber: contractNumber,
    //        year: year
    //    },
    //    function (result) {
    //        $('#inputClientName').val(result.name).trigger('change');
    //        $('#inputExploitationPlace').val(result.exploitationPlace).trigger('change');
    //});
}

// Загрузка файла для предпросмотра
function UploadFile() {

    let file = document.querySelector('#newDocumentFileInput').files[0];
    let fileReader = new FileReader();

    fileReader.onload = function (event) {
        let content = event.target.result;
        document.querySelector('.pdf-holder').setAttribute('src',`${content}`)
    };

    fileReader.readAsDataURL(file);
}


