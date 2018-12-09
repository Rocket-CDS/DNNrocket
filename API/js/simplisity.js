(function ($) {

    $.fn.panelButtons = function (buttonSelector) {
        $(this).empty();
        $(buttonSelector).appendTo(this);
        $(buttonSelector).show();
    };

}(jQuery));

(function ($) {

    $.fn.getSimplisity = function (cmdurl, scmd, sfields) {
        console.log('$.fn.getSimplisity: ', cmdurl, scmd, '#' + this.attr('id'), sfields);
        simplisityPost(cmdurl, scmd, '', '#' + this.attr('id'), '', false, 0, sfields, true,'');
    };

}(jQuery));


(function ($) {

    $.fn.activateSimplisityPanel = function (cmdurl, options) {

        var settings = $.extend({
            cancelicon: '<i class="fa fa-times-circle"></i>',
            selecticon: '<i class="fa fa-sort"></i>',
            moveicon: '<i class="fa fa-sign-in"></i>'
        }, options);


        $(this).unbind("change");
        $(this).change(function () {

            $('.simplisity_click').each(function (index) {

                $(this).attr("s-index", index);

                $(this).unbind("click");
                $(this).click(function () {
                    $('#simplisity_loader').show();
                    callGetFunction(this, cmdurl);
                });

            });

            $('.simplisity_confirmclick').each(function (index) {

                $(this).attr("s-index", index);

                $(this).unbind("click");
                $(this).click(function () {
                    if (confirm($(this).attr("s-confirm"))) {
                        $('#simplisity_loader').show();
                        callGetFunction(this, cmdurl);
                    }
                });

            });

            $('.simplisity_itemup').each(function (index) {
                $(this).attr("s-index", index);
                $(this).parents('li').first().attr("s-index", index);
                $(this).parents('li').first().find('.simplisity_itemdown').attr("s-index", index);
                $(this).parents('li').first().find('.simplisity_itemremove').attr("s-index", index);

                $(this).unbind("click");
                $(this).click(function () {
                    simplisity_moveitemup(this);
                });
            });

            $('.simplisity_itemdown').each(function (index) {
                $(this).unbind("click");
                $(this).click(function () {
                    simplisity_moveitemdown(this);
                });
            });

            $('.simplisity_itemremove').each(function (index) {
                $(this).unbind("click");
                $(this).click(function () {
                    simplisity_removelistitem(this);
                });
            });

            $('.simplisity_itemundo').each(function (index) {
                if (typeof $(this).attr("s-recylebin") !== 'undefined') {
                    if (typeof $('#simplisity_recyclebin_' + $(this).attr("s-recylebin")).val() === 'undefined') {
                        var elementstr = '<div id="simplisity_recyclebin_' + $(this).attr("s-recylebin") + '" style="display:none;" ></div>';
                        var elem = document.createElement('span');
                        elem.innerHTML = elementstr;
                        document.body.appendChild(elem);
                    }
                }                
                $(this).unbind("click");
                $(this).click(function () {
                    simplisity_undoremovelistitem(this);
                });
            });

            $('.simplisity_sortrow').each(function (index) {
                $(this).attr("s-index", index);

                $(this).empty();
                if ($('#simplisity_selecteditemid').val() !== '') {
                    $(this).append(settings.moveicon);
                    $(this).attr("s-status", "move");
                }
                else {
                    $(this).append(settings.selecticon);
                    $(this).attr("s-status", "select");
                }

                $(this).unbind("click");
                $(this).click(function () {

                    switch ($(this).attr("s-status")) {
                        case 'cancel':
                            $('.simplisity_sortrow').each(function (index) {
                                $(this).empty();
                                $(this).append(settings.selecticon);
                                $(this).attr("s-status", "select");
                            });
                            $('#simplisity_selecteditemid').val('');
                            break;
                        case 'move':
                            var sfields = $(this).attr('s-fields');
                            var fieldsout = '';
                            if (sfields !== '') {
                                var paramlist = sfields.split(',');
                                for (var i = 0; i < paramlist.length; i++) {
                                    var s = paramlist[i].split(':');
                                    if (s.length === 2) {
                                        if (s[1].startsWith('#')) {
                                            fieldsout = fieldsout + s[0] + ':' + $(s[1]).val();
                                        }
                                        else {
                                            fieldsout = fieldsout + s[0] + ':' + s[1];
                                        }
                                        fieldsout = fieldsout + ',';
                                    }
                                }

                                if (fieldsout.charAt(fieldsout.length - 1) === ',') {
                                    fieldsout = fieldsout.substr(0, fieldsout.length - 1);
                                }
                            }
                            fieldsout = fieldsout + ',movetopluginsid:' + $(this).attr("s-itemid");

                            $(this).attr("s-fields", fieldsout);

                            $('#simplisity_selecteditemid').val('');

                            callGetFunction(this, cmdurl);

                            break;
                        default:
                            $(this).empty();
                            $(this).append(settings.cancelicon);
                            $(this).attr("s-status", "cancel");
                            $('#simplisity_selecteditemid').val($(this).attr("s-itemid"));

                            $('.simplisity_sortrow').each(function (index) {
                                if ($(this).attr("s-status") !== 'cancel') {
                                    $(this).empty();
                                    $(this).append(settings.moveicon);
                                    $(this).attr("s-status", "move");
                                }
                            });
                    }
                });
            });

        });
    };

    }(jQuery));

