using System;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [EnumDescriptionResource ("Remotion.SecurityManager.Globalization.Domain.AccessControl.GroupSelection")]
  public enum GroupSelection
  {
    All = 0,
    OwningGroup = 1,
    //SpecificGroup = 4,
    //SpecificGroupType = 6,
  }

  // OwningGroup, SpecificGroup
  //  OnlyThisGroup
  //  ThisGroupAndParentGroups
  //  OnlyParentGroups
  //  ThisGroupAndChildGroups
  //  OnlyChildGroups

  //  SpecificGroupType
  //   GroupTypeofGroup
  //   GroupTypeOfParentGroups
  //   GroupTypeOfChildGroups
}
