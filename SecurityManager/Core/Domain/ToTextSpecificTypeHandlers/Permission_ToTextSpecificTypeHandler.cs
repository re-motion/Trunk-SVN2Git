// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 
using System;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.Domain.ToTextSpecificTypeHandlers
{
  [ToTextSpecificHandler]
  public class Permission_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<Permission>
  {
    public override void ToText (Permission x, IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<Permission> ("").e (x.AccessType).e (x.Allowed).ie ();
    }
  }
}