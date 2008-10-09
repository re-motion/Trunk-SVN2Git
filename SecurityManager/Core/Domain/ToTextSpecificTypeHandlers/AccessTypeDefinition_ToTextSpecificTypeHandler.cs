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
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Text.StringExtensions;

namespace Remotion.SecurityManager.Domain.ToTextSpecificTypeHandlers
{
  [ToTextSpecificHandler]
  public class AccessTypeDefinition_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<AccessTypeDefinition>
  {
    public override void ToText (AccessTypeDefinition x, IToTextBuilder toTextBuilder)
    {
      //string shortName = x.Name.LeftUntilChar('|');
      string shortName = x.DisplayName;

      if (toTextBuilder.OutputComplexity >= ToTextBuilderBase.ToTextBuilderOutputComplexityLevel.Complex)
      {
        //toTextBuilder.sbLiteral ("ATD(",")").elements (x.Name, x.Value).se();
        toTextBuilder.ib<AccessTypeDefinition> ("").elements (shortName, x.Value).se ();
      }
      else
      {
        //toTextBuilder.ib<AccessTypeDefinition> ("").e (x.Name).se();
        toTextBuilder.ib<AccessTypeDefinition> ("").e (shortName).se ();
      }
    }

   
  }
}