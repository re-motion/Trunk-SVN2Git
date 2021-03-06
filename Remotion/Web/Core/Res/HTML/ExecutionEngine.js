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

// The context contains all information required by the WXE page.
// isCacheDetectionEnabled: true to detect whether the user is viewing a cached page.
// refreshInterval: The refresh interfal in milli-seconds. zero to disable refreshing.
// refreshUrl: The URL used to post the refresh request to. Must not be null if refreshInterval is greater than zero.
// abortUrl: The URL used to post the abort request to. null to disable the abort request.
// statusIsAbortingMessage: The message displayed when the user attempts to submit while an abort is in progress. 
//    null to disable the message.
// statusIsCachedMessage: The message displayed when the user returns to a cached page. null to disable the message.
// wxePostBackSequenceNumberFieldID: The ID of the WXE postback sequence number.
// dmaWxePostBackSequenceNumberFieldID: The ID of the DMA WXE postback sequence number. null to disable the updating of
//    the field.
function WxePage_Context(
      isCacheDetectionEnabled,
      refreshInterval, refreshUrl,
      abortUrl,
      statusIsAbortingMessage, statusIsCachedMessage,
      wxePostBackSequenceNumberFieldID,
      dmaWxePostBackSequenceNumberFieldID)
{
  ArgumentUtility.CheckNotNullAndTypeIsBoolean('isCacheDetectionEnabled', isCacheDetectionEnabled);
  ArgumentUtility.CheckNotNullAndTypeIsNumber('refreshInterval', refreshInterval);
  ArgumentUtility.CheckTypeIsString('refreshUrl', refreshUrl);
  ArgumentUtility.CheckTypeIsString('abortUrl', abortUrl);
  ArgumentUtility.CheckTypeIsString('statusIsAbortingMessage', statusIsAbortingMessage);
  ArgumentUtility.CheckTypeIsString('statusIsCachedMessage', statusIsCachedMessage);
  ArgumentUtility.CheckTypeIsString('wxePostBackSequenceNumberFieldID', wxePostBackSequenceNumberFieldID);
  ArgumentUtility.CheckTypeIsString('dmaWxePostBackSequenceNumberFieldID', dmaWxePostBackSequenceNumberFieldID);

  // The URL used to post the refresh request to.
  var _refreshUrl = null;
  // The timer used to invoke the refreshing.
  var _refreshTimer = null;
  if (refreshInterval > 0)
  {
    ArgumentUtility.CheckNotNull('refreshUrl', refreshUrl);
    _refreshUrl = refreshUrl;
    _refreshTimer = window.setInterval(function() { WxePage_Context._instance.Refresh(); }, refreshInterval);
  };
  var _httpStatusFunctionTimeout = 409;

  // The URL used to post the abort request to.
  var _abortUrl = abortUrl;
  var _isAbortEnabled = abortUrl != null;

  // The message displayed when the user attempts to submit while an abort is in progress. null to disable the message.
  var _statusIsAbortingMessage = statusIsAbortingMessage;
  // The message displayed when the user returns to a cached page. null to disable the message.
  var _statusIsCachedMessage = statusIsCachedMessage;

  var _isCacheDetectionEnabled = isCacheDetectionEnabled;

  var _wxePostBackSequenceNumberFieldID = wxePostBackSequenceNumberFieldID;
  var _dmaWxePostBackSequenceNumberFieldID = dmaWxePostBackSequenceNumberFieldID;

  // Handles the page loading event.
  this.OnLoading = function(hasSubmitted, isCached, isAsynchronous)
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('hasSubmitted', hasSubmitted);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isCached', isCached);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isAsynchronous', isAsynchronous);

    if (_dmaWxePostBackSequenceNumberFieldID != null)
    {
      var dmaWxePostBackSequenceNumberField = document.getElementById(_dmaWxePostBackSequenceNumberFieldID);
      var postBackSequenceNumber = document.getElementById(_wxePostBackSequenceNumberFieldID).value;
      dmaWxePostBackSequenceNumberField.value = postBackSequenceNumber;
    }

    if (!isAsynchronous && _refreshTimer != null)
    {
      // The lock remains alive until the page is reloaded using a GET-request or synchronous post-back.
      this.EstablishPageKeepAliveLock();
    }
  };

  // Handles the page loaded event.
  this.OnLoaded = function(hasSubmitted, isCached, isAsynchronous)
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('hasSubmitted', hasSubmitted);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isCached', isCached);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isAsynchronous', isAsynchronous);

    if (_isCacheDetectionEnabled
        && (isCached || hasSubmitted))
    {
      this.ShowStatusIsCachedMessage();
    }
  };

  this.OnUnload = function()
  {
    this.Dispose();
  }

  // Handles the page abort event.
  this.OnAbort = function(hasSubmitted, isCached)
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('hasSubmitted', hasSubmitted);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isCached', isCached);

    if (_isAbortEnabled
        && (_isCacheDetectionEnabled && (!isCached || hasSubmitted)))
    {
      var successHandler = function (args) {};
      var errorHandler = function (args) {};
      SmartPage_Context.Instance.SendOutOfBandRequest(_abortUrl, successHandler, errorHandler);
    }
  };

  this.Dispose = function()
  {
    if (_refreshTimer != null)
      window.clearInterval(_refreshTimer);
  }

  // Handles the refresh timer events
  this.Refresh = function()
  {
    var successHandler = function (args) {};
    var errorHandler = function (args)
    {
      if (args.Status === _httpStatusFunctionTimeout)
      {
        if (window.console)
          window.console.warn ('WXE function has timed out. Stopping refresh- and abort-requests.');

        if (_refreshTimer !== null)
        {
          window.clearInterval (_refreshTimer);
          _refreshTimer = null;
        }
        _isAbortEnabled = false;
      }
    };
    SmartPage_Context.Instance.SendOutOfBandRequest(_refreshUrl + '&WxePage_Garbage=' + Math.random(), successHandler, errorHandler);
  };

  // Evaluates whether the postback request should continue.
  // returns: true to continue with request
  this.CheckFormState = function(isAborting, hasSubmitted, hasUnloaded, isCached)
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isAborting', isAborting);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('hasSubmitted', hasSubmitted);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('hasUnloaded', hasUnloaded);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isCached', isCached);

    if (_isCacheDetectionEnabled
        && (isCached || hasSubmitted || hasUnloaded))
    {
      this.ShowStatusIsCachedMessage();
      return false;
    }
    if (isAborting)
    {
      this.ShowStatusIsAbortingMessage();
      return false;
    }
    else
    {
      return true;
    }
  };

  // Shows the "page is aborting" message
  this.ShowStatusIsAbortingMessage = function()
  {
    if (_statusIsAbortingMessage != null)
      SmartPage_Context.Instance.ShowMessage('WxeStatusIsAbortingMessage', _statusIsAbortingMessage);
  };

  // Shows the "page is cached" message
  this.ShowStatusIsCachedMessage = function()
  {
    if (_statusIsCachedMessage != null)
      SmartPage_Context.Instance.ShowMessage('WxeStatusIsCachedMessage', _statusIsCachedMessage);
  };

  this.EstablishPageKeepAliveLock = function()
  {
    var lockName = btoa(Math.random() * 10e18);
    var lockIndex = 0;

    function onLockTaken(lock)
    {
      if (lock === null)
      {
        lockIndex++;
        navigator.locks.request(lockName + lockIndex, { ifAvailable: true }, onLockTaken);
        return null;
      }
      else
      {
        return new Promise(function (resolve, reject) { });
      }
    }

    // The navigator.locks API is only available for secure origins, i.e. sites using HTTPS.
    // The navigator.locks API is only available in Chromium-based browsers, versions 69.* and later.
    if (navigator.locks && navigator.locks.request)
    {
      navigator.locks.request(lockName + lockIndex, { ifAvailable: true }, onLockTaken);
    }
  }
}

// The single instance of the WxePage_Context object
WxePage_Context._instance = null;

WxePage_Context.GetInstance = function()
{
  return WxePage_Context._instance;
}

WxePage_Context.SetInstance = function(instance)
{
  ArgumentUtility.CheckNotNull('instance', instance);

  if (WxePage_Context._instance != null)
    WxePage_Context._instance.Dispose();

  WxePage_Context._instance = instance;
}

function WxePage_OnLoading(hasSubmitted, isCached, isAsynchronous)
{
  WxePage_Context._instance.OnLoading(hasSubmitted, isCached, isAsynchronous);
}

function WxePage_OnLoaded(hasSubmitted, isCached, isAsynchronous)
{
  WxePage_Context._instance.OnLoaded(hasSubmitted, isCached, isAsynchronous);
}

function WxePage_OnUnload()
{
  WxePage_Context._instance.OnUnload();
}

function WxePage_OnAbort(hasSubmitted, isCached)
{
  WxePage_Context._instance.OnAbort(hasSubmitted, isCached);
}

function WxePage_CheckFormState(isAborting, hasSubmitted, hasUnloaded, isCached)
{
  return WxePage_Context._instance.CheckFormState(isAborting, hasSubmitted, hasUnloaded, isCached);
}
