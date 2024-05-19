$(document).ready(function () {
    $('body').on('click', '.btnAddList', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var soluong = 1;
        $.ajax({
            url: '/admin/WareHouseImport/AddListProduct',
            type: 'POST',
            data: { id: id, soluong: soluong },
            success: function (rs) {
                if (rs.Success) {
                    if (rs.Code == 1) {
                        LoadList();
                        const Toast = Swal.mixin({
                            toast: true,
                            position: "top-end",
                            showConfirmButton: false,
                            timer: 3000,
                            timerProgressBar: true,
                            didOpen: (toast) => {
                                toast.onmouseenter = Swal.stopTimer;
                                toast.onmouseleave = Swal.resumeTimer;
                            }
                        });

                        Toast.fire({
                            icon: "success",
                            title: "Thêm thành công"
                        });
                    }
                }
                else {
                }

            }

        });

    });
    $('body').on('click', '.btnDelete', function (e) {
        e.preventDefault();
        var id = $(this).data('id');


        Swal.fire({
            title: "Bạn có chắc?",
            text: "Muốn xóa sản phẩn này!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Xóa !"
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/admin/WareHouseImport/Delete',
                    type: 'POST',
                    data: { id: id },
                    success: function (rs) {
                        if (rs.Success) {
                            if (rs.Code == 1) {
                                $('#checkout_items').html(rs.Count);
                                LoadList();
                                const Toast = Swal.mixin({
                                    toast: true,
                                    position: "top-end",
                                    showConfirmButton: false,
                                    timer: 3000,
                                    timerProgressBar: true,
                                    didOpen: (toast) => {
                                        toast.onmouseenter = Swal.stopTimer;
                                        toast.onmouseleave = Swal.resumeTimer;
                                    }
                                });

                                Toast.fire({
                                    icon: "success",
                                    title: "Xoá thành công"
                                });
                            }
                        }
                    }
                });
            }
        });

    });
});

function LoadList() {
    $.ajax({
        url: '/admin/WareHouseImport/Partail_ListProduct',
        type: 'GET',
        success: function (rs) {
            $('#load_data').html(rs);
        }

    });
}
