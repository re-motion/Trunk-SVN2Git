﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using Remotion.Utilities;

namespace Remotion.TypePipe.Implementation
{
  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation : ParamList
  {


    public ParamListImplementation ()
    {

    }

    public override Type FuncType
    {
      get { return typeof (Func<object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action castAction;
      try
      {
        castAction = (Action) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction ();
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<object> castFunc;
      try
      {
        castFunc = (Func<object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc ();
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] {  };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] {  };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1> : ParamList
  {
    private readonly A1 _a1;

    public ParamListImplementation (A1 a1)
    {
      _a1 = a1;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1> castAction;
      try
      {
        castAction = (Action<A1>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, object> castFunc;
      try
      {
        castFunc = (Func<A1, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;

    public ParamListImplementation (A1 a1, A2 a2)
    {
      _a1 = a1;
      _a2 = a2;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2> castAction;
      try
      {
        castAction = (Action<A1, A2>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3> castAction;
      try
      {
        castAction = (Action<A1, A2, A3>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4, A5> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;
    private readonly A5 _a5;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
      _a5 = a5;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, A5, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4, A5>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4, A5> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4, A5>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4, _a5);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, A5, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, A5, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4, _a5);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4), typeof (A5) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4, _a5 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4, A5, A6> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;
    private readonly A5 _a5;
    private readonly A6 _a6;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
      _a5 = a5;
      _a6 = a6;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, A5, A6, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4, A5, A6>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4, A5, A6> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4, A5, A6>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4, _a5, _a6);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, A5, A6, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, A5, A6, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4, _a5, _a6);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4), typeof (A5), typeof (A6) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4, _a5, _a6 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4, A5, A6, A7> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;
    private readonly A5 _a5;
    private readonly A6 _a6;
    private readonly A7 _a7;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
      _a5 = a5;
      _a6 = a6;
      _a7 = a7;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, A5, A6, A7, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4, A5, A6, A7>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4, A5, A6, A7> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4, A5, A6, A7>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4, _a5, _a6, _a7);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, A5, A6, A7, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, A5, A6, A7, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4, _a5, _a6, _a7);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4), typeof (A5), typeof (A6), typeof (A7) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4, _a5, _a6, _a7 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4, A5, A6, A7, A8> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;
    private readonly A5 _a5;
    private readonly A6 _a6;
    private readonly A7 _a7;
    private readonly A8 _a8;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
      _a5 = a5;
      _a6 = a6;
      _a7 = a7;
      _a8 = a8;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, A5, A6, A7, A8, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4, A5, A6, A7, A8>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4, A5, A6, A7, A8> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4, A5, A6, A7, A8>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, A5, A6, A7, A8, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, A5, A6, A7, A8, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4), typeof (A5), typeof (A6), typeof (A7), typeof (A8) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4, A5, A6, A7, A8, A9> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;
    private readonly A5 _a5;
    private readonly A6 _a6;
    private readonly A7 _a7;
    private readonly A8 _a8;
    private readonly A9 _a9;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
      _a5 = a5;
      _a6 = a6;
      _a7 = a7;
      _a8 = a8;
      _a9 = a9;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4, A5, A6, A7, A8, A9> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4), typeof (A5), typeof (A6), typeof (A7), typeof (A8), typeof (A9) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;
    private readonly A5 _a5;
    private readonly A6 _a6;
    private readonly A7 _a7;
    private readonly A8 _a8;
    private readonly A9 _a9;
    private readonly A10 _a10;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
      _a5 = a5;
      _a6 = a6;
      _a7 = a7;
      _a8 = a8;
      _a9 = a9;
      _a10 = a10;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4), typeof (A5), typeof (A6), typeof (A7), typeof (A8), typeof (A9), typeof (A10) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;
    private readonly A5 _a5;
    private readonly A6 _a6;
    private readonly A7 _a7;
    private readonly A8 _a8;
    private readonly A9 _a9;
    private readonly A10 _a10;
    private readonly A11 _a11;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
      _a5 = a5;
      _a6 = a6;
      _a7 = a7;
      _a8 = a8;
      _a9 = a9;
      _a10 = a10;
      _a11 = a11;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4), typeof (A5), typeof (A6), typeof (A7), typeof (A8), typeof (A9), typeof (A10), typeof (A11) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;
    private readonly A5 _a5;
    private readonly A6 _a6;
    private readonly A7 _a7;
    private readonly A8 _a8;
    private readonly A9 _a9;
    private readonly A10 _a10;
    private readonly A11 _a11;
    private readonly A12 _a12;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
      _a5 = a5;
      _a6 = a6;
      _a7 = a7;
      _a8 = a8;
      _a9 = a9;
      _a10 = a10;
      _a11 = a11;
      _a12 = a12;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4), typeof (A5), typeof (A6), typeof (A7), typeof (A8), typeof (A9), typeof (A10), typeof (A11), typeof (A12) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;
    private readonly A5 _a5;
    private readonly A6 _a6;
    private readonly A7 _a7;
    private readonly A8 _a8;
    private readonly A9 _a9;
    private readonly A10 _a10;
    private readonly A11 _a11;
    private readonly A12 _a12;
    private readonly A13 _a13;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
      _a5 = a5;
      _a6 = a6;
      _a7 = a7;
      _a8 = a8;
      _a9 = a9;
      _a10 = a10;
      _a11 = a11;
      _a12 = a12;
      _a13 = a13;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4), typeof (A5), typeof (A6), typeof (A7), typeof (A8), typeof (A9), typeof (A10), typeof (A11), typeof (A12), typeof (A13) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;
    private readonly A5 _a5;
    private readonly A6 _a6;
    private readonly A7 _a7;
    private readonly A8 _a8;
    private readonly A9 _a9;
    private readonly A10 _a10;
    private readonly A11 _a11;
    private readonly A12 _a12;
    private readonly A13 _a13;
    private readonly A14 _a14;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
      _a5 = a5;
      _a6 = a6;
      _a7 = a7;
      _a8 = a8;
      _a9 = a9;
      _a10 = a10;
      _a11 = a11;
      _a12 = a12;
      _a13 = a13;
      _a14 = a14;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4), typeof (A5), typeof (A6), typeof (A7), typeof (A8), typeof (A9), typeof (A10), typeof (A11), typeof (A12), typeof (A13), typeof (A14) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;
    private readonly A5 _a5;
    private readonly A6 _a6;
    private readonly A7 _a7;
    private readonly A8 _a8;
    private readonly A9 _a9;
    private readonly A10 _a10;
    private readonly A11 _a11;
    private readonly A12 _a12;
    private readonly A13 _a13;
    private readonly A14 _a14;
    private readonly A15 _a15;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
      _a5 = a5;
      _a6 = a6;
      _a7 = a7;
      _a8 = a8;
      _a9 = a9;
      _a10 = a10;
      _a11 = a11;
      _a12 = a12;
      _a13 = a13;
      _a14 = a14;
      _a15 = a15;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4), typeof (A5), typeof (A6), typeof (A7), typeof (A8), typeof (A9), typeof (A10), typeof (A11), typeof (A12), typeof (A13), typeof (A14), typeof (A15) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;
    private readonly A5 _a5;
    private readonly A6 _a6;
    private readonly A7 _a7;
    private readonly A8 _a8;
    private readonly A9 _a9;
    private readonly A10 _a10;
    private readonly A11 _a11;
    private readonly A12 _a12;
    private readonly A13 _a13;
    private readonly A14 _a14;
    private readonly A15 _a15;
    private readonly A16 _a16;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
      _a5 = a5;
      _a6 = a6;
      _a7 = a7;
      _a8 = a8;
      _a9 = a9;
      _a10 = a10;
      _a11 = a11;
      _a12 = a12;
      _a13 = a13;
      _a14 = a14;
      _a15 = a15;
      _a16 = a16;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15, _a16);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15, _a16);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4), typeof (A5), typeof (A6), typeof (A7), typeof (A8), typeof (A9), typeof (A10), typeof (A11), typeof (A12), typeof (A13), typeof (A14), typeof (A15), typeof (A16) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15, _a16 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;
    private readonly A5 _a5;
    private readonly A6 _a6;
    private readonly A7 _a7;
    private readonly A8 _a8;
    private readonly A9 _a9;
    private readonly A10 _a10;
    private readonly A11 _a11;
    private readonly A12 _a12;
    private readonly A13 _a13;
    private readonly A14 _a14;
    private readonly A15 _a15;
    private readonly A16 _a16;
    private readonly A17 _a17;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16, A17 a17)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
      _a5 = a5;
      _a6 = a6;
      _a7 = a7;
      _a8 = a8;
      _a9 = a9;
      _a10 = a10;
      _a11 = a11;
      _a12 = a12;
      _a13 = a13;
      _a14 = a14;
      _a15 = a15;
      _a16 = a16;
      _a17 = a17;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15, _a16, _a17);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15, _a16, _a17);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4), typeof (A5), typeof (A6), typeof (A7), typeof (A8), typeof (A9), typeof (A10), typeof (A11), typeof (A12), typeof (A13), typeof (A14), typeof (A15), typeof (A16), typeof (A17) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15, _a16, _a17 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;
    private readonly A5 _a5;
    private readonly A6 _a6;
    private readonly A7 _a7;
    private readonly A8 _a8;
    private readonly A9 _a9;
    private readonly A10 _a10;
    private readonly A11 _a11;
    private readonly A12 _a12;
    private readonly A13 _a13;
    private readonly A14 _a14;
    private readonly A15 _a15;
    private readonly A16 _a16;
    private readonly A17 _a17;
    private readonly A18 _a18;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16, A17 a17, A18 a18)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
      _a5 = a5;
      _a6 = a6;
      _a7 = a7;
      _a8 = a8;
      _a9 = a9;
      _a10 = a10;
      _a11 = a11;
      _a12 = a12;
      _a13 = a13;
      _a14 = a14;
      _a15 = a15;
      _a16 = a16;
      _a17 = a17;
      _a18 = a18;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15, _a16, _a17, _a18);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15, _a16, _a17, _a18);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4), typeof (A5), typeof (A6), typeof (A7), typeof (A8), typeof (A9), typeof (A10), typeof (A11), typeof (A12), typeof (A13), typeof (A14), typeof (A15), typeof (A16), typeof (A17), typeof (A18) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15, _a16, _a17, _a18 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, A19> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;
    private readonly A5 _a5;
    private readonly A6 _a6;
    private readonly A7 _a7;
    private readonly A8 _a8;
    private readonly A9 _a9;
    private readonly A10 _a10;
    private readonly A11 _a11;
    private readonly A12 _a12;
    private readonly A13 _a13;
    private readonly A14 _a14;
    private readonly A15 _a15;
    private readonly A16 _a16;
    private readonly A17 _a17;
    private readonly A18 _a18;
    private readonly A19 _a19;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16, A17 a17, A18 a18, A19 a19)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
      _a5 = a5;
      _a6 = a6;
      _a7 = a7;
      _a8 = a8;
      _a9 = a9;
      _a10 = a10;
      _a11 = a11;
      _a12 = a12;
      _a13 = a13;
      _a14 = a14;
      _a15 = a15;
      _a16 = a16;
      _a17 = a17;
      _a18 = a18;
      _a19 = a19;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, A19, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, A19>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, A19> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, A19>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15, _a16, _a17, _a18, _a19);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, A19, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, A19, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15, _a16, _a17, _a18, _a19);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4), typeof (A5), typeof (A6), typeof (A7), typeof (A8), typeof (A9), typeof (A10), typeof (A11), typeof (A12), typeof (A13), typeof (A14), typeof (A15), typeof (A16), typeof (A17), typeof (A18), typeof (A19) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15, _a16, _a17, _a18, _a19 };
    }
  }

  /// <summary>
  /// Implements <see cref="ParamList"/> for a specific number of arguments. Use one of the <see cref="ParamList.Create"/> overloads to create
  /// instances of the <see cref="ParamList"/> implementation classes.
  /// </summary>
  public class ParamListImplementation<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, A19, A20> : ParamList
  {
    private readonly A1 _a1;
    private readonly A2 _a2;
    private readonly A3 _a3;
    private readonly A4 _a4;
    private readonly A5 _a5;
    private readonly A6 _a6;
    private readonly A7 _a7;
    private readonly A8 _a8;
    private readonly A9 _a9;
    private readonly A10 _a10;
    private readonly A11 _a11;
    private readonly A12 _a12;
    private readonly A13 _a13;
    private readonly A14 _a14;
    private readonly A15 _a15;
    private readonly A16 _a16;
    private readonly A17 _a17;
    private readonly A18 _a18;
    private readonly A19 _a19;
    private readonly A20 _a20;

    public ParamListImplementation (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16, A17 a17, A18 a18, A19 a19, A20 a20)
    {
      _a1 = a1;
      _a2 = a2;
      _a3 = a3;
      _a4 = a4;
      _a5 = a5;
      _a6 = a6;
      _a7 = a7;
      _a8 = a8;
      _a9 = a9;
      _a10 = a10;
      _a11 = a11;
      _a12 = a12;
      _a13 = a13;
      _a14 = a14;
      _a15 = a15;
      _a16 = a16;
      _a17 = a17;
      _a18 = a18;
      _a19 = a19;
      _a20 = a20;
    }

    public override Type FuncType
    {
      get { return typeof (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, A19, A20, object>); }
    }

    public override Type ActionType
    {
      get { return typeof (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, A19, A20>); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, A19, A20> castAction;
      try
      {
        castAction = (Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, A19, A20>) action;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("action", action.GetType(), ActionType);
      }

      castAction (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15, _a16, _a17, _a18, _a19, _a20);
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, A19, A20, object> castFunc;
      try
      {
        castFunc = (Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, A19, A20, object>) func;
      }
      catch (InvalidCastException)
      {
        throw ArgumentUtility.CreateArgumentTypeException ("func", func.GetType(), FuncType);
      }

      return castFunc (_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15, _a16, _a17, _a18, _a19, _a20);
    }

    public override Type[] GetParameterTypes ()
    {
      return new Type[] { typeof (A1), typeof (A2), typeof (A3), typeof (A4), typeof (A5), typeof (A6), typeof (A7), typeof (A8), typeof (A9), typeof (A10), typeof (A11), typeof (A12), typeof (A13), typeof (A14), typeof (A15), typeof (A16), typeof (A17), typeof (A18), typeof (A19), typeof (A20) };
    }

    public override object[] GetParameterValues ()
    {
      return new object[] { _a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10, _a11, _a12, _a13, _a14, _a15, _a16, _a17, _a18, _a19, _a20 };
    }
  }

}
