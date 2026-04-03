var dataTable;

$(document).ready(function () 
{
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {url: '/admin/company/getall'},
        "columns":[
            { data: 'name' },
            { data: 'streetAddress' },
            { data: 'city' },
            { data: 'state' },
            { data: 'postalCode' },
            { data: 'phoneNumber' },
            {
                data: 'id',
                "render": function (data) 
                {
                    return `<div class="admin-table-row-actions" role="group">
                                <a href="/admin/company/upsert?id=${data}" class="admin-btn admin-btn-secondary"><i class="bi bi-pencil-square"></i><span>Edit</span></a>
                                <a onClick=Delete('/admin/company/deleteAt/${data}') class="admin-btn admin-btn-danger"><i class="bi bi-trash3"></i><span>Delete</span></a>
                            </div>`
                }
            }
        ]
    });
}
