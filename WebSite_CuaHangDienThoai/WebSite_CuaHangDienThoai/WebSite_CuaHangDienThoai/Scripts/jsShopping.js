$(document).ready(function () {
    var firstButton = $('.btnViewPrice:first');
    var productId = $('.btnViewPrice:first').data('id');
    var dungLuong = $('.btnViewPrice:first').data('dungluong');

    // Tải giá sản phẩm khi trang được tải
    loadPrice(productId, dungLuong);
    firstButton.css({
        'color': 'orangered',
        'background-color': 'white'
    });
    $('.product-item').each(function () {
        // Lấy ra nút đầu tiên trong khung sản phẩm hiện tại
        var firstButton = $(this).find('.btnViewPrice:first');
        // Nếu nút đầu tiên được tìm thấy
        if (firstButton.length > 0) {
            // Lấy dữ liệu từ nút đầu tiên
            var productId = firstButton.data('id');
            var dungLuong = firstButton.data('dungluong');
            // Tải giá sản phẩm cho nút đầu tiên
            loadPrice(productId, dungLuong);
            // Áp dụng CSS cho nút đầu tiên
            firstButton.css({
                'color': 'orangered',
                'background-color': 'white'
            });
        }
    });


    //$('.btnViewPrice').on('click', function () {
    //    var productId = $(this).data('id');
    //    var dungLuong = $(this).data('dungluong');
    //    loadPrice(productId, dungLuong);
    //    loadPhanTramSale(productId, dungLuong);

    //    $('.btnViewPrice').removeClass('active');
    //    $(this).addClass('active');
    //    // Xóa CSS cho tất cả các button và thêm CSS cho button được nhấn
    //    $('.btnViewPrice').removeAttr('style');
    //    $(this).css({
    //        'color': 'orangered',
    //        'background-color': 'white'
    //    });
    //});


    $('.btnViewPrice').on('click', function () {
        var productId = $(this).data('id');
        var dungLuong = $(this).data('dungluong');
        loadPrice(productId, dungLuong);
        loadPhanTramSale(productId, dungLuong);
        // Loại bỏ lớp active từ tất cả các nút
        $('.btnViewPrice').removeClass('active');
        // Thêm lớp active cho nút được nhấp
        $(this).addClass('active');
        // Loại bỏ CSS từ tất cả các nút và thêm CSS cho nút được nhấp
        $('.btnViewPrice').removeAttr('style');
        $(this).css({
            'color': 'orangered',
            'background-color': 'white'
        });
    });

    //$(document).ready(function () {
    //    // Bắt sự kiện click vào mỗi thẻ <li>
    //    $('.singleProductthumbnails').on('click', 'li', function () {
    //        // Lấy giá trị của thuộc tính data-id
    //        var productDetailId = $(this).data('id');
    //        // Gọi hàm loadImages với productDetailId đã lấy được
    //        loadImages(productDetailId);
    //    });
    //});
    //$(document).ready(function () {
    //    // Kích hoạt sự kiện click trên li đầu tiên
    //    $('.singleProductthumbnails li:first-child').click();
    //});

    $(document).ready(function () {
        // Bắt sự kiện click vào mỗi thẻ <li> có lớp CSS là 'liImg'
        $('.liImg').on('click', function () {
            // Lấy giá trị của thuộc tính data-id
            var productDetailId = $(this).data('id');
            // Gọi hàm loadImages với productDetailId đã lấy được
            loadImages(productDetailId);
        });
    });

    $(document).ready(function () {

        $('KyThuat').on('click', function () {
            alter("dda")
        });
    });


    $(document).ready(function () {
        // Kích hoạt sự kiện click trên li đầu tiên
        $('.liImg:first-child').click();
    });


    $('#search-form').submit(function (e) {
        e.preventDefault();
        var searchString = $('#search').val();
        window.location.href = '/Search?searchString=' + searchString;
    });
        

  /* Start Tim kim san pham */
    $(document).ready(function () {
        var suggestUrl = '@Url.Action("Suggest", "Product")'; 
        $('#search').keypress(function (e) {
            if (e.which == 13) {
                e.preventDefault();
                var searchString = $(this).val();
                window.location.href = '/Search?searchString=' + searchString;
            }
        });

        // AJAX gợi ý sản phẩm
        // AJAX gợi ý sản phẩm
        $('#search').on('input', function () {
            $.ajax({


            });
            var searchString = $(this).val().trim();
            clearTimeout(timeout); // Xóa bỏ timeout trước đó
            if (searchString !== '') {
                // Đặt một timeout mới
                timeout = setTimeout(function () {
                    $.ajax({
                        url: suggestUrl,
                        type: 'GET',
                        data: { searchString: searchString },
                        success: function (data) {
                            var suggestionsHtml = '';
                            $.each(data, function (index, product) {
                                suggestionsHtml += '<div class="col-md-12">';
                                suggestionsHtml += '<div class="me-lg-5">';
                                suggestionsHtml += '<div class="d-flex">';
                                suggestionsHtml += '<img src="' + product.Image + '" style="width: 10%; height: 10%" />';
                                suggestionsHtml += '<div class="">';
                                suggestionsHtml += '<div class="text-img">';
                                suggestionsHtml += '<span>' + product.Title + '</span>';
                                suggestionsHtml += '<p>';
                                suggestionsHtml += '<h5>' + product.Price + '</h5>';
                                suggestionsHtml += '<strike>' + product.OldPrice + '</strike> -' + product.Discount + '%';
                                suggestionsHtml += '</p>';
                                suggestionsHtml += '</div>';
                                suggestionsHtml += '</div>';
                                suggestionsHtml += '</div>';
                                suggestionsHtml += '</div>';
                                suggestionsHtml += '</div>';
                            });
                            $('.productsuggestions').html(suggestionsHtml);
                        }
                    });
                }, 2000); // Khoảng thời gian chờ (ms)
            }
        });
    });
    $(document).ready(function () {
        var suggestUrl = '/Product/Suggest';
        $('#search').keyup(function () {
            var searchString = $(this).val().trim();
            if (searchString !== '') {
                $.ajax({
                    url: suggestUrl,
                    type: 'GET',
                    data: { searchString: searchString },
                    success: function (data) {
                        var suggestionsHtml = '';
                        $.each(data, function (index, product) {
                            suggestionsHtml += '<div class="col-md-12">';
                            suggestionsHtml += '<div class="me-lg-5">';
                            suggestionsHtml += '<div class="d-flex">';
                            suggestionsHtml += '<img src="' + product.Image + '" style="width: 10%; height: 10%" />';
                            suggestionsHtml += '<div class="">';
                            suggestionsHtml += '<div class="text-img">';
                            suggestionsHtml += '<span>' + product.Title + '</span>';
                            suggestionsHtml += '<p>';
                            suggestionsHtml += '<h5>' + product.Price + '</h5>';
                            suggestionsHtml += '<strike>' + product.OldPrice + '</strike> -' + product.Discount + '%';
                            suggestionsHtml += '</p>';
                            suggestionsHtml += '</div>';
                            suggestionsHtml += '</div>';
                            suggestionsHtml += '</div>';
                            suggestionsHtml += '</div>';
                            suggestionsHtml += '</div>';
                        });
                        $('.productsuggestions').html(suggestionsHtml);
                    }
                });
            }
        });
    });


    /* End Tim kim san pham */



});
function loadImages(productDetailId) {
    $.ajax({
        url: '/Product/Partail_LoadListImgByProDetailId',
        type: 'GET',
        data: { id: productDetailId },
        success: function (result) {
            // Cập nhật phần tử có class loadImgByProductDetailId với kết quả từ controller
            $('.loadImgByProductDetailId').html(result);
        },
        error: function () {
            alert('Đã xảy ra lỗi khi tải danh sách hình ảnh.');
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
//function loadPrice(productId, dungLuong) {
//    $.ajax({
//        url: '/ProductDetail/PriceById',
//        type: 'GET',
//        data: { id: productId, DungLuong: dungLuong },
//        success: function (response) {
//            var targetLoadPrice = $('.loadPrice[data-id="' + productId + '"]');
//            targetLoadPrice.html(response);
//        }
//    });
//}

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





function loadPhanTramSale(productId, dungLuong) {
    $.ajax({
        url: '/ProductDetail/Partial_PhanTramGiaGiam',
        type: 'GET',
        data: { id: productId, DungLuong: dungLuong },
        success: function (response) {
            var targetloadPhanTramSale = $('.loadPhanTramSale[data-id="' + productId + '"]');
            if (response.trim() !== "") { // Kiểm tra xem có dữ liệu được trả về không
                targetloadPhanTramSale.html(response).css('visibility', 'visible'); // Thiết lập visibility thành visible
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