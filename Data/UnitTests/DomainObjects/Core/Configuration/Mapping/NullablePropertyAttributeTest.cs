// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class NullablePropertyAttributeTest
  {
    private class StubNullablePropertyAttribute: NullablePropertyAttribute
    {
      public StubNullablePropertyAttribute()
      {
      }
    }

    private StubNullablePropertyAttribute _attribute;
    private INullablePropertyAttribute _nullable;

    [SetUp]
    public void SetUp()
    {
      _attribute = new StubNullablePropertyAttribute();
      _nullable = _attribute;
    }

    [Test]
    public void GetNullable_FromDefault()
    {
      Assert.IsTrue (_attribute.IsNullable);
      Assert.IsTrue (_nullable.IsNullable);
    }

    [Test]
    public void GetNullable_FromExplicitValue()
    {
      _attribute.IsNullable = false;
      Assert.IsFalse (_attribute.IsNullable);
      Assert.IsFalse (_nullable.IsNullable);
    }
  }
}
