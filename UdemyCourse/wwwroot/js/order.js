var dataTable
$(document).ready(function () {
    loadDataTable();
});
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/order/getall' },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "name", "width": "15%" },
            { "data": "phoneNumber", "width": "10%" },
            { "data": "applicationUser.email", "width": "15%" },
            { "data": "orderStatus", "width": "15%" },
            { "data": "orderTotal", "width": "15%" },

            {
                data: 'orderId',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group"> 
                        <a href = "/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-fill"></i> Edit </a>
                    </div>`
                },
                "width": "25%"
            }
        ]
        
    });
}
