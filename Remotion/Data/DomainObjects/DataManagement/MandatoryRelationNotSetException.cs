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
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement
{
  [Serializable]
  public class MandatoryRelationNotSetException : DataManagementException
  {
    // types

    // static members and constants

    // member fields

    private DomainObject _domainObject;
    private string _propertyName;

    // construction and disposing

    public MandatoryRelationNotSetException (DomainObject domainObject, string propertyName, string message) 
        : this (domainObject, propertyName, message, null) 
    { 
    }

    public MandatoryRelationNotSetException (DomainObject domainObject, string propertyName, string message, Exception inner) 
        : base (message, inner) 
    {
      _domainObject = domainObject;
      _propertyName = propertyName;
    }
    
    protected MandatoryRelationNotSetException (SerializationInfo info, StreamingContext context) : base (info, context) 
    { 
    }

    // methods and properties

    public DomainObject DomainObject
    {
      get { return _domainObject; }
    }

    public string PropertyName
    {
      get { return _propertyName; }
    }
  }
}
