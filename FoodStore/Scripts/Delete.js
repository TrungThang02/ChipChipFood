$(document).on('click', '.Delete', function (e) {
    var link = $(this).attr("href");
    e.preventDefault();


    bootbox.confirm({
        title: "Xóa sản phẩm",
        message: "Bạn có chắc chắn muốn xóa nó ?",
        buttons: {
            cancel: {
                label: '<i class="fa fa-times"></i> Hủy'
            },
            confirm: {
                label: '<i class="fa fa-check"></i> Xóa'
            }
        },
        callback: function (result) {
            if (result) {
                document.location.href = link;
            }
        }
    });

})
