// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [EnumDescriptionResource ("Remotion.SecurityManager.Globalization.Domain.AccessControl.GroupHierarchyCondition")]
  [UndefinedEnumValue (Undefined)]
  //[Flags] //Must not be official flags enum since business objects interface does not support this.
  [DisableEnumValues (Parent, Children)]
  public enum GroupHierarchyCondition
  {
    Undefined = 0,
    This = 1,
    Parent = 2,
    Children = 4,
    ThisAndParent = This | Parent, // 3
    ThisAndChildren = This | Children, // 5
    ThisAndParentAndChildren = This | Parent | Children, // 7
  }
}
