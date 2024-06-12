
    $(document).ready(function () {
        //hiển thị form upload ảnh
        LoadCapti();


 
        });




    function LoadCapti() {
            var productId = $('#loadCapacity').data('product-id');

    $.ajax({
        url: '/ProductDetail/Partial_CapacityByProductsId',
    type: 'GET',
    data: {id: productId }, // Truyền ID vào request
    success: function (rs) {
        $('#loadCapacity').html(rs.Count);
                },
    error: function (error) {
        console.log(error);
                }
            });
        }

