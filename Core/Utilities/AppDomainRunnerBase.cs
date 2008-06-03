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

namespace Remotion.Utilities
{
  /// <summary>
  /// Base class for executing code in a separate <see cref="AppDomain"/>.
  /// </summary>
  [Serializable]
  public abstract class AppDomainRunnerBase
  {
    private readonly AppDomainSetup _appDomainSetup;

    protected AppDomainRunnerBase (AppDomainSetup appDomainSetup)
    {
      ArgumentUtility.CheckNotNull ("appDomainSetup", appDomainSetup);

      _appDomainSetup = appDomainSetup;
    }

    protected abstract void CrossAppDomainCallbackHandler ();

    public AppDomainSetup AppDomainSetup
    {
      get { return _appDomainSetup; }
    }

    public void Run ()
    {
      AppDomain appDomain = null;
      AppDomainAssemblyResolver appDomainAssemblyResolver = null;
      try
      {
        appDomain = AppDomain.CreateDomain (_appDomainSetup.ApplicationName, AppDomain.CurrentDomain.Evidence, _appDomainSetup);
        appDomainAssemblyResolver = new AppDomainAssemblyResolver (AppDomain.CurrentDomain.SetupInformation, appDomain);
        appDomain.DoCallBack (CrossAppDomainCallbackHandler);
      }
      finally
      {
        if (appDomain != null)
          AppDomain.Unload (appDomain);

        if (appDomainAssemblyResolver != null)
          appDomainAssemblyResolver.Dispose();
      }
    }
  }
}
