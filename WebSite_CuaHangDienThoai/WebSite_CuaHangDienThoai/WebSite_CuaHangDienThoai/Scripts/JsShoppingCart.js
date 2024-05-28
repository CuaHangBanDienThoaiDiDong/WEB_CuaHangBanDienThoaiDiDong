$(document).ready(function () {
    // Hàm tải danh sách sản phẩm


    $('body').on('input', '.QuantityListCheckOut', function (e) {
        var productId = $(this).attr('id');
        var newQuantity = $(this).val();
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
                        url: '/ShoppingCart/DeleteCartItem',
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
                else {
                    LoadListProCheckOut();
                    LoadListPriceCheckOut();   
                }
            });
        }
        else {
            $.ajax({
                type: 'POST',
                url: '/ShoppingCart/UpdateQuantityCartItem',
                data: {
                    id: productId,
                    quantity: newQuantity
                },
                success: function (result) {
                    console.log(result);
                    if (result.Success) {
                        console.log('Cập nhật số lượng thành công');
                        LoadListProCheckOut();
                        LoadListPriceCheckOut();
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





    function LoadListProCheckOut() {
        $.ajax({
            url: '/ShoppingCart/Partial_ChiTietSanPhamMua',
            type: 'GET',
            success: function (rs) {
                $('.loadListProCheckOut').html(rs);
                attachDeleteHandlers(); // Gán lại sự kiện click sau khi tải lại danh sách sản phẩm
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }


    // Hàm tải danh sách sản phẩm
    function LoadListPriceCheckOut() {
        $.ajax({
            url: '/ShoppingCart/Partial_TotalPriceCheckOut',
            type: 'GET',
            success: function (rs) {
                $('.loadPice').html(rs);
                attachDeleteHandlers(); // Gán lại sự kiện click sau khi tải lại danh sách sản phẩm
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }


    //Start updateSoLuong products checkout

    function updateQuantityListProuctsCheckOut() {
        $('body').on('input', '.QuantityListCheckOut', function (e) {
            var productId = $(this).attr('id');
            var newQuantity = $(this).val();

            $.ajax({
                type: 'POST',
                url: '/ShoppingCart/UpdateQuantityCartItem',
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
        });
    } 


    //end updateSoLuong products checkout


    // Hàm gán sự kiện click cho nút "Xóa" sử dụng event delegation
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
                        url: '/ShoppingCart/DeleteCartItem',
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

    // Gọi hàm LoadListProCheckOut để tải danh sách sản phẩm khi trang được tải
    LoadListProCheckOut();
    LoadListPriceCheckOut();









});





