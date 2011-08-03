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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// The <see cref="InfrastructureStoragePropertyDefinitionProvider"/> class is responsible to create 
  /// <see cref="IRdbmsStoragePropertyDefinition"/> objects for infrastructure columns.
  /// </summary>
  public class InfrastructureStoragePropertyDefinitionProvider : IInfrastructureStoragePropertyDefinitionProvider
  {
    // TODO Review 4207: Should be an instance method, and part of the interface
    public static ObjectIDStoragePropertyDefinition GetObjectIDStoragePropertyDefinition (IEntityDefinition entityDefinition)
    {
      return new ObjectIDStoragePropertyDefinition (
          new SimpleStoragePropertyDefinition (entityDefinition.IDColumn),
          new SimpleStoragePropertyDefinition (entityDefinition.ClassIDColumn));
    }

    // TODO Review 4207: Should be an instance method, and part of the interface
    public static SimpleStoragePropertyDefinition GetTimestampStoragePropertyDefinition (IEntityDefinition entityDefinition)
    {
      return new SimpleStoragePropertyDefinition (entityDefinition.TimestampColumn);
    }

    private readonly ColumnDefinition _idColumnDefinition;
    private readonly ColumnDefinition _classIDColumnDefinition;
    private readonly ColumnDefinition _timestampColumnDefinition;

    public InfrastructureStoragePropertyDefinitionProvider (
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider)
    {
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);

      _idColumnDefinition = new ColumnDefinition (
          storageNameProvider.IDColumnName,
          typeof (ObjectID),
          storageTypeInformationProvider.ObjectIDStorageType,
          false,
          true);
      _classIDColumnDefinition = new ColumnDefinition (
          storageNameProvider.ClassIDColumnName,
          typeof (string),
          storageTypeInformationProvider.ClassIDStorageType,
          false,
          false);
      _timestampColumnDefinition = new ColumnDefinition (
          storageNameProvider.TimestampColumnName,
          typeof (object),
          storageTypeInformationProvider.TimestampStorageType,
          false,
          false);
    }

    public ColumnDefinition GetIDColumnDefinition ()
    {
      return _idColumnDefinition;
    }

    public ColumnDefinition GetClassIDColumnDefinition ()
    {
      return _classIDColumnDefinition;
    }

    public virtual ColumnDefinition GetTimestampColumnDefinition ()
    {
      return _timestampColumnDefinition;
    }
  }
}