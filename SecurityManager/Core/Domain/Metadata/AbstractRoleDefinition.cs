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
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
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
      return NewObject<AbstractRoleDefinition>().With();
    }

    public static AbstractRoleDefinition NewObject (Guid metadataItemID, string name, int value)
    {
      return NewObject<AbstractRoleDefinition>().With (metadataItemID, name, value);
    }

    public static ObjectList<AbstractRoleDefinition> Find (EnumWrapper[] abstractRoles)
    {
      ArgumentUtility.CheckNotNull ("abstractRoles", abstractRoles);

      var abstractRoleNames = (from abstractRole in abstractRoles select abstractRole.Name).ToArray();

      var result = from ar in QueryFactory.CreateQueryable<AbstractRoleDefinition>()
                   where abstractRoleNames.Contains (ar.Name)
                   orderby ar.Index
                   select ar;

      return result.ToObjectList();
    }

    public static ObjectList<AbstractRoleDefinition> FindAll ()
    {
      var result = from ar in QueryFactory.CreateQueryable<AbstractRoleDefinition>()
                   orderby ar.Index
                   select ar;

      return result.ToObjectList();
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
    [EditorBrowsable (EditorBrowsableState.Never)]
    protected abstract ObjectList<AccessControlEntry> AccessControlEntries { get; }
  }
}