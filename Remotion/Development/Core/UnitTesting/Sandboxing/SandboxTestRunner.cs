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
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Sandboxing
{
  public class SandboxTestRunner : MarshalByRefObject
  {
    public static void Run (Type[] types, IPermission[] permissions, Assembly[] fullTrustAssemblies)
    {
      using (var sandbox = Sandbox.CreateSandbox (permissions, fullTrustAssemblies))
      {
        var runner = sandbox.CreateSandboxedInstance<SandboxTestRunner> (permissions);
        try
        {
          runner.RunTests (types);
        }
        catch (TargetInvocationException ex)
        {
          throw ExceptionUtility.PreserveStackTrace (ex.InnerException);
        }
      }
    }

    private void RunTests (Type[] types)
    {
      foreach (var t in types)
        Run (t);

    }

    private void Run (Type type)
    {
      var testInstance = Activator.CreateInstance (type);

      var setupMethod = type.GetMethods ().Where (m => m.IsDefined (typeof (SetUpAttribute), false)).SingleOrDefault ();
      var tearDownMethod = type.GetMethods ().Where (m => m.IsDefined (typeof (TearDownAttribute), false)).SingleOrDefault ();
      var testMethods = type.GetMethods ().Where (m => m.IsDefined (typeof (TestAttribute), false));

      foreach (var testMethod in testMethods)
      {
        if (setupMethod != null)
          setupMethod.Invoke (testInstance, null);

        testMethod.Invoke (testInstance, null);

        if (tearDownMethod != null)
          tearDownMethod.Invoke (testInstance, null);

      }
    }
  }
}