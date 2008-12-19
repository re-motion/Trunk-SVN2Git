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

namespace Remotion.UnitTests.Mixins.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class DeclarativeConfigurationBuilderGenericInheritanceTest
  {
    public class GenericClass<T> { }
    public class DerivedGenericClassFromOpen<T> : GenericClass<T> { }
    public class DerivedGenericClassFromClosed<T> : GenericClass<int> { }
    public class DerivedClassFromClosed : GenericClass<int> { }
    public class DerivedDerivedGenericClassFromOpen<T> : DerivedGenericClassFromOpen<T> { }

    [Extends (typeof (GenericClass<>))]
    public class MixinForOpenGeneric { }

    [Extends (typeof (GenericClass<int>))]
    public class MixinForClosedGeneric { }

    [Extends (typeof (DerivedGenericClassFromOpen<>))]
    public class MixinForDerivedOpenGeneric { }

    [Extends (typeof (DerivedGenericClassFromOpen<int>))]
    public class MixinForDerivedClosedGeneric { }

    [Extends (typeof (DerivedDerivedGenericClassFromOpen<int>))]
    public class MixinForDerivedDerivedClosedGeneric { }

    [Test]
    public void OpenGenericClassContext_Open()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (GenericClass<>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Test]
    public void ClosedGenericClassContext_Closed_NoOwnMixin ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (GenericClass<string>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Test]
    public void ClosedGenericClassContext_Closed_WithOwnMixin ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (GenericClass<int>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedGenericClassFromOpenContext_Open ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (DerivedGenericClassFromOpen<>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedGenericClassFromOpenContext_Closed_NoOwnMixin ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (DerivedGenericClassFromOpen<string>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedGenericClassFromOpenContext_Closed_WithOwnMixin ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (DerivedGenericClassFromOpen<int>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedClosedGeneric)));
      Assert.AreEqual (4, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedGenericClassFromClosedContext_Open ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (DerivedGenericClassFromClosed<>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedGenericClassFromClosedContext_Closed_NoOwnMixins ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (DerivedGenericClassFromClosed<int>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedGenericClassFromOpenContext_Open ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (DerivedDerivedGenericClassFromOpen<>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedGenericClassFromOpenContext_Closed_NoOwnMixin ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (DerivedDerivedGenericClassFromOpen<string>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedGenericClassFromOpenContext_Closed_WithOwnMixin ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (DerivedDerivedGenericClassFromOpen<int>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedClosedGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedDerivedClosedGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedDerivedClosedGeneric)));
      Assert.AreEqual (5, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedClassFromClosedContext ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (DerivedClassFromClosed));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }
  }
}
