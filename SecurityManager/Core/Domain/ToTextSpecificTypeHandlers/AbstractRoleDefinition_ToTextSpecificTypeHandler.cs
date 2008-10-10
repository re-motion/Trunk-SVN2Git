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
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.ToTextSpecificTypeHandlers
{
  [ToTextSpecificHandler]
  public class AbstractRoleDefinition_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<AbstractRoleDefinition>
  {
    public override void ToText (AbstractRoleDefinition abstractRoleDefinition, IToTextBuilder toTextBuilder)
    {

      ArgumentUtility.CheckNotNull ("abstractRoleDefinition", abstractRoleDefinition);
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      toTextBuilder.ib<AbstractRoleDefinition> ("").e (abstractRoleDefinition.DisplayName).ie ();
    }
  }
}