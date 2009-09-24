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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationTest
  {
    [Test]
    public void Initialization ()
    {
      
    }

    [Test]
    public void Initialization_Empty ()
    {
      var configuration = new MixinConfiguration();
      Assert.IsEmpty (configuration.ClassContexts);
    }

    [Test]
    public void GetContext_Configured ()
    {
      var context = MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1));
      Assert.That (context, Is.Not.Null);
    }

    [Test]
    public void GetContext_Configured_ButEmpty ()
    {
      var configuration = MixinConfiguration.BuildNew ().ForClass<NullTarget> ().BuildConfiguration ();
      Assert.That (configuration.ClassContexts.ContainsExact (typeof (NullTarget)), Is.True);

      var context = configuration.GetContext (typeof (NullTarget));
      Assert.That (context, Is.Null);
    }

    [Test]
    public void GetContext_ReturnsNull_IfNotConfigured ()
    {
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)), Is.False);

      var context = MixinConfiguration.ActiveConfiguration.GetContext (typeof (object));
      Assert.That (context, Is.Null);
    }

    [Test]
    public void GetContext_NoNewContext_GeneratedForGeneratedType ()
    {
      var expectedContext = MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1));

      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
      var actualContext = MixinConfiguration.ActiveConfiguration.GetContext (generatedType);
      Assert.That (actualContext, Is.EqualTo (expectedContext));
    }

    [Test]
    public void GetContextForce_Configured ()
    {
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)), Is.True);

      var context = MixinConfiguration.ActiveConfiguration.GetContextForce (typeof (BaseType1));
      Assert.That (context, Is.SameAs (MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1))));
    }

    [Test]
    public void GetContextForce_ReturnsNew_IfNotConfigured ()
    {
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)), Is.False);

      var context = MixinConfiguration.ActiveConfiguration.GetContextForce (typeof (object));
      Assert.That (context, Is.Not.Null);
      Assert.That (context.Type, Is.SameAs (typeof (object)));
    }

    [Test]
    public void GetContextForce_NewContext_GeneratedForGeneratedType ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
      var newContext = MixinConfiguration.ActiveConfiguration.GetContextForce (generatedType);
      var baseContext = MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1));

      Assert.That (newContext, Is.Not.EqualTo (baseContext));
      Assert.That (newContext.Type, Is.SameAs (generatedType));
    }

    [Test]
    public void GetContextForce_ResultNotCached ()
    {
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)), Is.False);
      var context1 = MixinConfiguration.ActiveConfiguration.GetContextForce (typeof (object));
      var context2 = MixinConfiguration.ActiveConfiguration.GetContext (typeof (object));

      Assert.That (context1, Is.Not.Null);
      Assert.That (context2, Is.Null);
    }

    [Test]
    public void SetMasterConfiguration ()
    {
      var mixinConfiguration = new MixinConfiguration ();
      MixinConfiguration.SetMasterConfiguration (mixinConfiguration);
      Assert.That (MixinConfiguration.GetMasterConfiguration (), Is.SameAs (mixinConfiguration));
    }

    [Test]
    public void GetMasterConfiguration_Default ()
    {
      var oldMasterConfiguration = MixinConfiguration.GetMasterConfiguration ();

      try
      {
        MixinConfiguration.SetMasterConfiguration (null);
        var newMasterConfiguration = MixinConfiguration.GetMasterConfiguration ();

        Assert.That (newMasterConfiguration, Is.Not.Null);
        Assert.That (newMasterConfiguration, Is.Not.SameAs (oldMasterConfiguration));
        Assert.That (newMasterConfiguration.GetContext (typeof (BaseType1)), Is.Not.Null);
      }
      finally
      {
        MixinConfiguration.SetMasterConfiguration (oldMasterConfiguration);
      }
    }
  }
}
