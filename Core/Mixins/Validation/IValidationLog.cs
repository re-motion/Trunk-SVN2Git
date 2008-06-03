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
using System.Collections.Generic;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.Validation
{
  public interface IValidationLog
  {
    // methods for writing the log
    
    void ValidationStartsFor (IVisitableDefinition definition);
    void ValidationEndsFor (IVisitableDefinition definition);

    void Succeed (IValidationRule rule);
    void Warn (IValidationRule rule);
    void Fail (IValidationRule rule);
    void UnexpectedException (IValidationRule rule, Exception ex);

    // methods for reading the log

    IEnumerable<ValidationResult> GetResults ();
    int ResultCount { get; }

    int GetNumberOfFailures();
    int GetNumberOfWarnings ();
    int GetNumberOfSuccesses ();
    int GetNumberOfUnexpectedExceptions ();
    int GetNumberOfRulesExecuted ();
    void MergeIn (IValidationLog log);
  }
}
