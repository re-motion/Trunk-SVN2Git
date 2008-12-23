// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Reflection.CodeGeneration
{
  public interface IClassEmitter : IAttributableEmitter
  {
    TypeBuilder TypeBuilder { get; }
    Type BaseType { get; }

    ConstructorEmitter CreateConstructor (ArgumentReference[] arguments);
    ConstructorEmitter CreateConstructor (Type[] arguments);
    void CreateDefaultConstructor ();
    ConstructorEmitter CreateTypeConstructor ();
    FieldReference CreateField (string name, Type fieldType);
    FieldReference CreateField (string name, Type fieldType, FieldAttributes attributes);
    FieldReference CreateStaticField (string name, Type fieldType);
    FieldReference CreateStaticField (string name, Type fieldType, FieldAttributes attributes);
    CustomMethodEmitter CreateMethod (string name, MethodAttributes attributes);
    CustomPropertyEmitter CreateProperty (string name, PropertyKind propertyKind, Type propertyType);

    CustomPropertyEmitter CreateProperty (
        string name, PropertyKind propertyKind, Type propertyType, Type[] indexParameters, PropertyAttributes attributes);

    CustomEventEmitter CreateEvent (string name, EventKind eventKind, Type eventType, EventAttributes attributes);
    CustomEventEmitter CreateEvent (string name, EventKind eventKind, Type eventType);
    CustomMethodEmitter CreateMethodOverride (MethodInfo baseMethod);

    /// <summary>
    /// Creates a full-named method override, i.e. a method override with the same visibility as whose name includes the name of the base method's
    /// declaring type, similar to an explicit interface implementation.
    /// </summary>
    /// <param name="baseMethod">The base method to override.</param>
    /// <returns>A <see cref="CustomMethodEmitter"/> for the full-named method override.</returns>
    /// <remarks>This method can be useful when overriding several (shadowed) methods of the same name inherited by different base types.</remarks>
    CustomMethodEmitter CreateFullNamedMethodOverride (MethodInfo baseMethod);

    CustomMethodEmitter CreateInterfaceMethodImplementation (MethodInfo interfaceMethod);

    /// <summary>
    /// Creates a public interface method implementation, i.e. an interface implementation with public visibility whose name equals the name
    /// of the interface method (like a C# implicit interface implementation).
    /// </summary>
    /// <param name="interfaceMethod">The interface method to implement.</param>
    /// <returns>A <see cref="CustomMethodEmitter"/> for the interface implementation.</returns>
    /// <remarks>The generated method has public visibility and the <see cref="MethodAttributes.NewSlot"/> flag set. This means that the method
    /// will shadow methods from the base type with the same name and signature, not override them. Use <see cref="CreateFullNamedMethodOverride"/> to
    /// explicitly create an override for such a method.</remarks>
    CustomMethodEmitter CreatePublicInterfaceMethodImplementation (MethodInfo interfaceMethod);

    CustomPropertyEmitter CreatePropertyOverride (PropertyInfo baseProperty);
    CustomPropertyEmitter CreateInterfacePropertyImplementation (PropertyInfo interfaceProperty);
    CustomPropertyEmitter CreatePublicInterfacePropertyImplementation (PropertyInfo interfaceProperty);
    CustomEventEmitter CreateEventOverride (EventInfo baseEvent);
    CustomEventEmitter CreateInterfaceEventImplementation (EventInfo interfaceEvent);
    CustomEventEmitter CreatePublicInterfaceEventImplementation (EventInfo interfaceEvent);

    /// <summary>
    /// Creates a nested class within the type emitted by this <see cref="IClassEmitter"/>.
    /// </summary>
    /// <param name="typeName">The name of the nested type.</param>
    /// <param name="baseType">The base type of the nested type.</param>
    /// <param name="interfaces">The interfaces to be implemented by the nested type.</param>
    /// <returns>A new <see cref="IClassEmitter"/> for the nested class.</returns>
    IClassEmitter CreateNestedClass (string typeName, Type baseType, Type[] interfaces);

    void ReplicateBaseTypeConstructors (Action<ConstructorEmitter> preStatementsAdder, Action<ConstructorEmitter> postStatementsAdder);
    
    MethodInfo GetPublicMethodWrapper (MethodInfo methodToBeWrapped);
    Type BuildType ();
  }
}