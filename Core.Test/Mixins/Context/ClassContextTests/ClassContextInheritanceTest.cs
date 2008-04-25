using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;

namespace Remotion.UnitTests.Mixins.Context.ClassContextTests
{
  [TestFixture]
  public class ClassContextInheritanceTest
  {
    [Test]
    public void ContainsOverrideForMixin_False ()
    {
      ClassContext context = new ClassContext (typeof (string), typeof (NullTarget), typeof (GenericClassExtendedByMixin<>));

      Assert.IsFalse (context.Mixins.ContainsOverrideForMixin (typeof (int))); // completely unrelated
      Assert.IsFalse (context.Mixins.ContainsOverrideForMixin (typeof (DerivedNullTarget))); // subtype
      Assert.IsTrue (context.Mixins.ContainsOverrideForMixin (typeof (GenericClassExtendedByMixin<object>))); // specialization doesn't matter
      Assert.IsFalse (context.Mixins.ContainsOverrideForMixin (typeof (DerivedGenericMixin<>))); // subtype
    }

    [Test]
    public void ContainsOverrideForMixin_Same ()
    {
      ClassContext context = new ClassContext (typeof (string), typeof (NullTarget), typeof (GenericClassExtendedByMixin<>));

      Assert.IsTrue (context.Mixins.ContainsOverrideForMixin (typeof (NullTarget)));
      Assert.IsTrue (context.Mixins.ContainsOverrideForMixin (typeof (GenericClassExtendedByMixin<>)));
    }

    [Test]
    public void ContainsOverrideForMixin_True ()
    {
      ClassContext context = new ClassContext (typeof (string), typeof (DerivedNullTarget), typeof (GenericMixinWithVirtualMethod<object>));
      
      Assert.IsTrue (context.Mixins.ContainsOverrideForMixin (typeof (NullTarget))); // supertype
      Assert.IsTrue (context.Mixins.ContainsOverrideForMixin (typeof (GenericMixinWithVirtualMethod<>))); // less specialized
    }

    [Test]
    public void ContainsOverrideForMixin_DerivedAndSpecialized ()
    {
      ClassContext context = new ClassContext (typeof (string), typeof (DerivedNullTarget), typeof (DerivedGenericMixin<object>));

      Assert.IsTrue (context.Mixins.ContainsOverrideForMixin (typeof (GenericMixinWithVirtualMethod<>)));
      Assert.IsTrue (context.Mixins.ContainsOverrideForMixin (typeof (GenericMixinWithVirtualMethod<object>)));
      Assert.IsTrue (context.Mixins.ContainsOverrideForMixin (typeof (DerivedGenericMixin<>)));
      Assert.IsTrue (context.Mixins.ContainsOverrideForMixin (typeof (DerivedGenericMixin<object>)));
      Assert.IsTrue (context.Mixins.ContainsOverrideForMixin (typeof (GenericMixinWithVirtualMethod<string>))); // different type arguments don't matter
      Assert.IsTrue (context.Mixins.ContainsOverrideForMixin (typeof (DerivedGenericMixin<string>))); // different specialization doesn't matter
    }

    [Test]
    public void Mixins ()
    {
      ClassContext baseContext = new ClassContext (typeof (string), typeof (DateTime), typeof (int));
      ClassContext inheritor = new ClassContext (typeof (double)).InheritFrom (baseContext);

      Assert.AreEqual (2, inheritor.Mixins.Count);
      Assert.That (inheritor.Mixins, Is.EquivalentTo (baseContext.Mixins));
    }

    [Test]
    public void ContainsAssignableMixin ()
    {
      ClassContext baseContext = new ClassContext (typeof (string), typeof (DerivedNullTarget));

      ClassContext inheritor = new ClassContext (typeof (double)).InheritFrom (baseContext);

      Assert.IsTrue (inheritor.Mixins.ContainsAssignableMixin (typeof (DerivedNullTarget)));
      Assert.IsTrue (inheritor.Mixins.ContainsAssignableMixin (typeof (NullTarget)));
    }

    [Test]
    public void MixinContext ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<DateTime>().WithDependency<int>().BuildClassContext();
      ClassContext inheritor = new ClassContext (typeof (double)).InheritFrom (baseContext);

      Assert.AreEqual (baseContext.Mixins[typeof (DateTime)], inheritor.Mixins[typeof (DateTime)]);
    }

