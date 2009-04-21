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
  public class ConcreteMixinTypeAttributeTest
  {
    [Test]
    public void FromAttributeApplication ()
    {
      ConcreteMixinTypeAttribute attribute = ((ConcreteMixinTypeAttribute[]) typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers).GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false)).Single();
      var mixinDefinition = attribute.GetMixinDefinition (TargetClassDefinitionCache.Current);

      Assert.That (mixinDefinition.MixinIndex, Is.EqualTo (0));
      Assert.That (mixinDefinition.Type, Is.EqualTo (typeof (MixinWithAbstractMembers)));
      Assert.That (mixinDefinition.TargetClass.Type, Is.EqualTo (typeof (ClassOverridingMixinMembers)));
      Assert.That (mixinDefinition.MixinKind, Is.EqualTo (MixinKind.Used));
      Assert.That (mixinDefinition.TargetClass.ConfigurationContext.Mixins[typeof (MixinWithAbstractMembers)].IntroducedMemberVisibility, Is.EqualTo (MemberVisibility.Private));
      Assert.That (mixinDefinition.MixinDependencies.Count, Is.EqualTo (0));
      Assert.That (mixinDefinition.TargetClass.ConfigurationContext.CompleteInterfaces, Is.Empty);
    }

    [Test]
    public void FromClassContextSimple ()
    {
      var simpleContext = new ClassContext (typeof (object), typeof (string));
      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (7, simpleContext);

      Assert.That (attribute.MixinIndex, Is.EqualTo (7));
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

      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (5, context);

      Assert.That (attribute.MixinIndex, Is.EqualTo (5));
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

      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (5, context);

      Assert.That (attribute.MixinIndex, Is.EqualTo (5));
      var deserializer = new AttributeClassContextDeserializer (attribute.Data);
      Assert.That (ClassContext.Deserialize (deserializer), Is.EqualTo (context));
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
  }
}
