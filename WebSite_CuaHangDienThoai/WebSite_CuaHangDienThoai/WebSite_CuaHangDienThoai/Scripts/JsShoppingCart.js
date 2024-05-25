$(document).ready(function () {
   


    $(".btndeleteCheckOut").on("click", function (e) {
        e.preventDefault(); // Ngăn chặn hành động mặc định của thẻ <button>

        // Lấy ProductDetailId từ thuộc tính dữ liệu (data)
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
                    url: '/ShoppingCart/DeleteCartItem', // Đảm bảo URL này đúng
                    type: 'POST',
                    data: { id: productId },
                    success: function (rs) {
                        if (rs.success) {
                            if (rs.code == 1) {
                                LoadListProCheckOut(); // Gọi hàm này để tải lại danh sách sản phẩm sau khi xóa

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
                                    title: "Xóa thành công"
                                });
                            }
                        }
                    },
                    error: function (xhr, status, error) {
                        // Xử lý lỗi nếu có
                        console.log(xhr.responseText); // In lỗi ra console để debug
                    }
                });
            }
        });

        // Gọi lại hàm attachDeleteHandlers sau khi tải lại danh sách sản phẩm

    });

    function LoadListProCheckOut() {
        $.ajax({
            url: '/ShoppingCart/Partial_ChiTietSanPhamMua', // Đảm bảo URL này đúng
            type: 'GET',
            success: function (rs) {
                $('.loadListProCheckOut').html(rs);
                attachDeleteHandlers(); // Gán lại sự kiện click cho các nút xóa
            }
        });
    }

});