(function ($) {

    $.fn.simplisityStartUp = function (cmdurl, options) {

        var settings = $.extend({
            activatepanel: true,
            overlayclass: 'w3-overlay',
        }, options);
        
        $('#simplisity_loader').remove();
        $('#simplisity_selecteditemid').remove();
        
        var elementstr = '<div class="' + settings.overlayclass + '" style="" id="simplisity_loader"></div>';
        elementstr += '<input id="simplisity_selecteditemid" type="hidden" value="" />';

        var elem = document.createElement('span');
        elem.innerHTML = elementstr;
        document.body.appendChild(elem);

            $('#simplisity_loader').show();
            $('.simplisity_panel').each(function () {
                var sreturn = '#' + $(this).attr('id');
                callGetFunction(this, cmdurl, sreturn);
                if (settings.activatepanel) {
                    $(this).activateSimplisityPanel(cmdurl, options);
                }
            });
    };
}(jQuery));

$(document).on("simplisityposytgetcompleted", simplisity_nbxgetCompleted); // assign a completed event for the ajax calls

function simplisity_nbxgetCompleted(e) {

    if ((typeof e.safter !== 'undefined') && e.safter !== '') {
        var funclist = e.safter.split(',');
        for (var i = 0; i < funclist.length; i++) {
            window[funclist[i]]();
        }
    }

    if (e.sloader === true) {
        $('#simplisity_loader').hide();
    }

}

function simplisityPost(scmdurl, scmd, spost, sreturn, spostitem, sappend, sindex, sfields, shideloader, safter) {

    var cmdupdate = scmdurl + '?cmd=' + scmd;
    var values = '';
    if (typeof spostitem === 'undefined' || spostitem === '') {
        values = simplisity_getxml(spost, sfields);
    }
    else {

        values = simplisity_getxmlitems(spost, spostitem, sfields);
    }

    var request = $.ajax({
        type: "POST",
        url: cmdupdate,
        cache: false,
        timeout: 90000,
        data: { inputxml: encodeURI(values), simplisity_cmd: scmd }
    });

    request.done(function (data) {
        if (data !== 'noaction') {
            if ((typeof sreturn !== 'undefined') && sreturn !== '') {
                if (typeof sappend === 'undefined' || sappend === '') {
                    $(sreturn).children().remove();
                    $(sreturn).html(data).trigger('change');
                } else
                    $(sreturn).append(data).trigger('change');
            }

            $.event.trigger({
                type: "simplisityposytgetcompleted",
                cmd: scmd,
                sindex: sindex,
                sloader: shideloader,
                sreturn: sreturn,
                safter: safter
            });
        }
    });

    request.fail(function (jqXHR, textStatus) {
        $('#simplisity_loader').hide();
    });
}

async function callBeforeFunction(element) {
    if ((typeof $(element).attr('s-before') !== 'undefined') && $(element).attr('s-before') !== '') {
        var funclist = $(element).attr('s-before').split(',');
        for (var i = 0; i < funclist.length; i++) {
            window[funclist[i]]();
        }
    }
    return;
}

async function callGetFunction(element, cmdurl, returncontainer) {

    await callBeforeFunction(element);

    var scmdurl = $(element).attr("s-cmdurl");
    if (typeof scmdurl === 'undefined' || scmdurl === '') {
        scmdurl = cmdurl;
    }
    var sreturn = $(element).attr("s-return");
    if (typeof sreturn === 'undefined' || sreturn === '') {
        sreturn = returncontainer;
    }

    var scmd = $(element).attr("s-cmd");
    var spost = $(element).attr("s-post");
    var spostitem = $(element).attr("s-postitem");
    var sappend = $(element).attr("s-append");
    var sindex = $(element).attr("s-index");
    var sfields = $(element).attr("s-fields");
    var safter = $(element).attr("s-after");
    var shideloader = $(element).attr("s-hideloader");
    if (typeof scmd !== 'undefined' && scmd !== '') {

        if (typeof shideloader === 'undefined') {
            shideloader = true;
        }
        console.log(scmdurl, scmd, spost, sreturn, spostitem, sappend, sindex, sfields, shideloader, safter);

        simplisityPost(scmdurl, scmd, spost, sreturn, spostitem, sappend, sindex, sfields, shideloader, safter);
    }
}

