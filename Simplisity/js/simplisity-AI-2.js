var ajaxPostCmd = [];
var debugmode = false;

// Helper function to convert jQuery objects to DOM elements
function simplisity_toDomElement(element) {
    if (!element) return element;
    
    // Check if it's a jQuery object
    if (element.jquery || (element.length !== undefined && element[0] && element[0].nodeType)) {
        if (debugmode === true) {
console.warn('[simplisity_toDomElement] Converting jQuery object to DOM element');
        }
        return element.get ? element.get(0) : element[0];
    }
    
    if (element.nodeType) {
        return element;
    }
    
  if (typeof element === 'string') {
        return document.querySelector(element);
    }
    
    return element;
}

HTMLElement.prototype.getSimplisity = function (cmdurl, scmd, sfields, safter, spost) {
    if (debugmode === true) {
console.log('[getSimplisity] ', cmdurl, scmd, '#' + this.getAttribute('id'), sfields, spost);
    }
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

HTMLElement.prototype.simplisityStartUp = function (cmdurl, options) {
    const settings = Object.assign({
        activatepanel: true,
        overlayclass: '',
    debug: false,
     hideprocess: true
    }, options);

debugmode = settings.debug;

    simplisity_createStaticPageFields(cmdurl, settings);
    simplisity_createTempPageFields();

    document.querySelectorAll('.simplisity_panel').forEach(function (panel) {
      panel.setAttribute('s-activepanel', settings.activatepanel);
        ajaxPostCmd.push(panel);
 });

    simplisity_panelAjaxFunction(ajaxPostCmd[ajaxPostCmd.length - 1]);

 if (settings.hideprocess) {
        document.querySelectorAll('.simplisity_loader').forEach(el => el.style.display = 'none');
    }
};

function simplisity_createStaticPageFields(cmdurl, settings) {
    document.querySelectorAll('.simplisity_loader').forEach(el => el.remove());
    const fieldsToRemove = ['simplisity_systemkey', 'simplisity_cmdurl', 'simplisity_fileuploadlist', 'simplisity_fileuploadbase64'];
    fieldsToRemove.forEach(id => {
        const el = document.getElementById(id);
        if (el) el.remove();
    });

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
    if (!document.getElementById('simplisity_params')) {
        var elementstr = '<input id="simplisity_params" type="hidden" value="" />';
var elem = document.createElement('span');
        elem.innerHTML = elementstr;
  document.body.appendChild(elem);
    }
}

function simplisity_removepagefields() {
    const el = document.getElementById('simplisity_params');
    if (el) el.remove();
}

function simplisity_panelAjaxFunction(panelelement) {
    if ((typeof panelelement !== 'undefined') && panelelement !== '') {
        ajaxPostCmd.pop();

        var sreturn = '#' + panelelement.getAttribute('id');
        var activepanel = panelelement.getAttribute('s-activepanel');
        var cmdurl = document.getElementById('simplisity_cmdurl')?.value;

        if (activepanel) {
            panelelement.activateSimplisityPanel(cmdurl);
        }

        simplisity_callserver(panelelement, cmdurl, sreturn);

        if (debugmode === true) {
        console.log('[simplisity_panelAjaxFunction()] s-cmd: ', panelelement.getAttribute('s-cmd'));
 }
    }
}

document.addEventListener("simplisitypostgetcompleted", simplisity_nbxgetCompleted);

function simplisity_nbxgetCompleted(e) {
  if ((typeof e.detail.safter !== 'undefined') && e.detail.safter !== '') {
    var funclist = e.detail.safter.split(',');
        for (var i = 0; i < funclist.length; i++) {
  if (typeof (window[funclist[i]]) === "function" || typeof (window[funclist[i]]) === "object") {
       window[funclist[i]]();
 } else {
                console.log('ERROR: function does not exist. ' + funclist[i]);
      }
        }
    }

  simplisity_sessionfieldaction();

    document.querySelectorAll('input[id*="simplisity_fileuploadlist"]').forEach(el => el.value = '');
    document.querySelectorAll('input[id*="simplisity_fileuploadbase64"]').forEach(el => el.value = '');

    if (debugmode === true) {
        console.log('-------END AJAX CALL------- ');
    }

    simplisity_panelAjaxFunction(ajaxPostCmd[ajaxPostCmd.length - 1]);

    document.querySelectorAll('.simplisity_fadeout').forEach(el => {
  el.style.transition = 'opacity 2s';
        el.style.opacity = '0';
        setTimeout(() => el.style.display = 'none', 2000);
    });

    if (e.detail.sloader === true) {
        document.querySelectorAll('.simplisity_loader').forEach(el => el.style.display = 'none');
    }
}

function simplisityPost(scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, sdropdownlist, reload, sreturntype, paramfields) {
    if (debugmode === true) {
console.log('[simplisityPost()] scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, sdropdownlist:---> ', scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, sdropdownlist);
    }

    if (typeof scmd !== 'undefined' && scmd !== '') {
        if (paramfields == '') {
      paramfields = simplisity_sessionpost();
        }

  var systemkey = simplisity_getsystemkey(sfields);

    if (typeof reload === 'undefined' || reload === '') {
            reload = 'false';
        }

      var cmdupdate = '';
        if (scmdurl.includes("?")) {
            cmdupdate = scmdurl;
   } else {
     cmdupdate = scmdurl + '?cmd=' + scmd + '&systemkey=' + systemkey;
        }

        var jsonData = simplisity_ConvertFormToJSON(spost, slist, sfields, paramfields);
        var jsonParam = simplisity_ConvertParamToJSON(sfields, paramfields);

        if ((typeof sdropdownlist !== 'undefined') && sdropdownlist !== '') {
            if (debugmode === true) {
  console.log('------- START AJAX CALL [dropdown] ------- ' + scmd);
       }

      fetch(cmdupdate, {
     method: 'POST',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
       body: new URLSearchParams({
  inputjson: encodeURIComponent(jsonData),
        paramjson: encodeURIComponent(jsonParam),
     simplisity_cmd: scmd
   })
   .then(response => {
   if (!response.ok) {
  throw new Error('HTTP error! status: ' + response.status);
   }
       const contentType = response.headers.get("content-type");
if (!contentType || !contentType.includes("application/json")) {
       return response.text().then(text => {
     console.error('Expected JSON but received:', text.substring(0, 500));
         throw new TypeError("Server didn't return JSON");
       });
     }
 return response.json();
})
   .then(json => {
         const dropdown = document.querySelector(sdropdownlist);
        if (dropdown) {
        dropdown.innerHTML = '';
           var jsonObj = simplisity_parsejson(json);
    for (var i = 0; i < jsonObj.length; i++) {
   var obj = jsonObj[i];
      const option = document.createElement('option');
     option.value = obj.key;
 option.textContent = obj.value;
            dropdown.appendChild(option);
      }
    }
          if ((typeof safter !== 'undefined') && safter !== '') {
         var funclist = safter.split(',');
   for (var i = 0; i < funclist.length; i++) {
window[funclist[i]]();
  }
      }
    if (shideloader === true) {
          document.querySelectorAll('.simplisity_loader').forEach(el => el.style.display = 'none');
   }
    })
 .catch(error => {
   console.error('Error:', error);
     console.error('Failed request URL:', cmdupdate);
     console.error('Command:', scmd);
        document.querySelectorAll('.simplisity_loader').forEach(el => el.style.display = 'none');
  });
 } else {
 if (debugmode === true) {
    console.log('------- START AJAX CALL------- ' + scmd);
            }

       if (sreturntype === 'json') {
        fetch(cmdupdate, {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
  body: new URLSearchParams({
    inputjson: encodeURIComponent(jsonData),
    paramjson: encodeURIComponent(jsonParam),
   simplisity_cmd: scmd
  })
          })
     .then(response => {
     if (!response.ok) {
         throw new Error('HTTP error! status: ' + response.status);
 }
  const contentType = response.headers.get("content-type");
    if (!contentType || !contentType.includes("application/json")) {
     return response.text().then(text => {
     console.error('Expected JSON but received:', text.substring(0, 500));
   throw new TypeError("Server didn't return JSON");
           });
}
      return response.json();
     })
         .then(json => {
window.sessionStorage.setItem(sreturn, JSON.stringify(json));

    if ((typeof safter !== 'undefined') && safter !== '') {
   var funclist = safter.split(',');
            for (var i = 0; i < funclist.length; i++) {
      window[funclist[i]]();
    }
     }
      if (shideloader === true) {
      document.querySelectorAll('.simplisity_loader').forEach(el => el.style.display = 'none');
  }
      })
      .catch(error => {
          console.error('Error:', error);
         console.error('Failed request URL:', cmdupdate);
   console.error('Command:', scmd);
       document.querySelectorAll('.simplisity_loader').forEach(el => el.style.display = 'none');
   });
            } else {
                fetch(cmdupdate, {
   method: 'POST',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
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
 document.open();
  document.write(data);
   document.close();
     } else {
          const returnEl = document.querySelector(sreturn);
       if (returnEl) {
   if ((typeof sappend === 'undefined') || sappend === '' || sappend === false) {
            returnEl.innerHTML = data;
 returnEl.dispatchEvent(new Event('change', { bubbles: true }));
       } else {
      returnEl.insertAdjacentHTML('beforeend', data);
        returnEl.dispatchEvent(new Event('change', { bubbles: true }));
               }
  }
      }
     }

  if (reload === 'true') {
                 location.reload();
             } else {
              const event = new CustomEvent("simplisitypostgetcompleted", {
        detail: {
          cmd: scmd,
    sindex: sindex,
                sloader: shideloader,
sreturn: sreturn,
       safter: safter
      }
   });
       document.dispatchEvent(event);
    }

  const paramsEl = document.getElementById('simplisity_params');
   if (paramsEl) paramsEl.value = '';
          }
             })
                .catch(error => {
         console.error('Error:', error);
          console.error('Failed request URL:', cmdupdate);
 console.error('Command:', scmd);
                  document.querySelectorAll('.simplisity_loader').forEach(el => el.style.display = 'none');
    });
       }
    }
}

async function simplisity_callBeforeFunction(element) {
    try {
    element = simplisity_toDomElement(element);
        
   if (!element || !element.nodeType) {
     return;
        }

        const beforeAttr = element.getAttribute('s-before');
     if ((typeof beforeAttr !== 'undefined') && beforeAttr !== '' && beforeAttr !== null) {
            var funclist = beforeAttr.split(',');
            for (var i = 0; i < funclist.length; i++) {
          if (typeof (window[funclist[i]]) === "function" || typeof (window[funclist[i]]) === "object") {
           window[funclist[i]]();
       } else {
     console.log('ERROR: function does not exist. ' + funclist[i]);
  }
            }
        }
    } catch (e) {
        console.log('Error!', e);
    }
    return;
}

function simplisity_callSessionFields(element) {
    element = simplisity_toDomElement(element);
 
    if (!element || !element.nodeType) {
  return;
    }

  const sessionFieldAttr = element.getAttribute('s-sessionfield');
    if ((typeof sessionFieldAttr !== 'undefined') && sessionFieldAttr !== '' && sessionFieldAttr !== null) {
        var funclist = sessionFieldAttr.split(',');
  for (var i = 0; i < funclist.length; i++) {
  const fieldSelector = funclist[i].replace('#', '');
            const fieldEl = document.getElementById(fieldSelector);
 if (fieldEl) {
      simplisity_setSessionField(fieldSelector, fieldEl.value);
       }
        }
    }
    return;
}

async function simplisity_callserver(element, cmdurl, returncontainer, reload) {
    try {
     element = simplisity_toDomElement(element);
        
        if (!element || !element.nodeType) {
      console.error('[simplisity_callserver] Invalid element passed:', element);
       return;
        }

     const elementValue = element.value || '';
  simplisity_setParamField('activevalue', elementValue);

   simplisity_setParamField("simplisity_language", simplisity_getCookieValue("simplisity_language"));
  simplisity_setParamField("simplisity_editlanguage", simplisity_getCookieValue("simplisity_editlanguage"));

        var scmd = element.getAttribute("s-cmd");
        if (typeof scmd !== 'undefined' && scmd !== '' && scmd !== null) {
         var sshowloader = element.getAttribute("s-showloader");
  if (sshowloader !== 'false') {
       document.querySelectorAll('.simplisity_loader').forEach(el => el.style.display = 'block');
            }

            await simplisity_callBeforeFunction(element);

    if (element.getAttribute("s-stop") !== 'stop') {
                var scmdurl = element.getAttribute("s-cmdurl");
         if (typeof scmdurl === 'undefined' || scmdurl === '' || scmdurl === null) {
          scmdurl = cmdurl;
                }
    if (typeof scmdurl === 'undefined' || scmdurl === '' || scmdurl === null) {
   const cmdUrlEl = document.getElementById('simplisity_cmdurl');
        scmdurl = cmdUrlEl ? cmdUrlEl.value : '';
     }

       var sreturn = element.getAttribute("s-return");
     if (typeof sreturn === 'undefined' || sreturn === '' || sreturn === null) {
         sreturn = returncontainer;
         if (typeof sreturn === 'undefined' || sreturn === '') {
              sreturn = '#simplisity_startpanel';
        }
    }

        const reloadAttr = element.getAttribute("s-reload");
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

         if (typeof scmd === 'undefined') {
      scmd = '';
           }

if (typeof sfields === 'undefined') {
    sfields = '';
  }

        if (typeof shideloader === 'undefined') {
          shideloader = true;
}

    const fileUploadListEl = document.querySelector('input[id*="simplisity_fileuploadlist"]');
      if (fileUploadListEl && fileUploadListEl.value !== '') {
         if (typeof sfields === 'undefined' || sfields === '') {
       sfields = '{"fileuploadlist":"' + fileUploadListEl.value + '"}';
      } else {
              sfields = sfields.substring(0, sfields.length - 1) + ',"fileuploadlist":"' + fileUploadListEl.value + '"}';
                 }
    }

       const fileUploadBase64El = document.querySelector('input[id*="simplisity_fileuploadbase64"]');
   if (fileUploadBase64El && fileUploadBase64El.value !== '') {
        if (typeof sfields === 'undefined' || sfields === '') {
             sfields = '{"fileuploadbase64":"' + fileUploadBase64El.value + '"}';
   } else {
         sfields = sfields.substring(0, sfields.length - 1) + ',"fileuploadbase64":"' + fileUploadBase64El.value + '"}';
}
    }

simplisity_setSessionField("simplisity_return", sreturn);
          await simplisity_callSessionFields(element);

          simplisityPost(scmdurl, scmd, spost, sreturn, slist, sappend, sindex, sfields, shideloader, safter, sdropdownlist, reload, sreturntype, '');
   } else {
      element.setAttribute('s-stop', '');
      document.querySelectorAll('.simplisity_loader').forEach(el => el.style.display = 'none');
          }
        }
    } catch (e) {
        console.log('Error!', e);
        document.querySelectorAll('.simplisity_loader').forEach(el => el.style.display = 'none');
    }
    return;
}

function simplisity_ConvertParamToJSON(sfields, paramfields) {
  var viewData = {
        sfield: [],
      system: []
 };

    var jsonDataF = {};
    if (typeof sfields !== 'undefined' && sfields !== '') {
        var obj = simplisity_parsejson(sfields);
    jsonDataF = simplisity_mergeJson({}, jsonDataF, obj);
    }

    if (typeof paramfields !== 'undefined' && paramfields !== '') {
 var obj2 = simplisity_parsejson(paramfields);
   jsonDataF = simplisity_mergeJson({}, jsonDataF, obj2);
    }

    viewData.sfield.push(jsonDataF);

    var system = '{"systemkey":"' + simplisity_getsystemkey(sfields) + '","requesturl":"' + window.location.href + '"}';
    var systemobj = simplisity_parsejson(system);
    viewData.system.push(systemobj);

    if (debugmode === true) {
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

    if (spost && typeof spost !== 'undefined' && spost !== null && spost !== '') {
        var sposts = spost.split(',');
        sposts.forEach((post) => {
          const container = document.querySelector(post);
       if (container) {
      container.querySelectorAll('input, textarea, select').forEach(function (input) {
             var htmlType = input.getAttribute("type");
  if (htmlType !== '' && htmlType !== 'checkbox' && htmlType !== 'text' && htmlType !== 'radio' && htmlType !== 'select' && htmlType !== 'hidden') {
       htmlType = 'hidden';
  }

         if (input.getAttribute("s-update") !== 'ignore' && input.id !== '') {
    var postvalue = input.value || '';
         if (input.getAttribute("s-datatype") === 'coded') {
  postvalue = simplisity_encode(postvalue);
            }

    var jsonData = {};
           jsonData['id'] = input.id || '';
 jsonData['value'] = postvalue;
       jsonData['s-post'] = post || '';
         jsonData['s-update'] = input.getAttribute("s-update") || '';
  jsonData['s-datatype'] = input.getAttribute("s-datatype") || '';
            jsonData['s-xpath'] = input.getAttribute("s-xpath") || '';
    jsonData['type'] = htmlType || 'select';
 jsonData['checked'] = input.checked || '';
       jsonData['name'] = input.getAttribute("name") || '';
      viewData.postdata.push(jsonData);
        }
   });
        }
        });
    }

    if (slist && typeof slist !== 'undefined' && slist !== null && slist !== '') {
        var slists = slist.split(',');
        slists.forEach((list) => {
 var lp2 = 1;
 const listElements = document.querySelectorAll(list);
        listElements.forEach(function (listEl) {
     listEl.querySelectorAll('input, textarea, select').forEach(function (input) {
  var htmlType = input.getAttribute("type");
   if (htmlType !== '' && htmlType !== 'checkbox' && htmlType !== 'text' && htmlType !== 'radio' && htmlType !== 'select' && htmlType !== 'hidden') {
   htmlType = 'hidden';
  }

 if (input.getAttribute("s-update") !== 'ignore' && input.id !== '') {
       var postvalue = input.value || '';
     if (input.getAttribute("s-datatype") === 'coded') {
     postvalue = simplisity_encode(postvalue);
    }

      var jsonDataL = {};
      jsonDataL['id'] = input.id || '';
      jsonDataL['value'] = postvalue || '';
      jsonDataL['row'] = lp2.toString() || '';
      jsonDataL['listname'] = list || '';
      jsonDataL['s-update'] = input.getAttribute("s-update") || '';
      jsonDataL['s-datatype'] = input.getAttribute("s-datatype") || '';
      jsonDataL['s-xpath'] = input.getAttribute("s-xpath") || '';
      jsonDataL['type'] = htmlType || 'select';
      jsonDataL['checked'] = input.checked || '';
   jsonDataL['name'] = input.getAttribute("name") || '';
      viewData.listdata.push(jsonDataL);
    }
        });
       lp2 += 1;
  });
  });
    }

    var jsonDataF = {};
    if (typeof sfields !== 'undefined' && sfields !== null && sfields !== '') {
   var obj = simplisity_parsejson(sfields);
   jsonDataF = simplisity_mergeJson({}, jsonDataF, obj);
    }

    if (typeof paramfields !== 'undefined' && paramfields !== null && paramfields !== '') {
  var obj2 = simplisity_parsejson(paramfields);
        jsonDataF = simplisity_mergeJson({}, jsonDataF, obj2);
    }

    viewData.sfield.push(jsonDataF);

    var system = '{"systemkey":"' + simplisity_getsystemkey(sfields) + '"}';
    var systemobj = simplisity_parsejson(system);
    viewData.system.push(systemobj);

    if (debugmode === true) {
     console.log('json: ' + simplisity_stringifyjson(viewData));
    }

    return simplisity_stringifyjson(viewData);
}

function simplisity_getpostjson(spost) {
    var viewData = {
    postdata: []
    };

    if (spost && typeof spost !== 'undefined' && spost !== null && spost !== '') {
        var sposts = spost.split(',');
 sposts.forEach((post) => {
            const container = document.querySelector(post);
            if (container) {
             container.querySelectorAll('input, textarea, select').forEach(function (input) {
           if (input.getAttribute("s-update") !== 'ignore' && input.id !== '') {
  var postvalue = input.value || '';
          if (input.getAttribute("s-datatype") === 'coded') {
            postvalue = simplisity_encode(postvalue);
  }

var jsonData = {};
            jsonData['id'] = input.id || '';
       jsonData['value'] = postvalue;
  jsonData['s-post'] = post || '';
           jsonData['s-update'] = input.getAttribute("s-update") || '';
         jsonData['s-datatype'] = input.getAttribute("s-datatype") || '';
       jsonData['s-xpath'] = input.getAttribute("s-xpath") || '';
             jsonData['type'] = input.getAttribute("type") || 'select';
            jsonData['checked'] = input.checked || '';
        jsonData['name'] = input.getAttribute("name") || '';
      viewData.postdata.push(jsonData);
         }
    });
      }
        });
    }

    if (debugmode === true) {
   console.log('json: ' + simplisity_stringifyjson(viewData));
    }

    return simplisity_stringifyjson(viewData);
}

function simplisity_getlistjson(slist) {
    var viewData = {
 listdata: []
    };

    if (slist && typeof slist !== 'undefined' && slist !== null && slist !== '') {
  var slists = slist.split(',');
        slists.forEach((list) => {
            var lp2 = 1;
            const listElements = document.querySelectorAll(list);
 listElements.forEach(function (listEl) {
    listEl.querySelectorAll('input, textarea, select').forEach(function (input) {
  if (input.getAttribute("s-update") !== 'ignore' && input.id !== '') {
                var postvalue = input.value || '';
  if (input.getAttribute("s-datatype") === 'coded') {
        postvalue = simplisity_encode(postvalue);
           }

                var jsonDataL = {};
 jsonDataL['id'] = input.id || '';
       jsonDataL['value'] = postvalue || '';
 jsonDataL['row'] = lp2.toString() || '';
     jsonDataL['listname'] = list || '';
   jsonDataL['s-update'] = input.getAttribute("s-update") || '';
       jsonDataL['s-datatype'] = input.getAttribute("s-datatype") || '';
         jsonDataL['s-xpath'] = input.getAttribute("s-xpath") || '';
               jsonDataL['type'] = input.getAttribute("type") || 'select';
         jsonDataL['checked'] = input.checked || '';
           jsonDataL['name'] = input.getAttribute("name") || '';
            viewData.listdata.push(jsonDataL);
                  }
        });
     lp2 += 1;
            });
        });
    }

    if (debugmode === true) {
        console.log('json: ' + simplisity_stringifyjson(viewData));
    }

    return simplisity_stringifyjson(viewData);
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

function simplisity_removegriditem(item) {
    simplisity_remove(item, 'div');
}

function simplisity_remove(item, tagName) {
    item = simplisity_toDomElement(item);
    
    if (!item || !item.nodeType) {
        console.error('[simplisity_remove] Invalid item:', item);
   return;
    }

    const slist = item.getAttribute('s-removelist').replace('.', '');
    const sindex = item.getAttribute('s-index');
    
    let parent = item;
 while (parent && parent.tagName.toLowerCase() !== tagName.toLowerCase()) {
        parent = parent.parentElement;
    }
    
    if (parent && parent.getAttribute('s-index') === sindex) {
        parent.classList.remove(slist);
        parent.classList.add(slist + '_deleted');
        
      const recylebin = item.getAttribute('s-recylebin');
 const recycleBinEl = document.getElementById('simplisity_recyclebin_' + recylebin);
        
        if (recycleBinEl) {
     recycleBinEl.appendChild(parent);
    } else {
   parent.remove();
        }

        const undoElements = document.querySelectorAll('.simplisity_itemundo[s-recylebin="' + recylebin + '"]');
   undoElements.forEach(el => el.style.display = 'block');
    }
}

function simplisity_undoremovelistitem(item) {
    item = simplisity_toDomElement(item);
    
    if (!item || !item.nodeType) {
        console.error('[simplisity_undoremovelistitem] Invalid item:', item);
        return;
    }

    const sreturn = item.getAttribute('s-return');
    const sundoselector = item.getAttribute('s-removelist') + "_deleted";
    const slist = item.getAttribute('s-removelist').replace('.', '');
    const recylebin = item.getAttribute('s-recylebin');
    const recycleBinEl = document.getElementById('simplisity_recyclebin_' + recylebin);

    if (recycleBinEl) {
        const returnEl = document.querySelector(sreturn);
     const deletedItems = recycleBinEl.querySelectorAll(sundoselector);
 const lastItem = deletedItems[deletedItems.length - 1];
        
        if (lastItem && returnEl) {
    lastItem.classList.remove(slist + "_deleted");
 lastItem.classList.add(slist);
            returnEl.appendChild(lastItem);
     }
 }
    
    if (recycleBinEl && recycleBinEl.querySelectorAll(sundoselector).length === 0) {
        const undoElements = document.querySelectorAll('.simplisity_itemundo[s-recylebin="' + recylebin + '"]');
        undoElements.forEach(el => el.style.display = 'none');
    }
}

function simplisity_emptyrecyclebin(recyclebin) {
    const recycleBinEl = document.getElementById('simplisity_recyclebin_' + recyclebin);
    if (recycleBinEl) recycleBinEl.remove();
    
    const undoElements = document.querySelectorAll('.simplisity_itemundo[s-recylebin="' + recyclebin + '"]');
    undoElements.forEach(el => el.style.display = 'none');
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
        const paramsEl = document.getElementById('simplisity_params');
     var jsonParams = paramsEl ? paramsEl.value : '';
   var obj = {};
        if (typeof jsonParams !== 'undefined' && jsonParams !== '') {
            obj = simplisity_parsejson(jsonParams);
  }
        obj[fieldkey] = fieldvalue;
        if (paramsEl) paramsEl.value = simplisity_stringifyjson(obj);
      simplisity_setSessionField(fieldkey, fieldvalue);
    }
}

function simplisity_getParamField(fieldkey) {
    const paramsEl = document.getElementById('simplisity_params');
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
        const systemKeyEl = document.getElementById('simplisity_systemkey');
  systemkey = systemKeyEl ? systemKeyEl.value : '';
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
 const systemKeyEl = document.getElementById('simplisity_systemkey');
    return systemKeyEl ? systemKeyEl.value : '';
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
    } catch (err) {
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

    const paramsEl = document.getElementById('simplisity_params');
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
     const fileUploadEl = document.querySelector(fileuploadselector);
     if (!fileUploadEl) return;

        var systemkey = simplisity_getsystemkey(fileUploadEl.getAttribute('s-fields'));
        if (systemkey === '' || typeof systemkey === 'undefined') {
            const systemKeyEl = document.getElementById('simplisity_systemkey');
            systemkey = systemKeyEl ? systemKeyEl.value : '';
        }

        var rexpr = fileUploadEl.getAttribute('s-regexpr');
        if (rexpr === '') {
      rexpr = '/(\.|\/)(gif|jpe?g|jpg|png|pdf|zip|xml|json)$/i';
        }
        var maxFileSize = parseInt(fileUploadEl.getAttribute('s-maxfilesize'));
        if (isNaN(maxFileSize) || maxFileSize === '') {
        maxFileSize = 5000000000;
        }
        var maxChunkSize = parseInt(fileUploadEl.getAttribute('s-maxchunksize'));
 if (isNaN(maxChunkSize) || maxChunkSize === '') {
     maxChunkSize = 10000000;
    }

        console.warn('File upload plugin conversion requires custom implementation or alternative library');
    } catch (e) {
        console.log('Error!', e);
    }
}

function simplisity_generateFileUniqueIdentifier(data) {
    var file = data.files[0];
    result = file.relativePath || file.webkitRelativePath || file.fileName || file.name;
    return result;
}

function simplisity_assignevents(cmdurl) {
    document.querySelectorAll('.simplisity_change').forEach(function (el, index) {
        el.setAttribute("s-index", index);
        
        const newEl = el.cloneNode(true);
        el.parentNode.replaceChild(newEl, el);
      
   newEl.addEventListener("change", function () {
simplisity_setParamField("simplisity_change", new Date().toISOString());
       simplisity_callserver(this, cmdurl);
       return false;
   });
    });

    document.querySelectorAll('.simplisity_click').forEach(function (el, index) {
        el.setAttribute("s-index", index);
        
    const newEl = el.cloneNode(true);
 el.parentNode.replaceChild(newEl, el);
        
        newEl.addEventListener("click", function (e) {
            simplisity_setParamField("simplisity_click", new Date().toISOString());
            simplisity_callserver(this, cmdurl);
            
      const stateObj = this.getAttribute("s-fields");
    const href = this.getAttribute("href");
        if (typeof href !== 'undefined' && href !== null) {
   if (href.includes(window.location.hostname)) {
               history.pushState(stateObj, "Title", href);
  }
      }
       e.preventDefault();
   return false;
        });
    });

    document.querySelectorAll('.simplisity_confirmclick').forEach(function (el, index) {
        el.setAttribute("s-index", index);
     
  const newEl = el.cloneNode(true);
        el.parentNode.replaceChild(newEl, el);
        
        newEl.addEventListener("click", function (e) {
            if (confirm(this.getAttribute("s-confirm"))) {
              simplisity_setParamField("simplisity_confirmclick", new Date().toISOString());
            simplisity_callserver(this, cmdurl);
      }
            e.preventDefault();
            return false;
     });
    });

    document.querySelectorAll('.simplisity_removelistitem').forEach(function (el, index) {
        el.setAttribute("s-index", index);
        let parent = el.closest('li');
     if (parent) parent.setAttribute("s-index", index);
     
        const newEl = el.cloneNode(true);
        el.parentNode.replaceChild(newEl, el);
        
        newEl.addEventListener("click", function () {
            simplisity_removelistitem(this);
  });
    });

    document.querySelectorAll('.simplisity_removegriditem').forEach(function (el, index) {
        el.setAttribute("s-index", index);
let parent = el.closest('div');
     if (parent) parent.setAttribute("s-index", index);
        
        const newEl = el.cloneNode(true);
        el.parentNode.replaceChild(newEl, el);

        newEl.addEventListener("click", function () {
          simplisity_removegriditem(this);
        });
    });

    document.querySelectorAll('.simplisity_removetablerow').forEach(function (el, index) {
      el.setAttribute("s-index", index);
        let parent = el.closest('tr');
        if (parent) parent.setAttribute("s-index", index);

        const newEl = el.cloneNode(true);
        el.parentNode.replaceChild(newEl, el);
        
        newEl.addEventListener("click", function () {
 simplisity_removetablerow(this);
        });
    });

    document.querySelectorAll('.simplisity_itemundo').forEach(function (el, index) {
        const recylebin = el.getAttribute("s-recylebin");
        if (typeof recylebin !== 'undefined' && recylebin !== null) {
          if (!document.getElementById('simplisity_recyclebin_' + recylebin)) {
      var elementstr = '<div id="simplisity_recyclebin_' + recylebin + '" style="display:none;" ></div>';
             var elem = document.createElement('span');
     elem.innerHTML = elementstr;
   document.body.appendChild(elem);
            }
 }
     
        const newEl = el.cloneNode(true);
        el.parentNode.replaceChild(newEl, el);
        
   newEl.addEventListener("click", function () {
  simplisity_undoremovelistitem(this);
        });
    });

    document.querySelectorAll('.simplisity_fileupload').forEach(function (el, index) {
        simplisity_initFileUpload('#' + el.getAttribute('id'));
    });

    document.querySelectorAll('.simplisity_base64upload').forEach(function (el, index) {
        el.setAttribute("s-index", index);
      
        const newEl = el.cloneNode(true);
        el.parentNode.replaceChild(newEl, el);
    
        newEl.addEventListener("change", function () {
  simplisity_base64wait(this, cmdurl);
 return false;
 });
    });

    document.querySelectorAll('.simplisity_filedownload').forEach(function (el, index) {
        var params = "cmd=" + el.getAttribute('s-cmd');
        var sfields = el.getAttribute('s-fields');

        if (typeof sfields !== 'undefined' && sfields !== '' && sfields !== null) {
        var obj = simplisity_parsejson(sfields);
            for (var x in obj) {
       params = params + '&' + x + '=' + simplisity_encode(simplisity_getField(sfields, x));
      }

            var systemkey = simplisity_getsystemkey(sfields);
       params = params + '&systemkey=' + systemkey;
        }
     const cmdUrlEl = document.getElementById("simplisity_cmdurl");
        var cmdurl = cmdUrlEl ? cmdUrlEl.value : '';
      el.setAttribute('href', cmdurl + '?' + params);
    });
}

function simplisity_sessionfieldaction() {
    document.querySelectorAll('input.simplisity_sessionfield').forEach(function (el) {
        var v = simplisity_getSessionField(el.getAttribute('id'));
  if (typeof v !== 'undefined' && v !== '') {
            el.value = v;
        }
    });

    document.querySelectorAll('select.simplisity_sessionfield').forEach(function (selectEl) {
        var v = simplisity_getSessionField(selectEl.getAttribute('id'));
        if (typeof v !== 'undefined' && v !== '') {
            selectEl.querySelectorAll('option').forEach(function (option) {
if (option.value === v) {
           selectEl.value = v;
     }
     });
        }
    });
}

async function simplisity_base64wait(element, cmdurl) {
    element = simplisity_toDomElement(element);
    
    if (!element || !element.nodeType) {
        console.error('[simplisity_base64wait] Invalid element:', element);
        return;
    }

    var filelist = element.files;
    var flist = '';
    for (let i = 0; i < filelist.length; i++) {
        flist += simplisity_encode(filelist[i].name) + '*';
 }
    flist = flist.substring(0, flist.length - 1);
    const fileUploadListEl = document.getElementById('simplisity_fileuploadlist');
    if (fileUploadListEl) fileUploadListEl.value = flist;

    var result = await simplisity_fileListToBase64(filelist);
    var base64str = '';
    for (let i2 = 0; i2 < result.length; i2++) {
        base64str += result[i2];
    }
    base64str = base64str.substring(0, base64str.length - 1);
    const fileUploadBase64El = document.getElementById('simplisity_fileuploadbase64');
    if (fileUploadBase64El) fileUploadBase64El.value = base64str;
    simplisity_callserver(element, cmdurl);
}

async function simplisity_fileListToBase64(fileList) {
 function getBase64(file) {
   const reader = new FileReader();
        return new Promise(resolve => {
      reader.onload = ev => {
      resolve(ev.target.result + '*');
         };
        reader.readAsDataURL(file);
        });
    }

    const promises = [];
    for (let i = 0; i < fileList.length; i++) {
        promises.push(getBase64(fileList[i]));
    }

    return await Promise.all(promises);
}
