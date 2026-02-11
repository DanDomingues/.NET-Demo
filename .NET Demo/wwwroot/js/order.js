var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    var statusValues = ["inprocess", "completed", "pending", "approved"];
    for (var i = 0; i < statusValues.length; i++) {
        if (url.includes(statusValues[i])) {
            loadDataTable(statusValues[i]);
            return;
        }
    }
    loadDataTable("all");
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/order/getall=' + status },
        "columns":[
            { data: 'id' },
            { data: 'name' },
            { data: 'phoneNumber' },
            { data: 'applicationUser.email' },
            { data: 'orderStatus' },
            { data: 'orderTotal' },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="m-75 btn-group" role="group">
                                <a href="/admin/order/upsert?orderId=${data}" class="btn btn-primary mx-2"><i class="bi bi-feather"></i></a>
                            </div>`
                }
            }
        ]
    });
}