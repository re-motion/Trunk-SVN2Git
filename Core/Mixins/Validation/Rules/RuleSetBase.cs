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

namespace Remotion.Mixins.Validation.Rules
{
  public abstract class RuleSetBase : IRuleSet
  {
    public abstract void Install (ValidatingVisitor visitor);

    protected void SingleShould (bool test, IValidationLog log, IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("log", log);
      ArgumentUtility.CheckNotNull ("rule", rule);

      if (!test)
        log.Warn (rule);
      else
        log.Succeed (rule);
    }

    protected void SingleMust (bool test, IValidationLog log, IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("log", log);
      ArgumentUtility.CheckNotNull ("rule", rule);

      if (!test)
        log.Fail (rule);
      else
        log.Succeed (rule);
    }
  }
}
