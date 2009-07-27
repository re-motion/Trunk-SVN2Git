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
Type.registerNamespace ('Remotion.UI');

Remotion.UI.ClickBehavior = function Remotion$UI$ClickBehavior (element)
{
  Remotion.UI.ClickBehavior.initializeBase(this, [element]);
}

function Remotion$UI$ClickBehavior$add_click(handler)
{
  this.get_events().addHandler('click', handler);
}

function Remotion$UI$ClickBehavior$remove_click(handler)
{
  this.get_events().removeHandler('click', handler);
}

function Remotion$UI$ClickBehavior$dispose() 
{
  $clearHandlers(this.get_element());
  Remotion.UI.ClickBehavior.callBaseMethod(this, 'dispose');
}

function Remotion$UI$ClickBehavior$initialize()
{
  Remotion.UI.ClickBehavior.callBaseMethod(this, 'initialize');
  $addHandlers(this.get_element(), { 'click' : this._onClick }, this);
}

function Remotion$UI$ClickBehavior$_onClick()
{
  var handler = this.get_events().getHandler('click');
  if (handler)
    handler(this, Sys.EventArgs.Empty);
}
    
Remotion.UI.ClickBehavior.prototype = 
{
  _clickHandler: null,

  add_click: Remotion$UI$ClickBehavior$add_click,
  
  remove_click: Remotion$UI$ClickBehavior$remove_click,

  dispose: Remotion$UI$ClickBehavior$dispose,

  initialize: Remotion$UI$ClickBehavior$initialize,

  _onClick: Remotion$UI$ClickBehavior$_onClick
}

Remotion.UI.ClickBehavior.descriptor = 
{
  events: [ {name: 'click'} ]
}

Remotion.UI.ClickBehavior.registerClass('Remotion.UI.ClickBehavior', Sys.UI.Behavior);


Sys.UI.DomElement.getLocation = function (element) 
{
  var offsetX = 0;
  var offsetY = 0;
  var parent;
  
  for (parent = element; parent; parent = parent.offsetParent) 
  {
    if (parent.offsetLeft) 
      offsetX += parent.offsetLeft;
    if (parent.clientLeft)
      offsetX += parent.clientLeft;
    if (parent.scrollLeft)
      offsetX -= parent.scrollLeft;

    if (parent.offsetTop)
      offsetY += parent.offsetTop;
    if (parent.clientTop)
      offsetY += parent.clientTop;
    if (parent.scrollTop)
      offsetY -= parent.scrollTop;
  }

  return { x: offsetX, y: offsetY };
}