function simplisity_getxmlitems(selectordiv, selectoritemdiv, sfields) {
    // get each item div into xml format.
    var values = "<root>";

    var $inputs = $(selectordiv).children(':input');
    $inputs.each(function () {
        values += simplisity_getctrlxml($(this));
    });

    var $selects = $(selectordiv).children(' select');
    $selects.each(function () {
        strID = $(this).attr("id");
        nam = strID.split('_');
        var shortID = nam[nam.length - 1];
        var lp = 1;
        while (shortID.length < 4 && nam.length > lp) {
            lp++;
            shortID = nam[nam.length - lp];
        }

        var updAttr = $(this).attr("update");
        var strUpdate = '';
        if (typeof updAttr !== 'undefined') {
            strUpdate = 'upd="' + updAttr + '"';
        }

        values += '<f t="dd" ' + strUpdate + ' id="' + shortID + '" val="' + $(this).val() + '"><![CDATA[' + $('#' + strID + ' option:selected').text() + ']]></f>';
    });

    $(selectordiv).children(selectoritemdiv).each(function () {
        values += '<root>';
        var $iteminputs = $(this).find(':input');
        $iteminputs.each(function () {
            values += simplisity_getctrlxml($(this));
        });

        var $itemselects = $(this).find(' select');
        $itemselects.each(function () {
            strID = $(this).attr("id");
            nam = strID.split('_');
            var shortID = nam[nam.length - 1];
            var lp = 1;
            while (shortID.length < 4 && nam.length > lp) {
                lp++;
                shortID = nam[nam.length - lp];
            }

            var updAttr = $(this).attr("update");
            var strUpdate = '';
            if (typeof updAttr !== 'undefined') {
                strUpdate = 'upd="' + updAttr + '"';
            }

            values += '<f t="dd" ' + strUpdate + ' id="' + shortID + '" val="' + $(this).val() + '"><![CDATA[' + $('#' + strID + ' option:selected').text() + ']]></f>';
        });

        values += '</root>';
    });

    values += simplisity_getfields(sfields);

    values += '</root>';

    return values;
};

function simplisity_getxml(selectordiv, sfields) {

    // get all the inputs into an array.
    var values = "<root>";

    if (selectordiv !== '') {
        var $inputs = $(selectordiv + ' :input');
        $inputs.each(function () {
            values += simplisity_getctrlxml($(this));
        });

        var $selects = $(selectordiv + ' select');
        $selects.each(function () {
            strID = $(this).attr("id");
            nam = strID.split('_');
            var shortID = nam[nam.length - 1];
            var lp = 1;
            while (shortID.length < 4 && nam.length > lp) {
                lp++;
                shortID = nam[nam.length - lp];
            }
            var updAttr = $(this).attr("update");
            var strUpdate = '';
            if (typeof updAttr !== 'undefined') strUpdate = 'upd="' + updAttr + '"';

            values += '<f t="dd" ' + strUpdate + ' id="' + shortID + '" val="' + $(this).val() + '"><![CDATA[' + $('#' + strID + ' option:selected').text() + ']]></f>';
        });
    }

    values += simplisity_getfields(sfields);

    values += '</root>';

    return values;

};

