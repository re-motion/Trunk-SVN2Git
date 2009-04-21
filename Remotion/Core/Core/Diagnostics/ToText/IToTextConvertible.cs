// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
namespace Remotion.Diagnostics.ToText
{
  /// <summary>
  /// Implement <see cref="IToTextConvertible"/> on a class to supply <see cref="To"/>.<see cref="To.Text"/> / <see cref="ToTextBuilder"/> support
  /// for class instances. 
  /// </summary>
  /// <remarks>
  /// Note that an externally registered type handler (see <see cref="To"/>-class description)
  /// takes precedence over an <see cref="IToTextConvertible"/> implementation within a class.
  /// </remarks>
  /// 
  /// <example><code><![CDATA[
  /// public class Foo : IToTextConvertible
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

  public interface IToTextConvertible
  {
    void ToText (IToTextBuilder toTextBuilder);
  }
}