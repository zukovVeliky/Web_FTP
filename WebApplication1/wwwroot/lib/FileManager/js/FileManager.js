var funkce;
var file;
var koncovka;
var ico;
var NSoubory = 0;
var NAdresari = 0;
var seznamAdresaruDelete = "";
var seznamSouboruDelete = "";
var myDropzone = null;
var setPath = "UzivatelskeSoubory";
var pageFunction = "ZachyceniURL";
/*
console.log("document.URL : " + document.URL);
console.log("document.location.href : " + document.location.href);
console.log("document.location.origin : " + document.location.origin);
console.log("document.location.hostname : " + document.location.hostname);
console.log("document.location.host : " + document.location.host);
console.log("document.location.pathname : " + document.location.pathname);
*/

function StartManager() {
    NSoubory = 0;
    NAdresari = 0;
    
    $('.context-menu-one,.context-menu-two').on('mousedown', function (e) {
        if ($('.context-menu-one.nu-selected').length==0 && $('.context-menu-two.nu-selected').length==0) {
            ico = $(this).children('div').children('.Icona').css('background-image');
            imgDrag.src = ico.substring(5, ico.length - 2); 
        }


    });

    $('.context-menu-one').on('keydown click', function (e) {
        // ctrl + click
        if (e.ctrlKey && e.type == 'click') {
            
            kontrolaStavu(this);
        }

        NSoubory = 0;
        NAdresari = 0;



        $('.context-menu-one.nu-selected').each(function () { NSoubory++; });
        $('.context-menu-two.nu-selected').each(function () { NAdresari++; });

        if (NSoubory > 0 && NAdresari > 0) { imgDrag.src = imgDragFileFolder; }
        if (NSoubory == 0 && NAdresari == 1) { imgDrag.src = imgDragFolder; }
        if (NSoubory == 0 && NAdresari > 1) { imgDrag.src = imgDragMultiFolder; }
        if (NSoubory == 1 && NAdresari == 0) { imgDrag.src = imgDragFile; }
        if (NSoubory > 1 && NAdresari == 0) { imgDrag.src = imgDragMultiFile; }


    });

    $('.context-menu-three').on('click', function (e) {
        // click + not ctrl
        if (!e.ctrlKey && e.type == 'click') {


            $('#FMC').load('/filemanager/filemanager?back=true&SET=' + encodeURIComponent($('#settings').val()) +'&LD=' + encodeURIComponent($('#root').val()) +  ' #managerFileKolekce', function () { StartManager(); InitMenu();});
            

            //__doPostBack('Akce1', 'go_up_folder');
        }
    });

    $('.context-menu-two').on('click', function (e) {
        // click + not ctrl
        if (!e.ctrlKey && e.type == 'click') {
            var fileE = $(this).children('div').children('.FullName').val();
            
            $('#FMC').load('/filemanager/filemanager?SD=' + encodeURIComponent(fileE) + '&SET=' + encodeURIComponent($('#settings').val()) +'&LD=' + encodeURIComponent($('#root').val()) + ' #managerFileKolekce', function () { StartManager(); InitMenu();});
            //__doPostBack('HiddenAkce1', 'go_down_folder~' + fileE);

            

           // __doPostBack('Akce1', 'go_down_folder~' + fileE);
        }
    });
    
    $('.context-menu-two').on('keydown click', function (e) {


        // ctrl + click
        if (e.ctrlKey && e.type == 'click') {
            kontrolaStavu(this);
        }

        NSoubory = 0;
        NAdresari = 0;

        $('.context-menu-one.nu-selected').each(function () { NSoubory++; });
        $('.context-menu-two.nu-selected').each(function () { NAdresari++; });

        if (NSoubory > 0 && NAdresari > 0) { imgDrag.src = imgDragFileFolder; }
        if (NSoubory == 0 && NAdresari == 1) { imgDrag.src = imgDragFolder; }
        if (NSoubory == 0 && NAdresari > 1) { imgDrag.src = imgDragMultiFolder; }
        if (NSoubory == 1 && NAdresari == 0) { imgDrag.src = imgDragFile; }
        if (NSoubory > 1 && NAdresari == 0) { imgDrag.src = imgDragMultiFile; }


    });

    function kontrolaStavu(element) {

        var fileE = $(element).children('div').children('.NameSelektor').text();
        var koncovka = $(element).children('div').children('.Koncovka').text();
        if ($(element).hasClass('nu-selected')) { $(element).removeClass('nu-selected'); } else { $(element).addClass('nu-selected'); }


    }

}

