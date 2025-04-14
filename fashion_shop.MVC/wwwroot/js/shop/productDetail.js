$(document).ready(function () {
    $(".add-to-cart-btn").on("click", function (e) {
        e.preventDefault();

        const productId = $(this).data("product-id");

        $.ajax({
            url: `/cart/add-to-cart?productId=${productId}`,
            type: "GET",
            success: function (response) {
                //    
                console.log(productId);

            },
            error: function () {
                alert("Thêm vào giỏ hàng thất bại!");
            }
        });
    });
});
