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
