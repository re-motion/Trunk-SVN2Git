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
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class AccessControlList : AccessControlObject
  {
    // types

    // static members and constants

    public static AccessControlList NewObject ()
    {
      return NewObject<AccessControlList>().With();
    }

    public new static AccessControlList GetObject (ObjectID id)
    {
      return DomainObject.GetObject<AccessControlList> (id);
    }

    // member fields

    private ObjectList<AccessControlEntry> _accessControlEntriesToBeDeleted;
    private ObjectList<StateCombination> _stateCombinationsToBeDeleted;

    // construction and disposing

    protected AccessControlList ()
    {
      SubscribeCollectionEvents();
    }

    // methods and properties

    //TODO: Add test for initialize during on load
    protected override void OnLoaded (LoadMode loadMode)
    {
      base.OnLoaded (loadMode);
      SubscribeCollectionEvents(); // always subscribe collection events when the object gets a new data container
    }

    private void SubscribeCollectionEvents ()
    {
      StateCombinations.Added += StateCombinations_Added;
      AccessControlEntries.Added += AccessControlEntries_Added;
    }

    private void StateCombinations_Added (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      var stateCombination = (StateCombination) args.DomainObject;
      var stateCombinations = StateCombinations;
      if (stateCombinations.Count == 1)
        stateCombination.Index = 0;
      else
        stateCombination.Index = stateCombinations[stateCombinations.Count - 2].Index + 1;
      Touch();
      if (Class != null)
        Class.Touch();
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

    [DBBidirectionalRelation ("AccessControlLists")]
    [DBColumn ("SecurableClassID")]
    [Mandatory]
    public abstract SecurableClassDefinition Class { get; set; }

    [DBBidirectionalRelation ("AccessControlList", SortExpression = "[Index] ASC")]
    [Mandatory]
    public abstract ObjectList<StateCombination> StateCombinations { get; }

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

      var allowedAccessTypes = new Set<AccessTypeDefinition> ();
      var deniedAccessTypes = new Set<AccessTypeDefinition> ();

      foreach (var ace in FindMatchingEntries (token))
      {
        foreach (var allowedAccessType in ace.GetAllowedAccessTypes ())
        {
          allowedAccessTypes.Add (allowedAccessType);

          // Record the ACEs that contribute to the resulting AccessTypeDefinition-array.
          // Note that the access control logic is not modified by this code. Recorded information allows
          // deduction of whether the probing ACE was matched for ACL-expansion code (see AclExpander.AddAclExpansionEntry).
          if (accessTypeStatistics != null)
          {
            accessTypeStatistics.AddAccessTypesSupplyingAce (ace);
          }
        }

        foreach (var deniedAccessType in ace.GetDeniedAccessTypes ())
        {
          deniedAccessTypes.Add (deniedAccessType);

          // Record the ACEs that contribute to the resulting AccessTypeDefinition-array.
          // Note that the access control logic is not modified by this code. Recorded information allows
          // deduction of whether the probing ACE was matched for ACL-expansion code (see AclExpander.AddAclExpansionEntry).
          if (accessTypeStatistics != null)
          {
            accessTypeStatistics.AddAccessTypesSupplyingAce (ace);
          }
        }
      }

      foreach (var deniedAccessType in deniedAccessTypes)
        allowedAccessTypes.Remove (deniedAccessType);

      return new AccessInformation (allowedAccessTypes.ToArray (), deniedAccessTypes.ToArray ());
    }


    public AccessInformation GetAccessTypes (SecurityToken token)
    {
      ArgumentUtility.CheckNotNull ("token", token);
      return GetAccessTypes (token, null);
    }


    //public AccessInformation GetAccessTypes (SecurityToken token)
    //{
    //  ArgumentUtility.CheckNotNull ("token", token);

    //  var allowedAccessTypes = new Set<AccessTypeDefinition> ();
    //  var deniedAccessTypes = new Set<AccessTypeDefinition> ();

    //  // Collect allowed and denied access types from each matching ACE
    //  foreach (var ace in FindMatchingEntries (token))
    //  {
    //    allowedAccessTypes.AddRange (ace.GetAllowedAccessTypes());
    //    deniedAccessTypes.AddRange (ace.GetDeniedAccessTypes ());
    //  }

    //  // Remove allowed access type entries which are denied
    //  foreach (var deniedAccessType in deniedAccessTypes)
    //  {
    //    allowedAccessTypes.Remove (deniedAccessType);
    //  }

    //  return new AccessInformation (allowedAccessTypes.ToArray (), deniedAccessTypes.ToArray ());
    //}



    
    //TODO: Rewrite with test

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _accessControlEntriesToBeDeleted = AccessControlEntries.Clone();
      _stateCombinationsToBeDeleted = StateCombinations.Clone();
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      foreach (var accessControlEntry in _accessControlEntriesToBeDeleted)
        accessControlEntry.Delete();
      _accessControlEntriesToBeDeleted = null;

      foreach (var stateCombination in _stateCombinationsToBeDeleted)
        stateCombination.Delete();
      _stateCombinationsToBeDeleted = null;
    }

    public StateCombination CreateStateCombination ()
    {
      if (Class == null)
        throw new InvalidOperationException ("Cannot create StateCombination if no SecurableClassDefinition is assigned to this AccessControlList.");

      var stateCombination = StateCombination.NewObject();
      stateCombination.Class = Class;
      stateCombination.AccessControlList = this;

      return stateCombination;
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
