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

namespace JetBrains.Annotations
{
  /// <summary>
  /// Should be used on attributes and causes ReSharper to not mark symbols marked with such attributes as unused (as well as by other usage inspections)
  /// </summary>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  sealed partial class MeansImplicitUseAttribute : Attribute
  {
    public MeansImplicitUseAttribute ()
        : this (ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default) { }

    public MeansImplicitUseAttribute (ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
    {
      UseKindFlags = useKindFlags;
      TargetFlags = targetFlags;
    }

    public MeansImplicitUseAttribute (ImplicitUseKindFlags useKindFlags)
        : this (useKindFlags, ImplicitUseTargetFlags.Default)
    {
    }

    public MeansImplicitUseAttribute (ImplicitUseTargetFlags targetFlags)
        : this (ImplicitUseKindFlags.Default, targetFlags) { }

    public ImplicitUseKindFlags UseKindFlags { get; private set; }

    /// <summary>
    /// Gets value indicating what is meant to be used
    /// </summary>
    public ImplicitUseTargetFlags TargetFlags { get; private set; }
  }
}