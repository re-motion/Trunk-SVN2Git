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
using System.Collections.Generic;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.ToTextSpecificTypeHandlers
{
  [ToTextSpecificHandler]
  public class ListOfAclExpansionEntry_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<List<AclExpansionEntry>>
  {
    public override void ToText (List<AclExpansionEntry> listOfAclExpansionEntry, IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<List<AclExpansionEntry>> ("").nl();
      foreach (AclExpansionEntry aclExpansionEntry in listOfAclExpansionEntry)
      {
        toTextBuilder.e (aclExpansionEntry).nl ();
      }
      toTextBuilder.ie ();
    }
  }
}