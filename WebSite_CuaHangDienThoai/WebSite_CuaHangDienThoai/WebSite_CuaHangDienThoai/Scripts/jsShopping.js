$(document).ready(function () {

    ShowCount();

  

    // Set interval to update count every 5 seconds
    setInterval(function () {
        updateUnreadMessagesCount();
    }, 3000);

    // Gọi hàm chọn nút đầu tiên và gọi hàm loadPriceForByCapcityColor khi trang được tải
    selectFirstButtonAndLoadPrice();

    var firstButton = $('.btnViewPrice:first');
    var productId = $('.btnViewPrice:first').data('id');
    var dungLuong = $('.btnViewPrice:first').data('dungluong');
    loadPrice(productId, dungLuong);
    firstButton.css({
        'color': 'orangered',
        'background-color': 'white'
    });








    $('.product-item').each(function () {
        var firstButton = $(this).find('.btnViewPrice:first');
        if (firstButton.length > 0) {
            var productId = firstButton.data('id');
            var dungLuong = firstButton.data('dungluong');
            loadPrice(productId, dungLuong);
            firstButton.css({
                'color': 'orangered',
                'background-color': 'white'
            });
        }
    });





    //start nut hiện thị tiền theo  dung lượng cho trang Menu Alin
    $('.btnViewPrice').on('click', function () {
        var productId = $(this).data('id');
        var dungLuong = $(this).data('dungluong');
        loadPrice(productId, dungLuong);
        loadPhanTramSale(productId, dungLuong);
        $('.btnViewPrice').removeClass('active');
        $(this).addClass('active');
        $('.btnViewPrice').removeAttr('style');
        $(this).css({
            'color': 'orangered',
            'background-color': 'white'
        });
    });
    //End nut hiện thị tiền theo  dung lượng cho trang Menu Alin

    /* Start div lọc ảnh Products Partial_DetailImageById*/
    $(document).ready(function () {
        $('.itemTab').on('click', '.liImg', function () {
            var productDetailId = $(this).data('id');
            loadImages(productDetailId);
        });
        $('.itemTab .liImg:first-child').click();
    });
    /* End div lọc ảnh Products Partial_DetailImageById*/



   
 //start Hiện thị gợi ý sản phẩm
    $(document).ready(function () {
        $("#searchString").on("input", function () {
            var inputValue = $(this).val().trim();
            if (inputValue !== "") {
                $.ajax({
                    url: "/Product/Suggest",
                    type: "GET",
                    data: { searchString: inputValue },
                    success: function (response) {

                        $(".search-suggestions").html(response);
                    },
                    error: function (xhr, status, error) {

                        console.error(xhr.responseText);
                    }
                });
            }
        });
    });
    //End Hiện thị gợi ý sản phẩm

    //start ap ma giam gia
    $(document).ready(function () {
        var typingTimer;
        var doneTypingInterval = 1500;

        //$("#voucherCode").on("input", function () {
        //    clearTimeout(typingTimer); // Xóa timeout

        //    var voucherCode = $(this).val().trim();
        //    if (voucherCode.length >= 5) {
        //        typingTimer = setTimeout(function () {
        //            $.ajax({
        //                url: '/ShoppingCart/GetVoucher',
        //                type: 'GET',
        //                dataType: 'json',
        //                data: { Code: voucherCode },
        //                success: function (voucher) {
        //                    if (voucher.length > 0) {
        //                        var percentPriceReduction = voucher[0].PercentPriceReduction;
        //                        showVoucherInfo(voucher[0].Title, voucher[0].CreatedBy, voucher[0].CreatedDate, percentPriceReduction);

        //                        $(".btnApllyVoucher").removeClass("d-none").attr("data-percent", percentPriceReduction).attr("data-code", voucherCode);
        //                        $(".btnRemoveVoucher").removeClass("d-none");
        //                    } else {
        //                        $(".loadVoucher").html("<p>Không tìm thấy mã giảm giá</p>");
        //                        $(".btnApllyVoucher").addClass("d-none").attr("data-percent", "0");
        //                        $(".btnRemoveVoucher").addClass("d-none");
        //                    }
        //                },
        //                error: function () {
        //                    console.log("Lỗi khi gửi yêu cầu kiểm tra mã giảm giá.");
        //                }
        //            });
        //        }, doneTypingInterval);
        //    }
        //});
        $("#voucherCode").on("input", function () {
            clearTimeout(typingTimer); // Xóa timeout trước đó

            var doneTypingInterval = 500; // Độ trễ để gửi request (milisecond)
            typingTimer = setTimeout(function () {
                var voucherCode = $("#voucherCode").val().trim();
                if (voucherCode.length >= 5) {
                    $.ajax({
                        url: '/ShoppingCart/GetVoucher',
                        type: 'GET',
                        dataType: 'json',
                        data: { Code: voucherCode },
                        success: function (voucher) {
                            if (voucher.length > 0) {
                                var percentPriceReduction = voucher[0].PercentPriceReduction;
                                showVoucherInfo(voucher[0].Title, percentPriceReduction);

                                var currentDate = new Date(); // Ngày hiện tại
                                var startDate = voucher[0].UsedDate ? new Date(voucher[0].UsedDate) : null; // Ngày bắt đầu
                                var endDate = voucher[0].ModifiedDate ? new Date(voucher[0].ModifiedDate) : null; // Ngày kết thúc

                                if (voucher[0].Status === true) {
                                    $(".loadVoucher").html('<p class="text-danger">Mã giảm giá đã được sử dụng</p>');
                                    $(".btnApllyVoucher, .btnRemoveVoucher").addClass("d-none");
                                } else if (startDate && endDate && currentDate >= startDate && currentDate <= endDate) {
                                    $(".loadVoucher").html('<p class="text-success">Mã giảm giá còn hạn sử dụng</p>');
                                    $(".btnApllyVoucher").removeClass("d-none").attr("data-percent", percentPriceReduction).attr("data-code", voucherCode);
                                    $(".btnRemoveVoucher").removeClass("d-none");
                                } else if (currentDate < startDate) {
                                    $(".loadVoucher").html('<p>Không tìm thấy mã giảm giá hoặc mã giảm giá chưa bắt đầu</p>');
                                    $(".btnApllyVoucher").addClass("d-none").attr("data-percent", "0").attr("data-code", "");
                                    $(".btnRemoveVoucher").addClass("d-none");
                                } else if (currentDate > endDate) {
                                    $(".loadVoucher").html('<p class="text-danger">Mã giảm giá đã hết hạn sử dụng</p>');
                                    $(".btnApllyVoucher, .btnRemoveVoucher").addClass("d-none");
                                }
                            } else {
                                $(".loadVoucher").html("<p>Không tìm thấy mã giảm giá</p>");
                                $(".btnApllyVoucher").addClass("d-none").attr("data-percent", "0").attr("data-code", "");
                                $(".btnRemoveVoucher").addClass("d-none");
                            }
                        },
                        error: function () {
                            console.log("Lỗi khi gửi yêu cầu kiểm tra mã giảm giá.");
                            // Xử lý lỗi ở đây
                        }
                    });
                } else {
                    // Nếu input trống, ẩn các nút áp dụng và loại bỏ mã giảm giá
                    $(".loadVoucher").html("");
                    $(".btnApllyVoucher, .btnRemoveVoucher").addClass("d-none");
                }
            }, doneTypingInterval);
        });
        $("#voucherCode").on("input", function () {
            clearTimeout(typingTimer); // Xóa timeout nếu có

            var doneTypingInterval = 500; // Độ trễ để gửi request (milisecond)
            typingTimer = setTimeout(function () {
                var voucherCode = $("#voucherCode").val().trim();
                if (voucherCode.length >= 5) {
                    $.ajax({
                        url: '/ShoppingCart/GetVoucher',
                        type: 'GET',
                        dataType: 'json',
                        data: { Code: voucherCode },
                        success: function (data) {
                            if (data.hasOwnProperty('error')) {
                                // Xử lý khi không tìm thấy mã giảm giá
                                $(".loadVoucher").html("<p>" + data.error + "</p>");
                                $(".btnApplyVoucher").addClass("d-none").attr("data-percent", "0").attr("data-code", "");
                                $(".btnRemoveVoucher").addClass("d-none");
                            } else {
                                // Hiển thị thông tin mã giảm giá
                                var percentPriceReduction = data.PercentPriceReduction;
                                showVoucherInfo(data.Title, percentPriceReduction);

                                var currentDate = new Date(); // Lấy ngày hiện tại

                                var usedDate = data.UsedDate ? new Date(data.UsedDate) : null;
                                var modifiedDate = data.ModifiedDate ? new Date(data.ModifiedDate) : null;

                                if (data.Status === true) {
                                    $(".loadVoucher").html('<p class="text-danger">Mã giảm giá đã được sử dụng</p>');
                                    $(".btnApplyVoucher, .btnRemoveVoucher").addClass("d-none");
                                } else if (usedDate && modifiedDate && currentDate <= modifiedDate) {
                                    $(".loadVoucher").html('<p class="text-success">Mã giảm giá còn hạn sử dụng</p>');
                                    $(".btnApllyVoucher").removeClass("d-none").attr("data-percent", percentPriceReduction).attr("data-code", voucherCode);
                                    $(".btnRemoveVoucher").removeClass("d-none");
                                } else {
                                    $(".loadVoucher").html('<p class="text-danger">Mã giảm giá đã hết hạn sử dụng</p>');
                                    $(".btnApplyVoucher, .btnRemoveVoucher").addClass("d-none");
                                }
                            }
                        },
                        error: function () {
                            console.log("Lỗi khi gửi yêu cầu kiểm tra mã giảm giá.");
                            // Xử lý lỗi khi gọi API
                            $(".loadVoucher").html("<p>Có lỗi xảy ra khi kiểm tra mã giảm giá. Vui lòng thử lại sau.</p>");
                            $(".btnApplyVoucher").addClass("d-none").attr("data-percent", "0").attr("data-code", "");
                            $(".btnRemoveVoucher").addClass("d-none");
                        }
                    });

                } else {
                    // Nếu input trống, ẩn các nút áp dụng và nút xóa
                    $(".loadVoucher").html("");
                    $(".btnApplyVoucher, .btnRemoveVoucher").addClass("d-none");
                }
            }, doneTypingInterval); // Thực hiện hành động sau khi nhập xong sau 500ms
        });






        function showVoucherInfo(title, percentPriceReduction) {
            var voucherInfo = `
        <div class="d-flex justify-content-between">
            <p class="mb-2">Chương trình giảm: ${title}</p>
            <p class="mb-2 text-success">${percentPriceReduction}% / đơn hàng</p>
        </div>
    `;
            $(".loadVoucher").html(voucherInfo);
        }

    //    function showVoucherInfo(title, percentPriceReduction) {
    //        var voucherInfo = `
    //    <div class="d-flex justify-content-between">
    //        <p class="mb-2">Chương trình giảm: ${title}</p>
    //        <p class="mb-2 text-success">${percentPriceReduction}% / đơn hàng</p>
    //    </div>
    //`;
    //        $(".btnApllyVoucher").removeClass("d-none").attr("data-percent", percentPriceReduction).attr("data-code", voucherCode);
    //        $(".btnRemoveVoucher").removeClass("d-none");
    //        $(".loadVoucher").html(voucherInfo);
    //    }



        function updatePriceWithDiscount(percentPriceReduction,code ) {
            $.ajax({
                url: '/ShoppingCart/UpdateTotalPriceShoppingCartItem',
                type: 'POST',
                data: { PercentPriceReduction: percentPriceReduction, Code: code },
                success: function (data) {
                    if (data.Success) {
                        LoadListPriceCheckOut();
                        LoadListProCheckOut();
                    } else {
                        console.log("Lỗi khi cập nhật giá mục trong giỏ hàng.");
                    }
                },
                error: function () {
                    console.log("Lỗi khi gửi yêu cầu cập nhật giá mục trong giỏ hàng.");
                }
            });
        }

        function removeVoucher() {
            $.ajax({
                url: '/ShoppingCart/RemoveVoucher',
                type: 'POST',
                success: function (data) {
                    if (data.Success) {
                        LoadListPriceCheckOut();
                        LoadListProCheckOut();
                        $(".loadVoucher").html("<p>Không có mã giảm giá</p>");
                        $(".btnApllyVoucher").addClass("d-none").attr("data-percent", "0");
                        $(".btnRemoveVoucher").addClass("d-none");
                        $("#voucherCode").val('');
                    } else {
                        console.log("Lỗi khi xóa mã giảm giá.");
                    }
                },
                error: function () {
                    console.log("Lỗi khi gửi yêu cầu xóa mã giảm giá.");
                }
            });
        }

        $(".btnApllyVoucher").on("click", function () {
            var percentPriceReduction = $(this).attr("data-percent");
            var code = $(this).attr("data-code");
            updatePriceWithDiscount(percentPriceReduction, code);
            $(".btnApplyVoucher").addClass("d-none").attr("data-percent", "0").attr("data-code", "");
        });

        $(".btnRemoveVoucher").on("click", function () {
            removeVoucher();
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

        $("#voucherCode").keydown(function (event) {
            if (event.keyCode == 13) {
                event.preventDefault();
                applyVoucher();
                return false;
            }
        });
    });









    //End ap ma giam gia

    /* Start Tim kim san pham */
    $(document).ready(function () {
        var suggestUrl = '@Url.Action("Suggest", "Product")';
        $('#searchString').keypress(function (e) {
            if (e.which == 13) {
                e.preventDefault();
                var key = $(this).val();
                window.location.href = '/tim-kiem/key=' + key;
            }
        });


    });



    ///* Start Tim kim san pham */
    //$(document).ready(function () {
    //    var suggestUrl = '@Url.Action("Suggest", "Product")';
    //    $('#search').keypress(function (e) {
    //        if (e.which == 13) {
    //            e.preventDefault();
    //            var searchString = $(this).val();
    //            window.location.href = '/Search?searchString=' + searchString;
    //        }
    //    });


    //});


    /* End Tim kim san pham */


    //start nust thêm giỏ hàng btnAddtoCart

    $('body').on('click', '.btnAddtoCart', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var soLuong = 1;
        $.ajax({
            url: '/Shoppingcart/addtocart',
            type: 'POST',
            data: { id: id, soLuong: soLuong },
            success: function (rs) {
                if (rs.Success) {
                    /*  $('#checkout_items').html(rs.Count);*/
                    /*alert(rs.msg);*/
                    if (rs.code == 1) {
                        ShowCount();

                        Swal.fire({
                            position: "top-end",
                            icon: "success",
                            title: "Thêm giỏ hàng thành công",
                            showConfirmButton: false,
                            timer: 1100,
                            customClass: {
                                container: 'swal2-container-custom',
                                popup: 'swal2-popup-custom'
                            }
                        });
                    }
                }
                else {
                    if (rs.code == -2) {

                        location.href = "/Account/Login";
                    }
                }
            }
        });
    });

    //end btn thêm giỏ hàng btnAddtoCart

    //Start btn xoá sản phẩm giỏ hàng \

   








    $('body').on('click', '.btnDelete', function (e) {
        e.preventDefault();
        var id = $(this).data('id');

        Swal.fire({
            title: "Bạn muốn xoá?",
            text: "Bạn muốn xoá sản phẩm ra khỏi giỏ hàng!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Đồng ý, xoá!"
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/ShoppingCart/Delete',
                    type: 'POST',
                    data: { id: id },
                    success: function (rs) {
                        if (rs.Success) {
                            if (rs.Code == 1) {
                                LoadCart();
                                ShowCount();
                                /*alert("Xóa thành công");*/
                                 Swal.fire({
                                     position: "top-end",
                                     icon: "success",
                                     title: "Xoá sản phẩm thành công",
                                     showConfirmButton: false,
                                     timer: 1100,
                                     customClass: {
                                         container: 'swal2-container-custom',
                                         popup: 'swal2-popup-custom'
                                     }
                                 });
                            }
                        } else {
                            if (rs.Code == -1) {
                                Swal.fire({
                                    icon: "error",
                                    title: "Lỗi",
                                    text: "Lỗi hệ thống máy chủ",
                                    footer: '<a href="/dang-nhap">Quay về đăng nhập?</a>'
                                });
                            }
                            if (rs.Code == -2) {
                                Swal.fire({
                                    icon: "error",
                                    title: "Lỗi",
                                    text: "Lỗi hệ thống máy chủ",
                                    footer: '<a href="/dang-nhap">Quay về đăng nhập?</a>'
                                });
                            }
                            if (rs.Code == -3) {
                                Swal.fire({
                                    icon: "error",
                                    title: "Giỏ hàng bị lỗi",
                                    text: "Sản phẩm không tồn tại trong giỏ hàng",
                                    footer: '<a href="/gio-hang">Quay về giỏ hàng?</a>',
                                    customClass: {
                                        container: 'swal2-container-custom',
                                        popup: 'swal2-popup-custom'
                                    }
                                });
                            }
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        Swal.fire({
                            icon: "error",
                            title: "Lỗi",
                            text: "Lỗi khi kết nối đến máy chủ",
                            footer: '<a href="/dang-nhap">Quay về đăng nhập?</a>'
                        });
                    }
                });
            }
        });
    });

    //nd btn xoá sản phẩm giỏ hàng 






    //Start btn cập nhập số lượng trong giỏ hàng 

    $('body').on('input', '.Quantity', function (e) {
        var productId = $(this).attr('id');
        var newQuantity = $(this).val();
        if (newQuantity <= 0) {

            Swal.fire({
                title: "Bạn muốn xoá?",
                text: "Bạn muốn xoá sản phẩm ra khỏi giỏ hàng!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Đồng ý, xoá!"
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/ShoppingCart/Delete',
                        type: 'POST',
                        data: { id: productId },
                        success: function (rs) {
                            if (rs.Success) {
                                if (rs.Code == 1) {
                                    LoadCart();
                                    ShowCount();
                                    /*alert("Xóa thành công");*/
                                    Swal.fire({
                                        position: "top-end",
                                        icon: "success",
                                        title: "Xoá sản phẩm thành công",
                                        showConfirmButton: false,
                                        timer: 1100,
                                        customClass: {
                                            container: 'swal2-container-custom',
                                            popup: 'swal2-popup-custom'
                                        }
                                    });
                                }
                            } else {
                                if (rs.Code == -1) {
                                    Swal.fire({
                                        icon: "error",
                                        title: "Lỗi",
                                        text: "Lỗi hệ thống máy chủ",
                                        footer: '<a href="/dang-nhap">Quay về đăng nhập?</a>'
                                    });
                                }
                                if (rs.Code == -2) {
                                    Swal.fire({
                                        icon: "error",
                                        title: "Lỗi",
                                        text: "Lỗi hệ thống máy chủ",
                                        footer: '<a href="/dang-nhap">Quay về đăng nhập?</a>'
                                    });
                                }
                                if (rs.Code == -3) {
                                    Swal.fire({
                                        icon: "error",
                                        title: "Giỏ hàng bị lỗi",
                                        text: "Sản phẩm không tồn tại trong giỏ hàng",
                                        footer: '<a href="/gio-hang">Quay về giỏ hàng?</a>',
                                        customClass: {
                                            container: 'swal2-container-custom',
                                            popup: 'swal2-popup-custom'
                                        }
                                    });
                                }
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            Swal.fire({
                                icon: "error",
                                title: "Lỗi",
                                text: "Lỗi khi kết nối đến máy chủ",
                                footer: '<a href="/dang-nhap">Quay về đăng nhập?</a>'
                            });
                        }
                    });
                }
                else {
                    LoadCart();
                }
            });
        }
        else  {
            $.ajax({
                type: 'POST',
                url: '/ShoppingCart/UpdateQuantity',
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

    //End btn cập nhập số lượng trong giỏ hàng

    //start nut hiện thị tiền theo màu dung lượng cho trang detail


    $('.btnloadPricebyCapcity').on('click', function () {
        var productDetailIdloadPricebyColor = $(this).data('id');

        loadPriceForByCapcityColor(productDetailIdloadPricebyColor)
    });

    //End nut hiện thị tiền theo màu dung lượng cho trang detail

   

    //Start lấy nút màu đầu tiên trong Partail_ColorByProductsId cho trnag detail
    $('.btnloadPricebyCapcity').on('click', function () {
        $('.btnloadPricebyCapcity').removeClass('selected');
        $(this).addClass('selected');
        var productDetailId = $(this).data('id');
        loadPriceForByCapcityColor(productDetailId);
    });

    //End lấy nút màu đầu tiên trong Partail_ColorByProductsId cho trnag detail

    //Lay Id cho checkbox
    function getSelectedProductIds() {
        var selectedProductIds = [];
        $('.cbkItem:checked').each(function () {
            selectedProductIds.push($(this).data('id'));
        });
        return selectedProductIds;
    }

    //Start dưaht hàng
    $('.btn-dat-hang').on('click', function () {

        // Lấy danh sách ProductId từ các checkbox đã chọn
        var selectedProductIds = getSelectedProductIds();
        if (selectedProductIds != null) {
            sendAjaxRequest(selectedProductIds);
        }
        else {

        }

    });
    //End dưaht hàng list_Provinces_item
    //Start Load dropdown Phuong Xa
        $('.loadWardsBtn').click(function () {
            var id = $(this).data('id');
            $.ajax({
                url: '/ProvincesVN/Partial_Wards',
                type: 'GET',
                data: { id: id },
                success: function (result) {
                    $('.loadDataWards').html(result);
                },
                error: function (xhr, status, error) {
                    console.log(xhr.responseText);
                }
            });
        });
    //End Load dropdown Phuong Xa
    //Start Load dropdown Quaajn huyeejn 
    $('.dropdowm_list_Provinces_item').on('click', function () {
        var provincesName = $(this).text();
        $('.provincesNameChose').text(provincesName);
        var id = $(this).data('id');
        $.ajax({
            url: '/ProvincesVN/Partial_Districts',
            type: 'GET',
            data: { id: id },
            success: function (result) {
                $('.loadDataDistricts').html(result);
            },
        });
    });
    //End Load dropdown Quaajn huyeejn

   
});


//Start dat hang khi gui list
//gui list vao dat hang
function sendAjaxRequest(selectedProductIds) {

    $.ajax({
        url: '/ShoppingCart/DatHang',
        type: 'POST',
        data: { productIds: selectedProductIds },
        dataType: 'json',
        success: function (result) {
            // Xử lý kết quả từ server nếu cần
            if (result.Success) {
                console.log('Đặt hàng thành công');
                /*window.location.href = '/thanh-toan';*/
                window.location.href = '/thanh-toan';
               
            } else {
                if (result.code == -2) {

                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: "Vui lòng chọn sản phẩm",

                    });

                }
            }
        },
        error: function (error) {
            console.error('Lỗi khi gọi API:', error);
        }
    });
}

