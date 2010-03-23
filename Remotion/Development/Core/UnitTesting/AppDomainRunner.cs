// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System;
using Remotion.Utilities;
using System.IO;

namespace Remotion.Development.UnitTesting
{
  [Serializable]
  public class AppDomainRunner : AppDomainRunnerBase
  {
    public static void Run (Action<object[]> action, params object[] args)
    {
      Run (AppDomain.CurrentDomain.BaseDirectory, action, args);
    }

    public static void Run (string applicationBase, Action<object[]> action, params object[] args)
    {
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = applicationBase;
      var runner = new AppDomainRunner (setup, action, args);
      runner.Run ();
    }

    private readonly Action<object[]> _action;
    private readonly object[] _args;

    public AppDomainRunner (AppDomainSetup domainSetup, Action<object[]> action, params object[] args)
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
