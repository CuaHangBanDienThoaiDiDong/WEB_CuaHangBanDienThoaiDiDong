$(document).ready(function () {
    LoadList();
    $('body').on('click', '.btnAddList', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var soluong = 1;
        $.ajax({
            url: '/admin/Seller/AddListProduct',
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
                            timerProgressBar: true,F
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
    $('body').on('click', '.btnDeleteNhapKhoSanpham', function (e) {
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


    $('body').on('input', '.Quantity', function (e) {
        var productId = $(this).attr('id');
        var newQuantity = $(this).val();

        if (newQuantity <= 0) {
            Swal.fire({
                title: "Ban có muốn bỏ sản phẩm này",
                text: "",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Đồng ý"
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/admin/WareHouseImport/Delete',
                        type: 'POST',
                        data: { id: productId },
                        success: function (rs) {
                            if (rs.Success) {
                                if (rs.Code == 1) {
                                    /*$('#checkout_items').html(rs.Count);*/
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
        }
        else {
            $.ajax({
                type: 'POST',
                url: '/admin/WareHouseImport/UpdateQuanTity',
                data: {
                    id: productId,
                    quantity: newQuantity
                },
                success: function (result) {
                    console.log(result);
                    if (result.Success) {
                        console.log('Cập nhật số lượng thành công');
                    } else {
                        console.log('Có lỗi xảy ra: ' + result.msg);
                    }
                },
                error: function (error) {
                    console.log('Lỗi Ajax: ' + error.statusText);
                }
            });

        }

    });
});



//Start updateSoLuong products checkout

function updateQuantityListProuctsCheckOut() {
    $('body').on('input', '.QuantityListCheckOut', function (e) {
        var productId = $(this).attr('id');
        var newQuantity = $(this).val();

        // Kiểm tra nếu newQuantity nhỏ hơn hoặc bằng 0
        if (newQuantity <= 0) {
            Swal.fire({
                title: "Bạn có chắc?",
                text: "Muốn xóa sản phẩm này!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Xóa!"
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/admin/Seller/DeleteCartItem',
                        type: 'POST',
                        data: { id: productId },
                        success: function (rs) {
                            if (rs.Success) {
                                if (rs.Code == 1) {
                                    Partail_ListProduct();
                                    // Hiển thị thông báo xóa thành công
                                    Swal.fire({
                                        toast: true,
                                        position: 'top-start',
                                        icon: 'success',
                                        title: 'Xóa thành công',
                                        showConfirmButton: false,
                                        timer: 1500,
                                        timerProgressBar: true,
                                        didOpen: (toast) => {
                                            toast.addEventListener('mouseenter', Swal.stopTimer);
                                            toast.addEventListener('mouseleave', Swal.resumeTimer);
                                        }
                                    });
                                }
                            }
                        },
                        error: function (xhr, status, error) {
                            console.log(xhr.responseText);
                        }
                    });
                }
            });
        } else {
            // Nếu newQuantity hợp lệ, gửi yêu cầu AJAX để cập nhật số lượng
            $.ajax({
                type: 'POST',
                url: '/Admin/Seller/UpdateQuantityCartItem',
                data: {
                    id: productId,
                    quantity: newQuantity
                },
                success: function (result) {
                    console.log(result);
                    if (result.Success) {
                        LoadList();
                        console.log('Cập nhật số lượng thành công');
                    } else {
                        console.log('Có lỗi xảy ra: ' + result.msg);
                        Swal.fire({
                            icon: "error",
                            title: "Lỗi",
                            text: "Lỗi hệ thống máy chủ",
                            footer: '<a href="/dang-nhap">Quay về đăng nhập?</a>'
                        });
                    }
                },
                error: function (error) {
                    console.log('Lỗi Ajax: ' + error.statusText);
                }
            });
        }
    });
}




//end updateSoLuong products checkout




function attachDeleteHandlers() {
    $('body').on('click', '.btndeleteCheckOut', function (e) {
        e.preventDefault();
        var productId = $(this).data("id");

        Swal.fire({
            title: "Bạn có chắc?",
            text: "Muốn xóa sản phẩm này!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Xóa!"
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/Admin/Seller/DeleteCartItem',
                    type: 'POST',
                    data: { id: productId },
                    success: function (rs) {
                        if (rs.Success) {
                            if (rs.Code == 1) {
                                LoadListProCheckOut();
                                LoadListPriceCheckOut();
                                // Sử dụng SweetAlert2 để hiển thị thông báo ở góc trên bên trái
                                Swal.fire({
                                    toast: true,
                                    position: 'top-start', // Hiển thị ở góc trên bên trái
                                    icon: 'success',
                                    title: 'Xóa thành công',
                                    showConfirmButton: false,
                                    timer: 1500,
                                    timerProgressBar: true,
                                    didOpen: (toast) => {
                                        toast.addEventListener('mouseenter', Swal.stopTimer);
                                        toast.addEventListener('mouseleave', Swal.resumeTimer);
                                    }
                                });
                            }
                        }
                    },
                    error: function (xhr, status, error) {
                        console.log(xhr.responseText);
                    }
                });
            }
        });
    });
}

function LoadList() {
    $.ajax({
        url: '/admin/Seller/Partail_ListProduct',
        type: 'GET',
        success: function (rs) {
            $('#load_data').html(rs);
        }

    });
}
