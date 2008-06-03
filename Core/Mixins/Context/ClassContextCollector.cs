/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;

namespace Remotion.Mixins.Context
{
  public class ClassContextCollector
  {
    private readonly List<ClassContext> _classContexts = new List<ClassContext>();

    public IEnumerable<ClassContext> CollectedContexts
    {
      get { return _classContexts; }
    }

    public void Add (ClassContext context)
    {
      if (context != null)
        _classContexts.Add (context);
    }


    public ClassContext GetCombinedContexts (Type contextType)
    {
      switch (_classContexts.Count)
      {
        case 0:
          return null;
        case 1:
          return _classContexts[0].CloneForSpecificType (contextType);
        default:
          return new ClassContext (contextType).InheritFrom (_classContexts);
      }
    }
  }
}
