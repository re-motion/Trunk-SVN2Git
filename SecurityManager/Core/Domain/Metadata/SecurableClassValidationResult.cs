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
    private List<StateCombination> _invalidStateCombinations = new List<StateCombination> ();

    public bool IsValid
    {
      get { return _isValid; }
    }

    public ReadOnlyCollection<StateCombination> InvalidStateCombinations
    {
      get { return _invalidStateCombinations.AsReadOnly(); }
    }

    public void AddInvalidStateCombination (StateCombination invalidStateCombination)
    {
      ArgumentUtility.CheckNotNull ("invalidStateCombination", invalidStateCombination);

      _isValid = false;
      _invalidStateCombinations.Add (invalidStateCombination);
    }
  }
}
