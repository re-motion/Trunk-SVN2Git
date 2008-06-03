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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
public class DataContainerFactory
{
  private readonly IDataReader _dataReader;
  private readonly RdbmsProvider _provider;

  public DataContainerFactory (RdbmsProvider provider, IDataReader dataReader)
  {
    ArgumentUtility.CheckNotNull ("provider", provider);
    ArgumentUtility.CheckNotNull ("dataReader", dataReader);

    _dataReader = dataReader;
    _provider = provider;
  }

  public virtual DataContainer CreateDataContainer ()
  {
    if (_dataReader.Read ())
      return CreateDataContainerFromReader ();
    else
      return null;
  }

  public virtual DataContainerCollection CreateCollection ()
  {
    DataContainerCollection dataContainerCollection = new DataContainerCollection ();

    while (_dataReader.Read ())
    {
      DataContainer dataContainer = CreateDataContainerFromReader ();
      if (dataContainerCollection.Contains (dataContainer.ID))
        throw _provider.CreateRdbmsProviderException (
            "A database query returned duplicates of the domain object '{0}', which is not supported.",
            dataContainer.ID);
      dataContainerCollection.Add (dataContainer);
    }

    return dataContainerCollection;
  }

  protected virtual DataContainer CreateDataContainerFromReader ()
  {
    ValueConverter valueConverter = _provider.CreateValueConverter ();
    
    ObjectID id = valueConverter.GetID (_dataReader);
    if (id == null)
      throw _provider.CreateRdbmsProviderException ("An object returned from the database had a NULL ID, which is not supported.");

    object timestamp = _dataReader.GetValue (valueConverter.GetMandatoryOrdinal (_dataReader, "Timestamp"));

    DataContainer dataContainer = DataContainer.CreateForExisting (id, timestamp);

    foreach (PropertyDefinition propertyDefinition in id.ClassDefinition.GetPropertyDefinitions ())
    {
      object dataValue;

      try
      {
        dataValue = valueConverter.GetValue (id.ClassDefinition, propertyDefinition, _dataReader);
      }
      catch (Exception e)
      {
        throw _provider.CreateRdbmsProviderException (e, "Error while reading property '{0}' of object '{1}': {2}", 
            propertyDefinition.PropertyName, id, e.Message);
      }

      dataContainer.PropertyValues.Add (new PropertyValue (propertyDefinition, dataValue));
    }

    return dataContainer;
  }
}
}
