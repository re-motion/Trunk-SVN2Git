using System;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpansionAccessConditions  : IToText  
  {
    public bool OnlyIfUserIsOwner { get; set; }
    public bool OnlyIfGroupIsOwner { get; set; }
    public bool OnlyIfTenantIsOwner { get; set; }
    public bool OnlyIfAbstractRoleMatches
    {
      get { return AbstractRole != null; }
    }
    public AbstractRoleDefinition AbstractRole { get; set; }
    

    public override bool Equals (object obj)
    {
      var ac = obj as AclExpansionAccessConditions;
      if (ac == null)
      {
        return false;
      }

      //return (ac.OnlyIfAbstractRoleMatches == OnlyIfAbstractRoleMatches) && (ac.AbstractRole == AbstractRole) &&
      //  (ac.OnlyIfGroupIsOwner == OnlyIfGroupIsOwner) && (ac.OnlyIfTenantIsOwner == OnlyIfTenantIsOwner) &&
      //  (ac.OnlyIfUserIsOwner == OnlyIfUserIsOwner);
      
      return (ac.AbstractRole == AbstractRole) &&
        (ac.OnlyIfGroupIsOwner == OnlyIfGroupIsOwner) && (ac.OnlyIfTenantIsOwner == OnlyIfTenantIsOwner) &&
        (ac.OnlyIfUserIsOwner == OnlyIfUserIsOwner);
    }


    public void ToText (IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<AclExpansionAccessConditions>("");
      toTextBuilder.eIfNotEqualTo ("userMustOwn", OnlyIfUserIsOwner, false).eIfNotEqualTo ("groupMustOwn", OnlyIfGroupIsOwner, false);
      toTextBuilder.eIfNotEqualTo ("tenantMustOwn", OnlyIfTenantIsOwner, false);
      toTextBuilder.eIfNotEqualTo ("abstractRoleMustMatch", OnlyIfAbstractRoleMatches, false).eIfNotNull ("abstractRole", AbstractRole);
      toTextBuilder.ie ();
    }
  }
}