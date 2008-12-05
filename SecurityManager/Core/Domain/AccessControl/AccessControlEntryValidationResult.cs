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
using System.Text;
using Remotion.Collections;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// The <see cref="AccessControlEntryValidationResult"/> type collects validation state for the <see cref="AccessControlEntry"/> type.
  /// </summary>
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
