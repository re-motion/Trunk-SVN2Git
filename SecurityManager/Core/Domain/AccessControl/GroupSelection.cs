/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
