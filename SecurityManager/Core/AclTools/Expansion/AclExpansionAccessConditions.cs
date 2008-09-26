using System;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpansionAccessConditions
  {
    public bool OnlyIfUserIsOwner { get; set; }
    public bool OnlyIfGroupIsOwner { get; set; }
    public bool OnlyIfTenantIsOwner { get; set; }
    public bool OnlyIfAbstractRoleMatches { get; set; }
    public AbstractRoleDefinition AbstractRole { get; set; }
  }
}