using System;
using System.Reflection;

namespace Remotion.Reflection
{
  public interface ITypeInformation : IMemberInformation
  {
    /// <summary>
    /// Gets the fully qualified name of the <see cref="Type"/>, including the namespace of the <see cref="Type"/> but not the assembly.
    /// </summary>
    /// <returns>
    /// The fully qualified name of the <see cref="Type"/>, including the namespace of the <see cref="Type"/> but not the assembly; 
    /// or <see langword="null"/> if the current instance represents a generic type parameter, an array type, pointer type, 
    /// or byref type based on a type parameter, or a generic type that is not a generic type definition but contains unresolved type parameters.
    /// </returns>
    string FullName { get; }

    /// <summary>
    /// Gets the namespace of the <see cref="Type"/>.
    /// </summary>
    /// <returns>
    /// The namespace of the <see cref="Type"/>, or <see langword="null"/> if the current instance represents a generic parameter.
    /// </returns>
    string Namespace { get; }

    /// <summary>
    /// Gets the assembly-qualified name of the <see cref="Type"/>, which includes the name of the assembly from which the <see cref="Type"/> was loaded.
    /// </summary>
    /// <returns>
    /// The assembly-qualified name of the <see cref="Type"/>, which includes the name of the assembly from which the <see cref="Type"/> was loaded, or <see langword="null"/> if the current instance represents a generic type parameter.
    /// </returns>
    string AssemblyQualifiedName { get; }

    /// <summary>
    /// Gets the <see cref="System.Reflection.Assembly"/> in which the type is declared. For generic types, gets the <see cref="System.Reflection.Assembly"/> in which the generic type is defined.
    /// </summary>
    /// <returns>
    /// An <see cref="System.Reflection.Assembly"/> instance that describes the assembly containing the current type. For generic types, the instance describes the assembly that contains the generic type definition, not the assembly that creates and uses a particular constructed type.
    /// </returns>
    Assembly Assembly { get; }

