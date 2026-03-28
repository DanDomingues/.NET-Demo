var dataTable;

$(document).ready(function () 
{
    var url = new URLSearchParams(window.location.search);
    var userFilter = url.get("filter");
    var statusFilter = url.get("status") ?? "all";

    var statusValues = [
        "completed", 
        "pending", 
        "approved",
        "inprocess", 
        "shipped"];

    loadDataTable({ status: statusFilter, filter: userFilter});
});

function loadDataTable(options) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/customer/order/getallbystatus?status=' + options.status + '&filter=' + options.filter },
        "columns":[
            { data: 'id' },
            { data: 'name' },
            { data: 'phoneNumber' },
            { data: 'applicationUser.email' },
            { data: 'orderDate' },
            { data: 'orderStatus' },
            { data: 'orderTotal' },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="m-75 btn-group" role="group">
                                <a href="/customer/order/details?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-feather"></i></a>
                            </div>`
                }
            }
        ]
    });
}