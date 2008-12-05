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
using System.Threading;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationCurrentConfigurationTest
  {
    [SetUp]
    public void SetUp()
    {
      MixinConfiguration.SetActiveConfiguration (null);
    }

    [TearDown]
    public void TearDown ()
    {
      MixinConfiguration.SetActiveConfiguration (null);
      MixinConfiguration.ResetMasterConfiguration ();
    }

    [Test]
    public void InitialConfiguration()
    {
      Assert.IsFalse (MixinConfiguration.HasActiveConfiguration);
      MixinConfiguration context = MixinConfiguration.ActiveConfiguration;
      Assert.IsNotNull (context);
      Assert.IsTrue (MixinConfiguration.HasActiveConfiguration);
    }

    [Test (Description = "Checks whether this test fixture correctly resets the mixin configuration before running each tests.")]
    public void InitialConfiguration2 ()
    {
      Assert.IsFalse (MixinConfiguration.HasActiveConfiguration);
      MixinConfiguration context = MixinConfiguration.ActiveConfiguration;
      Assert.IsNotNull (context);
      Assert.IsTrue (MixinConfiguration.HasActiveConfiguration);
    }

    [Test (Description = "Ensures that the current assembly is scanned for the initial configuration.")]
    public void DefaultContext()
    {
      MixinConfiguration context = MixinConfiguration.ActiveConfiguration;
      Assert.AreNotEqual (0, context.ClassContexts.Count);
      Assert.IsTrue (context.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsTrue (context.ClassContexts.ContainsWithInheritance (typeof (BaseType3)));
      Assert.IsFalse (context.ClassContexts.ContainsWithInheritance (typeof (BaseType4)));
    }

    [Test]
    public void SetContext()
    {
      MixinConfiguration context = MixinConfiguration.ActiveConfiguration;
      Assert.AreSame (context, MixinConfiguration.ActiveConfiguration);
      MixinConfiguration newConfiguration = new MixinConfiguration();
      MixinConfiguration.SetActiveConfiguration (newConfiguration);
      Assert.AreNotSame (context, MixinConfiguration.ActiveConfiguration);
      Assert.AreSame (newConfiguration, MixinConfiguration.ActiveConfiguration);
    }

    [Test]
    public void ActiveConfigurationIsThreadSpecific()
    {
      MixinConfiguration context = MixinConfiguration.ActiveConfiguration;
      MixinConfiguration newConfiguration = new MixinConfiguration ();
      MixinConfiguration.SetActiveConfiguration (newConfiguration);

      MixinConfiguration context2 = null;
      MixinConfiguration newContext2 = new MixinConfiguration();

      Thread.MemoryBarrier();

      Thread secondThread = new Thread ((ThreadStart) delegate
      {
        context2 = MixinConfiguration.ActiveConfiguration;
        MixinConfiguration.SetActiveConfiguration (newContext2);
        Assert.AreSame (newContext2, MixinConfiguration.ActiveConfiguration);
      });
      secondThread.Start();
      secondThread.Join();

      Thread.MemoryBarrier();

      Assert.IsNotNull (context2);
      Assert.AreNotSame (context, context2);
      Assert.AreNotSame (newConfiguration, context2);
      Assert.AreNotSame (newContext2, MixinConfiguration.ActiveConfiguration);
      Assert.AreSame (newConfiguration, MixinConfiguration.ActiveConfiguration);
    }

    [Test]
    public void EnterScope()
    {
      MixinConfiguration context = new MixinConfiguration ();
      MixinConfiguration context2 = new MixinConfiguration ();
      Assert.IsFalse (MixinConfiguration.HasActiveConfiguration);
      using (context.EnterScope())
      {
        Assert.IsTrue (MixinConfiguration.HasActiveConfiguration);
        Assert.AreSame (context, MixinConfiguration.ActiveConfiguration);
        using (context2.EnterScope())
        {
          Assert.AreNotSame (context, MixinConfiguration.ActiveConfiguration);
          Assert.AreSame (context2, MixinConfiguration.ActiveConfiguration);
        }
        Assert.IsTrue (MixinConfiguration.HasActiveConfiguration);
        Assert.AreNotSame (context2, MixinConfiguration.ActiveConfiguration);
        Assert.AreSame (context, MixinConfiguration.ActiveConfiguration);
      }
      Assert.IsFalse (MixinConfiguration.HasActiveConfiguration);
    }

    [Test]
    public void CreateEmptyConfiguration()
    {
      Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      using (MixinConfiguration.BuildNew().EnterScope ())
      {
        Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      }
    }

    [Test]
    public void MasterConfigurationIsCopiedByNewThreads ()
    {
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)));
      MixinConfiguration.EditMasterConfiguration (delegate (MixinConfiguration masterConfiguration)
      {
        masterConfiguration.ClassContexts.Add (new ClassContext (typeof (object)));
      });
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)));
      ThreadRunner.Run (delegate
      {
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)));
      });
    }
  }
}
