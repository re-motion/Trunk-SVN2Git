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
  public class ValidationExceptionResultItemTest
  {
    // [Ignore ("TODO 4010")]
    public void Serialization ()
    {
      var rule = new DelegateValidationRule<TargetClassDefinition> (DummyRule);
      var item = new ValidationExceptionResultItem (rule, new Exception ("Test"));

      var deserializedItem = Serializer.SerializeAndDeserialize (item);

      Assert.That (deserializedItem.Exception.Message, Is.EqualTo ("Test"));
      Assert.That (deserializedItem.Message, Is.EqualTo (item.Message));
      Assert.That (deserializedItem.Rule, Is.EqualTo (item.Rule));
    }

    private void DummyRule (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
      throw new NotImplementedException();
    }
  }
}