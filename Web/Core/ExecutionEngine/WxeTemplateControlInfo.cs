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
using System.Web;
using System.Web.UI;
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{

  public class WxeTemplateControlInfo
  {
    private WxeHandler _wxeHandler;
    private WxePageStep _currentPageStep;
    private WxeFunction _currentFunction;
    private IWxeTemplateControl _control;
    /// <summary> Caches the <see cref="ResourceManagerSet"/> for this control. </summary>
    private ResourceManagerSet _cachedResourceManager;

    public WxeTemplateControlInfo (IWxeTemplateControl control)
    {
      ArgumentUtility.CheckNotNullAndType<TemplateControl> ("control", control);
      _control = control;
    }

    public virtual void Initialize (HttpContext context)
    {
      if (ControlHelper.IsDesignMode (_control, context))
        return;
      ArgumentUtility.CheckNotNull ("context", context);

      if (_control is Page)
      {
        _wxeHandler = context.Handler as WxeHandler;
      }
      else
      {
        IWxePage wxePage = _control.Page as IWxePage;
        if (wxePage == null)
          throw new InvalidOperationException (string.Format ("'{0}' can only be added to a Page implementing the IWxePage interface.", _control.GetType ().FullName));
        _wxeHandler = wxePage.WxeHandler;
      }
      if (_wxeHandler == null)
      {
        throw new HttpException (string.Format ("No current WxeHandler found. Most likely cause of the exception: "
            + "The page '{0}' has been called directly instead of using a WXE Handler to invoke the associated WXE Function.",
            _control.Page.GetType ()));
      }

      _currentPageStep = (WxePageStep) _wxeHandler.RootFunction.ExecutingStep;
      _currentFunction = WxeStep.GetFunction (_currentPageStep);
    }

    public WxeHandler WxeHandler
    {
      get { return _wxeHandler; }
    }

    public WxePageStep CurrentPageStep
    {
      get { return _currentPageStep; }
    }

    public WxeFunction CurrentFunction
    {
      get { return _currentFunction; }
    }

    public NameObjectCollection Variables
    {
      get { return (_currentPageStep == null) ? null : _currentPageStep.Variables; }
    }

    /// <summary> Find the <see cref="IResourceManager"/> for this control info. </summary>
    /// <param name="localResourcesType"> 
    ///   A type with the <see cref="MultiLingualResourcesAttribute"/> applied to it.
    ///   Typically an <b>enum</b> or the derived class itself.
    /// </param>
    protected IResourceManager GetResourceManager (Type localResourcesType)
    {
      Remotion.Utilities.ArgumentUtility.CheckNotNull ("localResourcesType", localResourcesType);

      //  Provider has already been identified.
      if (_cachedResourceManager != null)
        return _cachedResourceManager;

      //  Get the resource managers

      IResourceManager localResourceManager = MultiLingualResources.GetResourceManager (localResourcesType, true);
      Control namingContainer = _control.NamingContainer ?? (Control) _control;
      IResourceManager namingContainerResourceManager = ResourceManagerUtility.GetResourceManager (namingContainer, true);

      if (namingContainerResourceManager == null)
        _cachedResourceManager = new ResourceManagerSet (localResourceManager);
      else
        _cachedResourceManager = new ResourceManagerSet (localResourceManager, namingContainerResourceManager);

      return _cachedResourceManager;
    }
  }

}
