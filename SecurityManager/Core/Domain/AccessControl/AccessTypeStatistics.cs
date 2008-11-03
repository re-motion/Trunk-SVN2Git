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

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// Used to collect information about matching <see cref="AccessControlEntry"/>|s in calls
  /// to <see cref="AccessControlList.GetAccessTypes(SecurityToken, AccessTypeStatistics)"/>.
  /// </summary>
  //TODO MGI: Where are the unit tests?
  //TODO MGI: Where are the argument checks?
  public class AccessTypeStatistics
  {
    private readonly List<AccessControlEntry> _accessTypesSupplyingAces = new List<AccessControlEntry>();

    public void AddAccessTypesSupplyingAce (AccessControlEntry ace)
    {
      if (!IsInAccessTypesSupplyingAces(ace))
      {
        _accessTypesSupplyingAces.Add (ace);
      }
    }

    public bool IsInAccessTypesSupplyingAces (AccessControlEntry ace)
    {
      //TODO MGI: Why not use contains?
      return _accessTypesSupplyingAces.Find (x => (x == ace)) != null;
    }
  }
}