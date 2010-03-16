// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Globalization;
using Remotion.Security;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.Tenant")]
  [PermanentGuid ("BD8FB1A4-E300-4663-AB1E-D6BD7B106619")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Tenant : OrganizationalStructureObject
  {
    public enum Methods
    {
      Search
    }

    internal static Tenant NewObject ()
    {
      return NewObject<Tenant>();
    }

    public new static Tenant GetObject (ObjectID id)
    {
      return GetObject<Tenant> (id);
    }

    public static ObjectList<Tenant> FindAll ()
    {
      var result = from t in QueryFactory.CreateLinqQuery<Tenant>()
                   orderby t.Name
                   select t;

      return result.ToObjectList();
    }

    public static Tenant FindByUnqiueIdentifier (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);

      var result = from t in QueryFactory.CreateLinqQuery<Tenant>()
                   where t.UniqueIdentifier == uniqueIdentifier
                   select t;

      return result.ToArray().SingleOrDefault();
    }

    [DemandMethodPermission (GeneralAccessTypes.Search)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public static void Search ()
    {
      throw new NotImplementedException ("This method is only intended for framework support and should never be called.");
    }

    protected Tenant ()
    {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
      UniqueIdentifier = Guid.NewGuid().ToString();
// ReSharper restore DoNotCallOverridableMethodsInConstructor
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string UniqueIdentifier { get; set; }

    public abstract bool IsAbstract { get; set; }

    [DBBidirectionalRelation ("Children")]
    public abstract Tenant Parent { get; set; }

    [DBBidirectionalRelation ("Parent")]
    public abstract ObjectList<Tenant> Children { get; }

    public override string DisplayName
    {
      get { return Name; }
    }

    protected override string GetOwningTenant ()
    {
      return UniqueIdentifier;
    }

    /// <summary>
    /// Gets the <see cref="Tenant"/> objects that can be used as the parent for this <see cref="Tenant"/>, 
    /// provided the user as read access for the respective object.
    /// </summary>
    /// <returns>
    /// Returns all <see cref="Tenant"/> objects in the system, except those in the child-hierarchy
    /// and those for which the user does not have <see cref="GeneralAccessTypes.Read"/> access.
    /// </returns>
    public IEnumerable<Tenant> GetPossibleParentTenants ()
    {
      Tenant[] hierarchy;
      using (new SecurityFreeSection())
      {
        hierarchy = GetHierachy().ToArray();
      }

      var securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      return Tenant.FindAll().Except (hierarchy).Where (t => securityClient.HasAccess (t, AccessType.Get (GeneralAccessTypes.Read)));
    }

    /// <summary>
    /// Gets the <see cref="Tenant"/> and all of its <see cref="Children"/>, provided the user as read access for the respective object.
    /// </summary>
    /// <remarks>This collection will be empty, if the user does not have <see cref="GeneralAccessTypes.Read"/> access on the current object.</remarks>
    public IEnumerable<Tenant> GetHierachy ()
    {
      var securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      if (!securityClient.HasAccess (this, AccessType.Get (GeneralAccessTypes.Read)))
        return new Tenant[0];

      return new[] { this }.Union (Children.SelectMany (c => c.GetHierachy()));
    }
  }
}