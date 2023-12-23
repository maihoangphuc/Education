(function ($) {
    'use strict';
    $(function () {
        var body = $('body');
        var contentWrapper = $('.content-wrapper');
        var scroller = $('.container-scroller');
        var footer = $('.footer');
        var sidebar = $('.sidebar');


        // Collapse all submenus on page load
        $('.sidebar .collapse').collapse('hide');

        // Handle sidebar navigation click event
        $('.sidebar .nav-link').on('click', function () {
            var link = $(this);
            var parent = link.parent(); // Parent <li> element

            // Remove active class from all nav items
            $('.sidebar .nav-item').removeClass('active');

            // Remove expanded attribute from all nav links
            $('.sidebar .nav-link').attr('aria-expanded', 'false');

            // Add active class to the clicked nav item and its ancestors
            parent.addClass('active');
            link.parents('.nav-item').addClass('active');

            // If the clicked item has a submenu, toggle the aria-expanded attribute
            if (link.attr('aria-expanded') === 'false') {
                link.attr('aria-expanded', 'true');
            } else {
                link.attr('aria-expanded', 'false');
            }

            // Collapse other submenus
            $('.sidebar .collapse.show').collapse('hide');
        });

        //Change sidebar and content-wrapper height
        applyStyles();

        function applyStyles() {
            //Applying perfect scrollbar
            if (!body.hasClass("rtl")) {
                if ($('.settings-panel .tab-content .tab-pane.scroll-wrapper').length) {
                    const settingsPanelScroll = new PerfectScrollbar('.settings-panel .tab-content .tab-pane.scroll-wrapper');
                }
                if ($('.chats').length) {
                    const chatsScroll = new PerfectScrollbar('.chats');
                }
                if (body.hasClass("sidebar-fixed")) {
                    if ($('#sidebar').length) {
                        var fixedSidebarScroll = new PerfectScrollbar('#sidebar .nav');
                    }
                }
            }
        }

        $('[data-toggle="minimize"]').on("click", function () {
            if ((body.hasClass('sidebar-toggle-display')) || (body.hasClass('sidebar-absolute'))) {
                body.toggleClass('sidebar-hidden');
            } else {
                body.toggleClass('sidebar-icon-only');
            }
        });

        //checkbox and radios
        $(".form-check label,.form-radio label").append('<i class="input-helper"></i>');

        //Horizontal menu in mobile
        $('[data-toggle="horizontal-menu-toggle"]').on("click", function () {
            $(".horizontal-menu .bottom-navbar").toggleClass("header-toggled");
        });
        // Horizontal menu navigation in mobile menu on click
        var navItemClicked = $('.horizontal-menu .page-navigation >.nav-item');
        navItemClicked.on("click", function (event) {
            if (window.matchMedia('(max-width: 991px)').matches) {
                if (!($(this).hasClass('show-submenu'))) {
                    navItemClicked.removeClass('show-submenu');
                }
                $(this).toggleClass('show-submenu');
            }
        })

        $(window).scroll(function () {
            if (window.matchMedia('(min-width: 992px)').matches) {
                var header = $('.horizontal-menu');
                if ($(window).scrollTop() >= 70) {
                    $(header).addClass('fixed-on-scroll');
                } else {
                    $(header).removeClass('fixed-on-scroll');
                }
            }
        });
    });

    // focus input when clicking on search icon
    $('#navbar-search-icon').click(function () {
        $("#navbar-search-input").focus();
    });

})(jQuery);