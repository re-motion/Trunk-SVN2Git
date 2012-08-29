// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  [Serializable]
  public class ObjectCleanupException : Exception
  {
    private readonly ObjectID _objectID;
    private readonly Exception _originalException;

    public ObjectCleanupException (string message, ObjectID objectID, Exception innerException, Exception originalException)
        : base (message, ArgumentUtility.CheckNotNull ("innerException", innerException))
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      ArgumentUtility.CheckNotNull ("originalException", originalException);

      _objectID = objectID;
      _originalException = originalException;
    }

    protected ObjectCleanupException ([NotNull] SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
      _objectID = (ObjectID) info.GetValue ("_objectID", typeof (ObjectID));
      _originalException = (Exception) info.GetValue ("_originalException", typeof (Exception));
    }

    public ObjectID ObjectID
    {
      get { return _objectID; }
    }

    public Exception OriginalException
    {
      get { return _originalException; }
    }

    public override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData (info, context);

      info.AddValue ("_objectID", _objectID);
      info.AddValue ("_originalException", _originalException);
    }
  }
}