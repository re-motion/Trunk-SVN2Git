// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Utilities;
using Remotion.Logging;

namespace Remotion.Data.DomainObjects.Mapping
{
  public class MappingConfiguration
  {
    // types

    // static members and constants

    private static readonly ILog s_log = LogManager.GetLogger (typeof (MappingConfiguration));

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

    private readonly ClassDefinitionCollection _classDefinitions;
    private readonly RelationDefinitionCollection _relationDefinitions;
    private readonly bool _resolveTypes;
    private readonly IMappingNameResolver _nameResolver;

    // construction and disposing

    public MappingConfiguration (IMappingLoader loader)
    {
      ArgumentUtility.CheckNotNull ("loader", loader);

      s_log.Info ("Building mapping configuration...");

      using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to build and validate mapping configuration: {0}."))
      {
        _classDefinitions = loader.GetClassDefinitions();
        if (_classDefinitions == null)
          throw new InvalidOperationException (string.Format ("IMappingLoader.GetClassDefinitions() evaluated and returned null."));

        _relationDefinitions = loader.GetRelationDefinitions (_classDefinitions);
        if (_relationDefinitions == null)
          throw new InvalidOperationException (
              string.Format ("IMappingLoader.GetRelationDefinitions (ClassDefinitionCollection) evaluated and returned null."));

        _resolveTypes = loader.ResolveTypes;
        _nameResolver = loader.NameResolver;

        Validate();
      }
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

    public IMappingNameResolver NameResolver
    {
      get { return _nameResolver; }
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