function simplisity_getctrlxml(element) {

    var values = "";
    var strID = element.attr("id");
    if (strID != undefined) {
        var parentflag = false;
        var updAttr = element.attr("update");
        var strUpdate = '';
        if (updAttr != undefined)
            strUpdate = 'upd="' + updAttr + '"';
        else {
            if ($(element).parent() != undefined) {
                updAttr = $(element).parent().attr("update");
                if (updAttr != undefined) strUpdate = 'upd="' + updAttr + '"';
                parentflag = true;
            }
        }

        var nam = strID.split('_');
        var shortID = nam[nam.length - 1];
        var lp = 1;
        while (shortID.length < 4 && nam.length > lp) {
            lp++;
            shortID = nam[nam.length - lp];
        }
        if (element.attr("type") == 'radio') {
            values += '<f t="rb" ' + strUpdate + ' id="' + shortID + '" val="' + element.attr("value") + '"><![CDATA[' + element.is(':checked') + ']]></f>';
        } else if (element.attr("type") == 'checkbox') {
            var typecode = 'cb';
            if (parentflag) typecode = 'cbl';
            values += '<f t="' + typecode + '" ' + strUpdate + ' id="' + shortID + '" for="' + $('label[for=' + strID + ']').text() + '" val="' + element.attr("value") + '">' + element.is(':checked') + '</f>';
        } else if (element.attr("type") == 'text' || element.attr("type") == 'date' || element.attr("type") == 'email' || element.attr("type") == 'url') {
            if (element.attr("datatype") === undefined) {
                values += '<f t="txt" ' + strUpdate + ' id="' + shortID + '"><![CDATA[' + element.val() + ']]></f>';
            } else {
                values += '<f t="txt" ' + strUpdate + ' id="' + shortID + '" dt="' + element.attr("datatype") + '"><![CDATA[' + element.val() + ']]></f>';
            }
        } else if (element.attr("type") == 'hidden') {

            if (element.attr("datatype") == 'coded') {
                var coded = '';
                var str = element.val();
                for (var i = 0; i < str.length; i++) {
                    coded = coded + str.charCodeAt(i) + '.';
                }
                values += '<f t="hid" ' + strUpdate + ' id="' + shortID + '" dt="' + element.attr("datatype") + '">' + coded + '</f>';
            } else {
                values += '<f t="hid" ' + strUpdate + ' id="' + shortID + '"><![CDATA[' + element.val() + ']]></f>';
            }


        } else {
            values += '<f ' + strUpdate + ' id="' + shortID + '"><![CDATA[' + element.val() + ']]></f>';
        }
    }

    return values;

};

function simplisity_getfields(sfields) {

    var values = "";
    if (typeof sfields !== 'undefined' && sfields !== '') {
        sfields.replace(',,', '{comma}');
        sfields.replace('::', '{colon}');
        var sfieldlist = sfields.split(',');
        sfieldlist.forEach((field, index) => {
            field.replace('{comma}', ',');
            fieldsplit = field.split(':');
            values += '<f t="hid" id="' + fieldsplit[0].replace('{colon}', ':') + '"><![CDATA[' + fieldsplit[1].replace('{colon}', ':') + ']]></f>';
        });
    }
    return values;

};

function simplisity_moveitemup(item) {
    var sindex = $(item).attr('s-index');
    var liItem = $(item).parents("li[s-index=" + sindex + "]").first();
    var prev = $(liItem).prev();
    if (prev.length === 0)
        return;

    prev.css('z-index', 999).css('position', 'relative').animate({ top: liItem.height() }, 250);
    liItem.css('z-index', 1000).css('position', 'relative').animate({ top: '-' + prev.height() }, 300, function () {
        prev.css('z-index', '').css('top', '').css('position', '');
        liItem.css('z-index', '').css('top', '').css('position', '');
        liItem.insertBefore(prev);
    });

}

function simplisity_moveitemdown(item) {
    var sindex = $(item).attr('s-index');
    var liItem = $(item).parents("li[s-index=" + sindex + "]").first();
    var next = $(liItem).next();
    if (next.length === 0)
        return;
    next.css('z-index', 999).css('position', 'relative').animate({ top: '-' + liItem.height() }, 250);
    liItem.css('z-index', 1000).css('position', 'relative').animate({ top: next.height() }, 300, function () {
        next.css('z-index', '').css('top', '').css('position', '');
        liItem.css('z-index', '').css('top', '').css('position', '');
        liItem.insertAfter(next);
    });
}

function simplisity_removelistitem(item) {
    var sindex = $(item).attr('s-index');
    var liItem = $(item).parents("li[s-index=" + sindex + "]").first();
    var recylebin = $(item).attr('s-recylebin');

    if ($('#simplisity_recyclebin_' + recylebin).length > 0) {
        $('#simplisity_recyclebin_' + recylebin).append(liItem);
    } else { liItem.remove(); }
    $('.simplisity_itemundo[s-recylebin="' + recylebin + '"]').show();
}

function simplisity_undoremovelistitem(item) {
    var sreturn = $(item).attr('s-return');
    var sundoselector = $(item).attr('s-undoselector');
    var recylebin = $(item).attr('s-recylebin');

    if ($('#simplisity_recyclebin_' + recylebin).length > 0) {
        $(sreturn).append($('#simplisity_recyclebin_' + recylebin).find(sundoselector).last());
    }
    if ($('#simplisity_recyclebin_' + recylebin).children(sundoselector).length === 0) {
        $('.simplisity_itemundo').hide();
    }

}



