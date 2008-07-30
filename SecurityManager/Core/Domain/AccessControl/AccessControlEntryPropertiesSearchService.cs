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
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// The <see cref="AccessControlEntryPropertiesSearchService"/> is used to return the values that can be assigned to the various 
  /// properties of <see cref="AccessControlEntry"/>.
  /// </summary>
  public class AccessControlEntryPropertiesSearchService : ISearchAvailableObjectsService
  {
    private const string c_specificAbstractRoleName = "SpecificAbstractRole";
    private const string c_specificTenantName = "SpecificTenant";
    private const string c_specificPositionName = "SpecificPosition";

    public bool SupportsIdentity (IBusinessObjectReferenceProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      switch (property.Identifier)
      {
        case c_specificTenantName:
          return true;
        case c_specificPositionName:
          return true;
        case c_specificAbstractRoleName:
          return true;
        default:
          return false;
      }
    }

    public IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, string searchStatement)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      switch (property.Identifier)
      {
        case c_specificTenantName:
          return Tenant.FindAll().ToArray();
        case c_specificPositionName:
          return Position.FindAll ().ToArray ();
        case c_specificAbstractRoleName:
          return AbstractRoleDefinition.FindAll().ToArray ();
        default:
          throw new ArgumentException (
              string.Format (
                  "The property '{0}' is not supported by the '{1}' type.",
                  property.DisplayName,
                  typeof (AccessControlEntryPropertiesSearchService).FullName));
      }
    }
  }
}