function drag(ev) {
    //$.noConflict();
    var seznamSouboru = "";
    var seznamAdresaru = "";


    //alert($(ev.target).parent().children('.context-menu-one').children('div').children('.FullName').val());


    /// vytvoreni seznamu vybranych a presouvanzch souboru a adresaru
    $('.context-menu-one.nu-selected').each(function () {
        if (seznamSouboru == "") {
            seznamSouboru += $(this).children('div').children('.FullName').val();
        }
        else { seznamSouboru +="~"+ $(this).children('div').children('.FullName').val();}
    });
    $('.context-menu-two.nu-selected').each(function () {
        if (seznamAdresaru == "") {
            seznamAdresaru += $(this).children('div').children('.FullName').val();
        }
        else { seznamAdresaru += "~" + $(this).children('div').children('.FullName').val(); }
    });
    if (seznamSouboru == "" && seznamAdresaru == "" &&
        $(ev.target).parent().children('.context-menu-one').children('div').children('.FullName').val() != undefined)
    { seznamSouboru += $(ev.target).children('div').children('.FullName').val(); } 
    if (seznamAdresaru == "" && seznamSouboru == "" &&
        $(ev.target).parent().children('.context-menu-two').children('div').children('.FullName').val() != undefined)
    { seznamAdresaru += $(ev.target).children('div').children('.FullName').val(); } 

    //alert(PrenosElementu);


    ev.dataTransfer.setDragImage(imgDrag, 0, 0);
    ev.dataTransfer.setData("Text", seznamSouboru + "%" + seznamAdresaru); 
}
function allowDrop(ev) {
    ev.preventDefault();
}
function drop(ev) {
    var presun = "";
    if (ev.ctrlKey) { presun = "&Presun=Copy";}
    var copy = event.dataTransfer.getData("Text");
    var target = "";


    if ($(ev.target).parent().parent().parent().children('.context-menu-two').length == 1
        &&
        copy != "") {

        target = $(ev.target).parent().parent().parent().children('.context-menu-two').children('div').children('.FullName').val();

        $('#FMC').load('/filemanager/filemanager?Copy=' + encodeURIComponent(target + "|" + copy) + presun + '&SET=' + encodeURIComponent($('#settings').val()) +'&LD=' + encodeURIComponent($('#root').val()) + ' #managerFileKolekce', function () { StartManager(); InitMenu(); });

        //__doPostBack();
        //alert($(ev.target).parent().parent().parent().children('.context-menu-two').length);
    }
    if ($(ev.target).parent().parent().parent().children('.context-menu-three').length == 1
        &&
        copy != "") {
        $('#FMC').load('/filemanager/filemanager?Copy=' + encodeURIComponent("|" + copy) + presun + '&SET=' + encodeURIComponent($('#settings').val()) +'&LD=' + encodeURIComponent($('#root').val()) + ' #managerFileKolekce', function () { StartManager(); InitMenu(); });

       // __doPostBack('Akce3', copy);
    }


}

function PasteFilesOpen() {

}

