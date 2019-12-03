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
using System.Reflection;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Attributes.Validation
{
  /// <summary>
  /// Apply the <see cref="NotNullAttribute"/> to introduce a <see cref="NotNullValidator"/> constraint for a property.
  /// </summary>
  public class NotNullAttribute : AddingValidationAttributeBase
  {
    public NotNullAttribute ()
    {
    }

    protected override IEnumerable<IPropertyValidator> GetValidators (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      if (string.IsNullOrEmpty (ErrorMessage))
        return EnumerableUtility.Singleton (new NotNullValidator());
      else
        return EnumerableUtility.Singleton (new NotNullValidator (new InvariantValidationMessage (ErrorMessage)));
    }
  }
}