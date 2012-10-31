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
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  /// <summary>
  /// Analyzes <see cref="IHasCompleteInterface{TInterface}"/> markers implemented by a type and applies the respective configuration information
  /// to the <see cref="MixinConfigurationBuilder"/>.
  /// </summary>
  public class HasCompleteInterfaceMarkerAnalyzer : IMixinDeclarationAnalyzer<Type>
  {
    public void Analyze (Type type, MixinConfigurationBuilder configurationBuilder)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("configurationBuilder", configurationBuilder);

      var completeInterfaceMarkers = (from ifc in type.GetInterfaces()
                                      where ifc.IsGenericType
                                      let genericTypeDef = ifc.GetGenericTypeDefinition()
                                      where genericTypeDef == typeof (IHasCompleteInterface<>)
                                      let completeInterfaceType = ifc.GetGenericArguments ().Single ()
                                      where !completeInterfaceType.ContainsGenericParameters
                                      select completeInterfaceType).ToArray();

      if (completeInterfaceMarkers.Length > 0)
        configurationBuilder.ForClass (type).AddCompleteInterfaces (completeInterfaceMarkers);
    }
  }
}