function InitMenu() {
    if (window.opener.Refresh != null) {
        window.onunload = RefreshParent;
    }
    $.contextMenu({
        selector: '#FMC',
        //events: { show: function (options) { $(this).addClass('activeFIX'); }, hide: function (options) { $(this).removeClass('activeFIX'); } },
        callback: function (key, options) {

            funkce = key;
            file = $(this).children('div').children('.NameSelektor').text();
            koncovka = $(this).children('div').children('.Koncovka').text();
            ico = $(this).children('div').children('.Icona').css('background-image');


                if (key == "createFolder") {
                    $("#dialogCreateDirectory").dialog("open");
                }

                if (key == "load") {
                    //myDropzone.options.url = document.location.origin + "?uploadRoot=" + encodeURIComponent($('#root').val());
                    myDropzone.options.url = document.location.origin + "?uploadRoot=" + encodeURIComponent($('#uploadRoot').val());
                    //myDropzone.options.url =  "https://localhost:7184/Editace/Editaceblog" + "?uploadRoot=" + encodeURIComponent($('#uploadRoot').val());                      
                    $("#dialogLoad").dialog("open");

                }

                var m = key + " - " + $(this).children('td').children('.NameSelektor').text();
                window.console && console.log(m);
        },
        
        items: {


            "createFolder": { name: "Vytvořit adresář", icon: "edit" },
            "load": { name: "Vložit soubory", icon: "edit" }

        }
        

    }
    );
    // obrazek
    $.contextMenu({
        selector: '.IMG',
        events: { show: function (options) { $(this).addClass('activeFIX'); }, hide: function (options) { $(this).removeClass('activeFIX'); } },
        callback: function (key, options) {
            funkce = key;
            file = $(this).children('div').children('.FullName').val();
            koncovka = $(this).children('div').children('.Koncovka').val();
            ico = $(this).children('div').children('.Icona').css('background-image');
            if (file != "") {

                if (key == "nahledIMG") {

                   // $("#dialogNahled").css("height", "350px");
                    //$("#dialogNahled").css("width", "450px");

                    /*
                    var img = $('<img id="imgnahled">');
                                                
                    if (typeof SUBdirectory === "undefined")
                    { img.attr('src', document.location.origin + '/UzivatelskeSoubory/' + file); }
                    else
                    { img.attr('src', document.location.origin + '/UzivatelskeSoubory/'+ SUBdirectory + file); }

                    img.css("max-width", "500px");
                    img.css("max-heigth", "450px");
                    img.appendTo('#imagediv');
                    $("#dialogNahled").first().first().append(img);   
                    */

                    $('#dialogNahled').load('/filemanager/filemanager?&Nahled='+file+'&SET=' + encodeURIComponent($('#settings').val()) + '&LD=' + encodeURIComponent($('#root').val()) + ' #imgNahled', function () { StartManager(); InitMenu(); });
                    $("#dialogNahled").dialog("open");
                }

                if (key == "rename") {

                    $("#renameInput").attr("placeholder", file.replace("." + koncovka, ""));
                    $("#renameInput").attr("arg", "F");
                    $("#renameInput").val(file.replace("." + koncovka, ""));
                    $("#typSouboru").html("." + koncovka);
                    $("#dialogRename").dialog("open");

                }
                if (key == "createURL") {

                    var SUBdirectory = $('#root').val();
                    if (SUBdirectory != "") {
                        SUBdirectory = SUBdirectory.replace('\\', '/') + '/';
                    }

                    try {

                        // odesle url vybraneho obrazku strance ktera volal pruzkumnika
                        if (eval("window.opener." + pageFunction) != null) {
                            eval("window.opener." + pageFunction + "(document.location.origin + '/' + setPath + '/' + SUBdirectory + file);");
                            window.close();
                        }

                            window.opener.CKEDITOR.tools.callFunction("1", document.location.origin + '/' + setPath + '/' + SUBdirectory + file);
                            window.close();




                    }
                    catch (chyba) {
                        $("#Podkaz").text(document.location.origin + '/' + setPath + '/' + SUBdirectory + file);
                        $("#dialogOdkaz").dialog("open");
                        //alert(document.location.origin + '/UzivatelskeSoubory/' + $('#root').val() + file);

                    }
                }
                if (key == "download") {

                    StahnoutSoubor(file);

                }
                if (key == "komprimovat") {
                    seznamSouboruDelete = file;
                    Komprimovat();
                }

                if (key == "delete") {

                    seznamSouboruDelete = file;
                    $("#dialogDelete").dialog("open");

                }
                if (key == "createFolder") {
                    $("#dialogCreateDirectory").dialog("open");
                }

                if (key == "load") {
                    //myDropzone.options.url = document.location.origin + "?root=" + encodeURIComponent($('#root').val());
                    myDropzone.options.url = document.location.origin + "?uploadRoot=" + encodeURIComponent($('#uploadRoot').val());
                    //myDropzone.options.url = "https://localhost:7184/Editace/Editaceblog" + "?uploadRoot=" + encodeURIComponent($('#uploadRoot').val());  

                    $("#dialogLoad").dialog("open");

                }

                var m = key + " - " + $(this).children('td').children('.NameSelektor').text();
                window.console && console.log(m);

            }
        },
        items: {
            "nahledIMG": { name: "Náhled", icon: "edit" },
            "sep1": "----------",
            "createURL": { name: "Vytvořit odkaz", icon: "edit" },
            "download": { name: "Stáhnout", icon: "edit" },
            "rename": { name: "Přejmenovat", icon: "edit" },
            "komprimovat": { name: "Komprimovat", icon: "edit" },
            "delete": { name: "Odstranit", icon: "delete" },
            "sep2": "----------",
            "createFolder": { name: "Vytvořit adresář", icon: "edit" },
            "load": { name: "Vložit soubory", icon: "edit" }
        }
    });
    // ZIP
    $.contextMenu({

        selector: '.zip',
        events: { show: function (options) { $(this).addClass('activeFIX'); }, hide: function (options) { $(this).removeClass('activeFIX'); } },
        callback: function (key, options) {

            funkce = key;
            file = $(this).children('div').children('.FullName').val();
            koncovka = $(this).children('div').children('.Koncovka').val();
            ico = $(this).children('div').children('.Icona').css('background-image');
            if (file != "") {
                if (key == "unzip") {

                    Dekomprimovat();


                }
                if (key == "rename") {

                    $("#renameInput").attr("placeholder", file.replace("." + koncovka, ""));
                    $("#renameInput").attr("arg", "F");
                    $("#renameInput").val(file.replace("." + koncovka, ""));
                    $("#typSouboru").html("." + koncovka);
                    $("#dialogRename").dialog("open");

                }
                if (key == "createURL") {
                    var SUBdirectory = $('#root').val();
                    if (SUBdirectory != "") {
                        SUBdirectory = SUBdirectory.replace('\\', '/') + '/';
                    }

                    try {

                        window.opener.CKEDITOR.tools.callFunction("1", document.location.origin + '/' + setPath + '/' + SUBdirectory + file);
                        window.close();
                    }
                    catch (chyba) {
                        $("#Podkaz").text(document.location.origin + '/' + setPath + '/' + SUBdirectory + file);
                        $("#dialogOdkaz").dialog("open");
                    }
                    
                }
                if (key == "download") {

                    StahnoutSoubor(file);

                }
                if (key == "komprimovat") {
                    seznamSouboruDelete = file;
                    Komprimovat();
                }

                if (key == "delete") {

                    seznamSouboruDelete = file;
                    $("#dialogDelete").dialog("open");

                }
                if (key == "createFolder") {
                    $("#dialogCreateDirectory").dialog("open");
                }

                if (key == "load") {
                    //myDropzone.options.url = document.location.origin + "?root=" + encodeURIComponent($('#root').val());
                    myDropzone.options.url = document.location.origin + "?uploadRoot=" + encodeURIComponent($('#uploadRoot').val());
                    //myDropzone.options.url = "https://localhost:7184/Editace/Editaceblog" + "?uploadRoot=" + encodeURIComponent($('#uploadRoot').val());  

                    $("#dialogLoad").dialog("open");

                }

                var m = key + " - " + $(this).children('td').children('.NameSelektor').text();
                window.console && console.log(m);
            }
        },
        items: {

            "unzip": { name: "Rozbalit", icon: "edit" },
            "sep1": "----------",
            "createURL": { name: "Vytvořit odkaz", icon: "edit" },
            "download": { name: "Stáhnout", icon: "edit" },
            "rename": { name: "Přejmenovat", icon: "edit" },               
            "komprimovat": { name: "Komprimovat", icon: "edit" },
            "delete": { name: "Odstranit", icon: "delete" },
            "sep2": "----------",
            "createFolder": { name: "Vytvořit adresář", icon: "edit" },
            "load": { name: "Vložit soubory", icon: "edit" }

        }
    });
    // multi soubor
    $.contextMenu({
        selector: '.context-menu-one.nu-selected',
        events: { show: function (options) { $(this).addClass('activeFIX'); }, hide: function (options) { $(this).removeClass('activeFIX'); } },
        callback: function (key, options) {
            $(this).removeClass('activeFIX');
            funkce = key;
            file = $(this).children('div').children('.FullName').val();
            koncovka = $(this).children('div').children('.Koncovka').val();
            ico = $(this).children('div').children('.Icona').css('background-image');
            if (file != "") {
                if (key == "download") {


                    $('.context-menu-one.nu-selected').each(function () {
                        StahnoutSoubor($(this).children('div').children('.FullName').val());
                    });

                }
                if (key == "komprimovat") {
                    seznamSouboruDelete = file;
                    Komprimovat();
                }
                if (key == "delete") {
                    seznamSouboruDelete = file;
                    $("#dialogDelete").dialog("open");
                }
                if (key == "createFolder") {
                    $("#dialogCreateDirectory").dialog("open");
                }

                if (key == "load") {
                    //myDropzone.options.url = document.location.origin + "?root=" + encodeURIComponent($('#root').val());
                    myDropzone.options.url = document.location.origin + "?uploadRoot=" + encodeURIComponent($('#uploadRoot').val());
                    //myDropzone.options.url = "https://localhost:7184/Editace/Editaceblog" + "?uploadRoot=" + encodeURIComponent($('#uploadRoot').val());  
                    $("#dialogLoad").dialog("open");
                }

                var m = key + " - " + $(this).children('td').children('.NameSelektor').text();
                window.console && console.log(m);
                // selekt($(this).children('td').children('.NameSelektor').text(), key);
            }
        },
        items: {

            "download": { name: "Stáhnout", icon: "edit" },
            "komprimovat": { name: "Komprimovat", icon: "edit" },
            "delete": { name: "Odstranit", icon: "delete" },
            "sep1": "----------",
            "createFolder": { name: "Vytvořit adresář", icon: "edit" },
            "load": { name: "Vložit soubory", icon: "edit" }

        }

    });
    // single soubor
    $.contextMenu({
        selector: '.context-menu-one',
        events: {show: function (options) {$(this).addClass('activeFIX');},hide: function (options) {$(this).removeClass('activeFIX');}},
        callback: function (key, options) {           
            funkce = key;
            file = $(this).children('div').children('.FullName').val();
            koncovka = $(this).children('div').children('.Koncovka').val();
            ico = $(this).children('div').children('.Icona').css('background-image');
            if (file != "") {
                if (key == "rename") {
                    
                    $("#renameInput").attr("placeholder", file.replace("." + koncovka, ""));
                    $("#renameInput").attr("arg", "F");
                    $("#renameInput").val(file.replace("." + koncovka, ""));
                    $("#typSouboru").html("." + koncovka);
                    $("#dialogRename").dialog("open");
                    
                }
                if (key == "createURL") {

                    var SUBdirectory = $('#root').val();
                        if (SUBdirectory != "") {
                            SUBdirectory = SUBdirectory.replace('\\', '/') + '/';
                    }

                    try {

                        window.opener.CKEDITOR.tools.callFunction("1", document.location.origin + '/' + setPath + '/' + SUBdirectory + file);
                        window.close();
                    }
                    catch (chyba) {
                        $("#Podkaz").text(document.location.origin + '/' + setPath + '/' + SUBdirectory + file);
                        $("#dialogOdkaz").dialog("open");
                        //alert(document.location.origin + '/UzivatelskeSoubory/' + $('#root').val() + file);

                    }
                }
                if (key == "download") {

                    StahnoutSoubor(file);

                }
                if (key == "komprimovat") {
                    seznamSouboruDelete = file;
                    Komprimovat();
                }

                if (key == "delete") {

                    seznamSouboruDelete = file;
                    $("#dialogDelete").dialog("open");

                }
                if (key == "createFolder") {
                    $("#dialogCreateDirectory").dialog("open");
                }

                if (key == "load") {
                    //myDropzone.options.url = document.location.origin + "?root=" + encodeURIComponent($('#root').val());
                    myDropzone.options.url = document.location.origin + "?uploadRoot=" + encodeURIComponent($('#uploadRoot').val());
                    //myDropzone.options.url = "https://localhost:7184/Editace/Editaceblog" + "?uploadRoot=" + encodeURIComponent($('#uploadRoot').val());  

                    $("#dialogLoad").dialog("open");

                }

                var m = key + " - " + $(this).children('td').children('.NameSelektor').text();
                window.console && console.log(m);

            }
        },
        items: {
            
            "createURL": { name: "Vytvořit odkaz", icon: "edit" },
            "download": { name: "Stáhnout", icon: "edit" },
            "rename": { name: "Přejmenovat", icon: "edit" },
            "komprimovat": { name: "Komprimovat", icon: "edit" },
            "delete": { name: "Odstranit", icon: "delete" },
            "sep1": "----------",
            "createFolder": { name: "Vytvořit adresář", icon: "edit" },
            "load": {name: "Vložit soubory", icon :"edit"}
        }
    });
    // single adresar
    $.contextMenu({
        selector: '.context-menu-two',
        events: { show: function (options) { $(this).addClass('activeFIX'); }, hide: function (options) { $(this).removeClass('activeFIX'); } },
        callback: function (key, options) {

            funkce = key;
            file = $(this).children('div').children('.FullName').val();
            koncovka = $(this).children('div').children('.Koncovka').text();
            ico = $(this).children('div').children('.Icona').css('background-image');
            if (file != "") {
                if (key == "rename") {

                    //RenameOpen("");
                    $("#renameInput").attr("placeholder", file);
                    $("#renameInput").attr("arg", "D");
                    $("#renameInput").val(file);
                    $("#typSouboru").html("");
                    $("#dialogRename").dialog("open");


                }

                if (key == "komprimovat") {
                    seznamAdresaruDelete = file;
                    Komprimovat();
                }
                if (key == "delete") {

                    seznamAdresaruDelete = file;

                    $("#dialogDelete").dialog("open");

                }
                if (key == "createFolder") {
                    $("#dialogCreateDirectory").dialog("open");
                }


                if (key == "load") {
                    //myDropzone.options.url = document.location.origin + "?root=" + encodeURIComponent($('#root').val());
                   myDropzone.options.url = document.location.origin + "?uploadRoot=" + encodeURIComponent($('#uploadRoot').val());
                    //myDropzone.options.url = "https://localhost:7184/Editace/Editaceblog" + "?uploadRoot=" + encodeURIComponent($('#uploadRoot').val());  

                    $("#dialogLoad").dialog("open");
                }

                var m = key + " - " + $(this).children('td').children('.NameSelektor').text();
                window.console && console.log(m);
                // selekt($(this).children('td').children('.NameSelektor').text(), key);
            }
        },
        items: {

            "rename": { name: "Přejmenovat", icon: "edit" },
            "komprimovat": { name: "Komprimovat", icon: "edit" },
            "delete": { name: "Odstranit", icon: "delete" },
            "sep1": "----------",
            "createFolder": { name: "Vytvořit adresář", icon: "edit" },
            "load": { name: "Vložit soubory", icon: "edit" }


        }
    });
    // multi adresar
    $.contextMenu({
        selector: '.context-menu-two.nu-selected',
        events: { show: function (options) { $(this).addClass('activeFIX'); }, hide: function (options) { $(this).removeClass('activeFIX'); } },
        callback: function (key, options) {

            funkce = key;
            file = $(this).children('div').children('.FullName').val();
            koncovka = $(this).children('div').children('.Koncovka').text();
            ico = $(this).children('div').children('.Icona').css('background-image');
            if (file != "") {


                if (key == "komprimovat") {
                    seznamAdresaruDelete = file;
                    Komprimovat();
                }
                if (key == "delete") {

                    seznamAdresaruDelete = file;

                    $("#dialogDelete").dialog("open");
                    

                }
                if (key == "createFolder") {

                    $("#dialogCreateDirectory").dialog("open");
                }

                if (key == "load") {
                    //myDropzone.options.url = document.location.origin + "?root=" + encodeURIComponent($('#root').val());
                    myDropzone.options.url = document.location.origin + "?uploadRoot=" + encodeURIComponent($('#uploadRoot').val());
                    //myDropzone.options.url = "https://localhost:7184/Editace/Editaceblog" + "?uploadRoot=" + encodeURIComponent($('#uploadRoot').val());  

                    $("#dialogLoad").dialog("open");
                }

                var m = key + " - " + $(this).children('td').children('.NameSelektor').text();
                window.console && console.log(m);
                // selekt($(this).children('td').children('.NameSelektor').text(), key);
            }
        },
        items: {
        "komprimovat": { name: "Komprimovat", icon: "edit" },
        "delete": { name: "Odstranit", icon: "delete" },
        "sep1": "----------",
        "createFolder": { name: "Vytvořit adresář", icon: "edit" },
        "load": { name: "Vložit soubory", icon: "edit" }

        }
    });
    
    
}


    
function RefreshParent() {

        window.opener.location.reload();
    }


