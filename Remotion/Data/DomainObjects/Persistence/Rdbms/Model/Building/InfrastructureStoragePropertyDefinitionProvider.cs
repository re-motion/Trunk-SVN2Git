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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// The <see cref="InfrastructureStoragePropertyDefinitionProvider"/> class is responsible to create 
  /// <see cref="IRdbmsStoragePropertyDefinition"/> objects for infrastructure columns.
  /// </summary>
  public class InfrastructureStoragePropertyDefinitionProvider : IInfrastructureStoragePropertyDefinitionProvider
  {
    private readonly StorageTypeCalculator _storageTypeCalculator;
    private readonly IStorageNameProvider _storageNameProvider;

    public InfrastructureStoragePropertyDefinitionProvider (
        StorageTypeCalculator storageTypeCalculator,
        IStorageNameProvider storageNameProvider)
    {
      ArgumentUtility.CheckNotNull ("storageTypeCalculator", storageTypeCalculator);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);

      _storageTypeCalculator = storageTypeCalculator;
      _storageNameProvider = storageNameProvider;
    }

    public ColumnDefinition GetObjectIDColumnDefinition ()
    {
      return new ColumnDefinition (
          _storageNameProvider.IDColumnName, typeof (ObjectID), _storageTypeCalculator.ObjectIDStorageType, false, true);
    }

    public ColumnDefinition GetClassIDColumnDefinition ()
    {
      return new ColumnDefinition (
          _storageNameProvider.ClassIDColumnName, typeof (string), _storageTypeCalculator.ClassIDStorageType, false, false);
    }

    public virtual ColumnDefinition GetTimestampColumnDefinition ()
    {
      return new ColumnDefinition (
          _storageNameProvider.TimestampColumnName, typeof (object), _storageTypeCalculator.TimestampStorageType, false, false);
    }
  }
}