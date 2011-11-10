// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
