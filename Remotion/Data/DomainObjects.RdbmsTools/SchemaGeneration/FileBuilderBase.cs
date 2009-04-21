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
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using System.IO;

namespace Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration
{
  public abstract class FileBuilderBase
  {
    // types

    // static members and constants

    public static void Build (
        Type fileBuilderType,
        MappingConfiguration mappingConfiguration,
        StorageConfiguration storageConfiguration, 
        string outputPath)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("fileBuilderType", fileBuilderType, typeof (FileBuilderBase));
      ArgumentUtility.CheckNotNull ("mappingConfiguration", mappingConfiguration);
      ArgumentUtility.CheckNotNull ("persistenceConfiguration", storageConfiguration);
      ArgumentUtility.CheckNotNull ("outputPath", outputPath);

      if (outputPath != string.Empty && !Directory.Exists (outputPath))
        Directory.CreateDirectory (outputPath);

      bool createMultipleFiles = storageConfiguration.StorageProviderDefinitions.Count > 1;
      foreach (StorageProviderDefinition storageProviderDefinition in storageConfiguration.StorageProviderDefinitions)
      {
        RdbmsProviderDefinition rdbmsProviderDefinition = storageProviderDefinition as RdbmsProviderDefinition;
        if (rdbmsProviderDefinition != null)
          Build (fileBuilderType, mappingConfiguration, rdbmsProviderDefinition, GetFileName (rdbmsProviderDefinition, outputPath, createMultipleFiles));
      }
    }

    public static void Build (
        Type fileBuilderType,
        MappingConfiguration mappingConfiguration, 
        RdbmsProviderDefinition rdbmsProviderDefinition, 
        string fileName)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("fileBuilderType", fileBuilderType, typeof (FileBuilderBase));
      ArgumentUtility.CheckNotNull ("mappingConfiguration", mappingConfiguration);
      ArgumentUtility.CheckNotNull ("rdbmsProviderDefinition", rdbmsProviderDefinition);

      FileBuilderBase fileBuilder = 
          (FileBuilderBase) TypesafeActivator.CreateInstance (fileBuilderType).With (mappingConfiguration, rdbmsProviderDefinition);

      File.WriteAllText (fileName, fileBuilder.GetScript ());
    }

    public static string GetFileName (StorageProviderDefinition storageProviderDefinition, string outputPath, bool multipleStorageProviders)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("outputPath", outputPath);

      string fileName;
      if (multipleStorageProviders)
        fileName = string.Format ("SetupDB_{0}.sql", storageProviderDefinition.Name);
      else
        fileName = "SetupDB.sql";

      return Path.Combine (outputPath,  fileName);
    }

    // member fields

    private MappingConfiguration _mappingConfiguration;
    private RdbmsProviderDefinition _rdbmsProviderDefinition;

    private ClassDefinitionCollection _classes;

    // construction and disposing

    public FileBuilderBase (MappingConfiguration mappingConfiguration, RdbmsProviderDefinition rdbmsProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("mappingConfiguration", mappingConfiguration);
      ArgumentUtility.CheckNotNull ("rdbmsProviderDefinition", rdbmsProviderDefinition);

      _mappingConfiguration = mappingConfiguration;
      _rdbmsProviderDefinition = rdbmsProviderDefinition;
      _classes = GetClassesInStorageProvider (mappingConfiguration.ClassDefinitions, _rdbmsProviderDefinition.Name);
    }

    private ClassDefinitionCollection GetClassesInStorageProvider (ClassDefinitionCollection classDefinitions, string storageProviderID)
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      foreach (ClassDefinition currentClass in classDefinitions)
      {
        if (currentClass.StorageProviderID == _rdbmsProviderDefinition.Name)
          classes.Add (currentClass);
      }

      return classes;
    }

    // methods and properties

    public abstract string GetScript ();

    public MappingConfiguration MappingConfiguration
    {
      get { return _mappingConfiguration; }
    }

    public RdbmsProviderDefinition RdbmsProviderDefinition
    {
      get { return _rdbmsProviderDefinition; }
    }

    public ClassDefinitionCollection Classes
    {
      get { return _classes; }
    }
  }
}
