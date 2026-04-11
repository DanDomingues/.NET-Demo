var dataTable;

$(document).ready(function ()
{
    var url = new URLSearchParams(window.location.search);
    var userFilter = url.get("filter");
    var statusFilter = url.get("status") ?? "all";
    loadDataTable({ status: statusFilter, filter: userFilter });
});

function loadDataTable(options) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/customer/order/getallby?status=' + options.status + '&filter=' + options.filter },
        "columns": [
            { data: 'id' },
            { data: 'fullName' },
            { data: 'phoneNumber', className: 'text-start' },
            { data: 'applicationUser.email' },
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
                    return renderButtons(data, options.filter) 
                } 
            },
        ]
    });
}

function renderButtons(id, filter) 
{
    //TODO: Validate this as an icon and remove below lines (and css) when done
    //var buttonClass = isAdmin ? 'admin-table-action' : 'northstar-table-action';
    var isAdmin = document.body.classList.contains('admin-ops');
    var iconClass = isAdmin ? 'bi-pencil-square' : 'bi-eye'
    var label = isAdmin ? 'Edit Order' : 'View Order';

    return `<div class="m-75 btn-group" role="group">
                <a href="/customer/order/details?id=${id}&filter=${filter}" class="northstar-table-action" aria-label="${label}">
                    <i class="bi ${iconClass}"></i>
                </a>
            </div>`;
}
