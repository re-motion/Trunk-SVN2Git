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
using System.IO;
using System.Linq;
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// Generates setup files for a list of mapped classes.
  /// </summary>
  public class FileBuilder : IFileBuilder
  {
    public struct ScriptPair
    {
      private readonly string _createScript;
      private readonly string _dropScript;

      public ScriptPair (string createScript, string dropScript)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("createScript", createScript);
        ArgumentUtility.CheckNotNullOrEmpty ("dropScript", dropScript);

        _createScript = createScript;
        _dropScript = dropScript;
      }

      public string CreateScript
      {
        get { return _createScript; }
      }

      public string DropScript
      {
        get { return _dropScript; }
      }
    }

    public static void Build (
        ICollection<ClassDefinition> classDefinitions,
        ICollection<StorageProviderDefinition> storageProviders,
        string outputPath,
        Func<RdbmsProviderDefinition, IFileBuilder> fileBuilderFactory)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNull ("storageProviders", storageProviders);
      ArgumentUtility.CheckNotNull ("outputPath", outputPath);

      #region FileBuilder
      if (outputPath != String.Empty && !Directory.Exists (outputPath))
        Directory.CreateDirectory (outputPath);
      #endregion

      #region ScriptGenerator
      bool createMultipleFiles = storageProviders.Count > 1;
      foreach (var storageProviderDefinition in storageProviders)
      {
        var rdbmsProviderDefinition = storageProviderDefinition as RdbmsProviderDefinition;
        if (rdbmsProviderDefinition != null)
        {
          var fileBuilder = fileBuilderFactory (rdbmsProviderDefinition);
          var setupDbFileName = GetFileName (rdbmsProviderDefinition, outputPath, createMultipleFiles, "SetupDB");
          var tearDownDbFileName = GetFileName (rdbmsProviderDefinition, outputPath, createMultipleFiles, "TearDownDB");

          fileBuilder.Build (GetClassesInStorageProvider (classDefinitions, rdbmsProviderDefinition), setupDbFileName, tearDownDbFileName);
        }
      }
      #endregion
    }

    #region FileBuilder
    public static string GetFileName (
        StorageProviderDefinition storageProviderDefinition, string outputPath, bool multipleStorageProviders, string fileNamePrefix)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("outputPath", outputPath);
      ArgumentUtility.CheckNotNullOrEmpty ("fileNamePrefix", fileNamePrefix);

      string fileName;
      if (multipleStorageProviders)
        fileName = String.Format ("{0}_{1}.sql", fileNamePrefix, storageProviderDefinition.Name);
      else
        fileName = fileNamePrefix+ ".sql";

      return Path.Combine (outputPath, fileName);
    }
    #endregion

    #region ScriptGenerator
    public static IEnumerable<ClassDefinition> GetClassesInStorageProvider (
        IEnumerable<ClassDefinition> classDefinitions,
        RdbmsProviderDefinition rdbmsProviderDefinition)
    {
      return classDefinitions.Where (currentClass => currentClass.StorageEntityDefinition.StorageProviderDefinition == rdbmsProviderDefinition);
    }

    private readonly Func<IScriptBuilder> _scriptBuilderFactory;
    private readonly IEntityDefinitionProvider _entityDefinitionProvider;

    public FileBuilder (Func<IScriptBuilder> scriptBuilderFactory, IEntityDefinitionProvider entityDefinitionProvider)
    {
      ArgumentUtility.CheckNotNull ("scriptBuilderFactory", scriptBuilderFactory);
      ArgumentUtility.CheckNotNull ("entityDefinitionProvider", entityDefinitionProvider);

      _scriptBuilderFactory = scriptBuilderFactory;
      _entityDefinitionProvider = entityDefinitionProvider;
    }
    #endregion


    #region FileBuilder
    public void Build (IEnumerable<ClassDefinition> classDefinitions, string setupDBFileName, string tearDownDBFileName)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNullOrEmpty ("setupDBFileName", setupDBFileName);
      ArgumentUtility.CheckNotNullOrEmpty ("tearDownDBFileName", tearDownDBFileName);

      var script = GetScript (classDefinitions);
      File.WriteAllText (setupDBFileName, script.CreateScript);
      File.WriteAllText (tearDownDBFileName, script.DropScript);
    }
    #endregion

    public virtual ScriptPair GetScript (IEnumerable<ClassDefinition> classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      #region ScriptGenerator
      // TODO Review 3932: Add a test showing that when GetScript is called twice, the _scriptBuilderFactory is also called twice.
      var scriptBuilder = _scriptBuilderFactory();

      var entityDefintions = _entityDefinitionProvider.GetEntityDefinitions (classDefinitions);
      foreach (var entityDefinition in entityDefintions)
        scriptBuilder.AddEntityDefinition (entityDefinition);
      #endregion

      #region ScriptToStringConverter
      var createScriptStatements = new List<ScriptStatement>();
      var dropScriptStatements = new List<ScriptStatement>();
      var createScriptCollection = scriptBuilder.GetCreateScript();
      var dropScriptCollection = scriptBuilder.GetDropScript();

      createScriptCollection.AppendToScript (createScriptStatements);
      dropScriptCollection.AppendToScript (dropScriptStatements);

      return new ScriptPair (
          createScriptStatements.Aggregate (new StringBuilder(), (sb, stmt) => sb.AppendLine (stmt.Statement)).ToString(),
          dropScriptStatements.Aggregate (new StringBuilder(), (sb, stmt) => sb.AppendLine (stmt.Statement)).ToString());

      #endregion
    }
    
  }
}