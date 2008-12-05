// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class DeclarativeConfigurationBuilderInheritanceTest
  {
    [Uses (typeof (NullMixin))]
    public class Base { }

    public class Derived : Base { }

    [Uses (typeof (NullMixin2))]
    public class DerivedWithOwnMixin : Base { }

    public class DerivedDerived : Derived { }

    [Uses (typeof (NullMixin2))]
    public class DerivedDerivedWithOwnMixin : Derived { }

    [Uses (typeof (DerivedNullMixin))]
    public class DerivedWithOverride : Base { }

    [Uses (typeof (DerivedNullMixin))]
    public class DerivedDerivedWithOwnMixinAndOverride : DerivedDerivedWithOwnMixin { }

    [Test]
    public void BaseContext ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (Base));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedContext ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (Derived));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedContextWithOwnMixin ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (DerivedWithOwnMixin));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedContext ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (DerivedDerived));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedContextWithOwnMixin ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (DerivedDerivedWithOwnMixin));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedContextWithOwnMixin_Order1 ()
    {
      DeclarativeConfigurationBuilder builder = new DeclarativeConfigurationBuilder (null);
      builder.AddType (typeof (Base));
      builder.AddType (typeof (Derived));
      builder.AddType (typeof (DerivedDerivedWithOwnMixin));

      MixinConfiguration context = builder.BuildConfiguration();
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (DerivedDerivedWithOwnMixin));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedContextWithOwnMixin_Order2 ()
    {
      DeclarativeConfigurationBuilder builder = new DeclarativeConfigurationBuilder (null);
      builder.AddType (typeof (DerivedDerivedWithOwnMixin));
      builder.AddType (typeof (Derived));
      builder.AddType (typeof (Base));

      MixinConfiguration context = builder.BuildConfiguration ();
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (DerivedDerivedWithOwnMixin));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }


    [Test]
    public void DerivedContextWithOverride ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (DerivedWithOverride));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (DerivedNullMixin)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedContextWithOverrideAndOverride ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (DerivedDerivedWithOwnMixinAndOverride));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (DerivedNullMixin)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }
  }
}
