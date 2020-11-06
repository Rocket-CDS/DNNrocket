

function sidemenuaction() {

    // turn off the reload flag
    simplisity_setParamField("reload", false)

    var returnurl = sessionStorage.RocketModReturn;
    if (typeof returnurl !== 'undefined' && returnurl !== '') {
        $('.returnbutton').show();
    }

    $('.menuaccordian').unbind('click');
    $('.menuaccordian').click(function () {
        var actionid = $(this).attr("actionid")
        if ($('#' + actionid).hasClass('w3-show')) {
            $('#' + actionid).addClass('w3-hide');
            $('#' + actionid).removeClass('w3-show');
            $(this).removeClass('w3-theme-d1');
        }
        else {
            $('#' + actionid).addClass('w3-show');
            $('#' + actionid).removeClass('w3-hide');
            $(this).addClass('w3-theme-d1');
        }
    });

    $(window).resize(function () {
        if ($('#simplisity_fullloader').is(':visible')) {
            $('#mySidebar').width($('#mySidebar').parent().width());
        }
    });

    sidemenuchange();

    if ($('#menulanguageselectdiv').attr('editmode') === '1') {
        $('.langaugeselectsingleflag').appendTo('#menulanguageselectdiv');
        $('.langaugeselectsingleflag').show();
    }

}


    // Toggle between showing and hiding the sidebar, and add overlay effect
    function w3_open() {
        if (mySidebar.style.display === 'block') {
        mySidebar.style.display = 'none';
        } else {
        mySidebar.style.display = 'block';
        }
        $('#mySidebar').width($('#mySidebar').parent().width());
        $('#simplisity_fullloader').show();
    }

    // Close the sidebar with the close button
    function w3_close() {
        if ($('#simplisity_fullloader').is(':visible')) {
        mySidebar.style.display = 'none';
            $('#mySidebar').width(270)
            $('#simplisity_fullloader').hide();
        }
    }

    function sidemenuchange() {
        var menuindex = simplisity_getCookieValue('@(sidemenu.SystemKey)-menuindex');
        if (menuindex !== '') {
        $('.menubaritem').removeClass('w3-theme-d5');
            $('.menubaritem' + menuindex).addClass('w3-theme-d5');
        }
        w3_close();
        $('#sidebar_loader').hide();
    }

    function returnclick() {
        simplisity_setCookieValue('@(sidemenu.SystemKey)-menuindex', ''); // so we do not get wrong menu on admin entry.
        $('#simplisity_loader').show();
        $('.simplisity_panel').hide();
        $('#mySidebar').removeClass('w3-theme');
        $('#mySidebar').removeClass('w3-border-right');
        $('#mySidebar').add('w3-white');
        var returnurl = sessionStorage.RocketModReturn;
        if (typeof returnurl === 'undefined' || returnurl === '') {
            if (typeof window.history.back() === 'undefined') {
                var rtn = location.protocol + '//' + location.host;
                window.location.href = rtn;
            }
            else {
        window.history.back();
            }
        }
        else {
        window.location = returnurl;
        }
    }

    function languageChange() {
        $('.langchangesave').trigger("click");
    }

