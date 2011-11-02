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
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Security;
using Remotion.SecurityManager.Domain.SearchInfrastructure;
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

    public static IQueryable<Tenant> FindAll ()
    {
      return from t in QueryFactory.CreateLinqQuery<Tenant>()
             orderby t.Name
             select t;
    }

    public static Tenant FindByUnqiueIdentifier (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);

      var result = from t in QueryFactory.CreateLinqQuery<Tenant>()
                   where t.UniqueIdentifier == uniqueIdentifier
                   select t;

      return result.SingleOrDefault();
    }

    [DemandPermission (GeneralAccessTypes.Search)]
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
    [SearchAvailableObjectsServiceType (typeof (TenantPropertyTypeSearchService))]
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
    /// Gets all the <see cref="Tenant"/> objects in the <see cref="Parent"/> hierarchy, 
    /// provided the user has read access for the respective parent-object.
    /// </summary>
    /// <exception cref="PermissionDeniedException">
    /// Thrown if the user does not have <see cref="GeneralAccessTypes.Read"/> permissions on the current object.
    /// </exception>
    [DemandPermission (GeneralAccessTypes.Read)]
    public IEnumerable<Tenant> GetParents ()
    {
      var securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      securityClient.CheckMethodAccess (this, "GetParents");

      Func<Tenant, Tenant> parentResolver = g =>
      {
        if (g == this)
        {
          throw new InvalidOperationException (
              string.Format ("The parent hierarchy for group '{0}' cannot be resolved because a circular reference exists.", ID));
        }
        return g.Parent;
      };

      return Parent.CreateSequence (parentResolver, g => g != null && securityClient.HasAccess (g, AccessType.Get (GeneralAccessTypes.Read)));
    }

    /// <summary>
    /// Gets the <see cref="Tenant"/> objects that can be used as the parent for this <see cref="Tenant"/>, 
    /// provided the user as read access for the respective object.
    /// </summary>
    /// <returns>
    /// Returns all <see cref="Tenant"/> objects in the system, except those in the child-hierarchy
    /// and those for which the user does not have <see cref="GeneralAccessTypes.Read"/> access.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the child-hierarchy of this <see cref="Tenant"/> contains a circular reference.
    /// </exception>
    public IEnumerable<Tenant> GetPossibleParentTenants ()
    {
      Tenant[] hierarchy;
      using (new SecurityFreeSection())
      {
        hierarchy = GetHierachy().ToArray();
      }

      var securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      return Tenant.FindAll().ToArray().Except (hierarchy).Where (t => securityClient.HasAccess (t, AccessType.Get (GeneralAccessTypes.Read)));
    }

    /// <summary>
    /// Gets the <see cref="Tenant"/> and all of its <see cref="Children"/>.
    /// </summary>
    /// <remarks>
    ///   If the user does not have read access for the respective tenant, the hierarchy evaluation stops at this point for the evaluated branch.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the hierarchy contains a circular reference.
    /// </exception>
    public IEnumerable<Tenant> GetHierachy ()
    {
      return GetHierarchyWithSecurityCheck (this);
    }

    /// <summary>
    /// Resolves the hierarchy for the current tenant as long as the user has <see cref="GeneralAccessTypes.Read"/> permissions on the current object.
    /// </summary>
    private IEnumerable<Tenant> GetHierarchyWithSecurityCheck (Tenant startPoint)
    {
      var securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      if (!securityClient.HasAccess (this, AccessType.Get (GeneralAccessTypes.Read)))
        return new Tenant[0];

      return new[] { this }.Concat (Children.SelectMany (c => c.GetHierarchyWithCircularReferenceCheck (startPoint)));
    }

    /// <summary>
    /// Resolves the hierarchy for the current tenant as long as the current object is not equal to the <param name="startPoint"/>, 
    /// which would indicate a circular reference.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the current object equals the <paramref name="startPoint"/>.
    /// </exception>
    private IEnumerable<Tenant> GetHierarchyWithCircularReferenceCheck (Tenant startPoint)
    {
      if (this == startPoint)
      {
        throw new InvalidOperationException (
            string.Format ("The hierarchy for tenant '{0}' cannot be resolved because a circular reference exists.", startPoint.ID));
      }

      return GetHierarchyWithSecurityCheck (startPoint);
    }
  }
}