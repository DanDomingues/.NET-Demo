var dataTable;

$(document).ready(function () 
{
    loadDataTable();
});

$(document).on('change', '#roleField', function () {
    toggleCompanyField($(this).val());
});

function loadDataTable() 
{
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { data: 'fullName', width: '20%' },
            { data: 'email', width: '20%' },
            { data: 'phoneNumber', width: '15%' },
            { data: 'company.name', width: '15%' },
            { data: 'role', width: '15%' },
            {
                data: { id:'id', locked: 'locked' },
                "render": function (data) 
                {
                    return renderBody(
                        data.locked == true ? 'success' : 'danger', 
                        data.locked == true ? 'Unlock' : 'Lock', 
                        data);
                }
            }
        ]
    });

    function renderBody(tag, label, data)
    {                
        return `
        <div class="d-flex flex-column align-items-center gap-2 text-center">
            <a onClick=ToggleLock('${data.id}') class="btn btn-${tag} text-white" style="cursor: pointer; width: 120px;">
                <i class="bi bi-unlock-fill"></i>${label}
            </a>
            <button type="button" class="btn btn-outline-warning js-manageRole-modal-trigger" data-id="${data.id}" style="cursor: pointer; width: 120px;">
                <i class="bi bi-pencil-square"></i>Edit Role
            </button>
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

function toggleCompanyField(role)
{
    const shouldShowCompany = role === 'Company' || role === 'Employee';
    $('#companyField').toggle(shouldShowCompany);
}
