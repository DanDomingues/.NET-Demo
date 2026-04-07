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
            { data: 'fullName' },
            { data: 'email' },
            { data: 'phoneNumber', className: 'text-start' },
            { data: 'company.name' },
            { data: 'role' },
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
        <div class="admin-user-actions">
            <a onClick=ToggleLock('${data.id}') class="admin-btn ${data.locked == true ? 'admin-btn-success' : 'admin-btn-danger'}" style="cursor: pointer;">
                <i class="bi ${data.locked == true ? 'bi-unlock-fill' : 'bi-lock-fill'}"></i><span>${label}</span>
            </a>
            <button type="button" class="admin-btn admin-btn-secondary js-manageRole-modal-trigger" data-id="${data.id}" style="cursor: pointer;">
                <i class="bi bi-pencil-square"></i><span>Edit role</span>
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
