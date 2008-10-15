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
using System.Data;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class OppositeClassDefinitionRetriever
  {
    private static readonly InterlockedCache<Tuple<ClassDefinition, PropertyDefinition>, bool> s_hasClassIDColumnCache =
      new InterlockedCache<Tuple<ClassDefinition, PropertyDefinition>, bool> ();

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
      int classIDColumnOrdinal = GetClassIDColumnOrdinal (dataReader);
      CheckConsistentIDs (dataReader, objectIDColumnOrdinal, classIDColumnOrdinal);

      if (dataReader.IsDBNull (classIDColumnOrdinal))
        return _relatedClassDefinition;
      else
        return MappingConfiguration.Current.ClassDefinitions.GetMandatory (dataReader.GetString (classIDColumnOrdinal));
    }

    private int GetClassIDColumnOrdinal (IDataReader dataReader)
    {
      int classIDColumnOrdinal;
      try
      {
        classIDColumnOrdinal = dataReader.GetOrdinal (RdbmsProvider.GetClassIDColumnName (_propertyDefinition.StorageSpecificName));
      }
      catch (IndexOutOfRangeException)
      {
        throw _provider.CreateRdbmsProviderException (
            "Incorrect database format encountered."
                + " Entity '{0}' must have column '{1}' defined, because opposite class '{2}' is part of an inheritance hierarchy.",
            _classDefinition.GetEntityName (),
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
            _classDefinition.GetEntityName ());
      }

      if (!dataReader.IsDBNull (objectIDColumnOrdinal) && dataReader.IsDBNull (classIDColumnOrdinal))
      {
        throw _provider.CreateRdbmsProviderException (
            "Incorrect database value encountered. Column '{0}' of entity '{1}' must not contain null.",
            RdbmsProvider.GetClassIDColumnName (_propertyDefinition.StorageSpecificName),
            _classDefinition.GetEntityName ());
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
                try
                {
                  dataReader.GetOrdinal (RdbmsProvider.GetClassIDColumnName (_propertyDefinition.StorageSpecificName));
                  return true;
                }
                catch (IndexOutOfRangeException)
                {
                  return false;
                }
              });

      if (hasClassIDColumn)
      {
        throw _provider.CreateRdbmsProviderException (
            "Incorrect database format encountered. Entity '{0}' must not contain column '{1}', because opposite class '{2}' is not part of an inheritance hierarchy.",
            _classDefinition.GetEntityName (),
            RdbmsProvider.GetClassIDColumnName (_propertyDefinition.StorageSpecificName),
            _relatedClassDefinition.ID);
      }
    }
  }
}
