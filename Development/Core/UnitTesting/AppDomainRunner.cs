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
using Remotion.Utilities;
using System.IO;

namespace Remotion.Development.UnitTesting
{
  [Serializable]
  public class AppDomainRunner : AppDomainRunnerBase
  {
    public static void Run (Proc<object[]> action, params object[] args)
    {
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.DynamicBase = Path.GetTempPath();
      AppDomainRunner runner = new AppDomainRunner (setup, action, args);
      runner.Run ();
    }

    private Proc<object[]> _action;
    private object[] _args;

    public AppDomainRunner (AppDomainSetup domainSetup, Proc<object[]> action, params object[] args)
      : base (domainSetup)
    {
      _action = action;
      _args = args;
    }

    protected override void CrossAppDomainCallbackHandler ()
    {
      _action (_args);
    }
  }
}
