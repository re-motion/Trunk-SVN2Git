using System;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Reflection.CodeGeneration
{
  public interface IClassEmitter : IAttributableEmitter
  {
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
    /// Creates a private method override, i.e. a method override with private visibility whose name includes the name of the base method's
    /// declaring type, similar to an explicit interface implementation.
    /// </summary>
    /// <param name="baseMethod">The base method to override.</param>
    /// <returns>A <see cref="CustomMethodEmitter"/> for the private method override.</returns>
    /// <remarks>This method can be useful when overriding several (shadowed) methods of the same name inherited by different base types.</remarks>
    CustomMethodEmitter CreatePrivateMethodOverride (MethodInfo baseMethod);

    CustomMethodEmitter CreateInterfaceMethodImplementation (MethodInfo interfaceMethod);

    /// <summary>
    /// Creates a public interface method implementation, i.e. an interface implementation with public visibility whose name equals the name
    /// of the interface method (like a C# implicit interface implementation).
    /// </summary>
    /// <param name="interfaceMethod">The interface method to implement.</param>
    /// <returns>A <see cref="CustomMethodEmitter"/> for the interface implementation.</returns>
    /// <remarks>The generated method has public visibility and the <see cref="MethodAttributes.NewSlot"/> flag set. This means that the method
    /// will shadow methods from the base type with the same name and signature, not override them. Use <see cref="CustomClassEmitter.CreatePrivateMethodOverride"/> to
    /// explicitly create an override for such a method.</remarks>
    CustomMethodEmitter CreatePublicInterfaceMethodImplementation (MethodInfo interfaceMethod);

    CustomPropertyEmitter CreatePropertyOverride (PropertyInfo baseProperty);
    CustomPropertyEmitter CreateInterfacePropertyImplementation (PropertyInfo interfaceProperty);
    CustomPropertyEmitter CreatePublicInterfacePropertyImplementation (PropertyInfo interfaceProperty);
    CustomEventEmitter CreateEventOverride (EventInfo baseEvent);
    CustomEventEmitter CreateInterfaceEventImplementation (EventInfo interfaceEvent);
    CustomEventEmitter CreatePublicInterfaceEventImplementation (EventInfo interfaceEvent);
    void ReplicateBaseTypeConstructors (params Statement[] postBaseCallInitializationStatements);
    
    MethodInfo GetPublicMethodWrapper (MethodInfo methodToBeWrapped);
    Type BuildType ();
    
    TypeBuilder TypeBuilder { get; }
    Type BaseType { get; }
  }
}