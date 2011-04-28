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
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements
{
  /// <summary>
  /// The <see cref="BatchSeparatorStatement"/> adds a batch-separator to a script-statement for a relational database.
  /// </summary>
  public class BatchSeparatorStatement : IScriptElement
  {
    public void AppendToScript (List<ScriptStatement> script, ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("script", script);
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      var lastStatement = script.LastOrDefault();
      if(lastStatement!=null && lastStatement.Statement!=sqlDialect.BatchSeparator)
        script.Add (new ScriptStatement (sqlDialect.BatchSeparator));
    }
  }
}