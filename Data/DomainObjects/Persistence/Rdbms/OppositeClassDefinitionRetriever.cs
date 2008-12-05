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
using System.Data;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class OppositeClassDefinitionRetriever
  {
    private static readonly InterlockedCache<Tuple<ClassDefinition, PropertyDefinition>, bool> s_hasClassIDColumnCache =
        new InterlockedCache<Tuple<ClassDefinition, PropertyDefinition>, bool>();

    public static void ResetCache ()
    {
      s_hasClassIDColumnCache.Clear();
    }

    private readonly RdbmsProvider _provider;
    private readonly ClassDefinition _classDefinition;
    private readonly PropertyDefinition _propertyDefinition;
    private readonly ClassDefinition _relatedClassDefinition;

    public OppositeClassDefinitionRetriever (RdbmsProvider provider, ClassDefinition classDefinition, PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("provider", provider);

      _provider = provider;
      _classDefinition = classDefinition;
      _propertyDefinition = propertyDefinition;
      _relatedClassDefinition = _classDefinition.GetMandatoryOppositeClassDefinition (_propertyDefinition.PropertyName);
    }

    public ClassDefinition GetMandatoryOppositeClassDefinition (IDataReader dataReader, int objectIDColumnOrdinal)
    {
      if (_relatedClassDefinition.IsPartOfInheritanceHierarchy && _classDefinition.StorageProviderID == _relatedClassDefinition.StorageProviderID)
        return GetOppositeClassDefinitionInInheritanceHierarchy (dataReader, objectIDColumnOrdinal);
      else
      {
        CheckNoOppositeClassID (dataReader);
        return _relatedClassDefinition;
      }
    }

    private ClassDefinition GetOppositeClassDefinitionInInheritanceHierarchy (IDataReader dataReader, int objectIDColumnOrdinal)
    {
      int classIDColumnOrdinal = TryGetClassIDColumnOrdinal (dataReader);
      CheckConsistentIDs (dataReader, objectIDColumnOrdinal, classIDColumnOrdinal);

      if (dataReader.IsDBNull (classIDColumnOrdinal))
        return _relatedClassDefinition;
      else
        return MappingConfiguration.Current.ClassDefinitions.GetMandatory (dataReader.GetString (classIDColumnOrdinal));
    }

    private int TryGetClassIDColumnOrdinal (IDataReader dataReader)
    {
      int classIDColumnOrdinal;
      bool hasClassIDColumn = TryGetClassIDColumnOrdinal (dataReader, out classIDColumnOrdinal);

      if (!hasClassIDColumn)
      {
        throw _provider.CreateRdbmsProviderException (
            "Incorrect database format encountered."
            + " Entity '{0}' must have column '{1}' defined, because opposite class '{2}' is part of an inheritance hierarchy.",
            _classDefinition.GetEntityName(),
            RdbmsProvider.GetClassIDColumnName (_propertyDefinition.StorageSpecificName),
            _relatedClassDefinition.ID);
      }

      return classIDColumnOrdinal;
    }

    private void CheckConsistentIDs (IDataReader dataReader, int objectIDColumnOrdinal, int classIDColumnOrdinal)
    {
      if (dataReader.IsDBNull (objectIDColumnOrdinal) && !dataReader.IsDBNull (classIDColumnOrdinal))
      {
        throw _provider.CreateRdbmsProviderException (
            "Incorrect database value encountered. Column '{0}' of entity '{1}' must not contain a value.",
            RdbmsProvider.GetClassIDColumnName (_propertyDefinition.StorageSpecificName),
            _classDefinition.GetEntityName());
      }

      if (!dataReader.IsDBNull (objectIDColumnOrdinal) && dataReader.IsDBNull (classIDColumnOrdinal))
      {
        throw _provider.CreateRdbmsProviderException (
            "Incorrect database value encountered. Column '{0}' of entity '{1}' must not contain null.",
            RdbmsProvider.GetClassIDColumnName (_propertyDefinition.StorageSpecificName),
            _classDefinition.GetEntityName());
      }
    }

    private void CheckNoOppositeClassID (IDataReader dataReader)
    {
      // Note: We cannot ask an IDataReader if a specific column exists without an exception thrown by IDataReader.
      // Because throwing and catching exceptions is a very time consuming operation the result is cached per entity
      // and relation and is reused in subsequent calls.

      bool hasClassIDColumn =
          s_hasClassIDColumnCache.GetOrCreateValue (
              Tuple.NewTuple (_classDefinition, _propertyDefinition),
              delegate
              {
                int classIDColumnOrdinal;
                return TryGetClassIDColumnOrdinal (dataReader, out classIDColumnOrdinal);
              });

      if (hasClassIDColumn)
      {
        throw _provider.CreateRdbmsProviderException (
            "Incorrect database format encountered."
            + " Entity '{0}' must not contain column '{1}', because opposite class '{2}' is not part of an inheritance hierarchy.",
            _classDefinition.GetEntityName(),
            RdbmsProvider.GetClassIDColumnName (_propertyDefinition.StorageSpecificName),
            _relatedClassDefinition.ID);
      }
    }

    private bool TryGetClassIDColumnOrdinal (IDataReader dataReader, out int classIDColumnOrdinal)
    {
      //Note: Despite the IDataReaders documentation, some implementations of IDataReader.GetOrdinal return -1 instead of throwing an IndexOutOfRangeException
      try
      {
        classIDColumnOrdinal = dataReader.GetOrdinal (RdbmsProvider.GetClassIDColumnName (_propertyDefinition.StorageSpecificName));
        if (classIDColumnOrdinal == -1)
          return false;
        else
          return true;
      }
      catch (IndexOutOfRangeException)
      {
        classIDColumnOrdinal = -1;
        return false;
      }
    }
  }
}
