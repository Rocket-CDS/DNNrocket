
var ajaxPostCmd = [];
var debugmode = false;

(function ($) {

    $.fn.getSimplisity = function (cmdurl, scmd, sfields, safter) {
        if (debugmode) {
            // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
            console.log('[$.fn.getSimplisity] ', cmdurl, scmd, '#' + this.attr('id'), sfields);
        }
        simplisityPost(cmdurl, scmd, '', '#' + this.attr('id'), '', false, 0, sfields, true, safter);
    };

}(jQuery));


(function ($) {

    $.fn.activateSimplisityPanel = function (cmdurl) {

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
            debug: false
        }, options);

        debugmode = settings.debug;

        $('#simplisity_loader').remove();
        $('#simplisity_fileuploadlist').remove();
        $('#simplisity_params').remove();
        $('#simplisity_searchfields').remove();
        $('#simplisity_systemkey').remove();
        $('#simplisity_cmdurl').remove();

        var elementstr = '<div class="' + settings.overlayclass + ' " style="" id="simplisity_loader">';
        elementstr += '<i class="fa fa-spinner fa-spin w3-display-middle " style="font-size:48px"></i>';
        elementstr += '</div>';

        elementstr += '<div class="' + settings.overlayclass + '" style="" id="simplisity_fullloader"></div>';

        elementstr += '<input id="simplisity_fileuploadlist" type="hidden" value="" />';
        elementstr += '<input id="simplisity_params" type="hidden" value="" />';
        elementstr += '<input id="simplisity_searchfields" type="hidden" value="" />';
        elementstr += '<input id="simplisity_systemkey" type="hidden" value="' + settings.systemkey + '" />';
        elementstr += '<input id="simplisity_cmdurl" type="hidden" value="' + cmdurl + '" />';

        var elem = document.createElement('span');
        elem.innerHTML = elementstr;
        document.body.appendChild(elem);

        $('.simplisity_panel').each(function () {
            $(this).attr('s-activepanel', settings.activatepanel);
            ajaxPostCmd.push(this);
        });

        panelAjaxFunction(ajaxPostCmd[ajaxPostCmd.length - 1]);

        $('#simplisity_loader').hide();

    };
}(jQuery));

function panelAjaxFunction(panelelement) {
    if ((typeof panelelement !== 'undefined') && panelelement !== '') {
        ajaxPostCmd.pop();

        var sreturn = '#' + $(panelelement).attr('id');
        var activepanel = $(panelelement).attr('s-activepanel');
        var cmdurl = $('simplisity_cmdurl').val();

        if (activepanel) {
            $(panelelement).activateSimplisityPanel(cmdurl);
        }

        simplisity_callserver(panelelement, cmdurl, sreturn);

        if (debugmode) {
            // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
            console.log('[panelAjaxFunction()] s-cmd: ', $(panelelement).attr('s-cmd'));
        }
    }
}

function IsValidTrackCmd(cmd) {
    var tcmd = $('#simplisity_validtrackcmd').val();
    if (tcmd === ',,') {
        return true;
    }
    if (tcmd.includes(',' + cmd + ',')) {
        return true;
    }
    return false;
}


$(document).on("simplisitypostgetcompleted", simplisity_nbxgetCompleted); // assign a completed event for the ajax calls

function simplisity_nbxgetCompleted(e) {


    if ((typeof e.safter !== 'undefined') && e.safter !== '') {
        var funclist = e.safter.split(',');
        for (var i = 0; i < funclist.length; i++) {
            window[funclist[i]]();
        }
    }

    // clear any uploaded files after completed call
    $('input[id*="simplisity_fileuploadlist"]').val('');
       
    if (debugmode) {
        // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
        console.log('-------END AJAX CALL------- ');
    }

    panelAjaxFunction(ajaxPostCmd[ajaxPostCmd.length - 1]);

    $('.simplisity_fadeout').fadeOut(2000);

    if (e.sloader === true) {
        $('#simplisity_loader').hide();
    }

 }

