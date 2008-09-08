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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  [Serializable]
  public class WxeVariableReference
  {
    private readonly string _name;

    public WxeVariableReference (string variableName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("variableName", variableName);
      if (! System.Text.RegularExpressions.Regex.IsMatch (variableName, @"^([a-zA-Z_][a-zA-Z0-9_]*)$"))
        throw new ArgumentException (string.Format ("The variable name '{0}' is not valid.", variableName), "variableName");
      _name = variableName;
    }

    public string Name
    {
      get { return _name; }
    }

    public override bool Equals (object obj)
    {
      WxeVariableReference other = obj as WxeVariableReference;
      if (other == null)
        return false;
    
      return this._name == other._name;
    }

    public override int GetHashCode()
    {
      return _name.GetHashCode();
    }
  }
}
