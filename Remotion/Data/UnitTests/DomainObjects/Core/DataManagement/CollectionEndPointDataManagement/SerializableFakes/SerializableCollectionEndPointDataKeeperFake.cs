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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement.SerializableFakes
{
  [Serializable]
  public class SerializableCollectionEndPointDataKeeperFake : ICollectionEndPointDataKeeper
  {
    public bool IsDataComplete
    {
      get { throw new NotImplementedException(); }
    }

    public IDomainObjectCollectionData CollectionData
    {
      get { throw new NotImplementedException(); }
    }

    public IDomainObjectCollectionData OriginalCollectionData
    {
      get { throw new NotImplementedException(); }
    }

    public void RegisterOriginalObject (DomainObject domainObject)
    {
      throw new NotImplementedException();
    }

    public void UnregisterOriginalObject (ObjectID objectID)
    {
      throw new NotImplementedException();
    }

    public bool HasDataChanged (ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy)
    {
      throw new NotImplementedException();
    }

    public void MarkDataComplete ()
    {
      throw new NotImplementedException();
    }

    public void MarkDataIncomplete ()
    {
      throw new NotImplementedException();
    }

    public void CommitOriginalContents ()
    {
      throw new NotImplementedException();
    }
  }
}