const pinIcon = `
<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
     viewBox="0 0 24 24" fill="none" stroke="currentColor"
     stroke-width="2" stroke-linecap="round" stroke-linejoin="round"
     class="icon icon-tabler icon-tabler-map-pin">
  <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
  <path d="M9 11a3 3 0 1 0 6 0a3 3 0 0 0 -6 0"/>
  <path d="M17.657 16.657l-4.243 4.243a2 2 0 0 1 -2.828 0l-4.243 -4.243a8 8 0 1 1 11.314 0z"/>
</svg>`;

const bedIcon = `
<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
     viewBox="0 0 24 24" fill="none" stroke="currentColor"
     stroke-width="2" stroke-linecap="round" stroke-linejoin="round"
     class="icon icon-tabler icon-tabler-bed">
  <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
  <path d="M7 10m0 2a2 2 0 1 0 0 -4a2 2 0 0 0 0 4"/>
  <path d="M7 12h10v6h-10z"/>
  <path d="M5 12v6"/>
  <path d="M19 12v6"/>
  <path d="M4 18h16"/>
</svg>`;

(function ($) {
    function clamp(n, min, max) {
        return Math.max(min, Math.min(max, n));
    }

    function initStarRatings(root) {
        const $scope = root ? $(root) : $(document);
        const $nodes = $scope.find(".star-rating");

        $nodes.each(function () {
            const $el = $(this);
            const raw = $el.attr("data-stars");
            const stars = clamp(parseInt(raw, 10) || 0, 0, 5);
            const percent = (stars / 5) * 100;

            const $fill = $el.find(".stars-fill");
            if ($fill.length === 0) return;

            $fill.css("width", "0%");
            $fill.get(0).offsetWidth;

            requestAnimationFrame(() => {
                $fill.css("width", percent + "%");
            });
        });
    }

    window.initStarRatings = initStarRatings;

    function showToast(message, type = "success") {
        const $container = $("#toast-container");
        if ($container.length === 0) return;

        const $toast = $(`
            <div class="toast toast-${type} show" role="alert">
                <div class="toast-header">
                    <span class="me-auto fw-bold">MyRent</span>
                    <button type="button" class="btn-close ms-2" aria-label="Close"></button>
                </div>
                <div class="toast-body"></div>
            </div>
        `);

        $toast.find(".toast-body").text(message);

        $toast.find(".btn-close").on("click", function () {
            $toast.remove();
        });

        $container.append($toast);

        setTimeout(() => {
            $toast.removeClass("show").addClass("hide");
            setTimeout(() => $toast.remove(), 300);
        }, 4000);
    }

    window.showToast = showToast;

    function copyAddress() {
        const text = $("#property-address").text();
        if (!text) return;

        navigator.clipboard.writeText(text).then(() => {
            showToast("Address copied to clipboard");
        });
    }

    window.copyAddress = copyAddress;

    function initGalleryCarouselModal() {
        const $modal = $("#galleryModal");
        const $carousel = $("#galleryCarousel");

        if ($modal.length === 0 || $carousel.length === 0) return;

        if (!window.bootstrap || !window.bootstrap.Modal || !window.bootstrap.Carousel) {
            console.warn("Bootstrap not found. Ensure bootstrap.bundle/tabler is loaded before site.js.");
            return;
        }

        const modalEl = $modal.get(0);
        const carouselEl = $carousel.get(0);

        const modal = window.bootstrap.Modal.getOrCreateInstance(modalEl);
        const carousel = window.bootstrap.Carousel.getOrCreateInstance(carouselEl, {
            interval: false,
            ride: false,
            wrap: true
        });

        $(document).on("click", "a.gallery-thumb", function (e) {
            e.preventDefault();

            const indexRaw = $(this).attr("data-gallery-index");
            const index = Number.parseInt(indexRaw ?? "0", 10);
            if (Number.isNaN(index)) return;

            modal.show();
            requestAnimationFrame(() => carousel.to(index));
        });

        $(modalEl).on("hidden.bs.modal", () => {
            carousel.to(0);
        });
    }

    $(function () {
        initStarRatings();
        initGalleryCarouselModal();
    });

    const themeStorageKey = "theme";

    const moonIcon = `
        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
             viewBox="0 0 24 24" fill="none" stroke="currentColor"
             stroke-width="2" stroke-linecap="round" stroke-linejoin="round"
             class="icon icon-tabler icon-tabler-moon">
          <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
          <path d="M12 3c.132 0 .263 0 .393 0a7.5 7.5 0 0 0 7.92 12.446
                   a9 9 0 1 1 -8.313 -12.454l0 .008"/>
        </svg>`;

    const sunIcon = `
        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
             viewBox="0 0 24 24" fill="none" stroke="currentColor"
             stroke-width="2" stroke-linecap="round" stroke-linejoin="round"
             class="icon icon-tabler icon-tabler-sun">
          <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
          <path d="M8 12a4 4 0 1 0 8 0a4 4 0 1 0 -8 0"/>
          <path d="M3 12h1m8 -9v1m8 8h1m-9 8v1
                   m-6.4 -15.4l.7 .7m12.1 -.7l-.7 .7
                   m0 11.4l.7 .7m-12.1 -.7l-.7 .7"/>
        </svg>`;

    function applyTheme(theme) {

        $("html")
            .attr("data-bs-theme", theme)
            .toggleClass("theme-dark", theme === "dark");

        localStorage.setItem(themeStorageKey, theme);

        const $btn = $("#theme-toggle");
        if ($btn.length) {
            $btn.html(theme === "dark" ? sunIcon : moonIcon);
        }
    }

    window.toggleTheme = function () {
        const current = $("html").attr("data-bs-theme") === "dark" ? "dark" : "light";
        applyTheme(current === "dark" ? "light" : "dark");
    };
    
    function showLoader() {
        $("#page-loading").css("display", "flex");
    }

    function hideLoader() {
        $("#page-loading").hide();
    }

    function isInternalUrl(href) {
        try {
            const url = new URL(href, window.location.href);
            return url.origin === window.location.origin;
        } catch {
            return false;
        }
    }

        const saved = localStorage.getItem(themeStorageKey);
        const theme = (saved === "dark" || saved === "light") ? saved : "light";
        applyTheme(theme);

        hideLoader();

        $(document).on("click", "a[href]", function () {
            const $a = $(this);
            const href = $a.attr("href");
            if (!href) return;

            if (href.startsWith("#")) return;
            if (href.startsWith("mailto:")) return;
            if (href.startsWith("tel:")) return;
            if (href.startsWith("javascript:")) return;

            if ($a.attr("target") === "_blank") return;
            if ($a.is("[download]")) return;

            if (!isInternalUrl(href)) return;

            showLoader();
        });

        $(document).on("submit", "form", function () {
            showLoader();
        });

    $(window).on("beforeunload", function () {
        showLoader();
    });

    function reInitOnReturn() {
        hideLoader();
        initStarRatings();
    }

    $(window).on("pageshow", function () {
        reInitOnReturn();
    });

    $(window).on("popstate", function () {
        reInitOnReturn();
    });

    $(document).on("visibilitychange", function () {
        if (document.visibilityState === "visible") {
            reInitOnReturn();
        }
    });
})(jQuery);

