// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Utilities;
using Remotion.Utilities;

namespace Remotion.Linq.Clauses.StreamedData
{
  /// <summary>
  /// Describes sequence data streamed out of a <see cref="QueryModel"/> or <see cref="ResultOperatorBase"/>. Sequence data can be held by an object
  /// implementing <see cref="IEnumerable{T}"/>, and its items are described via a <see cref="ItemExpression"/>.
  /// </summary>
  public class StreamedSequenceInfo : IStreamedDataInfo
  {
    private static readonly MethodInfo s_executeMethod = (typeof (StreamedSequenceInfo).GetMethod ("ExecuteCollectionQueryModel"));

    public StreamedSequenceInfo (Type dataType, Expression itemExpression)
    {
      ArgumentUtility.CheckNotNull ("dataType", dataType);
      ArgumentUtility.CheckNotNull ("itemExpression", itemExpression);

      ResultItemType = ReflectionUtility.GetItemTypeOfIEnumerable (dataType, "dataType");
      if (!ResultItemType.IsAssignableFrom (itemExpression.Type))
      {
        var message = string.Format ("ItemExpression is of type '{0}', but should be '{1}' (or derived from it).", itemExpression.Type, ResultItemType);
        throw new ArgumentException (message, "itemExpression");
      }

      DataType = dataType;
      ItemExpression = itemExpression;
    }

    /// <summary>
    /// Gets the type of the items returned by the sequence described by this object, as defined by <see cref="DataType"/>. Note that because 
    /// <see cref="IEnumerable{T}"/> is covariant starting from .NET 4.0, this may be a more abstract type than what's returned by 
    /// <see cref="ItemExpression"/>'s <see cref="Expression.Type"/> property.
    /// </summary>
    public Type ResultItemType { get; private set; }

    /// <summary>
    /// Gets an expression that describes the structure of the items held by the sequence described by this object.
    /// </summary>
    /// <value>The expression for the sequence's items.</value>
    public Expression ItemExpression { get; private set; }

    /// <summary>
    /// Gets the type of the data described by this <see cref="StreamedSequenceInfo"/> instance. This is a type implementing
    /// <see cref="IEnumerable{T}"/>, where <c>T</c> is instantiated with a concrete type.
    /// </summary>
    public Type DataType { get; private set; }

    /// <summary>
    /// Returns a new <see cref="StreamedSequenceInfo"/> with an adjusted <see cref="DataType"/>.
    /// </summary>
    /// <param name="dataType">The type to use for the <see cref="DataType"/> property. The type must be convertible from the previous type, otherwise
    /// an exception is thrown. The type may be a generic type definition; in this case,
    /// the type definition is automatically closed with the type of the <see cref="ItemExpression"/>.</param>
    /// <returns>
    /// A new <see cref="StreamedSequenceInfo"/> with a new <see cref="DataType"/>.
    /// </returns>
    /// <exception cref="ArgumentException">The <paramref name="dataType"/> is not compatible with the items described by this
    /// <see cref="StreamedSequenceInfo"/>.</exception>
    public virtual IStreamedDataInfo AdjustDataType (Type dataType)
    {
      ArgumentUtility.CheckNotNull ("dataType", dataType);

      if (dataType.IsGenericTypeDefinition)
      {
        try
        {
          dataType = dataType.MakeGenericType (ResultItemType);
        }
        catch (ArgumentException ex)
        {
          var message = string.Format (
              "The generic type definition '{0}' could not be closed over the type of the ResultItemType ('{1}'). {2}", 
              dataType,
              ResultItemType, 
              ex.Message);
          throw new ArgumentException (message, "dataType");
        }
      }

      try
      {
        //TODO: RM-5940: Add Assertions
        return new StreamedSequenceInfo (dataType, ItemExpression);
      }
      catch (ArgumentException)
      {
        var message = string.Format (
              "'{0}' cannot be used as the data type for a sequence with an ItemExpression of type '{1}'.",
              dataType,
              ResultItemType);
        throw new ArgumentException (message, "dataType");
      }
    }

    /// <summary>
    /// Takes the given <paramref name="genericMethodDefinition"/> and instantiates it, substituting its generic parameter with the 
    /// item type of the sequence described by this object. The method must have exactly one generic parameter.
    /// </summary>
    /// <param name="genericMethodDefinition">The generic method definition to instantiate.</param>
    /// <returns>
    /// A closed generic instantiation of <paramref name="genericMethodDefinition"/> with this object's item type substituted for
    /// the generic parameter.
    /// </returns>
    public virtual MethodInfo MakeClosedGenericExecuteMethod (MethodInfo genericMethodDefinition)
    {
      ArgumentUtility.CheckNotNull ("genericMethodDefinition", genericMethodDefinition);

      if (!genericMethodDefinition.IsGenericMethodDefinition)
        throw new ArgumentException ("GenericMethodDefinition must be a generic method definition.", "genericMethodDefinition");

      if (genericMethodDefinition.GetGenericArguments ().Length != 1)
        throw new ArgumentException ("GenericMethodDefinition must have exactly one generic parameter.", "genericMethodDefinition");

      return genericMethodDefinition.MakeGenericMethod (ResultItemType);
    }

    public IStreamedData ExecuteQueryModel (QueryModel queryModel, IQueryExecutor executor)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);
      ArgumentUtility.CheckNotNull ("executor", executor);

      var executeMethod = s_executeMethod.MakeGenericMethod (ResultItemType);

      // wrap executeMethod into a delegate instead of calling Invoke in order to allow for exceptions that are bubbled up correctly
      var func = (Func<QueryModel, IQueryExecutor, IEnumerable>)
          Delegate.CreateDelegate (typeof (Func<QueryModel, IQueryExecutor, IEnumerable>), this, executeMethod);
      var result = func (queryModel, executor).AsQueryable ();

      return new StreamedSequence (result, new StreamedSequenceInfo (result.GetType(), ItemExpression));
    }

    public virtual IEnumerable ExecuteCollectionQueryModel<T> (QueryModel queryModel, IQueryExecutor executor)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);
      ArgumentUtility.CheckNotNull ("executor", executor);

      return executor.ExecuteCollection<T> (queryModel);
    }

    public override sealed bool Equals (object obj)
    {
      var other = obj as IStreamedDataInfo;
      return Equals (other);
    }

    public virtual bool Equals (IStreamedDataInfo obj)
    {
      if (obj == null)
        return false;

      if (GetType () != obj.GetType ())
        return false;

      var other = (StreamedSequenceInfo) obj;
      return DataType.Equals (other.DataType) && ItemExpression.Equals (other.ItemExpression);
    }

    public override int GetHashCode ()
    {
      return DataType.GetHashCode () ^ ItemExpression.GetHashCode();
    }
  }
}
