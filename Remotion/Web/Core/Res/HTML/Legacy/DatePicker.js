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
//  DatePicker.js contains client side scripts used by DatePickerPage 
//  and the caller of the DatePickerFrom.aspx IFrame contents page.

//  The currently displayed date picker
//  Belongs to the parent page.
var _datePicker_currentDatePicker = null;

//  Helper variable for event handling.
//  Belongs to the parent page.
//  The click event of the document fires after the methods bound to the button click have been 
//  executed. _datePicker_isEventAfterDatePickerButtonClick is used to identify those click events fired
//  because a date picker button had been clicked in contrast to events fired
//  beause of a click somewhere on the page.
var _datePicker_isEventAfterDatePickerButtonClick = false;

//  Shows the date picker frame below the button.
//  Belongs to the parent page.
//  button: The button that opened the date picker frame.
//  container: The page element containing the properties to be passed to the picker.
//  target: The input element receiving the value returned by the date picker.
function DatePicker_ShowDatePicker (button, container, target, src, width, height)
{
  var datePickerID = container.id + '_DatePicker';
  //  Tried to open the already open date picker?
  //  Close it and return.
  if (DatePicker_CloseVisibleDatePickerFrame (datePickerID))
    return;
    
  if (target.disabled || target.readOnly)
    return;

   
  var left = $(button).offset().left;
  var top = $(button).offset().top;
  
  var datePicker = window.document.createElement ('div');
  $(datePicker).width(width);
  $(datePicker).height(height);
  $(datePicker).css('position', 'absolute');
  $(datePicker).css('zIndex', '1100'); // Required so the DatePicker covers DropDownMenus
  $(datePicker).attr('id', datePickerID);
  $(datePicker).css('visibility', 'hidden');
  
  var frame = window.document.createElement ("iframe");
  datePicker.appendChild (frame);
  frame.src = src + '?TargetIDField=' + target.id + '&DatePickerIDField=' + datePicker.id + '&DateValueField=' + target.value;
  frame.frameBorder = 'no';
  frame.scrolling = 'no';
  frame.style.width = '100%';
  frame.style.height = '100%';
  frame.marginWidth = 0;
  frame.marginHeight = 0;
  
  var datePickerLeft;
  var datePickerTop;
  var datePickerWidth;
  var datePickerHeight;
  window.document.body.insertBefore(datePicker, window.document.body.children[0]);

  //  Adjust position so the date picker is shown below 
  //  and aligned with the right border of the button.
  $(datePicker).css('left', left - $(frame).width() + $(button).width());
  $(datePicker).css('top', top + $(button).height());
  datePickerLeft = $(datePicker).offset().left;
  datePickerTop = $(datePicker).offset().top;
  datePickerWidth = $(datePicker).width();
  datePickerHeight = $(datePicker).height();
  
  //  Re-adjust the button, in case available screen space is insufficient
  var totalBodyHeight = $('body').height();
  var visibleBodyTop = $('body').scrollTop();
  var visibleBodyHeight = $(window).height();
  
  var datePickerTopAdjusted = datePickerTop;
  if (visibleBodyTop + visibleBodyHeight < datePickerTop + datePickerHeight)
  {
    var newTop = $(button).offset().top - datePickerHeight;
    if (newTop >= 0)
      datePickerTopAdjusted = newTop;
  }
  
  var totalBodyWidth = $('body').width();
  var visibleBodyLeft = $('body').scrollLeft();
  var visibleBodyWidth = $(window).width();
  
  var datePickerLeftAdjusted = datePickerLeft;
  if (datePickerLeft < visibleBodyLeft && datePickerWidth <= visibleBodyWidth)
    datePickerLeftAdjusted = visibleBodyLeft;
  
  $(datePicker).css('left', datePickerLeftAdjusted);
  $(datePicker).css('top', datePickerTopAdjusted);

  if (   visibleBodyTop > 0
      && datePickerTopAdjusted < visibleBodyTop)
  {
    $('body').scrollTop(datePickerTopAdjusted);
  }
  
  _datePicker_currentDatePicker = datePicker;
  _datePicker_isEventAfterDatePickerButtonClick = true;
  window.parent.document.onclick = DatePicker_OnDocumentClick;
  $(datePicker).css('visibility', 'visible');
}

//  Closes the currently visible date picker frame.
//  Belongs to the parent page.
//  newDatePicker: The newly selected date picker frame, used to test whether the current frame 
//      is identical to the new frame.
//  returns true if the newDatePicker is equal to the visible date picker.
function DatePicker_CloseVisibleDatePickerFrame (newDatePickerID)
{
  var newDatePicker = document.getElementById (newDatePickerID);
  if (   newDatePicker != null
      && newDatePicker == _datePicker_currentDatePicker
      && newDatePicker.style.display != 'none')
  {
    return true;
  }        
  if (_datePicker_currentDatePicker != null)
  {
    var currentDatePicker = window.document.getElementById (_datePicker_currentDatePicker.id);
    var frameContent = currentDatePicker.children[0].contentWindow;
    frameContent.DatePicker_CloseDatePicker();
    _datePicker_currentDatePicker = null;
  }
  return false;
}

//  Called by the date picker when a new date is selected in the calendar. 
//  Belongs to the date picker frame.
function DatePicker_Calendar_SelectionChanged (value)
{
  var target = window.parent.document.getElementById (document.getElementById ('TargetIDField').value);
  var isValueChanged = target.value != value;
  DatePicker_CloseDatePicker();
  target.value = value;
  if (isValueChanged)
  {
    $(target).change();
  }
}

//  Closes the date picker frame
//  Belongs to the date picker frame.
function DatePicker_CloseDatePicker() 
{
  var target = window.parent.document.getElementById(document.getElementById('TargetIDField').value);
  window.parent.document.onclick = null;
  try
  {
    target.focus();
  }
  catch (e)
  {
  }  
  window.parent._datePicker_currentDatePicker = null;
  var datePicker = window.parent.document.getElementById (document.getElementById ('DatePickerIDField').value);
  datePicker.style.display = 'none';
}

//  Called by th event handler for the onclick event of the parent pages's document.
//  Belongs to the parent page.
//  Closes the currently open date picker frame, 
//  unless _datePicker_isEventAfterDatePickerButtonClick is set to false.
function DatePicker_OnDocumentClick()
{
  if (_datePicker_isEventAfterDatePickerButtonClick)
  {
    _datePicker_isEventAfterDatePickerButtonClick = false;
  }
  else if (_datePicker_currentDatePicker != null)
  {
    var currentDatePicker = window.document.getElementById (_datePicker_currentDatePicker.id);
    var frameContent = currentDatePicker.children[0].contentWindow;
    frameContent.DatePicker_CloseDatePicker();
    _datePicker_currentDatePicker = null;
  }  
}
