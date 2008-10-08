using System;
using Remotion.Data.DomainObjects;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.AccessControl;
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
    private AccessControlList _accessControlList;

    public AclExpansionEntry (User user, Role role, AccessControlList accessControlList,
                              AclExpansionAccessConditions accessConditions, AccessTypeDefinition[] accessTypeDefinitions)
    {
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNull ("role", role);
      ArgumentUtility.CheckNotNull ("accessControlList", accessControlList);
      ArgumentUtility.CheckNotNull ("accessConditions", accessConditions);
      ArgumentUtility.CheckNotNull ("accessTypeDefinitions", accessTypeDefinitions);
      User = user;
      Role = role;
      _accessControlList = accessControlList;
      AccessConditions = accessConditions;
      AccessTypeDefinitions = accessTypeDefinitions;
    }

    public User User { get; set; }
    public Role Role { get; set; }

    public SecurableClassDefinition Class {
      get { return _accessControlList.Class; }
    }
    public ObjectList<StateCombination> StateCombinations
    {
      get { return _accessControlList.StateCombinations; }
    }

    public AclExpansionAccessConditions AccessConditions { get; set; }
    public AccessTypeDefinition[] AccessTypeDefinitions { get; set; }



    public void ToText (IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<AclExpansionEntry> ("").e ("user", User.UserName).e ("role", Role).e (AccessTypeDefinitions).e ("conditions", AccessConditions).ie ();
    }
  }
}