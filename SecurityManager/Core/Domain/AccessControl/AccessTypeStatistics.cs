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
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// Used to collect information about matching/contributing <see cref="AccessControlEntry"/>|s in calls
  /// to <see cref="AccessControlList.GetAccessTypes(SecurityToken, AccessTypeStatistics)"/>.
  /// </summary>
  public class AccessTypeStatistics
  {
    private readonly List<AccessControlEntry> _accessTypesSupplyingAces = new List<AccessControlEntry>();
    private readonly List<AccessControlEntry> _matchingAces = new List<AccessControlEntry> ();

    public List<AccessControlEntry> AccessTypesSupplyingAces
    {
      get { return _accessTypesSupplyingAces; }
    }

    public List<AccessControlEntry> MatchingAces
    {
      get { return _matchingAces; }
    }

    public void AddAccessTypesContributingAce (AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull ("ace", ace);
      if (!IsInAccessTypesContributingAces(ace))
      {
        AccessTypesSupplyingAces.Add (ace);
      }
    }

    /// <summary>
    /// Returns true if the passed <see cref="AccessControlEntry"/> has contributed either to the allowing or denying access types
    /// in the call to <see cref="AccessControlList.GetAccessTypes(SecurityToken,AccessTypeStatistics)"/>.
    /// </summary>
    public bool IsInAccessTypesContributingAces (AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull ("ace", ace);
      return AccessTypesSupplyingAces.Contains(ace);;
    }


    public void AddMatchingAce (AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull ("ace", ace);
      if (!IsInMatchingAces (ace))
      {
        _matchingAces.Add (ace);
      }
    }

    /// <summary>
    /// Returns true if the passed <see cref="AccessControlEntry"/> matched internally
    /// in the call to <see cref="AccessControlList.GetAccessTypes(SecurityToken,AccessTypeStatistics)"/>.
    /// </summary>
    /// <remarks>
    /// Note that an <see cref="AccessControlEntry"/> matching does not mean that it contributed to either 
    /// allowing or denying resulting access types (see <see cref="IsInAccessTypesContributingAces"/> to check for this).
    /// </remarks>
    public bool IsInMatchingAces (AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull ("ace", ace);
      return _matchingAces.Contains (ace);
    }
  }
}