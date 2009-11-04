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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.Context.Suppression.TestDomain;
using System.Linq;

namespace Remotion.UnitTests.Mixins.Context.Suppression
{
  [TestFixture]
  public class IntegrationTest
  {
    [Test]
    public void MixinSuppressingOther ()
    {
      var mixins = MixinConfiguration.ActiveConfiguration.GetContext (typeof (ClassWithSuppressingMixins)).Mixins.Select (mc => mc.MixinType).ToArray();
      Assert.That (mixins, Is.EquivalentTo (new[] { typeof (MixinSuppressingOther) }));
    }

    [Test]
    public void MixinSuppressingItsBase ()
    {
      var mixins = MixinConfiguration.ActiveConfiguration.GetContext (typeof (ClassWithMixinSuppressingBase)).Mixins.Select (mc => mc.MixinType).ToArray ();
      Assert.That (mixins, Is.EquivalentTo (new[] { typeof (MixinSuppressingBase) }));
    }

    [Test]
    public void CircularSuppressingMixins ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (MixinWithCircularSuppress1))
          .AddType (typeof (MixinWithCircularSuppress2))
          .BuildConfiguration ();
      ClassContext classContext = configuration.ClassContexts.GetExact (typeof (ClassWithMixins));
      Assert.That (classContext.IsEmpty (), Is.True);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Mixin type "
        + "'Remotion.UnitTests.Mixins.Context.Suppression.TestDomain.MixinSuppressingItself' applied to target class "
        + "'Remotion.UnitTests.Mixins.Context.Suppression.TestDomain.ClassWithMixins' suppresses itself.")]
    public void SelfSuppressingMixin ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (MixinSuppressingItself)).BuildConfiguration ();
    }
    
    [Test]
    public void GenericSuppressingMixinWithSpecialization ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (GenericMixinWithSpecialization<,>))
          .AddType (typeof (MixinSuppressingOpenGenericMixin))
          .BuildConfiguration ();
      ClassContext classContext = configuration.GetContext (typeof (ClassWithMixins));
      Assert.IsFalse (classContext.Mixins.ContainsKey (typeof (GenericMixinWithSpecialization<List<int>, IList<int>>)));
    }

    [Test]
    public void GenericSuppressingMixinWithoutSpecialization ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (GenericMixinWithoutSpecialization<,>))
          .AddType (typeof (MixinSuppressingOpenGenericMixin))
          .BuildConfiguration ();
      ClassContext classContext = configuration.GetContext (typeof (ClassWithMixins));
      Assert.IsFalse (classContext.Mixins.ContainsKey (typeof (GenericMixinWithoutSpecialization<,>)));
    }

    [Test]
    public void ClosedGenericSuppressingMixin ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (GenericMixinWithSpecialization<,>))
          .AddType (typeof (MixinSuppressingClosedGenericMixin)).BuildConfiguration ();
      ClassContext classContext = configuration.GetContext (typeof (ClassWithMixins));
      Assert.IsFalse (classContext.Mixins.ContainsKey (typeof (GenericMixinWithSpecialization<List<int>, IList<int>>)));
    }

    [Test]
    public void ClosedGenericSuppressingMixin_WithWrongParameterTypes ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (GenericMixinWithSpecialization<,>))
          .AddType (typeof (MixinSuppressingGenericMixinWithWrongTypeParameters)).BuildConfiguration ();
      ClassContext classContext = configuration.GetContext (typeof (ClassWithMixins));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (GenericMixinWithSpecialization<List<int>, IList<int>>)));
    }
  }
}
