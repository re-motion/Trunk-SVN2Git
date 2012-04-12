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
using System;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Ast.Compiler;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.LambdaCompilation;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit
{
  /// <summary>
  /// Prepares method bodies so that code can be generated for them by expanding all <see cref="ITypePipeExpression"/> nodes not understood by
  /// <see cref="LambdaCompiler"/> and <see cref="ILGeneratingTypePipeExpressionVisitor"/>.
  /// </summary>
  public class ExpandingExpressionPreparer : IExpressionPreparer
  {
    public Expression PrepareConstructorBody (MutableConstructorInfo mutableConstructorInfo)
    {
      ArgumentUtility.CheckNotNull ("mutableConstructorInfo", mutableConstructorInfo);

      var visitor = new OriginalConstructorBodyReplacingExpressionVisitor (mutableConstructorInfo);
      return visitor.Visit (mutableConstructorInfo.Body);
    }

    // TODO RM-4753
    public Expression PrepareMethodBody (MutableMethodInfo mutableMethodInfo)
    {
      ArgumentUtility.CheckNotNull ("mutableMethodInfo", mutableMethodInfo);

      return mutableMethodInfo.Body;
    }
  }
}