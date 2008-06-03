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
using System.Collections.Generic;
using System.Data;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class ConcreteTableInheritanceRelationLoader
  {
    // types

    // static members and constants

    // member fields

    private readonly RdbmsProvider _provider;
    private readonly ClassDefinition _classDefinition;
    private readonly PropertyDefinition _propertyDefinition;
    private readonly ObjectID _relatedID;
    private readonly DataContainerLoader _dataContainerLoader;
    private readonly ObjectIDLoader _objectIDLoader;

    // construction and disposing

    public ConcreteTableInheritanceRelationLoader (
        RdbmsProvider provider,
        ClassDefinition classDefinition,
        PropertyDefinition propertyDefinition,
        ObjectID relatedID)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("relatedID", relatedID);

      _provider = provider;
      _classDefinition = classDefinition;
      _propertyDefinition = propertyDefinition;
      _relatedID = relatedID;
      _dataContainerLoader = new DataContainerLoader (_provider);
      _objectIDLoader = new ObjectIDLoader (_provider);
    }

    // methods and properties

    public RdbmsProvider Provider
    {
      get { return _provider; }
    }

    public DataContainerCollection LoadDataContainers ()
    {
      List<ObjectID> objectIDsInCorrectOrder = GetObjectIDsInCorrectOrder ();
      if (objectIDsInCorrectOrder.Count == 0)
        return new DataContainerCollection ();

      return _dataContainerLoader.LoadDataContainersFromIDs (objectIDsInCorrectOrder);
    }

    private List<ObjectID> GetObjectIDsInCorrectOrder ()
    {
      UnionSelectCommandBuilder builder = UnionSelectCommandBuilder.CreateForRelatedIDLookup (
          _provider, _classDefinition, _propertyDefinition, _relatedID);

      return _objectIDLoader.LoadObjectIDsFromCommandBuilder (builder);
    }
  }
}
