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
using System.Collections;
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Utilities;
using Rhino.Mocks.Constraints;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MockConstraints
{
  public class ContainsConstraint : AbstractConstraint
  {
    // types

    // static members and constants

    // member fields

    private readonly List<IsIn> _constraints = new List<IsIn> ();

    // construction and disposing

    public ContainsConstraint (params object[] objects) : this ((IEnumerable) objects)
    {
    }

    public ContainsConstraint (IEnumerable objects)
    {
      ArgumentUtility.CheckNotNull ("objects", objects);

      _constraints = new List<IsIn> ();
      foreach (object current in objects)
        _constraints.Add (new IsIn (current));
    }

    // methods and properties

    public override bool Eval (object obj)
    {
      foreach (IsIn constraint in _constraints)
      {
        if (!constraint.Eval (obj))
          return false;
      }
      return true;
    }

    public override string Message
    {
      get { return "contains multiple objects"; }
    }
  }
}
