using System;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  /// <summary>
  /// Serializable version of the <see cref="ValidationLogData"/> object.
  /// </summary>
  [Serializable]
  public class SerializableValidationLogData
  {
    private readonly int _NumberOfFailures;
    private readonly int _numberOfRulesExecuted;
    private readonly int _numberOfSuccesses;
    private readonly int _numberOfUnexpectedExceptions;
    private readonly int _numberOfWarnings;

    public SerializableValidationLogData (ValidationLogData validationLogData)
    {
      ArgumentUtility.CheckNotNull ("validationLogData", validationLogData);

      _NumberOfFailures = validationLogData.GetNumberOfFailures();
      _numberOfRulesExecuted = validationLogData.GetNumberOfRulesExecuted();
      _numberOfSuccesses = validationLogData.GetNumberOfSuccesses();
      _numberOfUnexpectedExceptions = validationLogData.GetNumberOfUnexpectedExceptions();
      _numberOfWarnings = validationLogData.GetNumberOfWarnings();
    }

    public int NumberOfRulesExecuted
    {
      get { return _numberOfRulesExecuted; }
    }

    public int NumberOfUnexpectedExceptions
    {
      get { return _numberOfUnexpectedExceptions; }
    }

    public int NumberOfFailures
    {
      get { return _NumberOfFailures; }
    }

    public int NumberOfWarnings
    {
      get { return _numberOfWarnings; }
    }

    public int NumberOfSuccesses
    {
      get { return _numberOfSuccesses; }
    }
  }
}