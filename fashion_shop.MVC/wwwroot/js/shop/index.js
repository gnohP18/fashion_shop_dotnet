$(document).ready(function () {
    $('.btn-search-by-category').on('click', function () {
        const slug = $(this).data('category-slug');

        const url = `/Shop/index?CategorySlug=${slug}`;

        window.location.href = url;
    })
})