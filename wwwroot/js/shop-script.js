(function ($) {
    "use strict";

    // Spinner
    var spinner = function () {
        setTimeout(function () {
            if ($('#spinner').length > 0) {
                $('#spinner').removeClass('show');
            }
        }, 1);
    };
    spinner(0);


    // Initiate the wowjs
    new WOW().init();


    // Sticky Navbar
    $(window).scroll(function () {
        if ($(this).scrollTop() > 45) {
            $('.nav-bar').addClass('sticky-top shadow-sm');
        } else {
            $('.nav-bar').removeClass('sticky-top shadow-sm');
        }
    });


    // Hero Header carousel
    $(".header-carousel").owlCarousel({
        items: 1,
        autoplay: true,
        smartSpeed: 2000,
        center: false,
        dots: false,
        loop: true,
        margin: 0,
        nav: true,
        navText: [
            '<i class="bi bi-arrow-left"></i>',
            '<i class="bi bi-arrow-right"></i>'
        ]
    });


    // ProductList carousel
    $(".productList-carousel").owlCarousel({
        autoplay: true,
        smartSpeed: 2000,
        dots: false,
        loop: true,
        margin: 25,
        nav: true,
        navText: [
            '<i class="fas fa-chevron-left"></i>',
            '<i class="fas fa-chevron-right"></i>'
        ],
        responsiveClass: true,
        responsive: {
            0: {
                items: 1
            },
            576: {
                items: 1
            },
            768: {
                items: 2
            },
            992: {
                items: 2
            },
            1200: {
                items: 3
            }
        }
    });

    // ProductList categories carousel
    $(".productImg-carousel").owlCarousel({
        autoplay: true,
        smartSpeed: 1500,
        dots: false,
        loop: true,
        items: 1,
        margin: 25,
        nav: true,
        navText: [
            '<i class="bi bi-arrow-left"></i>',
            '<i class="bi bi-arrow-right"></i>'
        ]
    });


    // Single Products carousel
    $(".single-carousel").owlCarousel({
        autoplay: true,
        smartSpeed: 1500,
        dots: true,
        dotsData: true,
        loop: true,
        items: 1,
        nav: true,
        navText: [
            '<i class="bi bi-arrow-left"></i>',
            '<i class="bi bi-arrow-right"></i>'
        ]
    });


    // ProductList carousel
    $(".related-carousel").owlCarousel({
        autoplay: true,
        smartSpeed: 1500,
        dots: false,
        loop: true,
        margin: 25,
        nav: true,
        navText: [
            '<i class="fas fa-chevron-left"></i>',
            '<i class="fas fa-chevron-right"></i>'
        ],
        responsiveClass: true,
        responsive: {
            0: {
                items: 1
            },
            576: {
                items: 1
            },
            768: {
                items: 2
            },
            992: {
                items: 3
            },
            1200: {
                items: 4
            }
        }
    });



    // Product Quantity
    $('.quantity button').on('click', function () {
        var button = $(this);
        var oldValue = button.parent().parent().find('input').val();
        if (button.hasClass('btn-plus')) {
            var newVal = parseFloat(oldValue) + 1;
        } else {
            if (oldValue > 0) {
                var newVal = parseFloat(oldValue) - 1;
            } else {
                newVal = 0;
            }
        }
        button.parent().parent().find('input').val(newVal);
    });



    // Back to top button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) {
            $('.back-to-top').fadeIn('slow');
        } else {
            $('.back-to-top').fadeOut('slow');
        }
    });
    $('.back-to-top').click(function () {
        $('html, body').animate({ scrollTop: 0 }, 1500, 'easeInOutExpo');
        return false;
    });




})(jQuery);

//cart / checkout script
// helper to read antiforgery token
// find antiforgery token value rendered by @Html.AntiForgeryToken()
function getAntiForgeryToken() {
    const el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
}

async function postUrlEncoded(url, data) {
    const token = getAntiForgeryToken();
    const params = new URLSearchParams();
    for (const k in data) params.append(k, data[k]);
    const res = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token
        },
        body: params.toString()
    });
    return res;
}

document.addEventListener('click', async function (e) {
    // increase
    if (e.target.matches('.btn-increase')) {
        const ma = e.target.getAttribute('data-ma');
        const row = document.querySelector('tr[data-rowid="' + ma + '"]');
        const input = row.querySelector('.qty-input');
        let qty = parseInt(input.value, 10) || 0;
        qty++;
        // call Update action
        const res = await postUrlEncoded('@Url.Action("Update","Cart")', { maSachGH: ma, soLuong: qty });
        if (res.ok) window.location.reload(); else alert('Update failed');
        return;
    }

    // decrease
    if (e.target.matches('.btn-decrease')) {
        const ma = e.target.getAttribute('data-ma');
        const row = document.querySelector('tr[data-rowid="' + ma + '"]');
        const input = row.querySelector('.qty-input');
        let qty = parseInt(input.value, 10) || 1;
        qty = Math.max(1, qty - 1);
        const res = await postUrlEncoded('@Url.Action("Update","Cart")', { maSachGH: ma, soLuong: qty });
        if (res.ok) window.location.reload(); else alert('Update failed');
        return;
    }

    // remove
    if (e.target.matches('.btn-remove')) {
        if (!confirm('Xóa sản phẩm khỏi giỏ?')) return;
        const ma = e.target.getAttribute('data-ma');
        const res = await postUrlEncoded('@Url.Action("Remove","Cart")', { maSachGH: ma });
        if (res.ok) window.location.reload(); else alert('Remove failed');
        return;
    }
});

// optional: handle manual qty input blur to set exact quantity
document.addEventListener('blur', async function (e) {
    if (!e.target.matches('.qty-input')) return;
    const ma = e.target.getAttribute('data-ma');
    let qty = parseInt(e.target.value, 10) || 1;
    if (qty < 1) qty = 1;
    const res = await postUrlEncoded('@Url.Action("Update","Cart")', { maSachGH: ma, soLuong: qty });
    if (res.ok) window.location.reload(); else alert('Update failed');
}, true);