function DialogNahled() {
    $("#dialogNahled").dialog(
        {
            autoOpen: false, height: 'auto', width: 'auto', modal: true,
            close: function () {
                //$("#dialogNahled").first().first().empty();
            }

        }
    );
}



function DialogLoad() {
    $("#dialogLoad").dialog(
        {
            autoOpen: false,modal: true,
            close: function () {
                myDropzone.removeAllFiles();
                //$('#FMC').load('/filemanager/filemanager?SET=' + encodeURIComponent($('#settings').val()) + '&LD=' + encodeURIComponent($('#root').val()) + ' #managerFileKolekce', function () { StartManager(); InitMenu(); });
                $('#FMC').load('/filemanager/filemanager?SET=' + encodeURIComponent($('#settings').val()) +'&LD=' + encodeURIComponent($('#uploadRoot').val()) + ' #managerFileKolekce', function () { StartManager(); InitMenu(); });
 }
            
        }
    );
}

function DialogOdkaz() {
    $("#dialogOdkaz").dialog({
        autoOpen: false, height: 'auto', width: 'auto',modal: true
    });
}

function DialogRename() {
    $("#dialogRename").dialog(
        {
            autoOpen: false, modal: true, buttons: [
                {
                    text: "OK",
                    click: function () {
                        if ($("#renameInput").val() != "")
                        {          
                            if ($("#renameInput").attr("arg") == "F") {
                                var oldName = $("#renameInput").attr("placeholder") + "." + koncovka;
                                var newName = $("#renameInput[type=text]").val() + "." + koncovka;
                                $('#FMC').load('/filemanager/filemanager?RenameF=' + encodeURIComponent(oldName + "|" + newName) + '&SET=' + encodeURIComponent($('#settings').val()) +'&LD=' + encodeURIComponent($('#root').val()) + ' #managerFileKolekce', function () { StartManager(); InitMenu(); });
                            }
                            if ($("#renameInput").attr("arg") == "D") {
                                var oldName = $("#renameInput").attr("placeholder");
                                var newName = $("#renameInput[type=text]").val();
                                $('#FMC').load('/filemanager/filemanager?RenameD=' + encodeURIComponent(oldName + "|" + newName) + '&SET=' + encodeURIComponent($('#settings').val()) +'&LD=' + encodeURIComponent($('#root').val()) + ' #managerFileKolekce', function () { StartManager(); InitMenu(); });
                            }
                        }


                        $(this).dialog("close");
                    }
                }
            ]
        }
    );
}

