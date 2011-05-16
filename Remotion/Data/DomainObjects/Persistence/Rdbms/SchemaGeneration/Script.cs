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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// <see cref="Script"/> holds a setup- and teardown script for a relational database.
  /// </summary>
  public class Script
  {
    private readonly RdbmsProviderDefinition _storageProviderDefinition;
    private readonly string _createScript;
    private readonly string _dropScript;

    public Script (RdbmsProviderDefinition storageProviderDefinition, string createScript, string dropScript)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("createScript", createScript);
      ArgumentUtility.CheckNotNull ("dropScript", dropScript);

      _storageProviderDefinition = storageProviderDefinition;
      _createScript = createScript;
      _dropScript = dropScript;
    }

    public RdbmsProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
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
}