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
using System.Data;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
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
    private readonly IStorageNameProvider _storageNameProvider;

    public OppositeClassDefinitionRetriever (RdbmsProvider provider, ClassDefinition classDefinition, PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("provider", provider);

      _provider = provider;
      _classDefinition = classDefinition;
      _propertyDefinition = propertyDefinition;
      _relatedClassDefinition = _classDefinition.GetMandatoryOppositeClassDefinition (_propertyDefinition.PropertyName);
      _storageNameProvider = new ReflectionBasedStorageNameProvider(); // TODO: Inject via ctor
    }

    public ClassDefinition GetMandatoryOppositeClassDefinition (IDataReader dataReader, int objectIDColumnOrdinal)
    {
      var classIDColumnName = _storageNameProvider.GetRelationClassIDColumnName (_propertyDefinition);
      var sourceStorageProviderDefinition = _classDefinition.StorageEntityDefinition.StorageProviderDefinition;
      var relatedStorageProviderDefinition = _relatedClassDefinition.StorageEntityDefinition.StorageProviderDefinition;
      if (_relatedClassDefinition.IsPartOfInheritanceHierarchy && sourceStorageProviderDefinition == relatedStorageProviderDefinition)
        return GetOppositeClassDefinitionInInheritanceHierarchy (dataReader, objectIDColumnOrdinal, classIDColumnName);
      else
      {
        CheckNoOppositeClassID (dataReader, classIDColumnName);
        return _relatedClassDefinition;
      }
    }

    private ClassDefinition GetOppositeClassDefinitionInInheritanceHierarchy (IDataReader dataReader, int objectIDColumnOrdinal, string classIDColumnName)
    {
      int classIDColumnOrdinal = TryGetClassIDColumnOrdinal (dataReader, classIDColumnName);
      CheckConsistentIDs (dataReader, objectIDColumnOrdinal, classIDColumnName, classIDColumnOrdinal);

      if (dataReader.IsDBNull (classIDColumnOrdinal))
        return _relatedClassDefinition;
      else
        return MappingConfiguration.Current.GetClassDefinition (dataReader.GetString (classIDColumnOrdinal));
    }

    private int TryGetClassIDColumnOrdinal (IDataReader dataReader, string classIDColumnName)
    {
      int classIDColumnOrdinal;
      bool hasClassIDColumn = TryGetClassIDColumnOrdinal (dataReader, classIDColumnName, out classIDColumnOrdinal);

      if (!hasClassIDColumn)
      {
        throw _provider.CreateRdbmsProviderException (
            "Incorrect database format encountered."
            + " Entity '{0}' must have column '{1}' defined, because opposite class '{2}' is part of an inheritance hierarchy.",
            _classDefinition.GetEntityName(),
            classIDColumnName,
            _relatedClassDefinition.ID);
      }

      return classIDColumnOrdinal;
    }

    private void CheckConsistentIDs (IDataReader dataReader, int objectIDColumnOrdinal, string classIDColumnName, int classIDColumnOrdinal)
    {
      if (dataReader.IsDBNull (objectIDColumnOrdinal) && !dataReader.IsDBNull (classIDColumnOrdinal))
      {
        throw _provider.CreateRdbmsProviderException (
            "Incorrect database value encountered. Column '{0}' of entity '{1}' must not contain a value.",
            classIDColumnName,
            _classDefinition.GetEntityName());
      }

      if (!dataReader.IsDBNull (objectIDColumnOrdinal) && dataReader.IsDBNull (classIDColumnOrdinal))
      {
        throw _provider.CreateRdbmsProviderException (
            "Incorrect database value encountered. Column '{0}' of entity '{1}' must not contain null.",
            classIDColumnName,
            _classDefinition.GetEntityName());
      }
    }

    private void CheckNoOppositeClassID (IDataReader dataReader, string classIDColumnName)
    {
      // Note: We cannot ask an IDataReader if a specific column exists without an exception thrown by IDataReader.
      // Because throwing and catching exceptions is a very time consuming operation the result is cached per entity
      // and relation and is reused in subsequent calls.

      bool hasClassIDColumn =
          s_hasClassIDColumnCache.GetOrCreateValue (
              Tuple.Create (_classDefinition, _propertyDefinition),
              delegate
              {
                int classIDColumnOrdinal;
                return TryGetClassIDColumnOrdinal (dataReader, classIDColumnName, out classIDColumnOrdinal);
              });

      if (hasClassIDColumn)
      {
        throw _provider.CreateRdbmsProviderException (
            "Incorrect database format encountered."
            + " Entity '{0}' must not contain column '{1}', because opposite class '{2}' is not part of an inheritance hierarchy.",
            _classDefinition.GetEntityName(),
            classIDColumnName,
            _relatedClassDefinition.ID);
      }
    }

    private bool TryGetClassIDColumnOrdinal (IDataReader dataReader, string classIDColumnName, out int classIDColumnOrdinal)
    {
      //Note: Despite the IDataReaders documentation, some implementations of IDataReader.GetOrdinal return -1 instead of throwing an IndexOutOfRangeException
      try
      {
        classIDColumnOrdinal = dataReader.GetOrdinal (classIDColumnName);
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