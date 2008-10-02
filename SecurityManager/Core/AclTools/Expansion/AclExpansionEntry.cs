using System;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// Represents a row in an access control list expansion (see <see cref="AclExpander"/>, <see cref="AclExpansion"/>).
  /// </summary>
  public class AclExpansionEntry : IToText
  {
    public AclExpansionEntry (User user, Role role, 
                              AclExpansionAccessConditions accessConditions, AccessTypeDefinition[] accessTypeDefinitions)
    {
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNull ("role", role);
      ArgumentUtility.CheckNotNull ("accessConditions", accessConditions);
      ArgumentUtility.CheckNotNull ("accessTypeDefinitions", accessTypeDefinitions);
      User = user;
      Role = role;
      AccessConditions = accessConditions;
      AccessTypeDefinitions = accessTypeDefinitions;
    }

    public User User { get; set; }
    public Role Role { get; set; }
    public AclExpansionAccessConditions AccessConditions { get; set; }
    public AccessTypeDefinition[] AccessTypeDefinitions { get; set; }


    public void ToText (IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib().e ("user", User.UserName).e ("role", Role).e (AccessTypeDefinitions).e ("conditions", AccessConditions).ie ();
    }
  }
}