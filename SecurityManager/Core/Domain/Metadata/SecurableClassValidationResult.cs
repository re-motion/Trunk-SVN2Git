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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  public class SecurableClassValidationResult
  {
    private bool _isValid = true;
    private readonly List<StateCombination> _duplicateStateCombinations = new List<StateCombination> ();
    private readonly List<StateCombination> _invalidStateCombinations = new List<StateCombination> ();

    public bool IsValid
    {
      get { return _isValid; }
    }

    public ReadOnlyCollection<StateCombination> DuplicateStateCombinations
    {
      get { return _duplicateStateCombinations.AsReadOnly(); }
    }

    public void AddDuplicateStateCombination (StateCombination duplicateStateCombination)
    {
      ArgumentUtility.CheckNotNull ("duplicateStateCombination", duplicateStateCombination);

      _isValid = false;
      _duplicateStateCombinations.Add (duplicateStateCombination);
    }

    public ReadOnlyCollection<StateCombination> InvalidStateCombinations
    {
      get { return _invalidStateCombinations.AsReadOnly (); }
    }

    public void AddInvalidStateCombination (StateCombination invalidStateCombination)
    {
      ArgumentUtility.CheckNotNull ("invalidStateCombination", invalidStateCombination);

      _isValid = false;
      _invalidStateCombinations.Add (invalidStateCombination);
    }
  }
}
