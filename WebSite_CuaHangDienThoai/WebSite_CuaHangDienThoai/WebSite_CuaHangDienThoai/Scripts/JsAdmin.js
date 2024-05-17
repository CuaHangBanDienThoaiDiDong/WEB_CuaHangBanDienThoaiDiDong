$(document).ready(function () {

    ShowCount();


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





//load giá tiền theo màu và dung lượng
function loadPriceForByCapcityColor(productId) {
    $.ajax({
        url: '/Admin/ProductDetail/Partial_CapacityByProductsId',
        type: 'GET',
        data: { id: productIdproductId },
        success: function (result) {

            $('.loadCapacity').html(result);
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






function ShowCount() {
    $.ajax({
        url: '/shoppingcart/ShowCount',
        type: 'GET',
        success: function (rs) {
            $('#checkout_items').html(rs.Count);
        }
    });
}
functiom


function LoadCart() {
    $.ajax({
        url: '/shoppingcart/Partial_ItemCart',
        type: 'GET',
        success: function (rs) {
            $('#load_data').html(rs);
        }
    });
}


