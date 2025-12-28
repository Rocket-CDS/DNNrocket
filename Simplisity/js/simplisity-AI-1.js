var s_ajaxPostCmd = [];
var debugmode = false;

HTMLElement.prototype.getSimplisity = function (cmdurl, scmd, sfields, safter, spost) {
    simplisityPost(cmdurl, scmd, spost, '#' + this.getAttribute('id'), '', false, 0, sfields, true, safter, '', '', '', '');
};

HTMLElement.prototype.activateSimplisityPanel = function (cmdurl) {
    simplisity_sessionfieldaction();
    simplisity_assignevents(cmdurl);

    const changeHandler = () => {
        simplisity_assignevents(cmdurl);
    };
    this.removeEventListener('change', changeHandler);
    this.addEventListener('change', changeHandler);
};
// Convert jQuery plugin to vanilla JavaScript function
function simplisityStartUp(cmdurl, options) {
    // Default settings
    var defaultSettings = {
        activatepanel: true,
        overlayclass: '',
        debug: false,
        hideprocess: true,
        systemkey: '' // Add systemkey to defaults
    };

    // Merge user options with defaults
    var settings = Object.assign({}, defaultSettings, options || {});

    // Set global debug mode
    if (typeof window.debugmode === 'undefined') {
        window.debugmode = settings.debug;
    } else {
        window.debugmode = settings.debug;
    }

    // Create static and temporary page fields
    simplisity_createStaticPageFields(cmdurl, settings);
    simplisity_createTempPageFields();

    // Process all simplisity panels
    var panels = document.querySelectorAll('.simplisity_panel');
    panels.forEach(function (panel) {
        panel.setAttribute('s-activepanel', settings.activatepanel);
        s_ajaxPostCmd.push(panel);
    });

    // Execute panel ajax function for the last panel
    if (s_ajaxPostCmd.length > 0) {
        simplisity_panelAjaxFunction(s_ajaxPostCmd[s_ajaxPostCmd.length - 1]);
    }

    // Hide loader if specified
    if (settings.hideprocess) {
        var loaders = document.querySelectorAll('.simplisity_loader');
        loaders.forEach(function (loader) {
            loader.style.display = 'none';
        });
    }
}

// Optional: Add as HTMLElement prototype method to mimic jQuery plugin syntax
HTMLElement.prototype.simplisityStartUp = function (cmdurl, options) {
    simplisityStartUp(cmdurl, options);
    return this; // Enable chaining
};


//#region Functions ----------------------------------------------------------------
function simplisityPost(scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, sdropdownlist, reload, sreturntype, paramfields) {

    if (typeof scmd !== 'undefined' && scmd !== '') {

        if (paramfields == '') {
            // NOTE: simplisity_sessionpost(); populates the "paramfields".  The parameter "paramfields" is left for legacy.
            // Load sessionParams into fieldParams to post to server.
            // These are to persist data across session, for search, sort, paging, etc.
            paramfields = simplisity_sessionpost();
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
            cmdupdate = scmdurl + '?cmd=' + scmd + '&systemkey=' + systemkey;
        }

        var jsonData = simplisity_ConvertFormToJSON(spost, slist, sfields, paramfields);
        var jsonParam = simplisity_ConvertParamToJSON(sfields, paramfields);

        if ((typeof sdropdownlist !== 'undefined') && sdropdownlist !== '') {

            fetch(cmdupdate, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: new URLSearchParams({
                    inputjson: encodeURIComponent(jsonData),
                    paramjson: encodeURIComponent(jsonParam),
                    simplisity_cmd: scmd
                })
            })
            .then(response => response.json())
            .then(json => {
                var dropdown = document.querySelector(sdropdownlist);
                dropdown.innerHTML = '';
                var jsonObj = simplisity_parsejson(json);
                for (var i = 0; i < jsonObj.length; i++) {
                    var obj = jsonObj[i];
                    var option = document.createElement('option');
                    option.value = obj.key;
                    option.textContent = obj.value;
                    dropdown.appendChild(option);
                }
                if ((typeof safter !== 'undefined') && safter !== '') {
                    var funclist = safter.split(',');
                    for (var i = 0; i < funclist.length; i++) {
                        window[funclist[i]]();
                    }
                }
                if (shideloader === true) {
                    var loaders = document.querySelectorAll('.simplisity_loader');
                    loaders.forEach(loader => loader.style.display = 'none');
                }
            });
        }
        else {

            if (sreturntype === 'json') {
                fetch(cmdupdate, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                    },
                    body: new URLSearchParams({
                        inputjson: encodeURIComponent(jsonData),
                        paramjson: encodeURIComponent(jsonParam),
                        simplisity_cmd: scmd
                    })
                })
                .then(response => response.json())
                .then(json => {
                    window.sessionStorage.setItem(sreturn, JSON.stringify(json)); // use session storage, idependant of browser.

                    if ((typeof safter !== 'undefined') && safter !== '') {
                        var funclist = safter.split(',');
                        for (var i = 0; i < funclist.length; i++) {
                            window[funclist[i]]();
                        }
                    }
                    if (shideloader === true) {
                        var loaders = document.querySelectorAll('.simplisity_loader');
                        loaders.forEach(loader => loader.style.display = 'none');
                    }
                });
            }
            else {
                fetch(cmdupdate, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                    },
                    body: new URLSearchParams({
                        inputjson: encodeURIComponent(jsonData),
                        paramjson: encodeURIComponent(jsonParam),
                        simplisity_cmd: scmd
                    })
                })
                .then(response => response.text())
                .then(data => {
                    if (data !== 'noaction') {
                        if ((typeof sreturn !== 'undefined') && sreturn !== '') {
                            if (sreturn === 'document') {
                                // replace the document (new FULL html page)
                                document.open();
                                document.write(data);
                                document.close();
                            } else {
                                var element = document.querySelector(sreturn);
                                if (element) {
                                    if ((typeof sappend === 'undefined') || sappend === '' || sappend === false) {
                                        while (element.firstChild) {
                                            element.removeChild(element.firstChild);
                                        }
                                        element.innerHTML = data;
                                    } else {
                                        element.insertAdjacentHTML('beforeend', data);
                                    }
                                    element.dispatchEvent(new Event('change', { bubbles: true }));
                                }
                            }
                        }

                        if (reload === 'true') {
                            location.reload();
                        } else {
                            // trigger completed.
                            var event = new CustomEvent('simplisitypostgetcompleted', {
                                detail: {
                                    cmd: scmd,
                                    sindex: sindex,
                                    sloader: shideloader,
                                    sreturn: sreturn,
                                    safter: safter
                                },
                                bubbles: true
                            });
                            document.dispatchEvent(event);
                        }

                        var paramsElement = document.querySelector('#simplisity_params');
                        if (paramsElement) {
                            paramsElement.value = ''; // clear param fields, so each call is fresh.
                        }
                    }
                })
                .catch(() => {
                    var loaders = document.querySelectorAll('.simplisity_loader');
                    loaders.forEach(loader => loader.style.display = 'none');
                });
            }

        }

    }
}

