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
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class TargetClassDefinitionFactoryTest
  {
    [Test]
    public void CreateTargetClassDefinition_ReturnsValidClassDefinition ()
    {
      var context = new ClassContext (typeof (BaseType1));
      var def = TargetClassDefinitionFactory.CreateTargetClassDefinition (context);
      Assert.That (def, Is.Not.Null);
      Assert.That (def.ConfigurationContext, Is.SameAs (context));
    }

    [Test]
    [ExpectedException (typeof (ValidationException))]
    public void CreateTargetClassDefinition_ValidatesWhenGeneratingDefinition ()
    {
      var cc = new ClassContext (typeof (DateTime));
      TargetClassDefinitionFactory.CreateTargetClassDefinition (cc);
    }
  }
}
