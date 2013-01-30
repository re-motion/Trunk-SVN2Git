// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
  [Instantiable]
  public abstract class StatefulAccessControlList : AccessControlList
  {
    private DomainObjectDeleteHandler _deleteHandler;

    public static StatefulAccessControlList NewObject ()
    {
      return NewObject<StatefulAccessControlList>();
    }

    protected StatefulAccessControlList ()
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
      StateCombinationsInternal.Added += StateCombinations_Added;
    }

    private void StateCombinations_Added (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      var stateCombination = (StateCombination) args.DomainObject;
      var stateCombinations = StateCombinationsInternal;
      if (stateCombinations.Count == 1)
        stateCombination.Index = 0;
      else
        stateCombination.Index = stateCombinations[stateCombinations.Count - 2].Index + 1;
    }

    public abstract int Index { get; set; }

    [DBBidirectionalRelation ("StatefulAccessControlLists")]
    [DBColumn ("StatefulAcl_ClassID")]
    [Mandatory]
    protected abstract SecurableClassDefinition MyClass { get; }

    [DBBidirectionalRelation ("AccessControlList", SortExpression = "Index ASC")]
    [Mandatory]
    protected abstract ObjectList<StateCombination> StateCombinationsInternal { get; }

    [StorageClassNone]
    public ReadOnlyCollection<StateCombination> StateCombinations
    {
      get { return StateCombinationsInternal.AsReadOnlyCollection(); }
    }

    public override SecurableClassDefinition Class
    {
      get { return MyClass; }
    }

    //TODO: Rewrite with test
    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _deleteHandler = new DomainObjectDeleteHandler (StateCombinationsInternal);
    }

    //TODO: Rewrite with test
    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      _deleteHandler.Delete();
    }

    public StateCombination CreateStateCombination ()
    {
      if (Class == null)
      {
        throw new InvalidOperationException (
            "Cannot create StateCombination if no SecurableClassDefinition is assigned to this StatefulAccessControlList.");
      }

      var stateCombination = StateCombination.NewObject();
      StateCombinationsInternal.Add (stateCombination);

      return stateCombination;
    }
  }
}