function simplisityPost(scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, sdropdownlist, reload) {

    if (debugmode) {
        // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
        console.log('[simplisityPost()] scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, sdropdownlist:--->    ', scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, sdropdownlist);
    }

    var systemkey = simplisity_getsystemkey(sfields);

    if (typeof reload === 'undefined' || reload === '') {
        reload = 'false';
    }

    var cmdupdate = '';
    if (scmdurl.includes("?")) {
        // possible external call and cmd param not required.
        cmdupdate = scmdurl;
    }
    else {
        cmdupdate = scmdurl + '?cmd=' + scmd + '&systemkey=' + simplisity_encode(systemkey);
    }

    var jsonData = ConvertFormToJSON(spost, slist);
    var jsonParam = ConvertParamToJSON(sfields);

    if ((typeof sdropdownlist !== 'undefined') && sdropdownlist !== '') {

        if (debugmode) {
            // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
            console.log('------- START AJAX CALL [dropdown] ------- ' + scmd);
        }

        $.ajax({
            type: "POST",
            url: cmdupdate,
            cache: false,
            async: true,
            dataType: 'json',
            timeout: 120000,
            data: { inputjson: encodeURIComponent(jsonData), paramjson: encodeURIComponent(jsonParam), simplisity_cmd: scmd },
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

        if (debugmode) {
            // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
            console.log('------- START AJAX CALL------- ' + scmd);
        }

        var request = $.ajax({
            type: "POST",
            url: cmdupdate,
            async: true,
            cache: false,
            timeout: 120000,
            data: { inputjson: encodeURIComponent(jsonData), paramjson: encodeURIComponent(jsonParam), simplisity_cmd: scmd }
        });

        request.done(function (data) {
            if (data !== 'noaction') {
                if ((typeof sreturn !== 'undefined') && sreturn !== '') {
                    if ((typeof sappend === 'undefined') || sappend === '' || sappend === false) {
                        $(sreturn).children().remove();
                        $(sreturn).html(data).trigger('change');
                    } else {
                        $(sreturn).append(data).trigger('change');
                    }
                }

                if (debugmode) {
                    // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
                    //console.log('returned data: ' + data);
                }

                if (reload === 'true') {
                    location.reload();
                } else {
                    // trigger completed.
                    $.event.trigger({
                        type: "simplisitypostgetcompleted",
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

    if ($(element).attr("s-stop") !== 'stop') {

        var scmdurl = $(element).attr("s-cmdurl");
        if (typeof scmdurl === 'undefined' || scmdurl === '') {
            scmdurl = cmdurl;
        }
        if (typeof scmdurl === 'undefined' || scmdurl === '') {
            scmdurl = $('#simplisity_cmdurl').val();
        }

        var sreturn = $(element).attr("s-return");
        if (typeof sreturn === 'undefined') {
            sreturn = returncontainer;
            if (typeof sreturn === 'undefined' || sreturn === '') {
                sreturn = '#simplisity_startpanel';
            }
        }

        if (typeof $(element).attr("s-reload") !== 'undefined' && $(element).attr("s-reload") !== '') {
            reload = $(element).attr("s-reload");
        }

        var scmd = $(element).attr("s-cmd");
        var spost = $(element).attr("s-post");
        var slist = $(element).attr("s-list");
        var sappend = $(element).attr("s-append");
        var sindex = $(element).attr("s-index");
        var sfields = $(element).attr("s-fields");
        var safter = $(element).attr("s-after");
        var shideloader = $(element).attr("s-hideloader");
        var sdropdownlist = $(element).attr("s-dropdownlist");

        if (typeof scmd === 'undefined') {
            scmd = '';
        }

        if (typeof sfields === 'undefined') {
            sfields = '';
        }

        simplisity_setParamField('activevalue',$(element).val());

        if (typeof shideloader === 'undefined') {
            shideloader = true;
        }
        if ($('input[id*="simplisity_fileuploadlist"]').val() !== '') {
            if (typeof sfields === 'undefined') {
                sfields = '{"fileuploadlist":"' + $('input[id*="simplisity_fileuploadlist"]').val() + '"}';
            } else {
                sfields = sfields.substring(0, sfields.length - 1) + ',"fileuploadlist":"' + $('input[id*="simplisity_fileuploadlist"]').val() + '"}';
            }
        }

        simplisityPost(scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, sdropdownlist, reload);
    }
    else {
        $(element).attr('s-stop', '');
        $('#simplisity_loader').hide();
    }

    return;
}

function ConvertParamToJSON(sfields) {

    var viewData = {
        sfield: [],
        system: []
    };

    // Put s-fields into the json object.
    var jsonDataF = {};
    if (typeof sfields !== 'undefined' && sfields !== '') {
        var obj = JSON.parse(sfields);
        jsonDataF = mergeJson({}, jsonDataF, obj);
    }

    // add any search fields
    var searchfields = $('#simplisity_searchfields').val();
    if (typeof searchfields !== 'undefined' && searchfields !== '') {
        var obj1 = JSON.parse(searchfields);
        jsonDataF = mergeJson({}, jsonDataF, obj1);
    }

    // add param fields
    var paramfields = $('#simplisity_params').val();
    if (typeof paramfields !== 'undefined' && paramfields !== '') {
        var obj2 = JSON.parse(paramfields);
        jsonDataF = mergeJson({}, jsonDataF, obj2);
    }

    viewData.sfield.push(jsonDataF);

    var system = '{"systemkey":"' + simplisity_getsystemkey(sfields) + '"}';
    var systemobj = JSON.parse(system);
    viewData.system.push(systemobj);

    if (debugmode) {
        // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
        console.log('[ConvertParamToJSON(sfields)] stringify json: ' + JSON.stringify(viewData));
    }

    return JSON.stringify(viewData);
}


function ConvertFormToJSON(spost, slist) {
    var viewData = {
        postdata: [],
        listdata: []        
    };

    // put input fields into the json object
    if (typeof spost !== 'undefined' && spost !== '') {
        var sposts = spost.split(',');
        sposts.forEach((post) => {
            $(post).find('input, textarea, select').each(function () {

                // if html type is NOT one supported, then convert it to hidden.
                var htmlType = this.getAttribute("type");
                if (htmlType !== '' && htmlType !== 'checkbox' && htmlType !== 'text' && htmlType !== 'radio' && htmlType !== 'select' && htmlType !== 'hidden') {
                    htmlType = 'hidden';
                }

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
                    jsonData['type'] = htmlType || 'select';
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

    if (debugmode) {
        // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
        console.log('json: ' + JSON.stringify(viewData));
    }

    return JSON.stringify(viewData);
}


function simplisity_getpostjson(spost) {
    var viewData = {
        postdata: []
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

    if (debugmode) {
        // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
        console.log('json: ' + JSON.stringify(viewData));
    }

    return JSON.stringify(viewData);
}


function simplisity_getlistjson(slist) {
    var viewData = {
        listdata: []
    };

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

    if (debugmode) {
        // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
        console.log('json: ' + JSON.stringify(viewData));
    }

    return JSON.stringify(viewData);
}



function mergeJson(target) {
    for (var argi = 1; argi < arguments.length; argi++) {
        var source = arguments[argi];
        for (var key in source) {
            target[key] = source[key];
        }
    }
    return target;
}

function simplisity_removetablerow(item) {
    simplisity_remove(item, 'tr');
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

function simplisity_emptyrecyclebin(recyclebin) {
    $('#simplisity_recyclebin_' + recyclebin).remove();
    $('.simplisity_itemundo[s-recylebin="' + recyclebin + '"]').hide();
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

function simplisity_setParamField(fieldkey, fieldvalue) {
    if (typeof fieldvalue !== 'undefined' && typeof fieldkey !== 'undefined') {
        var jsonParams = $('#simplisity_params').val();
        var obj = {};
        if (typeof jsonParams !== 'undefined' && jsonParams !== '') {
            obj = JSON.parse(jsonParams);
        }
        obj[fieldkey] = fieldvalue;
        $('#simplisity_params').val(JSON.stringify(obj));
    }
}

function simplisity_getParamField(fieldkey) {
    return simplisity_getField($('#simplisity_params').val(), fieldkey);
}

function simplisity_getField(sfields, fieldkey) {
    if (typeof sfields !== 'undefined' && sfields !== '') {
        if (typeof fieldkey !== 'undefined' && fieldkey !== '') {
            var obj = JSON.parse(sfields);
            return obj[fieldkey];
        }
    }
    return '';
}

function simplisity_getsystemkey(sfields) {
    var systemkey = simplisity_getField(sfields, 'systemkey');
    if (typeof systemkey === 'undefined' || systemkey === '') {
        systemkey = $('#simplisity_systemkey').val();
    }
    return systemkey;
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
    var searchfields = '{';
    var found = false;
    $('.simplisity_searchfield').each(function (index) {
        searchfields = searchfields + '"' + $(this).attr('id') + '":"' + $(this).val() + '",';
        found = true;
    });
    searchfields = searchfields.substring(0, searchfields.length - 1) + '}';

    if (found) {
        $('#simplisity_searchfields').val(searchfields);
        return searchfields;
    }
    return '';
}

function simplisity_encode(value) {
    var rtn = '';
    if (typeof value !== 'undefined' && value !== '') {
        for (var i = 0; i < value.length; i++) {
            rtn += value.charCodeAt(i) + '.';
        }
    }
    return rtn;
}

function simplisity_decode(value) {
    var rtn = '';
    if (typeof value !== 'undefined' && value !== '') {
        var valuelist = value.split('.');
        for (var i = 0; i < valuelist.length; i++) {
            if (valuelist[i] !== '') {
                rtn += String.fromCharCode(valuelist[i]);
            }
        }
    }
    return rtn;
}


async function initFileUpload(fileuploadselector) {

    var filecount = 0;
    var filesdone = 0;
    var systemkey = simplisity_getsystemkey($(fileuploadselector).attr('s-fields'));  // use systemkey so we can have multiple cookie for Different systems.
    if (systemkey === '' || typeof systemkey === 'undefined') {
        systemkey = $('#simplisity_systemkey').val();
    }

    var rexpr = $(fileuploadselector).attr('s-regexpr');
    if (rexpr === '') {
        rexpr = '/(\.|\/)(gif|jpe?g|png|pdf|zip|xml|json)$/i';
    }
    var maxFileSize = $(fileuploadselector).attr('s-maxfilesize');
    if (maxFileSize === '') {
        maxFileSize = 5000000;
    }

    $.cleanData($(fileuploadselector));

    $(fileuploadselector).off();

        $(fileuploadselector).fileupload({
            url: $(fileuploadselector).attr('s-uploadcmdurl'),
            maxFileSize: maxFileSize,
            acceptFileTypes: rexpr,
            dataType: 'json',
            dropZone: $(fileuploadselector).parent(),
            formData: { systemkey: systemkey }
        }).prop('disabled', !$.support.fileInput).parent().addClass($.support.fileInput ? undefined : 'disabled')
            .bind('fileuploadprogressall', function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $('#progress .progress-bar').css('width', progress + '%');
            })
            .bind('fileuploadadd', function (e, data) {
                $('input[id*="simplisity_fileuploadlist"]').val('');
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

            $(this).unbind("change");
            $(this).change(function () {

                simplisity_searchfields();

                simplisity_callserver(this, cmdurl);
            });

        });

        $('.simplisity_click').each(function (index) {

            $(this).attr("s-index", index);

            $(this).unbind("click");
            $(this).click(function () {
                simplisity_searchfields();
                simplisity_callserver(this, cmdurl);
            });

        });

        $('.simplisity_confirmclick').each(function (index) {

            $(this).attr("s-index", index);

            $(this).unbind("click");
            $(this).click(function () {
                if (confirm($(this).attr("s-confirm"))) {
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

        $('.simplisity_fileupload').each(function (index) {
            initFileUpload('#' + $(this).attr('id'));
        });

        $('.simplisity_filedownload').each(function (index) {
            var params = "cmd=" + $(this).attr('s-cmd');
            var sfields = $(this).attr('s-fields');

            if (typeof sfields !== 'undefined' && sfields !== '') {

                var obj = JSON.parse(sfields);
                for (x in obj) {
                    params = params + '&' + x + '=' + simplisity_encode(simplisity_getField(sfields, x));
                }

                var systemkey = simplisity_getsystemkey(sfields);  // use systemkey so we can have multiple cookie for Different systems.
                params = params + '&systemkey=' + simplisity_encode(systemkey);
            }
            var cmdurl = $("#simplisity_cmdurl").val();
            $(this).attr({
                href: cmdurl + '?' + params
            });
        });

}
