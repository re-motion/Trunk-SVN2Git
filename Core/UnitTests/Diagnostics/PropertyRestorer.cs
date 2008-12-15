// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Linq.Expressions;
using Remotion.Reflection;

namespace Remotion.UnitTests.Diagnostics
{
  //public class PropertyRestorer<TClass, TProperty> : IDisposable
  //{
  //  private readonly TClass _instance;
  //  private readonly Property<TClass, TProperty> _property;
  //  private readonly TProperty _propertyValue;

  //  public PropertyRestorer (TClass instance, Property<TClass, TProperty> property)
  //  {
  //    _instance = instance;
  //    _property = property;
  //    _propertyValue = _property.Get (_instance);
  //  }

  //  public void Dispose ()
  //  {
  //    _property.Set (_instance, _propertyValue);
  //  }
  //}

  public class PropertyRestorer<TClass, TProperty> : IDisposable
  {
    private readonly TClass _instance;
    private readonly Property<TClass, TProperty> _property;
    private readonly TProperty _propertyValue;

    public PropertyRestorer (TClass instance, Expression<Func<TClass, TProperty>> propertyLambda)
    {
      _instance = instance;
      _property = new Property<TClass, TProperty> (propertyLambda);
      _propertyValue = _property.Get (_instance);
    }

    public void Dispose ()
    {
      _property.Set (_instance, _propertyValue);
    }
  }
}