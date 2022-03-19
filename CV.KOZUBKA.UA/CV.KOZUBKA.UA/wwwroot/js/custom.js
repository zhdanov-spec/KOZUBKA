(function ($) {
    "use strict";
    
    /*navbar shrink
    * -------------------------------------------------------------------*/
    $(document).ready(function() {
        $(window).scroll(function() {
            if ($(document).scrollTop() > 50) {
                $('nav').addClass('shrink');
            } else {
                $('nav').removeClass('shrink');
            }
        });


        /* Scrolling Navbar
         * ------------------------------------------------------------------*/
        $(window).scroll(function() {
            if ($(".navbar").offset().top > 50) {
                $(".navbar-fixed-top").addClass("top-nav-collapse");
            } else {
                $(".navbar-fixed-top").removeClass("top-nav-collapse");
            }
        });

        /* Page Scrolling Smoothly to Link Target
         * -----------------------------------------------------------------------------*/
        $('a[href*=#]:not([href=#])').on('click', function() {
            if (location.pathname.replace(/^\//,'') == this.pathname.replace(/^\//,'')
                || location.hostname == this.hostname) {

                var target = $(this.hash);
                target = target.length ? target : $('[name=' + this.hash.slice(1) +']');
                if (target.length) {
                    $('html,body').animate({
                        scrollTop: target.offset().top - 32
                    }, 1000);
                    return false;
                }
            }
        });


        /*Item hover
         * -------------------------------------------------------*/
        $("a").hover(function(){
            $('a', this)
                .removeClass('animated bounceIn')
                .hide()
                .addClass('animated bounceIn')
                .show();
        });


    });









})(jQuery);
