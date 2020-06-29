
// Write your JavaScript code.
$(document).ready(function () {

    // Смена иконок при раскрытии ветви дерева документов
    document.querySelectorAll('#treeView .collapsable span.folder').forEach(element => {
        element.addEventListener('click', e => {
            element.classList.toggle('opened');
            console.log(element.previousElementSibling);
            element.previousElementSibling.classList.toggle('expandable-hitarea');
        });
    });
    document.querySelectorAll('#treeView .hitarea').forEach(hitarea => {
        hitarea.addEventListener('click', function () {
            hitarea.nextElementSibling.click();
        });
    });

    // Автозакрытие всплывающих оповещений
    window.setTimeout(function () {
        $('.alerts-container #inner-alert').fadeOut(3500, function () {
            $(this).remove();
        });
    }, 7000);

    // Показать или скрыть меню фильтрации поиска
    //document.querySelector('#search-bar-settings').addEventListener('click', function () {

    //    const filter = document.querySelector('.search-bar .search-bar-filter');
    //    const searchbar = document.querySelector('.search-bar');
    //    const searchbarCss = getComputedStyle(searchbar);
    //    const width = searchbarCss.width;

    //    if (width == '200px') {
    //        searchbar.style.width = '350px';
    //    }

    //    document.addEventListener('mouseup', LostFocus);
    //    filter.classList.toggle('active');
    //})

    // Выбор или снятие всех чекбоксов в меню фильтрации поиска
    document.querySelector('#select-all-checkbox').addEventListener('change', function () {
        const checkboxes = document.querySelectorAll('.search-bar-filter input[type="checkbox"]');

        if (this.checked) {
            checkboxes.forEach(function (checkbox) {
                checkbox.checked = true;
            });
        }
        else {
            checkboxes.forEach(function (checkbox) {
                checkbox.checked = false;
            });
        }
    });

    // Фокус на панели поиска
    document.querySelector('.search-bar').addEventListener('focus', function (e) {
        const searchbar = document.querySelector('.search-bar');
        const filter = document.querySelector('.search-bar .search-bar-filter');
        const prepend = document.querySelector('.search-bar .input-group-text');
        const input = document.querySelector('.search-bar input');

        prepend.style.background = "#fff url('../icons/icons8_search_24px.png') no-repeat center";
        input.style.background = '#fff';
        searchbar.style.width = '350px';
        document.addEventListener('mouseup', LostFocus);
        filter.classList.add('active');
    }, true);
});

// Снятие фокуса с панели поиска
function LostFocus(e) {
    const searchbar = document.querySelector('.search-bar');
    const filter = document.querySelector('.search-bar-filter');
    const prepend = document.querySelector('.search-bar .input-group-text');
    const input = document.querySelector('.search-bar input');

    if (!searchbar.isSameNode(e.target) && !searchbar.contains(e.target)) {
        prepend.style.background = "transparent url('../icons/icons8_search_24px_1.png') no-repeat center";
        input.style.background = 'transparent';
        searchbar.style.width = '200px';
        filter.classList.remove('active');
        document.removeEventListener('mouseup', LostFocus);
    }
}


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

    const file = document.querySelector('#newDocumentFileInput').files[0];
    const fileReader = new FileReader();

    fileReader.onload = function (event) {
        let content = event.target.result;
        document.querySelector('.pdf-holder').setAttribute('src', `${content}`)
    };

    fileReader.readAsDataURL(file);
}