    [Test]
    public void ExistingMixin_OverridesInherited ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<DateTime>().WithDependency<int>().BuildClassContext();
      ClassContext inheritor = new ClassContextBuilder (typeof (double)).AddMixin<DateTime>().WithDependency<decimal>().BuildClassContext()
          .InheritFrom (baseContext); // ignores inherited DateTime because DateTime already exists

      Assert.AreEqual (1, inheritor.Mixins.Count);
      Assert.IsTrue (inheritor.Mixins.ContainsKey (typeof (DateTime)));
      Assert.IsFalse (inheritor.Mixins[typeof (DateTime)].ExplicitDependencies.ContainsKey (typeof (int)));
      Assert.IsTrue (inheritor.Mixins[typeof (DateTime)].ExplicitDependencies.ContainsKey (typeof (decimal)));
    }

    [Test]
    public void DerivedMixin_OverridesInherited ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<NullTarget>().WithDependency<int>().BuildClassContext();

      ClassContext inheritor = new ClassContextBuilder (typeof (double)).AddMixin<DerivedNullTarget>().WithDependency<decimal>().BuildClassContext()
          .InheritFrom (baseContext); // ignores inherited NullTarget because DerivedNullTarget already exists

      Assert.AreEqual (1, inheritor.Mixins.Count);
      Assert.IsFalse (inheritor.Mixins.ContainsKey (typeof (NullTarget)));
      Assert.IsTrue (inheritor.Mixins.ContainsKey (typeof (DerivedNullTarget)));
      Assert.IsFalse (inheritor.Mixins[typeof (DerivedNullTarget)].ExplicitDependencies.ContainsKey (typeof (int)));
      Assert.IsTrue (inheritor.Mixins[typeof (DerivedNullTarget)].ExplicitDependencies.ContainsKey (typeof (decimal)));
    }

    [Test]
    public void BaseAndDerivedMixin_CanBeInherited ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<NullMixin> ().AddMixin<DerivedNullMixin>().BuildClassContext ();
      ClassContext inheritor = new ClassContext (typeof (double)).InheritFrom (baseContext);

      Assert.AreEqual (2, inheritor.Mixins.Count);
      Assert.IsTrue (inheritor.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (inheritor.Mixins.ContainsKey (typeof (DerivedNullMixin)));
    }

    [Test]
    public void BaseAndDerivedMixin_CanBeInherited_DifferentOrder ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<DerivedNullMixin> ().AddMixin<NullMixin> ().BuildClassContext ();
      ClassContext inheritor = new ClassContext (typeof (double)).InheritFrom (baseContext);

      Assert.AreEqual (2, inheritor.Mixins.Count);
      Assert.IsTrue (inheritor.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (inheritor.Mixins.ContainsKey (typeof (DerivedNullMixin)));
    }

    [Test]
    public void SpecializedGenericMixin_OverridesInherited ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin (typeof (GenericMixinWithVirtualMethod<>)).WithDependency<int>().BuildClassContext();

      ClassContext inheritor = new ClassContextBuilder (typeof (double)).AddMixin<GenericMixinWithVirtualMethod<object>>().WithDependency<decimal>().BuildClassContext()
          .InheritFrom (baseContext);

      Assert.AreEqual (1, inheritor.Mixins.Count);
      Assert.IsFalse (inheritor.Mixins.ContainsKey (typeof (GenericMixinWithVirtualMethod<>)));
      Assert.IsTrue (inheritor.Mixins.ContainsKey (typeof (GenericMixinWithVirtualMethod<object>)));
      Assert.IsFalse (inheritor.Mixins[typeof (GenericMixinWithVirtualMethod<object>)].ExplicitDependencies.ContainsKey (typeof (int)));
      Assert.IsTrue (inheritor.Mixins[typeof (GenericMixinWithVirtualMethod<object>)].ExplicitDependencies.ContainsKey (typeof (decimal)));
    }

    class DerivedGenericMixin<T> : GenericMixinWithVirtualMethod<T> where T : class { }

    [Test]
    public void SpecializedDerivedGenericMixin_OverridesInherited ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin (typeof (GenericMixinWithVirtualMethod<>)).WithDependency<int>().BuildClassContext();

      ClassContext inheritor = new ClassContextBuilder (typeof (double)).AddMixin<DerivedGenericMixin<object>>().WithDependency<decimal>().BuildClassContext()
          .InheritFrom (baseContext);

      Assert.AreEqual (1, inheritor.Mixins.Count);
      Assert.IsFalse (inheritor.Mixins.ContainsKey (typeof (GenericMixinWithVirtualMethod<>)));
      Assert.IsFalse (inheritor.Mixins.ContainsKey (typeof (GenericMixinWithVirtualMethod<object>)));
      Assert.IsFalse (inheritor.Mixins.ContainsKey (typeof (DerivedGenericMixin<>)));
      Assert.IsTrue (inheritor.Mixins.ContainsKey (typeof (DerivedGenericMixin<object>)));

      Assert.IsFalse (inheritor.Mixins[typeof (DerivedGenericMixin<object>)].ExplicitDependencies.ContainsKey (typeof (int)));
      Assert.IsTrue (inheritor.Mixins[typeof (DerivedGenericMixin<object>)].ExplicitDependencies.ContainsKey (typeof (decimal)));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException))]
    public void InheritedDerivedMixin_Throws ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<DerivedNullTarget>().WithDependency<int>().BuildClassContext();

      new ClassContextBuilder (typeof (double)).AddMixin<NullTarget>().WithDependency<decimal>().BuildClassContext()
          .InheritFrom (baseContext);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException))]
    public void InheritedSpecializedDerivedGenericMixin_Throws ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<DerivedGenericMixin<object>>().WithDependency<int>().BuildClassContext();

      new ClassContextBuilder (typeof (double)).AddMixin (typeof (GenericMixinWithVirtualMethod<>)).WithDependency<decimal>().BuildClassContext()
          .InheritFrom (baseContext);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The class System.Double inherits the mixin "
       + ".*DerivedGenericMixin\\`1 from class System.String, but it is explicitly configured for the less "
        + "specific mixin .*GenericMixinWithVirtualMethod\\`1\\[T\\].", MatchType = MessageMatch.Regex)]
    public void InheritedUnspecializedDerivedGenericMixin_Throws ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin (typeof (DerivedGenericMixin<>)).WithDependency<int>().BuildClassContext();

      new ClassContextBuilder (typeof (double)).AddMixin (typeof (GenericMixinWithVirtualMethod<>)).WithDependency<decimal>().BuildClassContext()
          .InheritFrom (baseContext);
    }

    [Test]
    public void CompleteInterfaces ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string))
          .AddCompleteInterface (typeof (object))
          .AddCompleteInterface (typeof (int))
          .BuildClassContext();

      ClassContext inheritor = new ClassContext (typeof (double)).InheritFrom (baseContext);

      Assert.AreEqual (2, inheritor.CompleteInterfaces.Count);
      Assert.That (inheritor.CompleteInterfaces, Is.EquivalentTo (inheritor.CompleteInterfaces));
    }

    [Test]
    public void ContainsCompleteInterface ()
    {
      ClassContext baseContext = new ClassContext (typeof (string), new MixinContext[0], new Type[] {typeof (object)});

      ClassContext inheritor = new ClassContext (typeof (double)).InheritFrom (baseContext);

      Assert.IsTrue (inheritor.CompleteInterfaces.ContainsKey (typeof (object)));
    }

    [Test]
    public void ExistingCompleteInterface_NotReplacedByInheritance ()
    {
      ClassContext baseContext = new ClassContext (typeof (string), new MixinContext[0], new Type[] { typeof (object) });

      ClassContext inheritor = new ClassContext (typeof (double), new MixinContext[0], new Type[] {typeof (object)})
          .InheritFrom (baseContext);

      Assert.AreEqual (1, inheritor.CompleteInterfaces.Count);
      Assert.IsTrue (inheritor.CompleteInterfaces.ContainsKey (typeof (object)));
    }

    [Test]
    public void InheritFrom_LeavesExistingData ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string))
          .AddMixin (typeof (DateTime))
          .AddCompleteInterface (typeof (object))
          .BuildClassContext();

      ClassContext inheritor = new ClassContextBuilder (typeof (double))
          .AddMixin (typeof (string))
          .AddCompleteInterface (typeof (int)).BuildClassContext()
          .InheritFrom (baseContext);

      Assert.AreEqual (2, inheritor.Mixins.Count);
      Assert.AreEqual (2, inheritor.CompleteInterfaces.Count);
    }
  }
}