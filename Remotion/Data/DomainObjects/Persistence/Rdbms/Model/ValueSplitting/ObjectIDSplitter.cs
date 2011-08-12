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
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.ValueSplitting
{
  /// <summary>
  /// Splits an <see cref="ObjectID"/> value into <see cref="TypedValue"/> instances for the value and ClassID, and recombines a set of split values 
  /// into a single  <see cref="ObjectID"/>.
  /// </summary>
  public class ObjectIDSplitter : IValueSplitter
  {
    private readonly IValueSplitter _valueSplitter;
    private readonly IValueSplitter _classIDValueSplitter;

    public ObjectIDSplitter (IValueSplitter valueSplitter, IValueSplitter classIDValueSplitter)
    {
      ArgumentUtility.CheckNotNull ("valueSplitter", valueSplitter);
      ArgumentUtility.CheckNotNull ("classIDValueSplitter", classIDValueSplitter);

      _valueSplitter = valueSplitter;
      _classIDValueSplitter = classIDValueSplitter;
    }

    public IEnumerable<TypedValue> Split (object value)
    {
      var objectID = ArgumentUtility.CheckType<ObjectID> ("value", value);
      if (objectID == null)
        return _valueSplitter.Split (null).Concat (_classIDValueSplitter.Split (null));

      return _valueSplitter.Split (objectID.Value).Concat (_classIDValueSplitter.Split (objectID.ClassID));
    }

    public IEnumerable<TypedValue> SplitForComparison (object value)
    {
      // TODO in case of integer primary keys: 
      // The code here only works if the ObjectID value alone (without ClassID) is unique. Otherwise, the ClassID must be included.

      var objectID = ArgumentUtility.CheckType<ObjectID> ("value", value);
      if (objectID == null)
        return _valueSplitter.SplitForComparison (null);

      return _valueSplitter.SplitForComparison (objectID.Value);
    }

    public object Combine (IEnumerator splitValueSource)
    {
      ArgumentUtility.CheckNotNull ("splitValueSource", splitValueSource);
      return null;
    }
  }
}