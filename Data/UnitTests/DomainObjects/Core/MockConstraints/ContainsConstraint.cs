// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
