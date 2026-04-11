function confirmAction(onConfirm) 
{
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes"
    }).then((result) => 
    {
        if (result.isConfirmed) 
        {
            onConfirm();
        }
    });
}

function Delete(url) 
{
    confirmAction(function () 
    {
        $.ajax({
            url: url,
            type: 'DELETE',
            success: function (data) {
                dataTable.ajax.reload();
                toastr.success(data.message);
                Swal.fire({
                    title: "Deleted!",
                    text: "Your file has been deleted.",
                    icon: "success"
                });
            }
        });
    });
}

function submitDeleteForm(formId, idFieldId, id) 
{
    confirmAction(function () 
    {
        document.getElementById(idFieldId).value = id;
        document.getElementById(formId).submit();
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