$(document).ready(function () {
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
        $('.btnViewPrice').removeClass('active');
        $(this).addClass('active');
        $('.btnViewPrice').removeAttr('style');
        $(this).css({
            'color': 'orangered',
            'background-color': 'white'
        });
    });

  
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


  


    $('#search-form').submit(function (e) {
        e.preventDefault();
        var searchString = $('#search').val();
        window.location.href = '/Search?searchString=' + searchString;
    });








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

      
    });
  

    /* End Tim kim san pham */



});





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