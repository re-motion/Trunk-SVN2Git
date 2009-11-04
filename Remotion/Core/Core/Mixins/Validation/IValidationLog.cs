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
using System.Collections.Generic;
using Remotion.Collections;
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

    IDataStore<object, object> ContextStore { get; }

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
