// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class StateCombination : AccessControlObject
  {
    // types

    // static members and constants

    public static StateCombination NewObject ()
    {
      return NewObject<StateCombination> ();
    }

    // member fields

    private DomainObjectDeleteHandler _deleteHandler;

    // construction and disposing

    protected StateCombination ()
    {
      SubscribeCollectionEvents();
    }

    // methods and properties

    //TODO: Add test for initialize during on load
    protected override void OnLoaded (LoadMode loadMode)
    {
      base.OnLoaded (loadMode);
      SubscribeCollectionEvents (); // always subscribe collection events when the object gets a new data container
    }

    private void SubscribeCollectionEvents ()
    {
      StateUsages.Added += StateUsages_Added;
    }

    private void StateUsages_Added (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      if (Class != null )
        Class.Touch ();
    }

    public abstract int Index { get; set; }

    [DBBidirectionalRelation ("StateCombination")]
    public abstract ObjectList<StateUsage> StateUsages { get; }

    [StorageClassNone]
    public SecurableClassDefinition Class
    {
      get { return AccessControlList != null ? AccessControlList.Class : null; }
    }

    [DBBidirectionalRelation ("StateCombinations")]
    [Mandatory]
    public abstract StatefulAccessControlList AccessControlList { get; set; }

    public bool MatchesStates (IList<StateDefinition> states)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("states", states);

      if (StateUsages.Count == 0 && states.Count > 0)
        return false;

      foreach (StateUsage stateUsage in StateUsages)
      {
        if (!states.Contains (stateUsage.StateDefinition))
          return false;
      }

      return true;
    }

    public void AttachState (StateDefinition state)
    {
      StateUsage stateUsage = StateUsage.NewObject ();
      stateUsage.StateDefinition = state;
      StateUsages.Add (stateUsage);
    }

    public StateDefinition[] GetStates ()
    {
      return StateUsages.Select (stateUsage => stateUsage.StateDefinition).ToArray();
    }

    //TODO: Rewrite with test

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _deleteHandler = new DomainObjectDeleteHandler (StateUsages);
    }
    
    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      _deleteHandler.Delete();
    }
  }
}
