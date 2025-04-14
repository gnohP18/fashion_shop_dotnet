$(document).ready(function () {
    $(".remove-item-cart-btn").on("click", function (e) {
        e.preventDefault();

        const productId = $(this).data("product-id");

        $.ajax({
            url: `/cart/remove-in-cart?productId=${productId}`,
            type: "DELETE",
            success: function (response) {
                $(`#product_row_${productId}`).remove();

                const remainingItems = $(".product-row").length;
                console.log(remainingItems);

                if (remainingItems === 0) {
                    $("#checkoutBtn").prop("disabled", true);
                }
            },
            error: function () {
                alert("Thêm vào giỏ hàng thất bại!");
            }
        });
    });
})