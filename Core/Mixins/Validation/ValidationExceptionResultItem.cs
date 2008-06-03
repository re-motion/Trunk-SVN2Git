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
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  [Serializable]
  public struct ValidationExceptionResultItem : IDefaultValidationResultItem
  {
    private IValidationRule _rule;
    private Exception _exception;

    public ValidationExceptionResultItem (IValidationRule rule, Exception exception)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      ArgumentUtility.CheckNotNull ("exception", exception);

      _rule = rule;
      _exception = exception;
    }

    public IValidationRule Rule
    {
      get { return _rule; }
    }

    public string Message
    {
      get { return _exception.ToString (); }
    }

    public Exception Exception
    {
      get { return _exception; }
    }
  }
}
