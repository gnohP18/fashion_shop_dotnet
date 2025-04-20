$(document).ready(function () {
    const isVariant = $('#is_variant').val();

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

        const $productItem = $(`#${code}`)

        $("#product_variant_price").text($productItem
            .data("price")
            .toLocaleString('vi-VN', { style: 'currency', currency: 'VND' }));

        $("#main_image").attr("src", $productItem.data("image"));

        $(".add-to-cart-btn").attr("data-productitem-id", $productItem.val());
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

        const productItemId = $(this).data("productitem-id");
        console.log(productItemId);

        $.ajax({
            url: `/cart/add-to-cart?productItemId=${productItemId}`,
            type: "GET",
            success: function (response) {
                //    
                console.log(productItemId);

            },
            error: function () {
                alert("Thêm vào giỏ hàng thất bại!");
            }
        });
    });

    if (isVariant.toLowerCase() === "true") {
        selectDefaultProductVariant();
    } else {
        $(".add-to-cart-btn").attr("data-productitem-id", $('#_').val());
    }
});
