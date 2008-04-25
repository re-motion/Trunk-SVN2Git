using System;
using System.Reflection;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  /// <summary>
  /// Provides information about a property of a bindable object and offers a way to get or set the property's value.
  /// </summary>
  public interface IPropertyInformation
  {
    /// <summary>
    /// Gets the type of the property, i.e. the type of values the property can store.
    /// </summary>
    /// <value>The type of the property.</value>
    Type PropertyType { get; }
    
    /// <summary>
    /// Gets the simple name of the property identifying it within its declaring type.
    /// </summary>
    /// <value>The simple property name.</value>
    string Name { get; }

    /// <summary>
    /// Gets the type declaring the property.
    /// </summary>
    /// <value>The declaring type of the property.</value>
    Type DeclaringType { get; }

    /// <summary>
    /// Gets the type the property was originally declared on.
    /// </summary>
    /// <returns>The type the property was originally declared on.</returns>
    /// <remarks>If the property represented by this instance overrides a property from a base type, this method will return the base type.</remarks>
    Type GetOriginalDeclaringType ();

    /// <summary>
    /// Determines whether the property can be set from the outside.
    /// </summary>
    /// <value>True if this instance has can be set from the outside; otherwise, false.</value>
    bool CanBeSetFromOutside { get; }

    /// <summary>
    /// Gets the one custom attribute of type <typeparamref name="T"/> declared on this property, or null if no such attribute exists.
    /// </summary>
    /// <typeparam name="T">The type of attribute to retrieve.</typeparam>
    /// <param name="inherited">If set to true, the inheritance hierarchy is searched for the attribute. Otherwise, only the <see cref="DeclaringType"/>
    /// is checked.</param>
    /// <exception cref="AmbiguousMatchException">More than one instance of the given attribute type <typeparamref name="T"/> is declared on this
    /// property.</exception>
    /// <returns>An instance of type <typeparamref name="T"/>, or <see langword="null"/> if no attribute of that type is declared on this property.</returns>
    T GetCustomAttribute<T> (bool inherited) where T : class;

    /// <summary>
    /// Gets the custom attributes of type <typeparamref name="T"/> declared on this property, or null if no such attribute exists.
    /// </summary>
    /// <typeparam name="T">The type of the attributes to retrieve.</typeparam>
    /// <param name="inherited">If set to true, the inheritance hierarchy is searched for the attributes. Otherwise, only the <see cref="DeclaringType"/>
    /// is checked.</param>
    /// <returns>An array of the attributes of type <typeparamref name="T"/> declared on this property, or an empty array if no attribute of
    /// that type is declared on this property.</returns>
    T[] GetCustomAttributes<T> (bool inherited) where T : class;

    /// <summary>
    /// Determines whether a custom attribute of the specified type <typeparamref name="T"/> is defined on the property.
    /// </summary>
    /// <typeparam name="T">The type of attribute to search for.</typeparam>
    /// <param name="inherited">If set to true, the inheritance hierarchy is searched for the attribute. Otherwise, only the <see cref="DeclaringType"/>
    /// is checked.</param>
    /// <returns>
    /// True if a custom attribute of the specified type is defined on the property; otherwise, false.
    /// </returns>
    bool IsDefined<T> (bool inherited) where T : class;

    /// <summary>
    /// Gets the value of the property for the given instance.
    /// </summary>
    /// <param name="instance">The instance to retrieve the value for, or <see langword="null"/> for a static property.</param>
    /// <param name="indexParameters">The index parameters to be used for property value retrieval.</param>
    /// <returns>The property's value for the given instance.</returns>
    /// <exception cref="ArgumentException">
    /// <para>The type of the elements of the <paramref name="indexParameters"/> array does not match the index argument types expected by the
    /// property.</para>
    /// <para>-or-</para>
    /// <para>The get accessor cannot be found.</para>
    /// </exception>
    /// <exception cref="TargetException">The <paramref name="instance"/> parameter is <see langword="null"/> although the property is not a static
    /// property or it does not match the property's declaring type.</exception>
    /// <exception cref="TargetParameterCountException">The number of items in the <paramref name="indexParameters"/> array does not match the number
    /// of index parameters expected by the property.</exception>
    /// <exception cref="TargetInvocationException">The property's get method throw an exception, see the <see cref="Exception.InnerException"/>
    /// property.</exception>
    /// <exception cref="MethodAccessException">The accessor was private or protected and could not be executed.</exception>
    object GetValue (object instance, object[] indexParameters);

    /// <summary>
    /// Sets the value of the property for the given instance.
    /// </summary>
    /// <param name="instance">The instance to set the value for, or <see langword="null"/> for a static property.</param>
    /// <param name="value">The property's value for the given instance.</param>
    /// <param name="indexParameters">The index parameters to be used for setting the property value.</param>
    /// <exception cref="ArgumentException">
    /// <para>The type of the elements of the <paramref name="indexParameters"/> array does not match the index argument types expected by the
    /// property.</para>
    /// <para>-or-</para>
    /// <para>The set accessor cannot be found.</para>
    /// </exception>
    /// <exception cref="TargetException">The <paramref name="instance"/> parameter is <see langword="null"/> although the property is not a static
    /// property or it does not match the property's declaring type.</exception>
    /// <exception cref="TargetParameterCountException">The number of items in the <paramref name="indexParameters"/> array does not match the number
    /// of index parameters expected by the property.</exception>
    /// <exception cref="TargetInvocationException">The property's get method throw an exception, see the <see cref="Exception.InnerException"/>
    /// property.</exception>
    /// <exception cref="MethodAccessException">The accessor was private or protected and could not be executed.</exception>
    void SetValue (object instance, object value, object[] indexParameters);
  }
}