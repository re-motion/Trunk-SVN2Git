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

namespace JetBrains.Annotations
{
  /// <summary>
  /// Indicates that the marked symbol is used implicitly (e.g. via reflection, in external library),
  /// so this symbol will not be marked as unused (as well as by other usage inspections)
  /// </summary>
  [AttributeUsage (AttributeTargets.All, AllowMultiple = false, Inherited = true)]
  public sealed class UsedImplicitlyAttribute : Attribute
  {
    [UsedImplicitly]
    public UsedImplicitlyAttribute ()
        : this (ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default) { }

    [UsedImplicitly]
    public UsedImplicitlyAttribute (ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
    {
      UseKindFlags = useKindFlags;
      TargetFlags = targetFlags;
    }

    [UsedImplicitly]
    public UsedImplicitlyAttribute (ImplicitUseKindFlags useKindFlags)
        : this (useKindFlags, ImplicitUseTargetFlags.Default) { }

    [UsedImplicitly]
    public UsedImplicitlyAttribute (ImplicitUseTargetFlags targetFlags)
        : this (ImplicitUseKindFlags.Default, targetFlags) { }

    [UsedImplicitly]
    public ImplicitUseKindFlags UseKindFlags { get; private set; }

    /// <summary>
    /// Gets value indicating what is meant to be used
    /// </summary>
    [UsedImplicitly]
    public ImplicitUseTargetFlags TargetFlags { get; private set; }
  }
}