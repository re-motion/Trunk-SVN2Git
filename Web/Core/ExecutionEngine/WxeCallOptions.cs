/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Specialized;
using System.Web.UI;

namespace Remotion.Web.ExecutionEngine
{
  internal class Sample
  {
    void OnClick (object sender, EventArgs e)
    {
      try
      {
        IWxeCallArguments args;
        args = WxeCallArguments.Default;                                    
        args = new WxePermaUrlOptions ();                                  
        args = new WxePermaUrlOptions (true);
        args = new WxeCallArguments ((Control) sender, new WxeCallOptionsExternal ("_blank"));
        args = new WxeCallArguments ((Control) sender, new WxeCallOptionsNoRepost ());
        args = new WxeCallArguments ((Control) sender, new WxeCallOptions ());
        // MyPage.Call (this, handler, arg1);
       }
      catch (WxeIgnorableException) { }
    }
  }

  public interface IWxeCallArguments
  {
    void Call (IWxePage page, WxeFunction function);
  }

  public class WxeCallArguments : IWxeCallArguments
  {
    private class DefaultArguments : IWxeCallArguments
    {
      public void Call (IWxePage page, WxeFunction function)
      {
        page.ExecuteFunction (function);
      }
    }

    private static IWxeCallArguments s_default = new DefaultArguments ();

    public static IWxeCallArguments Default
    {
      get { return s_default; }
    }

    private Control _sender;
    private WxeCallOptions _options;

    public WxeCallArguments (Control sender, WxeCallOptions options)
    {
      _options = options;
      _sender = sender;
    }

    public void Call (IWxePage page, WxeFunction function)
    {
      _options.Call (page, function, this);
    }

    public Control Sender
    {
      get { return _sender; }
    }

    public WxeCallOptions Options
    {
      get { return _options; }
    }
  }

  public class WxeCallOptions 
  {
    private WxePermaUrlOptions _usePermaUrl;

    public WxeCallOptions (WxePermaUrlOptions usePermaUrl)
    {
      _usePermaUrl = usePermaUrl;
    }

    public WxeCallOptions ()
      : this (null)
    {
    }

    public virtual void Call (IWxePage page, WxeFunction function, WxeCallArguments handler)
    {
      if (_usePermaUrl != null)
        _usePermaUrl.Call (page, function);
      else
        page.ExecuteFunction (function);
    }

    public WxePermaUrlOptions UsePermaUrl
    {
      get { return _usePermaUrl; }
      set { _usePermaUrl = value; }
    }
  }

  public class WxeCallOptionsNoRepost: WxeCallOptions
  {
    private bool? _usesEventTarget;

    public WxeCallOptionsNoRepost ()
      : this (null, null)
    {
    }

    public WxeCallOptionsNoRepost (bool? usesEventTarget)
      : this (usesEventTarget, null)
    {
    }

    public WxeCallOptionsNoRepost (WxePermaUrlOptions usePermaUrl)
      : this (null, usePermaUrl)
    {
    }

    public WxeCallOptionsNoRepost (bool? usesEventTarget, WxePermaUrlOptions usePermaUrl)
      : base (usePermaUrl)
    {
      _usesEventTarget = usesEventTarget;
    }

    public override void Call (IWxePage page, WxeFunction function, WxeCallArguments handler)
    {
      if (UsePermaUrl != null)
      {
        if (_usesEventTarget != null)
          page.ExecuteFunctionNoRepost (function, handler.Sender, _usesEventTarget.Value, true, UsePermaUrl.UseParentPermaUrl, UsePermaUrl.UrlParameters);
        else
          page.ExecuteFunctionNoRepost (function, handler.Sender, true, UsePermaUrl.UseParentPermaUrl, UsePermaUrl.UrlParameters);
      }
      else
      {
        if (_usesEventTarget != null)
          page.ExecuteFunctionNoRepost (function, handler.Sender, _usesEventTarget.Value);
        else
          page.ExecuteFunctionNoRepost (function, handler.Sender);
      }
    }

    public bool? UsesEventTarget 
    { 
      get { return _usesEventTarget; }
    }
  }

  public class WxeCallOptionsExternal : WxeCallOptions
  {
    private string _target;
    private string _features;
    private bool _returningPostback;

    public WxeCallOptionsExternal (string target, string features, bool returningPostback)
      : this (target, features, returningPostback, null)
    {
    }

    public WxeCallOptionsExternal (string target, string features)
      : this (target, features, true, null)
    {
    }

    public WxeCallOptionsExternal (string target)
      : this (target, null, true, null)
    {
    }

    public WxeCallOptionsExternal (string target, string features, bool returningPostback, WxePermaUrlOptions usePermaUrl)
      : base (usePermaUrl)
    {
      _target = target;
      _features = features;
      _returningPostback = returningPostback;
    }

    public override void Call (IWxePage page, WxeFunction function, WxeCallArguments handler)
    {
      if (UsePermaUrl != null)
        page.ExecuteFunctionExternal (function, _target, _features, handler.Sender, _returningPostback, true, UsePermaUrl.UseParentPermaUrl, UsePermaUrl.UrlParameters);
      else
        page.ExecuteFunctionExternal (function, _target, _features, handler.Sender, _returningPostback);

      throw new WxeCallExternalException();
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

    public bool ReturningPostback
    {
      get { return _returningPostback; }
      set { _returningPostback = value; }
    }
  }

  public class WxePermaUrlOptions : IWxeCallArguments
  {
    private bool _useParentPermaUrl;
    private NameValueCollection _urlParameters;

    public WxePermaUrlOptions ()
      : this (false, null)
    {
    }

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

    public void Call (IWxePage page, WxeFunction function)
    {
      page.ExecuteFunction (function, true, UseParentPermaUrl, UrlParameters);
    }
  }
}
