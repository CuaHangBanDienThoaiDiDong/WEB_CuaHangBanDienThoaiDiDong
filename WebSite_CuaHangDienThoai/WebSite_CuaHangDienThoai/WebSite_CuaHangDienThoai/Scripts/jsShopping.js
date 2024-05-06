$(document).ready(function () {

    ShowCount()



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
    $(document).ready(function () {

        $('KyThuat').on('click', function () {
            alter("dda")
        });
    });





   
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
        var tQuantity = $('#quantity_value').text();
        if (tQuantity != '') {
            soLuong = parseInt(tQuantity);
        }

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
                        location.reload(true);
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
                            title: "Thêm giỏ hàng thành công"
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

    //Start sự kiện lấy nút màu đầu tiên cho trong details
    //var firstButtonColor = $('.btnloadPricebyCapcity:first');
    //var productDetailIdloadPricebyColor = $('.btnloadPricebyCapcity:first').data('id');

    //loadPriceForByCapcityColor(productDetailIdloadPricebyColor, dungLuong);
    //firstButtonColor.css({
    //    'color': 'orangered',
    //    'background-color': 'white'
    //});

    //$('.btnloadPricebyCapcity').on('click', function () {
    //    // Loại bỏ lớp 'selected' từ tất cả các nút màu sắc
    //    $('.btnloadPricebyCapcity').removeClass('selected');

    //    // Gắn lớp 'selected' vào nút được nhấp
    //    $(this).addClass('selected');

    //    // Lấy ID của sản phẩm chi tiết từ thuộc tính data
    //    var productDetailId = $(this).data('id');

    //    // Kiểm tra xem nút hiện tại có class 'selected' hay không
    //    if ($(this).hasClass('selected')) {
    //        // Nếu có, gọi hàm loadPriceForByCapcityColor với productDetailId
    //        loadPriceForByCapcityColor(productDetailId);
    //    }
    //});
    //ENd sự kiện lấy nút màu đầu tiên cho trong details

    //Start lấy nút màu đầu tiên trong Partail_ColorByProductsId cho trnag detail
    $('.btnloadPricebyCapcity').on('click', function () {
        $('.btnloadPricebyCapcity').removeClass('selected');
        $(this).addClass('selected');
        var productDetailId = $(this).data('id');
        loadPriceForByCapcityColor(productDetailId);
    });

        // Gọi hàm chọn nút đầu tiên và gọi hàm loadPriceForByCapcityColor khi trang được tải
        selectFirstButtonAndLoadPrice();
    //End lấy nút màu đầu tiên trong Partail_ColorByProductsId cho trnag detail
});
//Start lấy nút màu đầu tiên trong Partail_ColorByProductsId cho trnag detail
function selectFirstButtonAndLoadPrice() {
   
    var firstButton = $('.btnloadPricebyCapcity').first();

   
    if (firstButton.length === 0) return;

  
    var productDetailId = firstButton.data('id');

   
    firstButton.addClass('selected');
    loadPriceForByCapcityColor(productDetailId);
}
//End lấy nút màu đầu tiên trong Partail_ColorByProductsId cho trnag detail
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






function ShowCount() {
    $.ajax({
        url: '/shoppingcart/ShowCount',
        type: 'GET',
        success: function (rs) {
            $('#checkout_items').html(rs.Count);
        }
    });
}