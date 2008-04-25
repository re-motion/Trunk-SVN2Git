using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using Remotion.Data.DomainObjects;
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
    // constants

    // types

    public enum Methods
    {
      Search
    }

    // static members

    private static readonly string s_currentKey = typeof (Tenant).AssemblyQualifiedName + "_Current";

    public static Tenant Current
    {
      get
      {
        ObjectID tenantID = (ObjectID) CallContext.GetData (s_currentKey);
        if (tenantID == null)
          return null;
        return GetObject (tenantID);
      }
      set
      {
        if (value == null)
          CallContext.SetData (s_currentKey, null);
        else
          CallContext.SetData (s_currentKey, value.ID);
      }
    }

    internal static Tenant NewObject ()
    {
      return NewObject<Tenant> ().With ();
    }

    public static new Tenant GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Tenant> (id);
    }

    public static DomainObjectCollection FindAll ()
    {
      Query query = new Query ("Remotion.SecurityManager.Domain.OrganizationalStructure.Tenant.FindAll");
      return ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
    }

    public static Tenant FindByUnqiueIdentifier (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);

      Query query = new Query ("Remotion.SecurityManager.Domain.OrganizationalStructure.Tenant.FindByUnqiueIdentifier");
      query.Parameters.Add ("@uniqueIdentifier", uniqueIdentifier);

      DomainObjectCollection tenants = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      if (tenants.Count == 0)
        return null;

      return (Tenant) tenants[0];
    }

    [DemandMethodPermission (GeneralAccessTypes.Search)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public static void Search ()
    {
      throw new NotImplementedException ("This method is only intended for framework support and should never be called.");
    }

    // member fields

    // construction and disposing

    protected Tenant ()
    {
      UniqueIdentifier = Guid.NewGuid ().ToString ();
    }

    // methods and properties

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

    // TODO: UnitTests
    public List<Tenant> GetPossibleParentTenants (ObjectID tenantID)
    {
      List<Tenant> clients = new List<Tenant> ();

      foreach (Tenant tenant in FindAll ())
      {
        if ((!Children.Contains (tenant.ID)) && (tenant.ID != this.ID))
          clients.Add (tenant);
      }
      return clients;
    }

    public ObjectList<Tenant> GetHierachy ()
    {
      ObjectList<Tenant> tenants = new ObjectList<Tenant> ();
      tenants.Add (this);
      foreach (Tenant tenant in Children)
        tenants.Combine (tenant.GetHierachy ());

      return tenants;
    }
  }
}
