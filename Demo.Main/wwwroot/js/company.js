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
                    return `<div class="m-75 btn-group" role="group">
                                <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-feather"></i> Edit</a>
                                <a onClick=Delete('/admin/company/deleteAt/${data}') class="btn btn-danger mx-2"><i class="bi bi-trash-fill"></i> Delete</a>
                            </div>`
                }
            }
        ]
    });
}