// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class TargetClassDefinitionTest
  {
    interface Interface
    {
      void Foo();
    }

    class Base
    {
      public void Foo() {}
    }

    class Derived : Base, Interface
    {
    }

    [Test]
    public void InterfaceMapAdjustedCorrectly ()
    {
      TargetClassDefinition def = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (Derived));
      Assert.IsFalse (def.Methods.ContainsKey (typeof (Derived).GetMethod ("Foo")));
      Assert.IsTrue (def.Methods.ContainsKey (typeof (Base).GetMethod ("Foo")));

      InterfaceMapping mapping = def.GetAdjustedInterfaceMap(typeof (Interface));
      Assert.AreEqual (def.Methods[typeof (Base).GetMethod ("Foo")].MethodInfo, mapping.TargetMethods[0]);
    }

    [Test]
    public void GetAllMethods ()
    {
      TargetClassDefinition def = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1));
      List<string> methodNames = new List<MethodDefinition> (def.GetAllMethods ()).ConvertAll<string> (
          delegate (MethodDefinition d) { return d.Name; });

      Assert.AreEqual (2, methodNames.FindAll (delegate (string s) { return s == "VirtualMethod"; }).Count);

      Assert.Contains ("get_VirtualProperty", methodNames);
      Assert.Contains ("set_VirtualProperty", methodNames);
      Assert.Contains ("get_Item", methodNames);
      Assert.Contains ("set_Item", methodNames);
      Assert.Contains ("add_VirtualEvent", methodNames);
      Assert.Contains ("remove_VirtualEvent", methodNames);
      Assert.Contains ("add_ExplicitEvent", methodNames);
      Assert.Contains ("remove_ExplicitEvent", methodNames);
      Assert.Contains ("ToString", methodNames);
      Assert.Contains ("Equals", methodNames);
      Assert.Contains ("GetHashCode", methodNames);
      Assert.Contains ("MemberwiseClone", methodNames);
      Assert.Contains ("GetType", methodNames);
      Assert.Contains ("Finalize", methodNames);

      Assert.AreEqual (16, methodNames.Count);

    }
  }
}
