(function ($) {
    "use strict";
    $(window).load(function () {
        $("#page-loader").fadeOut(400);
        $("#intro").addClass("animated fadeInDown");
        $("#intro-div").addClass("animated fadeInUp");
        $("#profile").addClass("animated zoomIn");
        $(function () {
            $(".filtr-container").filterizr();
            $(".simple-filter li").on("click", function () {
                $(".simple-filter li").removeClass("active-cat");
                $(this).addClass("active-cat");
            });
        });
    });
    $(document).ready(function () {
        AOS.init({ offset: 50, duration: 500, delay: 300, easing: "ease-in-sine", once: true });
        AOS.refresh();
        var $menuBtn = $("#nav-btn");
        var $sideNav = $("#side-nav");
        var $sideNavMask = $("#side-nav-mask");
        var $link = $(".nav-link");
        $menuBtn.on("click", function () {
            $sideNav.animate({ left: 0 }, "fast");
            $sideNavMask.addClass("visible");
        });
        $link.on("click", function () {
            $sideNav.animate({ left: -240 }, "fast");
            $sideNavMask.removeClass("visible");
        });
        $sideNavMask.on("click", function () {
            $sideNav.animate({ left: -240 }, "fast");
            $sideNavMask.removeClass("visible");
        });
        $("a[href*=#]:not([href=#])").on("click", function () {
            if (location.pathname.replace(/^\//, "") == this.pathname.replace(/^\//, "") || location.hostname == this.hostname) {
                var target = $(this.hash);
                target = target.length ? target : $("[name=" + this.hash.slice(1) + "]");
                if (target.length) {
                    $("html,body").animate({ scrollTop: target.offset().top - 32 }, 1000);
                    return false;
                }
            }
        });
        $(".skill-progress").each(function () {
            $(this)
                .find(".skill-determinate")
                .css({ width: jQuery(this).attr("data-percent") }, 7000);
        });
        $("#btn-1").on("click", function () {
            $("#content-1").slideToggle();
        });
        $("#btn-2").on("click", function () {
            $("#content-2").slideToggle();
        });
        $("#btn-3").on("click", function () {
            $("#content-3").slideToggle();
        });
        $("#btn-4").on("click", function () {
            $("#content-4").slideToggle();
        });
        $("#btn-5").on("click", function () {
            $("#content-5").slideToggle();
        });
        $("#btn-6").on("click", function () {
            $("#content-6").slideToggle();
        });
        $("#btn-7").on("click", function () {
            $("#content-7").slideToggle();
        });
        $("#btn-8").on("click", function () {
            $("#content-8").slideToggle();
        });
        $("#btn-9").on("click", function () {
            $("#content-9").slideToggle();
        });
        $("#btn-10").on("click", function () {
            $("#content-10").slideToggle();
        });
        $("#btn-11").on("click", function () {
            $("#content-11").slideToggle();
        });
        $("#btn-12").on("click", function () {
            $("#content-12").slideToggle();
        });
        var clientSwiper = new Swiper("#client-slider", {
            slidesPerView: 1,
            loop: true,
            direction: "horizontal",
            effect: "slide",
            speed: 1500,
            autoplay: 5000,
            spaceBetween: 0,
            pagination: ".swiper-pagination",
            paginationClickable: true,
            autoplayDisableOnInteraction: false,
        });
        var $monthBtn = $("#month-btn");
        var $yearBtn = $("#year-btn");
        var $month = $("#month");
        var $year = $("#year");
        $month.show();
        $year.hide();
        $monthBtn.on("click", function () {
            $month.show();
            $year.hide();
            $month.addClass("animated fadeIn");
            $monthBtn.addClass("active-cat");
            $yearBtn.removeClass("active-cat");
        });
        $yearBtn.on("click", function () {
            $month.hide();
            $year.show();
            $year.addClass("animated fadeIn");
            $yearBtn.addClass("active-cat animated");
            $monthBtn.removeClass("active-cat");
        });
        var offset = 300,
            scroll_top_duration = 700,
            $back_to_top = $(".back-to-top");
        $(window).scroll(function () {
            $(this).scrollTop() > offset ? $back_to_top.addClass("back-to-top-is-visible") : $back_to_top.removeClass("back-to-top-is-visible");
        });
        $back_to_top.on("click", function (event) {
            event.preventDefault();
            $("body,html").animate({ scrollTop: 0 }, scroll_top_duration);
        });
        $(".switcher-btn").on("click", function () {
            $(".switcher").animate({ right: 0 }, "medium");
            $(".back").addClass("visible");
        });
        $(".back").on("click", function () {
            $(".switcher").animate({ right: -135 }, "medium");
            $(".back").removeClass("visible");
        });
        $(".color-btn").on("click", function () {
            $(".switcher").animate({ right: -135 }, "medium");
            $(".back").removeClass("visible");
        });
        $("#color-red").on("click", function () {
            $("#color-switcher").attr("href", "stylesheets/style-red.css");
        });
        $("#color-pink").on("click", function () {
            $("#color-switcher").attr("href", "stylesheets/style-pink.css");
        });
        $("#color-purple").on("click", function () {
            $("#color-switcher").attr("href", "stylesheets/style-purple.css");
        });
        $("#color-deep-purple").on("click", function () {
            $("#color-switcher").attr("href", "stylesheets/style-deep-purple.css");
        });
        $("#color-indigo").on("click", function () {
            $("#color-switcher").attr("href", "stylesheets/style-indigo.css");
        });
        $("#color-blue").on("click", function () {
            $("#color-switcher").attr("href", "stylesheets/style-blue.css");
        });
        $("#color-teal").on("click", function () {
            $("#color-switcher").attr("href", "stylesheets/style-teal.css");
        });
        $("#color-green").on("click", function () {
            $("#color-switcher").attr("href", "stylesheets/style-green.css");
        });
        $("#color-light-green").on("click", function () {
            $("#color-switcher").attr("href", "stylesheets/style-light-green.css");
        });
        $("#color-amber").on("click", function () {
            $("#color-switcher").attr("href", "stylesheets/style-amber.css");
        });
        $("#color-orange").on("click", function () {
            $("#color-switcher").attr("href", "stylesheets/style-orange.css");
        });
        $("#color-deep-orange").on("click", function () {
            $("#color-switcher").attr("href", "stylesheets/style-deep-orange.css");
        });
        $("#color-brown").on("click", function () {
            $("#color-switcher").attr("href", "stylesheets/style-brown.css");
        });
        $("#color-blue-gray").on("click", function () {
            $("#color-switcher").attr("href", "stylesheets/style-blue-grey.css");
        });
        $("#color-gray").on("click", function () {
            $("#color-switcher").attr("href", "stylesheets/style-grey.css");
        });
    });
})(jQuery);
