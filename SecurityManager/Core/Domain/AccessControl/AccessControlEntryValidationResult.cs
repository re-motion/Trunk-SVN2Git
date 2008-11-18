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
using System.Text;
using Remotion.Collections;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public class AccessControlEntryValidationResult
  {
    private readonly Set<AccessControlEntryValidationError> _errors = new Set<AccessControlEntryValidationError>();

    public AccessControlEntryValidationResult ()
    {
    }

    public bool IsValid
    {
      get { return _errors.Count == 0; }
    }

    public AccessControlEntryValidationError[] GetErrors ()
    {
      return _errors.OrderBy (e => (int) e).ToArray();
    }

    public void SetError (AccessControlEntryValidationError error)
    {
      _errors.Add (error);
    }

    public string GetErrorMessage ()
    {
      StringBuilder errorMessageBuilder = new StringBuilder(_errors.Count * 100);
      errorMessageBuilder.Append ("The access control entry is in an invalid state:");
      foreach (var error in GetErrors())
      {
        errorMessageBuilder.AppendLine();
        errorMessageBuilder.Append ("  ");
        errorMessageBuilder.Append (EnumDescription.GetDescription (error));
      }

      return errorMessageBuilder.ToString();
    }
  }
}