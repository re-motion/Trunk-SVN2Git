// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
[Serializable]
public class IdentityTypeNotSupportedException : StorageProviderConfigurationException
{
  // types

  // static members and constants

  // member fields

  private Type _storageProviderType;
  private Type _invalidIdentityType;

  // construction and disposing

  public IdentityTypeNotSupportedException () : this ("Storage provider does not support provided identity type.") 
  {
  }

  public IdentityTypeNotSupportedException (string message) : base (message) 
  {
  }
  
  public IdentityTypeNotSupportedException (string message, Exception inner) : base (message, inner) 
  {
  }

  public IdentityTypeNotSupportedException (Type storageProviderType, Type invalidIdentityType) 
      : this (string.Format ("The StorageProvider '{0}' does not support identity values of type '{1}'.", storageProviderType, invalidIdentityType),
            storageProviderType, invalidIdentityType)
  {
  }

  public IdentityTypeNotSupportedException (string message, Type storageProviderType, Type invalidIdentityType) : base (message)
  {
    ArgumentUtility.CheckNotNull ("storageProviderType", storageProviderType);
    ArgumentUtility.CheckNotNull ("invalidIdentityType", invalidIdentityType);

    _storageProviderType = storageProviderType;
    _invalidIdentityType = invalidIdentityType;
  }
  
  protected IdentityTypeNotSupportedException (SerializationInfo info, StreamingContext context) : base (info, context) 
  {
    _storageProviderType = (Type) info.GetValue ("StorageProviderType", typeof (Type));
    _invalidIdentityType = (Type) info.GetValue ("InvalidIdentityType", typeof (Type));
  }

  // methods and properties

  public Type StorageProviderType
  {
    get { return _storageProviderType; }
  }

  public Type InvalidIdentityType
  {
    get { return _invalidIdentityType; }
  }

  public override void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    base.GetObjectData (info, context);

    info.AddValue ("StorageProviderType", _storageProviderType);
    info.AddValue ("InvalidIdentityType", _invalidIdentityType);
  }
}
}
