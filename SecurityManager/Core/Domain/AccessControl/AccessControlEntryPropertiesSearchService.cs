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
  /// Implementation of <see cref="ISearchAvailableObjectsService"/> for the <see cref="AccessControlEntry"/> type.
  /// </summary>
  /// <remarks>
  /// The service is applied to the <see cref="AccessControlEntry.SpecificTenant"/>, <see cref="AccessControlEntry.SpecificPosition"/>, 
  /// and <see cref="AccessControlEntry.SpecificAbstractRole"/> properties via the <see cref="SearchAvailableObjectsServiceTypeAttribute"/>.
  /// </remarks>
  public class AccessControlEntryPropertiesSearchService : SecurityManagerSearchServiceBase<AccessControlEntry>
  {
    public AccessControlEntryPropertiesSearchService ()
    {
      AddSearchDelegate ("SpecificTenant", delegate { return Tenant.FindAll().ToArray(); });
      AddSearchDelegate ("SpecificGroup", SearchGroups);
      AddSearchDelegate ("SpecificUser", SearchUsers);
      AddSearchDelegate ("SpecificPosition", delegate { return Position.FindAll().ToArray(); });
      AddSearchDelegate ("SpecificGroupType", delegate { return GroupType.FindAll().ToArray(); });
      AddSearchDelegate ("SpecificAbstractRole", delegate { return AbstractRoleDefinition.FindAll().ToArray(); });
    }

    private IBusinessObject[] SearchGroups (
        AccessControlEntry referencingObject, IBusinessObjectReferenceProperty property, ISearchAvailableObjectsArguments searchArguments)
    {
      var defaultSearchArguments = ArgumentUtility.CheckNotNullAndType<DefaultSearchArguments> ("searchArguments", searchArguments);
      ArgumentUtility.CheckNotNullOrEmpty ("defaultSearchArguments.SearchStatement", defaultSearchArguments.SearchStatement);
      ObjectID tenantID = ObjectID.Parse (defaultSearchArguments.SearchStatement);

      return Group.FindByTenantID (tenantID).ToArray();
    }

    private IBusinessObject[] SearchUsers (
        AccessControlEntry referencingObject, IBusinessObjectReferenceProperty property, ISearchAvailableObjectsArguments searchArguments)
    {
      var defaultSearchArguments = ArgumentUtility.CheckNotNullAndType<DefaultSearchArguments> ("searchArguments", searchArguments);
      ArgumentUtility.CheckNotNullOrEmpty ("defaultSearchArguments.SearchStatement", defaultSearchArguments.SearchStatement);
      ObjectID tenantID = ObjectID.Parse (defaultSearchArguments.SearchStatement);

      return User.FindByTenantID (tenantID).ToArray();
    }
  }
}