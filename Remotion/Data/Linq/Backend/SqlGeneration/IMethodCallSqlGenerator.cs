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
using Remotion.Data.Linq.Backend.DataObjectModel;

namespace Remotion.Data.Linq.Backend.SqlGeneration
{
  /// <summary>
  /// This interface has to be implemented, when a new MethodCallGenerator is generated. This generator has to handle method calls which are not
  /// supported as default by the framework. This generator has to be registered to <see cref="MethodCallSqlGeneratorRegistry"/>.
  /// </summary>
  public interface IMethodCallSqlGenerator
  {
    /// <summary>
    /// The method has to contain the logic for generating sql code for the method call. 
    /// </summary>
    /// <param name="methodCall"><see cref="MethodCall"/></param>
    /// <param name="commandBuilder"><see cref="ICommandBuilder"/></param>
    void GenerateSql (MethodCall methodCall, ICommandBuilder commandBuilder);
  }
}
