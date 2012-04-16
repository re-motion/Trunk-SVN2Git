// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System.Collections.Generic;
using Microsoft.Scripting.Ast;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection.BodyBuilding
{
  /// <summary>
  /// Provides access to parameters and custom expression for building the bodies of modified constructors. 
  /// See also <see cref="MutableConstructorInfo.SetBody"/>.
  /// </summary>
  public class ConstructorBodyModificationContext : MethodBodyModificationContext
  {
    public ConstructorBodyModificationContext (
        MutableType declaringType, IEnumerable<ParameterExpression> parameterExpressions, Expression previousBody)
        : base (declaringType, parameterExpressions, previousBody, false)
    {
      ArgumentUtility.CheckNotNull ("previousBody", previousBody);
    }

    public Expression GetConstructorCall (params Expression[] arguments)
    {
      ArgumentUtility.CheckNotNull ("arguments", arguments);

      return GetConstructorCall (((IEnumerable<Expression>) arguments));
    }

    public Expression GetConstructorCall (IEnumerable<Expression> arguments)
    {
      ArgumentUtility.CheckNotNull ("arguments", arguments);

      return ConstructorBodyContextUtility.GetConstructorCallExpression (This, arguments);
    }
  }
}