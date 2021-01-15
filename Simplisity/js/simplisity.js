
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

        simplisity_sessionfieldaction(); // set sessionfields to saved value.

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

        simplisity_createStaticPageFields(cmdurl, settings);
        simplisity_createTempPageFields();

        $('.simplisity_panel').each(function () {
            $(this).attr('s-activepanel', settings.activatepanel);
            ajaxPostCmd.push(this);
        });

        simplisity_panelAjaxFunction(ajaxPostCmd[ajaxPostCmd.length - 1]);

        $('#simplisity_loader').hide();

    };
}(jQuery));

function simplisity_createStaticPageFields(cmdurl, settings) {
    // inject static fields.
    $('#simplisity_loader').remove();
    $('#simplisity_systemkey').remove();
    $('#simplisity_cmdurl').remove();
    $('#simplisity_fileuploadlist').remove();

    var elementstr = '<div class="' + settings.overlayclass + ' " style="z-index:999;" id="simplisity_loader">';
    elementstr += '<i class="fa fa-spinner fa-spin w3-display-middle " style="font-size:48px"></i>';
    elementstr += '</div>';
    elementstr += '<div class="' + settings.overlayclass + '" style="" id="simplisity_fullloader"></div>';
    elementstr += '<input id="simplisity_systemkey" type="hidden" value="' + settings.systemkey + '" />';
    elementstr += '<input id="simplisity_cmdurl" type="hidden" value="' + cmdurl + '" />';
    elementstr += '<input id="simplisity_fileuploadlist" type="hidden" value="" />';

    var elem = document.createElement('span');
    elem.innerHTML = elementstr;
    document.body.appendChild(elem);

}

function simplisity_createTempPageFields() {
    // inject any temporary fields that simplisity needs.
    var elementstr = '';
    if ($('#simplisity_params').length === 0) {
        elementstr += '<input id="simplisity_params" type="hidden" value="" />';
        var elem = document.createElement('span');
        elem.innerHTML = elementstr;
        document.body.appendChild(elem);
    }
}
function simplisity_removepagefields() {
    // remove temporary fields that simplisity needs.
    $('#simplisity_params').remove();
}

function simplisity_panelAjaxFunction(panelelement) {
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
            console.log('[simplisity_panelAjaxFunction()] s-cmd: ', $(panelelement).attr('s-cmd'));
        }
    }
}

$(document).on("simplisitypostgetcompleted", simplisity_nbxgetCompleted); // assign a completed event for the ajax calls

function simplisity_nbxgetCompleted(e) {

    if ((typeof e.safter !== 'undefined') && e.safter !== '') {
        var funclist = e.safter.split(',');
        for (var i = 0; i < funclist.length; i++) {
            window[funclist[i]]();
        }
    }

    // Action the session fields to populate the session fields.
    simplisity_sessionfieldaction();

    // clear any uploaded files after completed call
    $('input[id*="simplisity_fileuploadlist"]').val('');

    if (debugmode) {
        // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
        console.log('-------END AJAX CALL------- ');
    }

    simplisity_panelAjaxFunction(ajaxPostCmd[ajaxPostCmd.length - 1]);

    $('.simplisity_fadeout').fadeOut(2000);

    if (e.sloader === true) {
        $('#simplisity_loader').hide();
    }

}

