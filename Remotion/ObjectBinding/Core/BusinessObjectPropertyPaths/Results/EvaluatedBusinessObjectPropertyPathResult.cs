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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results
{
  public class EvaluatedBusinessObjectPropertyPathResult : IBusinessObjectPropertyPathResult
  {
    private readonly IBusinessObject _resultObject;
    private readonly IBusinessObjectProperty _resultProperty;

    public EvaluatedBusinessObjectPropertyPathResult (IBusinessObject resultObject, IBusinessObjectProperty resultProperty)
    {
      ArgumentUtility.CheckNotNull ("resultObject", resultObject);
      ArgumentUtility.CheckNotNull ("resultProperty", resultProperty);

      _resultObject = resultObject;
      _resultProperty = resultProperty;
    }

    public bool IsEvaluated
    {
      get { return true; }
    }

    public object GetValue ()
    {
      if (!_resultProperty.IsAccessible (_resultObject.BusinessObjectClass, _resultObject))
        return null;

      return _resultObject.GetProperty (_resultProperty);
    }

    public string GetString (string format)
    {
      if (!_resultProperty.IsAccessible (_resultObject.BusinessObjectClass, _resultObject))
        return _resultObject.BusinessObjectClass.BusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder();

      return _resultObject.GetPropertyString (_resultProperty, format);
    }

    public IBusinessObjectProperty ResultProperty
    {
      get { return _resultProperty; }
    }

    public IBusinessObject ResultObject
    {
      get { return _resultObject; }
    }
  }
}