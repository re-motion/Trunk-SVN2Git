// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

function BocAutoCompleteReferenceValue()
{
}

BocAutoCompleteReferenceValue.Bind = 
function(textbox, hiddenField, button, webServiceUrl, businessObjectClass, businessObjectPropery, businessObjectID, args) {
    textbox.autocomplete(webServiceUrl,
        {
            extraParams: { 'businessObjectClass': businessObjectClass, 'businessObjectProperty': businessObjectPropery, 'businessObjectID': businessObjectID, 'args': args },
            minChars: 0,
            max: 30, // Set query limit
            mustMatch: false, //set true if should clear input on no results
            matchContains: true,
            scrollHeight: 220,
            width: 200, //Define width of results
            extraBind: button.attr('id'),
            dataType: 'json',
            parse: function(data) {
                return $.map(data.d, function(row) {
                    return {
                        data: row,
                        value: row.UniqueIdentifier,
                        result: row.DisplayName
                    }
                });
            },
            formatItem: function(item) {
                return item.DisplayName; //What we display on input box
            }
        }
    ).result(function(e, item) {
        var dummy = "";
        hiddenField.val(item.UniqueIdentifier); //What we populate on hidden box
        textbox.trigger('change');
    });
};

BocAutoCompleteReferenceValue.AdjustPosition = function(control, isEmbedded) {

    var totalWidth = control.innerWidth();
    var totalHeight = control.innerHeight();

    var icon = control.find('img.bocAutoCompleteReferenceValueContent').parent();

    var left = 0;
    if (icon.length > 0)
        left = icon.outerWidth(true);

    var optionsMenu = control.find('div.bocAutoCompleteReferenceValueOptionsMenu');
    var right = 0;
    if (!isEmbedded && optionsMenu.length > 0)
        right = optionsMenu.outerWidth(true);

    var contentSpan = control.find('span.bocAutoCompleteReferenceValueContent');
    contentSpan.height(totalHeight);
    contentSpan.css('left', left + 'px');
    contentSpan.css('right', right + 'px');

    if (isEmbedded) {
        var dropDownList = control.find('.bocAutoCompleteReferenceValueDropDownList');
        var dropDownMenu = dropDownList.parent().parent();

        if (dropDownMenu.length > 0) {
            dropDownMenu.css('height', dropDownList.parent().outerWidth(true));
            dropDownMenu.height(control.find('.bocAutoCompleteReferenceValueDropDownList').parent().outerHeight(true));
            icon.css('top', (dropDownMenu.innerHeight() - icon.outerHeight()) / 2);
        }

    }
    else {
        var dropDownList = control.find('.bocAutoCompleteReferenceValueDropDownList');
        if (dropDownList.length > 0) {
            var heightDifference = dropDownList.height() - optionsMenu.height();
            var offset = heightDifference / 2;
            optionsMenu.css('top', offset);
        }
    }
};

//  Returns the number of rows selected for the specified BocList
BocAutoCompleteReferenceValue.GetSelectionCount = function(referenceValueHiddenFieldId) {
    var hiddenField = document.getElementById(referenceValueHiddenFieldId);
    if (hiddenField == null || hiddenField.value == null || hiddenField.value.length == 0)
        return 0;
    
    return 1;
}
