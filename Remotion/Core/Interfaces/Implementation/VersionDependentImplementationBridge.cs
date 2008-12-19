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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Implementation
{
  public static class VersionDependentImplementationBridge<T>
  {
    public static T Implementation
    {
      get
      {
        try
        {
          return ImplementationClass.Implementation;
        }
        catch (TypeInitializationException ex)
        {
          throw VersionDependentImplementationException.Wrap (typeof (T), ex.InnerException);
        }
      }
    }

    private class ImplementationClass
    {
      public static readonly T Implementation = GetImplementation ();

      private static T GetImplementation ()
      {
        var attributes = (ConcreteImplementationAttribute[]) typeof (T).GetCustomAttributes (typeof (ConcreteImplementationAttribute), false);
        if (attributes.Length != 1)
        {
          string message = string.Format ("Cannot get a version-dependent implementation of type '{0}': Expected one "
              + "ConcreteImplementationAttribute applied to the type, but found {1}.", typeof (T).FullName, attributes.Length);
          throw new InvalidOperationException (message);
        }
        return (T) attributes[0].InstantiateType ();
      }
    }
  }
}
