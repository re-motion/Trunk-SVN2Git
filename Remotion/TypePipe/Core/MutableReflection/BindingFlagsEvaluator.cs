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
using System.Reflection;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// Evaluates <see cref="MethodAttributes"/> and <see cref="FieldAttributes"/> against the specified <see cref="BindingFlags"/>.
  /// This is used by <see cref="MemberSelector"/> to filter members.
  /// </summary>
  public class BindingFlagsEvaluator : IBindingFlagsEvaluator
  {
    public bool HasRightAttributes (MethodAttributes methodAttributes, BindingFlags bindingFlags)
    {
      return HasRightVisibility (methodAttributes, bindingFlags) && HasRightInstanceOrStaticFlag (methodAttributes, bindingFlags);
    }

    public bool HasRightAttributes (FieldAttributes fieldAttributes, BindingFlags bindingFlags)
    {
      // It's safe to cast from FieldAttributes to MethodAttributes because the relevant flag values are the same. This is checked by a unit test.
      return HasRightAttributes ((MethodAttributes) fieldAttributes, bindingFlags);
    }

    public bool HasRightVisibility (MethodAttributes methodAttributes, BindingFlags bindingFlags)
    {
      var isPublic = ((methodAttributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public);
      if (isPublic)
        return IsFlagDefined (bindingFlags, BindingFlags.Public);
      else
        return IsFlagDefined (bindingFlags, BindingFlags.NonPublic);
    }

    public bool HasRightInstanceOrStaticFlag (MethodAttributes methodAttributes, BindingFlags bindingFlags)
    {
      if (!IsFlagDefined (bindingFlags, BindingFlags.Static) && !IsFlagDefined (bindingFlags, BindingFlags.Instance))
        return false;

      var isStatic = (methodAttributes & MethodAttributes.Static) == MethodAttributes.Static;
      if (isStatic)
        return IsFlagDefined (bindingFlags, BindingFlags.Static);
      else
        return IsFlagDefined (bindingFlags, BindingFlags.Instance);
    }

    private bool IsFlagDefined (BindingFlags bindingFlags, BindingFlags checkedFlag)
    {
      return (bindingFlags & checkedFlag) == checkedFlag;
    }
  }
}