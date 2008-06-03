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
