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
#if !NET_3_5
using System.Linq;
#endif
using System.Linq.Expressions;
using Remotion.Data.Linq.Collections;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.Backend.DetailParsing
{
  public class ParserRegistry
  {
#if !NET_3_5
    private class TypeComperer : IComparer<Type>
    {
      public int Compare(Type left, Type right)
      {
        if (left == right)
          return 0;
        if (left.IsAssignableFrom(right))
          return -1;
        else
          return 1;
      }
    }
#endif

    private readonly MultiDictionary<Type, IParser> _parsers;

    public ParserRegistry ()
    {
      _parsers = new MultiDictionary<Type, IParser>();
    }

    public void RegisterParser (Type expressionType, IParser parser)
    {
      _parsers[expressionType].Insert (0, parser);
    }

    public IEnumerable<IParser> GetParsers (Type expressionType)
    {
#if NET_3_5
      return _parsers[expressionType];
#else
      return _parsers
          .Where (p => p.Key.IsAssignableFrom (expressionType))
          .OrderByDescending (p => p.Key, new TypeComperer ())
          .Select (p => p.Value)
          .FirstOrDefault () ?? new IParser[0];
#endif
    }
  
    public IParser GetParser (Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      foreach (IParser parser in GetParsers (expression.GetType()))
      {
        if (parser.CanParse (expression))
          return parser;
      }
      throw new ParserException ("Cannot parse " + expression + ", no appropriate parser found");
    }
  }
}
