var dataTable

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("completed");
        }
        else {
            if (url.includes("pending")) {
                loadDataTable("pending");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else {
                    loadDataTable("all");
                }
                
            }
        }
    }
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll?status=" + status
        },
        "columns": [
            {"data":"id","width":"5%"},
            {"data":"name","width":"10%"},
            {"data":"phoneNumber","width":"15%"},
            {"data":"applicationUser.email","width":"15%"},
            {"data":"orderStatus","width":"11%"},
            { "data": "paymentStatus", "width": "16%" },
            {"data":"orderAmount","width":"16%"},
            {
                "data": "id",
                "render": function(data) {
                    return `<div role="group">
                        <a href="/Admin/Order/Details?orderId=${data}" class="btn btn-info"><i class="bi bi-info-lg"></i> &nbsp; Detail</a>
                    `
                },
                "width": "12%"
            }
        ]
    });
}

