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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Samples.PhotoStuff.Variant1;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationGeneralTest
  {
    [Test]
    public void NewMixinConfigurationDoesNotKnowAnyClasses ()
    {
      MixinConfiguration configuration = new MixinConfiguration();
      Assert.AreEqual (0, configuration.ClassContexts.Count);
      Assert.IsFalse (configuration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
    }

    [Test]
    public void BuildFromTestAssembly ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      CheckConfiguration (configuration);
    }

    [Test]
    public void BuildFromTestAssemblies ()
    {
      Assembly[] assemblies = new Assembly[] { typeof (BaseType1).Assembly, typeof (Photo).Assembly };
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (null, assemblies);
      CheckConfiguration (configuration);
    }

    private static void CheckConfiguration (MixinConfiguration configuration)
    {
      Assert.IsTrue (configuration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));

      Assert.That (configuration.ClassContexts, Is.Not.Empty);
      
      ClassContext contextForBaseType1 = configuration.ClassContexts.GetWithInheritance (typeof (BaseType1));
      Assert.AreEqual (2, contextForBaseType1.Mixins.Count);

      Assert.IsTrue (contextForBaseType1.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (contextForBaseType1.Mixins.ContainsKey (typeof (BT1Mixin2)));
    }

    [Test]
    public void NewConfiguration ()
    {
      MixinConfiguration configuration = new MixinConfiguration();
      Assert.AreEqual (0, configuration.ClassContexts.Count);
      Assert.IsFalse (configuration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsNull (configuration.ClassContexts.GetWithInheritance (typeof (BaseType1)));
      Assert.IsEmpty (configuration.ClassContexts);
    }

    [Test]
    public void AddClassContext ()
    {
      MixinConfiguration configuration = new MixinConfiguration();
      ClassContext newContext1 = new ClassContext (typeof (BaseType1));
      configuration.ClassContexts.Add (newContext1);
      Assert.AreEqual (1, configuration.ClassContexts.Count);
      Assert.IsTrue (configuration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.AreSame (newContext1, configuration.ClassContexts.GetWithInheritance (typeof (BaseType1)));
      Assert.Contains (newContext1, configuration.ClassContexts);

      ClassContext newContext2 = new ClassContext (typeof (BaseType2));
      configuration.ClassContexts.Add (newContext2);
      Assert.IsNotNull (newContext2);
      Assert.AreNotSame (newContext1, newContext2);
    }

    [Test]
    public void RemoveClassContext()
    {
      MixinConfiguration configuration = new MixinConfiguration();
      ClassContext newContext1 = new ClassContext (typeof (BaseType1));
      configuration.ClassContexts.Add (newContext1);

      Assert.IsTrue (configuration.ClassContexts.RemoveExact (typeof (BaseType1)));
      Assert.IsFalse (configuration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsFalse (configuration.ClassContexts.RemoveExact (typeof (BaseType1)));
    }

    [Test]
    public void AddOrReplaceClassContext()
    {
      MixinConfiguration configuration = new MixinConfiguration ();
      ClassContext existingContext = new ClassContext (typeof (BaseType2));
      configuration.ClassContexts.Add (existingContext);

      ClassContext replacement = new ClassContext (typeof (BaseType2));
      Assert.AreSame (existingContext, configuration.ClassContexts.GetWithInheritance (typeof (BaseType2)));
      
      configuration.ClassContexts.AddOrReplace (replacement);
      
      Assert.AreNotSame (existingContext, configuration.ClassContexts.GetWithInheritance (typeof (BaseType2)));
      Assert.AreSame (replacement, configuration.ClassContexts.GetWithInheritance (typeof (BaseType2)));

      ClassContext additionalContext = new ClassContext (typeof (BaseType3));
      Assert.IsFalse (configuration.ClassContexts.ContainsWithInheritance (additionalContext.Type));
      configuration.ClassContexts.AddOrReplace (additionalContext);
      Assert.IsTrue (configuration.ClassContexts.ContainsWithInheritance (additionalContext.Type));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "already added", MatchType = MessageMatch.Contains)]
    public void ThrowsOnDoubleAdd ()
    {
      MixinConfiguration configuration = new MixinConfiguration ();
      configuration.ClassContexts.Add (new ClassContext (typeof (BaseType1)));
      configuration.ClassContexts.Add (new ClassContext (typeof (BaseType1)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void ThrowsOnDoubleAdd_ViaConstructor ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      configuration.ClassContexts.Add (new ClassContext (typeof (BaseType1)));
    }

    [Test]
    public void Clear ()
    {
      MixinConfiguration configuration = new MixinConfiguration();
      ClassContext classContext = new ClassContext (typeof (object));
      configuration.ClassContexts.Add (classContext);
      configuration.RegisterInterface (typeof (IServiceProvider), classContext);

      Assert.AreEqual (1, configuration.ClassContexts.Count);
      Assert.AreSame (classContext, configuration.ResolveInterface (typeof (IServiceProvider)));

      configuration.Clear();

      Assert.AreEqual (0, configuration.ClassContexts.Count);
      Assert.IsNull (configuration.ResolveInterface (typeof (IServiceProvider)));
    }

    [Test]
    public void CopyTo_Simple ()
    {
      MixinConfiguration source = new MixinConfigurationBuilder (null)
          .ForClass (typeof (BaseType1))
          .AddMixin (typeof (BT1Mixin1)).WithDependency (typeof (IBaseType34))
          .AddCompleteInterface (typeof (IBaseType33))
          .BuildConfiguration ();

      MixinConfiguration destination = new MixinConfiguration ();
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

      MixinConfiguration destination = new MixinConfiguration ();
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
          + "configuration: The interface Remotion.UnitTests.Mixins.SampleTypes.IBaseType33 has already been associated with a class context.\r\n"
          + "Parameter name: destination")]
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
          .BuildConfiguration ();

      destination.RegisterInterface (typeof (IBaseType33), typeof (BaseType2));

      source.CopyTo (destination);
    }

    [Test]
    public void GetContextNonRecursive ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetExact (typeof (DerivedNullTarget));
        Assert.IsNull (context);
      }
    }

    [Test]
    public void GenericTypesNotTransparentlyConvertedToTypeDefinitions ()
    {
      MixinConfiguration configuration = new MixinConfiguration ();
      configuration.ClassContexts.Add (new ClassContext (typeof (List<int>)));
      Assert.IsTrue (configuration.ClassContexts.ContainsWithInheritance (typeof (List<int>)));
      Assert.IsFalse (configuration.ClassContexts.ContainsWithInheritance (typeof (List<>)));

      configuration.ClassContexts.Add (new ClassContext (typeof (List<string>)));

      Assert.AreEqual (2, configuration.ClassContexts.Count);
      configuration.ClassContexts.AddOrReplace (new ClassContext (typeof (List<double>)));
      Assert.AreEqual (3, configuration.ClassContexts.Count);

      ClassContext classContext1 = configuration.ClassContexts.GetWithInheritance (typeof (List<int>));
      ClassContext classContext2 = configuration.ClassContexts.GetExact (typeof (List<string>));
      Assert.AreNotSame (classContext1, classContext2);

      ClassContext classContext3 = configuration.ClassContexts.GetWithInheritance (typeof (List<List<int>>));
      Assert.IsNull (classContext3);

      Assert.IsFalse (configuration.ClassContexts.RemoveExact (typeof (List<bool>)));
      Assert.AreEqual (3, configuration.ClassContexts.Count);
      Assert.IsTrue (configuration.ClassContexts.RemoveExact (typeof (List<int>)));
      Assert.AreEqual (2, configuration.ClassContexts.Count);
      Assert.IsFalse (configuration.ClassContexts.RemoveExact (typeof (List<int>)));
    }

    [Test]
    public void AddContextForGenericSpecialization ()
    {
      MixinConfiguration configuration = new MixinConfiguration ();
      configuration.ClassContexts.Add (new ClassContext (typeof (List<>)));

      Assert.AreEqual (1, configuration.ClassContexts.Count);
      Assert.IsTrue (configuration.ClassContexts.ContainsWithInheritance (typeof (List<int>)));
      Assert.IsTrue (configuration.ClassContexts.ContainsWithInheritance (typeof (List<>)));

      Assert.AreNotSame (configuration.ClassContexts.GetWithInheritance (typeof (List<>)), configuration.ClassContexts.GetWithInheritance (typeof (List<int>)));

      configuration.ClassContexts.Add (new ClassContext (typeof (List<int>)));

      Assert.AreEqual (2, configuration.ClassContexts.Count);
      Assert.IsTrue (configuration.ClassContexts.ContainsWithInheritance (typeof (List<int>)));
      Assert.IsTrue (configuration.ClassContexts.ContainsWithInheritance (typeof (List<>)));

      Assert.AreNotSame (configuration.ClassContexts.GetWithInheritance (typeof (List<>)), configuration.ClassContexts.GetWithInheritance (typeof (List<int>)));
    }

    [Test]
    public void AddOrReplaceContextForGenericSpecialization ()
    {
      MixinConfiguration configuration = new MixinConfiguration ();
      configuration.ClassContexts.Add (new ClassContext (typeof (List<>)));

      Assert.AreEqual (1, configuration.ClassContexts.Count);
      Assert.IsTrue (configuration.ClassContexts.ContainsWithInheritance (typeof (List<int>)));
      Assert.IsTrue (configuration.ClassContexts.ContainsWithInheritance (typeof (List<>)));

      Assert.AreNotSame (configuration.ClassContexts.GetWithInheritance (typeof (List<>)), configuration.ClassContexts.GetWithInheritance (typeof (List<int>)));

      ClassContext listIntContext = new ClassContext (typeof (List<int>));
      configuration.ClassContexts.AddOrReplace (listIntContext);

      Assert.AreEqual (2, configuration.ClassContexts.Count);
      Assert.IsTrue (configuration.ClassContexts.ContainsWithInheritance (typeof (List<int>)));
      Assert.IsTrue (configuration.ClassContexts.ContainsWithInheritance (typeof (List<>)));

      Assert.AreNotSame (configuration.ClassContexts.GetWithInheritance (typeof (List<>)), configuration.ClassContexts.GetWithInheritance (typeof (List<int>)));
      Assert.AreSame (listIntContext, configuration.ClassContexts.GetWithInheritance (typeof (List<int>)));

      ClassContext newListIntContext = new ClassContext (typeof (List<int>));
      configuration.ClassContexts.AddOrReplace (newListIntContext);
      Assert.AreEqual (2, configuration.ClassContexts.Count);

      Assert.AreSame (newListIntContext, configuration.ClassContexts.GetWithInheritance (typeof (List<int>)));
    }

    [Test]
    public void GetContextForGenericTypeDefinitions ()
    {
      MixinConfiguration configuration = new MixinConfiguration ();
      configuration.ClassContexts.Add (new ClassContext (typeof (List<>)));
      Assert.IsTrue (configuration.ClassContexts.ContainsWithInheritance (typeof (List<int>)));
      Assert.IsTrue (configuration.ClassContexts.ContainsWithInheritance (typeof (List<>)));

      ClassContext classContext1 = configuration.ClassContexts.GetWithInheritance (typeof (List<int>));
      ClassContext classContext2 = configuration.ClassContexts.GetExact (typeof (List<>));
      Assert.AreNotSame (classContext1, classContext2);
    }

    [Test]
    public void GetOrAddContextForGenericTypeDefinitions ()
    {
      MixinConfiguration configuration = new MixinConfiguration ();
      ClassContext genericListContext = new ClassContext (typeof (List<>));
      configuration.ClassContexts.Add (genericListContext);

      ClassContext listIntContext = configuration.ClassContexts.GetWithInheritance (typeof (List<int>));
      ClassContext listListContext = configuration.ClassContexts.GetWithInheritance (typeof (List<List<int>>));
      Assert.AreNotSame (listIntContext, listListContext);
      Assert.AreNotEqual (listIntContext, listListContext);
      Assert.IsNotNull (listListContext);

      ClassContext listListContext2 = configuration.ClassContexts.GetWithInheritance (typeof (List<List<int>>));
      Assert.AreNotSame (listListContext, listListContext2);
      Assert.AreEqual (listListContext, listListContext2);

      ClassContext genericListContext2 = configuration.ClassContexts.GetWithInheritance (typeof (List<>));
      Assert.AreSame (genericListContext, genericListContext2);
    }

    [Test]
    public void RemoveClassContextForGenericTypeDefinitions ()
    {
      MixinConfiguration configuration = new MixinConfiguration ();
      configuration.ClassContexts.Add (new ClassContext (typeof (List<>)));
      Assert.IsFalse (configuration.ClassContexts.RemoveExact (typeof (List<int>)));
      Assert.AreEqual (1, configuration.ClassContexts.Count);
      
      configuration.ClassContexts.Add (new ClassContext (typeof (List<int>)));
      Assert.AreEqual (2, configuration.ClassContexts.Count);
      
      Assert.IsTrue (configuration.ClassContexts.RemoveExact (typeof (List<int>)));
      Assert.AreEqual (1, configuration.ClassContexts.Count);
      Assert.IsFalse (configuration.ClassContexts.RemoveExact (typeof (List<int>)));
      Assert.AreEqual (1, configuration.ClassContexts.Count);

      Assert.IsTrue (configuration.ClassContexts.RemoveExact (typeof (List<>)));
      Assert.AreEqual (0, configuration.ClassContexts.Count);
      Assert.IsFalse (configuration.ClassContexts.RemoveExact (typeof (List<>)));
      Assert.AreEqual (0, configuration.ClassContexts.Count);
    }
  }
}
