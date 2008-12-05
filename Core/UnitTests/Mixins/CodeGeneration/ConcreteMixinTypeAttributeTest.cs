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
  public class ConcreteMixinTypeAttributeTest
  {
    [ConcreteMixinType (3, typeof (ConcreteMixinTypeAttributeTest),
        new[] {MixinKind.Extending, MixinKind.Used, MixinKind.Extending}, 
        new[] {typeof (string), typeof (object), typeof (int)},
        new[] {typeof (int)},
        new[] {typeof (object), typeof (double), typeof (bool), typeof (NextMixinDependency), typeof (string), typeof (bool)})]
    private class TestType
    {
    }

    [Test]
    public void FromAttributeApplication ()
    {
      ConcreteMixinTypeAttribute attribute = ((ConcreteMixinTypeAttribute[]) typeof (TestType).GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false))[0];

      Assert.That (attribute.MixinIndex, Is.EqualTo (3));
      Assert.That (attribute.TargetType, Is.EqualTo (typeof (ConcreteMixinTypeAttributeTest)));
      Assert.That (attribute.MixinKinds, Is.EqualTo (new object[] {MixinKind.Extending, MixinKind.Used, MixinKind.Extending}));
      Assert.That (attribute.MixinTypes, Is.EqualTo (new object[] { typeof (string), typeof (object), typeof (int) }));
      Assert.That (attribute.CompleteInterfaces, Is.EqualTo (new object[] { typeof (int) }));
      Assert.That (attribute.ExplicitDependenciesPerMixin,
          Is.EqualTo (new object[] { typeof (object), typeof (double), typeof (bool), typeof (NextMixinDependency), typeof (string), typeof (bool), }));
    }

    [Test]
    public void FromClassContextSimple ()
    {
      var simpleContext = new ClassContext (typeof (object), typeof (string));
      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (7, simpleContext);

      Assert.That (attribute.MixinIndex, Is.EqualTo (7));
      Assert.That (attribute.TargetType, Is.EqualTo (typeof (object)));
      Assert.That (attribute.MixinTypes, Is.EqualTo (new object[] { typeof (string), }));
      Assert.That (attribute.CompleteInterfaces, Is.Empty);
      Assert.That (attribute.ExplicitDependenciesPerMixin, Is.Empty);
    }

    [Test]
    public void FromClassContextComplex ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (string)).WithDependency (typeof (bool))
          .AddMixin (typeof (double)).WithDependency (typeof (int))
          .BuildClassContext();

      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (5, context);

      Assert.That (attribute.MixinIndex, Is.EqualTo (5));
      Assert.That (attribute.TargetType, Is.EqualTo (typeof (int)));
      Assert.That (attribute.MixinTypes, Is.EqualTo (new object[] { typeof (string), typeof (double), }));
      Assert.That (attribute.CompleteInterfaces, Is.EqualTo (new object[] { typeof (uint), }));
      Assert.That (attribute.ExplicitDependenciesPerMixin, 
          Is.EqualTo (new object[] { typeof (string), typeof (bool), typeof (NextMixinDependency), typeof (double), typeof (int), }));
    }

    [Test]
    public void FromClassContext_MixinKinds ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (string)).OfKind (MixinKind.Extending)
          .AddMixin (typeof (double)).OfKind (MixinKind.Used)
          .BuildClassContext ();

      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (5, context);

      Assert.That (attribute.MixinIndex, Is.EqualTo (5));
      Assert.That (attribute.TargetType, Is.EqualTo (typeof (int)));
      Assert.That (attribute.MixinKinds, Is.EqualTo (new object[] { MixinKind.Extending, MixinKind.Used }));
      Assert.That (attribute.MixinTypes, Is.EqualTo (new object[] { typeof (string), typeof (double), }));
      Assert.That (attribute.CompleteInterfaces, Is.EqualTo (new object[] { typeof (uint), }));
      Assert.That (attribute.ExplicitDependenciesPerMixin, Is.Empty);
    }


    [Test]
    public void GetMixinDefinition ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (BaseType3));
      MixinDefinition referenceDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (context).Mixins[0];

      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (0, context);
      MixinDefinition definition = attribute.GetMixinDefinition (TargetClassDefinitionCache.Current);
      Assert.That (definition, Is.SameAs (referenceDefinition));
    }

    [Test]
    [Ignore ("TODO FS: COMMONS-829")]
    public void Roundtrip_WithPublicVisibility_IntegrationTest()
    {
      var classContext = new ClassContext (typeof (BaseType1), new MixinContext (MixinKind.Used, typeof (BT1Mixin1), MemberVisibility.Public));
      var attribute = ConcreteMixedTypeAttribute.FromClassContext (classContext);
      var classContext2 = attribute.GetClassContext ();

      Assert.That (classContext2, Is.EqualTo (classContext));
    }
  }
}
