using System;
using System.Collections.Generic;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// Represents a row in an access control list expansion (see <see cref="AclExpander"/>).
  /// </summary>
  public class AclExpansionEntry : IToText
  {
    private readonly AccessControlList _accessControlList;

    public AclExpansionEntry (
        User user,
        Role role,
        AccessControlList accessControlList,
        AclExpansionAccessConditions accessConditions,
        AccessTypeDefinition[] allowedAccessTypes,
        AccessTypeDefinition[] deniedAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNull ("role", role);
      ArgumentUtility.CheckNotNull ("accessControlList", accessControlList);
      ArgumentUtility.CheckNotNull ("accessConditions", accessConditions);
      ArgumentUtility.CheckNotNull ("accessTypeDefinitions", allowedAccessTypes);
      User = user;
      Role = role;
      _accessControlList = accessControlList;
      AccessConditions = accessConditions;
      AllowedAccessTypes = allowedAccessTypes;
      DeniedAccessTypes = deniedAccessTypes;
    }

    public User User { get; set; }
    public Role Role { get; set; }

    public SecurableClassDefinition Class
    {
      get { return _accessControlList.Class; }
    }

    public IList<StateCombination> StateCombinations
    {
      get
      {
        if (_accessControlList is StatefulAccessControlList)
          return ((StatefulAccessControlList) _accessControlList).StateCombinations;
        else
          return new StateCombination[0];
      }
    }

    public AclExpansionAccessConditions AccessConditions { get; set; }
    public AccessTypeDefinition[] AllowedAccessTypes { get; set; }
    public AccessTypeDefinition[] DeniedAccessTypes { get; set; }


    public void ToText (IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      toTextBuilder.ib<AclExpansionEntry> ("").e ("user", User.UserName).e ("role", Role).e (AllowedAccessTypes).e ("conditions", AccessConditions).ie();
    }
  }
}