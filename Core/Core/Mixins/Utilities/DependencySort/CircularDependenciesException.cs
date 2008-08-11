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

namespace Remotion.Mixins.Utilities.DependencySort
{
  public class CircularDependenciesException<T> : Exception
  {
    private readonly IEnumerable<T> _circulars;

    public CircularDependenciesException (string message, IEnumerable<T> circulars)
        : base (message)
    {
      _circulars = circulars;
    }

    public CircularDependenciesException (string message, IEnumerable<T> circulars, Exception inner)
        : base (message, inner)
    {
      _circulars = circulars;
    }

    public IEnumerable<T> Circulars
    {
      get { return _circulars; }
    }
  }
}
