// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

// The context contains all information required by the smart page.
// theFormID: The ID of the HTML Form on the page.
// isDirtyStateTrackingEnabled: true if the page should watch the form-fields for changes.
// isDirty: true if the page is dirty (client or server-side)
// abortMessage: The message displayed when the user attempts to leave the page. null to disable the message.
// statusIsSubmittingMessage: The message displayed when the user attempts to submit while a submit is already in 
//    progress. null to disable the message.
// smartScrollingFieldID: The ID of the hidden field containing the smart scrolling data.
// smartFocusFieldID: The ID of the hidden field containing the smart focusing data.
// checkFormStateFunctionName: The name of the function used to evaluate whether to submit the form.
//    null if no external logic should be incorporated.
// eventHandlers: The hashtable of eventhandlers: Hashtable < event-key, Array < event-handler > >
function SmartPage_Context(
    theFormID,
    isDirtyStateTrackingEnabled,
    abortMessage, statusIsSubmittingMessage,
    smartScrollingFieldID, smartFocusFieldID,
    checkFormStateFunctionName)
{
  ArgumentUtility.CheckNotNullAndTypeIsString('theFormID', theFormID);
  ArgumentUtility.CheckNotNullAndTypeIsBoolean('isDirtyStateTrackingEnabled', isDirtyStateTrackingEnabled);
  ArgumentUtility.CheckTypeIsString('abortMessage', abortMessage);
  ArgumentUtility.CheckTypeIsString('statusIsSubmittingMessage', statusIsSubmittingMessage);
  ArgumentUtility.CheckTypeIsString('smartScrollingFieldID', smartScrollingFieldID);
  ArgumentUtility.CheckTypeIsString('smartFocusFieldID', smartFocusFieldID);
  ArgumentUtility.CheckTypeIsString('checkFormStateFunctionName', checkFormStateFunctionName);

  var _theForm;

  var _isDirtyStateTrackingEnabled = isDirtyStateTrackingEnabled;
  var _isDirty = false;

  // The message displayed when the user attempts to leave the page.
  // null to disable the message.
  var _abortMessage = abortMessage;
  var _isAbortConfirmationEnabled = abortMessage != null;

  var _isSubmitting = false;
  var _hasSubmitted = false;
  // Special flag to support the OnBeforeUnload part
  var _isSubmittingBeforeUnload = false;
  // The message displayed when the user attempts to submit while a submit is already in progress.
  // null to disable the message.
  var _statusIsSubmittingMessage = statusIsSubmittingMessage;

  var _isAborting = false;
  var _isCached = false;
  // Special flag to support the OnBeforeUnload part
  var _isAbortingBeforeUnload = false;

  // The name of the function used to evaluate whether to submit the form.
  // null if no external logic should be incorporated.
  var _checkFormStateFunctionName = checkFormStateFunctionName;

  var _statusMessageWindow = null;
  var _hasUnloaded = false;
  var _isMsIEAspnetPostBack = false;
  var _isMsIEFormClicked = false;

  var _aspnetFormOnSubmit = null;
  var _aspnetDoPostBack = null;
  // Sepcial flag to support the Form.OnSubmit event being executed by the ASP.NET __doPostBack function.
  var _isExecutingDoPostBack = false;

  // The hidden field containing the smart scrolling data.
  var _smartScrollingFieldID = null;
  // The hidden field containing the smart focusing data.
  var _smartFocusFieldID = null;

  var _activeElement = null;
  // The hashtable of eventhandlers: Hashtable < event-key, Array < event-handler > >
  var _eventHandlers = new Array();
  var _trackedIDs = new Array();
  var _synchronousPostBackCommands = new Array();

  var _isMsIE = !isNaN(BrowserUtility.GetIEVersion());
  var _cacheStateHasSubmitted = 'hasSubmitted';
  var _cacheStateHasLoaded = 'hasLoaded';

  var _loadHandler = function () { SmartPage_Context.Instance.OnLoad(); };
  var _beforeUnloadHandler = function () { return SmartPage_Context.Instance.OnBeforeUnload(); };
  var _unloadHandler = function () { return SmartPage_Context.Instance.OnUnload(); };
  var _scrollHandler = function () { SmartPage_Context.Instance.OnScroll(); };
  var _resizeHandler = function () { SmartPage_Context.Instance.OnResize(); };
  var _formSubmitHandler = function () { return SmartPage_Context.Instance.OnFormSubmit(); };
  var _formClickHandler = function (evt) { return SmartPage_Context.Instance.OnFormClick(evt); };
  var _doPostBackHandler = function (eventTarget, eventArg) { SmartPage_Context.Instance.DoPostBack(eventTarget, eventArg); };
  var _valueChangedHandler = function (evt) { SmartPage_Context.Instance.OnValueChanged(evt); };
  var _elementFocusHandler = function (evt) { SmartPage_Context.Instance.OnElementFocus(evt); };
  var _elementBlurHandler = function (evt) { SmartPage_Context.Instance.OnElementBlur(evt); };

  this.Init = function ()
  {
    _theForm = window.document.forms[theFormID];
    {
      if (_theForm == null)
        throw ('"' + theFormID + '" does not specify a Form.');
    }

    if (smartScrollingFieldID != null)
    {
      if (_theForm.elements[smartScrollingFieldID] == null)
        throw ('"' + smartScrollingFieldID + '" does not specify a element of Form "' + _theForm.id + '".');
    }

    if (smartFocusFieldID != null)
    {
      if (_theForm.elements[smartFocusFieldID] == null)
        throw ('"' + smartFocusFieldID + '" does not specify a element of Form "' + _theForm.id + '".');
    }

    _smartScrollingFieldID = smartScrollingFieldID;
    _smartFocusFieldID = smartFocusFieldID;

    AttachPageLevelEventHandlers();
  };

  this.set_EventHandlers = function (eventHandlers)
  {
    ArgumentUtility.CheckTypeIsObject('eventHandlers', eventHandlers);
    _eventHandlers = eventHandlers;
  };

  this.set_TrackedIDs = function (trackedIDs)
  {
    ArgumentUtility.CheckTypeIsObject('trackedIDs', trackedIDs);
    _trackedIDs = trackedIDs;
  };

  this.set_SynchronousPostBackCommands = function (synchronousPostBackCommands)
  {
    ArgumentUtility.CheckTypeIsObject('synchronousPostBackCommands', synchronousPostBackCommands);
    _synchronousPostBackCommands = synchronousPostBackCommands;
  };

  // Attaches the event handlers to the page's events.
  function AttachPageLevelEventHandlers()
  {
    RemoveEventHandler(window, 'load', _loadHandler);
    AddEventHandler(window, 'load', _loadHandler);

    // IE, Mozilla 1.7, Firefox 0.9
    window.onbeforeunload = _beforeUnloadHandler;

    window.onunload = _unloadHandler;

    RemoveEventHandler(window, 'scroll', _scrollHandler);
    AddEventHandler(window, 'scroll', _scrollHandler);

    PageUtility.Instance.RegisterResizeHandler('#' + _theForm.id, _resizeHandler);

    _aspnetFormOnSubmit = _theForm.onsubmit;
    _theForm.onsubmit = _formSubmitHandler;
    _theForm.onclick = _formClickHandler;
    if (TypeUtility.IsDefined(window.__doPostBack))
    {
      _aspnetDoPostBack = window.__doPostBack;
      window.__doPostBack = _doPostBackHandler;
    }
  };


  // Called after page's html content is complete.
  // Used to perform initalization code that only requires complete the HTML source but not necessarily all images.
  this.OnStartUp = function (isAsynchronous, isDirty)
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isAsynchronous', isAsynchronous);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isDirty', isDirty);

    _isDirty = isDirty;

    if (_isDirtyStateTrackingEnabled)
      AttachDataChangedEventHandlers();

    AttachFocusEventHandlers(window.document.body);

    if (TypeUtility.IsDefined(window.Sys) && TypeUtility.IsDefined(Sys.WebForms) && TypeUtility.IsDefined(Sys.WebForms.PageRequestManager))
    {
      Sys.WebForms.PageRequestManager.prototype._updatePanel = Sys$WebForms$PageRequestManager$_updatePanel;
      Sys.WebForms.PageRequestManager.getInstance().add_endRequest(SmartPage_PageRequestManager_endRequest);
    }
  };

  // Replacement for Sys.WebForms.PageRequestManager.prototype._updatePanel
  function Sys$WebForms$PageRequestManager$_updatePanel(updatePanelElement, rendering)
  {
    for (var updatePanelID in this._scriptDisposes)
    {
      if (this._elementContains(updatePanelElement, document.getElementById(updatePanelID)))
      {
        var disposeScripts = this._scriptDisposes[updatePanelID];
        for (var i = 0, l = disposeScripts.length; i < l; i++)
        {
          eval(disposeScripts[i]);
        }
        delete this._scriptDisposes[updatePanelID];
      }
    }
    if (TypeUtility.IsDefined(this._destroyTree)) // .NET 3.5 AJAX library
    {
      this._destroyTree(updatePanelElement);
    }
    else if (TypeUtility.IsDefined(Sys.Application.disposeElement)) // .NET 4.0 AJAX library
    {
      Sys.Application.disposeElement(updatePanelElement, true);
    }
    else
    {
      throw "Unsupported AJAX library detected."
    }
    $(updatePanelElement).empty().append(rendering);
  }

  function SmartPage_PageRequestManager_endRequest(sender, args)
  {
    if (args.get_error() != undefined && args.get_error().httpStatusCode == '500')
    {
      var errorMessage = args.get_error().message;
      args.set_errorHandled(true);

      var errorBody = '<div class="SmartPageErrorBody"><div>' + errorMessage + '</div></div>';

      SmartPage_Context.Instance.ShowMessage("SmartPageServerErrorMessage", errorBody);
    }
  }

  // Attached the OnValueChanged event handler to all form data elements listed in _trackedIDs.
  function AttachDataChangedEventHandlers()
  {
    for (var i = 0; i < _trackedIDs.length; i++)
    {
      var id = _trackedIDs[i];
      var element = _theForm.elements[id];
      if (element == null)
        continue;

      var tagName = element.tagName.toLowerCase();

      if (tagName == 'input')
      {
        var type = element.type.toLowerCase();
        if (type == 'text' || type == 'hidden')
        {
          RemoveEventHandler(element, 'change', _valueChangedHandler);
          AddEventHandler(element, 'change', _valueChangedHandler);
        }
        else if (type == 'checkbox' || type == 'radio')
        {
          RemoveEventHandler(element, 'click', _valueChangedHandler);
          AddEventHandler(element, 'click', _valueChangedHandler);
        }
      }
      else if (tagName == 'textarea' || tagName == 'select')
      {
        RemoveEventHandler(element, 'change', _valueChangedHandler);
        AddEventHandler(element, 'change', _valueChangedHandler);
      }
    }
  };

  // Event handler attached to the change event of tracked form elements
  this.OnValueChanged = function ()
  {
    _isDirty = true;
  };

  // Attaches the event handlers to the OnFocus and OnBlur events.
  function AttachFocusEventHandlers(currentElement)
  {
    if (_isMsIE)
    {
      //  Special work-around for not accessing window.document.activeElement during page load in IE, 
      //  since this throws an error if the page is inside an iframe
      return;
    }
    else if (TypeUtility.IsUndefined(window.document.activeElement))
    {
      if (currentElement == null)
        return;

      if (TypeUtility.IsDefined(currentElement.id) && !StringUtility.IsNullOrEmpty(currentElement.id)
        && IsFocusableTag(currentElement.tagName))
      {
        currentElement.onfocus = _elementFocusHandler;
        currentElement.onblur = _elementBlurHandler;
      }

      for (var i = 0; i < currentElement.childNodes.length; i++)
      {
        var element = currentElement.childNodes[i];
        AttachFocusEventHandlers(element);
      }
    }
    else if ($.browser.webkit)
    {
      // webkit does not set focus on hyperlink-clicks
      $('a').each(function ()
      {
        RemoveEventHandler(this, 'mousedown', _elementFocusHandler);
        AddEventHandler(this, 'mousedown', _elementFocusHandler);
      });
    }
  };

  //  Gets the element that caused the current event.
  this.GetActiveElement = function ()
  {
    try
    {
      if (TypeUtility.IsDefined(window.document.activeElement) && window.document.activeElement != null
          && window.document.body != window.document.activeElement && (jQuery('body').find(window.document.activeElement).length == 1)
          && TypeUtility.IsDefined(window.document.activeElement.tagName) && IsFocusableTag(window.document.activeElement.tagName))
      {
        _activeElement = window.document.activeElement;
      }
    }
    catch (e)
    {
    }

    if (_activeElement != null
        && (TypeUtility.IsUndefined(_activeElement.parentNode) || _activeElement.parentNode == null))
    {
      _activeElement = null;
    }

    return _activeElement;
  };

  //  Sets the element that caused the current event.
  this.SetActiveElement = function (value)
  {
    _activeElement = value;
  };

  // Backs up the smart scrolling and smart focusing data for the next post back.
  this.Backup = function ()
  {
    if (_smartScrollingFieldID != null)
      _theForm.elements[_smartScrollingFieldID].value = SmartScrolling_Backup(this.GetActiveElement());
    if (_smartFocusFieldID != null)
      _theForm.elements[_smartFocusFieldID].value = SmartFocus_Backup(GetFocusableElement(this.GetActiveElement()));
  };

  // Restores the smart scrolling and smart focusing data from the previous post back.
  this.Restore = function ()
  {
    if (_smartScrollingFieldID != null)
      SmartScrolling_Restore(_theForm.elements[_smartScrollingFieldID].value);
    if (_smartFocusFieldID != null)
      SmartFocus_Restore(_theForm.elements[_smartFocusFieldID].value);
  };

  // Event handler for window.OnLoad
  this.OnLoad = function ()
  {
    if (TypeUtility.IsDefined(window.Sys) && TypeUtility.IsDefined(Sys.WebForms) && TypeUtility.IsDefined(Sys.WebForms.PageRequestManager))
    {
      Sys.WebForms.PageRequestManager.getInstance().remove_pageLoaded(SmartPage_PageRequestManager_pageLoaded);
      Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(SmartPage_PageRequestManager_pageLoaded);
    }

    var isAsynchronous = false;
    this.PageLoaded(isAsynchronous);
  };

  this.PageRequestManager_pageLoaded = function (sender, args)
  {
    var isAsynchronous = sender && sender.get_isInAsyncPostBack();
    if (isAsynchronous)
    {
      this.PageLoaded(isAsynchronous);
    }
  };

  this.PageLoaded = function (isAsynchronous)
  {
    if (!isAsynchronous)
      this.CheckIfCached();

    this.Restore();

    _isSubmitting = false;
    _isSubmittingBeforeUnload = false;
    this.HideStatusMessage();

    ExecuteEventHandlers(_eventHandlers['onload'], _hasSubmitted, _isCached, isAsynchronous);
  };

  // Determines whether the page was loaded from cache.
  this.CheckIfCached = function ()
  {
    var field = _theForm.SmartPage_CacheDetectionField;
    if (field.value == _cacheStateHasSubmitted)
    {
      _hasSubmitted = true;
      _isCached = true;
    }
    else if (field.value == _cacheStateHasLoaded)
    {
      _isCached = true;
    }
    else
    {
      this.SetCacheDetectionFieldLoaded();
    }
  };

  // Marks the page as loaded.
  this.SetCacheDetectionFieldLoaded = function ()
  {
    var field = _theForm.SmartPage_CacheDetectionField;
    field.value = _cacheStateHasLoaded;
  };

  // Marks the page as submitted.
  this.SetCacheDetectionFieldSubmitted = function ()
  {
    var field = _theForm.SmartPage_CacheDetectionField;
    field.value = _cacheStateHasSubmitted;
  };

  // Event handler for window.OnBeforeUnload.
  // __doPostBack
  // {
  //   Form.submit()
  //   {
  //     OnBeforeUnload()
  //   } 
  // }
  // Wait For Response
  // OnUnload()
  this.OnBeforeUnload = function ()
  {
    console.log (window.location.href + " OnBeforeUnload");
    _isAbortingBeforeUnload = false;
    var displayAbortConfirmation = false;

    if (!_hasUnloaded
        && !_isCached
        && !_isSubmittingBeforeUnload
        && !_isAborting && _isAbortConfirmationEnabled)
    {
      var activeElement = this.GetActiveElement();
      var isJavaScriptAnchor = IsJavaScriptAnchor(activeElement);
      var isAbortConfirmationRequired = !isJavaScriptAnchor
                                        && (!_isDirtyStateTrackingEnabled || _isDirty);

      if (isAbortConfirmationRequired)
      {
        _isAbortingBeforeUnload = true;
        displayAbortConfirmation = true;
      }
    }
    else if (_isSubmittingBeforeUnload)
    {
      _isSubmittingBeforeUnload = false;
    }

    ExecuteEventHandlers(_eventHandlers['onbeforeunload']);
    if (displayAbortConfirmation)
    {
      // IE alternate/official version: window.event.returnValue = SmartPage_Context.Instance.AbortMessage
      return _abortMessage;
    }
  };

  // Event handler for window.OnUnload.
  this.OnUnload = function ()
  {
    if ((!_isSubmitting || _isAbortingBeforeUnload)
        && !_isAborting)
    {
      _isAborting = true;
      ExecuteEventHandlers(_eventHandlers['onabort'], _hasSubmitted, _isCached);
      _isAbortingBeforeUnload = false;
    }
    ExecuteEventHandlers(_eventHandlers['onunload']);
    _hasUnloaded = true;
    _isSubmitting = false;
    _isAborting = false;

    _theForm = null;
    _activeElement = null;

    _loadHandler = null;
    _beforeUnloadHandler = null;
    _unloadHandler = null;
    _scrollHandler = null;
    _resizeHandler = null;
    _formSubmitHandler = null;
    _formClickHandler = null;
    _doPostBackHandler = null;
    _valueChangedHandler = null;
    _elementFocusHandler = null;
    _elementBlurHandler = null;

  };

  // Override for the ASP.NET __doPostBack method.
  this.DoPostBack = function (eventTarget, eventArgument)
  {
    console.log(window.location.href + " DoPostBack");
    var eventSource = document.getElementById(UniqueIDToClientID(eventTarget));
    this.SetActiveElement(eventSource);
    var _this = this;
    setTimeout(function () { _this.DoPostBackInternal(eventTarget, eventArgument); }, 0);
  };

  this.DoPostBackInternal = function (eventTarget, eventArgument)
  {
    console.log(window.location.href + " DoPostBackInternal");
    // Debugger space
    var dummy = 0;
    var continueRequest = this.CheckFormState();
    if (continueRequest)
    {
      _isSubmitting = true;
      _isSubmittingBeforeUnload = true;

      this.Backup();

      ExecuteEventHandlers(_eventHandlers['onpostback'], eventTarget, eventArgument);
      this.SetCacheDetectionFieldSubmitted();

      _isExecutingDoPostBack = true;
      _theForm.__EVENTTARGET.value = eventTarget;
      _theForm.__EVENTARGUMENT.value = eventArgument;
      _aspnetDoPostBack(eventTarget, eventArgument);
      _theForm.__EVENTTARGET.value = '';
      _theForm.__EVENTARGUMENT.value = '';
      _isExecutingDoPostBack = false;

      if (_isMsIE)
      {
        if (!_isMsIEFormClicked)
          _isMsIEAspnetPostBack = true;
        _isMsIEFormClicked = false;
      }
    }
  };

  // Event handler for Form.Submit.
  this.OnFormSubmit = function ()
  {
    console.log(window.location.href + " OnFormSubmit");
    if (_isExecutingDoPostBack)
    {
      if (_aspnetFormOnSubmit != null)
        return _aspnetFormOnSubmit();
      else
        return true;
    }
    else
    {
      var eventSource = this.GetEventTarget();
      if (eventSource == null)
      {
        eventSource = this.GetActiveElement();
        eventSource = GetFocusableElement(eventSource);
      }
      var eventSourceID = (eventSource != null) ? eventSource.id : null;

      if (this.CheckIsAsyncPostback(eventSource))
      {
        if (this.IsSynchronousPostBackRequired(eventSourceID, _theForm.__EVENTARGUMENT.value))
        {
          this.DoPostBackInternal(_theForm.__EVENTTARGET.value, _theForm.__EVENTARGUMENT.value);
          return false;
        }
        else
        {
          var continueRequest = this.CheckFormState();
          if (continueRequest)
          {
            _isSubmitting = true;
            _isSubmittingBeforeUnload = true;

            this.Backup();
            ExecuteEventHandlers(_eventHandlers['onpostback'], _theForm.__EVENTTARGET.value, _theForm.__EVENTARGUMENT.value);
            return true;
          }
          else
          {
            return false;
          }
        }
      }

      var continueRequest = this.CheckFormState();
      if (continueRequest)
      {
        _isSubmitting = true;
        _isSubmittingBeforeUnload = true;

        this.Backup();

        ExecuteEventHandlers(_eventHandlers['onpostback'], eventSourceID, '');
        this.SetCacheDetectionFieldSubmitted();

        if (_aspnetFormOnSubmit != null)
          return _aspnetFormOnSubmit();
        else
          return true;
      }
      else
      {
        return false;
      }
    }
  };

  // Event handler for Form.OnClick.
  this.OnFormClick = function (evt)
  {
    console.log(window.location.href + " OnFormClick");
    var eventSource = eventSource = GetEventSource(evt);
    this.SetActiveElement(eventSource);
    eventSource = GetFocusableElement(eventSource);

    var eventSourceID = (eventSource != null) ? eventSource.id : null;

    if (this.CheckIsAsyncPostback(eventSource) && this.IsSynchronousPostBackRequired(eventSourceID, _theForm.__EVENTARGUMENT.value))
      return true;

    if (_isMsIE)
    {
      if (_isMsIEAspnetPostBack)
      {
        _isMsIEFormClicked = false;
        _isMsIEAspnetPostBack = false;
        return void (0);
      }
      else
      {
        _isMsIEFormClicked = true;
      }
    }

    if (IsJavaScriptAnchor(eventSource))
    {
      var continueRequest = this.CheckFormState();
      if (!continueRequest)
        return false;
      else
        return void (0);
    }
    else
    {
      return void (0);
    }
  };

  // returns: true to continue with request.
  this.CheckFormState = function ()
  {
    var continueRequest = true;
    var fct = null;
    if (_checkFormStateFunctionName != null)
      fct = GetFunctionPointer(_checkFormStateFunctionName);
    if (fct != null)
    {
      try
      {
        continueRequest = fct(_isAborting, _hasSubmitted, _hasUnloaded, _isCached);
      }
      catch (e)
      {
      }
    }

    if (!continueRequest)
    {
      return false;
    }
    else if (_isSubmitting)
    {
      this.ShowStatusIsSubmittingMessage();
      return false;
    }
    else
    {
      return true;
    }
  };

  this.CheckIsAsyncPostback = function (element)
  {
    if (element == null)
      return false;

    if (this.IsAsyncPostback(element))
      return true;

    return false;
  };

  this.IsAsyncPostback = function (element)
  {
    ArgumentUtility.CheckNotNull('element', element);

    if (TypeUtility.IsUndefined(window.Sys) || TypeUtility.IsUndefined(Sys.WebForms) || TypeUtility.IsUndefined(Sys.WebForms.PageRequestManager))
      return false;

    var postbackSettings = GetPostbackSettings(Sys.WebForms.PageRequestManager.getInstance(), element);
    return postbackSettings.async;
  };

  function GetPostbackSettings(pageRequestManager, element)
  {
    ArgumentUtility.CheckNotNull('pageRequestManager', pageRequestManager);
    ArgumentUtility.CheckNotNull('element', element);
    if ((element.id == null) || (element.id.length == 0))
      return CreatePostbackSettings(false, null);


    var updatePanelIDs = pageRequestManager._updatePanelIDs;
    for (var i = updatePanelIDs.length - 1; i >= 0; i--)
    {
      var updatePanel = document.getElementById(UniqueIDToClientID(updatePanelIDs[i]));
      if (updatePanel != null && jQuery(updatePanel).find('#' + element.id).length == 1)
        return CreatePostbackSettings(true, updatePanelIDs[i]);
    }

    var asyncPostbackControlIDs = pageRequestManager._asyncPostBackControlIDs;
    for (var i = 0; i < asyncPostbackControlIDs.length; i++)
    {
      var asyncPostbackControl = document.getElementById(asyncPostbackControlIDs[i]);
      if (element == asyncPostbackControl)
        return CreatePostbackSettings(true, pageRequestManager._scriptManagerID);
    }

    return CreatePostbackSettings(false, null);
  }

  function CreatePostbackSettings(async, panelID, sourceElement)
  {
    return { async: async, panelID: panelID, sourceElement: sourceElement };
  }

  // Event handler for Window.OnScroll.
  this.OnScroll = function ()
  {
    if (_statusMessageWindow != null)
      AlignStatusMessage(_statusMessageWindow);
    ExecuteEventHandlers(_eventHandlers['onscroll']);
  };

  // Event handler for Window.OnResize.
  this.OnResize = function ()
  {
    if (_statusMessageWindow != null)
      AlignStatusMessage(_statusMessageWindow);
    ExecuteEventHandlers(_eventHandlers['onresize']);
  };

  // Sends an AJAX request to the server. Fallback to the load-image technique.
  this.SendOutOfBandRequest = function (url)
  {
    ArgumentUtility.CheckNotNullAndTypeIsString('url', url);
    try
    {
      var xhttp;
      if (TypeUtility.IsDefined(window.XMLHttpRequest))
        xhttp = new XMLHttpRequest();
      else
        xhttp = new ActiveXObject('Microsoft.XMLHTTP');

      var method = 'GET';
      var isSynchronousCall = false;
      xhttp.open(method, url, isSynchronousCall);
      xhttp.send();
    }
    catch (e)
    {
      try
      {
        var image = new Image();
        image.src = url;
      }
      catch (e)
      {
      }
    }
  };

  function AddEventHandler(object, eventType, handler)
  {
    $(object).bind(eventType, handler);
    return;
  };

  function RemoveEventHandler(object, eventType, handler)
  {
    $(object).unbind(eventType, handler);
    return;
  }

  // Executes the event handlers.
  // eventHandlers: an array of event handlers.
  function ExecuteEventHandlers(eventHandlers)
  {
    if (eventHandlers == null)
      return;

    for (var i = 0; i < eventHandlers.length; i++)
    {
      var eventHandler = GetFunctionPointer(eventHandlers[i]);
      if (eventHandler != null)
      {
        var arg1 = null;
        var arg2 = null;
        var args = ExecuteEventHandlers.arguments;

        if (args.length > 1)
          arg1 = args[1];
        if (args.length > 2)
          arg2 = args[2];

        try
        {
          eventHandler(arg1, arg2);
        }
        catch (e)
        {
        }
      }
    }
  };

  // Evaluates the string and returns the specified function.
  // A String pointing to a valid function.
  // Returns: The function or null if the function could not be found.
  function GetFunctionPointer(functionName)
  {
    ArgumentUtility.CheckTypeIsString('functionName', functionName);
    if (StringUtility.IsNullOrEmpty(functionName))
      return null;
    var fct = null;
    try
    {
      fct = eval(functionName);
    }
    catch (e)
    {
      return null;
    }
    if (TypeUtility.IsFunction(fct))
      return fct;
    else
      return null;
  };

  // Shows the status message informing the user that the page is already submitting.
  this.ShowStatusIsSubmittingMessage = function ()
  {
    if (_statusIsSubmittingMessage != null)
      this.ShowMessage('SmartPageStatusIsSubmittingMessage', _statusIsSubmittingMessage);
  };

  //  Shows a status message in the window using a DIV
  this.ShowMessage = function (id, message)
  {
    ArgumentUtility.CheckNotNullAndTypeIsString('id', id);
    ArgumentUtility.CheckNotNullAndTypeIsString('message', message);

    if (_statusMessageWindow == null)
    {
      var statusMessageWindow;
      var statusMessageBlock;
      if (_isMsIE)
      {
        statusMessageWindow = window.document.createElement('DIV');

        var iframe = window.document.createElement('IFRAME');
        iframe.src = 'javascript:false;';
        iframe.style.width = '100%';
        iframe.style.height = '100%';
        iframe.style.left = '0';
        iframe.style.top = '0';
        iframe.style.position = 'absolute';
        iframe.style.filter = 'progid:DXImageTransform.Microsoft.Alpha(style=0,opacity=0)';
        iframe.style.border = 'none';
        statusMessageWindow.appendChild(iframe);

        statusMessageBlock = window.document.createElement('DIV');
        statusMessageBlock.style.width = '100%';
        statusMessageBlock.style.height = '100%';
        statusMessageBlock.style.left = '0';
        statusMessageBlock.style.top = '0';
        statusMessageBlock.style.position = 'absolute';
        statusMessageWindow.appendChild(statusMessageBlock);
      }
      else
      {
        statusMessageWindow = window.document.createElement('DIV');
        statusMessageBlock = statusMessageWindow;
      }

      statusMessageWindow.id = id;
      statusMessageWindow.style.width = '50%';
      statusMessageWindow.style.height = '50%';
      statusMessageWindow.style.left = '25%';
      statusMessageWindow.style.top = '25%';
      statusMessageWindow.style.position = 'absolute';
      statusMessageBlock.innerHTML =
            '<table style="border:none; height:100%; width:100%"><tr><td style="text-align:center;">'
          + message
          + '</td></tr></table>';

      window.document.body.appendChild(statusMessageWindow, _theForm);
      AlignStatusMessage(statusMessageWindow);
      _statusMessageWindow = statusMessageWindow;
    }
  };

  // (Re-)Aligns the status message in the window.
  function AlignStatusMessage(message)
  {
    ArgumentUtility.CheckNotNullAndTypeIsObject('message', message);

    var scrollLeft = window.document.body.scrollLeft;
    var scrollTop = window.document.body.scrollTop;
    var windowWidth = window.document.body.clientWidth;
    var windowHeight = window.document.body.clientHeight;

    message.style.left = windowWidth / 2 - message.offsetWidth / 2 + scrollLeft;
    message.style.top = windowHeight / 2 - message.offsetHeight / 2 + scrollTop;
  };

  this.HideStatusMessage = function ()
  {
    if (_statusMessageWindow != null)
    {
      window.document.body.removeChild(_statusMessageWindow);
      _statusMessageWindow = null;
    }
  };

  // Determines whether the elements of the specified tag can receive the focus.
  function IsFocusableTag(tagName)
  {
    ArgumentUtility.CheckTypeIsString('tagName', tagName);
    if (StringUtility.IsNullOrEmpty(tagName))
      return false;
    tagName = tagName.toLowerCase();
    return (tagName == 'a' ||
            tagName == 'button' ||
            tagName == 'input' ||
            tagName == 'textarea' ||
            tagName == 'select');
  };

  function GetFocusableElement(element)
  {
    while (element != null)
    {
      if (TypeUtility.IsUndefined(element.tagName))
        return null;

      if (IsFocusableTag(element.tagName))
        return element;

      element = element.parentNode;
    }

    return null;
  }

  function UniqueIDToClientID(uniqueID)
  {
    return uniqueID.replace(/\$/g, '_');
  }

  // Determines whether the element (or it's parent) is an anchor tag 
  // and if javascript is used as the HREF.
  function IsJavaScriptAnchor(element)
  {
    if (TypeUtility.IsNull(element) || TypeUtility.IsUndefined(element.tagName))
      return false;
    ArgumentUtility.CheckTypeIsObject('element', element);

    var tagName = element.tagName.toLowerCase();
    if (tagName == 'a'
        && TypeUtility.IsDefined(element.href) && element.href != null
        && element.href.substring(0, 11).toLowerCase() == 'javascript:')
    {
      return true;
    }
    else if (tagName == 'input'
             || tagName == 'select'
             || tagName == 'textarea'
             || tagName == 'button'
             || tagName == 'li'
             || tagName == 'p'
             || tagName == 'div'
             || tagName == 'td'
             || tagName == 'table'
             || tagName == 'form'
             || tagName == 'body'
             || tagName == 'html')
    {
      return false;
    }
    else
    {
      return IsJavaScriptAnchor(element.parentNode);
    }
  };

  // Event handler for the form-elements loosing the focus.
  this.OnElementBlur = function (evt)
  {
    this.SetActiveElement(null);
  };

  // Event handler for the form-elements receiving the focus.
  this.OnElementFocus = function (evt)
  {
    var eventSource = GetEventSource(evt);
    if (eventSource != null)
      this.SetActiveElement(eventSource);
  };

  // Gets the source element for the event.
  // evt: the event object. Used for Mozilla browsers.
  function GetEventSource(evt)
  {
    var e = TypeUtility.IsUndefined(evt) ? window.event : evt;
    if (e == null)
      return null;

    if (TypeUtility.IsDefined(e.target) && e.target != null)
      return e.target;
    else if (TypeUtility.IsDefined(e.srcElement) && e.srcElement != null)
      return e.srcElement;
    else
      return null;
  };

  this.GetEventTarget = function ()
  {
    if (TypeUtility.IsUndefined(_theForm.__EVENTTARGET))
      return null;

    if (StringUtility.IsNullOrEmpty(_theForm.__EVENTTARGET.value))
      return null;

    return document.getElementById(UniqueIDToClientID(_theForm.__EVENTTARGET.value));
  };

  this.IsSynchronousPostBackRequired = function (eventTargetID, eventArguments)
  {
    if (StringUtility.IsNullOrEmpty(eventTargetID))
      return true;

    var id = UniqueIDToClientID(eventTargetID) + '|' + eventArguments;
    for (var i = _synchronousPostBackCommands.length - 1; i >= 0; i--)
    {
      if (_synchronousPostBackCommands[i] == id)
        return true;
    }

    return false;
  };

  this.ClearIsSubmitting = function ()
  {
    _isSubmitting = false;
  };

  this.DisableAbortConfirmation = function ()
  {
    _isAbortConfirmationEnabled = false;
  };

  this.ShowAbortConfirmation = function ()
  {
    if (_isAbortConfirmationEnabled && (!_isDirtyStateTrackingEnabled || _isDirty))
      return window.confirm(_abortMessage);
    else
      return true;
  };

  // Perform initialization
  this.Init();
}

// The single instance of the SmartPage_Context object
SmartPage_Context.Instance = null;

// Called after page's html content is complete.
function SmartPage_OnStartUp(isAsynchronous, isDirty)
{
  AspNetPatches.Apply();
  SmartPage_Context.Instance.OnStartUp(isAsynchronous, isDirty);
}

function SmartPage_PageRequestManager_pageLoaded(sender, args)
{
  SmartPage_Context.Instance.PageRequestManager_pageLoaded(sender, args);
}

function RenderThisHtml(theHtml)
{
  document.write(theHtml);
}
