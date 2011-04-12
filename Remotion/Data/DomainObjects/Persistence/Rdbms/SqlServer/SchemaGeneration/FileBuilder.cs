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

// TODO Review 3857: Move to Rdbms\SchemaGeneration namespace

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// Generates setup files for a list of mapped classes.
  /// </summary>
  public class FileBuilder : IFileBuilder
  {
    public static void Build (
        ICollection<ClassDefinition> classDefinitions,
        ICollection<StorageProviderDefinition> storageProviders,
        string outputPath,
        Func<RdbmsProviderDefinition, IFileBuilder> fileBuilderFactory)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNull ("storageProviders", storageProviders);
      ArgumentUtility.CheckNotNull ("outputPath", outputPath);

      if (outputPath != String.Empty && !Directory.Exists (outputPath))
        Directory.CreateDirectory (outputPath);

      bool createMultipleFiles = storageProviders.Count > 1;
      foreach (var storageProviderDefinition in storageProviders)
      {
        var rdbmsProviderDefinition = storageProviderDefinition as RdbmsProviderDefinition;
        if (rdbmsProviderDefinition != null)
        {
          var fileBuilder = fileBuilderFactory (rdbmsProviderDefinition);
          var fileName = GetFileName (rdbmsProviderDefinition, outputPath, createMultipleFiles);
          fileBuilder.Build (classDefinitions, fileName);
        }
      }
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

    private readonly ScriptBuilderBase _scriptBuilder;

    public FileBuilder (ScriptBuilderBase scriptBuilder)
    {
      ArgumentUtility.CheckNotNull ("scriptBuilder", scriptBuilder);

      _scriptBuilder = scriptBuilder;
    }

    public void Build (IEnumerable<ClassDefinition> classDefinitions, string fileName)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);

      var script = GetScript (classDefinitions);
      File.WriteAllText (fileName, script);
    }

    public virtual string GetScript (IEnumerable<ClassDefinition> classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      var classDefinitionsForStorageProvider = GetClassesInStorageProvider (classDefinitions, _scriptBuilder.RdbmsProviderDefinition);

      var entityDefintions = GetEntityDefinitions (classDefinitionsForStorageProvider);
      return _scriptBuilder.GetScript (entityDefintions);
    }

    protected virtual IEnumerable<ClassDefinition> GetClassesInStorageProvider (
        IEnumerable<ClassDefinition> classDefinitions,
        RdbmsProviderDefinition rdbmsProviderDefinition)
    {
      return classDefinitions.Where (currentClass => currentClass.StorageEntityDefinition.StorageProviderDefinition == rdbmsProviderDefinition);
    }

    protected virtual IEnumerable<IEntityDefinition> GetEntityDefinitions (IEnumerable<ClassDefinition> classDefinitions)
    {
      return classDefinitions
          .Select (cd => cd.StorageEntityDefinition)
          .OfType<IEntityDefinition>();
    }
  }
}