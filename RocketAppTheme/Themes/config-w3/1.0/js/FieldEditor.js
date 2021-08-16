$(document).ready(function () {

        $(':checkbox').addClass('w3-check');

    $("#fielddatalist").sortable();
    $("#fielddatalist").disableSelection();

    $('.addvaluekey').unbind("click");
        $('.addvaluekey').click(function () {
            var row = $(this).attr('row');
    dnnrocket_addtextli(row,'','');
});

        $('.dictionarykeyvalue').each(function (index) {
            var row = $(this).attr('row');
    dnnrocket_popdictionary(row);
});

$('.inputtypefield').unbind("change");
        $('.inputtypefield').change(function () {

            var row = $(this).attr('row');
    $('#dictionarydisplay' + row).hide();
    $('.imagesize' + row).hide();
    $('.allowempty' + row).hide();
    $('.defaulttextbox' + row).show();

            if (($(this).val().toLowerCase() === 'dropdown') || ($(this).val().toLowerCase() === 'radiolist') || ($(this).val().toLowerCase() === 'checkboxlist')) {
        $('#dictionarydisplay' + row).show();
    }

            if (($(this).val().toLowerCase() === 'image') || ($(this).val().toLowerCase() === 'imagefull')) {
        $('#attributes_' + row).val("");
    $('.imagesize' + row).show();
    $('.defaulttextbox' + row).hide();
}

            if (($(this).val().toLowerCase() === 'internalpage')) {
        $('.allowempty' + row).show();
    $('.defaulttextbox' + row).hide();
}

            if (($(this).val().toLowerCase() === 'textbox')) {
        $('#attributes_' + row).val(" class='w3-input w3-border' autocomplete='off' ");
    }

            if (($(this).val().toLowerCase() === 'textarea')) {
                $('#attributes_' + row).val(" class='w3-input w3-border' rows='4' ");
    }

            if (($(this).val().toLowerCase() === 'document')) {
        $('#attributes_' + row).val(" class='w3-input w3-border' autocomplete='off' ");
    $('.defaulttextbox' + row).hide();
}

            if (($(this).val().toLowerCase() === 'checkbox')) {
        $('#attributes_' + row).val(" class='w3-check' ");
    }

            if (($(this).val().toLowerCase() === 'dropdown') || ($(this).val().toLowerCase() === 'internalpage')) {
        $('#attributes_' + row).val("class='w3-select w3-border' ");
    }

            if (($(this).val().toLowerCase() === 'radiolist')) {
        $('#attributes_' + row).val("class='w3-input w3-border' ");
    }

            if (($(this).val().toLowerCase() === 'checkbox')) {
        $('#attributes_' + row).val("class='w3-check' ");
    }

            if (($(this).val().toLowerCase() === 'checkboxlist')) {
        $('#attributes_' + row).val("class='w3-input w3-border' ");
    }


});

$('.editbutton').unbind("click");
        $('.editbutton').click(function () {
            var row = $(this).attr('row');
    dnnrocket_showdetail(row);
});

$('.editbuttonclose').unbind("click");
        $('.editbuttonclose').click(function () {
            var row = $(this).attr('row');
    dnnrocket_hidedetail(row);
});

$('.isonlist').unbind("click");
        $('.isonlist').click(function () {
            var row = $(this).attr('row');
            if ($(this).prop('checked')) {
                var listcolval = 1;
                $('.listcol:visible').each(function(i, obj) {
        listcolval = listcolval + 1;
    });
    $('#listcol_' + row).val(listcolval);
    $('.listcolclass' + row).show();
}
            else {
        $('.listcolclass' + row).hide();
    }
});

        $('.isonlist').each(function(i, obj) {
            var row = $(this).attr('row');
            if ($(this).prop('checked')) {
        $('.listcolclass' + row).show();
    }
            else {
        $('.listcolclass' + row).hide();
    }
});

});

    function dnnrocket_showdetail(row) {
        $('#detailDisplay' + row).show();
    $("#fielddatalist").sortable("option", "disabled", true);
    $('#editbutton' + row).hide();
    $('#editbuttoncloseup' + row).show();
    $('#editbuttonclosedown' + row).show();
    $('#editbutton' + row).addClass('detailopen');
}

    function dnnrocket_hidedetail(row) {
        $('#detailDisplay' + row).hide();

    $('#editbutton' + row).show();
    $('#editbuttoncloseup' + row).hide();
    $('#editbuttonclosedown' + row).hide();
    $('#editbutton' + row).removeClass('detailopen');

    var activatesort = true;
        $('.editbutton').each(function (index) {
            if ($(this).hasClass('detailopen')) {
        activatesort = false;
    }
});
        if (activatesort) {
        $("#fielddatalist").sortable("option", "disabled", false);
    }
}


    function dnnrocket_addtextli(index, key, value) {

        if (typeof value === 'undefined') {
        value = '';
    }

    var addtext = '';
        addtext += '<li class="dictionarydata' + index + ' w3-card w3-margin-bottom w3-display-container"> ';
        addtext += ' <div class="w3-row">';
        addtext += '		<div class="w3-col w3-padding m4">';
        addtext += '			<input value="' + key + '" row="' + index + '" value="" s-update="ignore" class=" dictionaryInput dictionaryKey' + index + ' w3-input w3-border" autocomplete="off" type="text">';
        addtext += '		</div>';
        addtext += '		<div class="w3-col w3-padding m7">';
        addtext += '			<input value="' + value + '" id="dictionaryvalue' + index + '" s-update="ignore" row="' + index + '" value="" class=" dictionaryInput dictionaryValue' + index + ' w3-input w3-border" autocomplete="off" type="text">';
        addtext += '		</div>';
        addtext += '		<div class="w3-col w3-padding m1">';
        addtext += '		<span class="hidedictionary w3-button w3-transparent" row="' + index + '" >&times;</span>';
        addtext += '		</div>';
        addtext += '	</div>';
        addtext += '</li>';
    
            $('.dictionarylist' + index).append(addtext);
    
        $('.dictionarylist' + index).sortable({
                stop: function (event, ui) {
                var row = $(this).attr('row');
            dnnrocket_savedictionary(row);
        }
    });
    $('.dictionarylist' + index).disableSelection();


    $('.dictionaryInput').unbind("change");
        $('.dictionaryInput').change(function () {
            var row = $(this).attr('row');
            dnnrocket_savedictionary(row);
        });

        $('.hidedictionary').unbind("click");
        $('.hidedictionary').click(function () {
                this.parentElement.parentElement.parentElement.style.display = 'none';
            var row = $(this).attr('row');
            dnnrocket_savedictionary(row);
        });

    }


    function dnnrocket_savedictionary(row) {
        var keyList = '';
            var valueList = '';
        $('.dictionaryKey' + row).each(function (index) {
            if ($(this).is(':visible')) {
                keyList += simplisity_encode($(this).val().replace(/"/g, "'")) + ',';
            }
        });
        keyList = keyList.slice(0, -1);

        $('.dictionaryValue' + row).each(function (index) {
            if ($(this).is(':visible')) {
                valueList += simplisity_encode($(this).val().replace(/"/g, "'")) + ',';
            }
        });
        valueList = valueList.slice(0, -1);

        $('#dictionarykey_' + row).val(keyList);
        $('#dictionaryvalue_' + row).val(valueList);

    }

    function dnnrocket_popdictionary(row) {

        var keyList = $('#dictionarykey_' + row).val().split(',');
        var valueList = $('#dictionaryvalue_' + row).val().split(',');

        var arrayLength = keyList.length;
        for (var i = 0; i < arrayLength; i++) {
            var key = simplisity_decode(keyList[i]);
            if (key !== '') {
                var value = simplisity_decode(valueList[i]);
                dnnrocket_addtextli(row, key, value)
            }
        }

    $('#dictionarydisplay' + row).hide();
    $('.imagesize' + row).hide();
    $('.allowempty' + row).hide();
    $('.defaulttextbox' + row).show();

        if (($('#type_' + row).val().toLowerCase() === 'dropdown') || ($('#type_' + row).val().toLowerCase() === 'radiolist') || ($('#type_' + row).val().toLowerCase() === 'checkboxlist')) {
                $('#dictionarydisplay' + row).show();
            }
    
        if (($('#type_' + row).val().toLowerCase() === 'image') || ($('#type_' + row).val().toLowerCase() === 'imagefull')) {
                $('.imagesize' + row).show();
            $('.defaulttextbox' + row).hide();
        }

        if (($('#type_' + row).val().toLowerCase() === 'internalpage')) {
                $('.allowempty' + row).show();
            $('.defaulttextbox' + row).hide();
        }
        if (($('#type_' + row).val().toLowerCase() === 'document')) {
                $('.defaulttextbox' + row).hide();
            }
    
    
        }
    
    function validateform() {

                //$('#Fields').show();

                $('.validationfail').hide();

            jQuery.validator.addMethod("alphanumeric", function (value, element) {
            return this.optional(element) || /^[a-zA-Z0-9]+$/.test(value);
        }, 'Error');

        jQuery.validator.addMethod("notnumeric", function (value, element) {
            return this.optional(element) || !$.isNumeric(value);
        }, 'Error');

        jQuery.validator.addMethod("notstartnumeric", function (value, element) {
            return this.optional(element) || !$.isNumeric(value.substring(0, 1));
        }, 'Error');

        jQuery.validator.setDefaults({
                ignore: ":hidden:not(#fielddatasection input[type=text])",
        });

        $("#fielddatasection input[type=text]").each(function () {
            if (!$(this).valid()) {
                $(this).css({ 'background-color': '#ffe6e6' });
                $('.activatevalidation').attr('s-stop', 'stop');
            $('.validationfail').show();
        }
    });
}

