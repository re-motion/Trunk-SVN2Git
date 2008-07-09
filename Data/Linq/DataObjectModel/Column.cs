/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Utilities;

namespace Remotion.Data.Linq.DataObjectModel
{
  public struct Column : ICriterion
  {
    public readonly IColumnSource ColumnSource;
    // If Name is null, the column represents access to the whole ColumnSource. For tables, this would be the whole table; for let clauses either a
    // table, a column, or a computed value.
    public readonly string Name;

    public Column (IColumnSource columnSource, string name)
    {
      ArgumentUtility.CheckNotNull ("fromSource", columnSource);
      Name = name;
      ColumnSource = columnSource;
    }

    public override string ToString ()
    {
      return (ColumnSource != null ? ColumnSource.AliasString : "<null>") + "." + Name;
    }

    public void Accept (IEvaluationVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.VisitColumn (this);
    }
  }
}
