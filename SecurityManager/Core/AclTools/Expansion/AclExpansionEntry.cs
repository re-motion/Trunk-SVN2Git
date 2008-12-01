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
  // TODO AE: Make this class immutable. (Its set accessors aren't used from the outside anyways.)
  // TODO AE: Remove commented code. (Do not commit.)
  public class AclExpansionEntry : IToTextConvertible
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
      get { return AccessControlList.Class; }
    }


    // TODO AE: Properties throwing exceptions are not good style. Consider returning a useful default (null? empty list?) or create a method.
    public IList<StateCombination> StateCombinations
    {
      get
      {
        if (AccessControlList is StatefulAccessControlList)
        {
          return ((StatefulAccessControlList) AccessControlList).StateCombinations;
        }
        else
        {
          //StateCombination stateCombination = StateCombination.NewObject ();
          //stateCombination.AccessControlList = _accessControlList;
          //return new StateCombination[] { stateCombination  };
          //return new StateCombination[0];

          // Throw exception in case of StatelessAccessControlList, to avoid "silent failure" in calling code
          throw new InvalidOperationException (@"StateCombinations not defined for StatelessAccessControlList. Test for ""is StatefulAccessControlList"" in calling code.");
        }
      }
    }

    public AclExpansionAccessConditions AccessConditions { get; set; }
    public AccessTypeDefinition[] AllowedAccessTypes { get; set; }
    public AccessTypeDefinition[] DeniedAccessTypes { get; set; }

    public AccessControlList AccessControlList
    {
      get { return _accessControlList; }
    }


    public void ToText (IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      toTextBuilder.ib<AclExpansionEntry> ("").e ("user", User.UserName).e ("role", Role);
      toTextBuilder.e ("allowed",AllowedAccessTypes).e ("denied",DeniedAccessTypes);
      toTextBuilder.e ("conditions", AccessConditions).ie ();
    }
  }
}