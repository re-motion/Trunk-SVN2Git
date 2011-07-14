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
using System.Data;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  public class UnsupportedStoragePropertyDefinition : IRdbmsStoragePropertyDefinition
  {
    string IStoragePropertyDefinition.Name
    {
      get { return "unsupported property"; }
    }

    public IEnumerable<ColumnDefinition> GetColumns ()
    {
      return new ColumnDefinition[0];
    }

    public ColumnDefinition GetColumnForLookup ()
    {
      // TODO Review 4129: throw NotSupportedException
      throw new NotImplementedException ();
    }

    public ColumnDefinition GetColumnForForeignKey ()
    {
      // TODO Review 4129: throw NotSupportedException
      throw new NotImplementedException ();
    }

    public object Read (IDataReader dataReader, IColumnOrdinalProvider ordinalProvider)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);
      ArgumentUtility.CheckNotNull ("ordinalProvider", ordinalProvider);

      // TODO Review 4129: throw NotSupportedException
      return null;
    }

    public IEnumerable<IDataParameter> CreateDataParameters (IDbCommand command, object value, string key)
    {
      ArgumentUtility.CheckNotNull ("command", command);
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);

      // TODO Review 4129: throw NotSupportedException
      return new IDataParameter[0];
    }
  }
}