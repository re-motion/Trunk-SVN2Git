// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 
namespace Remotion.Diagnostics.ToText
{
  /// <summary>
  /// Implement <see cref="IToText"/> on a class to supply <see cref="To"/>.<see cref="To.Text"/> / <see cref="ToTextBuilder"/> support
  /// for class instances. 
  /// </summary>
  /// <remarks>
  /// Note that an externally registered type handler (see <see cref="To"/>-class description)
  /// takes precedence over an <see cref="IToText"/> implementation within a class.
  /// </remarks>
  /// 
  /// <example><code><![CDATA[
  /// public class Foo : IToText
  /// {
  ///   protected string userName, firstName, lastName;
  ///   protected int age;
  ///
  ///   public void ToText (IToTextBuilder toTextBuilder)
  ///   {
  ///     toTextBuilder.ib<Foo>();
  ///     toTextBuilder.e(() => userName).e("first", firstName).e("last", lastName).e(() => age);
  ///     toTextBuilder.ie();
  ///   }
  /// }
  /// ]]></code></example>
  // TODO AE: What's this?
  public class Foo : IToText
  {
    protected string userName, firstName, lastName;
    protected int age;

    public void ToText (IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<Foo>();
      toTextBuilder.e (() => userName).e ("first", firstName).e ("last", lastName).e (() => age);
      toTextBuilder.ie ();
    }
  }

  // TODO AE: Interface names should be nouns, eg. IToTextConvertible.
  public interface IToText
  {
    void ToText (IToTextBuilder toTextBuilder);
  }
}