function simplisityPost(scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, sdropdownlist, reload, sreturntype) {

    if (debugmode) {
        // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
        console.log('[simplisityPost()] scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, sdropdownlist:--->    ', scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, sdropdownlist);
    }

    if (typeof scmd !== 'undefined' && scmd !== '') {

        // Load sessionParams into fieldParams to post to server. 
        // These are to persist data across session, for search, sort, paging, etc.
        simplisity_sessionpost();

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
            cmdupdate = scmdurl + '?cmd=' + scmd + '&systemkey=' + systemkey;
        }

        var jsonData = simplisity_ConvertFormToJSON(spost, slist, sfields);
        var jsonParam = simplisity_ConvertParamToJSON(sfields);

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
                    $(sdropdownlist).html('');
                    var jsonObj = simplisity_parsejson(json);
                    for (var i = 0; i < jsonObj.length; i++) {
                        var obj = jsonObj[i];
                        $(sdropdownlist).append("<option value='" + obj.key + "'>" + obj.value + "</option>");
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
                    if (sreturntype === 'json') {
                        data = JSON.stringify(data);
                    }
                    if ((typeof sreturn !== 'undefined') && sreturn !== '') {
                        if (sreturn === 'document') {
                            // replace the document (new FULL html page)
                            document.open();
                            document.write(data);
                            document.close();
                        } else {
                            if ((typeof sappend === 'undefined') || sappend === '' || sappend === false) {
                                $(sreturn).children().remove();
                                $(sreturn).html(data).trigger('change');
                            } else {
                                $(sreturn).append(data).trigger('change');
                            }
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

                    $('#simplisity_params').val(''); // clear param fields, so each call is fresh.
                }
            });

            request.fail(function (jqXHR, textStatus) {
                $('#simplisity_loader').hide();
            });

        }

    }
}

async function simplisity_callBeforeFunction(element) {
    if ((typeof $(element).attr('s-before') !== 'undefined') && $(element).attr('s-before') !== '') {
        var funclist = $(element).attr('s-before').split(',');
        for (var i = 0; i < funclist.length; i++) {
            window[funclist[i]]();
        }
    }
    return;
}

function simplisity_callSessionFields(element) {
    if ((typeof $(element).attr('s-sessionfield') !== 'undefined') && $(element).attr('s-sessionfield') !== '') {
        var funclist = $(element).attr('s-sessionfield').split(',');
        for (var i = 0; i < funclist.length; i++) {
            simplisity_setSessionField(funclist[i].replace('#', ''), $(funclist[i]).val());
        }
    }
    return;
}

async function simplisity_callserver(element, cmdurl, returncontainer, reload) {

    var scmd = $(element).attr("s-cmd");
    if (typeof scmd !== 'undefined' && scmd !== '' && scmd !== null) {

        $('#simplisity_loader').show();

        await simplisity_callBeforeFunction(element);

        await simplisity_callSessionFields(element);

        if ($(element).attr("s-stop") !== 'stop') {

            var scmdurl = $(element).attr("s-cmdurl");
            if (typeof scmdurl === 'undefined' || scmdurl === '') {
                scmdurl = cmdurl;
            }
            if (typeof scmdurl === 'undefined' || scmdurl === '') {
                scmdurl = $('#simplisity_cmdurl').val();
            }

            var sreturn = $(element).attr("s-return");
            if (typeof sreturn === 'undefined' || sreturn === '') {
                sreturn = returncontainer;
                if (typeof sreturn === 'undefined' || sreturn === '') {
                    sreturn = '#simplisity_startpanel';
                }
            }

            if (typeof $(element).attr("s-reload") !== 'undefined' && $(element).attr("s-reload") !== '') {
                reload = $(element).attr("s-reload");
            }

            var spost = $(element).attr("s-post");
            var slist = $(element).attr("s-list");
            var sappend = $(element).attr("s-append");
            var sindex = $(element).attr("s-index");
            var sfields = $(element).attr("s-fields");
            var safter = $(element).attr("s-after");
            var shideloader = $(element).attr("s-hideloader");
            var sdropdownlist = $(element).attr("s-dropdownlist");
            var sreturntype = $(element).attr("s-returntype");

            if (typeof scmd === 'undefined') {
                scmd = '';
            }

            if (typeof sfields === 'undefined') {
                sfields = '';
            }

            simplisity_setParamField('activevalue', $(element).val());

            if (typeof shideloader === 'undefined') {
                shideloader = true;
            }
            if ($('input[id*="simplisity_fileuploadlist"]').val() !== '') {
                if (typeof sfields === 'undefined' || sfields === '') {
                    sfields = '{"fileuploadlist":"' + $('input[id*="simplisity_fileuploadlist"]').val() + '"}';
                } else {
                    sfields = sfields.substring(0, sfields.length - 1) + ',"fileuploadlist":"' + $('input[id*="simplisity_fileuploadlist"]').val() + '"}';
                }
            }

            simplisityPost(scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, sdropdownlist, reload, sreturntype);
        }
        else {
            $(element).attr('s-stop', '');
            $('#simplisity_loader').hide();
        }
    }

    return;
}

