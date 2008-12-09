// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
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
      return NewObject<StatefulAccessControlList>().With ();
    }

    public new static StatefulAccessControlList GetObject (ObjectID id)
    {
      return GetObject<StatefulAccessControlList> (id);
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
      StateCombinations.Added += StateCombinations_Added;
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

    public abstract int Index { get; set; }

    [DBBidirectionalRelation ("StatefulAccessControlLists")]
    [DBColumn ("StatefulAcl_ClassID")]
    [Mandatory]
    protected abstract SecurableClassDefinition MyClass { get; set; }

    [DBBidirectionalRelation ("AccessControlList", SortExpression = "[Index] ASC")]
    [Mandatory]
    public abstract ObjectList<StateCombination> StateCombinations { get; }

    public override SecurableClassDefinition Class
    {
      get { return MyClass; }
      set { MyClass = value; }
    }

    //TODO: Rewrite with test
    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _deleteHandler = new DomainObjectDeleteHandler (StateCombinations);
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
        throw new InvalidOperationException (
            "Cannot create StateCombination if no SecurableClassDefinition is assigned to this StatefulAccessControlList.");

      var stateCombination = StateCombination.NewObject();
      stateCombination.AccessControlList = this;

      return stateCombination;
    }
  }
}
