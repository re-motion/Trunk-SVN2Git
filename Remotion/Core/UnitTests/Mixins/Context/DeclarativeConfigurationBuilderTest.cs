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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Samples.PhotoStuff.Variant1;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Context
{
  [TestFixture]
  public class DeclarativeConfigurationBuilderTest
  {
    private DeclarativeConfigurationBuilder _builder;
    private ClassContext _globalClassContext;

    [SetUp]
    public void SetUp ()
    {
      _builder = new DeclarativeConfigurationBuilder (null);
      _globalClassContext = new ClassContextBuilder (typeof (TargetClassForGlobalMix))
          .AddMixin (typeof (MixinForGlobalMix)).WithDependency (typeof (AdditionalDependencyForGlobalMix))
          .AddMixin (typeof (AdditionalDependencyForGlobalMix)).BuildClassContext ();
    }

    [Test]
    public void AddType ()
    {
      _builder.AddType (typeof (object));
      _builder.AddType (typeof (string));

      Assert.That (_builder.AllTypes, Is.EquivalentTo (new[] { typeof (object), typeof (string) }));
    }

    [Test]
    public void AddType_Twice ()
    {
      _builder.AddType (typeof (object));
      _builder.AddType (typeof (object));

      Assert.That (_builder.AllTypes, Is.EquivalentTo (new[] { typeof (object) }));
    }

    [Test]
    public void AddType_WithDerivedType ()
    {
      _builder.AddType (typeof (string));

      Assert.That (_builder.AllTypes, Is.EquivalentTo (new[] { typeof (object), typeof (string) }));
    }

    [Test]
    public void AddType_WithOpenGenericType ()
    {
      _builder.AddType (typeof (List<>));

      Assert.That (_builder.AllTypes, Is.EquivalentTo (new[] { typeof (List<>), typeof (object) }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Type must be non-generic or a generic type definition.\r\nParameter name: type")]
    public void AddType_WithClosedGenericType ()
    {
      _builder.AddType (typeof (List<int>));

      Assert.That (_builder.AllTypes, Is.EquivalentTo (new[] { typeof (List<>) }));
    }

    class DerivedList : List<int> { }

    [Test]
    public void AddType_WithDerivedFromGenericType ()
    {
      _builder.AddType (typeof (DerivedList));

      Assert.That (_builder.AllTypes, Is.EquivalentTo (new[] { typeof (DerivedList), typeof (List<>), typeof (object) }));
    }

    [IgnoreForMixinConfiguration]
    public class TypeIgnored
    {
    }

    [Test]
    public void AddAssembly_AddsTypesInAssembly ()
    {
      _builder.AddAssembly (typeof (DeclarativeConfigurationBuilderTest).Assembly);
      Assert.That (_builder.AllTypes, List.Contains (typeof (BaseType1)));
    }

    [Test]
    public void AddAssembly_IgnoresTaggedTypes ()
    {
      _builder.AddAssembly (typeof (DeclarativeConfigurationBuilderTest).Assembly);

      Assert.That (_builder.AllTypes, List.Not.Contains (typeof (TypeIgnored)));
    }

    [Test]
    public void AddAssembly_IgnoresGeneratedTypes ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
      _builder.AddAssembly (generatedType.Assembly);

      Assert.That (_builder.AllTypes, Is.Empty);
    }

    [Test]
    public void AddAssembly_CheckTypes ()
    {
      _builder.AddAssembly (typeof (DeclarativeConfigurationBuilderTest).Assembly);

      var referenceBuilder = new DeclarativeConfigurationBuilder (null);
      foreach (Type t in typeof (DeclarativeConfigurationBuilderTest).Assembly.GetTypes ())
      {
        if (!t.IsDefined (typeof (IgnoreForMixinConfigurationAttribute), false) && !MixinTypeUtility.IsGeneratedByMixinEngine (t))
          referenceBuilder.AddType (t);
      }

      Assert.That (_builder.AllTypes, Is.EquivalentTo ((ICollection) referenceBuilder.AllTypes));
    }

    [Uses (typeof (NullMixin))]
    [IgnoreForMixinConfiguration]
    class User { }

    [Extends (typeof (NullTarget))]
    [IgnoreForMixinConfiguration]
    class Extender { }

    [CompleteInterface (typeof (NullTarget))]
    [IgnoreForMixinConfiguration]
    interface ICompleteInterface { }


    [Test]
    public void BuildConfiguration ()
    {
      _builder.AddType (typeof (User));
      _builder.AddType (typeof (Extender));
      _builder.AddType (typeof (ICompleteInterface));

      MixinConfiguration configuration = _builder.BuildConfiguration();
      ClassContext c1 = new ClassContextBuilder (typeof (User)).AddMixin (typeof (NullMixin)).OfKind (MixinKind.Used).BuildClassContext();
      ClassContext c2 = new ClassContextBuilder(typeof (NullTarget))
          .AddMixin (typeof (Extender)).AddCompleteInterface (typeof (ICompleteInterface)).BuildClassContext ();
      Assert.That (configuration.ClassContexts, Is.EquivalentTo (new object[] { c1, c2, _globalClassContext }));
    }

    [Test]
    public void BuildConfiguration_WithParentConfiguration ()
    {
      MixinConfiguration parentConfiguration = MixinConfiguration.BuildNew().ForClass<int>().AddMixin<string>().BuildConfiguration();
      var builder = new DeclarativeConfigurationBuilder (parentConfiguration);
      builder.AddType (typeof (User));

      MixinConfiguration configuration = builder.BuildConfiguration ();
      ClassContext c1 = new ClassContextBuilder (typeof (User)).AddMixin (typeof (NullMixin)).OfKind (MixinKind.Used).BuildClassContext ();
      Assert.That (configuration.ClassContexts,
          Is.EquivalentTo (new object[] { c1, parentConfiguration.GetContext (typeof (int)), _globalClassContext }));
    }

    [Test]
    public void BuildConfigurationFromAssemblies ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());

      Assert.That (configuration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)), Is.True);

      ClassContext contextForBaseType1 = configuration.GetContext (typeof (BaseType1));
      Assert.That (contextForBaseType1.Mixins.Count, Is.EqualTo (2));

      Assert.That (contextForBaseType1.Mixins.ContainsKey (typeof (BT1Mixin1)), Is.True);
      Assert.That (contextForBaseType1.Mixins.ContainsKey (typeof (BT1Mixin2)), Is.True);
    }

    [Test]
    public void BuildConfigurationFromAssemblies_Multiple ()
    {
      var assemblies = new[] { typeof (BaseType1).Assembly, typeof (Photo).Assembly };
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (null, assemblies);
      Assert.That (configuration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)), Is.True);
      Assert.That (configuration.ClassContexts.ContainsWithInheritance (typeof (Photo)), Is.True);
    }
  }
}
