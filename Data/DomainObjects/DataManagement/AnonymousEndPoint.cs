// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
public class AnonymousEndPoint : IEndPoint
{
  // types

  // static members and constants

  // member fields

  private readonly RelationDefinition _relationDefinition;
  private readonly AnonymousRelationEndPointDefinition _definition;
  private readonly ClientTransaction _clientTransaction;
  private readonly DomainObject _domainObject;

  // construction and disposing

  // This end point stores a DomainObject rather than an ObjectID in order to support AnonymousEndPoints storing discarded objects (which can
  // happen with unidirectional relations.)
  public AnonymousEndPoint (ClientTransaction clientTransaction, DomainObject domainObject, RelationDefinition relationDefinition)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
    ArgumentUtility.CheckNotNull ("domainObjace", domainObject);
    ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

    _definition = GetAnonymousRelationEndPointDefinition (relationDefinition);
    _relationDefinition = relationDefinition;
    _clientTransaction = clientTransaction;
    _domainObject = domainObject;
  }

  protected AnonymousEndPoint (RelationDefinition relationDefinition)
  {
    ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);
    
    _definition = GetAnonymousRelationEndPointDefinition (relationDefinition);
    _relationDefinition = relationDefinition;
  }

  private AnonymousRelationEndPointDefinition GetAnonymousRelationEndPointDefinition (RelationDefinition relationDefinition)
  {
    foreach (IRelationEndPointDefinition endPointDefinition in relationDefinition.EndPointDefinitions)
    {
      if (endPointDefinition.IsNull)
        return (AnonymousRelationEndPointDefinition) endPointDefinition;
    }

    throw new ArgumentException ("The provided relation definition must contain a AnonymousRelationEndPointDefinition.", "relationDefinition");
  }

  // methods and properties

  #region IEndPoint Members

  public virtual ClientTransaction ClientTransaction
  {
    get { return _clientTransaction; }
  }

  public virtual DomainObject GetDomainObject ()
  {
    return _domainObject;
  }

  public virtual DataContainer GetDataContainer ()
  {
    DomainObject domainObject = GetDomainObject ();
    return _clientTransaction.GetDataContainer(domainObject);
  }

  public virtual ObjectID ObjectID
  {
    get { return _domainObject.ID; }
  }

  public RelationDefinition RelationDefinition
  {
    get { return _relationDefinition; }
  }

  public IRelationEndPointDefinition Definition
  {
    get { return _definition; }
  }

  #endregion

  #region INullObject Members

  public virtual bool IsNull
  {
    get { return false; }
  }

  #endregion
}
}
