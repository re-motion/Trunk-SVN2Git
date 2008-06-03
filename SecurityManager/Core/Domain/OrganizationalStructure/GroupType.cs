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
using Remotion.Globalization;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.GroupType")]
  [PermanentGuid ("BDBB9696-177B-4b73-98CF-321B2FBEAD0C")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class GroupType : OrganizationalStructureObject
  {
    public enum Methods
    {
      Search
    }

    public static GroupType NewObject ()
    {
      return NewObject<GroupType> ().With ();
    }

    public static DomainObjectCollection FindAll ()
    {
      Query query = new Query ("Remotion.SecurityManager.Domain.OrganizationalStructure.GroupType.FindAll");
      return ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
    }

    [DemandMethodPermission (GeneralAccessTypes.Search)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public static void Search ()
    {
      throw new NotImplementedException ("This method is only intended for framework support and should never be called.");
    }

    protected GroupType ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("GroupType")]
    public abstract ObjectList<Group> Groups { get; }

    [DBBidirectionalRelation ("GroupType")]
    public abstract ObjectList<GroupTypePosition> Positions { get; }

    [EditorBrowsable (EditorBrowsableState.Never)]
    [DBBidirectionalRelation ("SpecificGroupType")]
    protected abstract ObjectList<AccessControlEntry> AccessControlEntries { get; }

    //TODO: UnitTests
    public override string DisplayName
    {
      get { return Name; }
    }
  }
}
