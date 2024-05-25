var product = {
    init: function () {
        product.registerEvents();
    },
    registerEvents: function () {
        $("#btnCommentNew").off('click').on('click', function (e) {
            e.preventDefault();

            var btn = $(this);

            var productid = btn.data('productid');

            var username = btn.data('username');
            var customerid = btn.data('customerid');
            var commentmsg = document.getElementById("txtCommentNew");
            var ratinglist = document.getElementById("ratingList");
            var rate = document.getElementById('dllRate');

            if (customerid == "") {
                bootbox.alert({
                    message: "Vui lòng đăng nhập để bình luận",
                    callback: function () {
                        window.location.href = "https://localhost:44317/User/DangNhap?id=" + productid;
                    }
                })

                return
            }
            if (String(commentmsg.value).trim() == "") {
                bootbox.alert("Chưa nhập nội dung bình luận")
                return
            }


            $.ajax({
                url: '/Products/AddNewComment',
                data: {
                    productid: productid,
                    customerid: customerid,
                    parentid: 0,
                    commentmsg: commentmsg.value,
                    rate: rate.value,
                    username: username
                },
                dataType: 'json',
                type: "GET",
                success: function (res) {

                    if (res.status == true) {
                        bootbox.alert({
                            message: "Bạn đã thêm bình luận thành công",
                            size: 'medium',
                            closeButton: false
                        });
                        $("#reviews").load("https://localhost:44317/Products/GetComment/?productid=" + productid)

                        $("#txtCommentNew").val("");
                        return false;
                    } else {
                        bootbox.alert({
                            message: "Thêm bình luận không thành công",
                            closeButton: false
                        })
                    }
                }
            })
        })
    }
}

product.init();