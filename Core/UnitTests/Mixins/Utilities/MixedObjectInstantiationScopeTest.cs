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
using Remotion.Mixins.Utilities;

namespace Remotion.UnitTests.Mixins.Utilities
{
  [TestFixture]
  public class MixedObjectInstantiationScopeTest
  {
    [SetUp]
    public void SetUp ()
    {
      MixedObjectInstantiationScope.SetCurrent (null);
    }

    [TearDown]
    public void TearDown ()
    {
      MixedObjectInstantiationScope.SetCurrent (null);
    }
    
    [Test]
    public void ScopeInitializedOnDemand ()
    {
      Assert.IsFalse (MixedObjectInstantiationScope.HasCurrent);
      Assert.IsNotNull (MixedObjectInstantiationScope.Current);
      Assert.IsTrue (MixedObjectInstantiationScope.HasCurrent);
    }

    [Test (Description = "Checks (in conjunction with ScopeInitializedOnDemand) whether this test fixture correctly resets the scope.")]
    public void CurrentIsReset ()
    {
      Assert.IsFalse (MixedObjectInstantiationScope.HasCurrent);
      Assert.IsNotNull (MixedObjectInstantiationScope.Current);
      Assert.IsTrue (MixedObjectInstantiationScope.HasCurrent);
    }

    [Test]
    public void DefaultMixinInstancesEmpty ()
    {
      Assert.IsNotNull (MixedObjectInstantiationScope.Current.SuppliedMixinInstances);
      Assert.AreEqual (0, MixedObjectInstantiationScope.Current.SuppliedMixinInstances.Length);
    }

    [Test]
    public void InstancesCanBeSuppliedInScopes ()
    {
      Assert.AreEqual (0, MixedObjectInstantiationScope.Current.SuppliedMixinInstances.Length);
      using (new MixedObjectInstantiationScope ("1", "2"))
      {
        Assert.AreEqual (2, MixedObjectInstantiationScope.Current.SuppliedMixinInstances.Length);
        Assert.AreEqual ("1", MixedObjectInstantiationScope.Current.SuppliedMixinInstances[0]);
        Assert.AreEqual ("2", MixedObjectInstantiationScope.Current.SuppliedMixinInstances[1]);
        
        using (new MixedObjectInstantiationScope ("a"))
        {
          Assert.AreEqual (1, MixedObjectInstantiationScope.Current.SuppliedMixinInstances.Length);
          Assert.AreEqual ("a", MixedObjectInstantiationScope.Current.SuppliedMixinInstances[0]);

          using (new MixedObjectInstantiationScope ())
          {
            Assert.AreEqual (0, MixedObjectInstantiationScope.Current.SuppliedMixinInstances.Length);
          }

          Assert.AreEqual (1, MixedObjectInstantiationScope.Current.SuppliedMixinInstances.Length);
          Assert.AreEqual ("a", MixedObjectInstantiationScope.Current.SuppliedMixinInstances[0]);
        }

        Assert.AreEqual (2, MixedObjectInstantiationScope.Current.SuppliedMixinInstances.Length);
        Assert.AreEqual ("1", MixedObjectInstantiationScope.Current.SuppliedMixinInstances[0]);
        Assert.AreEqual ("2", MixedObjectInstantiationScope.Current.SuppliedMixinInstances[1]);
      }
      Assert.AreEqual (0, MixedObjectInstantiationScope.Current.SuppliedMixinInstances.Length);
    }
  }
}
