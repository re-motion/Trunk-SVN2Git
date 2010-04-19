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
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Augments the <see cref="Table"/> type with meta-information about the mapping.
  /// </summary>
  [Obsolete ("This LINQ provider will soon be removed. (1.13.55)")]
  public class MappedTable : Table
  {
    public MappedTable (string name, string alias, ClassDefinition classDefinition)
        : base(name, alias)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      ClassDefinition = classDefinition;
    }

    public ClassDefinition ClassDefinition { get; private set; }
  }
}