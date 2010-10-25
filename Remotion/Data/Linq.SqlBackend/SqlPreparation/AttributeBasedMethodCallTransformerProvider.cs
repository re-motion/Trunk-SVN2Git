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
using System;
using System.Linq.Expressions;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlPreparation
{
  /// <summary>
  /// Retrieves <see cref="IMethodCallTransformer"/> instances based on the <see cref="MethodCallTransformerAttribute"/>.
  /// </summary>
  public class AttributeBasedMethodCallTransformerProvider : IMethodCallTransformerProvider
  {
    public IMethodCallTransformer GetTransformer (MethodCallExpression methodCallExpression)
    {
      ArgumentUtility.CheckNotNull ("methodCallExpression", methodCallExpression);

      var attribute = 
          (MethodCallTransformerAttribute) Attribute.GetCustomAttribute (methodCallExpression.Method, typeof (MethodCallTransformerAttribute), true);

      if (attribute == null)
        return null;

      try
      {
        var transformer = (IMethodCallTransformer) Activator.CreateInstance (attribute.TransformerType);
        return transformer;
      }
      catch (MissingMethodException ex)
      {
        var message = string.Format (
            "The method call transformer '{0}' has no public default constructor and therefore cannot be used with the MethodCallTransformerAttribute.", 
            attribute.TransformerType);
        throw new MissingMethodException (message, ex);
      }
    }
  }
}