(function ($) {
    
    const $input = $("#search-input");
    const $menu = $("#search-suggestions");
    let timer = null;

    if ($input.length === 0) return;

    function hideMenu() {
        $menu.hide().empty();
    }

    function openSuggestionsFor(q, skipDelay = false) {
        const query = (q || "").trim();

        clearTimeout(timer);

        if (query.length < 2) {
            hideMenu();
            return;
        }
        
        let delay = skipDelay ? 0 : 300;

        timer = setTimeout(() => {
            $.get("/search", { q: query })
                .done(function (items) {

                    console.log(items);
                    $menu.empty();

                    if (!items || items.length === 0) {
                        hideMenu();
                        return;
                    }

                    items.forEach(item => {

                        const type = (item.kind || "").toLowerCase();
                        const isLocation = type === "city" || type === "country";
                        const icon = isLocation ? pinIcon : bedIcon;
                        const title = ((item.title ?? item.label) || "").trim();
                        const stars = !isLocation
                            ? ("★★★★★".slice(0, item.stars || 0) + "☆☆☆☆☆".slice(0, 5 - (item.stars || 0)))
                            : "";
                        
                        let value = ((item.value ?? title) || "").trim();

                        const $row = $(`
                            <a href="#" class="dropdown-item d-flex align-items-center">
                                <span class="me-2 text-secondary d-inline-flex"></span>
                    
                                <div class="flex-fill">
                                    <div class="fw-semibold"></div>
                                    <div class="text-secondary small"></div>
                                </div>
                    
                                <div class="ms-2 text-yellow small"></div>
                            </a>
                        `);

                        $row.find("span.me-2").html(icon);
                        $row.find(".fw-semibold").text(title || value || "");
                        $row.find(".text-secondary.small").text(item.subtitle || (isLocation ? "" : ""));
                        $row.find(".text-yellow.small").text(stars);
                        $row.on("click", function (e) {
                            
                            e.preventDefault();

                            let toInput = title;

                            if (!toInput) {
                                toInput = value.includes(":")
                                    ? value.split(":").slice(1).join(":").trim()
                                    : value;
                            }

                            $input.val(toInput);

                            hideMenu();

                            const form = $input.closest("form").get(0);
                            if (form) form.submit();
                        });

                        $menu.append($row);
                    });

                    $menu.show();
                })
                .fail(function () {
                    hideMenu();
                });
        }, delay);
    }

    $input.on("input", function () {
        openSuggestionsFor($(this).val());
    });

    $input.on("focus click", function () {
        openSuggestionsFor($(this).val(), true);
    });

    $(document).on("click", function (e) {
        if (!$(e.target).closest("#search-input, #search-suggestions").length) {
            hideMenu();
        }
    });
})(jQuery);

(function ($) {
    const $input = $("#search-input");
    const $clear = $("#search-clear");

    if ($input.length === 0) return;

    function toggleClear() {
        $clear.toggleClass("d-none", !$input.val());
    }

    // Show / hide clear icon while typing
    $input.on("input", function () {
        toggleClear();
    });

    // Clear button behavior
    $clear.on("click", function () {
        $input.val("").focus();
        toggleClear();

        // Hide suggestions if open
        $("#search-suggestions").hide().empty();
    });

    // Initial state (important for page reload / back button)
    toggleClear();
})(jQuery);