document.addEventListener("simplisitypostgetcompleted", simplisity_nbxgetCompleted); // assign a completed event for the ajax calls
function simplisity_nbxgetCompleted(e) {
    // Access event detail properties (CustomEvent structure)
    var eventDetail = e.detail || e;

    if ((typeof eventDetail.safter !== 'undefined') && eventDetail.safter !== '') {
        var funclist = eventDetail.safter.split(',');
        for (var i = 0; i < funclist.length; i++) {
            if (typeof (window[funclist[i]]) === "function" || typeof (window[funclist[i]]) === "object") {
                window[funclist[i]]();
            } else {
                console.log('ERROR: function does not exist. ' + funclist[i]);
            }
        }
    }

    // Action the session fields to populate the session fields.
    simplisity_sessionfieldaction();

    // clear any uploaded files after completed call
    var fileUploadListInputs = document.querySelectorAll('input[id*="simplisity_fileuploadlist"]');
    fileUploadListInputs.forEach(function (input) {
        input.value = '';
    });

    var fileUploadBase64Inputs = document.querySelectorAll('input[id*="simplisity_fileuploadbase64"]');
    fileUploadBase64Inputs.forEach(function (input) {
        input.value = '';
    });

    if (typeof debugmode !== 'undefined' && debugmode === true) {
        // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
        console.log('-------END AJAX CALL------- ');
    }

    simplisity_panelAjaxFunction(s_ajaxPostCmd[s_ajaxPostCmd.length - 1]);

    // Fade out elements with simplisity_fadeout class
    var fadeoutElements = document.querySelectorAll('.simplisity_fadeout');
    fadeoutElements.forEach(function (element) {
        element.style.transition = 'opacity 2s';
        element.style.opacity = '0';
        setTimeout(function () {
            element.style.display = 'none';
        }, 2000);
    });

    if (eventDetail.sloader === true) {
        var loaders = document.querySelectorAll('.simplisity_loader');
        loaders.forEach(function (loader) {
            loader.style.display = 'none';
        });
    }
}
function simplisity_sessionpost() {
    // This will post ALL data fields in the sessionParams back to the server in the param fields.
    var p = simplisity_parsejson(simplisity_sessionjson());
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
    simplisity_setSessionField('browsersessionid', browser_sessionid); // add to session data so it is in the cookie

    // set a browserid, to use serverside to identify the browser
    var browser_id = window.localStorage.getItem('browserid');
    if (!browser_id) {
        browser_id = CreateUUID();
        window.localStorage.setItem('browserid', browser_id);
    }
    simplisity_setParamField('browserid', browser_id); // return browser_sessionid
    simplisity_setSessionField('browserid', browser_id); // add to session data so it is in the cookie

    var paramsEl = document.querySelector('#simplisity_params');
    return paramsEl ? paramsEl.value : ''; // get session/param values
}

async function simplisity_callBeforeFunction(element) {
    try {
        var sBeforeAttr = element.getAttribute('s-before');
        if (typeof sBeforeAttr !== 'undefined' && sBeforeAttr !== null && sBeforeAttr !== '') {
            var funclist = sBeforeAttr.split(',');
            for (var i = 0; i < funclist.length; i++) {
                if (typeof window[funclist[i]] === "function" || typeof window[funclist[i]] === "object") {
                    window[funclist[i]]();
                } else {
                    console.log('ERROR: function does not exist. ' + funclist[i]);
                }
            }
        }
    }
    catch (e) {
        console.log('Error!', e);
    }
    return;
}
function simplisity_callSessionFields(element) {
    var sessionFieldAttr = element.getAttribute('s-sessionfield');
    if (typeof sessionFieldAttr !== 'undefined' && sessionFieldAttr !== null && sessionFieldAttr !== '') {
        var funclist = sessionFieldAttr.split(',');
        for (var i = 0; i < funclist.length; i++) {
            var fieldSelector = funclist[i].replace('#', '');
            var fieldElement = document.querySelector(funclist[i]);
            var fieldValue = '';

            if (fieldElement) {
                if (fieldElement.value !== undefined) {
                    fieldValue = fieldElement.value;
                } else if (fieldElement.tagName === 'SELECT') {
                    fieldValue = fieldElement.value;
                }
            }

            simplisity_setSessionField(fieldSelector, fieldValue);
        }
    }
    return;
}

