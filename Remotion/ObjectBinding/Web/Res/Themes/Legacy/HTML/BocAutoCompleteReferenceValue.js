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
function(textbox, hiddenField, button, webServiceUrl, webServiceMethod,
         completionSetCount, completionInterval, suggestionInterval,
         nullValueString, businessObjectClass, businessObjectPropery, businessObjectID, args)
{
  textbox.autocomplete(webServiceUrl, webServiceMethod,
        {
          extraParams: { 'businessObjectClass': businessObjectClass, 'businessObjectProperty': businessObjectPropery, 'businessObjectID': businessObjectID, 'args': args },
          nullValue: nullValueString, // the hidden field value indicating that no value has been selected
          minChars: 0,
          max: completionSetCount, // Set query limit

          // re-motion: distinguish between list delay and aout-fill delay
          displayListDelay: completionInterval,
          autoFillDelay: suggestionInterval,

          autoFill: false,
          mustMatch: false, //set true if should clear input on no results
          matchContains: true,
          scrollHeight: 220,
          dropDownButtonId: button.attr('id'),
          dataType: 'json',
          parse: function(data)
          {
            return $.map(data, function(row)
            {
              return {
                data: row,
                value: row.UniqueIdentifier,
                result: row.DisplayName
              }
            });
          },
          formatItem: function(item)
          {
            return item.DisplayName; //What we display on input box
          }
        }
    ).result(function(e, item)
    {
      hiddenField.val(item.UniqueIdentifier); //What we populate on hidden box
      textbox.trigger('change');
    });
};

//  Returns the number of rows selected for the specified ReferenceValue
BocAutoCompleteReferenceValue.GetSelectionCount = function(referenceValueHiddenFieldID, nullValue)
{
  var hiddenField = document.getElementById(referenceValueHiddenFieldID);
  if (hiddenField == null || hiddenField.value == nullValue)
    return 0;

  return 1;
}
