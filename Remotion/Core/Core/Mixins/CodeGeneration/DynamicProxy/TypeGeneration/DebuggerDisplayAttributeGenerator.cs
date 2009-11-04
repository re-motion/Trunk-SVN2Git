// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy.TypeGeneration
{
  /// <summary>
  /// Generates the <see cref="DebuggerDisplayAttribute"/> on the given emitted members or classes.
  /// </summary>
  public class DebuggerDisplayAttributeGenerator
  {
    private static readonly ConstructorInfo s_debuggerDisplayAttributeConstructor =
      typeof (DebuggerDisplayAttribute).GetConstructor (new[] { typeof (string) });
    private static readonly PropertyInfo s_debuggerDisplayNameProperty = typeof (DebuggerDisplayAttribute).GetProperty ("Name");

    public void AddDebuggerDisplayAttribute (IAttributableEmitter emitter, string displayString)
    {
      ArgumentUtility.CheckNotNull ("emitter", emitter);
      ArgumentUtility.CheckNotNull ("displayString", displayString);

      var attributeBuilder = new CustomAttributeBuilder (s_debuggerDisplayAttributeConstructor, new object[] { displayString });
      emitter.AddCustomAttribute (attributeBuilder);
    }

    public void AddDebuggerDisplayAttribute (IAttributableEmitter emitter, string displayString, string nameString)
    {
      ArgumentUtility.CheckNotNull ("emitter", emitter);
      ArgumentUtility.CheckNotNull ("displayString", displayString);
      ArgumentUtility.CheckNotNull ("nameString", nameString);

      var attributeBuilder = new CustomAttributeBuilder (
          s_debuggerDisplayAttributeConstructor,
          new object[] { displayString }, 
          new[] { s_debuggerDisplayNameProperty }, 
          new object[] { nameString });
      emitter.AddCustomAttribute (attributeBuilder);
    }
  }
}
