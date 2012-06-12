// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// The <see cref="StoragePropertyDefinitionResolver"/> is responsible to get all <see cref="IRdbmsStoragePropertyDefinition"/> instances for a 
  /// <see cref="ClassDefinition"/>.
  /// </summary>
  public class StoragePropertyDefinitionResolver : IStoragePropertyDefinitionResolver
  {
    private readonly IRdbmsPersistenceModelProvider _persistenceModelProvider;

    public StoragePropertyDefinitionResolver (IRdbmsPersistenceModelProvider persistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull ("persistenceModelProvider", persistenceModelProvider);
      _persistenceModelProvider = persistenceModelProvider;
    }

    public IRdbmsPersistenceModelProvider PersistenceModelProvider
    {
      get { return _persistenceModelProvider; }
    }

    public IEnumerable<IRdbmsStoragePropertyDefinition> GetStoragePropertiesForHierarchy (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var allClassesInHierarchy = classDefinition
          .CreateSequence (cd => cd.BaseClass)
          .Reverse ()
          .Concat (classDefinition.GetAllDerivedClasses ());

      var storageProperties =
          (from cd in allClassesInHierarchy
           from PropertyDefinition pd in cd.MyPropertyDefinitions
           where pd.StorageClass == StorageClass.Persistent
           select _persistenceModelProvider.GetStoragePropertyDefinition (pd))
           .Distinct ();

      return storageProperties;
    }
  }
}