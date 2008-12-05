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
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationInheritanceTest
  {
    [Test]
    public void InheritingMixinConfigurationKnowsClassesFromBasePlusOwn ()
    {
      MixinConfiguration ac = new MixinConfiguration ();
      Assert.AreEqual (0, ac.ClassContexts.Count);
      ac.ClassContexts.Add (new ClassContext (typeof (BaseType1)));
      ac.ClassContexts.Add (new ClassContext (typeof (BaseType2)));
      Assert.AreEqual (2, ac.ClassContexts.Count);

      MixinConfiguration ac2 = new MixinConfiguration (ac);
      Assert.AreEqual (2, ac2.ClassContexts.Count);
      Assert.IsTrue (ac2.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsTrue (ac2.ClassContexts.ContainsWithInheritance (typeof (BaseType2)));
      Assert.IsFalse (ac2.ClassContexts.ContainsWithInheritance (typeof (BaseType3)));

      Assert.IsNotNull (ac2.ClassContexts.GetWithInheritance (typeof (BaseType1)));
      Assert.IsNotNull (ac2.ClassContexts.GetWithInheritance (typeof (BaseType2)));
      Assert.IsNull (ac2.ClassContexts.GetWithInheritance (typeof (BaseType3)));

      ac2.ClassContexts.Add (new ClassContext (typeof (BaseType3)));
      Assert.AreEqual (3, ac2.ClassContexts.Count);
      Assert.IsTrue (ac2.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsTrue (ac2.ClassContexts.ContainsWithInheritance (typeof (BaseType2)));
      Assert.IsTrue (ac2.ClassContexts.ContainsWithInheritance (typeof (BaseType3)));

      Assert.IsNotNull (ac2.ClassContexts.GetWithInheritance (typeof (BaseType1)));
      Assert.IsNotNull (ac2.ClassContexts.GetWithInheritance (typeof (BaseType2)));
      Assert.IsNotNull (ac2.ClassContexts.GetWithInheritance (typeof (BaseType3)));

      Assert.AreEqual (3, ac2.ClassContexts.Count);
      Assert.Contains (ac.ClassContexts.GetWithInheritance (typeof (BaseType1)), ac2.ClassContexts);
      Assert.Contains (ac.ClassContexts.GetWithInheritance (typeof (BaseType2)), ac2.ClassContexts);
      Assert.Contains (ac2.ClassContexts.GetWithInheritance (typeof (BaseType3)), ac2.ClassContexts);
    }

    [Test]
    public void OverridingClassContextsFromParent ()
    {
      MixinConfiguration ac = new MixinConfiguration ();
      Assert.AreEqual (0, ac.ClassContexts.Count);
      ac.ClassContexts.Add (new ClassContext (typeof (BaseType1)));
      ac.ClassContexts.Add (new ClassContext (typeof (BaseType2)));
      Assert.AreEqual (2, ac.ClassContexts.Count);

      MixinConfiguration ac2 = new MixinConfiguration (ac);
      Assert.AreEqual (2, ac2.ClassContexts.Count);
      Assert.IsTrue (ac2.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.AreEqual (ac.ClassContexts.GetWithInheritance (typeof (BaseType1)), ac2.ClassContexts.GetWithInheritance (typeof (BaseType1)));

      ClassContext newContext = new ClassContext (typeof (BaseType1));
      ac2.ClassContexts.AddOrReplace (newContext);
      Assert.AreEqual (2, ac2.ClassContexts.Count);

      Assert.IsTrue (ac2.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.AreNotSame (ac.ClassContexts.GetWithInheritance (typeof (BaseType1)), ac2.ClassContexts.GetWithInheritance (typeof (BaseType1)));
    }

    [Test]
    public void GetContextWorksRecursively ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<NullTarget> ().Clear ().AddMixins (typeof (NullMixin)).EnterScope ())
      {
        ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (DerivedNullTarget));
        Assert.IsNotNull (context);
        Assert.AreEqual (typeof (DerivedNullTarget), context.Type);
        Assert.IsTrue (context.Mixins.ContainsKey (typeof (NullMixin)));
      }
    }

    [Test]
    public void GetContextWorksRecursively_OverGenericDefinition ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (GenericTargetClass<>)).Clear ().AddMixins (typeof (NullMixin)).EnterScope ())
      {
        ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (GenericTargetClass<object>));
        Assert.IsNotNull (context);
        Assert.AreEqual (typeof (GenericTargetClass<object>), context.Type);
        Assert.IsTrue (context.Mixins.ContainsKey (typeof (NullMixin)));
      }
    }

    [Test]
    public void GetContextWorksRecursively_OverGenericDefinitionAndBase ()
    {
      using (MixinConfiguration.BuildFromActive ()
          .ForClass (typeof (GenericTargetClass<>)).Clear ().AddMixins (typeof (NullMixin))
          .ForClass (typeof (GenericTargetClass<object>)).Clear ().AddMixins (typeof (NullMixin2))
          .EnterScope ())
      {
        ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (DerivedGenericTargetClass<object>));
        Assert.IsNotNull (context);
        Assert.AreEqual (typeof (DerivedGenericTargetClass<object>), context.Type);
        Assert.IsTrue (context.Mixins.ContainsKey (typeof (NullMixin)));
        Assert.IsTrue (context.Mixins.ContainsKey (typeof (NullMixin2)));
      }
    }

    [Test]
    public void ContainsContextWorksRecursively ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<NullTarget> ().Clear ().AddMixins (typeof (NullMixin)).EnterScope ())
      {
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (DerivedNullTarget)));
      }
    }

    [Test]
    public void ContainsContextWorksRecursively_OverGenericDefinition ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (GenericTargetClass<>)).Clear ().AddMixins (typeof (NullMixin)).EnterScope ())
      {
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (GenericTargetClass<object>)));
      }
    }
  }
}
