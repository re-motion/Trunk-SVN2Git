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
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class AccessControlList : AccessControlObject
  {
    private ObjectList<AccessControlEntry> _accessControlEntriesToBeDeleted;

    protected AccessControlList ()
    {
      SubscribeCollectionEvents();
    }

    //TODO: Add test for initialize during on load
    protected override void OnLoaded (LoadMode loadMode)
    {
      base.OnLoaded (loadMode);
      SubscribeCollectionEvents(); // always subscribe collection events when the object gets a new data container
    }

    private void SubscribeCollectionEvents ()
    {
      AccessControlEntries.Added += AccessControlEntries_Added;
    }

    private void AccessControlEntries_Added (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      var ace = (AccessControlEntry) args.DomainObject;
      var accessControlEntries = AccessControlEntries;
      if (accessControlEntries.Count == 1)
        ace.Index = 0;
      else
        ace.Index = accessControlEntries[accessControlEntries.Count - 2].Index + 1;
      Touch();
    }

    public void Touch ()
    {
      if (State == StateType.Unchanged)
        MarkAsChanged ();
    }

    public abstract int Index { get; set; }

    [StorageClassNone]
    public abstract SecurableClassDefinition Class { get; }

    [DBBidirectionalRelation ("AccessControlList", SortExpression = "[Index] ASC")]
    public abstract ObjectList<AccessControlEntry> AccessControlEntries { get; }

    /// <summary>
    /// Returns the ACEs which match the passed <see cref="SecurityToken"/>
    /// </summary>
    /// <param name="token">The security token that will be matched against the ACL entries. Must not be <see langword="null" />.</param>
    /// <returns>array of ACEs</returns>
    public AccessControlEntry[] FindMatchingEntries (SecurityToken token)
    {
      ArgumentUtility.CheckNotNull ("token", token);

      var entries = new List<AccessControlEntry>();

      foreach (var entry in AccessControlEntries)
      {
        if (entry.MatchesToken (token))
          entries.Add (entry);
      }

      return entries.ToArray();
    }

    public AccessInformation GetAccessTypes (SecurityToken token, AccessTypeStatistics accessTypeStatistics)
    {
      ArgumentUtility.CheckNotNull ("token", token);

      var allowedAccessTypesResult = new Set<AccessTypeDefinition> ();
      var deniedAccessTypesResult = new Set<AccessTypeDefinition> ();

      foreach (var ace in FindMatchingEntries (token))
      {
        var allowedAccesTypesForCurrentAce = ace.GetAllowedAccessTypes();
        var deniedAccessTypesForCurrentAce = ace.GetDeniedAccessTypes ();

        // Record the ACEs that contribute to the resulting AccessTypeDefinition-array.
        // The recorded information allows deduction of whether the probing ACE was matched for ACL-expansion code
        // (see AclExpander.AddAclExpansionEntry).
        if (accessTypeStatistics != null)
        {
          accessTypeStatistics.AddMatchingAce (ace);
          if (allowedAccesTypesForCurrentAce.Length > 0 || deniedAccessTypesForCurrentAce.Length > 0)
          {
            accessTypeStatistics.AddAccessTypesContributingAce (ace);
          }
        }

        // Add allowed/denied access types of ACE to result
        allowedAccessTypesResult.AddRange (allowedAccesTypesForCurrentAce);
        deniedAccessTypesResult.AddRange (deniedAccessTypesForCurrentAce);
      }

      // Deny always wins => Remove allowed access types which are also denied from result.
      foreach (var deniedAccessType in deniedAccessTypesResult)
        allowedAccessTypesResult.Remove (deniedAccessType);

      return new AccessInformation (allowedAccessTypesResult.ToArray (), deniedAccessTypesResult.ToArray ());
    }

    public AccessInformation GetAccessTypes (SecurityToken token)
    {
      ArgumentUtility.CheckNotNull ("token", token);
      return GetAccessTypes (token, null);
    }

    //TODO: Rewrite with test
    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _accessControlEntriesToBeDeleted = AccessControlEntries.Clone();
    }

    //TODO: Rewrite with test
    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      foreach (var accessControlEntry in _accessControlEntriesToBeDeleted)
        accessControlEntry.Delete();
      _accessControlEntriesToBeDeleted = null;
    }

    public AccessControlEntry CreateAccessControlEntry ()
    {
      if (Class == null)
        throw new InvalidOperationException ("Cannot create AccessControlEntry if no SecurableClassDefinition is assigned to this AccessControlList.");

      var accessControlEntry = AccessControlEntry.NewObject();
      foreach (var accessTypeDefinition in Class.AccessTypes)
        accessControlEntry.AttachAccessType (accessTypeDefinition);
      accessControlEntry.AccessControlList = this;

      return accessControlEntry;
    }
  }
}
