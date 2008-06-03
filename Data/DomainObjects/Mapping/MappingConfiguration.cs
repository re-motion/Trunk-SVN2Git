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
using System.Collections;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  public class MappingConfiguration
  {
    // types

    // static members and constants


    private static MappingConfiguration s_mappingConfiguration;

    public static MappingConfiguration Current
    {
      get
      {
        if (s_mappingConfiguration == null)
        {
          lock (typeof (MappingConfiguration))
          {
            if (s_mappingConfiguration == null)
              s_mappingConfiguration = new MappingConfiguration (DomainObjectsConfiguration.Current.MappingLoader.CreateMappingLoader());
          }
        }

        return s_mappingConfiguration;
      }
    }

    public static void SetCurrent (MappingConfiguration mappingConfiguration)
    {
      if (mappingConfiguration != null)
      {
        if (!mappingConfiguration.ResolveTypes)
          throw CreateArgumentException ("mappingConfiguration", "Argument 'mappingConfiguration' must have property 'ResolveTypes' set.");

        try
        {
          mappingConfiguration.Validate();
        }
        catch (Exception ex)
        {
          throw CreateArgumentException (
              ex, "mappingConfiguration", "The specified MappingConfiguration is invalid due to the following reason: '{0}'.", ex.Message);
        }
      }

      lock (typeof (MappingConfiguration))
      {
        s_mappingConfiguration = mappingConfiguration;
      }
    }

    private static ArgumentException CreateArgumentException (Exception innerException, string argumentName, string message, params object[] args)
    {
      return new ArgumentException (string.Format (message, args), argumentName, innerException);
    }

    private static ArgumentException CreateArgumentException (string argumentName, string message, params object[] args)
    {
      return CreateArgumentException (null, argumentName, message, args);
    }

    // member fields

    private ClassDefinitionCollection _classDefinitions;
    private RelationDefinitionCollection _relationDefinitions;
    private bool _resolveTypes;
    
    // construction and disposing

    public MappingConfiguration (IMappingLoader loader)
    {
      ArgumentUtility.CheckNotNull ("loader", loader);

      _classDefinitions = loader.GetClassDefinitions();
      if (_classDefinitions == null)
        throw new InvalidOperationException (string.Format ("IMappingLoader.GetClassDefinitions() evaluated and returned null."));

      _relationDefinitions = loader.GetRelationDefinitions (_classDefinitions);
      if (_relationDefinitions == null)
        throw new InvalidOperationException (string.Format ("IMappingLoader.GetRelationDefinitions (ClassDefinitionCollection) evaluated and returned null."));
      
      _resolveTypes = loader.ResolveTypes;
      
      Validate();
    }

    // methods and properties

    public void Validate()
    {
      _classDefinitions.Validate();
    }

    /// <summary>
    /// Gets a flag whether type names in the configuration file should be resolved to their corresponding .NET <see cref="Type"/>.
    /// </summary>
    public bool ResolveTypes
    {
      get { return _resolveTypes; }
    }

    public ClassDefinitionCollection ClassDefinitions
    {
      get { return _classDefinitions; }
    }

    public RelationDefinitionCollection RelationDefinitions
    {
      get { return _relationDefinitions; }
    }

    public bool Contains (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      return _classDefinitions.Contains (classDefinition);
    }

    public bool Contains (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      if (propertyDefinition.ClassDefinition == null)
        return false;

      ClassDefinition foundClassDefinition = _classDefinitions[propertyDefinition.ClassDefinition.ID];
      if (foundClassDefinition == null)
        return false;

      return foundClassDefinition.Contains (propertyDefinition);
    }

    public bool Contains (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      return _relationDefinitions.Contains (relationDefinition);
    }

    public bool Contains (IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      if (relationEndPointDefinition.RelationDefinition == null)
        return false;

      RelationDefinition foundRelationDefinition = _relationDefinitions[relationEndPointDefinition.RelationDefinition.ID];
      if (foundRelationDefinition == null)
        return false;

      return ((IList) foundRelationDefinition.EndPointDefinitions).Contains (relationEndPointDefinition);
    }
  }
}

