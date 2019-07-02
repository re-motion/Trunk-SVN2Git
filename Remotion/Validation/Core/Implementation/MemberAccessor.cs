// Decompiled with JetBrains decompiler
// Type: FluentValidation.MemberAccessor`2
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Remotion.Validation.Implementation
{
  public class MemberAccessor<TObject, TValue>
  {
    private readonly Expression<Func<TObject, TValue>> getExpression;
    private readonly Func<TObject, TValue> getter;
    private readonly Action<TObject, TValue> setter;

    public MemberAccessor(Expression<Func<TObject, TValue>> getExpression)
    {
      this.getExpression = getExpression;
      this.getter = getExpression.Compile();
      this.setter = MemberAccessor<TObject, TValue>.CreateSetExpression(getExpression).Compile();
      this.Member = getExpression.GetMember<TObject, TValue>();
    }

    private static Expression<Action<TObject, TValue>> CreateSetExpression(
      Expression<Func<TObject, TValue>> getExpression)
    {
      ParameterExpression parameterExpression = Expression.Parameter(getExpression.Body.Type);
      return Expression.Lambda<Action<TObject, TValue>>((Expression)Expression.Assign(getExpression.Body, (Expression)parameterExpression), getExpression.Parameters.First<ParameterExpression>(), parameterExpression);
    }

    public MemberInfo Member { get; private set; }

    public TValue Get(TObject target)
    {
      return this.getter(target);
    }

    public void Set(TObject target, TValue value)
    {
      this.setter(target, value);
    }

    protected bool Equals(MemberAccessor<TObject, TValue> other)
    {
      return this.Member.Equals((object)other.Member);
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object)null, obj))
        return false;
      if (object.ReferenceEquals((object)this, obj))
        return true;
      if (obj.GetType() != this.GetType())
        return false;
      return this.Equals((MemberAccessor<TObject, TValue>)obj);
    }

    public override int GetHashCode()
    {
      return this.Member.GetHashCode();
    }

    public static implicit operator Expression<Func<TObject, TValue>>(
      MemberAccessor<TObject, TValue> @this)
    {
      return @this.getExpression;
    }

    public static implicit operator MemberAccessor<TObject, TValue>(
      Expression<Func<TObject, TValue>> @this)
    {
      return new MemberAccessor<TObject, TValue>(@this);
    }
  }
}