
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

    // Показать или скрыть меню фильтрации поиска
    document.querySelector('#search-bar-settings').addEventListener('click', function () {
        let div = document.querySelector('.search-bar .search-bar-filter');
        div.classList.toggle('active');
    })

    // Выбор или снятие всех чекбоксов в меню фильтрации поиска
    document.querySelector('#select-all-checkbox').addEventListener('change', function () {
        let checkboxes = document.querySelectorAll('.search-bar-filter input[type="checkbox"]');
        if (this.checked) {
            checkboxes.forEach(function (checkbox) {
                checkbox.checked = true;
            })
        }
        else {
            checkboxes.forEach(function (checkbox) {
                checkbox.checked = false;
            })
        }
    });

    document.querySelector('.search-bar').addEventListener('focus', function (e) {
        let searchbar = document.querySelector('.search-bar');
        let filter = document.querySelector('.search-bar-filter');

        searchbar.style.width = '350px';

    }, true);

    document.querySelector('.search-bar').addEventListener('focusout', function (e) {
        let searchbar = document.querySelector('.search-bar');
        let filter = document.querySelector('.search-bar-filter');

        if (filter.classList.contains('active'))
            filter.classList.remove('active');

        searchbar.style.width = '200px';

    }, true);

});

// Открыть окно просмотра документа
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

// Загрузка файла для предпросмотра
function UploadFile() {

    let file = document.querySelector('#newDocumentFileInput').files[0];
    let fileReader = new FileReader();

    fileReader.onload = function (event) {
        let content = event.target.result;
        document.querySelector('.pdf-holder').setAttribute('src', `${content}`)
    };

    fileReader.readAsDataURL(file);
}


