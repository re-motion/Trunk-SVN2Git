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
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.UnitTests.Mixins.Validation
{
  [TestFixture]
  public class ValidationResultTest
  {
    [Test]
    [Ignore ("TODO 4010")]
    public void Serialization ()
    {
      var parentDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (object));
      var nestedDefinition = DefinitionObjectMother.CreateMixinDefinition (parentDefinition, typeof (string));
      var validationResult = new ValidationResult (nestedDefinition);

      var rule = new DelegateValidationRule<MixinDefinition> (DummyRule);

      validationResult.Successes.Add (new ValidationResultItem (rule));
      validationResult.Exceptions.Add (new ValidationExceptionResultItem (rule, new Exception ("Test")));

      var deserializedResult = Serializer.SerializeAndDeserialize (validationResult);

      Assert.That (deserializedResult.Definition, Is.EqualTo (validationResult.Definition));
      Assert.That (deserializedResult.GetParentDefinitionString(), Is.EqualTo (validationResult.GetParentDefinitionString()));
      Assert.That (deserializedResult.Successes.Count, Is.EqualTo (validationResult.Successes.Count));
      Assert.That (deserializedResult.Exceptions.Count, Is.EqualTo (validationResult.Exceptions.Count));
    }

    private void DummyRule (DelegateValidationRule<MixinDefinition>.Args args)
    {
      throw new NotImplementedException();
    }
  }
}