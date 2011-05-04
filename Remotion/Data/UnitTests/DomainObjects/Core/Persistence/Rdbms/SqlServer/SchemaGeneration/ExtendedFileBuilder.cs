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
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class ExtendedFileBuilder : FileBuilder
  {
    public ExtendedFileBuilder (Func<CompositeScriptBuilder> scriptBuilderFactory)
        : base (scriptBuilderFactory, new ExtendedEntityDefinitionProvider())
    {
    }

    public override ScriptPair GetScript (IEnumerable<ClassDefinition> classDefinitions)
    {
      var scripts = base.GetScript (classDefinitions);
      var dropStript = new StringBuilder (scripts.DropScript);
      var createScript = new StringBuilder (scripts.CreateScript);

      dropStript.Insert (0, "IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'Test') BEGIN EXEC('CREATE SCHEMA Test') END\r\nGO\r\n");
      dropStript.Insert (0, "--Extendend file-builder comment at the beginning\r\n");
      createScript.AppendLine ("--Extendend file-builder comment at the end");

      return new ScriptPair(createScript.ToString(), dropStript.ToString());
    }
   
  }
}