async function simplisity_callserver(element, cmdurl, returncontainer, reload) {
    try {

        // ADD THIS VALIDATION AT THE START
        // Check if element is valid and has getAttribute method
        if (!element || typeof element.getAttribute !== 'function') {
            console.error('simplisity_callserver: Invalid element passed', element);
            return;
        }

        // Get element value - handle different element types
        var elementValue = '';
        if (element.value !== undefined) {
            elementValue = element.value;
        } else if (element.tagName === 'SELECT') {
            elementValue = element.value;
        }

        simplisity_setParamField('activevalue', elementValue); // do first

        // Add language cookies to param
        simplisity_setParamField("simplisity_language", simplisity_getCookieValue("simplisity_language"));
        simplisity_setParamField("simplisity_editlanguage", simplisity_getCookieValue("simplisity_editlanguage"));

        var scmd = element.getAttribute("s-cmd");
        if (typeof scmd !== 'undefined' && scmd !== '' && scmd !== null) {

            var sshowloader = element.getAttribute("s-showloader");
            if (sshowloader !== 'false') {
                var loaders = document.querySelectorAll('.simplisity_loader');
                loaders.forEach(function (loader) {
                    loader.style.display = 'block';
                });
            }

            await simplisity_callBeforeFunction(element); // Before s-stop, so s-stop='stop' can be added on validation.

            if (element.getAttribute("s-stop") !== 'stop') {

                // FIX: Add null check here
                var scmdurl = element.getAttribute("s-cmdurl");
                if (!scmdurl || scmdurl === '') {  
                    scmdurl = cmdurl;
                }
                if (!scmdurl || scmdurl === '') {  
                    var cmdurlEl = document.querySelector('#simplisity_cmdurl');
                    scmdurl = cmdurlEl ? cmdurlEl.value : '';
                }

                // ADDITIONAL SAFETY: If still no cmdurl, log error and return
                if (!scmdurl || scmdurl === '') {
                    console.error('simplisity_callserver: No cmdurl found. Element:', element, 'cmdurl param:', cmdurl);
                    var loaders = document.querySelectorAll('.simplisity_loader');
                    loaders.forEach(function (loader) {
                        loader.style.display = 'none';
                    });
                    return;
                }

                var sreturn = element.getAttribute("s-return");
                if (typeof sreturn === 'undefined' || sreturn === '' || sreturn === null) {  
                    sreturn = returncontainer;
                    if (typeof sreturn === 'undefined' || sreturn === '' || sreturn === null) {
                        sreturn = '#simplisity_startpanel';
                    }
                }

                var reloadAttr = element.getAttribute("s-reload");
                if (typeof reloadAttr !== 'undefined' && reloadAttr !== '' && reloadAttr !== null) {
                    reload = reloadAttr;
                }

                var spost = element.getAttribute("s-post");
                var slist = element.getAttribute("s-list");
                var sappend = element.getAttribute("s-append");
                var sindex = element.getAttribute("s-index");
                var sfields = element.getAttribute("s-fields");
                var safter = element.getAttribute("s-after");
                var shideloader = element.getAttribute("s-hideloader");
                var sdropdownlist = element.getAttribute("s-dropdownlist");
                var sreturntype = element.getAttribute("s-returntype");

                if (typeof scmd === 'undefined' || scmd === null) {
                    scmd = '';
                }

                if (typeof sfields === 'undefined' || sfields === null) {  
                    sfields = '';
                }

                if (typeof shideloader === 'undefined' || shideloader === null) {
                    shideloader = true;
                }

                // Handle file upload list
                var fileUploadListInputs = document.querySelectorAll('input[id*="simplisity_fileuploadlist"]');
                var fileUploadListValue = '';
                if (fileUploadListInputs.length > 0) {
                    fileUploadListValue = fileUploadListInputs[0].value;
                }

                if (fileUploadListValue !== '') {
                    if (typeof sfields === 'undefined' || sfields === '') {
                        sfields = '{"fileuploadlist":"' + fileUploadListValue + '"}';
                    } else {
                        sfields = sfields.substring(0, sfields.length - 1) + ',"fileuploadlist":"' + fileUploadListValue + '"}';
                    }
                }

                // Handle file upload base64
                var fileUploadBase64Inputs = document.querySelectorAll('input[id*="simplisity_fileuploadbase64"]');
                var fileUploadBase64Value = '';
                if (fileUploadBase64Inputs.length > 0) {
                    fileUploadBase64Value = fileUploadBase64Inputs[0].value;
                }

                if (fileUploadBase64Value !== '') {
                    if (typeof sfields === 'undefined' || sfields === '') {
                        sfields = '{"fileuploadbase64":"' + fileUploadBase64Value + '"}';
                    } else {
                        sfields = sfields.substring(0, sfields.length - 1) + ',"fileuploadbase64":"' + fileUploadBase64Value + '"}';
                    }
                }

                simplisity_setSessionField("simplisity_return", sreturn);
                await simplisity_callSessionFields(element);

                simplisityPost(scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, sdropdownlist, reload, sreturntype, '');
            }
            else {
                element.setAttribute('s-stop', '');
                var loaders = document.querySelectorAll('.simplisity_loader');
                loaders.forEach(function (loader) {
                    loader.style.display = 'none';
                });
            }
        }
    }
    catch (e) {
        console.log('Error!', e);
    }
    return;
}

