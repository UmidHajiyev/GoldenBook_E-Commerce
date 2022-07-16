﻿var dataTable

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url":"/Admin/Product/GetAll"
        },
        "columns": [
            {"data":"name","width":"15%"},
            {"data":"author","width":"15%"},
            {"data":"isbn","width":"15%"},
            {"data":"price","width":"15%"},
            {"data":"category.name","width":"15%"},
            {
                "data": "id",
                "render": function(data) {
                    return `<div role="group">
                        <a href="/Admin/Product/Upsert?id=${data}" class="btn btn-info"><i class="bi bi-pen"></i> &nbsp; Edit</a>
                        <button onClick=Delete('/Admin/Product/Delete/${data}') class="btn btn-danger"><i class="bi bi-trash3"></i> &nbsp; Delete</button></div>
                    `
                },
                "width": "20%"
            }
        ]
    });
}


function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data)
                {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}