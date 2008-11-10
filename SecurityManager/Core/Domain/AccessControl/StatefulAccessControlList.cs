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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
  [Instantiable]
  public abstract class StatefulAccessControlList : AccessControlList
  {
    public static StatefulAccessControlList NewObject ()
    {
      return NewObject<StatefulAccessControlList>().With ();
    }

    public new static StatefulAccessControlList GetObject (ObjectID id)
    {
      return GetObject<StatefulAccessControlList> (id);
    }

    private ObjectList<StateCombination> _stateCombinationsToBeDeleted;

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

      _stateCombinationsToBeDeleted = StateCombinations.Clone();
    }

    //TODO: Rewrite with test
    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      foreach (var stateCombination in _stateCombinationsToBeDeleted)
        stateCombination.Delete();
      _stateCombinationsToBeDeleted = null;
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