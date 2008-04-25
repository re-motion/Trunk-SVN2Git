using System;
using Remotion.Collections;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

/// <summary> Exposes non-public members of the <see cref="WxeParameterConverter"/> type. </summary>
public class WxeParameterConverterMock: WxeParameterConverter
{
  public WxeParameterConverterMock (WxeParameterDeclaration parameter)
    : base (parameter)
  {
  }

  public new string ConvertVarRefToString (WxeVariableReference varRef, NameObjectCollection callerVariables)
  {
    return base.ConvertVarRefToString (varRef, callerVariables);
  }

  public new string ConvertObjectToString (object value)
  {
    return base.ConvertObjectToString (value);
  }

  public new void CheckForRequiredOutParameter()
  {
    base.CheckForRequiredOutParameter();
  }

  public new object TryConvertObjectToString (object value)
  {
    return base.TryConvertObjectToString (value);
  }
}

}