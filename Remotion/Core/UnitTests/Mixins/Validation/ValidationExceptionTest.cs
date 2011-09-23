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
using Remotion.Development.UnitTesting;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.UnitTests.Mixins.Validation
{
  [TestFixture]
  public class ValidationExceptionTest
  {
    [Test]
    [Ignore ("TODO 4010")]
    public void Serialization ()
    {
      var log = new DefaultValidationLog();
      var rule = new DelegateValidationRule<TargetClassDefinition> (DummyRule);

      var definition = TargetClassDefinitionFactory.CreateTargetClassDefinition (new ClassContext (typeof (object)));
      log.ValidationStartsFor (definition);
      log.Succeed (rule);
      log.ValidationEndsFor (definition);

      var exception = new ValidationException ("Message", log);

      var deserializedException = Serializer.SerializeAndDeserialize (exception);
      Assert.That (deserializedException.Message, Is.EqualTo (exception.Message));
      Assert.That (deserializedException.ValidationLog.GetNumberOfSuccesses(), Is.EqualTo (exception.ValidationLog.GetNumberOfSuccesses()));
    }

    private void DummyRule (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
      throw new NotImplementedException();
    }
  }
}