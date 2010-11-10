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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  public class MappingConfiguration
  {
    // types

    // static members and constants

    private static readonly ILog s_log = LogManager.GetLogger (typeof (MappingConfiguration));

    private static readonly DoubleCheckedLockingContainer<MappingConfiguration> s_mappingConfiguration =
        new DoubleCheckedLockingContainer<MappingConfiguration> (
            () => new MappingConfiguration (DomainObjectsConfiguration.Current.MappingLoader.CreateMappingLoader()));

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
        _resolveTypes = loader.ResolveTypes;
        _nameResolver = loader.NameResolver;

        _classDefinitions = new ClassDefinitionCollection (loader.GetClassDefinitions (), true, true);

        ValidateClassDefinitions();
        ValidatePropertyDefinitions();

        _relationDefinitions = new RelationDefinitionCollection (loader.GetRelationDefinitions (_classDefinitions), true);

        ValidateRelationDefinitions();

        // TODO: Peristence mapping:
        // foreach (root in classDefinitions.GetInheritanceRootClasses())
        //   root.StorageProviderDefinition.PersistenceMappingBuilder.SetPersistenceMapping (root);

        ValidatePersistenceMapping ();

        SetMappingReadOnly();

        ValidateSortExpression();
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

    private void SetMappingReadOnly ()
    {
      foreach (ClassDefinition classDefinition in _classDefinitions)
        classDefinition.SetReadOnly();
      _classDefinitions.SetReadOnly();
      _relationDefinitions.SetReadOnly();
    }

    private void ValidateClassDefinitions ()
    {
      if (_classDefinitions.Count > 0)
      {
        var classDefinitionValidator = ClassDefinitionValidator.Create();
        AnalyzeMappingValidationResults (classDefinitionValidator.Validate (_classDefinitions.Cast<ClassDefinition>()));
      }
    }

    private void ValidatePropertyDefinitions ()
    {
      if (_classDefinitions.Count > 0)
      {
        var propertyDefinitionValidator = PropertyDefinitionValidator.Create();
        AnalyzeMappingValidationResults (propertyDefinitionValidator.Validate (_classDefinitions.Cast<ClassDefinition>()));
      }
    }

    private void ValidateRelationDefinitions ()
    {
      if (_relationDefinitions.Count > 0)
      {
        var relationDefinitionValidator = RelationDefinitionValidator.Create();
        AnalyzeMappingValidationResults (relationDefinitionValidator.Validate (_relationDefinitions.Cast<RelationDefinition>()));
      }
    }

    private void ValidatePersistenceMapping ()
    {
      if (_classDefinitions.Count > 0)
      {
        var persistenceMappingValidator = PersistenceMappingValidator.Create();
        AnalyzeMappingValidationResults (persistenceMappingValidator.Validate (_classDefinitions.Cast<ClassDefinition>()));
      }
    }

    private void ValidateSortExpression ()
    {
      if (_relationDefinitions.Count > 0)
      {
        var sortExpressionValidator = SortExpressionValidator.Create();
        AnalyzeMappingValidationResults (sortExpressionValidator.Validate (_relationDefinitions.Cast<RelationDefinition>()));
      }
    }

    private void AnalyzeMappingValidationResults (IEnumerable<MappingValidationResult> mappingValidationResults)
    {
      var mappingValidationResultsArray = mappingValidationResults.ToArray();
      if (mappingValidationResultsArray.Any())
        throw CreateMappingException (mappingValidationResultsArray);
    }

    private MappingException CreateMappingException (IEnumerable<MappingValidationResult> mappingValidationResults)
    {
      var messages = new StringBuilder();
      foreach (var validationResult in mappingValidationResults){
        if (messages.Length > 0)
          messages.AppendLine (new string ('-', 10));
        messages.AppendLine (validationResult.Message);
      }

      return new MappingException (messages.ToString().Trim());
    }
  }
}