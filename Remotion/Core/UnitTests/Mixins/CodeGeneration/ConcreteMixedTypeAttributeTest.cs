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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Context.Serialization;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class ConcreteMixedTypeAttributeTest
  {
    [Test]
    public void FromAttributeApplication ()
    {
      ConcreteMixedTypeAttribute attribute = ((ConcreteMixedTypeAttribute[]) typeof (LoadableConcreteMixedTypeForBaseType1).GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false)).Single();
      var targetClassDefinition = attribute.GetTargetClassDefinition (TargetClassDefinitionCache.Current);

      Assert.That (targetClassDefinition.Type, Is.EqualTo (typeof (BaseType1)));
      Assert.That (targetClassDefinition.Mixins.Count, Is.EqualTo (1));
      Assert.That (targetClassDefinition.Mixins[0].Type, Is.EqualTo (typeof (BT1Mixin1)));
      Assert.That (targetClassDefinition.Mixins[0].MixinKind, Is.EqualTo (MixinKind.Used));
      Assert.That (targetClassDefinition.ConfigurationContext.Mixins[typeof (BT1Mixin1)].IntroducedMemberVisibility, Is.EqualTo (MemberVisibility.Private));
      Assert.That (targetClassDefinition.Mixins[0].MixinDependencies.Count, Is.EqualTo (0));
      Assert.That (targetClassDefinition.ConfigurationContext.CompleteInterfaces, Is.Empty);
    }

    [Test]
    public void FromClassContextSimple ()
    {
      var simpleContext = new ClassContext (typeof (object), typeof (string));
      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (simpleContext);

      var deserializer = new AttributeClassContextDeserializer (attribute.Data);
      Assert.That (ClassContext.Deserialize (deserializer), Is.EqualTo (simpleContext));
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

      var deserializer = new AttributeClassContextDeserializer (attribute.Data);
      Assert.That (ClassContext.Deserialize (deserializer), Is.EqualTo (context));
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
      var deserializer = new AttributeClassContextDeserializer (attribute.Data);
      Assert.That (ClassContext.Deserialize (deserializer), Is.EqualTo (context));
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
    public void GetClassContext_Dependencies ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddMixin (typeof (object)).OfKind (MixinKind.Extending).WithDependencies (typeof (double), typeof (bool))
          .AddMixin (typeof (string)).OfKind (MixinKind.Extending).WithDependencies (typeof (bool))
          .AddMixin (typeof (int)).OfKind (MixinKind.Extending)
          .BuildClassContext ();

      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (context);
      ClassContext regeneratedContext = attribute.GetClassContext ();

      Assert.AreEqual (3, regeneratedContext.Mixins.Count);

      Assert.That (regeneratedContext.Mixins[typeof (object)].ExplicitDependencies, Is.EqualTo (new object[] { typeof (double), typeof (bool) }));
      Assert.That (regeneratedContext.Mixins[typeof (string)].ExplicitDependencies, Is.EqualTo (new object[] { typeof (bool) }));
      Assert.That (regeneratedContext.Mixins[typeof (int)].ExplicitDependencies, Is.Empty);
    }

    [Test]
    public void GetTargetClassDefinition ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (BaseType3));
      TargetClassDefinition referenceDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (context);

      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (context);
      TargetClassDefinition definition = attribute.GetTargetClassDefinition (TargetClassDefinitionCache.Current);
      Assert.AreSame (referenceDefinition, definition);
    }

    [Test]
    public void AttributeWithGenericType ()
    {
      ClassContext context = new ClassContext (typeof (List<>)).SpecializeWithTypeArguments (new[] {typeof (int)});
      Assert.AreEqual (typeof (List<int>), context.Type);
      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (context);
      ClassContext context2 = attribute.GetClassContext ();
      Assert.AreEqual (typeof (List<int>), context2.Type);

      TargetClassDefinition definition = attribute.GetTargetClassDefinition (TargetClassDefinitionCache.Current);
      Assert.AreEqual (typeof (List<int>), definition.Type);
    }

    [Test]
    public void Roundtrip_WithPublicVisibility_IntegrationTest ()
    {
      var classContext = new ClassContext (typeof (BaseType1), new MixinContext (MixinKind.Used, typeof (BT1Mixin1), MemberVisibility.Public));
      var attribute = ConcreteMixedTypeAttribute.FromClassContext (classContext);
      var classContext2 = attribute.GetClassContext ();

      Assert.That (classContext2, Is.EqualTo (classContext));
    }
  }
}
