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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Context.Serialization;
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
      var attribute = ((ConcreteMixedTypeAttribute[]) 
          typeof (LoadableConcreteMixedTypeForBaseType1).GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false)).Single();
      var classContext = attribute.GetClassContext ();

      var expectedContext = new ClassContext (typeof (BaseType1), new MixinContext (MixinKind.Used, typeof (BT1Mixin1), MemberVisibility.Private));
      Assert.That (classContext, Is.EqualTo (expectedContext));
    }

    [Test]
    public void FromClassContextSimple ()
    {
      var simpleContext = new ClassContext (typeof (object), typeof (string));
      ConcreteMixedTypeAttribute attribute = CreateAttribute (simpleContext);

      var deserializer = new AttributeClassContextDeserializer (attribute.ClassContextData);
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

      ConcreteMixedTypeAttribute attribute = CreateAttribute (context);

      var deserializer = new AttributeClassContextDeserializer (attribute.ClassContextData);
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

      ConcreteMixedTypeAttribute attribute = CreateAttribute (context);
      var deserializer = new AttributeClassContextDeserializer (attribute.ClassContextData);
      Assert.That (ClassContext.Deserialize (deserializer), Is.EqualTo (context));
    }

    [Test]
    public void GetClassContextSimple ()
    {
      var simpleContext = new ClassContext (typeof (object), typeof (string));
      ConcreteMixedTypeAttribute attribute = CreateAttribute (simpleContext);
      ClassContext regeneratedContext = attribute.GetClassContext();

      Assert.That (simpleContext, Is.EqualTo (regeneratedContext));
      Assert.That (simpleContext, Is.Not.SameAs (regeneratedContext));
    }

    [Test]
    public void GetClassContextComplex ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddMixin (typeof (double))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (string)).WithDependency (typeof (bool))
          .BuildClassContext();

      ConcreteMixedTypeAttribute attribute = CreateAttribute (context);
      ClassContext regeneratedContext = attribute.GetClassContext ();

      Assert.That (context, Is.EqualTo (regeneratedContext));
      Assert.That (context, Is.Not.SameAs (regeneratedContext));
    }

    [Test]
    public void GetClassContext_MixinKinds ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (string)).OfKind (MixinKind.Extending)
          .AddMixin (typeof (double)).OfKind (MixinKind.Used)
          .BuildClassContext ();

      ConcreteMixedTypeAttribute attribute = CreateAttribute (context);
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

      ConcreteMixedTypeAttribute attribute = CreateAttribute (context);
      ClassContext regeneratedContext = attribute.GetClassContext ();

      Assert.That (regeneratedContext.Mixins.Count, Is.EqualTo (3));

      Assert.That (regeneratedContext.Mixins[typeof (object)].ExplicitDependencies, Is.EqualTo (new object[] { typeof (double), typeof (bool) }));
      Assert.That (regeneratedContext.Mixins[typeof (string)].ExplicitDependencies, Is.EqualTo (new object[] { typeof (bool) }));
      Assert.That (regeneratedContext.Mixins[typeof (int)].ExplicitDependencies, Is.Empty);
    }

    [Test]
    public void AttributeWithGenericType ()
    {
      ClassContext context = new ClassContext (typeof (List<>)).SpecializeWithTypeArguments (new[] {typeof (int)});
      Assert.That (context.Type, Is.EqualTo (typeof (List<int>)));
      ConcreteMixedTypeAttribute attribute = CreateAttribute (context);

      ClassContext regeneratedContext = attribute.GetClassContext ();
      Assert.That (regeneratedContext.Type, Is.EqualTo (typeof (List<int>)));
    }

    [Test]
    public void Roundtrip_WithPublicVisibility_IntegrationTest ()
    {
      var classContext = new ClassContext (typeof (BaseType1), new MixinContext (MixinKind.Used, typeof (BT1Mixin1), MemberVisibility.Public));
      var attribute = CreateAttribute (classContext);
      var classContext2 = attribute.GetClassContext ();

      Assert.That (classContext2, Is.EqualTo (classContext));
    }

    private ConcreteMixedTypeAttribute CreateAttribute (ClassContext context)
    {
      return ConcreteMixedTypeAttribute.FromClassContext (context, context.Mixins.Select (m => m.MixinType).ToArray());
    }
  }
}
