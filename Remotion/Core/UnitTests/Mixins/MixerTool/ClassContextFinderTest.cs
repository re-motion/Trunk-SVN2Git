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
using System.ComponentModel.Design;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.MixerTool;
using Remotion.UnitTests.Mixins.MixerTool.TestDomain;
using System.Linq;
using Remotion.UnitTests.Mixins.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.MixerTool
{
  [TestFixture]
  public class ClassContextFinderTest
  {
    private ClassContext _configuredClassContext1;
    private ClassContext _configuredClassContext2;
    private ClassContext _genericClassContext;
    private ClassContext _interfaceClassContext;

    private MixinConfiguration _configuration;

    private ITypeDiscoveryService _configuredTypeDiscoveryServiceStub;

    [SetUp]
    public void SetUp ()
    {
      _configuredClassContext1 = new ClassContext (typeof (BaseType1), typeof (NullMixin));
      _configuredClassContext2 = new ClassContext (typeof (NullTarget), typeof (NullMixin));
      _genericClassContext = new ClassContext (typeof (GenericTargetClass<>), typeof (NullMixin));
      _interfaceClassContext = new ClassContext (typeof (IBaseType2), typeof (NullMixin));

      var classContexts = new ClassContextCollection (_configuredClassContext1, _configuredClassContext2, _genericClassContext, _interfaceClassContext);
      _configuration = new MixinConfiguration (classContexts);

      _configuredTypeDiscoveryServiceStub = CreateTypeDiscoveryServiceStub (
          _configuredClassContext1.Type, 
          _configuredClassContext2.Type, 
          _genericClassContext.Type, 
          _interfaceClassContext.Type);
    }

    [Test]
    public void FindClassContexts_ConfiguredContexts ()
    {
      var finder = new ClassContextFinder (_configuredTypeDiscoveryServiceStub);
      var result = finder.FindClassContexts (_configuration).ToArray ();

      Assert.That (result, List.Contains (_configuredClassContext1));
      Assert.That (result, List.Contains (_configuredClassContext2));
    }

    [Test]
    public void FindClassContexts_ConfiguredContexts_NoGenerics ()
    {
      var finder = new ClassContextFinder (_configuredTypeDiscoveryServiceStub);
      var result = finder.FindClassContexts (_configuration).ToArray ();

      Assert.That (result, List.Not.Contains (_genericClassContext));
    }

    [Test]
    public void FindClassContexts_ConfiguredContexts_NoInterfaces ()
    {
      var finder = new ClassContextFinder (_configuredTypeDiscoveryServiceStub);
      var result = finder.FindClassContexts (_configuration).ToArray ();

      Assert.That (result, List.Not.Contains (_interfaceClassContext));
    }

    [Test]
    public void FindClassContexts_InheritedContexts ()
    {
      var finder = new ClassContextFinder (CreateTypeDiscoveryServiceStub (typeof (DerivedNullTarget)));
      var result = finder.FindClassContexts (_configuration).SingleOrDefault (cc => cc.Type == typeof (DerivedNullTarget));

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    public void FindClassContexts_InheritedContexts_NoTypesMarkedWithIgnoreAttribute ()
    {
      var finder = new ClassContextFinder (CreateTypeDiscoveryServiceStub (typeof (ClassWithIgnoreAttribute)));
      var result = finder.FindClassContexts (_configuration).SingleOrDefault (cc => cc.Type == typeof (ClassWithIgnoreAttribute));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void FindClassContexts_InheritedContexts_NoDuplicates ()
    {
      var finder = new ClassContextFinder (CreateTypeDiscoveryServiceStub (typeof (NullTarget)));
      var result = finder.FindClassContexts (_configuration).Count (cc => cc.Type == typeof (NullTarget));

      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void FindClassContexts_InheritedContexts_NoNonInherited ()
    {
      var finder = new ClassContextFinder (CreateTypeDiscoveryServiceStub (typeof (object)));
      var result = finder.FindClassContexts (_configuration).Count (cc => cc == null || cc.Type == typeof (object));

      Assert.That (result, Is.EqualTo (0));
    }

    [Test]
    public void FindClassContexts_InheritedContexts_NoGenerics ()
    {
      var finder = new ClassContextFinder (CreateTypeDiscoveryServiceStub (typeof (GenericDerivedNullTarget<>)));
      var result = finder.FindClassContexts (_configuration).Count (cc => cc.Type == typeof (GenericDerivedNullTarget<>));

      Assert.That (result, Is.EqualTo (0));
    }

    [Test]
    public void FindClassContexts_InheritedContexts_NoInterfaces ()
    {
      var finder = new ClassContextFinder (CreateTypeDiscoveryServiceStub (typeof (IDerivedIBaseType2)));
      var result = finder.FindClassContexts (_configuration).Count (cc => cc.Type == typeof (IDerivedIBaseType2));

      Assert.That (result, Is.EqualTo (0));
    }

    [Test]
    public void FindClassContexts_NoMixedTypes ()
    {
      var generatedType = ConcreteTypeBuilder.Current.GetConcreteType (_configuredClassContext1);
      var typeDiscoveryServiceStub = CreateTypeDiscoveryServiceStub (generatedType);

      var finder = new ClassContextFinder (typeDiscoveryServiceStub);
      var result = finder.FindClassContexts (_configuration).ToArray ();

      Assert.That (result, Is.Empty);
    }


    private ITypeDiscoveryService CreateTypeDiscoveryServiceStub (params Type[] stubResult)
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, false)).Return (stubResult);
      return typeDiscoveryServiceStub;
    }
  }
}
