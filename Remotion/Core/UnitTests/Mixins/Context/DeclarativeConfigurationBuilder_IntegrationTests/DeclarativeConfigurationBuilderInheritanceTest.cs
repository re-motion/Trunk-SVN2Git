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
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly());
      ClassContext classContext = configuration.GetContext (typeof (Base));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedContext ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (Derived));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedContextWithOwnMixin ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedWithOwnMixin));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedContext ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedDerived));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedContextWithOwnMixin ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedDerivedWithOwnMixin));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedContextWithOwnMixin_Order1 ()
    {
      var builder = new DeclarativeConfigurationBuilder (null);
      builder.AddType (typeof (Base));
      builder.AddType (typeof (Derived));
      builder.AddType (typeof (DerivedDerivedWithOwnMixin));

      MixinConfiguration configuration = builder.BuildConfiguration();
      ClassContext classContext = configuration.GetContext (typeof (DerivedDerivedWithOwnMixin));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedContextWithOwnMixin_Order2 ()
    {
      var builder = new DeclarativeConfigurationBuilder (null);
      builder.AddType (typeof (DerivedDerivedWithOwnMixin));
      builder.AddType (typeof (Derived));
      builder.AddType (typeof (Base));

      MixinConfiguration configuration = builder.BuildConfiguration ();
      ClassContext classContext = configuration.GetContext (typeof (DerivedDerivedWithOwnMixin));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }


    [Test]
    public void DerivedContextWithOverride ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedWithOverride));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (DerivedNullMixin)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedContextWithOverrideAndOverride ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedDerivedWithOwnMixinAndOverride));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (DerivedNullMixin)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }
  }
}
