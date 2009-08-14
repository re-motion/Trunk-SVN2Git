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
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins
{
  [TestFixture]
  public class TargetClassDefinitionUtilityTest
  {
    [Test]
    public void GetContext_ReturnsNull_IfNotConfigured ()
    {
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)), Is.False);

      var context = MixinConfiguration.ActiveConfiguration.GetContext (typeof (object));
      Assert.That (context, Is.Null);
    }

    [Test]
    public void GetContext_ReturnsNew_IfNotConfigured_Force ()
    {
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)), Is.False);

      var context = MixinConfiguration.ActiveConfiguration.GetContextForce (typeof (object));
      Assert.That (context, Is.Not.Null);
      Assert.That (context.Type, Is.SameAs (typeof (object)));
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
    public void GetContext_NewContext_GeneratedForGeneratedType_Force ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
      var newContext = MixinConfiguration.ActiveConfiguration.GetContextForce (generatedType);
      var baseContext = MixinConfiguration.ActiveConfiguration.ClassContexts.GetExact (typeof (BaseType1));

      Assert.That (newContext, Is.Not.EqualTo (baseContext));
      Assert.That (newContext.Type, Is.SameAs (generatedType));
    }

    [Test]
    public void GetContext_ForcedGeneration_IsNotPersistent ()
    {
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)), Is.False);
      var context1 = MixinConfiguration.ActiveConfiguration.GetContextForce (typeof (object));
      var context2 = MixinConfiguration.ActiveConfiguration.GetContext (typeof (object));

      Assert.That (context1, Is.Not.Null);
      Assert.That (context2, Is.Null);
    }
  }
}