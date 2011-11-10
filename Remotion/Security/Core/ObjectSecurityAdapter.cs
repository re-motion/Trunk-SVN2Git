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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Security
{
  public class ObjectSecurityAdapter : IObjectSecurityAdapter
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public ObjectSecurityAdapter ()
    {
    }

    // methods and properties

    public bool HasAccessOnGetAccessor (ISecurableObject securableObject, IPropertyInformation propertyInformation)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);

      if (SecurityFreeSection.IsActive)
        return true;

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      return securityClient.HasPropertyReadAccess (securableObject, propertyInformation.GetGetMethod (true) ?? new NullMethodInformation());
    }

    public bool HasAccessOnSetAccessor (ISecurableObject securableObject, IPropertyInformation propertyInformation)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);

      if (SecurityFreeSection.IsActive)
        return true;

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      return securityClient.HasPropertyWriteAccess (securableObject, propertyInformation.GetSetMethod (true) ?? new NullMethodInformation ());
    }
  }
}
