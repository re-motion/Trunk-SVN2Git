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
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class UndefinedEnumValueAttributeTest : TestBase
  {
    private enum TestEnum
    {
      Undefined = 0,
      Value1 = 1,
      Value2 = 2
    }

    [Test]
    public void Initialize ()
    {
      UndefinedEnumValueAttribute undefinedValueAttribute = new UndefinedEnumValueAttribute (TestEnum.Undefined);
    
      Assert.AreEqual (TestEnum.Undefined, undefinedValueAttribute.GetValue());
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void InitializeWithInvalidValue ()
    {
      TestEnum invalidValue = (TestEnum) (-1);
      UndefinedEnumValueAttribute undefinedValueAttribute = new UndefinedEnumValueAttribute (invalidValue);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void InitializeWithObjectOfInvalidType ()
    {
      UndefinedEnumValueAttribute undefinedValueAttribute = new UndefinedEnumValueAttribute (this);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void InitializeWithNull ()
    {
      UndefinedEnumValueAttribute undefinedValueAttribute = new UndefinedEnumValueAttribute (null);
    }
  }
}
