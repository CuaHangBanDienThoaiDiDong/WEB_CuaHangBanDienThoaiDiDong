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

    //$('.sortProductCategory').on('click', function () {
    //    var category = $(this).data('filter');
     
    //    sortCategory(category);
    //    loadPhanTramSale(productId, dungLuong);

    //    $('.sortProductCategory').removeClass('active');
    //    $(this).addClass('active');
    //});

});
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