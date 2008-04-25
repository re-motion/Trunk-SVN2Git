// remove this file from the application project when using a newer egora FX build that contains WxeExecuteFunctionOptions

using System;
using Remotion.Web.ExecutionEngine;
using System.Collections.Specialized;
using System.Web.UI;

namespace Remotion.Web.ExecutionEngine
{
  public class WxeExecuteFunctionExternalException : WxeException
  {
    public WxeExecuteFunctionExternalException ()
      : base ("This exception does not indicate an error. It is used to cancel the processing of an event handler that used WxeExecuteFunctionOptions.External. Please catch this exception after invoking 'Call'.")
    {
    }
  }

  public class WxeExecuteFunctionOptions
  {
    public class NoRepost: WxeExecuteFunctionOptions
    {
      private Control _sender;
      private bool? _usesEventTarget;

      public NoRepost (Control sender)
        : this (sender, null, null)
      {
      }

      public NoRepost (Control sender, bool? usesEventTarget)
        : this (sender, usesEventTarget, null)
      {
      }

      public NoRepost (Control sender, WxePermaUrlOptions usePermaUrl)
        : this (sender, null, usePermaUrl)
      {
      }

      public NoRepost (Control sender, bool? usesEventTarget, WxePermaUrlOptions usePermaUrl)
        : base (usePermaUrl)
      {
        _sender = sender;
        _usesEventTarget = usesEventTarget;
      }

      public override void Execute (IWxePage page, WxeFunction function)
      {
        if (UsePermaUrl != null)
        {
          if (_usesEventTarget != null)
            page.ExecuteFunctionNoRepost (function, Sender, _usesEventTarget.Value, true, _usePermaUrl.UseParentPermaUrl, _usePermaUrl.UrlParameters);
          else
            page.ExecuteFunctionNoRepost (function, Sender, true, _usePermaUrl.UseParentPermaUrl, _usePermaUrl.UrlParameters);
        }
        else
        {
          if (_usesEventTarget != null)
            page.ExecuteFunctionNoRepost (function, Sender, _usesEventTarget.Value);
          else
            page.ExecuteFunctionNoRepost (function, Sender);
        }
      }

      public Control Sender
      {
        get { return _sender; }
        set { _sender = value; }
      }

      public bool? UsesEventTarget 
      { 
        get { return _usesEventTarget; }
        set { _usesEventTarget = value; }
      }
    }

    public class External: WxeExecuteFunctionOptions
    {
      private string _target;
      private string _features;
      private Control _sender;
      private bool _returningPostback;
      private NameValueCollection _callerUrlParameters;

      public External (string target, string features, Control sender, bool returningPostback)
        : this (target, features, sender, returningPostback, null)
      {
      }

      public External (string target, string features, Control sender)
        : this (target, features, sender, true, null)
      {
      }

      public External (string target, Control sender)
        : this (target, null, sender, true, null)
      {
      }

      public External (string target, string features, Control sender, bool returningPostback, NameValueCollection callerUrlParameter)
      {
        _target = target;
        _features = features;
        _sender = sender;
        _returningPostback = returningPostback;
        _callerUrlParameters = callerUrlParameter;
      }

      public override void Execute (IWxePage page, WxeFunction function)
      {
        if (UsePermaUrl != null)
          page.ExecuteFunctionExternal (function, _target, _features, _sender, _returningPostback, true, _usePermaUrl.UseParentPermaUrl, _usePermaUrl.UrlParameters);
        else
          page.ExecuteFunctionExternal (function, _target, _features, _sender, _returningPostback);

        throw new WxeExecuteFunctionExternalException();
      }
      
      public string Target
      {
        get { return _target; }
        set { _target = value; }
      }

      public string Features
      {
        get { return _features; }
        set { _features = value; }
      }

      public Control Sender
      {
        get { return _sender; }
        set { _sender = value; }
      }

      public bool ReturningPostback
      {
        get { return _returningPostback; }
        set { _returningPostback = value; }
      }

      public NameValueCollection CallerUrlParameters
      {
        get { return _callerUrlParameters; }
        set { _callerUrlParameters = value; }
      }
    }

    private WxePermaUrlOptions _usePermaUrl;

    public WxeExecuteFunctionOptions ()
      : this (null)
    {
    }

    public WxeExecuteFunctionOptions (WxePermaUrlOptions usePermaUrl)
    {
      _usePermaUrl = usePermaUrl;
    }

    public virtual void Execute (IWxePage page, WxeFunction function)
    {
      if (_usePermaUrl != null)
        page.ExecuteFunction (function, true, _usePermaUrl.UseParentPermaUrl, _usePermaUrl.UrlParameters);
      else
        page.ExecuteFunction (function);
    }

    public WxePermaUrlOptions UsePermaUrl
    {
      get { return _usePermaUrl; }
      set { _usePermaUrl = value; }
    }
  }

  public class WxePermaUrlOptions
  {
    private bool _useParentPermaUrl;
    private NameValueCollection _urlParameters;

    public WxePermaUrlOptions (bool useParentPermaUrl)
      : this (useParentPermaUrl, null)
    {
    }

    public WxePermaUrlOptions (bool useParentPermaUrl, NameValueCollection urlParameters)
    {
      _useParentPermaUrl = useParentPermaUrl;
      _urlParameters = urlParameters;
    }

    public bool UseParentPermaUrl
    {
      get { return _useParentPermaUrl; }
      set { _useParentPermaUrl = value; }
    }

    public NameValueCollection UrlParameters
    {
      get { return _urlParameters; }
      set { _urlParameters = value; }
    }
  }
}
