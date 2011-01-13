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
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Thrown when an object cannot be loaded or a relation cannot be resolved because it would generate an inconsistent state in the 
  /// <see cref="ClientTransaction"/>.
  /// </summary>
  /// <remarks>
  /// This exception can be caused by a variety of issues. Here are a few examples:
  /// <list type="bullet">
  /// <item>
  /// <para>
  /// When loading a single object that holds the foreign key of a bidirectional 1:1 relationship, the scenarios similar to the following lead to a 
  /// conflict. Sample scenario: Order - OrderTicket; OrderTicket contains the foreign key.
  /// <list type="number">
  ///  <item>Object OrderTicket1 is loaded, holding a foreign key to Order1.</item>
  ///  <item>Object OrderTicket2 is loaded, also holding a foreign key to Order1.</item>
  /// </list>
  /// This is a conflict because in a bidirectional 1:1 relation, only one object can relate to Order1. It can occur when an object is manually
  /// loaded by ID, when a number of objects is loaded by ID, when a query result is loaded, when a relation is resolved, and in other scenarios.
  /// </para>
  /// <para>
  /// It will also occur when the virtual side Order1.OrderTicket is resolved (no matter whether it is resolved to an actual object or to 
  /// <see langword="null" />) and then OrderTicket2 is loaded, holding a foreign key to Order1.
  /// </para>
  /// <para>
  /// The reason for this conflict can be an inconsistent state in the database, but it can also be caused by the database changing between steps 2 
  /// and 3. The <see cref="LoadConflictException"/> is thrown when the loaded object is to be registered in the <see cref="ClientTransaction"/>,
  /// and it will result in the object not being registered or returned. The load operation triggering the conflict is aborted.
  /// </para>
  /// </item>
  /// <item>
  /// <para>
  /// When a virtual 1:1 relation property (ie, the property in a bidirectional 1:1 relation that does not hold the foreign key) is loaded and that 
  /// load operation returns an object that already exists in the <see cref="ClientTransaction"/>, a <see cref="LoadConflictException"/> will be thrown.
  /// </para>
  /// <para>
  /// Consider the following scenario (using Order - OrderTicket, where OrderTicket contains the foreign key):
  /// <list type="number">
  /// <item>Object OrderTicket1 is loaded, holding a foreign key to Order1.</item>
  /// <item>Order2.OrderTicket is resolved by issuing a database query. The query returns OrderTicket1.</item>
  /// </list>
  /// This is a conflict because in bidirectional 1:1 relations, OrderTicket1 can only relate to one object. It can occur when a relation property 
  /// is accessed for the first time, or when a relation property is indirectly loaded (eg, because Order2 is assigned a new OrderTicket).
  /// </para>
  /// <para>
  /// The reason for this conflict is usually that the database has changed between steps 1 and 2. The <see cref="LoadConflictException"/> is thrown 
  /// when the related object is to be registered in the <see cref="ClientTransaction"/>, and it will result in the relation property remaining in 
  /// "unresolved" state. The load operation triggering the conflict is aborted and no related object is returned.
  /// </para>
  /// </item>
  /// </list>
  /// </remarks>
  public class LoadConflictException : Exception
  {
    public LoadConflictException (string message)
        : base (message)
    {
    }

    public LoadConflictException (string message, Exception innerException)
        : base (message, innerException)
    {
    }

    protected LoadConflictException ([NotNull] SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }
  }
}