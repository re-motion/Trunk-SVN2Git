// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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

BocAutoCompleteReferenceValue.Initialize = function (
    textbox, hiddenField, button, command, searchServiceUrl,
    completionSetCount, dropDownDisplayDelay, dropDownRefreshDelay, selectionUpdateDelay,
    searchStringValidationInfo,
    nullValueString,
    isAutoPostBackEnabled,
    searchContext,
    iconServiceUrl,
    iconContext,
    commandInfo,
    resources)
{
  ArgumentUtility.CheckNotNullAndTypeIsObject('textbox', textbox);
  ArgumentUtility.CheckNotNullAndTypeIsObject('hiddenField', hiddenField);
  ArgumentUtility.CheckNotNullAndTypeIsObject('button', button);
  ArgumentUtility.CheckTypeIsObject('command', command);
  ArgumentUtility.CheckNotNullAndTypeIsString('searchServiceUrl', searchServiceUrl);
  ArgumentUtility.CheckNotNullAndTypeIsNumber('completionSetCount', completionSetCount);
  ArgumentUtility.CheckNotNullAndTypeIsNumber('dropDownDisplayDelay', dropDownDisplayDelay);
  ArgumentUtility.CheckNotNullAndTypeIsNumber('dropDownRefreshDelay', dropDownRefreshDelay);
  ArgumentUtility.CheckNotNullAndTypeIsNumber('selectionUpdateDelay', selectionUpdateDelay);
  ArgumentUtility.CheckNotNullAndTypeIsObject('searchStringValidationInfo', searchStringValidationInfo);
  ArgumentUtility.CheckNotNullAndTypeIsString('nullValueString', nullValueString);
  ArgumentUtility.CheckNotNullAndTypeIsBoolean('isAutoPostBackEnabled', isAutoPostBackEnabled);
  ArgumentUtility.CheckNotNullAndTypeIsObject('searchContext', searchContext);
  ArgumentUtility.CheckTypeIsString('iconServiceUrl', iconServiceUrl);
  ArgumentUtility.CheckTypeIsObject('iconContext', iconContext);
  ArgumentUtility.CheckTypeIsObject('commandInfo', commandInfo);
  ArgumentUtility.CheckNotNullAndTypeIsObject('resources', resources);

  textbox.autocomplete(searchServiceUrl, 'Search', 'SearchExact',
        {
          extraParams: searchContext,
          isAutoPostBackEnabled: isAutoPostBackEnabled,
          nullValue: nullValueString, // the hidden field value indicating that no value has been selected
          searchStringValidationParams:
          {
            inputRegex: new RegExp(searchStringValidationInfo.ValidSearchStringRegex),
            dropDownTriggerRegex: new RegExp(searchStringValidationInfo.ValidSearchStringForDropDownRegex),
            dropDownTriggerRegexFailedMessage: searchStringValidationInfo.SearchStringForDropDownDoesNotMatchRegexMessage
          },
          max: completionSetCount, // Set query limit

          dropDownDisplayDelay: dropDownDisplayDelay,
          dropDownRefreshDelay: dropDownRefreshDelay,
          selectionUpdateDelay: selectionUpdateDelay,

          autoFill: true,
          mustMatch: false, // set true if should clear input on no results
          matchContains: true,
          multiple: false, // not supprted
          dropDownButtonId: button.attr('id'),
          inputAreaClass: 'content',
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
                data : row,
                value : row.UniqueIdentifier,
                result : row.DisplayName
              };
            });
          },
          formatItem: function (item) //What we display on input box
          {
            var row = $('<li/>');

            if (item.IconUrl != '')
            {
              var img = $('<img/>');
              img.attr ({ src : item.IconUrl });
              row.append ($('<div/>').append (img));
            }

            var displayName = $('<span/>');
            displayName.text (item.DisplayName);
            row.append ($ ('<div/>').append (displayName));

            return row.html();
          },
          formatMatch: function (item) //The value used by the cache
          {
            return item.DisplayName;
          },
          handleRequestError: function (err)
          {
            SetError (resources.LoadDataFailedErrorMessage);
          },
          clearRequestError: function ()
          {
            ClearError();
          }
        }
    ).invalidateResult(function (e, item)
    {
      if (hiddenField.val() == nullValueString)
        return;

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
    if (command == null || command.length == 0)
      return;

    if (isAutoPostBackEnabled)
    {
      command = BocReferenceValueBase.UpdateCommand(command, null, null, null, null, function () { });
    }
    else
    {
      var businessObject = null;
      if (selectedValue != nullValueString)
        businessObject = selectedValue;

      var errorHandler = function (error)
      {
        SetError (resources.LoadIconFailedErrorMessage);
      };

      command = BocReferenceValueBase.UpdateCommand(command, businessObject, iconServiceUrl, iconContext, commandInfo, errorHandler);
    } 
  }

  function ClearError()
  {
    if (textbox.hasClass('error'))
    {
      textbox.attr ('title', textbox.data ('title-backup'));
      textbox.removeData ('title-backup');
      textbox.removeClass ('error');
    }
  };

  function SetError(message)
  {
    if (!textbox.hasClass('error'))
    {
      var oldTitle = textbox.attr ('title');
      if (TypeUtility.IsUndefined (oldTitle))
        oldTitle = null;

      textbox.data ('title-backup', oldTitle);
    }
    textbox.attr ('title', message);
    textbox.addClass ('error');
  };
};

//  Returns the number of rows selected for the specified ReferenceValue
BocAutoCompleteReferenceValue.GetSelectionCount = function (referenceValueHiddenFieldID, nullValueString)
{
  ArgumentUtility.CheckNotNullAndTypeIsString('referenceValueHiddenFieldID', referenceValueHiddenFieldID);
  ArgumentUtility.CheckNotNullAndTypeIsString('nullValueString', nullValueString);

  var hiddenField = document.getElementById(referenceValueHiddenFieldID);
  if (hiddenField == null || hiddenField.value == nullValueString)
    return 0;

  return 1;
};

