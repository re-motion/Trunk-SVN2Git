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
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Scripting.UnitTests
{
  public class ScriptContextTestHelper
  {
    public static ScriptContext CreateTestScriptContext (string name, ITypeFilter typeFilter)
    {
      return (ScriptContext) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (ScriptContext).Assembly, "Remotion.Scripting.ScriptContext", name, typeFilter);
    }

    public static ScriptContext CreateTestScriptContext (string name)
    {
      var typeFilterStub = MockRepository.GenerateStub<ITypeFilter> ();
      return CreateTestScriptContext (name, typeFilterStub);
    }

    public static ScriptContext CreateTestScriptContext ()
    {
      return CreateTestScriptContext ("DummyScriptContext");
    }

    public static void ClearScriptContexts ()
    {
      PrivateInvoke.InvokeNonPublicStaticMethod (typeof (ScriptContext), "ClearScriptContexts");
    }

    public static void ReleaseAllScriptContexts ()
    {
      PrivateInvoke.InvokeNonPublicStaticMethod (typeof (ScriptContext), "ReleaseAllScriptContexts");
    }
  }
}
