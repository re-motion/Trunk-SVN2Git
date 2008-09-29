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
    
    public override bool Equals (object obj)
    {
      var ac = obj as AclExpansionAccessConditions;
      if (ac == null)
      {
        return false;
      }

      return (ac.OnlyIfAbstractRoleMatches == OnlyIfAbstractRoleMatches) && (ac.AbstractRole == AbstractRole) &&
        (ac.OnlyIfGroupIsOwner == OnlyIfGroupIsOwner) && (ac.OnlyIfTenantIsOwner == OnlyIfTenantIsOwner) &&
        (ac.OnlyIfUserIsOwner == OnlyIfUserIsOwner);
    }
  }
}