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
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
{
  public class AclExpansionEntryCreator_GetAccessTypesResult
  {
    public AclProbe AclProbe { get; private set; }
    public AccessTypeStatistics AccessTypeStatistics { get; private set; }
    public AccessInformation AccessInformation { get; private set; }

    public AclExpansionEntryCreator_GetAccessTypesResult (AccessInformation accessInformation, AclProbe aclProbe, AccessTypeStatistics accessTypeStatistics)
    {
      AclProbe = aclProbe;
      AccessTypeStatistics = accessTypeStatistics;
      AccessInformation = accessInformation;
    }
  }
}