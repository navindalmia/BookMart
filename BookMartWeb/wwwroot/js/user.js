var dataTable;
$(document).ready(function () {
    loadDataTable();
    
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable(
        {
            "scrollX": true,  
            "responsive": true,  //for responsiveness    
            "autoWidth": false  ,//for responsiveness
            "ajax": { url: '/admin/user/getall' },
            "columns": [
                { data: 'name', "width": "200px" },
                { data: 'email', "width": "250px" },
                { data: 'phoneNumber', "width": "150px" },
                { data: 'company.name', "width": "200px" },
                { data: 'role', "width": "120px" },
                {
                    data: { id: "id", lockoutEnd: "lockoutEnd" },
                    "render": function (data) {
                        var today = new Date().getTime();
                        var lockout = new Date(data.lockoutEnd).getTime();

                        if (lockout > today) {
                            return `
                            
                            <div class = "d-flex justify-content-center gap-1 flex-wrap">
                                <a onclick=LockUnlock('${data.id}')  class = "btn btn-danger text-white flex-grow-1 w-auto" style = "cursor:pointer; ">
                                    <i class= "bi bi-unlock-fill"></i> Unlock
                                </a>
                                 <a class = "btn btn-danger text-white flex-grow-1 w-auto" style = "cursor:pointer; ">
                                    <i class= "bi bi-pencil-square"></i> Permission
                                </a>
                            </div>
                            `

                        }
                        else {
                            return `
                            
                             <div class = "d-flex justify-content-center gap-1 flex-wrap">
                                <a onclick=LockUnlock('${data.id}')  class = "btn btn-success text-white flex-grow-1 w-auto" style = "cursor:pointer; ">
                                    <i class= "bi bi-lock-fill"></i> Lock
                                </a>
                                 <a class = "btn btn-danger text-white flex-grow-1 w-auto" style = "cursor:pointer; ">
                                    <i class= "bi bi-pencil-square"></i> Permission
                                </a>
                            </div>
                            `

                        }

                       
                    }
                    , "width": "25%"
                }



            ]

        }
    );

}
function LockUnlock(id) {
        $.ajax({
            type: "POST",
            url: '/Admin/User/LockUnlock',
            data: JSON.stringify(id),
            contentType: "application/json",
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    dataTable.ajax.reload();
                }
            }
        })
    }

//}

