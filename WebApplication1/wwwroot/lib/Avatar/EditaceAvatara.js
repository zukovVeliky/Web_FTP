// EditaceAvatara.js - Vanilla JavaScript version (bez jQuery)
var ZobrazeniCircleSquare = 'square'; // 'circle' nebo 'square'

function EditaceAvataraCircleSquare(druhZobrazeni) {
    ZobrazeniCircleSquare = druhZobrazeni;
}

// Globalni CTRL klavesa handler pro vsechny Croppie kontejnery
(function initCroppieCtrlHandler() {
    let isCtrlPressed = false;

    document.addEventListener('keydown', function (e) {
        if (e.ctrlKey && !isCtrlPressed) {
            isCtrlPressed = true;
            document.querySelectorAll('.croppie-container').forEach(container => {
                container.classList.remove('drag-disabled');
                container.classList.add('drag-enabled');
            });
        }
    });

    document.addEventListener('keyup', function (e) {
        if (!e.ctrlKey && isCtrlPressed) {
            isCtrlPressed = false;
            document.querySelectorAll('.croppie-container').forEach(container => {
                container.classList.remove('drag-enabled');
                container.classList.add('drag-disabled');
            });
        }
    });

    // Reset stavu po ztrate fokusu okna.
    window.addEventListener('blur', function () {
        if (isCtrlPressed) {
            isCtrlPressed = false;
            document.querySelectorAll('.croppie-container').forEach(container => {
                container.classList.remove('drag-enabled');
                container.classList.add('drag-disabled');
            });
        }
    });
})();

function UpdateHiddenFromCroppie(element, elementHidden, data) {
    if (!element || !elementHidden) return;
    var croppieData = data;
    if (!croppieData && element.croppieInstance && typeof element.croppieInstance.get === 'function') {
        croppieData = element.croppieInstance.get();
    }
    if (!croppieData || !croppieData.points) return;

    var pole = elementHidden.value.split('~');
    elementHidden.value = pole[0] + "~" +
        croppieData.points[0] + "~" +
        croppieData.points[1] + "~" +
        croppieData.points[2] + "~" +
        croppieData.points[3] + "~" +
        croppieData.zoom + "~" +
        pole[6] + "~" +
        pole[7];
}

function EditaceAvatara(Element, ElementHidden) {
    // ZĂ­skĂˇnĂ­ elementu a hodnot
    const elementHidden = document.getElementById(ElementHidden);
    const element = document.getElementById(Element);
    
    if (!elementHidden || !element) {
        console.error('Element not found:', Element, ElementHidden);
        return;
    }

    var pole = elementHidden.value.split('~');
    pole[0] = document.location.origin + '/' + pole[0];

    // VypoÄŤĂ­tat optimĂˇlnĂ­ velikost pro boundary (max 100% ĹˇĂ­Ĺ™ky kontejneru)
    const containerWidth = element.offsetWidth || parseInt(pole[6]);
    const containerHeight = element.offsetHeight || parseInt(pole[7]);
    
    const viewportWidth = Math.min(parseInt(pole[6]), containerWidth);
    const viewportHeight = Math.min(parseInt(pole[7]), containerHeight);
    
    // Inicializace Croppie (vanilla JS API)
    var avatar = new Croppie(element, {
        viewport: { 
            width: viewportWidth, 
            height: viewportHeight, 
            type: ZobrazeniCircleSquare 
        },
        boundary: { 
            width: viewportWidth, 
            height: viewportHeight 
        },
        mouseWheelZoom: 'ctrl',  // Pouze s CTRL klavesou
        showZoomer: false,
        enableOrientation: true,
        update: function (data) {
            UpdateHiddenFromCroppie(element, elementHidden, data);
        }
    });
    
    // Drag zakazan na zacatku, povoli se po stisku CTRL.
    element.classList.add('drag-disabled');

    // Bind obrĂˇzku
    if (pole[1] != pole[3]) {
        avatar.bind({
            url: pole[0],
            points: [
                parseFloat(pole[1]), 
                parseFloat(pole[2]), 
                parseFloat(pole[3]), 
                parseFloat(pole[4])
            ],
            zoom: parseFloat(pole[5])
        }).then(function () {
            UpdateHiddenFromCroppie(element, elementHidden);
        });
    } else {
        avatar.bind({
            url: pole[0]
        }).then(function () {
            UpdateHiddenFromCroppie(element, elementHidden);
        });
    }

    // Event listener pro update
    element.addEventListener('update', function (ev) {
        const detail = ev.detail || ev;
        const data = detail.data || detail;
        UpdateHiddenFromCroppie(element, elementHidden, data);
    });

    // VnitĹ™nĂ­ funkce ZachyceniURL (nepouĹľĂ­vĂˇ se, ale zachovĂˇna pro kompatibilitu)
    function ZachyceniURL(AvatarURL) {
        if (typeof window.showInfoModal === 'function') {
            window.showInfoModal('ObrĂˇzek byl vybrĂˇn.', 'ĂšspÄ›ch');
        }
        const hiddenEl = document.getElementById('HiddenParametry');
        if (hiddenEl) {
            var pole = hiddenEl.value.split('~');
            hiddenEl.value = AvatarURL + "~0~0~0~0~0~" + pole[6] + "~" + pole[7];

            const contejner = document.getElementById("Contejner");
            if (contejner && contejner.croppie) {
                contejner.croppie.bind({
                    url: AvatarURL
                });
            }
        }
    }

    // UloĹľenĂ­ reference na avatar pro pozdÄ›jĹˇĂ­ pouĹľitĂ­
    element.croppieInstance = avatar;
}

function NastaveniAvatara2(Image, arg) {
    const imageEl = document.getElementById(Image);
    if (!imageEl) return;

    var pole = arg.split('~');
    imageEl.style.transform = "scale(" + pole[0] + ")";
    imageEl.style.left = pole[1] + "px";
    imageEl.style.top = pole[2] + "px";
}

function NastaveniAvatara(Image, ElementHidden) {
    const imageEl = document.getElementById(Image);
    const hiddenEl = document.getElementById(ElementHidden);
    
    if (!imageEl || !hiddenEl) return;

    var pole = hiddenEl.value.split('~');
    var zoom = 1 / (parseFloat(pole[5]) / (parseInt(pole[6]) / parseInt(pole[7])));
    
    const width = imageEl.offsetWidth;
    const height = imageEl.offsetHeight;
    
    // Transform scale
    imageEl.style.transform = "scale(" + (parseFloat(pole[5]) / (parseInt(pole[6]) / parseInt(pole[7]))) + ")";
    
    // Left position
    const leftPos = (-1) * (((width - (width / zoom)) / 2) + (parseInt(pole[1]) / zoom));
    imageEl.style.left = leftPos + "px";
    
    // Top position
    const topPos = (-1) * (((height - (height / zoom)) / 2) + (parseInt(pole[2]) / zoom));
    imageEl.style.top = topPos + "px";
}

