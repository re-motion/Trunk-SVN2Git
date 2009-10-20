// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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

namespace Remotion.Mixins.Context.Suppression
{
  /// <summary>
  /// Defines a rule suppressing specific mixin types in a mixin configuration.
  /// </summary>
  public interface IMixinSuppressionRule
  {
    /// <summary>
    /// Removes all mixins affected by this rule from the <paramref name="configuredMixinTypes"/> dictionary.
    /// </summary>
    /// <param name="configuredMixinTypes">The dictionary from which to remove the affected mixins.</param>
    void RemoveAffectedMixins (Dictionary<Type, MixinContext> configuredMixinTypes);
  }
}