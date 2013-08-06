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
using Remotion.TypePipe.Dlr.Ast;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection.BodyBuilding
{
  /// <summary>
  /// Provides access to expressions needed for building instance initializations.
  /// </summary>
  public class InitializationBodyContext : BodyContextBase
  {
    private readonly ParameterExpression _initializationSemantics;

    public InitializationBodyContext (MutableType declaringType, bool isStatic, ParameterExpression initializationSemantics)
        : base (declaringType, isStatic)
    {
      ArgumentUtility.CheckNotNull ("initializationSemantics", initializationSemantics);

      _initializationSemantics = initializationSemantics;
    }

    /// <summary>
    /// Represents a parameter of type <see cref="TypePipe.Implementation.InitializationSemantics"/> which can be used to determine the
    /// initialization context in which the code is executed.
    /// </summary>
    public ParameterExpression InitializationSemantics
    {
      get { return _initializationSemantics; }
    }
  }
}