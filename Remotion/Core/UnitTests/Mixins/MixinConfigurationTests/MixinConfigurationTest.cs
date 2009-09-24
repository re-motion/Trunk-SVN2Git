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
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationTest
  {
    [Test]
    public void Initialization ()
    {
      
    }

    [Test]
    public void Initialization_Empty ()
    {
      var configuration = new MixinConfiguration();
      Assert.IsEmpty (configuration.ClassContexts);
    }

    [Test]
    public void CopyTo_Simple ()
    {
      MixinConfiguration source = new MixinConfigurationBuilder (null)
          .ForClass (typeof (BaseType1))
          .AddMixin (typeof (BT1Mixin1)).WithDependency (typeof (IBaseType34))
          .AddCompleteInterface (typeof (IBaseType33))
          .BuildConfiguration ();

      var destination = new MixinConfiguration ();
      source.CopyTo (destination);
      Assert.That(destination.ClassContexts, Is.EquivalentTo(source.ClassContexts));
      Assert.IsNotNull (destination.ResolveInterface (typeof (IBaseType33)));
      Assert.AreSame (destination.ClassContexts.GetExact (typeof (BaseType1)), destination.ResolveInterface (typeof (IBaseType33)));
    }

    [Test]
    public void CopyTo_WithParent ()
    {
      MixinConfiguration parent = new MixinConfigurationBuilder (null)
          .ForClass (typeof (BaseType2))
          .AddMixin (typeof (BT2Mixin1))
          .AddCompleteInterface (typeof (IBaseType31))
          .BuildConfiguration();
      
      MixinConfiguration source = new MixinConfigurationBuilder (parent)
          .ForClass (typeof (BaseType1))
          .AddMixin (typeof (BT1Mixin1)).WithDependency (typeof (IBaseType34))
          .AddCompleteInterface (typeof (IBaseType33))
          .BuildConfiguration ();

      var destination = new MixinConfiguration ();
      source.CopyTo (destination);
      Assert.That(destination.ClassContexts, Is.EquivalentTo(source.ClassContexts));
      Assert.IsNotNull (destination.ResolveInterface (typeof (IBaseType33)));
      Assert.AreSame (destination.ClassContexts.GetExact (typeof (BaseType1)), destination.ResolveInterface (typeof (IBaseType33)));
      Assert.IsNotNull (destination.ResolveInterface (typeof (IBaseType31)));
      Assert.AreSame (destination.ClassContexts.GetExact (typeof (BaseType2)), destination.ResolveInterface (typeof (IBaseType31)));
    }

    [Test]
    public void CopyTo_WithReplacement ()
    {
      MixinConfiguration source = new MixinConfigurationBuilder (null)
          .ForClass (typeof (BaseType1))
          .AddMixin (typeof (BT1Mixin1)).WithDependency (typeof (IBaseType34))
          .AddCompleteInterface (typeof (IBaseType33))
          .BuildConfiguration ();

      MixinConfiguration destination = new MixinConfigurationBuilder (null)
          .ForClass (typeof (BaseType1))
          .AddMixin (typeof (BT1Mixin2)).WithDependency (typeof (IBaseType35))
          .AddCompleteInterface (typeof (IBaseType34))
          .BuildConfiguration ();

      source.CopyTo (destination);

      Assert.That(destination.ClassContexts, Is.EquivalentTo(source.ClassContexts));
      Assert.IsNotNull (destination.ResolveInterface (typeof (IBaseType33)));
      Assert.AreSame (destination.ClassContexts.GetExact (typeof (BaseType1)), destination.ResolveInterface (typeof (IBaseType33)));
      Assert.IsNull (destination.ResolveInterface (typeof (IBaseType35)));
    }

    [Test]
    public void CopyTo_WithAddition ()
    {
      MixinConfiguration source = new MixinConfigurationBuilder (null)
          .ForClass (typeof (BaseType1))
          .AddMixin (typeof (BT1Mixin1)).WithDependency (typeof (IBaseType34))
          .AddCompleteInterface (typeof (IBaseType33))
          .BuildConfiguration ();

      MixinConfiguration destination = new MixinConfigurationBuilder (null)
          .ForClass (typeof (BaseType2))
          .AddMixin (typeof (BT1Mixin2)).WithDependency (typeof (IBaseType35))
          .AddCompleteInterface (typeof (IBaseType34))
          .BuildConfiguration ();

      source.CopyTo (destination);

      Assert.IsTrue (destination.ClassContexts.ContainsExact (typeof (BaseType1)));
      Assert.IsTrue (destination.ClassContexts.ContainsExact (typeof (BaseType2)));

      Assert.AreSame (destination.ClassContexts.GetExact (typeof (BaseType1)), destination.ResolveInterface (typeof (IBaseType33)));
      Assert.AreSame (destination.ClassContexts.GetExact (typeof (BaseType2)), destination.ResolveInterface (typeof (IBaseType34)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The given destination configuration object conflicts with the source "
          + "configuration: There is an ambiguity in complete interfaces: interface 'Remotion.UnitTests.Mixins.SampleTypes.IBaseType33' refers to "
          + "both class", MatchType = MessageMatch.Contains)] // cannot write full message, order undefined
    public void CopyTo_ThrowsWhenConflictWithRegisteredInterface ()
    {
      MixinConfiguration source = new MixinConfigurationBuilder (null)
          .ForClass (typeof (BaseType1))
          .AddMixin (typeof (BT1Mixin1)).WithDependency (typeof (IBaseType34))
          .AddCompleteInterface (typeof (IBaseType33))
          .BuildConfiguration ();

      MixinConfiguration destination = new MixinConfigurationBuilder (null)
          .ForClass (typeof (BaseType2))
          .AddMixin (typeof (BT1Mixin2)).WithDependency (typeof (IBaseType35))
          .AddCompleteInterface (typeof (IBaseType33))
          .BuildConfiguration ();

      source.CopyTo (destination);
    }

    [Test]
    public void GetContext_Configured ()
    {
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)), Is.True);

      var context = MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1));
      Assert.That (context, Is.SameAs (MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (BaseType1))));
    }

    [Test]
    public void GetContext_Configured_ButEmpty ()
    {
      var configuration = MixinConfiguration.BuildNew ().ForClass<NullTarget> ().BuildConfiguration ();
      Assert.That (configuration.ClassContexts.ContainsExact (typeof (NullTarget)), Is.True);

      var context = configuration.GetContext (typeof (NullTarget));
      Assert.That (context, Is.Null);
    }

    [Test]
    public void GetContext_ReturnsNull_IfNotConfigured ()
    {
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)), Is.False);

      var context = MixinConfiguration.ActiveConfiguration.GetContext (typeof (object));
      Assert.That (context, Is.Null);
    }

    [Test]
    public void GetContext_NoNewContext_GeneratedForGeneratedType ()
    {
      var expectedContext = MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1));

      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
      var actualContext = MixinConfiguration.ActiveConfiguration.GetContext (generatedType);
      Assert.That (actualContext, Is.EqualTo (expectedContext));
    }

    [Test]
    public void GetContextForce_Configured ()
    {
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)), Is.True);

      var context = MixinConfiguration.ActiveConfiguration.GetContextForce (typeof (BaseType1));
      Assert.That (context, Is.SameAs (MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (BaseType1))));
    }

    [Test]
    public void GetContextForce_ReturnsNew_IfNotConfigured ()
    {
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)), Is.False);

      var context = MixinConfiguration.ActiveConfiguration.GetContextForce (typeof (object));
      Assert.That (context, Is.Not.Null);
      Assert.That (context.Type, Is.SameAs (typeof (object)));
    }

    [Test]
    public void GetContextForce_NewContext_GeneratedForGeneratedType ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
      var newContext = MixinConfiguration.ActiveConfiguration.GetContextForce (generatedType);
      var baseContext = MixinConfiguration.ActiveConfiguration.ClassContexts.GetExact (typeof (BaseType1));

      Assert.That (newContext, Is.Not.EqualTo (baseContext));
      Assert.That (newContext.Type, Is.SameAs (generatedType));
    }

    [Test]
    public void GetContextForce_ResultNotCached ()
    {
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)), Is.False);
      var context1 = MixinConfiguration.ActiveConfiguration.GetContextForce (typeof (object));
      var context2 = MixinConfiguration.ActiveConfiguration.GetContext (typeof (object));

      Assert.That (context1, Is.Not.Null);
      Assert.That (context2, Is.Null);
    }

  }
}
