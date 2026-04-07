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
            { data: 'employeeCount'},
            { data: 'phoneNumber' },
            {
                data: 'id',
                "render": function (data) 
                {
                    return `<div class="admin-table-row-actions" role="group">
                                <button class="admin-btn admin-btn-secondary js-company-modal-trigger" data-id="${data}" style="cursor: pointer;">
                                    <i class="bi bi-pencil-square"></i>
                                    <span>Edit</span>
                                </button>
                                <a onClick=Delete('/admin/company/deleteAt/${data}') class="admin-btn admin-btn-danger"><i class="bi bi-trash3"></i><span>Delete</span></a>
                            </div>`
                }
            }
        ]
    });
}
