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

function renderDateTime(data) {
    var date = new Date(data);
    var day = date.getDate().toString().padStart(2, '0');
    var month = (date.getMonth() + 1).toString().padStart(2, '0');
    var year = date.getFullYear().toString().padStart();
    var hour = date.getHours().toString().padStart(2, '0');
    var minute = date.getMinutes().toString().padStart(2, '0');
    var second = date.getSeconds().toString().padStart(2, '0');
    return `${day}/${month}/${year} ${hour}:${minute}:${second}`;
}

function renderCurrency(data) {
    var usdCurrencyFormatter = new Intl.NumberFormat('en-US', 
    {
        style: 'currency',
        currency: 'USD'
    });
    return usdCurrencyFormatter.format(Number(data));
}

function renderButtons(id, filter) {
    return `<div class="m-75 btn-group" role="group">
                <a href="/customer/order/details?id=${id}&filter=${filter}" class="btn btn-primary mx-2"><i class="bi bi-feather"></i></a>
            </div>`;
}
