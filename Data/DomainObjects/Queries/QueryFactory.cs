/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Data.Linq.SqlGeneration.SqlServer;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// Provides a central entry point to get instances of <see cref="IQuery"/> and <see cref="DomainObjectQueryable{T}"/> query objects. Use this 
  /// factory to create LINQ queries or to read queries from the <see cref="QueryConfiguration"/>.
  /// </summary>
  public static class QueryFactory
  {
    /// <summary>
    /// Creates a <see cref="DomainObjectQueryable{T}"/> used as the entry point to a LINQ query.
    /// </summary>
    /// <typeparam name="T">The <see cref="DomainObject"/> type to be queried.</typeparam>
    /// <param name="sqlGenerator">The <see cref="ISqlGenerator"/> object to be used for generating the query command string.</param>
    /// <returns>A <see cref="DomainObjectQueryable{T}"/> object as an entry point to a LINQ query.</returns>
    /// <remarks>
    /// Use this overload to explicitly specify the <see cref="ISqlGenerator"/> used to generate the query command string. To have the storage
    /// provider automatically supply the right generator, use the <see cref="CreateQueryable{T}()"/> overload.
    /// </remarks>
    /// <example>
    /// The following example creates a new instance of <see cref="SqlServerGenerator"/> and supplies it to 
    /// <see cref="CreateQueryable{T}(ISqlGenerator)"/>. The object returned by <see cref="CreateQueryable{T}(ISqlGenerator)"/> method is then used as 
    /// an entry point for a query that selects a number of <c>Order</c> objects, filters them by <c>OrderNumber</c>, and orders them by name of
    /// customer (which includes an implicit join between <c>Order</c> and <c>Customer</c> objects).
    /// <code>
    /// var generator = new SqlServerGenerator (new DatabaseInfo());
    /// var query =
    ///     from o in QueryFactory.CreateQueryable&lt;Order&gt; (generator)
    ///     where o.OrderNumber &lt;= 4
    ///     orderby o.Customer.Name
    ///     select o;
    /// var result = query.ToArray();
    /// </code>
    /// </example>
    public static DomainObjectQueryable<T> CreateQueryable<T> (ISqlGenerator sqlGenerator)
        where T : DomainObject
    {
      ArgumentUtility.CheckNotNull ("sqlGenerator", sqlGenerator);
      return new DomainObjectQueryable<T> (sqlGenerator);
    }

    /// <summary>
    /// Creates a <see cref="DomainObjectQueryable{T}"/> used as the entry point to a LINQ query.
    /// </summary>
    /// <typeparam name="T">The <see cref="DomainObject"/> type to be queried.</typeparam>
    /// <returns>A <see cref="DomainObjectQueryable{T}"/> object as an entry point to a LINQ query.</returns>
    /// <remarks>
    /// This overload uses the <see cref="ISqlGenerator"/> associated with <typeparamref name="T"/> via its <see cref="StorageProviderDefinition"/>.
    /// Use the <see cref="CreateQueryable{T}()"/> overload to explicitly specify an <see cref="ISqlGenerator"/> instance.
    /// </remarks>
    /// <example>
    /// The following example used <see cref="CreateQueryable{T}()"/> to retrieve 
    /// an entry point for a query that selects a number of <c>Order</c> objects, filters them by <c>OrderNumber</c>, and orders them by name of
    /// customer (which includes an implicit join between <c>Order</c> and <c>Customer</c> objects).
    /// <code>
    /// var generator = new SqlServerGenerator (new DatabaseInfo());
    /// var query =
    ///     from o in QueryFactory.CreateQueryable&lt;Order&gt; (generator)
    ///     where o.OrderNumber &lt;= 4
    ///     orderby o.Customer.Name
    ///     select o;
    /// var result = query.ToArray();
    /// </code>
    /// </example>
    public static DomainObjectQueryable<T> CreateQueryable<T> ()
        where T : DomainObject
    {
      return new DomainObjectQueryable<T> (GetDefaultSqlGenerator (typeof (T)));
    }

    /// <summary>
    /// Returns the default <see cref="ISqlGenerator"/> associated with the given <paramref name="domainObjectType"/>.
    /// </summary>
    /// <param name="domainObjectType">The <see cref="DomainObject"/> type whose <see cref="ISqlGenerator"/> should be retrieved.</param>
    /// <returns>The <see cref="ISqlGenerator"/> associated with the <paramref name="domainObjectType"/>'s storage provider.</returns>
    /// <remarks>
    /// Each <see cref="DomainObject"/> type is associated with a <see cref="StorageProviderDefinition"/>, which defines the storage provider
    /// used to load and store instances of the type. The <see cref="StorageProviderDefinition"/> also defines the <see cref="ISqlGenerator"/>
    /// instance used for querying instances of the <see cref="DomainObject"/> type. This method can be used to retrieve that 
    /// <see cref="ISqlGenerator"/> instance.
    /// </remarks>
    public static ISqlGenerator GetDefaultSqlGenerator (Type domainObjectType)
    {
      ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);
      var storageProviderID = MappingConfiguration.Current.ClassDefinitions.GetMandatory (domainObjectType).StorageProviderID;
      var storageProviderDefinition = DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions.GetMandatory (storageProviderID);
      return storageProviderDefinition.LinqSqlGenerator;
    }
  }
}