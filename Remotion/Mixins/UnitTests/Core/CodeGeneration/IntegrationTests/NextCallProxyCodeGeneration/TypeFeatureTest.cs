// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.NextCallProxyCodeGeneration
{
  [TestFixture]
  public class TypeFeatureTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeImplementsMarkerInterface ()
    {
      Type t = CreateMixedType (typeof (BaseType3), typeof (BT3Mixin3<,>));
      Type NextCallProxyType = t.GetNestedType ("NextCallProxy");
      Assert.IsTrue (typeof (IGeneratedNextCallProxyType).IsAssignableFrom (NextCallProxyType));
    }

    [Test]
    public void GeneratedTypeExists ()
    {
      Type t = CreateMixedType (typeof (BaseType3), typeof (BT3Mixin3<,>));
      Assert.IsNotNull (t.GetNestedType ("NextCallProxy"));
    }

    [Test]
    public void SubclassProxyHasNextCallProxyField ()
    {
      Type t = CreateMixedType (typeof (BaseType3), typeof (BT3Mixin3<,>));
      FieldInfo firstField = t.GetField ("__first", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (firstField);
      Assert.AreEqual (t.GetNestedType ("NextCallProxy"), firstField.FieldType);
    }

    [Test]
    public void GeneratedTypeHoldsDepthAndBase ()
    {
      Type t = CreateMixedType(typeof (BaseType3), typeof (BT3Mixin3<,>));
      Type proxyType = t.GetNestedType ("NextCallProxy");
      object proxy = Activator.CreateInstance (proxyType, new object[] { null, -1 });

      Assert.IsNotNull (proxyType.GetField ("__depth"));
      Assert.IsNotNull (proxyType.GetField ("__this"));

      Assert.AreEqual (-1, proxyType.GetField ("__depth").GetValue (proxy));
      Assert.AreEqual (t, proxyType.GetField ("__this").FieldType);
      Assert.IsNull (proxyType.GetField ("__this").GetValue (proxy));
    }

    [Test]
    public void GeneratedTypeImplementsOverriddenMethods1 ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3> ().Clear().AddMixins (typeof (BT3Mixin7Base), typeof (BT3Mixin4)).EnterScope())
      {
        Type t = TypeFactory.GetConcreteType (typeof (BaseType3));
        Type proxyType = t.GetNestedType ("NextCallProxy");

        Assert.IsNotNull (proxyType.GetMethod ("Remotion.Mixins.UnitTests.Core.TestDomain.BaseType3.IfcMethod", BindingFlags.Public | BindingFlags.Instance));
      }
    }

    [Test]
    public void GeneratedTypeImplementsOverriddenMethods2 ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (BT1Mixin1)).EnterScope())
      {
        Type t = TypeFactory.GetConcreteType (typeof (BaseType1));
        Type proxyType = t.GetNestedType ("NextCallProxy");

        Assert.IsNotNull (proxyType.GetMethod ("Remotion.Mixins.UnitTests.Core.TestDomain.BaseType1.VirtualMethod", BindingFlags.Public | BindingFlags.Instance));
        Assert.IsNull (proxyType.GetMethod ("Remotion.Mixins.UnitTests.Core.TestDomain.BaseType1.get_VirtualProperty", BindingFlags.Public | BindingFlags.Instance));
        Assert.IsNotNull (proxyType.GetMethod ("Remotion.Mixins.UnitTests.Core.TestDomain.BaseType1.set_VirtualProperty", BindingFlags.Public | BindingFlags.Instance));
        Assert.IsNotNull (proxyType.GetMethod ("Remotion.Mixins.UnitTests.Core.TestDomain.BaseType1.add_VirtualEvent", BindingFlags.Public | BindingFlags.Instance));
        Assert.IsNotNull (proxyType.GetMethod ("Remotion.Mixins.UnitTests.Core.TestDomain.BaseType1.remove_VirtualEvent", BindingFlags.Public | BindingFlags.Instance));
      }

      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (BT1Mixin2)).EnterScope())
      {
        Type t = TypeFactory.GetConcreteType (typeof (BaseType1));
        Type proxyType = t.GetNestedType ("NextCallProxy");

        Assert.IsNotNull (proxyType.GetMethod ("Remotion.Mixins.UnitTests.Core.TestDomain.BaseType1.get_VirtualProperty", BindingFlags.Public | BindingFlags.Instance));
        Assert.IsNull (proxyType.GetMethod ("Remotion.Mixins.UnitTests.Core.TestDomain.BaseType1.set_VirtualProperty", BindingFlags.Public | BindingFlags.Instance));
      }

    }
  }
}
