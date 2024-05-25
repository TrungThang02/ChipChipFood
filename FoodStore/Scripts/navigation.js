$(".danhmuc").on('click', function (e) {
    e.preventDefault();
    $(".danhmuc").removeClass("active");
    $(this).addClass('active');
})