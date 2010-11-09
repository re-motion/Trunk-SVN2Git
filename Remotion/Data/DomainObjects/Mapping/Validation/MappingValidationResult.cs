// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation
{
  /// <summary>
  /// <see cref="MappingValidationResult"/> is returned by the validate-methods of the mapping configuration validators and contains the information,
  /// if the rule is valid. If not, it also returns an error message.
  /// </summary>
  public class MappingValidationResult
  {
    public static MappingValidationResult CreateValidResult ()
    {
      return new MappingValidationResult (true, string.Empty);
    }

    public static MappingValidationResult CreateInvalidResult (string message)
    {
      return new MappingValidationResult (false, message);
    }
    
    private readonly bool _isValid;
    private readonly string _message;

    protected MappingValidationResult (bool isValid, string message)
    {
      ArgumentUtility.CheckNotNull ("message", message);

      _isValid = isValid;
      _message = message;
    }

    public bool IsValid
    {
      get { return _isValid; }
    }

    public string Message
    {
      get { return _message; }
    }
  }
}