function simplisity_ConvertParamToJSON(sfields) {

    var viewData = {
        sfield: [],
        system: []
    };

    // Put s-fields into the json object.
    var jsonDataF = {};
    if (typeof sfields !== 'undefined' && sfields !== '') {
        var obj = JSON.parse(sfields);
        jsonDataF = simplisity_mergeJson({}, jsonDataF, obj);
    }

    // add param fields
    var paramfields = $('#simplisity_params').val();
    if (typeof paramfields !== 'undefined' && paramfields !== '') {
        var obj2 = JSON.parse(paramfields);
        jsonDataF = simplisity_mergeJson({}, jsonDataF, obj2);
    }

    viewData.sfield.push(jsonDataF);

    var system = '{"systemkey":"' + simplisity_getsystemkey(sfields) + '","requesturl":"' + window.location.href + '"}';
    var systemobj = JSON.parse(system);
    viewData.system.push(systemobj);

    if (debugmode) {
        // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
        console.log('[simplisity_ConvertParamToJSON(sfields)] stringify json: ' + JSON.stringify(viewData));
    }

    return JSON.stringify(viewData);
}


function simplisity_ConvertFormToJSON(spost, slist, sfields) {
    var viewData = {
        sfield: [],
        system: [],
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

    // Put s-fields into the json object.
    var jsonDataF = {};
    if (typeof sfields !== 'undefined' && sfields !== '') {
        var obj = JSON.parse(sfields);
        jsonDataF = simplisity_mergeJson({}, jsonDataF, obj);
    }

    // add param fields
    var paramfields = $('#simplisity_params').val();
    if (typeof paramfields !== 'undefined' && paramfields !== '') {
        var obj2 = JSON.parse(paramfields);
        jsonDataF = simplisity_mergeJson({}, jsonDataF, obj2);
    }

    viewData.sfield.push(jsonDataF);

    var system = '{"systemkey":"' + simplisity_getsystemkey(sfields) + '"}';
    var systemobj = JSON.parse(system);
    viewData.system.push(systemobj);


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



function simplisity_mergeJson(target) {
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
    var slist = $(item).attr('s-removelist').replace('.', '');
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
    if (typeof fieldvalue !== 'undefined' && typeof fieldkey !== 'undefined' && fieldkey !== null && fieldkey !== 'null') {
        simplisity_createTempPageFields();
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
    return (nodeName === 'INPUT' && element.type.toLowerCase() === 'text');
};

var simplisity_isSelect = function (element) {
    var nodeName = element.nodeName;
    return nodeName === 'SELECT';
};

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

function simplisity_injectlink(value) {
    var link = '<link rel="stylesheet" href="' + value + '">';
    $('head').append(link);
}
function simplisity_injectscript(value) {
    $.getScript(value, function () { console.log('script inject'); });
}
function simplisity_systemkey() {
    return $('#simplisity_systemkey').val();
}
function simplisity_sessionjson() {
    var rtn = window.sessionStorage.getItem('simplisity_sessionparams'); // use session storage, idependant of browser.
    var out = '{"null":"null"}';
    if (rtn && typeof rtn !== 'undefined' && rtn !== '') {
        out = rtn;
    }
    try {
        var json = JSON.stringify(eval("(" + out + ")"));
        return json;
    }
    catch (err) {
        var json2 = JSON.stringify(eval("({'null':'null'})"));
        return json2;
    }
}
function simplisity_sessionremove() {
    window.sessionStorage.setItem('simplisity_sessionparams',''); // use session storage, idependant of browser.
}
function simplisity_sessionpost() {
    // This will post ALL data fields in the sessionParams back to the server in the param fields.
    var p = JSON.parse(simplisity_sessionjson());
    for (var key of Object.keys(p)) {
         simplisity_setParamField(key, p[key]);
    }

    // set a browser sessionid, to use serverside to identify the browser session
    var browser_sessionid = window.sessionStorage.getItem('browsersessionid');
    if (!browser_sessionid) {
        browser_sessionid = CreateUUID();
        window.sessionStorage.setItem('browsersessionid', browser_sessionid);
    }
    simplisity_setParamField('browsersessionid', browser_sessionid); // return browser_sessionid

    // set a browserid, to use serverside to identify the browser
    var browser_id = window.localStorage.getItem('browserid');
    if (!browser_id) {
        browser_id = CreateUUID();
        window.localStorage.setItem('browserid', browser_id);
    }
    simplisity_setParamField('browserid', browser_id); // return browser_sessionid

}

function CreateUUID() {
    rtn = ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    )
    // add time to try and get unique
    var d = new Date();
    rtn = rtn +"-" + d.getTime();
    return rtn;
}

function simplisity_setSessionField(fieldkey, fieldvalue) {
    if (typeof fieldvalue !== 'undefined' && fieldkey) {
        var jsonParams = simplisity_sessionjson();
        var obj = {};
        if (typeof jsonParams !== 'undefined' && jsonParams !== '') {
            obj = JSON.parse(jsonParams);
        }
        obj[fieldkey] = fieldvalue;

        window.sessionStorage.setItem('simplisity_sessionparams', JSON.stringify(obj)); // use session storage, idependant of browser.

        // cookie needed as cookie for toated version, where the module calls the toast server and we assign in the module before client based API call.
        simplisity_setCookieValue('simplisity_sessionparams', window.sessionStorage.getItem('simplisity_sessionparams'));

    }
}
function simplisity_getSessionField(fieldkey) {
    var result = JSON.parse(simplisity_sessionjson());
    return simplisity_getField(JSON.stringify(result), fieldkey);
}

function simplisity_parsejson(json) {
    var retval;
    if (typeof (json) === "string") {
        retval = JSON.parse(json);
    } else {
        // it's probably already an object
        retval = json;
    }
    return retval;
}


async function simplisity_initFileUpload(fileuploadselector) {

    var systemkey = simplisity_getsystemkey($(fileuploadselector).attr('s-fields'));  // use systemkey so we can have multiple cookie for Different systems.
    if (systemkey === '' || typeof systemkey === 'undefined') {
        systemkey = $('#simplisity_systemkey').val();
    }

    var rexpr = $(fileuploadselector).attr('s-regexpr');
    if (rexpr === '') {
        rexpr = '/(\.|\/)(gif|jpe?g|png|pdf|zip|xml|json)$/i';
    }
    var maxFileSize = parseInt($(fileuploadselector).attr('s-maxfilesize'));
    if (maxFileSize === '') {
        maxFileSize = 5000000000; // 5GB
    }
    var maxChunkSize = parseInt($(fileuploadselector).attr('s-maxchunksize'));
    if (maxChunkSize === '') {
        maxChunkSize = 10000000; //10MB 
    }

    $.cleanData($(fileuploadselector));

    $(fileuploadselector).off();


    $(fileuploadselector).parent().unbind("fileuploadprogressall");
    $(fileuploadselector).parent().unbind("fileuploadsubmit");
    $(fileuploadselector).parent().unbind("fileuploadadd");
    $(fileuploadselector).parent().unbind("fileuploaddrop");
    $(fileuploadselector).parent().unbind("fileuploadstop");

    $(fileuploadselector).fileupload({
        url: $(fileuploadselector).attr('s-uploadcmdurl'),
        maxFileSize: maxFileSize,
        maxChunkSize: maxChunkSize,
        acceptFileTypes: rexpr,
        dataType: 'json',
        dropZone: $(fileuploadselector).parent(),
        formData: { systemkey: systemkey }
    }).prop('disabled', !$.support.fileInput).parent().addClass($.support.fileInput ? undefined : 'disabled')
        .bind('fileuploadprogressall', function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('#simplisity-file-progress-bar').show();
            $('.simplisity-file-progress-bar').css('width', progress + '%');
            $('.simplisity-file-progress-bar').text(progress + '%');
        })
        .bind('fileuploadsubmit', function (e, data) {
            var identifier = simplisity_generateFileUniqueIdentifier(data);
            data.headers = $.extend(data.headers, { "X-File-Identifier": identifier });
        })
        .bind('fileuploadadd', function (e, data) {
            $('#simplisity-file-progress-bar').show();
            $.each(data.files, function (index, file) {
                $('input[id*="simplisity_fileuploadlist"]').val($('input[id*="simplisity_fileuploadlist"]').val() + simplisity_encode(file.name) + ';');
            });
        })
        .bind('fileuploaddrop', function (e, data) {
            $('#simplisity-file-progress-bar').show();
            $.each(data.files, function (index, file) {
                $('input[id*="simplisity_fileuploadlist"]').val($('input[id*="simplisity_fileuploadlist"]').val() + simplisity_encode(file.name) + ';');
            });
            $('.processing').show();
        })
        .bind('fileuploadstop', function (e) {

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

        });



}

function simplisity_generateFileUniqueIdentifier(data) {
    var file = data.files[0];
    result = file.relativePath || file.webkitRelativePath || file.fileName || file.name;
    return result;
}


function simplisity_assignevents(cmdurl) {

    $('.simplisity_change').each(function (index) {
        $(this).attr("s-index", index);

        $(this).unbind("change");
        $(this).change(function () {
            simplisity_callserver(this, cmdurl);
            return false;
        });
    });

    $('.simplisity_click').each(function (index) {
        $(this).attr("s-index", index);

        $(this).unbind("click");
        $(this).click(function () {
            simplisity_callserver(this, cmdurl);
            // add to browser bar and history
            var stateObj = $(this).attr("s-fields");
            if (typeof($(this).attr("href")) != 'undefined') {
                if ($(this).attr("href").match("^javascript:void(0)") === false) {
                    history.pushState(stateObj, "Title", $(this).attr("href"));
                }
            }
            return false;
        });
    });

    $('.simplisity_confirmclick').each(function (index) {

        $(this).attr("s-index", index);

        $(this).unbind("click");
        $(this).click(function () {
            if (confirm($(this).attr("s-confirm"))) {
                simplisity_callserver(this, cmdurl);
                return false;
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
        simplisity_initFileUpload('#' + $(this).attr('id'));
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
            params = params + '&systemkey=' + systemkey;
        }
        var cmdurl = $("#simplisity_cmdurl").val();
        $(this).attr({
            href: cmdurl + '?' + params
        });
    });

}

// Actions the session fields to populate them.
function simplisity_sessionfieldaction() {

    $('input.simplisity_sessionfield').each(function () {
        var v = simplisity_getSessionField($(this).attr('id'));
        if (typeof v !== 'undefined' && v !== '') {
            $(this).val(v);
        }
    });

    $('select.simplisity_sessionfield').each(function () {
        var v = simplisity_getSessionField($(this).attr('id'));
        if (typeof v !== 'undefined' && v !== '') {
            var selectctrl = $(this);
            $('#' + $(this).attr('id') + ' > option').each(function () {
                if (this.value === v) {
                    $(selectctrl).val(v);
                }
            });
        }
    });


}