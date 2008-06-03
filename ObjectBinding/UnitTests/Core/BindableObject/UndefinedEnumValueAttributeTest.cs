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
    
      Assert.AreEqual (TestEnum.Undefined, undefinedValueAttribute.Value);
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
