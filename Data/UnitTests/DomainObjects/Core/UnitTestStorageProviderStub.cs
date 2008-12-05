// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public class UnitTestStorageProviderStub : StorageProvider
  {
    // types

    // static members and constants

    // member fields

    private static int s_nextID = 0;

    // construction and disposing

    public UnitTestStorageProviderStub (UnitTestStorageProviderStubDefinition definition)
        : base (definition)
    {
    }

    public StorageProvider InnerProvider;

    // methods and properties

    public override DataContainer LoadDataContainer (ObjectID id)
    {
      if (InnerProvider != null)
        return InnerProvider.LoadDataContainer (id);
      else
      {
        DataContainer container = DataContainer.CreateForExisting (id, null, delegate (PropertyDefinition propertyDefinition)
        {
          if (propertyDefinition.PropertyName.EndsWith (".Name"))
            return "Max Sachbearbeiter";
          else
            return propertyDefinition.DefaultValue;
        });

        int idAsInt = (int) id.Value;
        if (s_nextID <= idAsInt)
          s_nextID = idAsInt + 1;
        return container;
      }
    }

    public override DataContainerCollection LoadDataContainers (IEnumerable<ObjectID> ids)
    {
      if (InnerProvider != null)
        return InnerProvider.LoadDataContainers (ids);
      else
        throw new NotImplementedException();
    }

    public override DataContainerCollection ExecuteCollectionQuery (IQuery query)
    {
      if (InnerProvider != null)
        return InnerProvider.ExecuteCollectionQuery (query);
      else
        return null;
    }

    public override object ExecuteScalarQuery (IQuery query)
    {
      if (InnerProvider != null)
        return InnerProvider.ExecuteScalarQuery (query);
      else
        return null;
    }

    public override void Save (DataContainerCollection dataContainers)
    {
      if (InnerProvider != null)
        InnerProvider.Save (dataContainers);
    }

    public override void SetTimestamp (DataContainerCollection dataContainers)
    {
      if (InnerProvider != null)
        InnerProvider.SetTimestamp (dataContainers);
    }

    public override DataContainerCollection LoadDataContainersByRelatedID (ClassDefinition classDefinition, string propertyName, ObjectID relatedID)
    {
      if (InnerProvider != null)
        return InnerProvider.LoadDataContainersByRelatedID (classDefinition, propertyName, relatedID);
      else
        return null;
    }

    public override void BeginTransaction ()
    {
      if (InnerProvider != null)
        InnerProvider.BeginTransaction();
    }

    public override void Commit ()
    {
      if (InnerProvider != null)
        InnerProvider.Commit();
    }

    public override void Rollback ()
    {
      if (InnerProvider != null)
        InnerProvider.Rollback();
    }

    public override ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      if (InnerProvider != null)
        return InnerProvider.CreateNewObjectID (classDefinition);
      else
        return new ObjectID (classDefinition, s_nextID++);
    }

    public new object GetFieldValue (DataContainer dataContainer, string propertyName, ValueAccess valueAccess)
    {
      return base.GetFieldValue (dataContainer, propertyName, valueAccess);
    }
  }
}
