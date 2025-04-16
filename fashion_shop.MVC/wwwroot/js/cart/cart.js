$(document).ready(function () {
    const calTotal = () => {
        let total = 0;

        const productRows = $(".product-row");

        productRows.each((index, el) => {
            const price = parseFloat($(el).find('input[name="product_price"]').val());
            const quantity = parseInt($(el).find('input[name="product_quantity"]').val());

            if (!isNaN(price) && !isNaN(quantity)) {
                total += price * quantity;
            }
        });

        $("#total").text(total.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' }));

        console.log(productRows.lenght);
    }

    $(".remove-item-cart-btn").on("click", function (e) {
        e.preventDefault();

        const productId = $(this).data("product-id");

        $.ajax({
            url: `/cart/remove-in-cart?productId=${productId}`,
            type: "DELETE",
            success: function (response) {
                $(`#product_row_${productId}`).remove();

                const remainingItems = $(".product-row").length;

                if (remainingItems === 0) {
                    $("#checkoutBtn").prop("disabled", true);
                }
            },
            error: function () {
                alert("Thêm vào giỏ hàng thất bại!");
            }
        });
    });

    $("#checkout_cart").on("click", function () {
        const productRows = $(".product-row").length;
        if (productRows > 0) {
            $.ajax({
                url: `/cart/checkout-cart`,
                type: "POST",
                success: function (response) {
                    window.location.href = "/Cart/SuccessCheckout";
                },
                error: function () {
                    // alert("Thêm vào giỏ hàng thất bại!");
                }
            });
        } else {
            window.location.href = "/shop/index"
        }
    })

    calTotal();
})