function DialogDelete() {
    $("#dialogDelete").dialog(
        {
            autoOpen: false, modal: true, buttons: [
                {
                    text: "Odstranit",
                    click: function () {

                        var mazani = "";
                        if (koncovka == "")
                        { mazani = SeznamSouboru("", seznamSouboruDelete) + '|' + SeznamAdresaru(file, seznamAdresaruDelete); }
                        else
                        { mazani  = SeznamSouboru(file, seznamSouboruDelete) + '|' + SeznamAdresaru("", seznamAdresaruDelete); }

                        $('#FMC').load('/filemanager/filemanager?Delete=' + encodeURIComponent(mazani) + '&SET=' + encodeURIComponent($('#settings').val()) +'&LD=' + encodeURIComponent($('#root').val()) + ' #managerFileKolekce', function () { StartManager(); InitMenu(); });
                    
                        seznamSouboruDelete = "";
                        seznamAdresaruDelete = "";
                        $(this).dialog("close");
                    }
                },{
                    text: "Zrušit",
                    click: function () {

                        $(this).dialog("close");
                    }
                }
                
                
            ]
        }
    );
}

function DialogCreate() {
    $("#dialogCreateDirectory").dialog(
        {
            autoOpen: false, modal: true, buttons: [
                {
                    text: "Vytvořit",
                    click: function () {

                        var newName = $("#directoryName[type=text]").val();

                        $('#FMC').load('/filemanager/filemanager?CreateD=' + encodeURIComponent(newName) + '&SET=' + encodeURIComponent($('#settings').val()) +'&LD=' + encodeURIComponent($('#root').val()) + ' #managerFileKolekce', function () { StartManager(); InitMenu(); });

                        $(this).dialog("close");
                    }
                }
            ]
        }
    );
}
function Komprimovat() {
    var mazani = "";
    if (koncovka == "") { mazani = SeznamSouboru("", seznamSouboruDelete) + '|' + SeznamAdresaru(file, seznamAdresaruDelete); }
    else { mazani = SeznamSouboru(file, seznamSouboruDelete) + '|' + SeznamAdresaru("", seznamAdresaruDelete); }

    $('#FMC').load('/filemanager/filemanager?ZIP=' + encodeURIComponent(mazani) + '&SET=' + encodeURIComponent($('#settings').val()) +'&LD=' + encodeURIComponent($('#root').val()) + ' #managerFileKolekce', function () { StartManager(); InitMenu(); });

    seznamSouboruDelete = "";
    seznamAdresaruDelete = "";

}
function Dekomprimovat() {
    $('#FMC').load('/filemanager/filemanager?UNZIP=' + encodeURIComponent(file) + '&SET=' + encodeURIComponent($('#settings').val()) +'&LD=' + encodeURIComponent($('#root').val()) + ' #managerFileKolekce', function () { StartManager(); InitMenu(); });

}
/*
function VytvoritOdkaz(file) {
    var SUBdirectory = $('#root').val();
    if (SUBdirectory != "") {
        SUBdirectory = SUBdirectory.replace('\\', '/') + '/';
    }
    try {

        window.opener.CKEDITOR.tools.callFunction("1", document.location.origin + '/UzivatelskeSoubory/' + SUBdirectory + file);
        window.close();
    }
    catch (chyba) {
        alert(document.location.origin + '/UzivatelskeSoubory/' + $('#root').val() + file);
    }
}
*/

function StahnoutSoubor(file) {
    $.ajax({
        url: document.location.origin + "/" + setPath + "/" + file,
        method: 'GET',
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data) {
            var a = document.createElement('a');
            var url = window.URL.createObjectURL(data);
            a.href = url;
            a.download = file;
            document.body.append(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);
        }
    });
}
function SeznamSouboru(elementSoubor, seznam) {
    $('.context-menu-one.nu-selected').each(function () {

        if ($(this).children('div').children('.FullName').val() != elementSoubor) {

            if (seznam == "") {
                seznam += $(this).children('div').children('.FullName').val();
            }
            else {
                seznam += "%" + $(this).children('div').children('.FullName').val();
            }
        }
    });
    return seznam;
}
function SeznamAdresaru(elementAdresare, seznam) {
    $('.context-menu-two.nu-selected').each(function () {
        if ($(this).children('div').children('.FullName').val() != elementAdresare) {
            if (seznam == "") {
                seznam += $(this).children('div').children('.FullName').val();
            }
            else {
                seznam += "%" + $(this).children('div').children('.FullName').val();
            }
        }

    });
    return seznam;
}

