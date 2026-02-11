var dataTable;

$(document).ready(function ()){
    loadDataTable();
}

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {url: '/admin/order/getall'},
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