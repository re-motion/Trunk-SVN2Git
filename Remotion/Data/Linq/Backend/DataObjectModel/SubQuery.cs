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
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.Backend.DataObjectModel
{
  public class SubQuery : IColumnSource, ICriterion
  {
    public SubQuery (QueryModel queryModel, ParseMode parseMode, string alias)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);
      ArgumentUtility.CheckNotNull ("parseMode", parseMode);

      QueryModel = queryModel;
      Alias = alias;
      ParseMode = parseMode;
    }

    public QueryModel QueryModel { get; private set; }
    public string Alias { get; private set; }
    public ParseMode ParseMode { get; private set; }

    public string AliasString
    {
      get { return Alias; }
    }

    public bool IsTable
    {
      get { return true; }
    }

    public override bool Equals (object obj)
    {
      var other = obj as SubQuery;
      return other != null && Equals (QueryModel, other.QueryModel) && Equals (Alias, other.Alias);
    }

    public override int GetHashCode ()
    {
      return Remotion.Utilities.EqualityUtility.GetRotatedHashCode (Alias, QueryModel);
    }

    public void Accept (IEvaluationVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.VisitSubQuery (this);
    }
  }
}
