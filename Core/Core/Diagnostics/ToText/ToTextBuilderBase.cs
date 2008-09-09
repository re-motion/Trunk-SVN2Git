using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText
{
  public abstract class ToTextBuilderBase : IToTextBuilderBase
  {
    protected ToTextProvider _toTextProvider;
    protected readonly Stack<SequenceStateHolder> _sequenceStack = new Stack<SequenceStateHolder> (16);

    public enum ToTextBuilderOutputComplexityLevel
    {
      Disable,
      Skeleton,
      Basic,
      Medium,
      Complex,
      Full,
    };


    public ToTextBuilderBase (ToTextProvider toTextProvider)
    {
      _toTextProvider = toTextProvider;
      OutputComplexity = ToTextBuilderOutputComplexityLevel.Basic;
      SequenceState = null;
    }


    public SequenceStateHolder SequenceState { get; protected set; }
    public virtual bool UseMultiLine { get; set; }
    public abstract bool Enabled { get; set; }
    //public abstract IToTextBuilderBase seperator { get; }
    //public abstract IToTextBuilderBase comma { get; }
    //public abstract IToTextBuilderBase colon { get; }
    //public abstract IToTextBuilderBase semicolon { get; }

    //public IToTextBuilderBase seperator
    //{
    //  get { AppendSeperator (); return this; }
    //}


    public ToTextBuilderOutputComplexityLevel OutputComplexity { get; protected set; }

    public bool IsInSequence
    {
      get { return SequenceState != null; }
    }

    public ToTextProvider ToTextProvider
    {
      get { return _toTextProvider; }
      set { _toTextProvider = value; }
    }

    public IToTextBuilderBase cSkeleton
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Skeleton);
      }
    }

    public IToTextBuilderBase cBasic
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Basic);
      }
    }

    public IToTextBuilderBase cMedium
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Medium);
      }
    }

    public IToTextBuilderBase cComplex
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Complex);
      }
    }

    public IToTextBuilderBase cFull
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Full);
      }
    }

    //public abstract IToTextBuilderBase ToTextString (string s);
    public void OutputDisable () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Disable; }
    public void OutputSkeleton () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Skeleton; }
    public void OutputBasic () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Basic; }
    public void OutputMedium () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Medium; }
    public void OutputComplex () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Complex; }
    public void OutputFull () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Full; }
    public abstract IToTextBuilderBase WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel complexityLevel);
    public abstract string CheckAndConvertToString ();
    protected abstract void BeforeAppendElement ();
    protected abstract void AfterAppendElement ();
    public abstract IToTextBuilderBase Flush ();
    public abstract IToTextBuilderBase WriteNewLine ();
    public abstract IToTextBuilderBase nl ();
    //public abstract virtual IToTextBuilderBase AppendSeperator ();
    protected abstract IToTextBuilderBase AppendObjectToString (object obj);

    public IToTextBuilderBase ts (object obj)
    {
      return AppendObjectToString (obj);
    }

    public IToTextBuilderBase AppendSequenceBegin (string name, string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix)
    {
      //BeforeAppendElement ();

      return SequenceBegin (name, sequencePrefix, firstElementPrefix, otherElementPrefix, elementPostfix, sequencePostfix);
    }

    protected abstract IToTextBuilderBase SequenceBegin (string name, string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix);

    public IToTextBuilderBase sb ()
    {
      return AppendSequenceBegin ("", "(", "", ",", "", ")");
    }

    public IToTextBuilderBase sb (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix)
    {
      return AppendSequenceBegin ("", sequencePrefix, firstElementPrefix, otherElementPrefix, elementPostfix, sequencePostfix);
    }

    public IToTextBuilderBase sb (string sequencePrefix, string separator, string sequencePostfix)
    {
      return AppendSequenceBegin ("", sequencePrefix, "", separator, "", sequencePostfix);
    }

    public IToTextBuilderBase sb (string sequencePrefix, string sequencePostfix)
    {
      return AppendSequenceBegin ("", sequencePrefix, "", ",", "", sequencePostfix);
    }


    public abstract IToTextBuilderBase AppendRawStringUnsafe (string s);

    public IToTextBuilderBase WriteRawString (string s)
    {
      AssertIsInRawSequence ();
      AppendRawStringUnsafe (s);
      return this;
    }

    private void AssertIsInRawSequence ()
    {
      Assertion.IsTrue(IsInRawSequence);
      //throw new NotImplementedException();
    }

    protected bool IsInRawSequence
    {
      get; set;
    }


    //public abstract IToTextBuilderBase AppendRawString (string s);

    public abstract IToTextBuilderBase AppendRawEscapedStringUnsafe (string s);

    public IToTextBuilderBase WriteRawEscapedString (string s) 
    {
      AssertIsInRawSequence ();
      AppendRawEscapedStringUnsafe (s);
      return this;
    }

    //public abstract IToTextBuilderBase AppendRawEscapedString (string s);
    public abstract IToTextBuilderBase sEsc (string s);

    public IToTextBuilderBase s (string s)
    {
      //return AppendRawString (s);
      return AppendRawStringUnsafe (s); // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }

    public abstract IToTextBuilderBase AppendRawCharUnsafe (char c);

    public IToTextBuilderBase WriteRawChar (char c)
    {
      AssertIsInRawSequence ();
      AppendRawCharUnsafe (c);
      return this;
    }

    //public abstract IToTextBuilderBase AppendRawChar (char c);
    public abstract IToTextBuilderBase AppendMember (string name, Object obj);

    public IToTextBuilderBase AppendMember<T> (Expression<Func<object, T>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      var variableName = RightUntilChar (expression.Body.ToString (), '.');
      var variableValue = expression.Compile ().Invoke (null);
      return AppendMember (variableName, variableValue);
    }

    public IToTextBuilderBase AppendMemberNonSequence (string name, Object obj)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      AppendMemberRaw (name, obj);
      return this;
    }

    protected abstract IToTextBuilderBase AppendMemberRaw (string name, Object obj);

    public IToTextBuilderBase m (Object obj)
    {
      return WriteElement (obj);
    }

    public IToTextBuilderBase m (string name, Object obj, bool honorSequence)
    {
      return honorSequence ? AppendMember (name, obj) : AppendMemberNonSequence (name, obj);
    }

    public IToTextBuilderBase m<T> (Expression<Func<object, T>> expression)
    {
      return AppendMember (expression);
    }

    public IToTextBuilderBase m (string name, Object obj)
    {
      return AppendMember (name, obj);
    }

    public abstract IToTextBuilderBase AppendEnumerable (IEnumerable collection);
    public abstract IToTextBuilderBase array (Array array);

    public IToTextBuilderBase collection (IEnumerable collection)
    {
      return AppendEnumerable (collection);
    }

    public abstract IToTextBuilderBase AppendArray (Array array);

    //public IToTextBuilderBase WriteElement (Object obj)
    //{
    //  _toTextProvider.ToText (obj, this);
    //  return this;
    //}

    //protected IToTextBuilderBase AppendToTextRaw (Object obj)
    //{
    //  _toTextProvider.ToText (obj, this);
    //  return this;
    //}

    //public IToTextBuilderBase e (Object obj)
    //{
    //  return WriteElement (obj);
    //}

    public IToTextBuilderBase e (Object obj, bool honorSequence)
    {
      return honorSequence ? WriteElement (obj) : WriteElement (obj);
    }

    public IToTextBuilderBase WriteElement (Object obj)
    {
      _toTextProvider.ToText (obj, this);
      return this;
    }


    public IToTextBuilderBase LowLevelWrite (string s)
    {
      return WriteRawString (s);
    }

    public abstract IToTextBuilderBase LowLevelWrite (Object obj);

    private IToTextBuilderBase AppendInstanceBegin (Type type)
    {
      SequenceBegin ("", "[" + type.Name, "  ", ",", "", "]");
      return this;
    }

    public IToTextBuilderBase beginInstance (Type type)
    {
      return AppendInstanceBegin (type);
    }

    private IToTextBuilderBase AppendInstanceEnd ()
    {
      SequenceEnd ();
      return this;
    }

    public IToTextBuilderBase endInstance ()
    {
      return AppendInstanceEnd ();
    }

    public IToTextBuilderBase AppendSequenceEnd ()
    {
      SequenceEnd ();
      return this;
    }

    protected abstract void SequenceEnd ();

    public IToTextBuilderBase se ()
    {
      return AppendSequenceEnd ();
    }

    public IToTextBuilderBase AppendSequenceElement (object obj)
    {
      //Assertion.IsTrue (IsInSequence);
      _toTextProvider.ToText (obj, this);
      return this;
    }

    public IToTextBuilderBase e (object obj)
    {
      return AppendSequenceElement (obj);
    }

    public IToTextBuilderBase AppendSequenceElements (params object[] sequenceElements)
    {
      Assertion.IsTrue (IsInSequence);
      foreach (var obj in sequenceElements)
      {
        AppendSequenceElement (obj);
      }
      return this;
    }

    public IToTextBuilderBase elements (params object[] sequenceElements)
    {
      return AppendSequenceElements (sequenceElements);
    }

    public IToTextBuilderBase elementsNumbered (string s1, int i0, int i1)
    {
      for (int i = i0; i <= i1; ++i)
      {
        AppendSequenceElement (s1 + i);
      }
      return this;
    }

    public void WriteRawElementBegin ()
    {
      IsInRawSequence = true;
      BeforeAppendElement();
    }

    public void WriteRawElementEnd ()
    {
      AfterAppendElement ();
      IsInRawSequence = false;
    }

    //public abstract IToTextBuilderBase EmitNamedSequenceBegin ();
    //public abstract IToTextBuilderBase EmitNamedSequenceEnd ();

    //public override IToTextBuilderBase EmitNamedSequenceBegin ()
    //{
    //  BeforeAppendElement ();
    //  return SequenceBegin (sequencePrefix, firstElementPrefix, otherElementPrefix, elementPostfix, sequencePostfix);
    //}

    //public override IToTextBuilderBase EmitNamedSequenceEnd ()
    //{
    //  throw new System.NotImplementedException ();
    //}



    private static string RightUntilChar (string s, char separator)
    {
      int iSeparator = s.LastIndexOf (separator);
      if (iSeparator > 0)
      {
        return s.Substring (iSeparator + 1, s.Length - iSeparator - 1);
      }
      else
      {
        return s;
      }
    }

    //public virtual IToTextBuilderBase WriteElement (object obj)
    //{
    //  _toTextProvider.ToText (obj, this);
    //  return this;
    //}
  }
}