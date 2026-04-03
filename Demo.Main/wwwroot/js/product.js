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
            { data: 'price50' },
            { data: 'price100' },
            { data: 'author' },
            { data: 'category.name' },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="m-75 btn-group" role="group">
                                <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-feather"></i> Edit</a>
                                <a onClick=Delete('/admin/product/deleteAt/${data}') class="btn btn-danger mx-2"><i class="bi bi-trash-fill"></i> Delete</a>
                                </div>`
                }
            }
        ]
    });
}