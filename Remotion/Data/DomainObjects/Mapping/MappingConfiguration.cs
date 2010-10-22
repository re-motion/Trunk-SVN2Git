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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Utilities;
using Remotion.Logging;
using System.Linq;

namespace Remotion.Data.DomainObjects.Mapping
{
  public class MappingConfiguration
  {
    // types

    // static members and constants

    private static readonly ILog s_log = LogManager.GetLogger (typeof (MappingConfiguration));

    private static readonly DoubleCheckedLockingContainer<MappingConfiguration> s_mappingConfiguration =
        new DoubleCheckedLockingContainer<MappingConfiguration> (
            () => new MappingConfiguration (DomainObjectsConfiguration.Current.MappingLoader.CreateMappingLoader ()));

    public static MappingConfiguration Current
    {
      get { return s_mappingConfiguration.Value; }
    }

    public static void SetCurrent (MappingConfiguration mappingConfiguration)
    {
      if (mappingConfiguration != null)
      {
        if (!mappingConfiguration.ResolveTypes)
          throw CreateArgumentException ("mappingConfiguration", "Argument 'mappingConfiguration' must have property 'ResolveTypes' set.");

        // TODO 3423
        //try
        //{
        //  mappingConfiguration.Validate();
        //}
        //catch (Exception ex)
        //{
        //  throw CreateArgumentException (
        //      ex, "mappingConfiguration", "The specified MappingConfiguration is invalid due to the following reason: '{0}'.", ex.Message);
        //}
      }

      s_mappingConfiguration.Value = mappingConfiguration;
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

      using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to build and validate mapping configuration: {elapsed}."))
      {
        _classDefinitions = loader.GetClassDefinitions();
        if (_classDefinitions == null)
          throw new InvalidOperationException (string.Format ("IMappingLoader.GetClassDefinitions() evaluated and returned null."));

        ValidateClassDefinitions ();
        ValidatePropertyDefinitions ();

        _relationDefinitions = loader.GetRelationDefinitions (_classDefinitions);
        if (_relationDefinitions == null)
          throw new InvalidOperationException (string.Format ("IMappingLoader.GetRelationDefinitions (ClassDefinitionCollection) evaluated and returned null."));

        ValidateRelationDefinitions ();
        ValidatePersistenceMapping ();

        _resolveTypes = loader.ResolveTypes;
        _nameResolver = loader.NameResolver;

        SetClassDefinitionsReadOnly ();
      }
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

      return foundRelationDefinition.Contains (relationEndPointDefinition);
    }

    private void SetClassDefinitionsReadOnly ()
    {
      foreach (ClassDefinition classDefinition in _classDefinitions)
        classDefinition.SetReadOnly ();
    }

    private void ValidateClassDefinitions ()
    {
      if (_classDefinitions.Count > 0)
      {
        var classDefinitionValidator = ClassDefinitionValidator.Create();
        var classDefinitionValidationResults = classDefinitionValidator.Validate (_classDefinitions.Cast<ClassDefinition>()).ToArray();
        if (classDefinitionValidationResults.Length > 0)
          throw CreateMappingException (classDefinitionValidationResults);
      }
    }

    private void ValidatePropertyDefinitions ()
    {
      if (_classDefinitions.Count > 0)
      {
        var propertyDefinitionValidator = ClassDefinitionValidator.Create ();
        var propertyDefinitionValidationResults = propertyDefinitionValidator.Validate (_classDefinitions.Cast<ClassDefinition> ()).ToArray ();
        if (propertyDefinitionValidationResults.Length > 0)
          throw CreateMappingException (propertyDefinitionValidationResults);
      }
    }

    private void ValidateRelationDefinitions ()
    {
      if (_relationDefinitions.Count > 0)
      {
        var relationDefinitionValidator = RelationDefinitionValidator.Create();
        var relationDefinitionValidationResults = relationDefinitionValidator.Validate (_relationDefinitions.Cast<RelationDefinition>()).ToArray();
        if (relationDefinitionValidationResults.Length > 0)
          throw CreateMappingException (relationDefinitionValidationResults);
      }
    }

    private void ValidatePersistenceMapping ()
    {
      if (_classDefinitions.Count > 0)
      {
        var persistenceMappingValidator = PersistenceMappingValidator.Create();
        var persistenceMappingValidationResults = persistenceMappingValidator.Validate (_classDefinitions.Cast<ClassDefinition>()).ToArray();
        if (persistenceMappingValidationResults.Length > 0)
          throw CreateMappingException (persistenceMappingValidationResults);
      }
    }

    private MappingException CreateMappingException (IEnumerable<MappingValidationResult> mappingValidationResults)
    {
      var messages = new StringBuilder();
      foreach (var validationResult in mappingValidationResults)
        messages.AppendLine (validationResult.Message);
      return new MappingException (messages.ToString().Trim());
    }
  }
}

