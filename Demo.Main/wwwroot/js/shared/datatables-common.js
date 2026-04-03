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
