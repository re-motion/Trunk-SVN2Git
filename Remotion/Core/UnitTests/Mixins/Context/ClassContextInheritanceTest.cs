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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Context
{
  [TestFixture]
  public class ClassContextInheritanceTest
  {
    [Test]
    public void InheritFrom_Mixins ()
    {
      var baseContext = new ClassContext (typeof (string), typeof (DateTime), typeof (int), typeof (DerivedNullTarget));
      var inheritor = new ClassContext (typeof (double)).InheritFrom (baseContext);

      Assert.AreEqual (3, inheritor.Mixins.Count);
      Assert.That (inheritor.Mixins, Is.EquivalentTo (baseContext.Mixins));
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
      var baseContext = new ClassContext (typeof (string), new MixinContext[0], new[] {typeof (object)});

      ClassContext inheritor = new ClassContext (typeof (double)).InheritFrom (baseContext);

      Assert.IsTrue (inheritor.CompleteInterfaces.ContainsKey (typeof (object)));
    }

    [Test]
    public void ExistingCompleteInterface_NotReplacedByInheritance ()
    {
      var baseContext = new ClassContext (typeof (string), new MixinContext[0], new[] { typeof (object) });

      ClassContext inheritor = new ClassContext (typeof (double), new MixinContext[0], new[] {typeof (object)})
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

    [Test]
    public void MixinsOnInterface ()
    {
      MixinConfiguration context = MixinConfiguration.BuildFromActive ()
          .ForClass<IBaseType2> ().AddMixin<BT2Mixin1> ().BuildConfiguration ();

      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (IBaseType2));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (BT2Mixin1)));
    }
  }
}
