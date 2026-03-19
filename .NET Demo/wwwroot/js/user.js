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
                    if(data.lockoutEnabled)
                    {
                        return renderBody('success', 'Unlock', data);
                    }
                    else
                        {
                            //TODO: Convert this to ternary if possible
                            return renderBody('danger', 'Lock', data);
                    }
                }
            }
        ]
    });

    function renderBody(tag, label, data)
    {
        return `
        <div class="text-center">
            <a onClick=ToggleLock('${data.id}') class="btn btn-${tag} text-white" style="cursor: pointer; width: 100px;">
                <i class="bi bi-unlock-fill"></i> ${label}
            </a>
            <a href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-danger text-white" style="cursor: pointer; width: 150px;">
                <i class="bi bi-pencil-square"></i> Permission
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