        /// <summary>
    /// Gets the type that declares the current nested type or generic type parameter.
    /// </summary>
    /// <returns>
    /// A <see cref="Type"/> object representing the enclosing type, if the current type is a nested type; or the generic type definition, 
    /// if the current type is a type parameter of a generic type; or the type that declares the generic method, 
    /// if the current type is a type parameter of a generic method; otherwise, <see langword="null"/>.
    /// </returns>
    new ITypeInformation DeclaringType { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is a class; that is, not a value type or interface.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is a class; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsClass { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is a value type.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is a value type; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsValueType { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is an interface; that is, not a class or a value type.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is an interface; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsInterface { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is an array.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is an array; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsArray { get; }

    /// <summary>
    /// Gets the number of dimensions in an <see cref="Array"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="Int32"/> containing the number of dimensions in the current Type.
    /// </returns>
    /// <exception cref="NotSupportedException">The functionality of this method is unsupported in the base class and must be implemented in a derived class instead.</exception>
    /// <exception cref="ArgumentException">The current Type is not an array.</exception>
    int GetArrayRank ();

    /// <summary>
    /// Returns a <see cref="Type"/> object representing an array of the current type, with the specified number of dimensions.
    /// </summary>
    /// <returns>
    /// A <see cref="Type"/> object representing an array of the current type, with the specified number of dimensions.
    /// </returns>
    /// <param name="rank">The number of dimensions for the array.</param>
    /// <exception cref="IndexOutOfRangeException"><paramref name="rank"/> is invalid. For example, 0 or negative.</exception>
    /// <exception cref="NotSupportedException">The invoked method is not supported in the base class.</exception>
    ITypeInformation MakeArrayType (int rank);

    /// <summary>
    /// Returns a <see cref="Type"/> object representing a one-dimensional array of the current type, with a lower bound of zero.
    /// </summary>
    /// <returns>
    /// A <see cref="Type"/> object representing a one-dimensional array of the current type, with a lower bound of zero.
    /// </returns>
    ITypeInformation MakeArrayType ();

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Type"/> represents an enumeration.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current <see cref="Type"/> represents an enumeration; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsEnum { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is a pointer.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is a pointer; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsPointer { get; }

    /// <summary>
    /// Returns a <see cref="Type"/> object that represents a pointer to the current type.
    /// </summary>
    /// <returns>
    /// A <see cref="Type"/> object that represents a pointer to the current type.
    /// </returns>
    /// <exception cref="NotSupportedException">The invoked method is not supported in the base class.</exception>
    ITypeInformation MakePointerType ();

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is passed by reference.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is passed by reference; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsByRef { get; }

    /// <summary>
    /// Returns a <see cref="Type"/> object that represents the current type when passed as a ref parameter.
    /// </summary>
    /// <returns>
    /// A <see cref="Type"/> object that represents the current type when passed as a ref parameter.
    /// </returns>
    /// <exception cref="NotSupportedException">The invoked method is not supported in the base class.</exception>
    ITypeInformation MakeByRefType ();

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is declared sealed.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is declared sealed; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsSealed { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is abstract and must be overridden.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is abstract; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsAbstract { get; }

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Type"/> object represents a type whose definition is nested inside the definition of another type.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is nested inside another type; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsNested { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is serializable.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is serializable; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsSerializable { get; }

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Type"/> encompasses or refers to another type; 
    /// that is, whether the current <see cref="Type"/> is an array, a pointer, or is passed by reference.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is an array, a pointer, or is passed by reference; otherwise, <see langword="false"/>.
    /// </returns>
    bool HasElementType { get; }

    /// <summary>
    /// When overridden in a derived class, returns the <see cref="Type"/> of the object encompassed or referred to by the current array, pointer or reference type.
    /// </summary>
    /// <returns>
    /// The <see cref="Type"/> of the object encompassed or referred to by the current array, pointer, or reference type, 
    /// or <see langword="null"/> if the current <see cref="Type"/> is not an array or a pointer, or is not passed by reference, 
    /// or represents a generic type or a type parameter in the definition of a generic type or generic method.
    /// </returns>
    ITypeInformation GetElementType ();

    /// <summary>
    /// Gets a value indicating whether the current type is a generic type.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current type is a generic type; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsGenericType { get; }

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Type"/> represents a generic type definition, from which other generic types can be constructed.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> object represents a generic type definition; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsGenericTypeDefinition { get; }

    /// <summary>
    /// Returns a <see cref="Type"/> object that represents a generic type definition from which the current generic type can be constructed.
    /// </summary>
    /// <returns>
    /// A <see cref="Type"/> object representing a generic type from which the current type can be constructed.
    /// </returns>
    /// <exception cref="InvalidOperationException">The current type is not a generic type. That is, <see cref="IsGenericType"/> returns <see langword="false"/>.</exception>
    /// <exception cref="NotSupportedException">The invoked method is not supported in the base class. Derived classes must provide an implementation.</exception>
    ITypeInformation GetGenericTypeDefinition ();

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Type"/> object has type parameters that have not been replaced by specific types.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> object is itself a generic type parameter or has type parameters for which specific types have not been supplied; otherwise, <see langword="false"/>.
    /// </returns>
    bool ContainsGenericParameters { get; }

    /// <summary>
    /// Returns an array of <see cref="Type"/> objects that represent the type arguments of a generic type or the type parameters of a generic type definition.
    /// </summary>
    /// <returns>
    /// An array of <see cref="Type"/> objects that represent the type arguments of a generic type. Returns an empty array if the current type is not a generic type.
    /// </returns>
    ITypeInformation[] GetGenericArguments ();

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Type"/> represents a type parameter in the definition of a generic type or method.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> object represents a type parameter of a generic type definition or generic method definition; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsGenericParameter { get; }

    /// <summary>
    /// Gets the position of the type parameter in the type parameter list of the generic type or method that declared the parameter, 
    /// when the <see cref="Type"/> object represents a type parameter of a generic type or a generic method.
    /// </summary>
    /// <returns>
    /// The position of a type parameter in the type parameter list of the generic type or method that defines the parameter. Position numbers begin at 0.
    /// </returns>
    /// <exception cref="InvalidOperationException">The current type does not represent a type parameter. That is, <see cref="IsGenericParameter"/> returns <see langword="false"/>.</exception>
    int GenericParameterPosition { get; }

    /// <summary>
    /// Returns an array of <see cref="Type"/> objects that represent the constraints on the current generic type parameter. 
    /// </summary>
    /// <returns>
    /// An array of <see cref="Type"/> objects that represent the constraints on the current generic type parameter.
    /// </returns>
    /// <exception cref="InvalidOperationException">The current <see cref="Type"/> object is not a generic type parameter. That is, the <see cref="IsGenericParameter"/> property returns <see langword="false"/>.</exception>
    ITypeInformation[] GetGenericParameterConstraints ();

    /// <summary>
    /// Gets a combination of <see cref="GenericParameterAttributes"/> flags that describe the covariance and special constraints of the current generic type parameter. 
    /// </summary>
    /// <returns>
    /// A bitwise combination of <see cref="GenericParameterAttributes"/> values that describes the covariance and special constraints of the current generic type parameter.
    /// </returns>
    /// <exception cref="InvalidOperationException">The current <see cref="Type"/> object is not a generic type parameter. That is, the <see cref="IsGenericParameter"/> property returns <see langword="false"/>.</exception>
    /// <exception cref="NotSupportedException">The invoked method is not supported in the base class.</exception>
    GenericParameterAttributes GenericParameterAttributes { get; }
  }
}