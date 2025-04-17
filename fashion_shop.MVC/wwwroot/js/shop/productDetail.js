$(document).ready(function () {
    const selectDefaultProductVariant = () => {
        const $productVariants = $(".product-variant");

        $productVariants.each((index, element) => {
            const $fistVariant = $(element).find(".variant-btn").eq(0);

            $fistVariant.addClass("btn-active");
            if (index === $productVariants.length - 1) {
                $fistVariant.trigger("click");
            }
        });
    }

    $(".variant-btn").on("click", function () {
        const $productVariant = $(this).closest(".product-variant");

        const $variants = $productVariant.find(".variant-btn");

        $variants.each((index, element) => {
            $(element).removeClass("btn-active");
        })
        $(this).addClass("btn-active");

        const code = getSelectionVariant();

        $("#product_variant_price").text($(`#${code}`)
            .data("price")
            .toLocaleString('vi-VN', { style: 'currency', currency: 'VND' }))
    })

    const getSelectionVariant = () => {
        const codes = [];
        // get all product active
        $(".variant-btn.btn-active").each((index, element) => {
            const variantCode = $(element).data("variant-code");

            const priorityCode = $(element).data("variant-priority");

            codes.push({ code: variantCode, priority: priorityCode });
        })

        return codes.sort((a, b) => a.priority - b.priority).map(_ => _.code).join("_");
    }

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

    selectDefaultProductVariant();
});
