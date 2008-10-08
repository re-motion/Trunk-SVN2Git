using System;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpansionAccessConditions  : IToText  
  {
    public bool IsOwningUserRequired { get; set; }
    public bool IsOwningGroupRequired { get; set; }
    public bool IsOwningTenantRequired { get; set; }
    public bool IsAbstractRoleRequired
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

      return (ac.AbstractRole == AbstractRole) &&
        (ac.IsOwningGroupRequired == IsOwningGroupRequired) && (ac.IsOwningTenantRequired == IsOwningTenantRequired) &&
        (ac.IsOwningUserRequired == IsOwningUserRequired);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (AbstractRole,IsOwningGroupRequired,IsOwningTenantRequired,IsOwningUserRequired);
    }


    public void ToText (IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<AclExpansionAccessConditions>("");
      toTextBuilder.eIfNotEqualTo ("userMustOwn", IsOwningUserRequired, false).eIfNotEqualTo ("groupMustOwn", IsOwningGroupRequired, false);
      toTextBuilder.eIfNotEqualTo ("tenantMustOwn", IsOwningTenantRequired, false);
      toTextBuilder.eIfNotEqualTo ("abstractRoleMustMatch", IsAbstractRoleRequired, false).eIfNotNull ("abstractRole", AbstractRole);
      toTextBuilder.ie ();
    }
  }
}