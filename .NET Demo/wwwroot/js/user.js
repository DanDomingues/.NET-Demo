var dataTable;

$(document).ready(function () 
{
    loadDataTable();
});

function loadDataTable() 
{
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { data: 'name', width: '20%' },
            { data: 'email', width: '20%' },
            { data: 'phoneNumber', width: '15%' },
            { data: 'company.name', width: '15%' },
            { data: 'role', width: '15%' },
            {
                data: { id:'id', lockoutEnabled:'lockoutEnabled' },
                "render": function (data) 
                {
                    return renderBody(
                        data.lockoutEnabled ? 'success' : 'danger', 
                        data.lockoutEnabled ? 'Unlock' : 'Lock', 
                        data);
                }
            }
        ]
    });

    function renderBody(tag, label, data)
    {
        return `
        <div class="text-center">
            <a onClick=ToggleLock('${data.id}') class="btn btn-${tag} text-white pb-5" style="cursor: pointer; width: 120px;">
                <i class="bi bi-unlock-fill"></i> ${label}
            </a>
            <a href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-danger text-white" style="cursor: pointer; width: 120px;">
                <i class="bi bi-pencil-square"></i> Edit Role
            </a>
        </div>
        `
    }
}

function ToggleLock(id)
{
    $.ajax({
        type: "POST",
        url: '/Admin/User/ToggleLock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            toastr.success(data.message);
            dataTable.ajax.reload();
        }
    });
}
