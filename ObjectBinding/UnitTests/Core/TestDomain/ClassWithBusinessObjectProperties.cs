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
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObject]
  public class ClassWithBusinessObjectProperties
  {
    private ClassWithSearchServiceTypeAttribute _searchServiceOnType;
    private ClassWithSearchServiceTypeAttribute _searchServiceOnProperty;
    private ClassWithIdentity _searchServiceFromPropertyWithIdentity;
    private ClassFromOtherBusinessObjectImplementation _noSearchService;
    private ClassWithIdentityFromOtherBusinessObjectImplementation _noSearchServiceWithIdentity;

    public ClassWithBusinessObjectProperties ()
    {
    }

    public ClassWithSearchServiceTypeAttribute SearchServiceFromType
    {
      get { return _searchServiceOnType; }
      set { _searchServiceOnType = value; }
    }

    [SearchAvailableObjectsServiceType (typeof (ISearchServiceOnProperty))]
    public ClassWithIdentity SearchServiceFromPropertyWithIdentity
    {
      get { return _searchServiceFromPropertyWithIdentity; }
      set { _searchServiceFromPropertyWithIdentity = value; }
    }

    [SearchAvailableObjectsServiceType (typeof (ISearchServiceOnProperty))]
    public ClassWithSearchServiceTypeAttribute SearchServiceFromProperty
    {
      get { return _searchServiceOnProperty; }
      set { _searchServiceOnProperty = value; }
    }

    public ClassFromOtherBusinessObjectImplementation NoSearchService
    {
      get { return _noSearchService; }
      set { _noSearchService = value; }
    }

    public ClassWithIdentityFromOtherBusinessObjectImplementation NoSearchServiceWithIdentity
    {
      get { return _noSearchServiceWithIdentity; }
      set { _noSearchServiceWithIdentity = value; }
    }
  }
}
