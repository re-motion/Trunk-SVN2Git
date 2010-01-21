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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  public partial class DomainObjectCollection
  {
    /// <summary>
    /// Implements <see cref="IDomainObjectCollectionTransformer"/> for <see cref="DomainObjectCollection"/>, thus allowing 
    /// <see cref="CollectionEndPointReplaceWholeCollectionCommand"/> to convert between stand-alone and associated collections.
    /// </summary>
    private class Transformer : IDomainObjectCollectionTransformer
    {
      public Transformer (DomainObjectCollection owningCollection)
      {
        Collection = owningCollection;
      }

      public DomainObjectCollection Collection { get; private set; }

      public void TransformToAssociated (ICollectionEndPoint endPoint)
      {
        var endPointDelegatingCollectionData = endPoint.CreateDelegatingCollectionData ();
        Assertion.IsTrue (endPointDelegatingCollectionData.RequiredItemType == Collection.RequiredItemType);

        Collection._dataStrategy = endPointDelegatingCollectionData;
      }

      public void TransformToStandAlone ()
      {
        Assertion.IsNotNull (Collection.AssociatedEndPoint);

        var standAloneDataStore = new DomainObjectCollectionData (Collection._dataStrategy.GetUndecoratedDataStore ()); // copy data
        Collection._dataStrategy = CreateDataStrategyForStandAloneCollection (standAloneDataStore, Collection.RequiredItemType, Collection);
      }
    }
  }
}