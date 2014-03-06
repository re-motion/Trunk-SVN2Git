// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FluentValidation.Results;
using Remotion.Validation.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  [Serializable]
  public class DomainObjectFluentValidationException : DomainObjectException
  {
    public DomainObjectFluentValidationException (IEnumerable<ValidationFailure> failures)
        : this (failures, null)
    {
    }

    public DomainObjectFluentValidationException (IEnumerable<ValidationFailure> failures, Exception inner)
        : base (BuildErrorMesage (failures), inner)
    {
    }

    protected DomainObjectFluentValidationException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }

    private static string BuildErrorMesage (IEnumerable<ValidationFailure> errors)
    {
      var errorsByValidatedObjects = errors.ToLookup (e => e.GetValidatedInstance());

      var errorMessage = new StringBuilder ("One or more DomainObject contain inconsistent data:\r\n\r\n");
      //TODO AO: move error message from exception to extension.
      foreach (var errorByValidatedObject in errorsByValidatedObjects)
      {
        errorMessage.AppendLine (GetKeyText (errorByValidatedObject.Key));
        errorMessage.AppendLine (
            string.Join (
                "\r\n",
                errorByValidatedObject.Select (t => " -- " + t.ErrorMessage)));
        errorMessage.AppendLine();
      }
      return errorMessage.ToString();
    }

    private static string GetKeyText (object validatedInstance)
    {
      var domainObject = validatedInstance as DomainObject;
      if (domainObject != null)
        return string.Format ("Object '{0}' with ID '{1}':", domainObject.ID.ClassID, domainObject.ID.Value);
      return string.Format ("Validation error on object of Type '{0}':", validatedInstance.GetType().FullName);
    }
  }
}