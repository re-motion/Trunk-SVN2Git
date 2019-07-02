// Decompiled with JetBrains decompiler
// Type: FluentValidation.ValidatorDescriptor`1
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.Rules;
using Remotion.Validation.Validators;

namespace Remotion.Validation
{
  /// <summary>Used for providing metadata about a validator.</summary>
  public class ValidatorDescriptor<T> : IValidatorDescriptor
  {
    protected IEnumerable<IValidationRule> Rules { get; private set; }

    public ValidatorDescriptor(IEnumerable<IValidationRule> ruleBuilders)
    {
      this.Rules = ruleBuilders;
    }

    public virtual string GetName(string property)
    {
      return this.Rules.OfType<PropertyRule>().Where<PropertyRule>((Func<PropertyRule, bool>) (x => x.PropertyName == property)).Select<PropertyRule, string>((Func<PropertyRule, string>) (x => x.GetDisplayName())).FirstOrDefault<string>();
    }

    public virtual ILookup<string, IPropertyValidator> GetMembersWithValidators()
    {
      return this.Rules.OfType<PropertyRule>().Where<PropertyRule>((Func<PropertyRule, bool>) (rule => rule.PropertyName != null)).SelectMany((Func<PropertyRule, IEnumerable<IPropertyValidator>>) (rule => rule.Validators), (rule, validator) => new
      {
        propertyName = rule.PropertyName,
        validator = validator
      }).ToLookup(x => x.propertyName, x => x.validator);
    }

    public IEnumerable<IPropertyValidator> GetValidatorsForMember(
      string name)
    {
      return this.GetMembersWithValidators()[name];
    }

    public IEnumerable<IValidationRule> GetRulesForMember(string name)
    {
      return (IEnumerable<IValidationRule>) this.Rules.OfType<PropertyRule>().Where<PropertyRule>((Func<PropertyRule, bool>) (rule => rule.PropertyName == name)).Select<PropertyRule, IValidationRule>((Func<PropertyRule, IValidationRule>) (rule => (IValidationRule) rule)).ToList<IValidationRule>();
    }

    public virtual string GetName(Expression<Func<T, object>> propertyExpression)
    {
      MemberInfo member = propertyExpression.GetMember<T, object>();
      if (member == (MemberInfo) null)
        throw new ArgumentException(string.Format("Cannot retrieve name as expression '{0}' as it does not specify a property.", (object) propertyExpression));
      return this.GetName(member.Name);
    }

    public IEnumerable<IPropertyValidator> GetValidatorsForMember<TValue>(MemberAccessor<T, TValue> accessor)
    {
      return this.Rules.OfType<PropertyRule>().Where<PropertyRule>((Func<PropertyRule, bool>) (rule => object.Equals((object) rule.Member, (object) accessor.Member))).SelectMany<PropertyRule, IPropertyValidator, IPropertyValidator>((Func<PropertyRule, IEnumerable<IPropertyValidator>>) (rule => rule.Validators), (Func<PropertyRule, IPropertyValidator, IPropertyValidator>) ((rule, validator) => validator));
    }
  }
}
