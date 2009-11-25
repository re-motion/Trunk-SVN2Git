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
using System.Text;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Provides common checks needed by relation end points when they are assigned new values.
  /// </summary>
  public static class RelationEndPointValueChecker
  {
    public static void CheckNotDeleted (IEndPoint endPoint, DomainObject domainObject)
    {
      if (domainObject != null && domainObject.TransactionContext[endPoint.ClientTransaction].State == StateType.Deleted)
        throw new ObjectDeletedException (domainObject.ID);
    }

    public static void CheckClientTransaction (IEndPoint endPoint, DomainObject domainObject, string exceptionFormatString)
    {
      if (domainObject != null && !domainObject.TransactionContext[endPoint.ClientTransaction].CanBeUsedInTransaction)
      {
        string transactionInfo = GetTransactionInfoForMismatchingClientTransactions (endPoint, domainObject);

        var formattedMessage = String.Format (
            exceptionFormatString, 
            domainObject.ID, 
            endPoint.Definition.PropertyName, 
            endPoint.ObjectID);
        throw new ClientTransactionsDifferException (formattedMessage + " The objects do not belong to the same ClientTransaction." + transactionInfo);
      }
    }

    private static string GetTransactionInfoForMismatchingClientTransactions (IEndPoint endPoint, DomainObject otherDomainObject)
    {
      var transactionInfo = new StringBuilder ();

      var endPointObject = endPoint.GetDomainObject ();
      if (otherDomainObject.HasBindingTransaction)
      {
        transactionInfo.AppendFormat (" The {0} object is bound to a BindingClientTransaction.", otherDomainObject.GetPublicDomainObjectType ().Name);
        if (endPointObject.HasBindingTransaction)
        {
          transactionInfo.AppendFormat (
              " The {0} object owning the property is also bound, but to a different BindingClientTransaction.",
              endPointObject.GetPublicDomainObjectType ().Name);
        }
      }
      else if (endPointObject.HasBindingTransaction)
      {
        transactionInfo.AppendFormat (
            " The {0} object owning the property is bound to a BindingClientTransaction.", 
            endPointObject.GetPublicDomainObjectType ().Name);
      }
      return transactionInfo.ToString();
    }
  }
}