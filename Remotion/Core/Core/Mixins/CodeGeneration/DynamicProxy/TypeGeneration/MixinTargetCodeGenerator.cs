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
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy.TypeGeneration
{
  /// <summary>
  /// Generates code needed to implement <see cref="IMixinTarget"/> on concrete mixed types.
  /// </summary>
  public class MixinTargetCodeGenerator
  {
    private static readonly PropertyInfo s_classContextProperty = typeof (IMixinTarget).GetProperty ("ClassContext");
    private static readonly MethodInfo s_classContextGetter = s_classContextProperty.GetGetMethod ();
    private static readonly PropertyInfo s_mixinProperty = typeof (IMixinTarget).GetProperty ("Mixins");
    private static readonly MethodInfo s_mixinsGetter = s_mixinProperty.GetGetMethod ();
    private static readonly PropertyInfo s_firstProperty = typeof (IMixinTarget).GetProperty ("FirstBaseCallProxy");
    private static readonly MethodInfo s_firstGetter = s_firstProperty.GetGetMethod ();

    private readonly string _targetClassName;
    private readonly FieldReference _classContextField;
    private readonly FieldReference _extensionsField;
    private readonly FieldReference _firstField;
    private readonly DebuggerDisplayAttributeGenerator _debuggerDisplayAttributeGenerator;

    public MixinTargetCodeGenerator (
        string targetClassName, 
        FieldReference classContextField, 
        FieldReference extensionsField, 
        FieldReference firstField, 
        DebuggerDisplayAttributeGenerator debuggerDisplayAttributeGenerator)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("targetClassName", targetClassName);
      ArgumentUtility.CheckNotNull ("classContextField", classContextField);
      ArgumentUtility.CheckNotNull ("extensionsField", extensionsField);
      ArgumentUtility.CheckNotNull ("firstField", firstField);
      ArgumentUtility.CheckNotNull ("debuggerDisplayAttributeGenerator", debuggerDisplayAttributeGenerator);

      _targetClassName = targetClassName;
      _classContextField = classContextField;
      _extensionsField = extensionsField;
      _firstField = firstField;
      _debuggerDisplayAttributeGenerator = debuggerDisplayAttributeGenerator;
    }

    public void ImplementIMixinTarget (IClassEmitter emitter)
    {
      ArgumentUtility.CheckNotNull ("emitter", emitter);

      CustomPropertyEmitter configurationProperty = emitter.CreateInterfacePropertyImplementation (s_classContextProperty);
      configurationProperty.GetMethod = emitter.CreateInterfaceMethodImplementation (s_classContextGetter);
      configurationProperty.ImplementWithBackingField (_classContextField);
      _debuggerDisplayAttributeGenerator.AddDebuggerDisplayAttribute (configurationProperty, "Class context for " + _targetClassName, "ClassContext");

      CustomPropertyEmitter mixinsProperty = emitter.CreateInterfacePropertyImplementation (s_mixinProperty);
      mixinsProperty.GetMethod = emitter.CreateInterfaceMethodImplementation (s_mixinsGetter);
      mixinsProperty.ImplementWithBackingField (_extensionsField);
      _debuggerDisplayAttributeGenerator.AddDebuggerDisplayAttribute (mixinsProperty, "Count = {__extensions.Length}", "Mixins");

      CustomPropertyEmitter firstProperty = emitter.CreateInterfacePropertyImplementation (s_firstProperty);
      firstProperty.GetMethod = emitter.CreateInterfaceMethodImplementation (s_firstGetter);
      firstProperty.ImplementWithBackingField (_firstField);
      _debuggerDisplayAttributeGenerator.AddDebuggerDisplayAttribute (firstProperty, "Generated proxy", "FirstBaseCallProxy");
    }
  }
}