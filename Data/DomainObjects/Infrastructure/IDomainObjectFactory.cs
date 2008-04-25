using System;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Public interface for a factory creating instances of domain objects.
  /// </summary>
  /// <remarks>
  /// This interface is used internally by <see cref="DomainObject.NewObject"/> and will likely not be used directly.
  /// If a factory does need to be accessed directly,
  /// <see cref="Remotion.Data.DomainObjects.Configuration.DomainObjectsConfiguration"/> can be used to access the currently used implementation
  /// of this interface.
  /// </remarks>
  public interface IDomainObjectFactory
  {
    /// <summary>
    /// Creates an interceptable type compatible to a given domain object base type.
    /// </summary>
    /// <param name="baseType">The domain object type the interceptable type must be compatible to.</param>
    /// <returns>An interceptable type compatible to the <paramref name="baseType"/>.</returns>
    /// <remarks><para>This method dynamically creates a subclass that intercepts certain method calls in order to perform management tasks.
    /// Using it ensures that the created domain object supports the new property syntax.</para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="baseType"/> argument is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="baseType"/> argument is sealed, contains abstract methods (apart from automatic
    /// properties), or is not derived from <see cref="DomainObject"/>.</exception>
    /// <exception cref="MappingException">The given <paramref name="baseType"/> is not part of the mapping.</exception>
    Type GetConcreteDomainObjectType (Type baseType);

    /// <summary>
    /// Gets a domain object type assignable to the given base type which intercepts property calls.
    /// </summary>
    /// <param name="baseTypeClassDefinition">The base domain object type whose properties should be intercepted.</param>
    /// <param name="concreteBaseType">The base domain object type whose properties should be intercepted.</param>
    /// <returns>A domain object type which intercepts property calls.</returns>
    /// <exception cref="ArgumentNullException">One of the parameters passed to this method is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentTypeException"><paramref name="concreteBaseType"/> cannot be assigned to the type specified by <paramref name="baseTypeClassDefinition"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="baseTypeClassDefinition"/> denotes an abstract, non-instantiable type.</exception>
    Type GetConcreteDomainObjectType (ClassDefinition baseTypeClassDefinition, Type concreteBaseType);

    /// <summary>
    /// Checkes whether a given domain object type was created by this factory implementation (but not necessarily the same factory instance).
    /// </summary>
    /// <param name="type">The type to be checked.</param>
    /// <returns>True if <paramref name="type"/> was created by a factory of the same implementation, else false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter was null.</exception>
    bool WasCreatedByFactory (Type type);

    /// <summary>
    /// Returns a construction object that can be used to instantiate objects of a given interceptable type.
    /// </summary>
    /// <typeparam name="TMinimal">The type statically returned by the construction object.</typeparam>
    /// <param name="type">The exatct interceptable type to be constructed; this must be a type returned by <see cref="GetConcreteDomainObjectType(Type)"/>.
    /// <typeparamref name="TMinimal"/> must be assignable from this type.</param>
    /// <returns>A construction object, which instantiates <paramref name="type"/> and returns <typeparamref name="TMinimal"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> argument is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="type"/> is not the same or a subtype of <typeparamref name="TMinimal"/> or
    /// <paramref name="type"/> wasn't created by this kind of factory.</exception>
    IFuncInvoker<TMinimal> GetTypesafeConstructorInvoker<TMinimal> (Type type) where TMinimal : DomainObject;

    /// <summary>
    /// Prepares an instance which has not been created via <see cref="GetTypesafeConstructorInvoker{TMinimal}"/> for use.
    /// </summary>
    /// <param name="instance">The instance to be prepared</param>
    /// <remarks>
    /// If an instance is constructed without a constructor call, e.g. by using
    /// <see cref="System.Runtime.Serialization.FormatterServices.GetSafeUninitializedObject"/>, this method can be used to have the factory
    /// perform any initialization work it would otherwise have performed via the constructor.
    /// </remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="instance"/> argument is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type created by this kind of factory.</exception>
    void PrepareUnconstructedInstance (DomainObject instance);
  }
}
