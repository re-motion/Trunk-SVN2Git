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
using System;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Allows a property to be redirected in the scope of LINQ queries.
  /// </summary>
  /// <remarks>
  /// Usually, LINQ queries can only be performed on properties that are mapped to a database columns. Trying to use them on
  /// columns marked with the <see cref="StorageClassNoneAttribute"/> will cause an exception. Sometimes, however, it can be
  /// useful to enable LINQ queries on such properties if they can be redirected to another property that is mapped to a column.
  /// That way, a public unmapped property that acts as a wrapper for a protected mapped property can still be used in queries.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class LinqPropertyRedirectionAttribute : Attribute, IMethodCallTransformerAttribute
  {
    /// <summary>
    /// Implements the transformations required to map a member onto another property.
    /// </summary>
    public class MethodCallTransformer : IMethodCallTransformer
    {
      private readonly PropertyInfo _mappedProperty;

      public MethodCallTransformer (PropertyInfo mappedProperty)
      {
        ArgumentUtility.CheckNotNull ("mappedProperty", mappedProperty);
        _mappedProperty = mappedProperty;
      }

      public PropertyInfo MappedProperty
      {
        get { return _mappedProperty; }
      }

      public Expression Transform (MethodCallExpression methodCallExpression)
      {
        ArgumentUtility.CheckNotNull ("methodCallExpression", methodCallExpression);

        throw new NotImplementedException ();
      }
    }


    /// <summary>
    /// Gets the target property the given <see cref="PropertyInfo"/> redirects to, or the given <see cref="PropertyInfo"/> if no
    /// <see cref="LinqPropertyRedirectionAttribute"/> is specified.
    /// </summary>
    /// <param name="propertyInfo">The property info whose target property should be retrieved.</param>
    /// <returns>The target property if the given <paramref name="propertyInfo"/> has the <see cref="LinqPropertyRedirectionAttribute"/> applied; 
    /// otherwise, the <paramref name="propertyInfo"/> itself. The target property is evaluated recursively, i.e., if the target it itself redirected,
    /// the target's target is returned, and so on.
    /// </returns>
    public static PropertyInfo GetTargetProperty (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      LinqPropertyRedirectionAttribute attribute;

      while ((attribute = AttributeUtility.GetCustomAttribute<LinqPropertyRedirectionAttribute> (propertyInfo, false)) != null)
      {
        PropertyInfo redirected;
        try
        {
          redirected = attribute.GetMappedProperty ();
        }
        catch (MappingException ex)
        {
          var message = string.Format ("'{0}.{1}': {2}", propertyInfo.DeclaringType, propertyInfo.Name, ex.Message);
          throw new MappingException (message, ex);
        }

        if (redirected.Equals (propertyInfo))
        {
          var message = string.Format ("'{0}.{1}': The member redirects LINQ queries to itself.", propertyInfo.DeclaringType, propertyInfo.Name);
          throw new MappingException (message);
        }

        if (redirected.PropertyType != propertyInfo.PropertyType)
        {
          var message = string.Format (
              "'{0}.{1}': The member redirects LINQ queries to the property '{2}.{3}', which has a different return type.",
              propertyInfo.DeclaringType, 
              propertyInfo.Name,
              redirected.DeclaringType,
              redirected.Name);
          throw new MappingException (message);
        }

        propertyInfo = redirected;
      }

      return propertyInfo;
    }

    private readonly Type _declaringType;
    private readonly string _mappedPropertyName;

    /// <summary>
    /// Initializes a new instance of the <see cref="LinqPropertyRedirectionAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">The declaring type of the property to which the attribute's target is redirected. The property must
    /// exist within the <see cref="DomainObject"/> holding the redirected attribute (or one of its persistent mixins).</param>
    /// <param name="mappedPropertyName">The name of the property to which the attribute's target is redirected.</param>
    public LinqPropertyRedirectionAttribute (Type declaringType, string mappedPropertyName)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("mappedPropertyName", mappedPropertyName);

      _declaringType = declaringType;
      _mappedPropertyName = mappedPropertyName;
    }

    /// <summary>
    /// Gets the declaring type of the property to which the attribute's target is redirected
    /// </summary>
    /// <value>The declaring type of the property to which the attribute's target is redirected.</value>
    public Type DeclaringType
    {
      get { return _declaringType; }
    }

    /// <summary>
    /// Gets the name of the property to which the attribute's target is redirected.
    /// </summary>
    /// <value>The name of the property to which the attribute's target is redirected.</value>
    public string MappedPropertyName
    {
      get { return _mappedPropertyName; }
    }

    /// <summary>
    /// Gets the property to which the attribute's target is redirected, throwing an exception if the property does not exist.
    /// </summary>
    /// <returns>The property to which the attribute's target is redirected.</returns>
    /// <exception cref="MappingException">The property does not exist.</exception>
    public PropertyInfo GetMappedProperty ()
    {
      var mappedProperty = DeclaringType.GetProperty (MappedPropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      if (mappedProperty == null)
      {
        var message = string.Format ("The member redirects LINQ queries to '{0}.{1}', which does not exist.", DeclaringType, MappedPropertyName);
        throw new MappingException (message);
      }
      return mappedProperty;
    }

    public IMethodCallTransformer GetTransformer ()
    {
      PropertyInfo mappedProperty;
      try
      {
        mappedProperty = GetMappedProperty ();
      }
      catch (MappingException ex)
      {
        throw new InvalidOperationException (ex.Message, ex);
      }
      
      return new MethodCallTransformer (mappedProperty);
    }
  }
}