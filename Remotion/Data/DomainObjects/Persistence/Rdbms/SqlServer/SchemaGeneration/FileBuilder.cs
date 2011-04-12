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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class FileBuilder
  {
    private readonly ScriptBuilderBase _scriptBuilder;

    public static void Build (ClassDefinitionCollection classDefinitions, StorageConfiguration storageConfiguration, string outputPath)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNull ("storageConfiguration", storageConfiguration);
      ArgumentUtility.CheckNotNull ("outputPath", outputPath);

      if (outputPath != String.Empty && !Directory.Exists (outputPath))
        Directory.CreateDirectory (outputPath);

      bool createMultipleFiles = storageConfiguration.StorageProviderDefinitions.Count > 1;
      foreach (StorageProviderDefinition storageProviderDefinition in storageConfiguration.StorageProviderDefinitions)
      {
        var rdbmsProviderDefinition = storageProviderDefinition as RdbmsProviderDefinition;
        if (rdbmsProviderDefinition != null)
          Build (classDefinitions, rdbmsProviderDefinition, GetFileName (rdbmsProviderDefinition, outputPath, createMultipleFiles));
      }
    }

    public static void Build (ClassDefinitionCollection classDefinitions, RdbmsProviderDefinition rdbmsProviderDefinition, string fileName)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNull ("rdbmsProviderDefinition", rdbmsProviderDefinition);

      var classDefinitionsForStorageProvider = GetClassesInStorageProvider (classDefinitions, rdbmsProviderDefinition);
      var scriptBuilder = rdbmsProviderDefinition.Factory.CreateSchemaScriptBuilder(rdbmsProviderDefinition);
      var fileBuilder = new FileBuilder (scriptBuilder);
      var script = fileBuilder.GetScript (classDefinitionsForStorageProvider);
      File.WriteAllText (fileName, script);
    }

    public static ClassDefinitionCollection GetClassesInStorageProvider (
        ClassDefinitionCollection classDefinitions, RdbmsProviderDefinition rdbmsProviderDefinition)
    {
      var classes = new ClassDefinitionCollection (false);
      foreach (ClassDefinition currentClass in classDefinitions)
      {
        if (currentClass.StorageEntityDefinition.StorageProviderDefinition == rdbmsProviderDefinition)
          classes.Add (currentClass);
      }

      return classes;
    }

    public static string GetFileName (StorageProviderDefinition storageProviderDefinition, string outputPath, bool multipleStorageProviders)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("outputPath", outputPath);

      string fileName;
      if (multipleStorageProviders)
        fileName = String.Format ("SetupDB_{0}.sql", storageProviderDefinition.Name);
      else
        fileName = "SetupDB.sql";

      return Path.Combine (outputPath, fileName);
    }

    public FileBuilder (ScriptBuilderBase scriptBuilder)
    {
      ArgumentUtility.CheckNotNull ("scriptBuilder", scriptBuilder);

      _scriptBuilder = scriptBuilder;
    }

    public virtual string GetScript (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      CheckClassDefinitions (classDefinitions);

      var entityDefintions = GetEntityDefinitions (classDefinitions);
      return _scriptBuilder.GetScript (entityDefintions);
    }

    protected virtual IEnumerable<IEntityDefinition> GetEntityDefinitions(ClassDefinitionCollection classDefinitions)
    {
      return classDefinitions.Cast<ClassDefinition>()
          .Select (cd => cd.StorageEntityDefinition).Where (ed => ed is IEntityDefinition).Cast<IEntityDefinition>();
    }

    private void CheckClassDefinitions (ClassDefinitionCollection classDefinitions)
    {
      foreach (ClassDefinition classDefinition in classDefinitions)
      {
        if (classDefinition.StorageEntityDefinition.StorageProviderDefinition != _scriptBuilder.RdbmsProviderDefinition)
        {
          throw new ArgumentException (
              string.Format (
                  "Class '{0}' has storage provider '{1}' defined, but storage provider '{2}' is required.",
                  classDefinition.ID,
                  classDefinition.StorageEntityDefinition.StorageProviderDefinition.Name,
                  _scriptBuilder.RdbmsProviderDefinition.Name));
        }
      }
    }
  }
}