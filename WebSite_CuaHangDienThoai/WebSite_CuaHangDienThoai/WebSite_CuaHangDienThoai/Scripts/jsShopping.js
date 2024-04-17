$(document).ready(function () {
   /* CheckPrice();*/

    ///////////////Xem gia theo dung luowjng trang Product/index
    //$('body').on('click', '.btnViewPrice', function (e) {
    //    e.preventDefault();
    //    var id = $(this).data('id');
    //    var dungLuong = $(this).data('dungluong');
    //    console.log(id + " - " + "Dung luong:", dungLuong);
    //    CheckPrice(id, dungLuong);
    //});

    //// Lấy giá trị dung lượng của nút active từ localStorage (nếu có)
    //var activedungluong = localstorage.getitem('activedungluong');

    //// nếu có giá trị dung lượng, chọn nút có giá trị dung lượng tương ứng làm active
    //if (activedungluong) {
    //    $('.btnviewprice').removeclass('active'); // loại bỏ class active từ tất cả các button
    //    $('.btnviewprice[data-dungluong="' + activedungluong + '"]').addclass('active'); // thêm class active cho button có giá trị dung lượng tương ứng
    //}
    //else {
    //    // nếu không có giá trị dung lượng, chọn nút đầu tiên làm active
    //    var firstdungluong = $('.btnviewprice').first().data('dungluong');
    //    localstorage.setitem('activedungluong', firstdungluong); // lưu giá trị dung lượng của nút đầu tiên vào localstorage
    //    $('.btnviewprice').first().addclass('active'); // thêm class active cho nút đầu tiên
    //}

    ////// Khi bấm vào một button trong class 'btnViewPrice'
    //$('body').on('click', '.btnViewPrice', function () {
    //    // Lấy giá trị dung lượng và id của button được bấm
    //    var dungLuong = $(this).data('dungluong');
    //    var productId = $(this).data('id');
    //    console.log("Giá trị dung lượng của nút được bấm: " + dungLuong);
    //    console.log("ID sản phẩm: " + productId);

    //    // Lưu giá trị dung lượng của nút active vào localStorage
    //    localStorage.setItem('activeDungLuong', dungLuong);

    //    // Loại bỏ class 'active' từ tất cả các button trong class 'btnViewPrice'
    //    $('.btnViewPrice').removeClass('active');
    //    // Thêm class 'active' cho button được bấm
    //    $(this).addClass('active');

    //    // Thực hiện các hành động khác cần thiết với productId (ví dụ: gọi hàm CheckPrice(productId, dungLuong))
    //});


    $('.btnViewPrice').on('click', function () {
        var productId = $(this).data('id');
        var dungLuong = $(this).data('dungluong');
        loadPrice(productId, dungLuong);
        loadPhanTramSale(productId, dungLuong);

        $('.btnViewPrice').removeClass('active');
        $(this).addClass('active');
    });

});
function loadPrice(productId, dungLuong) {
    $.ajax({
        url: '/ProductDetail/PriceById',
        type: 'GET',
        data: { id: productId, DungLuong: dungLuong },
        success: function (response) {
            var targetLoadPrice = $('.loadPrice[data-id="' + productId + '"]');
            targetLoadPrice.html(response);
        }
    });
}





//function loadPhanTramSale(productId, dungLuong) {
//    $.ajax({
//        url: '/ProductDetail/Partial_PhanTramGiaGiam',
//        type: 'GET',
//        data: { id: productId, DungLuong: dungLuong },
//        success: function (response) {
//            var targetloadPhanTramSale = $('.loadPhanTramSale[data-id="' + productId + '"]');
//            targetloadPhanTramSale.html(response);
//        }
//    });
//}



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



//function CheckPrice(id, dungLuong) {
//    $.ajax({
//        url: '/ProductDetail/PriceById',
//        type: 'GET',
//        data: { id: id, dungLuong: dungLuong },
//        success: function (rs) {
//            if (rs.trim() !== '') {
//                $('#loadPrice').html(rs);
//            }
           
//        }
//    });
//}





function ShowCount() {
    $.ajax({
        url: '/shoppingcart/ShowCount',
        type: 'GET',
        success: function (rs) {
            $('#checkout_items').html(rs.Count);
        }
    });
}