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
using Rhino.Mocks;
using Remotion.ObjectBinding;

namespace Remotion.ObjectBinding.UnitTests.Core.BusinessObjectPropertyPathTests
{
  public class BusinessObjectPropertyPathTestHelper
  {
    // types

    // static members

    public const string NotAccessible = "Not Accessible";

    // member fields

    private MockRepository _mocks;

    private IBusinessObjectProperty _mockProperty;
    private IBusinessObjectReferenceProperty _mockReferenceProperty;
    private IBusinessObjectReferenceProperty _mockReferenceListProperty;

    private IBusinessObjectClass _mockBusinessObjectClass;
    private IBusinessObjectClassWithIdentity _mockBusinessObjectClassWithIdentity;

    private IBusinessObject _mockBusinessObject;
    private IBusinessObjectWithIdentity _mockBusinessObjectWithIdentity;
    private IBusinessObjectWithIdentity[] _businessObjectWithIdentityList;

    private IBusinessObjectProvider _mockBusinessObjectProvider;

    // construction and disposing

    public BusinessObjectPropertyPathTestHelper ()
    {
      _mocks = new MockRepository ();

      _mockProperty = _mocks.StrictMock<IBusinessObjectProperty> ();
      _mockReferenceProperty = _mocks.StrictMock<IBusinessObjectReferenceProperty> ();
      _mockReferenceListProperty = _mocks.StrictMock<IBusinessObjectReferenceProperty> ();

      _mockBusinessObjectClass = _mocks.StrictMock<IBusinessObjectClass> ();
      _mockBusinessObjectClassWithIdentity = _mocks.StrictMock<IBusinessObjectClassWithIdentity> ();

      _mockBusinessObject = _mocks.StrictMock<IBusinessObject> ();
      _mockBusinessObjectWithIdentity = _mocks.StrictMock<IBusinessObjectWithIdentity> ();
      _businessObjectWithIdentityList = new IBusinessObjectWithIdentity[] { _mockBusinessObjectWithIdentity };

      _mockBusinessObjectProvider = _mocks.StrictMock<IBusinessObjectProvider> ();

      SetupResult.For (_mockBusinessObject.BusinessObjectClass).Return (_mockBusinessObjectClass);
      SetupResult.For (_mockBusinessObjectWithIdentity.BusinessObjectClass).Return (_mockBusinessObjectClassWithIdentity);
      SetupResult.For (_mockReferenceProperty.IsList).Return (false);
      SetupResult.For (_mockReferenceListProperty.IsList).Return (true);

      SetupResult.For (_mockProperty.Identifier).Return ("Property");
      SetupResult.For (_mockReferenceProperty.Identifier).Return ("ReferenceProperty");
      SetupResult.For (_mockReferenceListProperty.Identifier).Return ("ReferenceListProperty");

      SetupResult.For (_mockProperty.BusinessObjectProvider).Return (_mockBusinessObjectProvider);
      SetupResult.For (_mockReferenceProperty.BusinessObjectProvider).Return (_mockBusinessObjectProvider);
      SetupResult.For (_mockReferenceListProperty.BusinessObjectProvider).Return (_mockBusinessObjectProvider);

      SetupResult.For (_mockBusinessObjectClass.BusinessObjectProvider).Return (_mockBusinessObjectProvider);
      SetupResult.For (_mockBusinessObjectClassWithIdentity.BusinessObjectProvider).Return (_mockBusinessObjectProvider);

      SetupResult.For (_mockBusinessObjectProvider.GetPropertyPathSeparator ()).Return ('.');
      SetupResult.For (_mockBusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder ()).Return (BusinessObjectPropertyPathTestHelper.NotAccessible);
    }

    // methods and properties

    public IDisposable Ordered ()
    {
      return _mocks.Ordered ();
    }

    public void ReplayAll ()
    {
      _mocks.ReplayAll ();
    }

    public void VerifyAll ()
    {
      _mocks.VerifyAll ();
    }

    public IBusinessObjectProperty Property
    {
      get { return _mockProperty; }
    }

    public IBusinessObjectReferenceProperty ReferenceProperty
    {
      get { return _mockReferenceProperty; }
    }

    public IBusinessObjectReferenceProperty ReferenceListProperty
    {
      get { return _mockReferenceListProperty; }
    }

    public IBusinessObjectClass BusinessObjectClass
    {
      get { return _mockBusinessObjectClass; }
    }

    public IBusinessObjectClassWithIdentity BusinessObjectClassWithIdentity
    {
      get { return _mockBusinessObjectClassWithIdentity; }
    }

    public IBusinessObject BusinessObject
    {
      get { return _mockBusinessObject; }
    }

    public IBusinessObjectWithIdentity BusinessObjectWithIdentity
    {
      get { return _mockBusinessObjectWithIdentity; }
    }

    public IBusinessObjectWithIdentity[] BusinessObjectWithIdentityList
    {
      get { return _businessObjectWithIdentityList; }
    }

    public void ExpectOnceOnGetProperty (IBusinessObject businessObject, IBusinessObjectProperty property, object returnValue)
    {
      Expect.Call (businessObject.GetProperty (property)).Return (returnValue);
    }

    public void ExpectOnceOnGetPropertyString (IBusinessObject businessObject, IBusinessObjectProperty property, string format, string returnValue)
    {
      Expect.Call (businessObject.GetPropertyString (property, format)).Return (returnValue);
    }

    public void ExpectOnceOnIsAccessible (
        IBusinessObjectProperty property,
        IBusinessObjectClass businessObjectClass,
        IBusinessObject businessObject,
        bool returnValue)
    {
      Expect.Call (property.IsAccessible (businessObjectClass, businessObject)).Return (returnValue);
    }
  }
}
