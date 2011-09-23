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
using Remotion.Mixins.Validation;

namespace Remotion.UnitTests.Mixins.Validation
{
  [TestFixture]
  public class ValidationResultTest
  {
    [Test]
    public void GetDefinitionContextPath_NonNested ()
    {
      var validationResult = new ValidationResult (new ValidatedDefinitionID ("Definition", "1", null));
      Assert.That (validationResult.GetDefinitionContextPath(), Is.EqualTo (""));
    }

    [Test]
    public void GetDefinitionContextPath_NestedOnce ()
    {
      var validationResult = new ValidationResult (new ValidatedDefinitionID ("Definition", "1", new ValidatedDefinitionID ("Parent Definition", "2", null)));

      Assert.That (validationResult.GetDefinitionContextPath(), Is.EqualTo ("2"));
    }

    [Test]
    public void GetDefinitionContextPath_NestedTwice ()
    {
      var validationResult = new ValidationResult (
          new ValidatedDefinitionID (
              "Definition",
              "1",
              new ValidatedDefinitionID (
                  "Parent Definition",
                  "2",
                  new ValidatedDefinitionID ("ParentParent Definition", "3", null))));

      Assert.That (validationResult.GetDefinitionContextPath(), Is.EqualTo ("2 -> 3"));
    }

    [Test]
    public void Serialization ()
    {
      var validationResult = new ValidationResult (
          new ValidatedDefinitionID ("Definition", "1", new ValidatedDefinitionID ("Parent Definition", "2", null)));

      validationResult.Successes.Add (new ValidationResultItem ("rule name 1", "message"));
      validationResult.Exceptions.Add (new ValidationExceptionResultItem ("rule name 2", new Exception ("Test")));

      var deserializedResult = Serializer.SerializeAndDeserialize (validationResult);

      Assert.That (deserializedResult.ValidatedDefinitionID, Is.EqualTo (validationResult.ValidatedDefinitionID));
      Assert.That (deserializedResult.GetDefinitionContextPath(), Is.EqualTo (validationResult.GetDefinitionContextPath()));
      Assert.That (deserializedResult.Successes.Count, Is.EqualTo (validationResult.Successes.Count));
      Assert.That (deserializedResult.Exceptions.Count, Is.EqualTo (validationResult.Exceptions.Count));
    }
  }
}