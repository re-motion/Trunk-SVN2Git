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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.MixerTool;
using Remotion.UnitTests.Mixins.MixerTool.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;
using System.Linq;

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

    [SetUp]
    public void SetUp ()
    {
      _configuredClassContext1 = new ClassContext (typeof (BaseType1));
      _configuredClassContext2 = new ClassContext (typeof (NullTarget));
      _genericClassContext = new ClassContext (typeof (GenericTargetClass<>));
      _interfaceClassContext = new ClassContext (typeof (IBaseType2));

      _configuration = new MixinConfiguration ();
      _configuration.ClassContexts.Add (_configuredClassContext1);
      _configuration.ClassContexts.Add (_configuredClassContext2);
      _configuration.ClassContexts.Add (_genericClassContext);
      _configuration.ClassContexts.Add (_interfaceClassContext);
    }

    [Test]
    public void FindClassContexts_ConfiguredContexts ()
    {
      var finder = new ClassContextFinder (_configuration, Type.EmptyTypes);
      var result = finder.FindClassContexts ().ToArray ();

      Assert.That (result, List.Contains (_configuredClassContext1));
      Assert.That (result, List.Contains (_configuredClassContext2));
    }

    [Test]
    public void FindClassContexts_ConfiguredContexts_NoGenerics ()
    {
      var finder = new ClassContextFinder (_configuration, Type.EmptyTypes);
      var result = finder.FindClassContexts ().ToArray ();

      Assert.That (result, List.Not.Contains (_genericClassContext));
    }

    [Test]
    public void FindClassContexts_ConfiguredContexts_NoInterfaces ()
    {
      var finder = new ClassContextFinder (_configuration, Type.EmptyTypes);
      var result = finder.FindClassContexts ().ToArray ();

      Assert.That (result, List.Not.Contains (_interfaceClassContext));
    }

    [Test]
    public void FindClassContexts_InheritedContexts ()
    {
      var finder = new ClassContextFinder (_configuration, new[] { typeof (DerivedNullTarget) });
      var result = finder.FindClassContexts ().SingleOrDefault (cc => cc.Type == typeof (DerivedNullTarget));

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    public void FindClassContexts_InheritedContexts_NoTypesMarkedWithIgnoreAttribute ()
    {
      var finder = new ClassContextFinder (_configuration, new[] { typeof (ClassWithIgnoreAttribute) });
      var result = finder.FindClassContexts ().SingleOrDefault (cc => cc.Type == typeof (ClassWithIgnoreAttribute));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void FindClassContexts_InheritedContexts_NoDuplicates ()
    {
      var finder = new ClassContextFinder (_configuration, new[] { typeof (NullTarget) });
      var result = finder.FindClassContexts ().Count (cc => cc.Type == typeof (NullTarget));

      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void FindClassContexts_InheritedContexts_NoNonInherited ()
    {
      var finder = new ClassContextFinder (_configuration, new[] { typeof (object) });
      var result = finder.FindClassContexts ().Count (cc => cc == null || cc.Type == typeof (object));

      Assert.That (result, Is.EqualTo (0));
    }

    [Test]
    public void FindClassContexts_InheritedContexts_NoGenerics ()
    {
      var finder = new ClassContextFinder (_configuration, new[] { typeof (GenericDerivedNullTarget<>) });
      var result = finder.FindClassContexts ().Count (cc => cc.Type == typeof (GenericDerivedNullTarget<>));

      Assert.That (result, Is.EqualTo (0));
    }

    [Test]
    public void FindClassContexts_InheritedContexts_NoInterfaces ()
    {
      var finder = new ClassContextFinder (_configuration, new[] { typeof (IDerivedIBaseType2) });
      var result = finder.FindClassContexts ().Count (cc => cc.Type == typeof (IDerivedIBaseType2));

      Assert.That (result, Is.EqualTo (0));
    }
  }
}