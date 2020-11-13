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
using System.Runtime.CompilerServices;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Provides method for common exceptions
  /// </summary>
  public static class AssertionExceptionUtility
  {
    [NotNull]
    [MustUseReturnValue]
    public static WebTestException CreateControlDisabledException ([CallerMemberName] string operationName = "", IDriver driver = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("operationName", operationName);

      return CreateException (string.Format ("The control is currently in a disabled state. Therefore, the '{0}' operation is not possible.", operationName), driver);
    }

    [NotNull]
    [MustUseReturnValue]
    public static WebTestException CreateCommandDisabledException ([CallerMemberName] string operationName = "", IDriver driver = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("operationName", operationName);

      return CreateException (string.Format ("The command is currently in a disabled state. Therefore, the '{0}' operation is not possible.", operationName), driver);
    }

    [NotNull]
    [MustUseReturnValue]
    public static WebTestException CreateControlReadOnlyException (IDriver driver = null)
    {
      return CreateException ("The control is currently in a read-only state. Therefore, the operation is not possible.", driver);
    }

    [NotNull]
    [MustUseReturnValue]
    public static WebTestException CreateControlNotReadOnlyException (IDriver driver = null)
    {
      return CreateException ("The control is currently not in a read-only state. Therefore, the operation is not possible.", driver);
    }

    [NotNull]
    [MustUseReturnValue]
    public static WebTestException CreateControlMissingException ([NotNull] string exceptionDetails, IDriver driver = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("exceptionDetails", exceptionDetails);

      return CreateException ($"The element cannot be found: {exceptionDetails}", driver);
    }

    [NotNull]
    [MustUseReturnValue]
    public static WebTestException CreateControlAmbiguousException (IDriver driver, [NotNull] string exceptionDetails)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("exceptionDetails", exceptionDetails);

      return CreateException ($"Multiple elements were found: {exceptionDetails}", driver);
    }

    [NotNull]
    [MustUseReturnValue]
    [StringFormatMethod ("message")]
    public static WebTestException CreateExpectationException (IDriver driver, [NotNull] string message, params object[] args)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("message", message);

      return CreateException (string.Format (message, args), driver);
    }

    private static WebTestException CreateException (string message, IDriver driver = null)
    {
      return driver == null
          ? new WebTestException (message)
          : new WebTestException (
              $"{message}\r\n(Browser: {driver.GetBrowserName()}, version {driver.GetBrowserVersion()})\r\n(Webdriver version: {driver.GetWebDriverVersion()})");
    }
  }
}