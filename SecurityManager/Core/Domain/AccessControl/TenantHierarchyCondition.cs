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
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [EnumDescriptionResource ("Remotion.SecurityManager.Globalization.Domain.AccessControl.TenantHierarchyCondition")]
  [UndefinedEnumValue (Undefined)]
  //[Flags] //Must not be official flags enum since business objects interface does not support this.
  [DisableEnumValues (Parent)]
  public enum TenantHierarchyCondition
  {
    Undefined = 0,
    This = 1,
    Parent = 2,
    ThisAndParent = This | Parent,
  }
}