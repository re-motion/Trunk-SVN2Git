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
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class MixedInterfaceTest
  {
    [Test]
    public void MixedInterface_GetsClassContext_ViaUses ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetExact (typeof (IMixedInterface));
      Assert.IsTrue (context.Mixins.ContainsKey (typeof (NullMixin)));
    }

    [Test]
    public void MixedInterface_GetsClassContext_ViaExtends ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetExact (typeof (IMixedInterface));
      Assert.IsTrue (context.Mixins.ContainsKey (typeof (MixinExtendingMixedInterface)));
    }

    [Test]
    public void ImplementingClass_InheritsMixins ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (ClassWithMixedInterface));
      Assert.IsTrue (context.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (context.Mixins.ContainsKey (typeof (MixinExtendingMixedInterface)));
    }

    [Test]
    public void Definition_ForImplementingClass ()
    {
      Assert.IsTrue (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassWithMixedInterface)).Mixins.ContainsKey (typeof (NullMixin)));
    }
  }
}
