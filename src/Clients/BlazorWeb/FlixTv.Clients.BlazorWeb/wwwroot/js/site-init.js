// wwwroot/js/site-init.js
(() => {
    const markInit = el => el.dataset.initialized = "1";
    const notInit = el => !el.dataset.initialized;

    const initAll = (root = document) => {
        // 1) Select2 (s?nin admin.js-d?ki kimi ID-l?r v? ya data-attr)
        root.querySelectorAll('#quality, #country, #genre, #subscription, #rights, select[data-select2]')
            .forEach(el => { if (notInit(el) && window.$) { $(el).select2({ width: '100%' }); markInit(el); } });

        // 2) Magnific Popup (inline modal düym?l?ri)
        if (window.$ && $.fn.magnificPopup) {
            $('.open-modal', root).each(function () {
                if (notInit(this)) {
                    $(this).magnificPopup({
                        fixedContentPos: true, fixedBgPos: true,
                        overflowY: 'auto', type: 'inline', preloader: false,
                        focus: '#username', modal: false, removalDelay: 300,
                        mainClass: 'my-mfp-zoom-in'
                    });
                    markInit(this);
                }
            });
            // Daxili "ba?la" düym?l?ri üçün
            $('.modal__btn--dismiss', root).off('click.mfp').on('click.mfp', e => { e.preventDefault(); $.magnificPopup.close(); });
        }

        // 3) Smooth Scrollbar (müxt?lif konteynerl?r)
        if (window.Scrollbar) {
            ['.sidebar__nav', '.dashbox__table-wrap--1', '.dashbox__table-wrap--2',
                '.dashbox__table-wrap--3', '.dashbox__table-wrap--4', '.main__table-wrap']
                .forEach(sel => {
                    root.querySelectorAll(sel).forEach(el => {
                        if (notInit(el)) {
                            Scrollbar.init(el, { damping: 0.1, renderByPixels: true, alwaysShowTracks: true, continuousScrolling: true });
                            markInit(el);
                        }
                    });
                });
        }

        // 4) Plyr (video/audio)
        if (window.Plyr) {
            root.querySelectorAll('.plyr, video.plyr, audio.plyr').forEach(el => {
                if (notInit(el)) { new Plyr(el); markInit(el); }
            });
        }

        // 5) Template-in dig?r hiss?l?ri (menu toggle, bg image v? s.)
        // M?s: .header__btn toggle
        document.querySelectorAll('.header__btn').forEach(btn => {
            if (!btn.dataset.bound) {
                btn.addEventListener('click', () => {
                    btn.classList.toggle('header__btn--active');
                    document.querySelector('.header')?.classList.toggle('header--active');
                    document.querySelector('.sidebar')?.classList.toggle('sidebar--active');
                });
                btn.dataset.bound = "1";
            }
        });

        // data-bg fon ??kill?ri
        root.querySelectorAll('.section--bg').forEach(sec => {
            if (notInit(sec) && sec.dataset.bg) {
                Object.assign(sec.style, {
                    background: `url(${sec.dataset.bg})`,
                    backgroundPosition: 'center center',
                    backgroundRepeat: 'no-repeat',
                    backgroundSize: 'cover'
                });
                markInit(sec);
            }
        });
    };

    // ?lk yükl?nm?d?
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => initAll());
    } else {
        initAll();
    }

    // DOM d?yi?iklikl?rini izl?y?k (Blazor route d?yi?ikliyi, komponent renderi v? s.)
    const mo = new MutationObserver(() => initAll(document));
    mo.observe(document.body, { childList: true, subtree: true });

    // Brauzer geri/ir?li (history) zaman? da ehtiyat üçün
    window.addEventListener('popstate', () => initAll());
})();
