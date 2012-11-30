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
using Remotion.ObjectBinding.BusinessObjectPropertyPaths;

namespace Remotion.ObjectBinding.UnitTests.Core.BusinessObjectPropertyPaths
{
  [TestFixture]
  public class EvaluatedBusinessObjectPropertyPathResultTest
  {
    private BusinessObjectPropertyPathTestHelper _testHelper;
    private EvaluatedBusinessObjectPropertyPathResult _result;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new BusinessObjectPropertyPathTestHelper();
      _result = new EvaluatedBusinessObjectPropertyPathResult (_testHelper.BusinessObjectWithIdentity, _testHelper.Property);
    }

    [Test]
    public void GetValue ()
    {
      using (_testHelper.Ordered())
      {
        ExpectOnceOnPropertyIsAccessible (true);
        ExpectOnceOnBusinessObjectWithIdentityGetProperty (100);
      }
      _testHelper.ReplayAll();

      object actual = _result.GetValue();

      _testHelper.VerifyAll();
      Assert.That (actual, Is.EqualTo (100));
    }

    [Test]
    public void GetValue_WithAccessDenied ()
    {
      using (_testHelper.Ordered())
      {
        ExpectOnceOnPropertyIsAccessible (false);
      }
      _testHelper.ReplayAll();

      object actualObject = _result.GetValue();

      _testHelper.VerifyAll();
      Assert.That (actualObject, Is.Null);
    }

    [Test]
    public void GetPropertyString ()
    {
      using (_testHelper.Ordered())
      {
        ExpectOnceOnPropertyIsAccessible (true);
        ExpectOnceOnBusinessObjectWithIdentityGetPropertyString ("value", "format");
      }
      _testHelper.ReplayAll();

      string actual = _result.GetString ("format");

      _testHelper.VerifyAll();
      Assert.That (actual, Is.EqualTo ("value"));
    }

    [Test]
    public void GetString_WithAccessDenied ()
    {
      using (_testHelper.Ordered())
      {
        ExpectOnceOnPropertyIsAccessible (false);
      }
      _testHelper.ReplayAll();

      string actual = _result.GetString (string.Empty);

      _testHelper.VerifyAll();
      Assert.That (actual, Is.EqualTo (BusinessObjectPropertyPathTestHelper.NotAccessible));
    }

    [Test]
    public void GeResultProperty ()
    {
      Assert.That (_result.ResultProperty, Is.SameAs (_testHelper.Property));
    }

    [Test]
    public void GeResultObject ()
    {
      Assert.That (_result.ResultObject, Is.SameAs (_testHelper.BusinessObjectWithIdentity));
    }

    private void ExpectOnceOnPropertyIsAccessible (bool returnValue)
    {
      _testHelper.ExpectOnceOnIsAccessible (
          _testHelper.Property,
          _testHelper.BusinessObjectClassWithIdentity,
          _testHelper.BusinessObjectWithIdentity,
          returnValue);
    }

    private void ExpectOnceOnBusinessObjectWithIdentityGetProperty (int returnValue)
    {
      _testHelper.ExpectOnceOnGetProperty (_testHelper.BusinessObjectWithIdentity, _testHelper.Property, returnValue);
    }

    private void ExpectOnceOnBusinessObjectWithIdentityGetPropertyString (string returnValue, string format)
    {
      _testHelper.ExpectOnceOnGetPropertyString (_testHelper.BusinessObjectWithIdentity, _testHelper.Property, format, returnValue);
    }
  }
}