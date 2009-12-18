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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  [Serializable]
  public class RelationEndPointCollection : CommonCollection
  {
    // types

    // static members and constants

    // member fields

    private readonly ClientTransaction _clientTransaction;

    // construction and disposing

    public RelationEndPointCollection (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      _clientTransaction = clientTransaction;
    }

    public RelationEndPointCollection (RelationEndPointCollection collection, bool makeCollectionReadOnly)
        : this (collection.ClientTransaction)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      foreach (RelationEndPoint endPoint in collection)
        Add (endPoint);

      SetIsReadOnly (makeCollectionReadOnly);
    }

    // methods and properties

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public void Combine (RelationEndPointCollection endPoints)
    {
      foreach (RelationEndPoint endPoint in endPoints)
      {
        if (!Contains (endPoint))
          Add (endPoint);
      }
    }

    #region Standard implementation for collections

    public bool Contains (RelationEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      return BaseContains (endPoint.ID, endPoint);
    }

    public bool Contains (RelationEndPointID id)
    {
      return BaseContainsKey (id);
    }

    public RelationEndPoint this [int index]
    {
      get { return (RelationEndPoint) BaseGetObject (index); }
    }

    public RelationEndPoint this [RelationEndPointID id]
    {
      get { return (RelationEndPoint) BaseGetObject (id); }
    }

    public int Add (RelationEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      return BaseAdd (endPoint.ID, endPoint);
    }

    public void Remove (int index)
    {
      Remove (this[index]);
    }

    public void Remove (RelationEndPointID id)
    {
      Remove (this[id]);
    }

    public void Remove (RelationEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      BaseRemove (endPoint.ID);
    }

    public void Clear ()
    {
      BaseClear();
    }

    #endregion
  }
}
