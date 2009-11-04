// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
[Serializable]
public class DataContainerCollection : CommonCollection
{
  // types

  // static members and constants

  public static DataContainerCollection Join (DataContainerCollection firstCollection, DataContainerCollection secondCollection)
  {
    ArgumentUtility.CheckNotNull ("firstCollection", firstCollection);
    ArgumentUtility.CheckNotNull ("secondCollection", secondCollection);

    DataContainerCollection joinedCollection = new DataContainerCollection (firstCollection, false);
    foreach (DataContainer dataContainer in secondCollection)
    {
      if (!joinedCollection.Contains (dataContainer.ID))
        joinedCollection.Add (dataContainer);
    }

    return joinedCollection;
  }

  // member fields

  // construction and disposing

  public DataContainerCollection ()
  {
  }

  // standard constructor for collections
  public DataContainerCollection (IEnumerable collection, bool makeCollectionReadOnly)
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (DataContainer dataContainer in collection)
    {
      Add (dataContainer);
    }

    this.SetIsReadOnly (makeCollectionReadOnly);
  }

  // methods and properties

  public DataContainerCollection GetByState (StateType state)
  {
    ArgumentUtility.CheckValidEnumValue ("state", state);

    DataContainerCollection collection = new DataContainerCollection ();

    foreach (DataContainer dataContainer in this)
    {
      if (dataContainer.State == state)
        collection.Add (dataContainer);
    }

    return collection;
  }

  public DataContainerCollection GetDifference (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    DataContainerCollection difference = new DataContainerCollection ();

    foreach (DataContainer dataContainer in this)
    {
      if (!dataContainers.Contains (dataContainer.ID))
        difference.Add (dataContainer);
    }

    return difference;
  }

  public DataContainerCollection Merge (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    DataContainerCollection mergedCollection = new DataContainerCollection ();

    foreach (DataContainer dataContainer in this)
    {
      if (dataContainers.Contains (dataContainer.ID))
        mergedCollection.Add (dataContainers[dataContainer.ID]);
      else
        mergedCollection.Add (dataContainer);
    }

    return mergedCollection;
  }

  #region Standard implementation for collections

  public bool Contains (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    return BaseContains (dataContainer.ID, dataContainer);
  }

  public bool Contains (ObjectID id)
  {
    return BaseContainsKey (id);
  }

  public DataContainer this[int index]
  {
    get {return (DataContainer) BaseGetObject (index); }
  }

  public DataContainer this[ObjectID id]  
  {
    get { return (DataContainer) BaseGetObject (id); }
  }

  public int Add (DataContainer value)
  {
    ArgumentUtility.CheckNotNull ("value", value);
    
    return BaseAdd (value.ID, value);
  }

  public void Remove (int index)
  {
    Remove (this[index]);
  }

  public void Remove (ObjectID id)
  {
    Remove (this[id]);
  }

  public void Remove (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    BaseRemove (dataContainer.ID);
  }

  public void Clear ()
  {
    BaseClear ();
  }

  #endregion
}
}
