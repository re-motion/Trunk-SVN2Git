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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides an indexing mechanism to easily access a <see cref="DomainObject">DomainObject's</see> property in different
  /// <see cref="ClientTransaction">ClientTransactions</see>.
  /// </summary>
  /// <example>
  /// <code>
  /// [DBTable]
  /// [Instantiable]
  /// public abstract class MyDomainObject : DomainObject
  /// {
  ///   [DBColumn]
  ///   public abstract string Data { get; set; } // accessor for the current transaction, defines the property
  /// 
  ///   [StorageClassNone]
  ///   public TransactionalAccessor&lt;string&gt; DataTx // accessor for arbitrary transactions
  ///   {
  ///     get { return GetTransactionalAccessor&lt;string&gt; (Properties[typeof (MyDomainObject), "Data"]); }
  ///   }
  /// 
  ///   public static void Test (MyDomainObject obj)
  ///   {
  ///     ClientTransaction transaction1 = ClientTransaction.CreateRootTransaction();
  ///     ClientTransaction transaction2 = ClientTransaction.CreateRootTransaction();
  ///   
  ///     transaction1.EnlistDomainObject (obj);
  ///     transaction2.EnlistDomainObject (obj);
  /// 
  ///     obj.DataTx[transaction1] = "One";
  ///     obj.DataTx[transaction2] = "Two";
  /// 
  ///     Console.WriteLine (obj.DataTx[transaction1]); // "One"
  ///     Console.WriteLine (obj.DataTx[transaction2]); // "Two"
  ///   }
  /// }
  /// </code>
  /// </example>
  public class TransactionalAccessor<T>
  {
    private PropertyAccessor _propertyAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionalAccessor{T}"/> class.
    /// </summary>
    /// <param name="propertyAccessor">The property to be wrapped by this accessor.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="propertyAccessor"/> parameter is <see langword="null"/>.</exception>
    internal TransactionalAccessor (PropertyAccessor propertyAccessor)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      _propertyAccessor = propertyAccessor;
      _propertyAccessor.CheckType (typeof (T));
    }

    /// <summary>
    /// Gets or sets the wrapped property's value for the specified transaction.
    /// </summary>
    /// <value>The property's value in the specified transaction.</value>
    /// <param name="transaction">The <see cref="ClientTransaction"/> to get the value for.</param>
    /// <returns>The value of the encapsulated property.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="transaction"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the given <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The <see cref="DomainObject"/> was discarded.</exception>
    public T this[ClientTransaction transaction]
    {
      get
      {
        ArgumentUtility.CheckNotNull ("transaction", transaction);
        return _propertyAccessor.GetValueTx<T> (transaction);
      }
      set
      {
        ArgumentUtility.CheckNotNull ("transaction", transaction);
        _propertyAccessor.SetValueTx (transaction, value);
      }
    }
  }
}
