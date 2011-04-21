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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// Acts as an abstract base class for implementations generating a setup script for a list of entity definitions.
  /// </summary>
  public abstract class ScriptBuilderBase
  {
    private readonly RdbmsProviderDefinition _rdbmsProviderDefinition;
    private readonly ISqlDialect _sqlDialect;

    protected ScriptBuilderBase (RdbmsProviderDefinition rdbmsProviderDefinition, ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("rdbmsProviderDefinition", rdbmsProviderDefinition);
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      _rdbmsProviderDefinition = rdbmsProviderDefinition;
      _sqlDialect = sqlDialect;
    }

    public RdbmsProviderDefinition RdbmsProviderDefinition
    {
      get { return _rdbmsProviderDefinition; }
    }

    public ISqlDialect SqlDialect
    {
      get { return _sqlDialect; }
    }

    public abstract void AddEntityDefinition (IEntityDefinition entityDefinition);
    public abstract string GetCreateScript ();
    public abstract string GetDropScript ();
  }
}