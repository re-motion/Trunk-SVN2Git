using System;

namespace Remotion.Mixins
{
  /// <summary>
  /// Indicates that a mixin extends a specific class, providing some part of its functionality or public interface.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This attribute is effective for the declarative mixin configuration, which is in effect by default when an application is started.
  /// </para>
  /// <para> 
  /// Although the attribute itself is not inherited, its semantics in mixin configuration are: If a base class is configured to be mixed with a
  /// mixin type M by means of the <see cref="ExtendsAttribute"/>, this configuration setting is inherited by each of its (direct and indirect) subclasses.
  /// The subclasses will therefore also be mixed with the same mixin type M unless a second mixin M2 derived from M is applied to the subclass, thus
  /// overriding the inherited configuration. If M is configured for both base class and subclass, the base class configuration is ignored.
  /// </para>
  /// <para>
  /// This attribute can be applied to the same mixin class multiple times if it extends multiple target classes. It should not however be used to
  /// apply the same mixin multiple times to the same target class.
  /// </para>
  /// </remarks>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class ExtendsAttribute : MixinRelationshipAttribute
  {
    private readonly Type _targetType;

    private Type[] _mixinTypeArguments = Type.EmptyTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendsAttribute"/> class.
    /// </summary>
    /// <param name="targetType">The target type extended by this mixin.</param>
    public ExtendsAttribute (Type targetType)
    {
      if (targetType == null)
        throw new ArgumentNullException ("targetType");
      _targetType = targetType;
    }

    /// <summary>
    /// Gets the target type the mixin class applies to.
    /// </summary>
    /// <value>The target type the mixin class applies to.</value>
    public Type TargetType
    {
      get { return _targetType; }
    }

    /// <summary>
    /// Gets or sets the generic type arguments to be used when applying a generic mixin to the given target type. This is useful when the
    /// <see cref="ExtendsAttribute"/> is to be applied to a generic mixin class, but the default generic type specialization algorithm of the
    /// mixin engine does not give the desired results.
    /// </summary>
    /// <value>The generic type arguments to close the generic mixin type with.</value>
    /// <remarks>If this attribute is applied to a non-generic mixin class or if the types supplied don't match the mixin's generic parameters,
    /// an exception is thrown when the mixin configuration is analyzed.</remarks>
    public Type[] MixinTypeArguments
    {
      get { return _mixinTypeArguments; }
      set
      {
        if (value == null)
          throw new ArgumentNullException ("value");
        _mixinTypeArguments = value;
      }
    }
  }
}
