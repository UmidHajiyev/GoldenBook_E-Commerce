var dataTable

$(document).ready(function () {
    loadDataTable();
    document.getElementById("tblData_wrapper").style.color = "white";
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url":"/Admin/Order/GetAll"
        },
        "columns": [
            {"data":"id","width":"15%"},
            {"data":"name","width":"15%"},
            {"data":"phoneNumber","width":"16%"},
            {"data":"applicationUser.email","width":"16%"},
            {"data":"orderStatus","width":"13%"},
            {"data":"paymentStatus","width":"13%"},
            {
                "data": "id",
                "render": function(data) {
                    return `<div role="group">
                        <a href="/Admin/Order/Deatil?id=${data}" class="btn btn-info"><i class="bi bi-info-lg"></i> &nbsp; Detail</a>
                    `
                },
                "width": "12%"
            }
        ]
    });
}

