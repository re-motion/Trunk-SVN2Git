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

namespace Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement
{
  /// <summary>
  /// Decorates <see cref="IDomainObjectCollectionData"/> by raising events whenever the inner collection is modified.
  /// </summary>
  [Serializable]
  public class ObservableCollectionDataDecorator : ObservableCollectionDataDecoratorBase
  {
    public class DataChangeEventArgs : EventArgs
    {
      public readonly OperationKind Operation;
      public readonly DomainObject AffectedObject;
      public readonly int Index;

      public DataChangeEventArgs (OperationKind operation, DomainObject affectedObject, int index)
      {
        ArgumentUtility.CheckNotNull ("affectedObject", affectedObject);

        Operation = operation;
        AffectedObject = affectedObject;
        Index = index;
      }
    }

    public event EventHandler<DataChangeEventArgs> CollectionChanging;
    public event EventHandler<DataChangeEventArgs> CollectionChanged;

    public ObservableCollectionDataDecorator (IDomainObjectCollectionData wrappedData)
      : base (wrappedData)
    {
    }

    protected override void OnDataChanging (OperationKind operation, DomainObject affectedObject, int index)
    {
      var eventHandler = CollectionChanging;
      if (eventHandler != null)
        eventHandler (this, new DataChangeEventArgs (operation, affectedObject, index));
    }

    protected override void OnDataChanged (OperationKind operation, DomainObject affectedObject, int index)
    {
      var eventHandler = CollectionChanged;
      if (eventHandler != null)
        eventHandler (this, new DataChangeEventArgs (operation, affectedObject, index));
    }
  }
}
