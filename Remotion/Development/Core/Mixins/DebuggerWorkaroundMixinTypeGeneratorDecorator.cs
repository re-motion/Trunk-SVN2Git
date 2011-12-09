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
using Remotion.Mixins.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Development.Mixins
{
  /// <summary>
  /// Decorates <see cref="IMixinTypeGenerator"/> in order to notify <see cref="DebuggerWorkaroundModuleManagerDecorator"/> when the type is actually 
  /// generated.
  /// </summary>
  public class DebuggerWorkaroundMixinTypeGeneratorDecorator : IMixinTypeGenerator
  {
    private readonly IMixinTypeGenerator _innerMixinTypeGenerator;
    private readonly DebuggerWorkaroundModuleManagerDecorator _moduleManager;

    public DebuggerWorkaroundMixinTypeGeneratorDecorator (IMixinTypeGenerator innerMixinTypeGenerator, DebuggerWorkaroundModuleManagerDecorator moduleManager)
    {
      ArgumentUtility.CheckNotNull ("innerMixinTypeGenerator", innerMixinTypeGenerator);
      ArgumentUtility.CheckNotNull ("moduleManager", moduleManager);

      _innerMixinTypeGenerator = innerMixinTypeGenerator;
      _moduleManager = moduleManager;
    }

    public ConcreteMixinType GetBuiltType ()
    {
      using (StopwatchScope.CreateScope ((ctx, scope) => _moduleManager.TypeGenerated (scope.ElapsedTotal)))
      {
        return _innerMixinTypeGenerator.GetBuiltType ();
      }
    }
  }
}