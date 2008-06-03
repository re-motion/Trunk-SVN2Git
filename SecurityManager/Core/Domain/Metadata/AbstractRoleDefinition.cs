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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  public abstract class AbstractRoleDefinition : EnumValueDefinition
  {
    public static AbstractRoleDefinition NewObject ()
    {
      return NewObject<AbstractRoleDefinition> ().With ();
    }

    public static AbstractRoleDefinition NewObject (Guid metadataItemID, string name, int value)
    {
      return NewObject<AbstractRoleDefinition> ().With (metadataItemID, name, value);
    }

    public static DomainObjectCollection Find (EnumWrapper[] abstractRoles)
    {
      if (abstractRoles.Length == 0)
        return new DomainObjectCollection ();

      FindAbstractRolesQueryBuilder queryBuilder = new FindAbstractRolesQueryBuilder ();
      return ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (queryBuilder.CreateQuery (abstractRoles));
    }

    public static DomainObjectCollection FindAll ()
    {
      Query query = new Query ("Remotion.SecurityManager.Domain.Metadata.AbstractRoleDefinition.FindAll");
      return ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
    }

    protected AbstractRoleDefinition ()
    {
    }

    protected AbstractRoleDefinition (Guid metadataItemID, string name, int value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      MetadataItemID = metadataItemID;
      Name = name;
      Value = value;
    }

    [DBBidirectionalRelation ("SpecificAbstractRole")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected abstract ObjectList<AccessControlEntry> AccessControlEntries { get; }
  }
}
