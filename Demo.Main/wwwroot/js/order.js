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
            { data: 'phoneNumber' },
            { data: 'applicationUser.email' },
            { data: 'orderDate', render: renderDateTime },
            { data: 'orderStatus' },
            { data: 'paymentStatus' },
            { data: 'orderTotal', render: renderCurrency },
            { data: 'id', render: function (data) { return renderButtons(data, options.filter) } },
        ]
    });
}

function renderButtons(id, filter) {
    var isAdmin = document.body.classList.contains('admin-ops');
    var buttonClass = isAdmin ? 'admin-table-action' : 'northstar-table-action';
    return `<div class="m-75 btn-group" role="group">
                <a href="/customer/order/details?id=${id}&filter=${filter}" class="${buttonClass}"><i class="bi bi-feather"></i></a>
            </div>`;
}
