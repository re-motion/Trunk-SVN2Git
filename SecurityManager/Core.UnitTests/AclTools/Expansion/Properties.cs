using System;
using System.Linq.Expressions;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  /// <summary>
  /// Allows the creation of the property of a class independent of a concrete class instance,
  /// in the form of an <see cref="Property{TClass,TProperty}"/> object.
  /// </summary>
  /// <remarks>
  /// The resulting <see cref="Property{TClass,TProperty}"/> can then e.g. be passed to
  /// a method which can use it to access the property of arbitrary class instances.
  /// </remarks>
  /// <example>
  /// <code>
  /// <![CDATA[
  /// void PropertyTest (ObjectID tenantId)
  /// {
  ///   User[] users = User.FindByTenantID (tenantId).ToArray ();
  ///   IsUserPropertyNull (users, Properties<User>.Get (x => x.UserName));
  ///   IsUserPropertyNull (users, Properties<User>.Get (x => x.OwningGroup));
  ///   IsUserPropertyNull (users, Properties<User>.Get (x => x.Tenant));
  /// }
  ///
  /// bool IsUserPropertyNull<T> (User[] users, Property<User, T> userProperty) where T : class
  /// {
  ///   foreach (var user in users)
  ///   {
  ///     var propertyValue = userProperty.Get (user);
  ///     if (propertyValue == null)
  ///     {
  ///       return true;
  ///     }
  ///   }
  ///   return false;
  /// }   
  /// ]]>
  /// </code>
  /// </example>
  /// <typeparam name="T">The class for which we want to create the <see cref="Property{TClass,TProperty}"/> object.</typeparam>
  class Properties<T>
  {
    public static Property<T, R> Get<R> (Expression<Func<T, R>> lambda)
    {
      return new Property<T, R> (lambda);
    }
  }
}