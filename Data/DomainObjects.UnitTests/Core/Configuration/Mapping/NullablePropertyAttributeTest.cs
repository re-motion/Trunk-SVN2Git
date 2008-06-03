/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
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
