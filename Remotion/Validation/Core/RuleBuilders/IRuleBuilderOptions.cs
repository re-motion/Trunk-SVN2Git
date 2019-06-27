// Decompiled with JetBrains decompiler
// Type: FluentValidation.IRuleBuilderOptions`2
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using Remotion.Validation.Rules;

namespace Remotion.Validation.RuleBuilders
{
  /// <summary>Rule builder</summary>
  /// <typeparam name="T"></typeparam>
  /// <typeparam name="TProperty"></typeparam>
  public interface IRuleBuilderOptions<T, out TProperty> : IRuleBuilder<T, TProperty>, FluentValidation.Internal.IFluentInterface, FluentValidation.Internal.IConfigurable<PropertyRule, IRuleBuilderOptions<T, TProperty>>
  {
  }
}