// Site-wide JavaScript

$(document).ready(function () {
    // ========== SİDEBAR TOGGLE ==========
    $("#menu-toggle-btn").on("click", function (e) {
        e.preventDefault();
        $("#sidebar-wrapper").toggleClass("active");
        $("#menu-toggle-btn").toggleClass("active");

        // İkon yönünü değiştir
        let icon = $(this).find('i');
        if ($("#sidebar-wrapper").hasClass("active")) {
            icon.removeClass('fa-chevron-right').addClass('fa-chevron-left');
        } else {
            icon.removeClass('fa-chevron-left').addClass('fa-chevron-right');
        }
    });

    // ========== AKTİF MENÜ İŞARETLEME ==========
    var currentPath = window.location.pathname;
    $('#sidebarNav .nav-link').each(function () {
        var linkPath = $(this).attr('href');
        if (linkPath && currentPath.toLowerCase().includes(linkPath.toLowerCase())) {
            $(this).addClass('active');
        }
    });

    // ========== TOOLTIP AKTİVASYONU ==========
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // ========== AUTO DISMISS ALERTS ==========
    setTimeout(function () {
        $(".alert").not('.alert-permanent').fadeOut('slow', function () {
            $(this).remove();
        });
    }, 5000);
});