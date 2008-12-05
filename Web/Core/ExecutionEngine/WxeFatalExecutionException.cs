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
using System.Runtime.Serialization;

namespace Remotion.Web.ExecutionEngine
{
  //TODO: useful exception message
  [Serializable]
  public class WxeFatalExecutionException:WxeException
  {
    private readonly Exception _outerException;

    public WxeFatalExecutionException (Exception innerExcection, Exception outerException)
      : base ("Execution failed.\r\n" + innerExcection.Message + (outerException != null ? ("\r\n" + outerException.Message) : string.Empty), innerExcection)
    {
      _outerException = outerException;
    }

    public WxeFatalExecutionException (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }

    public Exception OuterException
    {
      get { return _outerException; }
    }
  }
}
