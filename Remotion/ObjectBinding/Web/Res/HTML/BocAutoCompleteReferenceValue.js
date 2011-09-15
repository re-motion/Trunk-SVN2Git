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

BocAutoCompleteReferenceValue.Initialize =
function (textbox, hiddenField, button, command, searchServiceUrl,
          completionSetCount, dropDownDisplayDelay, dropDownRefreshDelay, selectionUpdateDelay,
          nullValueString,
          isAutoPostBackEnabled,
          searchContext,
          iconServiceUrl,
          iconContext
          )
{
  textbox.autocomplete(searchServiceUrl, 'Search', 'SearchExact',
        {
          extraParams: searchContext,
          isAutoPostBackEnabled: isAutoPostBackEnabled,
          nullValue: nullValueString, // the hidden field value indicating that no value has been selected
          minChars: 0,
          max: completionSetCount, // Set query limit

          dropDownDisplayDelay: dropDownDisplayDelay,
          dropDownRefreshDelay: dropDownRefreshDelay,
          selectionUpdateDelay: selectionUpdateDelay,

          autoFill: true,
          mustMatch: false, //set true if should clear input on no results
          matchContains: true,
          scrollHeight: 220,
          dropDownButtonId: button.attr('id'),
          // this can be set to true/removed once the problem is fixed that an empty textbox still selects the first element, making it impossible to clear the selection
          selectFirst: function (inputValue, searchTerm)
          {
            return inputValue.length > 0;
          },
          dataType: 'json',
          parse: function (data)
          {
            return $.map(data, function (row)
            {
              return {
                data: row,
                value: row.UniqueIdentifier,
                result: row.DisplayName
              }
            });
          },
          formatItem: function (item) //What we display on input box
          {
            var row = $('<div/>');
            row.text(item.DisplayName);

            if (item.IconUrl != '')
            {
              var img = $('<img/>');
              img.attr({ src: item.IconUrl });
              row.prepend(' ');
              row.prepend(img);
            }

            return row.html();
          },
          formatMatch: function (item) //The value used by the cache
          {
            return item.DisplayName;
          }
        }
    ).invalidateResult(function (e, item)
    {
      hiddenField.val(nullValueString);
      UpdateCommand(nullValueString);
      //Do not fire change-event
    }).updateResult(function (e, item)
    {
      hiddenField.val(item.UniqueIdentifier);
      UpdateCommand(item.UniqueIdentifier);
      hiddenField.trigger('change');
    });


  function UpdateCommand(selectedValue)
  {
    if (command == null)
      return;

    if (isAutoPostBackEnabled)
      return;

    var businessObject = null;
    if (selectedValue != nullValueString)
      businessObject = selectedValue;

    command = BaseUpdateCommand(command, businessObject, iconServiceUrl, iconContext, '/dev/Remotion/trunk/Remotion/ObjectBinding/Web.Test/res/Remotion.Web/Themes/NovaBlue/Image/Spacer.gif');
  }
};

//  Returns the number of rows selected for the specified ReferenceValue
BocAutoCompleteReferenceValue.GetSelectionCount = function (referenceValueHiddenFieldID, nullValue)
{
  var hiddenField = document.getElementById(referenceValueHiddenFieldID);
  if (hiddenField == null || hiddenField.value == nullValue)
    return 0;

  return 1;
}

function BaseUpdateCommand(oldCommand, businessObject, iconServiceUrl, iconContext, nullIconUrl)
{
  ArgumentUtility.CheckNotNull('oldCommand', oldCommand);
  ArgumentUtility.CheckTypeIsString('businessObject', businessObject);
  ArgumentUtility.CheckTypeIsString('iconServiceUrl', iconServiceUrl);

  var oldIcon = oldCommand.find('img');

    var newCommand = $('<span/>');
    var oldCommandAttributes = oldCommand[0].attributes;
    for (var i = 0; i < oldCommandAttributes.length; i++)
      newCommand.attr(oldCommandAttributes[i].nodeName, oldCommandAttributes[i].nodeValue);

    var newIcon = oldIcon.clone();
    newIcon.attr({ src: nullIconUrl, alt: '' });
    newIcon.removeAttr('title');
    var newCommandTitle = newCommand.attr('title');
    if (!StringUtility.IsNullOrEmpty(newCommandTitle))
      newIcon.attr('title', newCommandTitle);
    newIcon.css({ width: oldIcon.width(), height: oldIcon.height() });
    newCommand.append(newIcon);

    oldCommand.replaceWith(newCommand);

    if (iconServiceUrl != null && iconContext != null && businessObject != null)
    {
      var pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();
      if (pageRequestManager.get_isInAsyncPostBack())
        return;

      var success = function (icon)
      {
        if (icon == null)
          return;
        
        newIcon.attr('src', icon.Url);
        
        newIcon.attr('alt', '');
        newIcon.attr('alt', icon.AlternateText);

        if (!StringUtility.IsNullOrEmpty(icon.ToolTip) && StringUtility.IsNullOrEmpty (newIcon.attr('title')))
          newIcon.attr('title', icon.ToolTip);
        
        newIcon.css({ width: icon.Width, heght: icon.Height });
      }
            
      var params = { businessObject: businessObject };
      for (var propertyName in iconContext)
        params[propertyName] = iconContext[propertyName];

      Sys.Net.WebServiceProxy.invoke(iconServiceUrl, 'GetIcon', false, params,
                                                  function (result, context, methodName)
                                                  {
                                                    success (result);
                                                  },
                                                  function (err, context, methodName)
                                                  {
                                                  });
    }

    return newCommand;
  }
