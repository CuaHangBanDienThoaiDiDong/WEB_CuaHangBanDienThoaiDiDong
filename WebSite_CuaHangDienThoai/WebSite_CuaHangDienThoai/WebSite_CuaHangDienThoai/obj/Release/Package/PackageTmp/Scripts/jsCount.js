$(document).ready(function () {
    /* ShowCount();*/
    ShowCountOrderNew();
    setInterval(function () {
        ShowCountOrderNew();
    }, 100);

});

function ShowCountOrderNew() {
    $.ajax({
        url: '/Admin/Order/ShowCountOrderNew',
        type: 'GET',
        success: function (rs) {
            if (rs && typeof rs.Count !== 'undefined') {
                $('#orderNew_items').html(rs.Count);
                $('#orderNew_items_warehouse').html(rs.Count); 
                $('#orderNew_items_index_li').html(rs.Count); 
            } else {
                console.error("Phản hồi không có thuộc tính Count.");
            }
        },
        error: function (xhr, status, error) {
            console.error("Lỗi khi gọi AJAX: ", error);
        }
    });
}