//End dat hang khi gui list




//Start lấy nút màu đầu tiên trong Partail_ColorByProductsId cho trnag detail
function selectFirstButtonAndLoadPrice() {
   
    var firstButton = $('.btnloadPricebyCapcity').first();

   
    if (firstButton.length === 0) return;

  
    var productDetailId = firstButton.data('id');
    
   
    firstButton.addClass('selected');
    loadPriceForByCapcityColor(productDetailId);
}
//End lấy nút màu đầu tiên trong Partail_ColorByProductsId cho trnag detaila
//load giá tiền theo màu và dung lượng
function loadPriceForByCapcityColor(productDetailId) {
    $.ajax({
        url: '/Product/Partial_LoadPriceForBoxSaving',
        type: 'GET',
        data: { id: productDetailId },
        success: function (result) {

            $('.loadPriceByColorCapcity').html(result);
        },
        error: function () {
            alert('Đã xảy ra lỗi khi tải tiền.');
        }
    });
}



function loadImages(productDetailId) {
    $.ajax({
        url: '/Product/Partail_LoadListImgByProDetailId',
        type: 'GET',
        data: { id: productDetailId },
        success: function (result) {
           
            $('.loadImgByProductDetailId').html(result);
        },
        error: function () {
            alert('Đã xảy ra lỗi khi tải danh sách hình ảnh.');
        }
    });
}

