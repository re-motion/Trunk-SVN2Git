using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.Security.Data.DomainObjects;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [PermanentGuid ("8DBA42FE-ECD9-4b10-8F79-48E7A1119414")]
  [Serializable]
  public abstract class OrganizationalStructureObject : BaseSecurityManagerObject, ISecurableObject, IDomainObjectSecurityContextFactory
  {
    private DomainObjectSecurityStrategy _securityStrategy;

    protected OrganizationalStructureObject ()
    {
    }

    protected virtual string GetOwningTenant ()
    {
      return null;
    }

    protected virtual string GetOwner ()
    {
      return null;
    }

    protected virtual string GetOwningGroup ()
    {
      return null;
    }

    protected virtual IDictionary<string, Enum> GetStates ()
    {
      return new Dictionary<string, Enum>();
    }

    protected virtual IList<Enum> GetAbstractRoles ()
    {
      return new List<Enum>();
    }

    ISecurityContext ISecurityContextFactory.CreateSecurityContext ()
    {
      using (new SecurityFreeSection())
      {
        return new SecurityContext (GetPublicDomainObjectType(), GetOwner(), GetOwningGroup(), GetOwningTenant(), GetStates(), GetAbstractRoles());
      }
    }

    bool IDomainObjectSecurityContextFactory.IsDiscarded
    {
      get { return IsDiscarded; }
    }

    bool IDomainObjectSecurityContextFactory.IsNew
    {
      get { return State == StateType.New; }
    }

    bool IDomainObjectSecurityContextFactory.IsDeleted
    {
      get { return State == StateType.Deleted; }
    }

    IObjectSecurityStrategy ISecurableObject.GetSecurityStrategy ()
    {
      if (_securityStrategy == null)
        _securityStrategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.None, this);

      return _securityStrategy;
    }

    Type ISecurableObject.GetSecurableType ()
    {
      return GetPublicDomainObjectType();
    }
  }
}