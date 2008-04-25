using System;
using System.Collections;
using System.Collections.Generic;
using Rhino.Mocks.Constraints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.MockConstraints
{
  public class ContainsConstraint : AbstractConstraint
  {
    // types

    // static members and constants

    // member fields

    private List<IsIn> _constraints = new List<IsIn> ();

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
