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
using System.ComponentModel;
using Remotion.Utilities;

namespace Remotion.Security
{
  // TODO FS: Move to Security.Interfaces
  public abstract class BaseDemandPermissionAttribute : Attribute
  {
    private readonly Enum[] _accessTypes;

    [EditorBrowsable (EditorBrowsableState.Never)]
    [Obsolete ("Do not use this constructor to initialize a new instance. It is required only for making the Attribute class CLS complient.", true)]
    protected BaseDemandPermissionAttribute ()
    {
      throw new NotSupportedException ("The default constructor is not supported by the BaseDemandPermissionAttribute. It is used only work around CLS compliancy issues of the C# compiler.");
    }

    protected BaseDemandPermissionAttribute (object[] accessTypes)
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("accessTypes", accessTypes);
      ArgumentUtility.CheckItemsType ("accessTypes", accessTypes, typeof (Enum));

      Enum[] accessTypeEnums = new Enum[accessTypes.Length];

      for (int i = 0; i < accessTypes.Length; i++)
        accessTypeEnums[i] = GetAccessType (accessTypes[i]);

      _accessTypes = accessTypeEnums;
    }

    public Enum[] GetAccessTypes ()
    {
      return _accessTypes;
    }

    private Enum GetAccessType (object accessType)
    {
      Type permissionType = accessType.GetType ();
      if (!permissionType.IsDefined (typeof (AccessTypeAttribute), false))
      {
        string message = string.Format (string.Format ("Enumerated Type '{0}' cannot be used as an access type. Valid access types must have the "
                + "Remotion.Security.AccessTypeAttribute applied.", permissionType.FullName));

        throw new ArgumentException (message, "accessType");
      }

      return (Enum) accessType;
    }
  }
}