function loadPriceForDetailProduct(productId, dungLuong) {
    $.ajax({
        url: '/ProductDetail/PriceById',
        type: 'GET',
        data: { id: productId, DungLuong: dungLuong },
        success: function (response) {
            loadPhanTramSale(productId, dungLuong)
            var targetLoadPrice = $('.loadPrice[data-id="' + productId + '"]');
            targetLoadPrice.html(response);
        }
    });
}


function loadPrice(productId, dungLuong) {
    $.ajax({
        url: '/ProductDetail/PriceById',
        type: 'GET',
        data: { id: productId, DungLuong: dungLuong },
        success: function (response) {
            loadPhanTramSale(productId, dungLuong)
            var targetLoadPrice = $('.loadPrice[data-id="' + productId + '"]');
            targetLoadPrice.html(response);
        }
    });
}

function sortCategory(category) {
    $.ajax({
        url: '/ProductCategory/Partial_LapTop',
        type: 'GET',
        data: { id: productId, DungLuong: dungLuong },
        success: function (response) {
            var targetLoadPrice = $('.loadPrice[data-id="' + productId + '"]');
            targetLoadPrice.html(response);
        }
    });
}


function ShowCount() {
    $.ajax({
        url: '/ShoppingCart/ShowCount',
        type: 'GET',
        success: function (rs) {
            $('#checkout_items').html(rs.Count);
        }
    });
}



