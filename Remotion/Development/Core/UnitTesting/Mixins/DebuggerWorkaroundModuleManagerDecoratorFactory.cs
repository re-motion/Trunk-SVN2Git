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
using System.Reflection.Emit;
using Remotion.Diagnostics;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;

namespace Remotion.Development.UnitTesting.Mixins
{
  /// <summary>
  /// Implements <see cref="IModuleManagerFactory"/> by creating instances of <see cref="DebuggerWorkaroundModuleManagerDecorator"/>. This can be used
  /// as a workaround for the Reflection.Emit bug where calls to <see cref="TypeBuilder.CreateType"/> take a very long time to complete 
  /// when the debugger is attached and a large number of types is generated into the same AssemblyBuilder.
  /// </summary>
  public class DebuggerWorkaroundModuleManagerDecoratorFactory : IModuleManagerFactory
  {
    private readonly int _maximumTypesPerAssembly;
    private readonly IModuleManagerFactory _decoratedFactory;
    private readonly IDebuggerInterface _debuggerInterface;

    /// <summary>
    /// Initializes a new instance of the <see cref="DebuggerWorkaroundModuleManagerDecoratorFactory"/> class.
    /// </summary>
    /// <param name="maximumTypesPerAssembly">Specify the number of types after which a new assembly should be created. This depends on your concrete 
    /// use cases, but you might try values such as 10, 30, or 50. Higher values mean less memory usage, lower numbers mean less time spent by 
    /// Reflection.Emit in <see cref="TypeBuilder.CreateType"/> when the debugger is attached.</param>
    /// <param name="debuggerInterface">Specify an instance of <see cref="DebuggerInterface"/>, or let your IoC container resolve the 
    /// <see cref="IDebuggerInterface"/> with the default configuration (which defaults to <see cref="DebuggerInterface"/>).</param>
    /// <param name="decoratedFactory">Specify an instance of <see cref="ModuleManagerFactory"/>, or a custom implementation of 
    /// <see cref="IModuleManagerFactory"/>.</param>
    public DebuggerWorkaroundModuleManagerDecoratorFactory (
        int maximumTypesPerAssembly, IDebuggerInterface debuggerInterface, IModuleManagerFactory decoratedFactory)
    {
      _maximumTypesPerAssembly = maximumTypesPerAssembly;
      _debuggerInterface = debuggerInterface;
      _decoratedFactory = decoratedFactory;
    }

    public IModuleManager Create ()
    {
      return new DebuggerWorkaroundModuleManagerDecorator (_maximumTypesPerAssembly, _debuggerInterface, _decoratedFactory.Create ());
    }
  }
}