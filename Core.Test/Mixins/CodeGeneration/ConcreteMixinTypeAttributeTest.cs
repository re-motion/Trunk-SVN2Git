using System;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Definitions;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class ConcreteMixinTypeAttributeTest
  {
    [ConcreteMixinType (3, typeof (ConcreteMixinTypeAttributeTest),
        new Type[] {typeof (string), typeof (object), typeof (int)},
        new Type[] {typeof (int)},
        new Type[] {typeof (object), typeof (double), typeof (bool), typeof (NextMixinDependency), typeof (string), typeof (bool)})]
    private class TestType
    {
    }

    [Test]
    public void FromAttributeApplication ()
    {
      ConcreteMixinTypeAttribute attribute = ((ConcreteMixinTypeAttribute[]) typeof (TestType).GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false))[0];

      Assert.AreEqual (3, attribute.MixinIndex);

      Assert.AreEqual (typeof (ConcreteMixinTypeAttributeTest), attribute.TargetType);
      
      Assert.AreEqual (3, attribute.MixinTypes.Length);
      Assert.AreEqual (typeof (string), attribute.MixinTypes[0]);
      Assert.AreEqual (typeof (object), attribute.MixinTypes[1]);
      Assert.AreEqual (typeof (int), attribute.MixinTypes[2]);

      Assert.AreEqual (1, attribute.CompleteInterfaces.Length);
      Assert.AreEqual (typeof (int), attribute.CompleteInterfaces[0]);

      Assert.AreEqual (6, attribute.ExplicitDependenciesPerMixin.Length);
      Assert.AreEqual (typeof (object), attribute.ExplicitDependenciesPerMixin[0]);
      Assert.AreEqual (typeof (double), attribute.ExplicitDependenciesPerMixin[1]);
      Assert.AreEqual (typeof (bool), attribute.ExplicitDependenciesPerMixin[2]);
      Assert.AreEqual (typeof (NextMixinDependency), attribute.ExplicitDependenciesPerMixin[3]);
      Assert.AreEqual (typeof (string), attribute.ExplicitDependenciesPerMixin[4]);
      Assert.AreEqual (typeof (bool), attribute.ExplicitDependenciesPerMixin[5]);
    }

    [Test]
    public void FromClassContextSimple ()
    {
      ClassContext simpleContext = new ClassContext (typeof (object), typeof (string));
      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (7, simpleContext);

      Assert.AreEqual (7, attribute.MixinIndex);
      Assert.AreEqual (typeof (object), attribute.TargetType);
      Assert.AreEqual (1, attribute.MixinTypes.Length);
      Assert.AreEqual (typeof (string), attribute.MixinTypes[0]);
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

      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (5, context);

      Assert.AreEqual (5, attribute.MixinIndex);

      Assert.AreEqual (typeof (int), attribute.TargetType);
      Assert.AreEqual (2, attribute.MixinTypes.Length);
      Assert.AreEqual (typeof (string), attribute.MixinTypes[0]);
      Assert.AreEqual (typeof (double), attribute.MixinTypes[1]);
      
      Assert.AreEqual (1, attribute.CompleteInterfaces.Length);
      Assert.AreEqual (typeof (uint), attribute.CompleteInterfaces[0]);

      Assert.AreEqual (5, attribute.ExplicitDependenciesPerMixin.Length);
      Assert.AreEqual (typeof (string), attribute.ExplicitDependenciesPerMixin[0]);
      Assert.AreEqual (typeof (bool), attribute.ExplicitDependenciesPerMixin[1]);
      Assert.AreEqual (typeof (NextMixinDependency), attribute.ExplicitDependenciesPerMixin[2]);
      Assert.AreEqual (typeof (double), attribute.ExplicitDependenciesPerMixin[3]);
      Assert.AreEqual (typeof (int), attribute.ExplicitDependenciesPerMixin[4]);
    }

    [Test]
    public void GetMixinDefinition ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (BaseType3));
      MixinDefinition referenceDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (context).Mixins[0];

      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (0, context);
      MixinDefinition definition = attribute.GetMixinDefinition ();
      Assert.AreSame (referenceDefinition, definition);
    }
  }
}