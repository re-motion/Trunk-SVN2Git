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
