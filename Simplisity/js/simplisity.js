
(function ($) {

    $.fn.getSimplisity = function (cmdurl, scmd, sfields, safter, strack) {
        //console.log('$.fn.getSimplisity: ', cmdurl, scmd, '#' + this.attr('id'), sfields);
        simplisityPost(cmdurl, scmd, '', '#' + this.attr('id'), '', false, 0, sfields, true, safter, strack);
    };

}(jQuery));


(function ($) {

    $.fn.activateSimplisityPanel = function (cmdurl, options) {

        var settings = $.extend({
        }, options);

        simplisity_assignevents(cmdurl);

        $(this).unbind("change");
        $(this).change(function () {
            simplisity_assignevents(cmdurl);
        });

    };

    }(jQuery));

(function ($) {

    $.fn.simplisityStartUp = function (cmdurl, options) {

        var settings = $.extend({
            activatepanel: true,
            overlayclass: 'w3-overlay',
            usehistory: true
        }, options);

        $('#simplisity_loader').remove();
        $('#simplisity_systemprovider').remove();
        $('#simplisity_fileuploadlist').remove();

        var elementstr = '<div class="' + settings.overlayclass + '" style="" id="simplisity_loader"></div>';
        elementstr += '<input id="simplisity_systemprovider" type="hidden" value="" />';
        elementstr += '<input id="simplisity_fileuploadlist" type="hidden" value="" />';

        var elem = document.createElement('span');
        elem.innerHTML = elementstr;
        document.body.appendChild(elem);

        $('#simplisity_systemprovider').val('');

        var systemprovider = simplisity_getSystemProvider($('#simplisity_startpanel').attr('s-fields'));  // use systemprovider so we can have multiple cookie for Different systems.
        if (systemprovider === '' || typeof systemprovider === 'undefined') {
            systemprovider = settings.systemprovider;
        }
        $('#simplisity_systemprovider').val(systemprovider);

        var scmd = simplisity_getCookieValue('s-cmd-menu-' + systemprovider);
        var sfields = simplisity_getCookieValue('s-fields-menu-' + systemprovider);
        if (scmd !== '' && settings.usehistory === true) {
            $('#simplisity_startpanel').removeAttr('s-cmd');
            $('#simplisity_startpanel').removeAttr('s-fields');
            $('#simplisity_startpanel').attr('s-cmd', scmd);
            $('#simplisity_startpanel').attr('s-fields', sfields);
        }

        $('.simplisity_panel').each(function () {
            var sreturn = '#' + $(this).attr('id');
            simplisity_callserver(this, cmdurl, sreturn);
            if (settings.activatepanel) {
                $(this).activateSimplisityPanel(cmdurl, options);
            }
        });

        $('#simplisity_loader').hide();

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

    // a change of langauge has been triggered.
    var nextlang = simplisity_getCookieValue('nextlang');
    if (nextlang !== '') {
        simplisity_setCookieValue('editlang', nextlang);
        simplisity_setCookieValue('nextlang', '');
        location.reload();
    }

    if (e.sloader === true) {
        $('#simplisity_loader').hide();
    }

    // show any button menus defined.
    if ($('.simplisity_buttons').length >= 1) {
        $('.simplisity_buttonpanel').empty();
        $('.simplisity_buttons').appendTo('.simplisity_buttonpanel');
        $('.simplisity_buttons').show();
        $('.simplisity_buttonpanel > .simplisity_buttons').removeClass('simplisity_buttons');
    }

    // clear any uploaded files after completed call
    $('input[id*="simplisity_fileuploadlist"]').val('');

}

function simplisityPost(scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, strack, sdropdownlist, reload) {

    //console.log('scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, strack, sdropdownlist:--->    ', scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, strack, sdropdownlist);

    var systemprovider = simplisity_getSystemProvider(sfields);
    simplisity_setCookieValue('s-cmdurl-' + systemprovider, scmdurl);
    simplisity_setCookieValue('s-current-systemprovider', systemprovider);

    if (typeof reload === 'undefined' || reload === '') {
        reload = 'false';
    }

    if (typeof strack !== 'undefined' && strack === 'true') {
        simplisity_setCookieValue('s-cmd-menu-' + systemprovider, scmd);
        simplisity_setCookieValue('s-fields-menu-' + systemprovider, sfields);
    }

    if (typeof strack !== 'undefined' && strack === 'clear') {
        simplisity_setCookieValue('s-cmd-menu-' + systemprovider, '');
        simplisity_setCookieValue('s-fields-menu-' + systemprovider, '');
    }

    var cmdupdate = scmdurl + '?cmd=' + scmd + '&systemprovider=' + simplisity_encode(systemprovider);

    var jsonData = ConvertFormToJSON(spost, slist, sfields);

    if ((typeof sdropdownlist !== 'undefined') && sdropdownlist !== '') {
        $.ajax({
            type: "POST",
            url: cmdupdate,
            cache: false,
            dataType: 'json',
            timeout: 90000,
            data: { inputjson: encodeURI(jsonData), simplisity_cmd: scmd },
            success: function (json) {
                var jsontest = JSON.stringify(eval("(" + json + ")"));
                var obj = JSON.parse(jsontest);
                // populate linked dropdownlist
                var len = obj.listkey.length;
                $(sdropdownlist).empty();
                for (var i = 0; i < len; i++) {
                    $(sdropdownlist).append("<option value='" + obj.listkey[i] + "'>" + obj.listvalue[i] + "</option>");
                }
                $('#simplisity_loader').hide();
            }
        });
    }
    else {

        var request = $.ajax({
            type: "POST",
            url: cmdupdate,
            cache: false,
            timeout: 90000,
            data: { inputjson: encodeURI(jsonData), simplisity_cmd: scmd }
        });

        request.done(function (data) {
            if (data !== 'noaction') {
                if ((typeof sreturn !== 'undefined') && sreturn !== '') {
                    if (typeof sappend === 'undefined' || sappend === '' || sappend === false) {
                        $(sreturn).children().remove();
                        $(sreturn).html(data).trigger('change');
                    } else
                        $(sreturn).append(data).trigger('change');
                }

                if (reload === 'true') {
                    location.reload();
                } else {
                    // trigger completed.
                    $.event.trigger({
                        type: "simplisityposytgetcompleted",
                        cmd: scmd,
                        sindex: sindex,
                        sloader: shideloader,
                        sreturn: sreturn,
                        safter: safter
                    });
                }
            }
        });

        request.fail(function (jqXHR, textStatus) {
            $('#simplisity_loader').hide();
        });


    }


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

async function simplisity_callserver(element, cmdurl, returncontainer, reload) {

    $('#simplisity_loader').show();

    await callBeforeFunction(element);

    var scmdurl = $(element).attr("s-cmdurl");
    if (typeof scmdurl === 'undefined' || scmdurl === '') {
        scmdurl = cmdurl;
    }

    var sreturn = $(element).attr("s-return");
    if (typeof sreturn === 'undefined') {
        sreturn = returncontainer;
        if (typeof sreturn === 'undefined' || sreturn === '') {
            sreturn = '#simplisity_startpanel';
        }
    }

    var scmd = $(element).attr("s-cmd");
    var spost = $(element).attr("s-post");
    var slist = $(element).attr("s-list");
    var sappend = $(element).attr("s-append");
    var sindex = $(element).attr("s-index");
    var sfields = $(element).attr("s-fields");
    var safter = $(element).attr("s-after");
    var strack = $(element).attr("s-track");
    var shideloader = $(element).attr("s-hideloader");
    var sdropdownlist = $(element).attr("s-dropdownlist");

    if (typeof scmdurl === 'undefined' || scmdurl === '') {
        var systemprovider = simplisity_getSystemProvider(sfields);
        scmdurl = simplisity_getCookieValue('s-cmdurl-' + systemprovider);
    }

    if (typeof scmd !== 'undefined' && scmd !== '') {

        if (typeof sfields === 'undefined') {
            sfields = '';
        }
        if (sfields.indexOf("activevalue") === -1)
        {            
            sfields = sfields + ',activevalue:' + simplisity_encode($(element).val().toString());
        }
        if (typeof shideloader === 'undefined') {
            shideloader = true;
        }
        if ($('input[id*="simplisity_fileuploadlist"]').val() !== '') {
            if (typeof sfields === 'undefined') {
                sfields = 'fileuploadlist:' + $('input[id*="simplisity_fileuploadlist"]').val();
            } else {
                sfields = sfields + ',fileuploadlist:' + $('input[id*="simplisity_fileuploadlist"]').val();
            }
        }
        //console.log('scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, strack, sdropdownlist:--->    ', scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, strack, sdropdownlist);
        simplisityPost(scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, strack, sdropdownlist, reload);
    }

    return;
}

function ConvertFormToJSON(spost, slist, sfields) {
    var viewData = {
        postdata: [],
        listdata: [],        
        sfield: []
    };

    // put input fields into the json object
    if (typeof spost !== 'undefined' && spost !== '') {
        var sposts = spost.split(',');
        sposts.forEach((post) => {
            $(post).find('input, textarea, select').each(function () {

                if (this.getAttribute("s-update") !== 'ignore' && this.id !== '') {

                    if (this.getAttribute("s-datatype") === 'coded') {
                        postvalue = this.value || '';
                        postvalue = simplisity_encode(postvalue);
                    }
                    else {
                        postvalue = this.value || '';
                    }

                    var jsonData = {};
                    jsonData['id'] = this.id || '';
                    jsonData['value'] = postvalue;
                    jsonData['s-post'] = post || '';
                    jsonData['s-update'] = this.getAttribute("s-update") || '';
                    jsonData['s-datatype'] = this.getAttribute("s-datatype") || '';
                    jsonData['s-xpath'] = this.getAttribute("s-xpath") || '';
                    jsonData['type'] = this.getAttribute("type") || 'select';
                    jsonData['checked'] = $(this).prop('checked') || '';
                    jsonData['name'] = this.getAttribute("name") || '';
                    viewData.postdata.push(jsonData);
                }

            });
        });
    }

    // crate any lists required.
    if (typeof slist !== 'undefined' && slist !== '') {
        var slists = slist.split(',');
        slists.forEach((list) => {

            var lp2 = 1;
            $(list).each(function () {
                $(this).find('input, textarea, select').each(function () {

                    if (this.getAttribute("s-update") !== 'ignore' && this.id !== '') {

                        if (this.getAttribute("s-datatype") === 'coded') {
                            postvalue = this.value || '';
                            postvalue = simplisity_encode(postvalue);
                        }
                        else {
                            postvalue = this.value || '';
                        }

                        var jsonDataL = {};
                        jsonDataL['id'] = this.id || '';
                        jsonDataL['value'] = postvalue || '';
                        jsonDataL['row'] = lp2.toString() || '';
                        jsonDataL['listname'] = list || '';
                        jsonDataL['s-update'] = this.getAttribute("s-update") || '';
                        jsonDataL['s-datatype'] = this.getAttribute("s-datatype") || '';
                        jsonDataL['s-xpath'] = this.getAttribute("s-xpath") || '';
                        jsonDataL['type'] = this.getAttribute("type") || 'select';
                        jsonDataL['checked'] = $(this).prop('checked') || '';
                        jsonDataL['name'] = this.getAttribute("name") || '';
                        viewData.listdata.push(jsonDataL);
                    }
                });
                lp2 += 1;
            });

        });
    }


    // Put s-fields into the json object.
    if (typeof sfields !== 'undefined' && sfields !== '') {
        sfields.replace(',,', '{comma}');
        sfields.replace('::', '{colon}');
        var sfieldlist = sfields.split(',');

        var jsonDataF = {};
        sfieldlist.forEach((field, index) => {
            if (field !== '') {
                fieldsplit = field.split(':');
                if (fieldsplit.length >= 2) {
                    jsonDataF[fieldsplit[0].replace('{comma}', ',').replace('{colon}', ':')] = simplisity_encode(fieldsplit[1].replace('{comma}', ',').replace('{colon}', ':')) || '';
                }
            }
        });

        // add any search fields
        var searchfields = simplisity_getCookieValue('s-searchfields');
        var searchList = searchfields.split(',');
        searchList.forEach((field, index) => {
            if (typeof field !== 'undefined') {
                fieldsplit = field.split(':');
                if (typeof fieldsplit[0] !== 'undefined' && fieldsplit[0] !== '') {
                    jsonDataF[simplisity_decode(fieldsplit[0])] = fieldsplit[1] || '';
                }
            }
        });

        // Add paging data
        var pagesize = simplisity_getCookieValue('s-pagesize');
        if (typeof pagesize !== 'undefined') {
            jsonDataF['pagesize'] = simplisity_encode(pagesize) || '';
        }
        var page = simplisity_getCookieValue('s-page');
        if (typeof page !== 'undefined') {
            jsonDataF['page'] = simplisity_encode(page) || '';
        }


        viewData.sfield.push(jsonDataF);
    }

    //console.log('json: ' + JSON.stringify(viewData));

    return JSON.stringify(viewData);
}


function simplisity_removetablerow(item) {
    simplisity_remove(item, 'tr')
}

function simplisity_removelistitem(item) {
    simplisity_remove(item, 'li');
}

function simplisity_remove(item, tagName) {
    var slist = $(item).attr('s-removelist').replace('.','');
    var sindex = $(item).attr('s-index');
    var liItem = $(item).parents(tagName + "[s-index=" + sindex + "]").first().removeClass(slist).addClass(slist + '_deleted');
    var recylebin = $(item).attr('s-recylebin');

    if ($('#simplisity_recyclebin_' + recylebin).length > 0) {
        $('#simplisity_recyclebin_' + recylebin).append(liItem);
    } else { liItem.remove(); }
    $('.simplisity_itemundo[s-recylebin="' + recylebin + '"]').show();
}

function simplisity_undoremovelistitem(item) {
    var sreturn = $(item).attr('s-return');
    var sundoselector = $(item).attr('s-removelist') + "_deleted";
    var slist = $(item).attr('s-removelist').replace('.', '');
    var recylebin = $(item).attr('s-recylebin');

    if ($('#simplisity_recyclebin_' + recylebin).length > 0) {
        $(sreturn).append($('#simplisity_recyclebin_' + recylebin).find(sundoselector).last().removeClass(slist + "_deleted").addClass(slist));
    }
    if ($('#simplisity_recyclebin_' + recylebin).children(sundoselector).length === 0) {
        $('.simplisity_itemundo[s-recylebin="' + recylebin + '"]').hide();
    }

}

function simplisity_getCookieValue(cookiename) {
    var b = document.cookie.match('(^|;)\\s*' + cookiename + '\\s*=\\s*([^;]+)');
    return b ? b.pop() : '';
}

function simplisity_setCookieValue(cookiename, cookievalue) {
    document.cookie = cookiename + "=" + cookievalue + ";path=/";
}

function simplisity_replaceAll(target, search, replacement) {
    return target.replace(new RegExp(search, 'g'), replacement);
}

function simplisity_getDictionary(sfields) {
    var dict = [];
    if (typeof sfields !== 'undefined' && sfields !== '') {
        sfields.replace(',,', '{comma}');
        sfields.replace('::', '{colon}');
        var sfieldlist = sfields.split(',');
        sfieldlist.forEach((field, index) => {
            field.replace('{comma}', ',');
            fieldsplit = field.split(':');
                dict.push({
                    key: fieldsplit[0],
                    value: fieldsplit[1]
                });
        });
    }
    return dict;
}


function simplisity_getField(sfields, fieldname) {
    var fieldvalue = '';
    if (typeof sfields !== 'undefined' && sfields !== '') {
        sfields.replace(',,', '{comma}');
        sfields.replace('::', '{colon}');
        var sfieldlist = sfields.split(',');
        sfieldlist.forEach((field, index) => {
            field.replace('{comma}', ',');
            fieldsplit = field.split(':');
            if (fieldsplit[0] === fieldname) {
                fieldvalue = fieldsplit[1];
            }
        });
    }
    return fieldvalue;
}

function simplisity_getSystemProvider(sfields) {
    var systemprovider = simplisity_getField(sfields, 'systemprovider');
    if (systemprovider === '') {
        systemprovider = $('#simplisity_systemprovider').val();
    }
    return systemprovider;
}

function simplisity_pagechange(element, cmdurl) {
    var scmd = $(element).attr('s-cmd');
    if (typeof scmd !== 'undefined' && scmd !== '') {
        simplisity_callserver(element, cmdurl);
    } else {
        location.reload();
    }
}

var simplisity_isTextInput = function (element) {
    var nodeName = element.nodeName;
    return (nodeName === 'INPUT' &&  element.type.toLowerCase() === 'text');
};

var simplisity_isSelect = function (element) {
    var nodeName = element.nodeName;
    return nodeName === 'SELECT';
};

function simplisity_searchfields() {
    // save and search fields to cookie.
    var searchfields = '';
    $('.simplisity_searchfield').each(function (index) {
        searchfields = searchfields + simplisity_encode($(this).attr('id')) + ':' + simplisity_encode($(this).val()) + ',';
    });
    simplisity_setCookieValue('s-searchfields', searchfields);
    return searchfields;
}

function simplisity_encode(value) {
    var rtn = '';
    for (var i = 0; i < value.length; i++) {
       rtn += value.charCodeAt(i) + '.';
    }
    return rtn;
}

function simplisity_decode(value) {
    var rtn = '';
    var valuelist = value.split('.');
    for (var i = 0; i < valuelist.length; i++) {
        if (valuelist[i] !== '') {
            rtn += String.fromCharCode(valuelist[i]);
        }
    }
    return rtn;
}

async function initFileUpload(fileuploadselector) {

    var filecount = 0;
    var filesdone = 0;
    var systemprovider = simplisity_getSystemProvider($(fileuploadselector).attr('s-fields'));  // use systemprovider so we can have multiple cookie for Different systems.
    if (systemprovider === '' || typeof systemprovider === 'undefined') {
        systemprovider = $('#simplisity_systemprovider').val();
    }

    var rexpr = $(fileuploadselector).attr('s-regexpr');
    if (rexpr === '') {
        rexpr = '/(\.|\/)(gif|jpe?g|png|pdf|zip)$/i';
    }
    var maxFileSize = $(fileuploadselector).attr('s-maxfilesize');
    if (maxFileSize === '') {
        maxFileSize = 5000000;
    }

    $.cleanData($(fileuploadselector));

    $(fileuploadselector).off();

        $(fileuploadselector).fileupload({
            url: $(fileuploadselector).attr('s-cmdurl'),
            maxFileSize: maxFileSize,
            acceptFileTypes: rexpr,
            dataType: 'json',
            dropZone: $(fileuploadselector).parent(),
            formData: { systemprovider: systemprovider }
        }).prop('disabled', !$.support.fileInput).parent().addClass($.support.fileInput ? undefined : 'disabled')
            .bind('fileuploadprogressall', function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $('#progress .progress-bar').css('width', progress + '%');
            })
            .bind('fileuploadadd', function (e, data) {
                $.each(data.files, function (index, file) {
                    $('input[id*="simplisity_fileuploadlist"]').val($('input[id*="simplisity_fileuploadlist"]').val() + simplisity_encode(file.name) + ';');
                    filesdone = filesdone + 1;
                });
            })
            .bind('fileuploaddrop', function (e, data) {
                    filecount = data.files.length;
                    $('.processing').show();
            })
            .bind('fileuploadstop', function (e) {
                if (filesdone === filecount) {
                    if ($('input[id*="simplisity_fileuploadlist"]').val() !== '') {

                        var reload = $(fileuploadselector).attr('s-reload');
                        if (typeof reload === 'undefined' || reload === '') {
                            reload = 'true';
                        }
                        simplisity_callserver($(fileuploadselector), '', '', reload);

                        filesdone = 0;
                        $('.processing').hide();
                        $('#progress .progress-bar').css('width', '0');

                    }
                }
            });

}


function simplisity_assignevents(cmdurl) {

        $('.simplisity_change').each(function (index) {

            $(this).attr("s-index", index);

            simplisity_setCookieValue('s-page', '1');

            $(this).unbind("change");
            $(this).change(function () {

                simplisity_searchfields();

                simplisity_callserver(this, cmdurl);
            });

        });

    $('.simplisity_menulink').each(function (index) {

        $(this).attr("s-index", index);

        simplisity_setCookieValue('s-page', '1');

        $(this).unbind("click");
        $(this).click(function () {
            simplisity_setCookieValue('s-lastmenuindex', index);
            simplisity_callserver(this, cmdurl);
        });

    });

        $('.simplisity_click').each(function (index) {

            $(this).attr("s-index", index);

            simplisity_setCookieValue('s-page', '1');

            $(this).unbind("click");
            $(this).click(function () {
                simplisity_searchfields();
                simplisity_setCookieValue('s-lastindex', index);
                simplisity_callserver(this, cmdurl);
            });

        });

        $('.simplisity_confirmclick').each(function (index) {

            $(this).attr("s-index", index);

            $(this).unbind("click");
            $(this).click(function () {
                if (confirm($(this).attr("s-confirm"))) {
                    simplisity_setCookieValue('s-lastindex', index);
                    simplisity_callserver(this, cmdurl);
                }
            });

        });


        $('.simplisity_removelistitem').each(function (index) {
            $(this).attr("s-index", index);
            $(this).parents('li').first().attr("s-index", index);

            $(this).unbind("click");
            $(this).click(function () {
                simplisity_removelistitem(this);
            });
        });

        $('.simplisity_removetablerow').each(function (index) {
            $(this).attr("s-index", index);
            $(this).parents('tr').first().attr("s-index", index);

            $(this).unbind("click");
            $(this).click(function () {
                simplisity_removetablerow(this);
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

        $('.simplisity_pageclick').each(function (index) {

            $(this).attr("s-index", index);

            $(this).unbind("click");
            $(this).click(function () {
                var p = $(this).attr("s-page");
                simplisity_setCookieValue('s-page', p);
                simplisity_pagechange(this, cmdurl);
            });

        });

        $('.simplisity_pagesize').each(function (index) {

            if (simplisity_isSelect(this)) {
                $(this).unbind("change");
                $(this).change(function () {
                    simplisity_setCookieValue('s-pagesize', $(this).val());
                    simplisity_pagechange(this, cmdurl);
                });
            }

            if (simplisity_isTextInput(this)) {
                $(this).unbind("click");
                $(this).click(function () {
                    simplisity_setCookieValue('s-pagesize', $(this).val());
                    simplisity_pagechange(this, cmdurl);
                });
            }

        });

        $('.simplisity_fileupload').each(function (index) {
            initFileUpload('#' + $(this).attr('id'));
        });

        $('.simplisity_filedownload').each(function (index) {
            var params = "cmd=" + $(this).attr('s-cmd');
            var sfields = $(this).attr('s-fields');

            if (typeof sfields !== 'undefined' && sfields !== '') {
                sfields.replace(',,', '{comma}');
                sfields.replace('::', '{colon}');
                var sfieldlist = sfields.split(',');
                sfieldlist.forEach((field, index) => {
                    fieldsplit = field.split(':');
                    params = params + '&' + fieldsplit[0].replace('{comma}', ',').replace('{colon}', ':') + '=' + simplisity_encode(fieldsplit[1].replace('{comma}', ',').replace('{colon}', ':'));
                });
                var systemprovider = simplisity_getSystemProvider(sfields);  // use systemprovider so we can have multiple cookie for Different systems.
                params = params + '&systemprovider=' + simplisity_encode(systemprovider);
            }

            $(this).attr({
                href: cmdurl + '?' + params
            });
        });

}
