// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class NullableTypeUtilityTest
  {
    [Test]
    public void IsNullableType_ValueType ()
    {
      Assert.That (NullableTypeUtility.IsNullableType (typeof (int)), Is.False);
      Assert.That (NullableTypeUtility.IsNullableType (typeof (DateTime)), Is.False);
    }

    [Test]
    public void IsNullableType_NullableValueType ()
    {
      Assert.That (NullableTypeUtility.IsNullableType (typeof (int?)), Is.True);
      Assert.That (NullableTypeUtility.IsNullableType (typeof (DateTime?)), Is.True);
    }

    [Test]
    public void IsNullableType_ReferenceType ()
    {
      Assert.That (NullableTypeUtility.IsNullableType (typeof (object)), Is.True);
      Assert.That (NullableTypeUtility.IsNullableType (typeof (string)), Is.True);
    }

    [Test]
    public void GetNullableType_ValueType ()
    {
      Assert.That (NullableTypeUtility.GetNullableType (typeof (int)), Is.EqualTo (typeof (int?)));
    }

    [Test]
    public void GetNullableType_NullableValueType ()
    {
      Assert.That (NullableTypeUtility.GetNullableType (typeof (int?)), Is.EqualTo (typeof (int?)));
    }

    [Test]
    public void GetNullableType_ReferenceType ()
    {
      Assert.That (NullableTypeUtility.GetNullableType (typeof (string)), Is.EqualTo (typeof (string)));
    }

    [Test]
    public void GetBasicType_ValueType ()
    {
      Assert.That (NullableTypeUtility.GetBasicType (typeof (int)), Is.EqualTo (typeof (int)));
    }

    [Test]
    public void GetBasicType_NullableValueType ()
    {
      Assert.That (NullableTypeUtility.GetBasicType (typeof (int?)), Is.EqualTo (typeof (int)));
    }


    [Test]
    public void GetBasicType_ReferenceType ()
    {
      Assert.That (NullableTypeUtility.GetBasicType (typeof (string)), Is.EqualTo (typeof (string)));
    }
  }
}
