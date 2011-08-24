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
namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// <see cref="IInfrastructureStoragePropertyDefinitionProvider"/> defines the API to create instances of <see cref="IRdbmsStoragePropertyDefinition"/>
  /// to use for infrastructure columns.
  /// </summary>
  public interface IInfrastructureStoragePropertyDefinitionProvider
  {
    ColumnDefinition GetIDColumnDefinition ();
    ColumnDefinition GetClassIDColumnDefinition ();
    ColumnDefinition GetTimestampColumnDefinition ();

    ObjectIDStoragePropertyDefinition GetObjectIDStoragePropertyDefinition ();
    IRdbmsStoragePropertyDefinition GetTimestampStoragePropertyDefinition ();

    IRdbmsStoragePropertyDefinition GetObjectIDStoragePropertyDefinition (IEntityDefinition entityDefinition);
    IRdbmsStoragePropertyDefinition GetTimestampStoragePropertyDefinition (IEntityDefinition entityDefinition);
  }
}