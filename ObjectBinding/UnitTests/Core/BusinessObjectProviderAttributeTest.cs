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
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core
{
  [TestFixture]
  public class BusinessObjectProviderAttributeTest
  {
    private class StubBusinessObjectProviderAttribute : BusinessObjectProviderAttribute
    {
      public StubBusinessObjectProviderAttribute (Type businessObjectProviderType)
          : base (businessObjectProviderType)
      {
      }
    }

    [Test]
    public void Initialize_WithValidType ()
    {
      BusinessObjectProviderAttribute attribute = new StubBusinessObjectProviderAttribute (typeof (BindableObjectProvider));

      Assert.That (attribute.BusinessObjectProviderType, Is.EqualTo (typeof (BindableObjectProvider)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Initialize_WithInvalidType ()
    {
      new StubBusinessObjectProviderAttribute (typeof (object));
    }
  }
}
