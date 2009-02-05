// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  public class CollectionEndPointRemoveModification : RelationEndPointModification
  {
    private readonly CollectionEndPoint _collectionEndPoint;
    private readonly IDomainObjectCollectionData _modifiedCollectionData;
    private readonly DomainObject _removedObject;
    private readonly DomainObjectCollection _modifiedCollection;

    public CollectionEndPointRemoveModification (CollectionEndPoint collectionEndPoint, IEndPoint endPointOfRemovedObject, IDomainObjectCollectionData collectionData)
        : base (ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint),
                ArgumentUtility.CheckNotNull ("endPointOfRemovedObject", endPointOfRemovedObject),
                new NullObjectEndPoint(endPointOfRemovedObject.Definition))
    {
      _collectionEndPoint = collectionEndPoint;
      _modifiedCollectionData = collectionData;
      _removedObject = OldEndPoint.GetDomainObject ();
      _modifiedCollection = _collectionEndPoint.OppositeDomainObjects;
    }

    public DomainObject RemovedObject
    {
      get { return _removedObject; }
    }

    public DomainObjectCollection ModifiedCollection
    {
      get { return _modifiedCollection; }
    }

    public IDomainObjectCollectionData ModifiedCollectionData
    {
      get { return _modifiedCollectionData; }
    }

    public override void Begin ()
    {
      ModifiedCollection.BeginRemove (RemovedObject);
      base.Begin ();
    }

    public override void Perform ()
    {
      ModifiedCollectionData.Remove (RemovedObject.ID);
    }

    public override void End ()
    {
      ModifiedCollection.EndRemove (RemovedObject);
      base.End ();
    }
  }
}