function simplisity_ConvertParamToJSON(sfields, paramfields) {

    var viewData = {
        sfield: [],
        system: []
    };

    // Put s-fields into the json object.
    var jsonDataF = {};
    if (typeof sfields !== 'undefined' && sfields !== '') {
        var obj = simplisity_parsejson(sfields);
        jsonDataF = simplisity_mergeJson({}, jsonDataF, obj);
    }

    // add param fields
    if (typeof paramfields !== 'undefined' && paramfields !== '') {
        var obj2 = simplisity_parsejson(paramfields);
        jsonDataF = simplisity_mergeJson({}, jsonDataF, obj2);
    }

    viewData.sfield.push(jsonDataF);

    var system = '{"systemkey":"' + simplisity_getsystemkey(sfields) + '","requesturl":"' + window.location.href + '"}';
    var systemobj = simplisity_parsejson(system);
    viewData.system.push(systemobj);

    if (debugmode === true) {
        // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
        console.log('[simplisity_ConvertParamToJSON(sfields)] stringify json: ' + simplisity_stringifyjson(viewData));
    }
    return simplisity_stringifyjson(viewData);
}
function simplisity_ConvertFormToJSON(spost, slist, sfields, paramfields) {
    var viewData = {
        sfield: [],
        system: [],
        postdata: [],
        listdata: []
    };

    // put input fields into the json object
    if (typeof spost !== 'undefined' && spost !== '' && spost !== null) {
        var sposts = spost.split(',');
        sposts.forEach((post) => {
            var container = document.querySelector(post);
            if (container) {
                var elements = container.querySelectorAll('input, textarea, select');
                elements.forEach(function (element) {
                    // if html type is NOT one supported, then convert it to hidden.
                    var htmlType = element.getAttribute("type");
                    if (htmlType !== '' && htmlType !== 'checkbox' && htmlType !== 'text' && htmlType !== 'radio' && htmlType !== 'select' && htmlType !== 'hidden') {
                        htmlType = 'hidden';
                    }

                    if (element.getAttribute("s-update") !== 'ignore' && element.id !== '') {
                        var postvalue;
                        if (element.getAttribute("s-datatype") === 'coded') {
                            postvalue = element.value || '';
                            postvalue = simplisity_encode(postvalue);
                        }
                        else {
                            postvalue = element.value || '';
                        }

                        var jsonData = {};
                        jsonData['id'] = element.id || '';
                        jsonData['value'] = postvalue;
                        jsonData['s-post'] = post || '';
                        jsonData['s-update'] = element.getAttribute("s-update") || '';
                        jsonData['s-datatype'] = element.getAttribute("s-datatype") || '';
                        jsonData['s-xpath'] = element.getAttribute("s-xpath") || '';
                        jsonData['type'] = htmlType || 'select';
                        jsonData['checked'] = (element.type === 'checkbox' || element.type === 'radio') ? element.checked : '';
                        jsonData['name'] = element.getAttribute("name") || '';
                        viewData.postdata.push(jsonData);
                    }
                });
            }
        });
    }

    // create any lists required.
    if (typeof slist !== 'undefined' && slist !== '' && slist !== null) {
        var slists = slist.split(',');
        slists.forEach((list) => {
            var lp2 = 1;
            var listElements = document.querySelectorAll(list);
            listElements.forEach(function (listElement) {
                var elements = listElement.querySelectorAll('input, textarea, select');
                elements.forEach(function (element) {
                    // if html type is NOT one supported, then convert it to hidden.
                    var htmlType = element.getAttribute("type");
                    if (htmlType !== '' && htmlType !== 'checkbox' && htmlType !== 'text' && htmlType !== 'radio' && htmlType !== 'select' && htmlType !== 'hidden') {
                        htmlType = 'hidden';
                    }

                    if (element.getAttribute("s-update") !== 'ignore' && element.id !== '') {
                        var postvalue;
                        if (element.getAttribute("s-datatype") === 'coded') {
                            postvalue = element.value || '';
                            postvalue = simplisity_encode(postvalue);
                        }
                        else {
                            postvalue = element.value || '';
                        }

                        var jsonDataL = {};
                        jsonDataL['id'] = element.id || '';
                        jsonDataL['value'] = postvalue || '';
                        jsonDataL['row'] = lp2.toString() || '';
                        jsonDataL['listname'] = list || '';
                        jsonDataL['s-update'] = element.getAttribute("s-update") || '';
                        jsonDataL['s-datatype'] = element.getAttribute("s-datatype") || '';
                        jsonDataL['s-xpath'] = element.getAttribute("s-xpath") || '';
                        jsonDataL['type'] = htmlType || 'select';
                        jsonDataL['checked'] = (element.type === 'checkbox' || element.type === 'radio') ? element.checked : '';
                        jsonDataL['name'] = element.getAttribute("name") || '';
                        viewData.listdata.push(jsonDataL);
                    }
                });
                lp2 += 1;
            });
        });
    }

    // Put s-fields into the json object.
    var jsonDataF = {};
    if (typeof sfields !== 'undefined' && sfields !== '' && sfields !== null) {
        var obj = simplisity_parsejson(sfields);
        jsonDataF = simplisity_mergeJson({}, jsonDataF, obj);
    }

    if (typeof paramfields !== 'undefined' && paramfields !== '' && paramfields !== null) {
        var obj2 = simplisity_parsejson(paramfields);
        jsonDataF = simplisity_mergeJson({}, jsonDataF, obj2);
    }

    viewData.sfield.push(jsonDataF);

    var system = '{"systemkey":"' + simplisity_getsystemkey(sfields) + '"}';
    var systemobj = simplisity_parsejson(system);
    viewData.system.push(systemobj);

    return simplisity_stringifyjson(viewData);
}
function simplisity_getpostjson(spost) {
    var viewData = {
        postdata: []
    };

    // put input fields into the json object
    if (typeof spost !== 'undefined' && spost !== '' && spost !== null) {
        var sposts = spost.split(',');
        sposts.forEach((post) => {
            var container = document.querySelector(post);
            if (container) {
                var elements = container.querySelectorAll('input, textarea, select');
                elements.forEach(function (element) {
                    if (element.getAttribute("s-update") !== 'ignore' && element.id !== '') {
                        var postvalue;
                        if (element.getAttribute("s-datatype") === 'coded') {
                            postvalue = element.value || '';
                            postvalue = simplisity_encode(postvalue);
                        }
                        else {
                            postvalue = element.value || '';
                        }

                        var jsonData = {};
                        jsonData['id'] = element.id || '';
                        jsonData['value'] = postvalue;
                        jsonData['s-post'] = post || '';
                        jsonData['s-update'] = element.getAttribute("s-update") || '';
                        jsonData['s-datatype'] = element.getAttribute("s-datatype") || '';
                        jsonData['s-xpath'] = element.getAttribute("s-xpath") || '';
                        jsonData['type'] = element.getAttribute("type") || 'select';
                        jsonData['checked'] = (element.type === 'checkbox' || element.type === 'radio') ? element.checked : '';
                        jsonData['name'] = element.getAttribute("name") || '';
                        viewData.postdata.push(jsonData);
                    }
                });
            }
        });
    }

     return simplisity_stringifyjson(viewData);
}
function simplisity_getlistjson(slist) {
    var viewData = {
        listdata: []
    };

    // create any lists required.
    if (typeof slist !== 'undefined' && slist !== '' && slist !== null) {
        var slists = slist.split(',');
        slists.forEach((list) => {
            var lp2 = 1;
            var listElements = document.querySelectorAll(list);
            listElements.forEach(function (listElement) {
                var elements = listElement.querySelectorAll('input, textarea, select');
                elements.forEach(function (element) {
                    if (element.getAttribute("s-update") !== 'ignore' && element.id !== '') {
                        var postvalue;
                        if (element.getAttribute("s-datatype") === 'coded') {
                            postvalue = element.value || '';
                            postvalue = simplisity_encode(postvalue);
                        }
                        else {
                            postvalue = element.value || '';
                        }

                        var jsonDataL = {};
                        jsonDataL['id'] = element.id || '';
                        jsonDataL['value'] = postvalue || '';
                        jsonDataL['row'] = lp2.toString() || '';
                        jsonDataL['listname'] = list || '';
                        jsonDataL['s-update'] = element.getAttribute("s-update") || '';
                        jsonDataL['s-datatype'] = element.getAttribute("s-datatype") || '';
                        jsonDataL['s-xpath'] = element.getAttribute("s-xpath") || '';
                        jsonDataL['type'] = element.getAttribute("type") || 'select';
                        jsonDataL['checked'] = (element.type === 'checkbox' || element.type === 'radio') ? element.checked : '';
                        jsonDataL['name'] = element.getAttribute("name") || '';
                        viewData.listdata.push(jsonDataL);
                    }
                });
                lp2 += 1;
            });
        });
    }
    return simplisity_stringifyjson(viewData);
}
function simplisity_createStaticPageFields(cmdurl, settings) {
    // inject static fields.
    var existingLoaders = document.querySelectorAll('.simplisity_loader');
    existingLoaders.forEach(loader => loader.remove());
    
    var existingSystemKey = document.querySelector('#simplisity_systemkey');
    if (existingSystemKey) existingSystemKey.remove();
    
    var existingCmdUrl = document.querySelector('#simplisity_cmdurl');
    if (existingCmdUrl) existingCmdUrl.remove();
    
    var existingFileUploadList = document.querySelector('#simplisity_fileuploadlist');
    if (existingFileUploadList) existingFileUploadList.remove();
    
    var existingFileUploadBase64 = document.querySelector('#simplisity_fileuploadbase64');
    if (existingFileUploadBase64) existingFileUploadBase64.remove();

    var elementstr = '<div class="' + settings.overlayclass + ' simplisity_loader ">';
    elementstr += '<div class="simplisity_loader_inner"></div>';
    elementstr += '</div>';
    elementstr += '<input id="simplisity_systemkey" type="hidden" value="' + settings.systemkey + '" />';
    elementstr += '<input id="simplisity_cmdurl" type="hidden" value="' + cmdurl + '" />';
    elementstr += '<input id="simplisity_fileuploadlist" type="hidden" value="" />';
    elementstr += '<input id="simplisity_fileuploadbase64" type="hidden" value="" />';

    var elem = document.createElement('span');
    elem.innerHTML = elementstr;
    document.body.appendChild(elem);
}

