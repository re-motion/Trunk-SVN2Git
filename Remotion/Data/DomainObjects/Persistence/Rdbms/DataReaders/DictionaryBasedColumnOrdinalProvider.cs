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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders
{
  /// <summary>
  /// The <see cref="DictionaryBasedColumnOrdinalProvider"/> calculates the index of a <see cref="ColumnDefinition"/> 
  /// based on <see cref="IDictionary{TKey,TValue}"/> entries.
  /// </summary>
  public class DictionaryBasedColumnOrdinalProvider : IColumnOrdinalProvider
  {
    private readonly IDictionary<ColumnDefinition, int> _ordinals;

    public DictionaryBasedColumnOrdinalProvider (IDictionary<ColumnDefinition, int> ordinals)
    {
      ArgumentUtility.CheckNotNull ("ordinals", ordinals);

      _ordinals = ordinals;
    }

    public int GetOrdinal (ColumnDefinition columnDefinition, IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      int index;
      if (_ordinals.TryGetValue (columnDefinition, out index))
        return index;

      throw new ArgumentException (string.Format ("No ordinal was defined for column '{0}'.", columnDefinition.Name), "columnDefinition");
    }
  }
}