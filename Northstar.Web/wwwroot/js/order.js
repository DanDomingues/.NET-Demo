var dataTable;

$(document).ready(function ()
{
    var url = new URLSearchParams(window.location.search);
    var userFilter = url.get("filter");
    var statusFilter = url.get("status") ?? "all";
    var isAdmin = (userFilter === "all");

    loadDataTable({ status: statusFilter, filter: userFilter, isAdmin: isAdmin });
});

function loadDataTable(options) {
    var columns = [
        { data: 'id', className: 'text-start'}
    ]

    if(options.isAdmin)
    {
        columns.push(
            { data: 'fullName' },
            { data: 'phoneNumber', className: 'text-start' },
            { data: 'applicationUser.email' });
    }

    columns.push(
        { data: 'orderDate', render: renderDateTime },
        { data: 'orderStatus' },
        { data: 'paymentStatus' },
        { data: 'orderTotal', render: renderCurrency },
        { 
            data: 'id', 
            width: '8%', 
            className: 'text-end', 
            render: function (data) 
            { 
                return renderButtons(data, options.filter, options.isAdmin) 
            } 
        }
    )

    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/customer/order/getallby?status=' + options.status + '&filter=' + options.filter },
        "columns": columns
    });
}

function renderButtons(id, filter, isAdmin) 
{
    var label = isAdmin ? 'Edit details' : 'View details';
    return `<div class="m-75 btn-group" role="group">
                <a href="/customer/order/details?id=${id}&filter=${filter}" class="northstar-table-action" aria-label="${label}">
                    <span>${label}</span>
                </a>
            </div>`;
}
