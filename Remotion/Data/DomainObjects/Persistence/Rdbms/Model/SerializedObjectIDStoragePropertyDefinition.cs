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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  public class SerializedObjectIDStoragePropertyDefinition : IObjectIDStoragePropertyDefinition
  {
    private readonly SimpleStoragePropertyDefinition _simpleStoragePropertyDefinition;

    public SerializedObjectIDStoragePropertyDefinition (SimpleStoragePropertyDefinition simpleStoragePropertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("simpleStoragePropertyDefinition", simpleStoragePropertyDefinition);

      _simpleStoragePropertyDefinition = simpleStoragePropertyDefinition;
    }

    public SimpleStoragePropertyDefinition SimpleStoragePropertyDefinition
    {
      get { return _simpleStoragePropertyDefinition; }
    }

    public ColumnDefinition GetColumnForLookup ()
    {
      return _simpleStoragePropertyDefinition.ColumnDefinition;
    }

    public IEnumerable<ColumnDefinition> GetColumns ()
    {
      return _simpleStoragePropertyDefinition.GetColumns();
    }

    public string Name
    {
      get { return _simpleStoragePropertyDefinition.Name; }
    }

    public bool Equals (IRdbmsStoragePropertyDefinition other)
    {
      if (other == null || other.GetType () != GetType ())
        return false;

      return _simpleStoragePropertyDefinition.Equals (((SerializedObjectIDStoragePropertyDefinition) other).SimpleStoragePropertyDefinition);
    }

    public bool IsNull
    {
      get { return _simpleStoragePropertyDefinition.IsNull; }
    }
    
  }
}