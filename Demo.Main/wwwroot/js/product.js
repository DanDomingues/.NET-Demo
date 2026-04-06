var dataTable;

$(document).ready(function (){
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {url: '/admin/product/getall'},
        "columns":[
            { data: 'title' },
            { data: 'isbn' },
            { data: 'price' },
            { data: 'discountedPrice' },
            { data: 'author' },
            { data: 'category.name' },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="admin-table-row-actions" role="group">
                                <a href="/admin/product/upsert?id=${data}" class="admin-btn admin-btn-secondary"><i class="bi bi-pencil-square"></i><span>Edit</span></a>
                                <a onClick=Delete('/admin/product/deleteAt/${data}') class="admin-btn admin-btn-danger"><i class="bi bi-trash3"></i><span>Delete</span></a>
                                </div>`
                }
            }
        ]
    });
}