function simplisity_createTempPageFields() {
    // inject any temporary fields that simplisity needs.
    if (!document.getElementById('simplisity_params')) {
        var input = document.createElement('input');
        input.id = 'simplisity_params';
        input.type = 'hidden';
        input.value = '';
        document.body.appendChild(input);
    }
}
function simplisity_removepagefields() {
    // remove temporary fields that simplisity needs.
    var element = document.getElementById('simplisity_params');
    if (element) {
        element.remove();
    }
}
function simplisity_panelAjaxFunction(panelelement) {
    if ((typeof panelelement !== 'undefined') && panelelement !== '') {
        s_ajaxPostCmd.pop();

        var sreturn = '#' + panelelement.id;
        var activepanel = panelelement.getAttribute('s-activepanel');
        var cmdurlElement = document.getElementById('simplisity_cmdurl');
        var cmdurl = cmdurlElement ? cmdurlElement.value : '';

        if (activepanel) {
            // Note: activateSimplisityPanel is a jQuery plugin - will need separate conversion
            // For now, calling it directly on the element
            if (typeof simplisity_activateSimplisityPanel === 'function') {
                simplisity_activateSimplisityPanel(panelelement, cmdurl);
            }
        }

        simplisity_callserver(panelelement, cmdurl, sreturn);

        if (debugmode === true) {
            // DEBUG ++++++++++++++++++++++++++++++++++++++++++++
            console.log('[simplisity_panelAjaxFunction()] s-cmd: ', panelelement.getAttribute('s-cmd'));
        }
    }
}



//#endregion


//#region Utils --------------------------------------------------------------
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

function simplisity_removegriditem(item) {
    simplisity_remove(item, 'div');
}

function simplisity_remove(item, tagName) {
    var element = typeof item === 'string' ? document.querySelector(item) : item;
    var slist = element.getAttribute('s-removelist').replace('.', '');
    var sindex = element.getAttribute('s-index');
    
    var parent = element.closest(tagName + "[s-index='" + sindex + "']");
    if (parent) {
        parent.classList.remove(slist);
        parent.classList.add(slist + '_deleted');
        
        var recylebin = element.getAttribute('s-recylebin');
        var recyclebinEl = document.querySelector('#simplisity_recyclebin_' + recylebin);
        
        if (recyclebinEl) {
            recyclebinEl.appendChild(parent);
        } else {
            parent.remove();
        }
        
        var undoElements = document.querySelectorAll('.simplisity_itemundo[s-recylebin="' + recylebin + '"]');
        undoElements.forEach(function(el) {
            el.style.display = 'block';
        });
    }
}

function simplisity_undoremovelistitem(item) {
    var element = typeof item === 'string' ? document.querySelector(item) : item;
    var sreturn = element.getAttribute('s-return');
    var slist = element.getAttribute('s-removelist').replace('.', '');
    var sundoselector = '.' + slist + "_deleted";
    var recylebin = element.getAttribute('s-recylebin');
    
    var recyclebinEl = document.querySelector('#simplisity_recyclebin_' + recylebin);
    if (recyclebinEl) {
        var deletedItems = recyclebinEl.querySelectorAll(sundoselector);
        if (deletedItems.length > 0) {
            var lastItem = deletedItems[deletedItems.length - 1];
            lastItem.classList.remove(slist + "_deleted");
            lastItem.classList.add(slist);
            document.querySelector(sreturn).appendChild(lastItem);
        }
    }
    
    var remainingDeleted = recyclebinEl ? recyclebinEl.querySelectorAll(sundoselector) : [];
    if (remainingDeleted.length === 0) {
        var undoElements = document.querySelectorAll('.simplisity_itemundo[s-recylebin="' + recylebin + '"]');
        undoElements.forEach(function(el) {
            el.style.display = 'none';
        });
    }
}

