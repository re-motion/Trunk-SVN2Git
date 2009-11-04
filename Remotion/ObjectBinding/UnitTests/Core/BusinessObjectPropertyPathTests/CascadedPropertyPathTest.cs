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

namespace Remotion.ObjectBinding.UnitTests.Core.BusinessObjectPropertyPathTests
{
  [TestFixture]
  public class CascadedPropertyPathTest
  {
    private BusinessObjectPropertyPathTestHelper _testHelper;
    private BusinessObjectPropertyPath _path;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new BusinessObjectPropertyPathTestHelper ();
      _path = new TestBusinessObjectPropertyPath (_testHelper.ReferenceProperty, _testHelper.Property);
    }

    [Test]
    public void GetValue ()
    {
      using (_testHelper.Ordered ())
      {
        ExpectOnceOnReferencePropertyIsAccessible (true);
        ExpectOnceOnBusinessObjectGetProperty (_testHelper.BusinessObjectWithIdentity);
        ExpectOnceOnPropertyIsAccessible (true);
        ExpectOnceOnBusinessObjectGetProperty (_testHelper.BusinessObjectWithIdentity);
        ExpectOnceOnBusinessObjectWithIdentityGetProperty (100);
      }
      _testHelper.ReplayAll ();

      object actual = _path.GetValue (_testHelper.BusinessObject, true, true);

      _testHelper.VerifyAll ();
      Assert.AreEqual (100, actual);
    }

    [Test]
    public void GetValue_WithUnreachableObject ()
    {
      IBusinessObjectWithIdentity businessObject = null;
      using (_testHelper.Ordered ())
      {
        ExpectOnceOnReferencePropertyIsAccessible (true);
        ExpectOnceOnBusinessObjectGetProperty (businessObject);
        ExpectOnceOnBusinessObjectGetProperty (businessObject);
      }
      _testHelper.ReplayAll ();

      object actual = _path.GetValue (_testHelper.BusinessObject, false, true);

      _testHelper.VerifyAll ();
      Assert.IsNull (actual);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "A null value was detected in element 0 of property path ReferenceProperty.Property. Cannot evaluate rest of path.")]
    public void GetValue_ThrowsInvalidOperationExceptionBecauseOfUnreachableObject ()
    {
      using (_testHelper.Ordered ())
      {
        ExpectOnceOnReferencePropertyIsAccessible (true);
        ExpectOnceOnBusinessObjectGetProperty (null);
      }
      _testHelper.ReplayAll ();

      _path.GetValue (_testHelper.BusinessObject, true, true);
    }

    [Test]
    public void GetValue_WithAccessDenied ()
    {
      using (_testHelper.Ordered ())
      {
        ExpectOnceOnReferencePropertyIsAccessible (false);
      }
      _testHelper.ReplayAll ();

      object actualObject = _path.GetValue (_testHelper.BusinessObject, true, true);

      _testHelper.VerifyAll ();
      Assert.IsNull (actualObject);
    }

    [Test]
    public void GetPropertyString ()
    {
      using (_testHelper.Ordered ())
      {
        ExpectOnceOnReferencePropertyIsAccessible (true);
        ExpectOnceOnBusinessObjectGetProperty (_testHelper.BusinessObjectWithIdentity);
        ExpectOnceOnPropertyIsAccessible (true);
        ExpectOnceOnBusinessObjectGetProperty (_testHelper.BusinessObjectWithIdentity);
        ExpectOnceOnBusinessObjectWithIdentityGetPropertyString ("value");
      }
      _testHelper.ReplayAll ();

      string actual = _path.GetString (_testHelper.BusinessObject, string.Empty);

      _testHelper.VerifyAll ();
      Assert.AreEqual ("value", actual);
    }

    [Test]
    public void GetPropertyString_WithUnreachableObject ()
    {
      IBusinessObjectWithIdentity businessObject = null;
      using (_testHelper.Ordered ())
      {
        ExpectOnceOnReferencePropertyIsAccessible (true);
        ExpectOnceOnBusinessObjectGetProperty (businessObject);
        ExpectOnceOnBusinessObjectGetProperty (businessObject);
      }
      _testHelper.ReplayAll ();

      string actual = _path.GetString (_testHelper.BusinessObject, string.Empty);

      _testHelper.VerifyAll ();
      Assert.AreEqual (string.Empty, actual);
    }

    [Test]
    public void GetPropertyString_WithAccessDenied ()
    {
      using (_testHelper.Ordered ())
      {
        ExpectOnceOnReferencePropertyIsAccessible (false);
      }
      _testHelper.ReplayAll ();

      string actual = _path.GetString (_testHelper.BusinessObject, string.Empty);

      _testHelper.VerifyAll ();
      Assert.AreEqual (BusinessObjectPropertyPathTestHelper.NotAccessible, actual);
    }

    private void ExpectOnceOnReferencePropertyIsAccessible (bool returnValue)
    {
      _testHelper.ExpectOnceOnIsAccessible (_testHelper.ReferenceProperty, _testHelper.BusinessObjectClass, _testHelper.BusinessObject, returnValue);
    }

    private void ExpectOnceOnPropertyIsAccessible (bool returnValue)
    {
      _testHelper.ExpectOnceOnIsAccessible (
          _testHelper.Property,
          _testHelper.BusinessObjectClassWithIdentity,
          _testHelper.BusinessObjectWithIdentity,
          returnValue);
    }

    private void ExpectOnceOnBusinessObjectGetProperty (IBusinessObjectWithIdentity businessObejctWithIdentity)
    {
      _testHelper.ExpectOnceOnGetProperty (_testHelper.BusinessObject, _path.Properties[0], businessObejctWithIdentity);
    }

    private void ExpectOnceOnBusinessObjectWithIdentityGetProperty (int returnValue)
    {
      _testHelper.ExpectOnceOnGetProperty (_testHelper.BusinessObjectWithIdentity, _path.LastProperty, returnValue);
    }

    private void ExpectOnceOnBusinessObjectWithIdentityGetPropertyString (string returnValue)
    {
      _testHelper.ExpectOnceOnGetPropertyString (_testHelper.BusinessObjectWithIdentity, _path.LastProperty, string.Empty, returnValue);
    }
  }
}