Remotion.UI.BocQuirksModeAutoCompleteReferenceValueBehavior = function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior (element)
{
    /// <param name="element" domElement="true"></param>
    var e = Function._validateParams(arguments, [
        {name: "element", domElement: true}
    ]);
    if (e) throw e;

    Remotion.UI.BocQuirksModeAutoCompleteReferenceValueBehavior.initializeBase (this, [element]);
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_appUrl () 
{ return this._appUrl; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_appUrl (value) 
{ this._appUrl = value;}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_CompletionInterval ()
{ return this._completionInterval; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_CompletionInterval (value) 
{ this._completionInterval = value; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_SuggestionInterval ()
{ return this._suggestionInterval; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_SuggestionInterval (value) 
{ this._suggestionInterval = value; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_CompletionList () 
{ return this._completionListElement; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_CompletionList (value) 
{ this._completionListElement = value; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_CompletionSetCount () 
{ return this._completionSetCount; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_CompletionSetCount (value) 
{ this._completionSetCount = value; }
 
function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_BusinessObjectClass () 
{ return this._businessObjectClass; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_BusinessObjectClass (value) 
{ this._businessObjectClass = value; }
 
function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_BusinessObjectProperty () 
{ return this._businessObjectProperty; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_BusinessObjectProperty (value) 
{ this._businessObjectProperty = value; }
 
function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_BusinessObjectID () 
{ return this._businessObjectID; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_BusinessObjectID (value) 
{ this._businessObjectID = value; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_Args () 
{ return this._args; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_Args (value) 
{ this._args = value; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_DropDownPanelClass () 
{ return this._dropDownPanelClass; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_DropDownPanelClass (value) 
{ this._dropDownPanelClass = value; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_MinimumPrefixLength () 
{ return this._minimumPrefixLength; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_MinimumPrefixLength (value) 
{ this._minimumPrefixLength = value; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_ServiceMethod () 
{ return this._serviceMethod; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_ServiceMethod (value) 
{ this._serviceMethod = value; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_ServiceUrl () 
{ return this._serviceUrl; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_ServiceUrl (value) 
{ this._serviceUrl = value; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_TextBoxID () 
{ return this._textBox != null ? this._textBox.id : null; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_TextBoxID (value) 
{
  var element = $get(value);
  Sys.Debug.assert (element != null, String.format("No text box with ID '{0}' found.", value));
  this._textBox = new Sys.UI.Control (element); 
  this._textBox.initialize ();
}
  
function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_TextBox () 
{ return this._textBox; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_TextBox (value) 
{ this._textBox = value; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_HiddenFieldID () 
{ return this._hiddenField != null ? this._hiddenField.id : null; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_HiddenFieldID (value) 
{
  var element = $get(value);
  Sys.Debug.assert (element != null, String.format("No hidden field with ID '{0}' found.", value));
  this._hiddenField = new Sys.UI.Control (element); 
  this._hiddenField.initialize ();
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_HiddenField () 
{ return this._hiddenField; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_HiddenField (value) 
{ this._hiddenField = value; }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$dispose () 
{
  if (this._updateTimer) 
  {
    this._updateTimer.remove_tick (this._updateTimerTickHandler);
    this._updateTimer.dispose ();
  }
  
  if (this._blurTimer) 
  {
    this._blurTimer.remove_tick (this._blurTimerTickHandler);
    this._blurTimer.dispose ();
  }

  if (this._suggestionTimer)
  {
    this._suggestionTimer.remove_tick (this._suggestionTimerTickHandler);
    this._suggestionTimer.dispose ();
  }
  
  var textBoxElement = this._textBox._element;
  $removeHandler (textBoxElement, 'focus', this._textBoxFocusHandler);
  $removeHandler (textBoxElement, 'blur', this._textBoxBlurHandler);
  $removeHandler (textBoxElement, 'keydown', this._textBoxKeyDownHandler);
  $removeHandler (textBoxElement, 'keyup', this._textBoxKeyUpHandler);
  
  $removeHandler (this._completionListElement, 'mousedown', this._completionListMouseDownHandler);
  $removeHandler (this._completionListElement, 'mouseup', this._completionListMouseUpHandler);
  $removeHandler (this._completionListElement, 'mouseover', this._completionListMouseOverHandler);
  $removeHandler (this._completionListElement, 'focus', this._completionListFocusHandler);
  $removeHandler (this._completionListElement, 'blur', this._completionListBlurHandler);

  this._dropDownImageClickBehavior.remove_click (this._dropDownImageClickHandler);
  $removeHandler (this._dropDownImage._element, 'mousedown', this._dropDownImageMouseDownHandler);
  
  this._updateTimerTickHandler = null;
  this._blurTickHandler = null;
  this._textBoxFocusHandler = null;
  this._textBoxBlurHandler = null;
  this._textBoxKeyDownHandler = null;
  this._textBoxKeyUpHandler = null;
  this._completionListFocusHandler = null;
  this._completionListBlurHandler = null;
  this._completionListMouseDownHandler = null;
  this._completionListMouseUpHandler = null;
  this._completionListMouseOverHandler = null;
  this._dropDownImageClickHandler = null;
  this._dropDownImageMouseDownHandler = null;
  
  this._textBox = null;
  this._hiddenField = null;
  this._dropDownImage = null;

  Remotion.UI.BocQuirksModeAutoCompleteReferenceValueBehavior.callBaseMethod (this, 'dispose');
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$initialize () 
{
  Remotion.UI.BocQuirksModeAutoCompleteReferenceValueBehavior.callBaseMethod (this, 'initialize');

  this._updateTimerTickHandler = Function.createDelegate (this, this._onUpdateTimerTick);
  this._blurTimerTickHandler = Function.createDelegate (this, this._onBlurTimerTick);
  this._suggestionTimerTickHandler = Function.createDelegate (this, this._onSuggestionTimerTick);
  this._textBoxFocusHandler = Function.createDelegate (this, this._onTextBoxFocus);
  this._textBoxBlurHandler = Function.createDelegate (this, this._onTextBoxBlur);
  this._textBoxKeyDownHandler = Function.createDelegate (this, this._onTextBoxKeyDown);
  this._textBoxKeyUpHandler = Function.createDelegate (this, this._onTextBoxKeyUp);
  this._completionListFocusHandler = Function.createDelegate (this, this._onCompletionListFocus);
  this._completionListBlurHandler = Function.createDelegate (this, this._onCompletionListBlur);
  this._completionListMouseDownHandler = Function.createDelegate (this, this._onCompletionListMouseDown);
  this._completionListMouseUpHandler = Function.createDelegate (this, this._onCompletionListMouseUp);
  this._completionListMouseOverHandler = Function.createDelegate (this, this._onCompletionListMouseOver);
  this._dropDownImageClickHandler = Function.createDelegate (this, this._onDropDownImageClick);
  this._dropDownImageMouseDownHandler = Function.createDelegate (this, this._onDropDownImageMouseDown);
 
  this._updateTimer = new Sys.Timer ();
  this._updateTimer.set_interval (this._completionInterval);
  this._updateTimer.add_tick (this._updateTimerTickHandler);

  this._blurTimer = new Sys.Timer ();
  this._blurTimer.set_interval (150);
  this._blurTimer.add_tick (this._blurTimerTickHandler);

  this._suggestionTimer = new Sys.Timer ();
  this._suggestionTimer.set_interval (this._suggestionInterval);
  this._suggestionTimer.add_tick (this._suggestionTimerTickHandler);
  
  var textBoxElement = this._textBox._element;
  textBoxElement.autocomplete = "off";
  $addHandler (textBoxElement, 'focus', this._textBoxFocusHandler);
  $addHandler (textBoxElement, 'blur', this._textBoxBlurHandler);
  $addHandler (textBoxElement, 'keydown', this._textBoxKeyDownHandler);
  $addHandler (textBoxElement, 'keyup', this._textBoxKeyUpHandler);

  var elementBounds = Sys.UI.DomElement.getBounds (this.get_element());
  
  var hasExternalCompletionList = this._completionListElement != null;
  if (!hasExternalCompletionList) {
      this._completionListElement = document.createElement ('DIV');
      this._completionListElement.attributes['class'].value = this._dropDownPanelClass;
      document.body.appendChild (this._completionListElement);
  }
  var completionListStyle = this._completionListElement.style;
  completionListStyle.visibility = 'hidden';
  if (!hasExternalCompletionList)
  {
    completionListStyle.backgroundColor = 'window';
    completionListStyle.color = 'windowtext';
    completionListStyle.border = 'solid 1px buttonshadow';
    completionListStyle.cursor = 'default';
  }
  completionListStyle.unselectable = 'unselectable';
  completionListStyle.overflow = 'auto';
  completionListStyle.width = (elementBounds.width) + 'px';
  $addHandler (this._completionListElement, 'mousedown', this._completionListMouseDownHandler);
  $addHandler (this._completionListElement, 'mouseup', this._completionListMouseUpHandler);
  $addHandler (this._completionListElement, 'mouseover', this._completionListMouseOverHandler);
  $addHandler (this._completionListElement, 'focus', this._completionListFocusHandler);
  $addHandler (this._completionListElement, 'blur', this._completionListBlurHandler);
  completionListStyle.display = 'block';
  document.body.appendChild (this._completionListElement);
  this._popUpControl = new Sys.UI.Control (this._completionListElement);
  completionListStyle.display = 'none';
  this._popupBehavior = new AjaxControlToolkit.PopupBehavior (this._completionListElement);
  this._popupBehavior.set_parentElement (this.get_element());
  this._popupBehavior.set_positioningMode (AjaxControlToolkit.PositioningMode.BottomLeft);
  //popUpControl.get_behaviors ().add (this._popupBehavior);
  this._popupBehavior.initialize ();
  this._popUpControl.initialize ();

  var imageElement = document.createElement ('IMG');
  this._dropDownImage = new Sys.UI.Control (imageElement);
  this._dropDownImage.get_element().src = 'res/Remotion.Web/Image/Spacer.gif';
  Sys.UI.DomElement.addCssClass (this._dropDownImage.get_element(), 'bocAutoCompleteReferenceValueDropDownImage');
  $addHandler (imageElement, 'mousedown', this._dropDownImageMouseDownHandler);
  
  if (true) // IE
    this.get_element().style.display = 'inline-block';
  this.get_element().style.width = textBoxElement.currentStyle.width;
  textBoxElement.style.width = '100%';
  this.get_element().insertBefore (imageElement, this._hiddenField._element);
  this.get_element().style.paddingRight = imageElement.currentStyle.marginRight.replace (/-/, '');
 
  this._dropDownImageClickBehavior = new Remotion.UI.ClickBehavior (imageElement);
  this._dropDownImageClickBehavior.add_click (this._dropDownImageClickHandler);
  
  this._dropDownImageClickBehavior.initialize ();
  this._dropDownImage.initialize ();
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_hideCompletionList () 
{
  this._popupBehavior.hide ();
  this._completionListElement.innerHTML = '';
  this._selectedIndex = -1;
  this._lastKeyWasCursorUpOrDown = false;
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_highlightItem (item, scrollIntoView) 
{
  var children = this._completionListElement.childNodes;
  if (this._selectedIndex != -1)
  {
    var isOldItemKnown = false;
    var style = children[this._selectedIndex].style;
    if (style.backgroundColor != '')
    {
      style.backgroundColor = '';
      style.color = '';
      isOldItemKnown = true;
    }
  
    if (!isOldItemKnown && item.previousSibling != null)
    {
      var style = item.previousSibling.style;
      if (style.backgroundColor != '')
      {
        style.backgroundColor = '';
        style.color = '';
        isOldItemKnown = true;
      }
    }
    
    if (!isOldItemKnown && item.nextSibling != null)
    {
      var style = item.nextSibling.style;
      if (style.backgroundColor != '')
      {
        style.backgroundColor = '';
        style.color = '';
        isOldItemKnown = true;
      }
    }
    
    if (!isOldItemKnown)
    {
      for (var i = children.length - 1; i >= 0; i--)
      {
        var style = children[i].style;
        style.backgroundColor = '';
        style.color = '';
      }
    }
  }

  item.style.backgroundColor = 'highlight';
  item.style.color = 'highlighttext';
  if (scrollIntoView)
    item.scrollIntoView ();
  selectedIndex = i;

  return item.index;
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onCompletionListMouseDown () 
{
  if (window.event.srcElement != this._completionListElement) 
  {
    var hasChanged = window.event.srcElement.value.DisplayName != this._oldText;
    this._setValue (window.event.srcElement.value, true);
    if (hasChanged)
      this._textBox._element.fireEvent ('onchange', null);      
  }
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onCompletionListMouseUp () 
{
  this._textBox.focus ();
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onCompletionListMouseOver () 
{
  if (window.event.srcElement != this._completionListElement)
    this._selectedIndex = this._highlightItem (window.event.srcElement, false);
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onTextBoxFocus () 
{
  if (this._oldText == null)
    this._oldText = this._textBox.get_element().value;
  this._currentPrefix = this._textBox.get_element().value.toUpperCase();
  this._updateTimer.set_enabled (true);
  this._blurTimer.set_enabled (false);
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onTextBoxKeyDown (ev)
{
  if (this._oldText == null)
    this._oldText = this._textBox.get_element().value;
    
  //var e = window.event;
  var e = ev;
  if (e.keyCode == Sys.UI.Key.esc) 
  {
    this._blurTimer.set_enabled (false);
    this._resetValue ();
    
   this._lastKeyWasCursorUpOrDown = false;
   e.preventDefault();
   e.returnValue = false;
  }
  else if (e.keyCode == Sys.UI.Key.up) 
  {
    this._lastKeyWasCursorUpOrDown = false;
    if (Sys.UI.DomElement.getVisible(this._completionListElement) && this._selectedIndex > 0) 
    {
      this._selectedIndex--;
      var child = this._completionListElement.childNodes[this._selectedIndex];
      this._setValue (child.value, false);
      this._highlightItem (child, !this._isCompletionListItemVisible (child));
      this._lastKeyWasCursorUpOrDown = true;
      e.stopPropagation();                
      e.preventDefault();
      e.returnValue = false;
    }
  }
  else if (e.keyCode == Sys.UI.Key.down) 
  {
    this._lastKeyWasCursorUpOrDown = false;
    if (Sys.UI.DomElement.getVisible(this._completionListElement))
    {
      if (this._selectedIndex < (this._completionListElement.childNodes.length - 1)) 
      {
        this._selectedIndex++;
        var child = this._completionListElement.childNodes[this._selectedIndex];
        this._setValue (child.value, false);
        this._highlightItem (child, !this._isCompletionListItemVisible (child));
        this._lastKeyWasCursorUpOrDown = true;
        e.stopPropagation();                
        e.preventDefault();
        e.returnValue = false;
      }
    }
    else
    {
      this._showCompletionList (true, false);  
    }
  }
  else if (e.keyCode == Sys.UI.Key.enter || e.keyCode == Sys.UI.Key.tab) 
  {
    if (Sys.UI.DomElement.getVisible(this._completionListElement) && this._selectedIndex != -1) 
    {
      if (e.keyCode == Sys.UI.Key.tab)
      {
        var completionItem = this._completionListElement.childNodes[this._selectedIndex].value;
        if (completionItem.DisplayName == this._textBox.get_element().value || completionItem.DisplayName == this._oldText)
        {
          var hasChanged = completionItem.DisplayName != this._oldText || this._lastKeyWasCursorUpOrDown;
          this._setValue (completionItem, true);
          if (hasChanged)
            this._textBox._element.fireEvent ('onchange', null);
        }
        else
        {
          this._resetValue ();
        }
        e.returnValue = true;
      }
      else if (e.keyCode == Sys.UI.Key.enter)
      {
        var completionItem = this._completionListElement.childNodes[this._selectedIndex].value;
        var hasChanged = completionItem.DisplayName != this._oldText || this._lastKeyWasCursorUpOrDown;
        this._setValue (completionItem, true);
        if (hasChanged)
          this._textBox._element.fireEvent ('onchange', null);
        e.preventDefault();
        e.returnValue = false;
      }
    }
    else if (this._textBox.get_element().value.length == 0)
    {
      this._setValue (null, true);
      e.returnValue = e.keyCode == Sys.UI.Key.tab;
    }
    this._lastKeyWasCursorUpOrDown = false;
  }
  else
  {
    this._lastKeyWasCursorUpOrDown = false;
  }
  
  this._updateTimer.set_enabled (true);
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onTextBoxKeyUp ()
{
  var e = window.event;

  var isControlKey = e.keyCode < 40 
      || e.keyCode == Sys.UI.Key.Delete
      || e.keyCode == Sys.UI.Key.WindowsDelete;

  if (!isControlKey && Sys.UI.DomElement.getVisible(this._completionListElement))
  {
    var item = this._findItemByText ();
    if (item != null)
    {
      this._highlightItem (item, !this._isCompletionListItemVisible (item));
      
      if (this._enableSuggestion)
      {
        this._enableSuggestion = false;
        this._suggestionTimer.set_enabled (true);
        
        var userText = this._textBox.get_element().value; 
        this._textBox.get_element().value = item.value.DisplayName;
        var start = userText.length;
        var end = this._textBox.get_element().value.length; 
        if (true) // IE
        {
          var range = this._textBox._element.createTextRange ();
          range.moveStart ('character', start);
          range.select ();
        }
        else // MOZ
        {
          //_textBox._element.setSelectionRange(start, end); 
        }
      }
    }
  }
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_findItemByText ()
{
  var children = this._completionListElement.childNodes;
  var text = this._textBox.get_element().value.toUpperCase();
  if (text.length > 0)
  {
    var length = children.length;
    for (var i = 0; i < length; i++)
    {
      var child = children[i];
      var displayName = child.value.DisplayName.substr (0, text.length);
      if (displayName.toUpperCase() == text)
        return child;
    }
  }    
  return null;
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onTextBoxBlur () 
{
  this._updateTimer.set_enabled (false);
  if (Sys.UI.DomElement.getVisible(this._completionListElement))
  {
    this._blurTimer.set_enabled (true);
  }
  else if (this._textBox.get_element().value.length == 0)
  {
    this._oldText = null;
    this._hiddenField._element.value = '';
  }
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onMethodComplete (result, context, methodName) 
{
//  var acBehavior = context[0];
//  var prefixText = context[1];
//  acBehavior._update (prefixText, result,  true);
    this._update(context, result,  true);
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onUpdateTimerTick (sender, eventArgs) 
{
  this._showCompletionList (false, false);
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_showCompletionList (forceOpen, ignorePrefix)
{
  if (this._serviceUrl && this._serviceMethod) 
  {
    var text = this._textBox.get_element().value;
    var currentRange = document.selection.createRange();
    if (currentRange.text.length > 0)
    {
      var textBoxRange = this._textBox._element.createTextRange();
      textBoxRange.setEndPoint ('EndToStart', currentRange);
      var textBoxRangeStartIndex = textBoxRange.text.length;      
      if (textBoxRangeStartIndex > 0 && textBoxRangeStartIndex + currentRange.text.length == text.length)
        text = text.substr (0, textBoxRangeStartIndex);
    }

    text = text.toUpperCase();
    
    if (text.length < this._minimumPrefixLength) 
    {
      this._currentPrefix = text;  
      this._update ('', null,  false);
      return;
    }
    
    if (forceOpen && !ignorePrefix && this._currentPrefix == text && this._cache && this._cache[text])
    {
      this._update (text, this._cache[text],  false);
      return;
    }

    if (forceOpen || ignorePrefix || this._currentPrefix != text)
    {
      this._currentPrefix = text;
      if (ignorePrefix)
        text = '';

      if (this._cache && this._cache[text]) 
      {
        this._update (text, this._cache[text],  false);
        return;
      }
      
      var params = {
        prefixText: (ignorePrefix ? '' : this._currentPrefix),
        completionSetCount : this._completionSetCount,
        businessObjectClass : this._businessObjectClass,
        businessObjectProperty : this._businessObjectProperty,
        businessObjectID : this._businessObjectID,
        args : this._args };
      Sys.Net.WebServiceProxy.invoke(this._serviceUrl, this._serviceMethod, false, params,
                                  Function.createDelegate(this, this._onMethodComplete),
                                  Function.createDelegate(this, this._onMethodFailed));
     }
  }
}

function Remotion$UI$AutoCompleteBehavior$_get_path() {
    return this.get_ServiceUrl();
}

function Remotion$UI$AutoCompleteBehavior$get_timeout() {
    /// <value type="Number"></value>
    if (arguments.length !== 0) throw Error.parameterCount();
    return 0;
}

function Remotion$UI$AutoCompleteBehavior$_onMethodFailed(err, context, methodName) {
debugger;
        }

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_setValue (completionItem, hideCompletionList)
{
  this._updateTimer.set_enabled (false);
  var displayName = completionItem != null ? completionItem.DisplayName : '';
  this._textBox.get_element().value = displayName;
  this._currentPrefix = displayName.toUpperCase();
  this._oldText = null;
  this._hiddenField._element.value = completionItem != null ? completionItem.UniqueIdentifier : '';
  if (hideCompletionList)
    this._hideCompletionList ();
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_update (prefixText, completionItems, cacheResults) 
{
  if (cacheResults)
  {
    if (!this._cache)
      this._cache = { };
  
    this._cache[prefixText] = completionItems;
  }

  this._completionListElement.innerHTML = '';
  this._selectedIndex = -1;
  
  this._popupBehavior.set_positioningMode (AjaxControlToolkit.PositioningMode.BottomRight);
  this._popupBehavior.show ();
  this._popupBehavior.hide ();
  
  if (completionItems && completionItems.length) 
  {
    var length = completionItems.length;
    for (var i = 0; i < length; i++) 
    {
      var completionItem = completionItems[i];
      
      var itemElement = document.createElement ('div');
      itemElement.appendChild (document.createTextNode (completionItem.DisplayName));
      itemElement.__item = '';
      itemElement.value = completionItem;
      itemElement.index = i;
      
      var itemElementStyle = itemElement.style;
      itemElementStyle.padding = '1px';
      itemElementStyle.textAlign = 'left';
      itemElementStyle.textOverflow = 'ellipsis';
              
      this._completionListElement.appendChild (itemElement);
      
      if (this._selectedIndex == -1 && this._currentPrefix.length > 0)
      {
        var displayName = completionItem.DisplayName.substr (0, this._currentPrefix.length);
        if (displayName.toUpperCase() == this._currentPrefix)
        {
          itemElement.style.backgroundColor = 'highlight';
          itemElement.style.color = 'highlighttext';
          this._selectedIndex = i;
        }
      }
    }
    var elementBounds = Sys.UI.DomElement.getBounds (this._element);
    this._completionListElement.style.width = (elementBounds.width) + 'px';
    this._repositionCompletionList ();
    this._popupBehavior.show ();
    if (this._selectedIndex != -1)
    {
      var child = this._completionListElement.childNodes[this._selectedIndex];
      if (!this._isCompletionListItemVisible (child))
        child.scrollIntoView();
    }
  }
}
  
function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_repositionCompletionList ()
{
  this._popupBehavior.hide ();
  this._popupBehavior.show ();

  document.recalc (true);
  var completionListTop = this._completionListElement.offsetTop;
  var completionListLeft = this._completionListElement.offsetLeft;
  var completionListHeight = this._completionListElement.offsetHeight;
  var completionListWidth = this._completionListElement.offsetWidth;
  if (this._completionListElement.scrollHeight < this._completionListElement.clientHeight)
  {
    this._completionListElement.style.height = 'auto';
    completionListHeight = this._completionListElement.clientHeight;
  }
  this._popupBehavior.hide ();

  var totalBodyHeight = window.document.body.scrollHeight;
  var visibleBodyTop = window.document.body.scrollTop;
  var visibleBodyHeight = window.document.body.offsetHeight;
  
  var bottomOverflow = (completionListTop + completionListHeight) - (visibleBodyTop + visibleBodyHeight);
  if (bottomOverflow > 0)
  {
    var topOverflow = (completionListHeight + this._textBox._element.offsetHeight) - completionListTop;
    if (topOverflow < 0)
    {
      this._popupBehavior.set_positioningMode (AjaxControlToolkit.PositioningMode.TopRight);
    }
    else
    {
      this._popupBehavior.set_positioningMode (AjaxControlToolkit.PositioningMode.BottomRight);
    }
  }
  else
  {
    this._popupBehavior.set_positioningMode (AjaxControlToolkit.PositioningMode.BottomRight);
  }
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_isCompletionListItemVisible (element)
{
  if (element.offsetParent.clientHeight < (element.offsetTop + element.offsetHeight - element.offsetParent.scrollTop))
    return false;
  if (element.offsetTop < element.offsetParent.scrollTop)
    return false;      
  return true;
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onBlurTimerTick (sender, eventArgs) 
{
  this._blurTimer.set_enabled (false);
  
  if (this._textBox.get_element().value.length == 0)
    this._setValue (null, true);
  else if (this._oldText != null || (Sys.UI.DomElement.getVisible(this._completionListElement) && this._selectedIndex == -1))
    this._resetValue ();
  else
    this._hideCompletionList ();  
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onCompletionListFocus () 
{
  this._blurTimer.set_enabled (false);
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onCompletionListBlur () 
{
  this._blurTimer.set_enabled (true);
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onDropDownImageMouseDown () 
{
  this._blurTimer.set_enabled (false);
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_resetValue ()
{
  this._textBox.get_element().value = (this._oldText != null ? this._oldText : '');
  this._removeSelection ();
  this._currentPrefix = this._oldText != null ? this._oldText.toUpperCase() : '';
  this._oldText = null;
  this._hideCompletionList ();  
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_removeSelection ()
{
  var start = this._textBox.get_element().value.length;
  var end = start;
  if (true) // IE
  {
    var range = this._textBox._element.createTextRange ();
    range.moveStart ('character', start);
    range.select ();
  }
  else // MOZ
  {
    //_textBox._element.setSelectionRange(start, end); 
  }
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onDropDownImageClick (sender, eventArgs) 
{
  if (Sys.UI.DomElement.getVisible(this._completionListElement))
  {
    this._hideCompletionList ();  
  }
  else
  {
    this._textBox.get_element().focus ();
    this._showCompletionList (true, true);
  }
}

function Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onSuggestionTimerTick (sender, eventArgs) 
{
  this._enableSuggestion = true;
  this._suggestionTimer.set_enabled (false);
}

Remotion.UI.BocQuirksModeAutoCompleteReferenceValueBehavior.prototype = {
    
  _appUrl : null,
  _serviceUrl : null,
  _serviceMethod : null,
  _minimumPrefixLength : 0,
  _completionSetCount : null,
  _businessObjectClass : null,
  _businessObjectProperty : null,
  _businessObjectID : null,
  _args : null,
  _dropDownPanelClass : null,
  _completionInterval : 1000,
  _suggestionInterval : 200,
  _completionListElement : null,
  _textBox : null,
  _hiddenField : null,
  _popupBehavior : null,
  _dropDownImage : null,
  _dropDownImageClickBehavior : null,
  _popUpControl : null,
  
  _updateTimer : null,
  _blurTimer : null,
  _suggestionTimer : null,
  _cache : null,
  _currentPrefix : null,
  _selectedIndex : -1,
  _oldText : null,
  _enableSuggestion : true,
  _lastKeyWasCursorUpOrDown : false,
  
  _textBoxFocusHandler : null,
  _textBoxBlurHandler : null,
  _textBoxKeyDownHandler : null,
  _textBoxKeyUpHandler : null,
  _completionListFocusHandler : null,
  _completionListBlurHandler : null,
  _completionListMouseDownHandler : null,
  _completionListMouseUpHandler : null,
  _completionListMouseOverHandler : null,
  _updateTimerTickHandler : null,
  _blurTimerTickHandler : null,
  _suggestionTimerTickHandler : null,
  _dropDownImageClickHandler : null,
  _dropDownImageMouseDownHandler : null,

  get_CompletionInterval: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_CompletionInterval,
  set_CompletionInterval: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_CompletionInterval,

  get_SuggestionInterval: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_SuggestionInterval,
  set_SuggestionInterval: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_SuggestionInterval,

  get_CompletionList: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_CompletionList,
  set_CompletionList: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_CompletionList,

  get_CompletionSetCount: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_CompletionSetCount,
  set_CompletionSetCount: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_CompletionSetCount,

  get_BusinessObjectClass: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_BusinessObjectClass,
  set_BusinessObjectClass: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_BusinessObjectClass,

  get_BusinessObjectProperty: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_BusinessObjectProperty,
  set_BusinessObjectProperty: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_BusinessObjectProperty,

  get_BusinessObjectID: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_BusinessObjectID,
  set_BusinessObjectID: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_BusinessObjectID,

  get_Args: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_Args,
  set_Args: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_Args,

  get_DropDownPanelClass: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_DropDownPanelClass,
  set_DropDownPanelClass: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_DropDownPanelClass,

  get_MinimumPrefixLength: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_MinimumPrefixLength,
  set_MinimumPrefixLength: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_MinimumPrefixLength,

  get_ServiceMethod: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_ServiceMethod,
  set_ServiceMethod: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_ServiceMethod,

  get_ServiceUrl: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_ServiceUrl,
  set_ServiceUrl: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_ServiceUrl,
  get_timeout: Remotion$UI$AutoCompleteBehavior$get_timeout,

  get_TextBoxID: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_TextBoxID,
  set_TextBoxID: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_TextBoxID,

  get_HiddenFieldID: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_HiddenFieldID,
  set_HiddenFieldID: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_HiddenFieldID,

  get_appUrl: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$get_appUrl,
  set_appUrl: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$set_appUrl,

  dispose: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$dispose,
  initialize: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$initialize,

  _hideCompletionList: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_hideCompletionList,
  _highlightItem: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_highlightItem,
  _onCompletionListMouseDown: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onCompletionListMouseDown,
  _onCompletionListMouseUp: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onCompletionListMouseUp,
  _onCompletionListMouseOver: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onCompletionListMouseOver,
  _onTextBoxFocus: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onTextBoxFocus,
  _onTextBoxKeyDown: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onTextBoxKeyDown,
  _onTextBoxKeyUp: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onTextBoxKeyUp,
  _findItemByText: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_findItemByText,
  _onTextBoxBlur: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onTextBoxBlur,
  _onMethodComplete: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onMethodComplete,
  _onMethodFailed: Remotion$UI$AutoCompleteBehavior$_onMethodFailed,
  _get_path: Remotion$UI$AutoCompleteBehavior$_get_path,
  _onUpdateTimerTick: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onUpdateTimerTick,
  _showCompletionList: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_showCompletionList,
  _setValue: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_setValue,
  _update: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_update,
  _repositionCompletionList: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_repositionCompletionList,
  _isCompletionListItemVisible: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_isCompletionListItemVisible,
  _onBlurTimerTick: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onBlurTimerTick,
  _onCompletionListFocus: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onCompletionListFocus,
  _onCompletionListBlur: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onCompletionListBlur,
  _onDropDownImageMouseDown: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onDropDownImageMouseDown,
  _resetValue: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_resetValue,
  _removeSelection: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_removeSelection,
  _onDropDownImageClick: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onDropDownImageClick,
  _onSuggestionTimerTick: Remotion$UI$BocQuirksModeAutoCompleteReferenceValueBehavior$_onSuggestionTimerTick
}

Remotion.UI.BocQuirksModeAutoCompleteReferenceValueBehavior.descriptor = {
    properties: [   {name: 'CompletionInterval', type: Number},
                    {name: 'SuggestionInterval', type: Number},
                    {name: 'CompletionList', type: Sys.UI.DomElement},
                    {name: 'CompletionSetCount', type: Number},
                    {name: 'BusinessObjectClass', type: String},
                    {name: 'BusinessObjectProperty', type: String},
                    {name: 'BusinessObjectID', type: String},
                    {name: 'Args', type: String},
                    {name: 'DropDownPanelClass', type: String},
                    {name: 'MinimumPrefixLength', type: Number},
                    {name: 'ServiceMethod', type: String},
                    {name: 'ServiceUrl', type: String},
                    {name: 'TextBoxID', type: String},
                    {name: 'HiddenFieldID', type: String},
                    {name: 'appUrl', type: String} ]
}
Remotion.UI.BocQuirksModeAutoCompleteReferenceValueBehavior.registerClass('Remotion.UI.BocQuirksModeAutoCompleteReferenceValueBehavior', Sys.UI.Behavior);