function simplisity_emptyrecyclebin(recyclebin) {
    var recyclebinEl = document.querySelector('#simplisity_recyclebin_' + recyclebin);
    if (recyclebinEl) {
        recyclebinEl.remove();
    }
    var undoElements = document.querySelectorAll('.simplisity_itemundo[s-recylebin="' + recyclebin + '"]');
    undoElements.forEach(function(el) {
        el.style.display = 'none';
    });
}

function simplisity_getCookieValue(cookiename) {
    var b = document.cookie.match('(^|;)\\s*' + cookiename + '\\s*=\\s*([^;]+)');
    return b ? b.pop() : '';
}

function simplisity_setCookieValue(cookiename, cookievalue) {
    document.cookie = cookiename + "=" + cookievalue + ";path=/;max-age=604800";
}

function simplisity_deleteCookie(cookiename) {
    if (simplisity_getCookieValue(cookiename)) {
        document.cookie = cookiename + "=;path=/;expires=Thu, 01 Jan 1970 00:00:01 GMT";
    }
}

function simplisity_replaceAll(target, search, replacement) {
    return target.replace(new RegExp(search, 'g'), replacement);
}

function simplisity_setParamField(fieldkey, fieldvalue) {
    if (typeof fieldvalue !== 'undefined' && typeof fieldkey !== 'undefined' && fieldkey !== null && fieldkey !== 'null') {
        simplisity_createTempPageFields();
        var paramsEl = document.querySelector('#simplisity_params');
        var jsonParams = paramsEl ? paramsEl.value : '';
        var obj = {};
        if (typeof jsonParams !== 'undefined' && jsonParams !== '') {
            obj = simplisity_parsejson(jsonParams);
        }
        obj[fieldkey] = fieldvalue;
        if (paramsEl) {
            paramsEl.value = simplisity_stringifyjson(obj);
        }
        simplisity_setSessionField(fieldkey, fieldvalue);
    }
}

function simplisity_getParamField(fieldkey) {
    var paramsEl = document.querySelector('#simplisity_params');
    return simplisity_getField(paramsEl ? paramsEl.value : '', fieldkey);
}

function simplisity_getField(sfields, fieldkey) {
    if (typeof sfields !== 'undefined' && sfields !== '') {
        if (typeof fieldkey !== 'undefined' && fieldkey !== '') {
            var obj = simplisity_parsejson(sfields);
            return obj[fieldkey];
        }
    }
    return '';
}

