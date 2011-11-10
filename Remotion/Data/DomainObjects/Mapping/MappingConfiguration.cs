// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  public class MappingConfiguration
  {
    #region Obsolete

    [Obsolete ("Use MappingConfiguration.GetClassDefinition (...) or MappingConfiguration.GetTypeDefinition (...) instead. (Version 1.13.105)", true)]
    public CommonCollection ClassDefinitions
    {
      get { throw new NotSupportedException ("Use MappingConfiguration.GetClassDefinition (...) or MappingConfiguration.GetTypeDefinition (...) instead."); }
    }

    [Obsolete ("This method is no longer supported. Access the RelationDefinition by first getting the ClassDefinition, retrieving the RelationDefintionEndPoint and finally the RelationDefintion. (Version 1.13.105)", true)]
    public CommonCollection RelationDefinitions
    {
      get { throw new NotSupportedException ("This method is no longer supported. Access the RelationDefinition by first getting the ClassDefinition, retrieving the RelationDefintionEndPoint and finally the RelationDefintion."); }
    }

    #endregion

    private static readonly ILog s_log = LogManager.GetLogger (typeof (MappingConfiguration));

    private static readonly DoubleCheckedLockingContainer<MappingConfiguration> s_mappingConfiguration =
        new DoubleCheckedLockingContainer<MappingConfiguration> (
            () =>
            new MappingConfiguration (
                DomainObjectsConfiguration.Current.MappingLoader.CreateMappingLoader(),
                new PersistenceModelLoader(new StorageGroupBasedStorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage))));

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

    private readonly ReadOnlyDictionary<Type, ClassDefinition> _typeDefinitions;
    private readonly ReadOnlyDictionary<string, ClassDefinition> _classDefinitions;
    private readonly ReadOnlyDictionary<string, RelationDefinition> _relationDefinitions;
    private readonly bool _resolveTypes;
    private readonly IMappingNameResolver _nameResolver;

    // construction and disposing

    public MappingConfiguration (IMappingLoader mappingLoader, IPersistenceModelLoader persistenceModelLoader)
    {
      ArgumentUtility.CheckNotNull ("mappingLoader", mappingLoader);
      ArgumentUtility.CheckNotNull ("persistenceModelLoader", persistenceModelLoader);

      s_log.Info ("Building mapping configuration...");
      
      using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to build and validate mapping configuration: {elapsed}."))
      {
        var typeDefinitions = mappingLoader.GetClassDefinitions();
        _typeDefinitions = new ReadOnlyDictionary<Type, ClassDefinition> (typeDefinitions.ToDictionary (td => td.ClassType));
        ValidateDuplicateClassIDs (typeDefinitions.OfType<ClassDefinition>());
        _classDefinitions = new ReadOnlyDictionary<string, ClassDefinition> (typeDefinitions.ToDictionary (cd => cd.ID));
        
        ValidateClassDefinitions (mappingLoader);
        ValidatePropertyDefinitions (mappingLoader);

        var relationDefinitions = mappingLoader.GetRelationDefinitions (_typeDefinitions);
        _relationDefinitions = new ReadOnlyDictionary<string, RelationDefinition> (relationDefinitions.ToDictionary (rd => rd.ID));

        ValidateRelationDefinitions (mappingLoader);

        foreach (var rootClass in GetInheritanceRootClasses (_typeDefinitions.Values))
        {
          persistenceModelLoader.ApplyPersistenceModelToHierarchy (rootClass);
          VerifyPersistenceModelApplied (rootClass);

          var validator = persistenceModelLoader.CreatePersistenceMappingValidator (rootClass);
          ValidatePersistenceMapping (validator, rootClass);
        }

        _resolveTypes = mappingLoader.ResolveTypes;
        _nameResolver = mappingLoader.NameResolver;

        SetMappingReadOnly ();

        ValidateSortExpression(mappingLoader);
      }
    }

    /// <summary>
    /// Gets a flag whether type names in the configuration file should be resolved to their corresponding .NET <see cref="Type"/>.
    /// </summary>
    public bool ResolveTypes
    {
      get { return _resolveTypes; }
    }

    public ClassDefinition[] GetTypeDefinitions ()
    {
      return _typeDefinitions.Values.ToArray();
    }

    public bool ContainsTypeDefinition (Type classType)
    {
      ArgumentUtility.CheckNotNull ("classType", classType);

      return _typeDefinitions.ContainsKey (classType);
    }

    public ClassDefinition GetTypeDefinition (Type classType)
    {
      ArgumentUtility.CheckNotNull ("classType", classType);

      return GetTypeDefinition (classType, type => CreateMappingException ("Mapping does not contain class '{0}'.", type));
    }

    public ClassDefinition GetTypeDefinition (Type classType, Func<Type, Exception> missingTypeDefinitionExceptionFactory)
    {
      ArgumentUtility.CheckNotNull ("classType", classType);
      ArgumentUtility.CheckNotNull ("missingTypeDefinitionExceptionFactory", missingTypeDefinitionExceptionFactory);

      var classDefinition = _typeDefinitions.GetValueOrDefault (classType);
      if (classDefinition == null)
        throw missingTypeDefinitionExceptionFactory (classType);

      return classDefinition;
    }

    public bool ContainsClassDefinition (string classID)
    {
      ArgumentUtility.CheckNotNull ("classID", classID);

      return _classDefinitions.ContainsKey (classID);
    }

    public ClassDefinition GetClassDefinition (string classID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

      return GetClassDefinition (classID, id => CreateMappingException ("Mapping does not contain class '{0}'.", id));
    }

    public ClassDefinition GetClassDefinition (string classID, Func<string, Exception> missingClassDefinitionExceptionFactory)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);
      ArgumentUtility.CheckNotNull ("missingClassDefinitionExceptionFactory", missingClassDefinitionExceptionFactory);

      var classDefinition = _classDefinitions.GetValueOrDefault (classID);
      if (classDefinition == null)
        throw missingClassDefinitionExceptionFactory (classID);

      return classDefinition;
    }

    public IMappingNameResolver NameResolver
    {
      get { return _nameResolver; }
    }

    private IEnumerable<ClassDefinition> GetInheritanceRootClasses (IEnumerable<ClassDefinition> classDefinitions)
    {
      var rootClasses = new Set<ClassDefinition> ();
      foreach (var classDefinition in classDefinitions)
      {
        var rootClassDefinition = classDefinition.GetInheritanceRootClass ();
        if (!rootClasses.Contains (rootClassDefinition))
          rootClasses.Add (rootClassDefinition);
      }

      return rootClasses;
    }

    private void VerifyPersistenceModelApplied (ClassDefinition classDefinition)
    {
      if (classDefinition.StorageEntityDefinition == null)
      {
        var message = string.Format ("The persistence model loader did not assign a storage entity to class '{0}'.", classDefinition.ID);
        throw new InvalidOperationException (message);
      }

      foreach (PropertyDefinition propDef in classDefinition.MyPropertyDefinitions)
      {
        if (propDef.StorageClass == StorageClass.Persistent && propDef.StoragePropertyDefinition == null)
        {
          var message = string.Format (
              "The persistence model loader did not assign a storage property to property '{0}' of class '{1}'.",
              propDef.PropertyName,
              classDefinition.ID);
          throw new InvalidOperationException (message);
        }
      }

      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
        VerifyPersistenceModelApplied (derivedClass);
    }

    private void SetMappingReadOnly ()
    {
      foreach (var classDefinition in _typeDefinitions.Values)
        classDefinition.SetReadOnly();
    }

    private void ValidateClassDefinitions (IMappingLoader mappingLoader)
    {
      var typeDefinitionValidator = mappingLoader.CreateClassDefinitionValidator();
      AnalyzeMappingValidationResults (typeDefinitionValidator.Validate (_typeDefinitions.Values));
    }

    private void ValidatePropertyDefinitions (IMappingLoader mappingLoader)
    {
      var propertyDefinitionValidator = mappingLoader.CreatePropertyDefinitionValidator();
      AnalyzeMappingValidationResults (propertyDefinitionValidator.Validate (_typeDefinitions.Values));
    }

    private void ValidateRelationDefinitions (IMappingLoader mappingLoader)
    {
      var relationDefinitionValidator = mappingLoader.CreateRelationDefinitionValidator();
      AnalyzeMappingValidationResults (relationDefinitionValidator.Validate (_relationDefinitions.Values));
    }

    private void ValidateSortExpression (IMappingLoader mappingLoader)
    {
      var sortExpressionValidator = mappingLoader.CreateSortExpressionValidator();
      AnalyzeMappingValidationResults (sortExpressionValidator.Validate (_relationDefinitions.Values));
    }

    private void ValidatePersistenceMapping (IPersistenceMappingValidator validator, ClassDefinition rootClass)
    {
      var classDefinitionsToValidate = new[] { rootClass }.Concat (rootClass.GetAllDerivedClasses());
      AnalyzeMappingValidationResults (validator.Validate (classDefinitionsToValidate));
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
      foreach (var validationResult in mappingValidationResults)
      {
        if (messages.Length > 0)
          messages.AppendLine (new string ('-', 10));
        messages.AppendLine (validationResult.Message);
      }

      return CreateMappingException (messages.ToString().Trim());
    }

    private void ValidateDuplicateClassIDs (IEnumerable<ClassDefinition> classDefinitions)
    {
      var duplicateGroups = from cd in classDefinitions
                            group cd by cd.ID
                              into cdGroups
                              where cdGroups.Count () > 1
                              select cdGroups;
      foreach (var duplicateGroup in duplicateGroups)
      {
        var duplicates = duplicateGroup.ToArray ();
        throw CreateMappingException (
            "Class '{0}' and '{1}' both have the same class ID '{2}'. Use the ClassIDAttribute to define unique IDs for these "
            + "classes. The assemblies involved are '{3}' and '{4}'.",
            duplicates[0].ClassType.FullName,
            duplicates[1].ClassType.FullName,
            duplicates[0].ID,
            duplicates[0].ClassType.Assembly.FullName,
            duplicates[1].ClassType.Assembly.FullName);
      }
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }
  }
}