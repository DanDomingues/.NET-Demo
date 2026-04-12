var dataTable;

$(document).ready(function ()
{
    var url = new URLSearchParams(window.location.search);
    var userFilter = url.get("filter");
    var statusFilter = url.get("status") ?? "all";
    var isAdmin = statusFilter === "all";

    loadDataTable({ status: statusFilter, filter: userFilter }, isAdmin);
});

function loadDataTable(options, isAdmin) {
    var columns = [
        { data: 'id' }
    ]

    if(isAdmin)
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
                return renderButtons(data, options.filter) 
            } 
        }
    )

    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/customer/order/getallby?status=' + options.status + '&filter=' + options.filter },
        "columns": columns
    });
}

function loadCustomerDataTable(options) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/customer/order/getallby?status=' + options.status + '&filter=' + options.filter },
        "columns": [
            { data: 'id' },
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
    var isAdmin = document.body.classList.contains('admin-ops');
    var iconClass = isAdmin ? 'bi-pencil-square' : 'bi-eye'
    var label = isAdmin ? 'Edit Order' : 'View Order';

    return `<div class="m-75 btn-group" role="group">
                <a href="/customer/order/details?id=${id}&filter=${filter}" class="northstar-table-action" aria-label="${label}">
                    <i class="bi ${iconClass}"></i>
                </a>
            </div>`;
}