function simplisity_getsystemkey(sfields) {
    var systemkey = simplisity_getField(sfields, 'systemkey');
    if (typeof systemkey === 'undefined' || systemkey === '') {
        var systemkeyEl = document.querySelector('#simplisity_systemkey');
        systemkey = systemkeyEl ? systemkeyEl.value : '';
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
    var link = document.createElement('link');
    link.rel = 'stylesheet';
    link.href = value;
    document.head.appendChild(link);
}

function simplisity_injectscript(value) {
    var script = document.createElement('script');
    script.src = value;
    document.head.appendChild(script);
}

function simplisity_systemkey() {
    var systemkeyEl = document.querySelector('#simplisity_systemkey');
    return systemkeyEl ? systemkeyEl.value : '';
}

function simplisity_sessionjson() {
    var rtn = simplisity_getCookieValue('simplisity_sessionparams');
    var out = '{"null":"null"}';
    if (rtn && typeof rtn !== 'undefined' && rtn !== '') {
        out = rtn;
    }
    try {
        var json = simplisity_stringifyjson(eval("(" + out + ")"));
        return json;
    }
    catch (err) {
        var json2 = simplisity_stringifyjson(eval("({'null':'null'})"));
        return json2;
    }
}

function simplisity_sessionremove() {
    simplisity_setCookieValue('simplisity_sessionparams', '');
}

function simplisity_sessionpost() {
    var p = simplisity_parsejson(simplisity_sessionjson());
    for (var key of Object.keys(p)) {
        simplisity_setParamField(key, p[key]);
    }

    var browser_sessionid = window.sessionStorage.getItem('browsersessionid');
    if (!browser_sessionid) {
        browser_sessionid = CreateUUID();
        window.sessionStorage.setItem('browsersessionid', browser_sessionid);
    }
    simplisity_setParamField('browsersessionid', browser_sessionid);
    simplisity_setSessionField('browsersessionid', browser_sessionid);

    var browser_id = window.localStorage.getItem('browserid');
    if (!browser_id) {
        browser_id = CreateUUID();
        window.localStorage.setItem('browserid', browser_id);
    }
    simplisity_setParamField('browserid', browser_id);
    simplisity_setSessionField('browserid', browser_id);

    var paramsEl = document.querySelector('#simplisity_params');
    return paramsEl ? paramsEl.value : '';
}

function CreateUUID() {
    var rtn = ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
    var d = new Date();
    rtn = rtn + "-" + d.getTime();
    return rtn;
}

function simplisity_setSessionField(fieldkey, fieldvalue) {
    if (typeof fieldvalue !== 'undefined' && fieldkey) {
        var jsonParams = simplisity_sessionjson();
        var obj = {};
        if (typeof jsonParams !== 'undefined' && jsonParams !== '') {
            obj = simplisity_parsejson(jsonParams);
        }
        obj[fieldkey] = fieldvalue;
        simplisity_setCookieValue('simplisity_sessionparams', simplisity_stringifyjson(obj));
    }
}

function simplisity_getSessionField(fieldkey) {
    var result = simplisity_parsejson(simplisity_sessionjson());
    return simplisity_getField(simplisity_stringifyjson(result), fieldkey);
}

function simplisity_requestjson(scmdurl, scmd, spost, id, sfields, safter) {
    simplisityPost(scmdurl, scmd, spost, id, '', false, 0, sfields, true, safter, '', false, 'json');
}

function simplisity_parsejson(json) {
    var retval;
    if (typeof (json) === "string") {
        retval = JSON.parse(json);
    } else {
        retval = json;
    }
    return retval;
}

function simplisity_stringifyjson(json) {
    var retval;
    if (typeof (json) === "string") {
        retval = json;
    } else {
        retval = JSON.stringify(json);
    }
    return retval;
}

async function simplisity_initFileUpload(fileuploadselector) {
    try {
        var element = document.querySelector(fileuploadselector);
        if (!element) return;

        var systemkey = simplisity_getsystemkey(element.getAttribute('s-fields'));
        if (systemkey === '' || typeof systemkey === 'undefined') {
            var systemkeyEl = document.querySelector('#simplisity_systemkey');
            systemkey = systemkeyEl ? systemkeyEl.value : '';
        }

        var rexpr = element.getAttribute('s-regexpr');
        if (!rexpr || rexpr === '') {
            rexpr = '/(\.|\/)(gif|jpe?g|jpg|png|pdf|zip|xml|json)/i';
        }
        
        var maxFileSize = parseInt(element.getAttribute('s-maxfilesize'));
        if (isNaN(maxFileSize) || maxFileSize === 0) {
            maxFileSize = 5000000000;
        }
        
        var maxChunkSize = parseInt(element.getAttribute('s-maxchunksize'));
        if (isNaN(maxChunkSize) || maxChunkSize === 0) {
            maxChunkSize = 10000000;
        }

        // Note: File upload functionality requires a third-party library like Dropzone.js
        // or custom implementation to replace jQuery File Upload plugin
        console.warn('File upload initialization requires additional implementation without jQuery File Upload plugin');
    }
    catch (e) {
        console.log('Error!', e);
    }
}

function simplisity_generateFileUniqueIdentifier(data) {
    var file = data.files[0];
    var result = file.relativePath || file.webkitRelativePath || file.fileName || file.name;
    return result;
}

function simplisity_assignevents(cmdurl) {
    // Change handlers - capture element in closure
    var changeElements = document.querySelectorAll('.simplisity_change');
    changeElements.forEach(function(elem, index) {
        elem.setAttribute("s-index", index);
        
        // Remove old handler if exists
        if (elem._changeHandler) {
            elem.removeEventListener("change", elem._changeHandler);
        }
        
        // Create new handler with captured element - IMPORTANT: Use regular function, not arrow
        elem._changeHandler = (function(capturedElem) {
            return function(event) {
                event.preventDefault();
                simplisity_setParamField("simplisity_change", new Date().toISOString());
                simplisity_callserver(capturedElem, cmdurl);
                return false;
            };
        })(elem);
        
        elem.addEventListener("change", elem._changeHandler);
    });

    // Click handlers
    var clickElements = document.querySelectorAll('.simplisity_click');
    clickElements.forEach(function(elem, index) {
        elem.setAttribute("s-index", index);
        
        if (elem._clickHandler) {
            elem.removeEventListener("click", elem._clickHandler);
        }
        
        elem._clickHandler = (function(capturedElem) {
            return function(event) {
                event.preventDefault();
                simplisity_setParamField("simplisity_click", new Date().toISOString());
                simplisity_callserver(capturedElem, cmdurl);
                
                var stateObj = capturedElem.getAttribute("s-fields");
                var href = capturedElem.getAttribute("href");
                if (href && href !== '' && href.includes(window.location.hostname)) {
                    history.pushState(stateObj, "Title", href);
                }
                return false;
            };
        })(elem);
        
        elem.addEventListener("click", elem._clickHandler);
    });

    // Confirm click handlers
    var confirmClickElements = document.querySelectorAll('.simplisity_confirmclick');
    confirmClickElements.forEach(function(elem, index) {
        elem.setAttribute("s-index", index);
        
        if (elem._confirmClickHandler) {
            elem.removeEventListener("click", elem._confirmClickHandler);
        }
        
        elem._confirmClickHandler = (function(capturedElem) {
            return function(event) {
                event.preventDefault();
                if (confirm(capturedElem.getAttribute("s-confirm"))) {
                    simplisity_setParamField("simplisity_confirmclick", new Date().toISOString());
                    simplisity_callserver(capturedElem, cmdurl);
                }
                return false;
            };
        })(elem);
        
        elem.addEventListener("click", elem._confirmClickHandler);
    });

    // Remove list item handlers
    var removeListItems = document.querySelectorAll('.simplisity_removelistitem');
    removeListItems.forEach(function(elem, index) {
        elem.setAttribute("s-index", index);
        var parent = elem.closest('li');
        if (parent) {
            parent.setAttribute("s-index", index);
        }
        
        if (elem._removeListItemHandler) {
            elem.removeEventListener("click", elem._removeListItemHandler);
        }
        
        elem._removeListItemHandler = (function(capturedElem) {
            return function(event) {
                event.preventDefault();
                simplisity_removelistitem(capturedElem);
            };
        })(elem);
        
        elem.addEventListener("click", elem._removeListItemHandler);
    });

    // Remove grid item handlers
    var removeGridItems = document.querySelectorAll('.simplisity_removegriditem');
    removeGridItems.forEach(function(elem, index) {
        elem.setAttribute("s-index", index);
        var parent = elem.closest('div');
        if (parent) {
            parent.setAttribute("s-index", index);
        }
        
        if (elem._removeGridItemHandler) {
            elem.removeEventListener("click", elem._removeGridItemHandler);
        }
        
        elem._removeGridItemHandler = (function(capturedElem) {
            return function(event) {
                event.preventDefault();
                simplisity_removegriditem(capturedElem);
            };
        })(elem);
        
        elem.addEventListener("click", elem._removeGridItemHandler);
    });

    // Remove table row handlers
    var removeTableRows = document.querySelectorAll('.simplisity_removetablerow');
    removeTableRows.forEach(function(elem, index) {
        elem.setAttribute("s-index", index);
        var parent = elem.closest('tr');
        if (parent) {
            parent.setAttribute("s-index", index);
        }
        
        if (elem._removeTableRowHandler) {
            elem.removeEventListener("click", elem._removeTableRowHandler);
        }
        
        elem._removeTableRowHandler = (function(capturedElem) {
            return function(event) {
                event.preventDefault();
                simplisity_removetablerow(capturedElem);
            };
        })(elem);
        
        elem.addEventListener("click", elem._removeTableRowHandler);
    });

    // Item undo handlers
    var itemUndos = document.querySelectorAll('.simplisity_itemundo');
    itemUndos.forEach(function(elem, index) {
        var recylebin = elem.getAttribute("s-recylebin");
        if (recylebin) {
            if (!document.querySelector('#simplisity_recyclebin_' + recylebin)) {
                var div = document.createElement('div');
                div.id = 'simplisity_recyclebin_' + recylebin;
                div.style.display = 'none';
                document.body.appendChild(div);
            }
        }
        
        if (elem._itemUndoHandler) {
            elem.removeEventListener("click", elem._itemUndoHandler);
        }
        
        elem._itemUndoHandler = (function(capturedElem) {
            return function(event) {
                event.preventDefault();
                simplisity_undoremovelistitem(capturedElem);
            };
        })(elem);
        
        elem.addEventListener("click", elem._itemUndoHandler);
    });

    // File upload handlers
    var fileUploads = document.querySelectorAll('.simplisity_fileupload');
    fileUploads.forEach(function(elem, index) {
        simplisity_initFileUpload('#' + elem.getAttribute('id'));
    });

    // Base64 upload handlers
    var base64Uploads = document.querySelectorAll('.simplisity_base64upload');
    base64Uploads.forEach(function(elem, index) {
        elem.setAttribute("s-index", index);
        
        if (elem._base64UploadHandler) {
            elem.removeEventListener("change", elem._base64UploadHandler);
        }
        
        elem._base64UploadHandler = (function(capturedElem) {
            return function(event) {
                simplisity_base64wait(capturedElem, cmdurl);
                return false;
            };
        })(elem);
        
        elem.addEventListener("change", elem._base64UploadHandler);
    });

    // File download handlers
    var fileDownloads = document.querySelectorAll('.simplisity_filedownload');
    fileDownloads.forEach(function(elem, index) {
        var params = "cmd=" + (elem.getAttribute('s-cmd') || '');
        var sfields = elem.getAttribute('s-fields');

        if (sfields) {
            var obj = simplisity_parsejson(sfields);
            for (var x in obj) {
                if (obj.hasOwnProperty(x)) {
                    params = params + '&' + x + '=' + simplisity_encode(simplisity_getField(sfields, x));
                }
            }

            var systemkey = simplisity_getsystemkey(sfields);
            params = params + '&systemkey=' + systemkey;
        }
        var cmdurlEl = document.querySelector("#simplisity_cmdurl");
        var downloadCmdurl = cmdurlEl ? cmdurlEl.value : '';
        elem.setAttribute('href', downloadCmdurl + '?' + params);
    });
}
//#endregion


//#region Components -----------------------------------------------------------
function simplisity_SimpleSearch(id) {
    var searchEl = document.getElementById(id);
    if (!searchEl) return;
    
    var formEl = searchEl.querySelector('form');
    if (formEl) {
        formEl.removeAttribute("onsubmit");
        formEl.addEventListener("submit", function(event) {
            event.preventDefault();
            var query = this.querySelector('input[type="text"], select').value;
            
            if (query) {
                // Set session field to persist search query
                simplisity_setSessionField('simplisity_searchquery', query);
                
                // Optionally, navigate to a new URL
                // var newUrl = this.getAttribute('action') + '?search=' + encodeURIComponent(query);
                // window.location.href = newUrl;
                
                // For now, just log the query
                console.log('Search query:', query);
            }
            return false;
        });
    }
}

function simplisity_SimplePager(id) {
    var pagerEl = document.getElementById(id);
    if (!pagerEl) return;
    
    var currentPage = 1;
    var pageSize = 10; // Default page size
    var totalItems = parseInt(pagerEl.getAttribute('data-totalitems')) || 0;
    var pageCount = Math.ceil(totalItems / pageSize);
    
    function updatePager() {
        var startItem = (currentPage - 1) * pageSize + 1;
        var endItem = Math.min(currentPage * pageSize, totalItems);
        
        // Update displayed item range
        var rangeEl = pagerEl.querySelector('.item-range');
        if (rangeEl) {
            rangeEl.textContent = 'Showing ' + startItem + ' to ' + endItem + ' of ' + totalItems;
        }
        
        // Update page links
        var pageLinks = pagerEl.querySelectorAll('.page-link');
        pageLinks.forEach(function(link) {
            var pageNum = parseInt(link.textContent);
            link.classList.remove('active');
            
            if (pageNum === currentPage) {
                link.classList.add('active');
            }
        });
        
        // Enable/disable Prev/Next buttons
        var prevBtn = pagerEl.querySelector('.btn-prev');
        var nextBtn = pagerEl.querySelector('.btn-next');
        if (prevBtn) {
            prevBtn.disabled = (currentPage === 1);
        }
        if (nextBtn) {
            nextBtn.disabled = (currentPage === pageCount);
        }
    }
    
    // Page link click handler
    pagerEl.addEventListener('click', function(event) {
        var target = event.target;
        
        if (target.classList.contains('page-link')) {
            var pageNum = parseInt(target.textContent);
            currentPage = pageNum;
            updatePager();
            
            // Optionally, trigger a data refresh here
            // loadData(currentPage, pageSize);
        }
        else if (target.classList.contains('btn-prev')) {
            if (currentPage > 1) {
                currentPage--;
                updatePager();
                
                // Optionally, trigger a data refresh here
                // loadData(currentPage, pageSize);
            }
        }
        else if (target.classList.contains('btn-next')) {
            if (currentPage < pageCount) {
                currentPage++;
                updatePager();
                
                // Optionally, trigger a data refresh here
                // loadData(currentPage, pageSize);
            }
        }
        
        return false;
    });
    
    updatePager();
}
//#endregion

if (typeof jQuery !== 'undefined') {
    (function ($) {
        $.fn.simplisityStartUp = function (cmdurl, options) {
            simplisityStartUp(cmdurl, options);
            return this; // Enable chaining
        };
    })(jQuery);
}
