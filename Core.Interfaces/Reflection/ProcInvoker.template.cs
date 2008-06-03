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

namespace Remotion.Reflection
{
  /// <summary>
  /// This interface allows invokers with fixed arguments to be returned without references to their generic argument types. 
  /// </summary>
  /// <remarks>
  /// <p>Note that casting a <c>ProcInvoker</c> to an interface is a boxing operation, thus creating an object on the
  /// heap and garbage collecting it later. For very performance-critical scenarios, it be better to avoid this and accept the references to 
  /// the invoker's generic argument types.</p>
  /// <p>It is recommended to wrap this interface within a <see cref="ProcInvokerWrapper"/>, because returning an interface could lead to 
  /// ambigous castings if the final call to <see cref="With{A1}"/> is missing, while using structs will usually lead to a compile-time error as 
  /// expected.</p>
  /// </remarks>
  public partial interface IProcInvoker
  {
    // @begin-skip
    void Invoke (Type[] valueTypes, object[] values);
    void Invoke (object[] values);
    // @end-skip

    // @begin-template first=1 generate=0..7 suppressTemplate=true

    // @replace "A<n>" ", " "<" ">"
    // @replace "A<n> a<n>" ", "
    void With<A1> (A1 a1);
    // @end-template
  }

  /// <summary>
  /// Used to wrap an <see cref="IProcInvoker"/> object rather than returning it directly.
  /// </summary>
  public partial struct ProcInvokerWrapper: IProcInvoker
  {
    // @begin-skip
    private readonly IProcInvoker _invoker;

    public ProcInvokerWrapper (IProcInvoker invoker)
    {
      _invoker = invoker;
    }

    public void Invoke (Type[] valueTypes, object[] values)
    {
      _invoker.Invoke (valueTypes, values);
    }
    public void Invoke (object[] values)
    {
      _invoker.Invoke (values);
    }
    // @end-skip

    // @begin-template first=1 generate=0..7 suppressTemplate=true

    // @replace "A<n>" ", " "<" ">"
    // @replace "A<n> a<n>" ", "
    public void With<A1> (A1 a1)
    {
      // @replace "a<n>" ", "
      _invoker.With (a1);
    }
    // @end-template
  }
}
