window.WhereJs = {
    initialiseWhereJS: function () {
        'use strict';
        // Mean Menu
        $('.mean-menu').meanmenu({
            meanScreenWidth: "1199"
        });
        // Header Sticky
        $(window).on('scroll', function () {
            if ($(this).scrollTop() > 120) {
                $('.navbar-area').addClass("is-sticky");
            }
            else {
                $('.navbar-area').removeClass("is-sticky");
            }
        });
        // tooltip
        $(function () {
            $('[data-bs-toggle="tooltip"]').tooltip()
        });

        // Others Option For Responsive JS
        $(".side-nav-responsive .dot-menu").on("click", function () {
            $(".side-nav-responsive .container-max .container").toggleClass("active");
        });

        // Metis Menu JS
        $(function () {
            $('#sidemenu-nav').metisMenu();
        });
        // Favorite JS
        $('.chat-list-header .header-right .favorite').on('click', function () {
            $(this).toggleClass('active');
        });


        // Burger Menu JS
        $('.burger-menu').on('click', function () {
            $(this).toggleClass('active');
            $('.main-content').toggleClass('hide-sidemenu-area');
            $('.sidemenu-area').toggleClass('toggle-sidemenu-area');
            $('.top-navbar').toggleClass('toggle-navbar-area');
        });
        $('.responsive-burger-menu').on('click', function () {
            $('.responsive-burger-menu').toggleClass('active');
            $('.sidemenu-area').toggleClass('active-sidemenu-area');
        });
        // FAQ Accordion JS
        $('.accordion').find('.accordion-title').on('click', function () {
            // Adds Active Class
            $(this).toggleClass('active');
            // Expand or Collapse This Panel
            $(this).next().slideToggle('fast');
            // Hide The Other Panels
            $('.accordion-content').not($(this).next()).slideUp('fast');
            // Removes Active Class From Other Titles
            $('.accordion-title').not($(this)).removeClass('active');
        });
        // Odometer JS
        $('.odometer').appear(function (e) {
            var odo = $(".odometer");
            odo.each(function () {
                var countNumber = $(this).attr("data-count");
                $(this).html(countNumber);
            });
        });
        
      

        
    }
}