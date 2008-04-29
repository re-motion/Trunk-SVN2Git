using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Security;
using Remotion.Utilities;
using System.Security.Principal;

namespace Remotion.Data.DomainObjects.Security
{
  [Flags]
  public enum RequiredSecurityForStates
  {
    None = 0,
    New = 1,
    Deleted = 2,
    NewAndDeleted = 3
  }

  [Serializable]
  public class DomainObjectSecurityStrategy : ObjectSecurityStrategy
  {
    // types

    // static members

    // member fields

    private RequiredSecurityForStates _requiredSecurityForStates;

    // construction and disposing

    public DomainObjectSecurityStrategy (
        RequiredSecurityForStates requiredSecurityForStates,
        IDomainObjectSecurityContextFactory securityContextFactory, 
        ISecurityStrategy securityStrategy)
      : base (securityContextFactory, securityStrategy)
    {
      _requiredSecurityForStates = requiredSecurityForStates;
    }

    public DomainObjectSecurityStrategy (
        RequiredSecurityForStates requiredSecurityForStates, 
        IDomainObjectSecurityContextFactory securityContextFactory)
      : base (securityContextFactory)
    {
      _requiredSecurityForStates = requiredSecurityForStates;
    }

    // methods and properties

    public override bool HasAccess (ISecurityProvider securityService, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      if (SecurityContextFactory.IsDiscarded)
        return true;

      bool isSecurityRequiredForNew = RequiredSecurityForStates.New == (RequiredSecurityForStates.New & _requiredSecurityForStates);
      if (!isSecurityRequiredForNew && SecurityContextFactory.IsNew)
        return true;

      bool isSecurityRequiredForDeleted = RequiredSecurityForStates.Deleted == (RequiredSecurityForStates.Deleted & _requiredSecurityForStates);
      if (!isSecurityRequiredForDeleted && SecurityContextFactory.IsDeleted)
        return true;

      return base.HasAccess (securityService, user, requiredAccessTypes);
    }

    public new IDomainObjectSecurityContextFactory SecurityContextFactory
    {
      get { return (IDomainObjectSecurityContextFactory) base.SecurityContextFactory; }
    }

    public RequiredSecurityForStates RequiredSecurityForStates
    {
      get { return _requiredSecurityForStates; }
    }
  }
}