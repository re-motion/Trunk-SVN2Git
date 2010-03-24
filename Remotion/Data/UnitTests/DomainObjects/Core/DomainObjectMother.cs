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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Reflection;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public static class DomainObjectMother
  {
    public static T CreateObjectInTransaction<T> (ClientTransaction transaction) where T : DomainObject
    {
      return (T) LifetimeService.NewObject (transaction, typeof (T), ParamList.Empty);
    }

    public static T CreateObjectInOtherTransaction<T> () where T : DomainObject
    {
      return CreateObjectInTransaction<T> (ClientTransaction.CreateRootTransaction());
    }

    public static T GetObjectInOtherTransaction<T> (ObjectID objectID) where T : DomainObject
    {
      var transaction = ClientTransaction.CreateRootTransaction ();
      return GetObjectInTransaction<T> (transaction, objectID);
    }

    public static T GetObjectInTransaction<T> (ClientTransaction transaction, ObjectID objectID) where T : DomainObject
    {
      return (T) LifetimeService.GetObject (transaction, objectID, true);
    }

    public static T CreateFakeObject<T> () where T: DomainObject
    {
      return CreateObjectInOtherTransaction<T> ();
    }

    public static T GetObjectReference<T> (ClientTransaction clientTransaction, ObjectID objectID) where T : DomainObject
    {
      return (T) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, typeof (ClientTransaction), "GetObjectReference", objectID);
    }
  }
}