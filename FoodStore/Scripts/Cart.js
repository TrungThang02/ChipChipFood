var giohang = {
    init: function () {
        giohang.registerEvent();
    },
    registerEvent: function () {

        $(".quantity").on('change', function (e) {
            e.preventDefault();
            var total = 0;
            var id = parseInt($(this).data("id"));
            var quantity = parseInt($(this)[0].value)
            var productName = $(".productName")[0].outerText;
            console.log($(".quantity"))

            for (var i = 0; i < $(".quantity").length; i++) {
                total += parseInt($(".quantity")[i].value)
            }
            const that = $(this);
            $.ajax({
                url: "/GioHang/CapNhatGioHang/",
                data: { id: id, quantity: quantity },
                dataType: 'json',
                type: 'GET',
                contentType: 'application/json;charset=utf-8',
                success: function (response) {
                    console.log(response)

                    response.item.productQuantity = $(that)[0].value
                    response.item.totalPrice = parseInt(response.item.productQuantity * response.item.productPrice);
                    console.log(response.item.totalPrice)
                    $(that)[0].value = quantity;
                    $(".qty")[0].innerHTML = total;
                    //console.log(response.item.totalPrice.toLocaleString('it-IT', { style: 'currency', currency: 'VND' }))

                    that[0].value = response.item.productQuantity;
                    $("#product" + id)[0].innerHTML = response.item.totalPrice.toLocaleString('it-IT', { style: 'currency', currency: 'VND' });

                }
            })
        }),

            //xoagiohang
            $(".removeItem").on("click", function (e) {
                e.preventDefault();
                var id = parseInt($(this).data("id"));
                console.log(parseInt($(".qty")[0].innerHTML));

                $.ajax({
                    url: "/GioHang/XoaSPKhoiGioHang/",
                    data: { productId: id },
                    dataType: 'json',
                    type: 'GET',
                    contentType: 'application/json;charset=utf-8',
                    success: function (response) {
                        console.log(response);
                        toastr.success("<br /><br /><button type='button' id='confirmationButtonYes' class='btn btn-success'>Yes</button> <button type='button' id='confirmationButtonNo' class='btn btn-danger'>No</button> ", 'Bạn có chắc muốn xóa?', {
                            closeButton: false,
                            allowHtml: true,
                            onShown: function (toast) {
                                $("#confirmationButtonYes").click(function () {
                                    $(".item" + id)[0].remove();

                                    $(".qty")[0].innerHTML = parseInt($(".qty")[0].innerHTML) - parseInt(response.item.productQuantity)
                                    if (parseInt($(".qty")[0].innerHTML) == 0) {
                                        $(".giohang")[0].innerHTML = '<div style="padding : 100px 0; display : flex; flex-direction : column; align-items : center; background-color : #f5f5f5;margin-top : 20px;gap : 30px"> <img src="https://cdn.iconscout.com/icon/free/png-256/bag-1682-1137855.png" /><h4>Giỏ hàng đang trống</h4> <a class="btnBuyNow" href="https://localhost:44317/">Mua ngay</a>  </div>'
                                    }
                                });
                            }
                        });

                    }
                })
            }),

            $(".add-to-cart-btn").on("click", function (e) {
                e.preventDefault();
                var id = parseInt($(this).data("id"));
                $.ajax({
                    url: "/GioHang/ThemGioHang",
                    data: { idproduct: id },
                    dataType: "json",
                    type: "GET",
                    contentType: 'application/json;charset=utf-8',
                    success: function (response) {
                       
                            toastr.success(response.message, `${response.item.productName} đã được thêm vào giỏ hàng`, { timeOut: 3000, toastClass: 'toasr' });

                            $(".qty")[0].innerHTML = parseInt($(".qty")[0].innerHTML) + parseInt(response.item.productQuantity)

                        $(".text" + id)[0].innerHTML = "Đã thêm";
                        $("#btn" + id)[0].disabled = true
                        console.log($("#btn" + id)[0].disabled)
                    }
                })
            });
        const navitem = document.querySelectorAll(".danhmuc");
        navitem.forEach((item) => {
            item.addEventListener('click', function () {
                item.classList.add('active');
            })
        })



    }
}

giohang.init();