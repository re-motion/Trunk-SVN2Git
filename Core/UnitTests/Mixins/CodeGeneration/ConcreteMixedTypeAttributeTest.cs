/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class ConcreteMixedTypeAttributeTest
  {
    [ConcreteMixedType (typeof (ConcreteMixedTypeAttributeTest),
        new MixinKind[] {MixinKind.Extending, MixinKind.Used, MixinKind.Extending},
        new Type[] {typeof (string), typeof (object), typeof (int)},
        new Type[] {typeof (int)},
        new Type[] {typeof (object), typeof (double), typeof (bool), typeof (NextMixinDependency), typeof (string), typeof (bool)})]
    private class TestType
    {
    }

    [Test]
    public void FromAttributeApplication ()
    {
      ConcreteMixedTypeAttribute attribute = ((ConcreteMixedTypeAttribute[]) typeof (TestType).GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false))[0];

      Assert.AreEqual (typeof (ConcreteMixedTypeAttributeTest), attribute.TargetType);

      Assert.That (attribute.MixinKinds, Is.EqualTo (new object[] { MixinKind.Extending, MixinKind.Used, MixinKind.Extending }));
      Assert.That (attribute.MixinTypes, Is.EqualTo (new object[] { typeof (string), typeof (object), typeof (int) }));
      Assert.That (attribute.CompleteInterfaces, Is.EqualTo (new object[] { typeof (int) }));
      Assert.That (attribute.ExplicitDependenciesPerMixin, Is.EqualTo (new object[] { typeof (object), typeof (double), typeof (bool), 
          typeof (NextMixinDependency), typeof (string), typeof (bool) }));
    }

    [Test]
    public void FromClassContextSimple ()
    {
      ClassContext simpleContext = new ClassContext (typeof (object), typeof (string));
      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (simpleContext);

      Assert.AreEqual (typeof (object), attribute.TargetType);
      Assert.That (attribute.MixinTypes, Is.EqualTo (new object[] {typeof (string)}));
      Assert.That (attribute.CompleteInterfaces, Is.Empty);
      Assert.AreEqual (0, attribute.CompleteInterfaces.Length);
      Assert.AreEqual (0, attribute.ExplicitDependenciesPerMixin.Length);
    }

    [Test]
    public void FromClassContextComplex ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (string)).WithDependency (typeof (bool))
          .AddMixin (typeof (double)).WithDependency (typeof (int))
          .BuildClassContext();

      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (context);

      Assert.AreEqual (typeof (int), attribute.TargetType);
      Assert.That (attribute.MixinTypes, Is.EqualTo (new object[] { typeof (string), typeof (double) }));
      Assert.That (attribute.CompleteInterfaces, Is.EqualTo (new object[] { typeof (uint) }));
      Assert.That (attribute.ExplicitDependenciesPerMixin, Is.EqualTo (new object[] { typeof (string), typeof (bool), 
          typeof (NextMixinDependency), typeof (double), typeof (int) }));
    }

    [Test]
    public void FromClassContext_MixinKinds ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (string)).OfKind (MixinKind.Extending)
          .AddMixin (typeof (double)).OfKind (MixinKind.Used)
          .BuildClassContext ();

      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (context);
      Assert.That (attribute.MixinKinds, Is.EqualTo (new object[] { MixinKind.Extending, MixinKind.Used }));
    }

    [Test]
    public void GetClassContextSimple ()
    {
      ClassContext simpleContext = new ClassContext (typeof (object), typeof (string));
      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (simpleContext);
      ClassContext regeneratedContext = attribute.GetClassContext();

      Assert.AreEqual (regeneratedContext, simpleContext);
      Assert.AreNotSame (regeneratedContext, simpleContext);
    }

    [Test]
    public void GetClassContextComplex ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddMixin (typeof (double))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (string)).WithDependency (typeof (bool))
          .BuildClassContext();

      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (context);
      ClassContext regeneratedContext = attribute.GetClassContext ();

      Assert.AreEqual (regeneratedContext, context);
      Assert.AreNotSame (regeneratedContext, context);
    }

    [Test]
    public void GetClassContext_MixinKinds ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (string)).OfKind (MixinKind.Extending)
          .AddMixin (typeof (double)).OfKind (MixinKind.Used)
          .BuildClassContext ();

      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (context);
      ClassContext regeneratedContext = attribute.GetClassContext ();
      Assert.That (regeneratedContext.Mixins[typeof (string)].MixinKind, Is.EqualTo (MixinKind.Extending));
      Assert.That (regeneratedContext.Mixins[typeof (double)].MixinKind, Is.EqualTo (MixinKind.Used));
    }

    [Test]
    public void DependencyParsing ()
    {
      ConcreteMixedTypeAttribute attribute = ((ConcreteMixedTypeAttribute[]) typeof (TestType).GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false))[0];
      ClassContext context = attribute.GetClassContext ();

      Assert.AreEqual (3, context.Mixins.Count);

      Assert.That (context.Mixins[typeof (object)].ExplicitDependencies, Is.EqualTo (new object[] { typeof (double), typeof (bool) }));
      Assert.That (context.Mixins[typeof (string)].ExplicitDependencies, Is.EqualTo (new object[] { typeof (bool) }));
      Assert.That (context.Mixins[typeof (int)].ExplicitDependencies, Is.Empty);
    }

    [Test]
    public void GetTargetClassDefinition ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (BaseType3));
      TargetClassDefinition referenceDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (context);

      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (context);
      TargetClassDefinition definition = attribute.GetTargetClassDefinition ();
      Assert.AreSame (referenceDefinition, definition);
    }

    [Test]
    public void AttributeWithGenericType ()
    {
      ClassContext context = new ClassContext (typeof (List<>)).SpecializeWithTypeArguments (new Type[] {typeof (int)});
      Assert.AreEqual (typeof (List<int>), context.Type);
      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (context);
      Assert.AreEqual (typeof (List<int>), attribute.TargetType);
      ClassContext context2 = attribute.GetClassContext ();
      Assert.AreEqual (typeof (List<int>), context2.Type);

      TargetClassDefinition definition = attribute.GetTargetClassDefinition ();
      Assert.AreEqual (typeof (List<int>), definition.Type);
    }
  }
}