function loadPhanTramSale(productId, dungLuong) {
    $.ajax({
        url: '/ProductDetail/Partial_PhanTramGiaGiam',
        type: 'GET',
        data: { id: productId, DungLuong: dungLuong },
        success: function (response) {
            var targetloadPhanTramSale = $('.loadPhanTramSale[data-id="' + productId + '"]');
            if (response.trim() !== "") { 
                targetloadPhanTramSale.html(response).css('visibility', 'visible'); 
            }
        },
        error: function () {
            console.log("Có lỗi xảy ra trong quá trình gọi Ajax.");
        }
    });
}




function updateUnreadMessagesCount() {
    $.ajax({
        url: '/Chat/ShowCountMess', 
        type: 'GET',
        success: function (response) {
            // Update unread messages count in the span
            $('#unreadMessagesCount').text(response.Count);
        },
        error: function (xhr, status, error) {
            console.error('Error while fetching unread messages count:', error);
        }
    });
}

// Call the function initially



function ShowCount() {
    $.ajax({
        url: '/shoppingcart/ShowCount',
        type: 'GET',
        success: function (rs) {
            $('#checkout_items').html(rs.Count);
        }
    });
}


function LoadCart() {
    $.ajax({
        url: '/shoppingcart/Partial_ItemCart',

        type: 'GET',
        success: function (rs) {
            $('#load_data').html(rs);
        }
    });
}


