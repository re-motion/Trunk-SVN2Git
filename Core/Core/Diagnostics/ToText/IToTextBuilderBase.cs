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
using System.Collections;
using System.Linq.Expressions;

namespace Remotion.Diagnostics.ToText
{
  public interface IToTextBuilderBase
  {
    /// <summary>
    /// <para>Get/set whether the TextBuilder allows explicit emission of newlines (see <see cref="WriteNewLine()"/>, <see cref="WriteNewLine(int)"/>).</para>
    /// </summary>
    bool AllowNewline { get; set; }

    /// <summary>
    /// <para>Get/set whether the TextBuilder writes to his underlying stream.</para>
    /// </summary>
    bool Enabled { get; set; }
 
    /// <summary>
    /// <para>Get whether the TextBuilder is currently in a sequence; see <see cref="WriteSequenceBegin"/></para>
    /// </summary>
    bool IsInSequence { get; }

    /// <summary>
    /// The <see cref="ToText.ToTextProvider"/> used by the <see cref="ToTextBuilder"/>.
    /// </summary>
    ToTextProvider ToTextProvider { get; set; }

    /// <summary>
    /// <para>Get The TextBuilder's current output complexity level. The output complexity level can be used by type/interface handlers
    /// to tailor the created output for different compactness needs of the user.</para>
    /// </summary>
    ToTextBuilderBase.ToTextBuilderOutputComplexityLevel OutputComplexity { get; }

    /// <summary>
    /// Write the following if the output complexity level is set to "skeleton" or higher.
    /// </summary>
    IToTextBuilderBase writeIfSkeletonOrHigher { get; }
    /// <summary>
    /// Write the following if the output complexity level is set to "basic" or higher.
    /// </summary>
    IToTextBuilderBase writeIfBasicOrHigher { get; }
    /// <summary>
    /// Write the following if the output complexity level is set to "medium" or higher.
    /// </summary>
    IToTextBuilderBase writeIfMediumOrHigher { get; }
    /// <summary>
    /// Write the following if the output complexity level is set to "complex" or higher.
    /// </summary>
    IToTextBuilderBase writeIfComplexOrHigher { get; }
    /// <summary>
    /// Write the following if the output complexity level is set to "full" or higher.
    /// </summary>
    IToTextBuilderBase writeIfFull { get; }
    /// <summary>
    /// Write the following if the output complexity level is to the passed <see cref="ToTextBuilderBase.ToTextBuilderOutputComplexityLevel"/>.
    /// </summary>
    IToTextBuilderBase WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderBase.ToTextBuilderOutputComplexityLevel complexityLevel);

    /// <summary>
    /// <para>Set the output complexity level to "disable", effectively disabling the <see cref="ToTextBuilder"/>.</para>
    /// </summary>
    void SetOutputComplexityToDisable ();
    /// <summary>
    /// <para>Set the output complexity level to "skeleton", the lowest complexity level where output is still produced.</para>
    /// </summary>
    void SetOutputComplexityToSkeleton ();
    /// <summary>
    /// <para>Set the output complexity level to "basic".</para>
    /// </summary>
    void SetOutputComplexityToBasic ();
    /// <summary>
    /// <para>Set the output complexity level to "medium".</para>
    /// </summary>
    void SetOutputComplexityToMedium ();
    /// <summary>
    /// <para>Set the output complexity level to "complex".</para>
    /// </summary>
    void SetOutputComplexityToComplex ();
    /// <summary>
    /// <para>Set the output complexity level to "full".</para>
    /// </summary>
    void SetOutputComplexityToFull ();



    /// <summary>
    /// <para>Check if e.g. all open sequences have been closed and return a string which represents the current <see cref="ToTextBuilder"/> text.</para> 
    /// </summary>
    string CheckAndConvertToString ();
    
    /// <summary>
    /// <para>Flushes the underlying stream writer.</para> 
    /// </summary>
    IToTextBuilderBase Flush ();


    /// <summary>
    /// <para>Starts raw element writing. Until <see cref="WriteRawElementEnd"/> is called, calls which write directly to the underlying
    /// stream writer (e.g. <see cref="WriteRawString"/>, <see cref="WriteRawChar"/>) are permitted.</para> 
    /// </summary>
    IToTextBuilderBase WriteRawElementBegin ();

    /// <summary>
    /// <para>Ends raw element writing started with a call to <see cref="WriteRawElementBegin"/>.</para> 
    /// </summary>
    IToTextBuilderBase WriteRawElementEnd ();

    
    /// <summary>
    /// <para>Writes the number of newline-tokens passed as an argument.</para>
    /// <para>Shorthand notation: <see cref="nl(int)"/></para>
    /// </summary>
    /// <remarks>
    /// <para>Note that for e.g. a <see cref="ToTextBuilder"/>
    /// writing an XML stream a newline will be expressed as an XML-tag, not as carriage return/linefeed characters.</para>
    /// </remarks>
    IToTextBuilderBase WriteNewLine (int numberNewlines);
    
    /// <summary>
    /// <para>Writes the passed number of newline-tokens. Shorthand notation for <see cref="WriteNewLine(int)"/></para>
    /// </summary>   
    IToTextBuilderBase nl (int numberNewlines);
    
    /// <summary>
    /// <para>Writes one newline-token. Equivalent to <see cref="WriteNewLine(int)"/> with an argument of 1.</para> 
    /// <para>Shorthand notation: <see cref="nl()"/></para>
    /// </summary>
    IToTextBuilderBase WriteNewLine ();
    
    /// <summary>
    /// <para>Writes one newline-token. Shorthand notation for <see cref="WriteNewLine()"/></para>
    /// </summary>   
    IToTextBuilderBase nl ();

    /// <summary>
    /// <para>Writes the given string directly to the output stream. The string is written directly to the output stream, 
    /// without considering sequences etc. A raw scope must be open for this call to be valid (see <see cref="WriteRawElementBegin"/>).</para>
    /// <para>Shorthand notation: <see cref="s"/></para>
    /// </summary>
    IToTextBuilderBase WriteRawString (string s);
    
    /// <summary>
    /// <para>Writes the given string directly to the output stream. Shorthand notation for <see cref="WriteRawString(string)"/>.</para>
    /// </summary>
    IToTextBuilderBase s (string s);
    
    /// <summary>
    /// <para>Writes the given string directly to the output stream, replacing newline, tabulator, etc characters with an escaped representation.
    /// The string is written directly to the output stream, 
    /// without considering sequences etc. A raw scope must be open for this call to be valid (see <see cref="WriteRawElementBegin"/>).</para>
    /// <para>Shorthand notation: <see cref="sEsc"/></para>
    /// </summary>
    IToTextBuilderBase WriteRawStringEscaped (string s);
    
    /// <summary>
    /// <para>Writes the given string directly to the output stream, replacing newline, tabulator, etc characters with 
    /// an escaped representation. Shorthand notation for <see cref="WriteRawStringEscaped(string)"/>.</para>
    /// </summary>
    IToTextBuilderBase sEsc (string s);

    /// <summary>
    /// <para>Writes the given character directly to the output stream. The string is written directly to the output stream, 
    /// without considering sequences etc. A raw scope must be open for this call to be valid (see <see cref="WriteRawElementBegin"/>).</para>
    /// </summary>
    IToTextBuilderBase WriteRawChar (char c);



    /// <summary>
    /// <para>Writes the object argument directly to the underlying stream. A raw scope must be open for this call to be valid (see <see cref="WriteRawElementBegin"/>).</para>
    /// </summary>
    IToTextBuilderBase WriteRaw (Object obj);



    /// <summary>
    /// <para>Begins a sequence with a given name, literal set of sequence-begin/end and element-prefix/postifx and -seperarator strings.</para>
    /// <para>Shorthand notation: <see cref="sbLiteral(string,string,string,string,string)"/>.</para>
    /// </summary>
    /// <remarks><para>
    /// Note that not all classes implementing <see cref="IToTextBuilderBase"/> are required to support this method.
    /// Use semantic sequence-begin-calls (e.g. <see cref="WriteSequenceBegin"/>, <see cref="WriteInstanceBegin"/>, <see cref="WriteSequenceArrayBegin"/>)
    /// to be compatible with all <see cref="IToTextBuilderBase"/> implementations.
    /// </para>
    /// </remarks>remarks>
    IToTextBuilderBase WriteSequenceLiteralBegin (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix);

    /// <summary>
    /// <para>Begins a sequence with a given literal pair of sequence-begin and -end strings.
    /// See <see cref="WriteSequenceLiteralBegin"/>.
    /// </para>
    /// <para>Shorthand notation: <see cref="sbLiteral(string,string)"/>.</para>
    /// </summary>
    IToTextBuilderBase sbLiteral (string sequencePrefix, string sequencePostfix);
    /// <summary>
    /// <para>Begins a sequence with a given literal pair of sequence-begin and -end strings, together with an element separator.
    /// See <see cref="WriteSequenceLiteralBegin"/>.
    /// </para>
    /// <para>Shorthand notation: <see cref="sbLiteral(string,string)"/>.</para>
    /// </summary>
    IToTextBuilderBase sbLiteral (string sequencePrefix, string separator, string sequencePostfix);
    /// <summary>
    /// <para>Begins a sequence with a given literal pair of sequence-begin and -end strings, together with an element separator.
    /// See <see cref="WriteSequenceLiteralBegin"/>.
    /// </para>
    /// </summary>
    IToTextBuilderBase sbLiteral (string sequencePrefix, string elementPostfix, string elementPrefix, string separator, string sequencePostfix);


    /// <summary>
    /// <para>Begins a new sequence, i.e. a number of consecutive elements bracketed by the sequence 
    /// which are automatically output in a seperated manner. </para>
    /// <para>Shorthand notation: <see cref="sb"/>.</para>
    /// </summary>
    /// <remarks><para>
    /// The way the sequence begin and end and the sequence elements are output and 
    /// separated is decided by the <see cref="ToTextBuilder"/>.</para>
    /// <para>Sequences can be nested: Each new sequence pushed the current sequence (if any) onto a sequence stack; ending the sequence with a call to <see cref="WriteSequenceEnd"/>
    /// pops the previous sequence from the stack making it active again.</para>
    /// <para>Within a sequence elements in the sequence can be emitted using calls to the WriteElement family of members
    /// (<see cref="WriteElement(object)"/>, <see cref="WriteElement(string, Object)"/>, <see cref="WriteElement{T}"/>).
    /// </para>
    /// </remarks>
    IToTextBuilderBase WriteSequenceBegin ();
    
    /// <summary>
    /// <para>Begins a new sequence. Shorthand notation for <see cref="WriteSequenceBegin"/>.</para>
    /// </summary>
    IToTextBuilderBase sb ();


    /// <summary>
    /// <para>Applies <see cref="ToText"/> to the passed argument and emits the result through the <see cref="ToTextBuilder"/>.</para>
    /// <para>Shorthand notation: <see cref="e(object)"/>.</para>
    /// </summary>
    IToTextBuilderBase WriteElement (object obj);
    
    /// <summary>
    /// <para>Applies <see cref="ToText"/> to the passed argument and emits the result through the <see cref="ToTextBuilder"/> as a name-value-pair.</para>
    /// <para>Shorthand notation: <see cref="e(string,object)"/>.</para>
    /// </summary>
    IToTextBuilderBase WriteElement (string name, Object obj);

    /// <summary>
    /// <para>Applies <see cref="ToText"/> to the passed variable and emits the result through the <see cref="ToTextBuilder"/>
    /// as a name-value-pair. Usage: <c>ttb.e(() => myVar);</c>.
    /// </para>
    /// <para>Shorthand notation: <see cref="e{T}"/>.</para>
    /// </summary>
    /// <remarks><para>
    /// Since the variable to write is passed in a lambda expression the name and value can be 
    /// deduced from just one term, eliminating name-value-mismatch-errors.</para>
    /// <example><code>
    /// var myList = List.New(5,3,1);
    /// toTextBuilder.e(() => myList);
    /// var result = toTextBuilder.CheckAndConvertToString(); // returns: myList={5,3,1}
    /// </code></example>
    /// </remarks>
    IToTextBuilderBase WriteElement<T> (Expression<Func<T>> expression);
    
    /// <summary>
    /// <para>Applies <see cref="ToText"/> to the passed argument and emits the result through the <see cref="ToTextBuilder"/>.</para>
    /// <para>Shorthand notation for <see cref="WriteElement(object)"/>.</para>
    /// </summary>
    IToTextBuilderBase e (Object obj);
    
    /// <summary>
    /// <para>Applies <see cref="ToText"/> to the passed argument and emits the result through the <see cref="ToTextBuilder"/> as a name-value-pair.</para>
    /// <para>Shorthand notation for <see cref="WriteElement{T}"/>.</para>
    /// </summary>
    IToTextBuilderBase e<T> (Expression<Func<T>> expression);
    
    /// <summary>
    /// <para>Applies <see cref="ToText"/> to the passed argument and emits the result through the <see cref="ToTextBuilder"/> as a name-value-pair.</para>
    /// <para>Shorthand notation for <see cref="WriteElement(string,object)"/>.</para>
    /// </summary>
    IToTextBuilderBase e (string name, Object obj);


    /// <summary>
    /// <para>Closes a sequence, activating the previously active sequence.</para>
    /// <para>Shorthand notation: <see cref="se"/>.</para>
    /// </summary>
    IToTextBuilderBase WriteSequenceEnd ();
    /// <summary>
    /// <para>Closes a sequence.</para>
    /// <para>Shorthand notation for <see cref="WriteSequenceEnd"/>.</para>
    /// </summary>
    IToTextBuilderBase se ();





    /// <summary>
    /// <para>Writes the passed parameters out as sequence elements.</para>
    /// <para>Shorthand notation: <see cref="elements"/>.</para>
    /// </summary>
    /// <remarks>
    /// <example><code>
    /// toTextBuilder.sb().WriteSequenceElements(x0,x1,x2,x3,x4).se(); 
    /// // Is equivalent to:
    /// toTextBuilder.sb().WriteElement(x0).WriteElement(x1).WriteElement(x2).WriteElement(x3).WriteElement(x4).se();
    /// </code></example>
    /// </remarks>
    IToTextBuilderBase WriteSequenceElements (params object[] sequenceElements);


    /// <summary>
    /// <para>Writes the passed parameters out as sequence elements.</para>
    /// <para>Shorthand notation for <see cref="WriteSequenceElements"/>.</para>
    /// </summary>
    /// <remarks>
    /// <example><code>
    /// toTextBuilder.sb().elements(x0,x1,x2,x3,x4).se(); 
    /// // Is equivalent to:
    /// toTextBuilder.sb().e(x0).e(x1).e(x2).e(x3).e(x4).se();
    /// </code></example>
    /// </remarks>
    IToTextBuilderBase elements (params object[] sequenceElements);
    
    /// <summary>
    /// <para>Writes out as sequence elements of the form <c>s+i0,s+i0+1,...,s+i1</c>.</para>
    /// </summary>
    /// <remarks>
    /// <example><code>
    /// toTextBuilder.sbLiteral ("", "|", ">", "", "").elementsNumbered ("a", 1, 5).se();
    /// var result = toTextBuilder.CheckAndConvertToString(); // returns: |a1>|a2>|a3>|a4>|a5>
    /// </code></example>
    /// </remarks>    
    IToTextBuilderBase elementsNumbered (string s, int i0, int i1);


    /// <summary>
    /// <para>Writes the passed parameters out as a standard sequence.</para>
    /// <para>Shorthand notation: <see cref="sequence"/>.</para>
    /// </summary>
    /// <remarks>
    /// <example><code>
    /// toTextBuilder.sequence(x0,x1,x2,x3,x4); 
    /// // Is equivalent to:
    /// toTextBuilder.sb().e(x0).e(x1).e(x2).e(x3).e(x4).se();
    /// </code></example>
    /// </remarks>
    IToTextBuilderBase WriteSequence (params object[] sequenceElements);
    
    /// <summary>
    /// <para>Writes the passed parameters out as a standard sequence.</para>
    /// <example><code>
    /// toTextBuilder.sequence(x0,x1,x2,x3,x4); 
    /// // Is equivalent to:
    /// toTextBuilder.sb().e(x0).e(x1).e(x2).e(x3).e(x4).se();
    /// </code></example>
    /// <para>Shorthand notation for <see cref="WriteSequence"/>.</para>
    /// </summary>    
    IToTextBuilderBase sequence (params object[] sequenceElements);


    /// <summary>
    /// <para>Begins a new sequence for writing an instance of the passed Type.</para>
    /// <para>Shorthand notation: <see cref="ib"/>.</para>
    /// </summary>
    IToTextBuilderBase WriteInstanceBegin (Type type);
    
    /// <summary>
    /// <para>Begins a new sequence for writing an instance of the passed Type.</para>
    /// <para>Shorthand notation for <see cref="WriteInstanceBegin"/>.</para>
    /// </summary>
    IToTextBuilderBase ib (Type type);
    
    /// <summary>
    /// <para>Begins a new sequence for writing an instance of the Type of the generic argument.</para>
    /// </summary>
    IToTextBuilderBase ib<T> ();
    
    /// <summary>
    /// <para>Closes an instance sequence.</para>
    /// </summary>
    IToTextBuilderBase ie();

    /// <summary>
    /// <para>Begins a new sequence for writing out the elements of an array. Note that arrays are handled automatically 
    /// by the <see cref="WriteElement(object)"/>, etc methods, so normally there is no need to call this method explicitely.</para>
    /// </summary>
    IToTextBuilderBase WriteSequenceArrayBegin ();
    
    /// <summary>
    /// <para>Writes out the passed array. Note that arrays are also handled automatically by <see cref="WriteElement(object)"/> so
    /// normally there is no need to call this method explicitely.</para>
    /// <para>Shorthand notation: <see cref="array"/>.</para>
    /// </summary>
    IToTextBuilderBase WriteArray (Array array);
    
    /// <summary>
    /// <para>Writes out the passed array.</para>
    /// <para>Shorthand notation for <see cref="WriteArray"/>.</para>
    /// </summary>
    IToTextBuilderBase array (Array array);

    /// <summary>
    /// <para>Writes out the passed enumerable. Note that enumerables are handled automatically 
    /// by <see cref="WriteElement(object)"/>-methods, so normally there is no need to call this method explicitely.</para>
    /// </summary>
    IToTextBuilderBase WriteEnumerable (IEnumerable enumerable);
    
    /// <summary>
    /// <para>Writes out the passed enumerable.</para>
    /// <para>Shorthand notation for <see cref="WriteEnumerable"/>.</para>
    /// </summary>
    IToTextBuilderBase enumerable (IEnumerable enumerable);

    /// <summary>
    /// <para>Writes out the passed dictionary. Note that dictionaries are handled automatically 
    /// by <see cref="WriteElement(object)"/>-methods, so normally there is no need to call this method explicitely.</para>
    /// </summary>
    IToTextBuilderBase WriteDictionary (IDictionary dictionary);
    
    /// <summary>
    /// <para>Writes out the passed dictionary.</para>
    /// <para>Shorthand notation for <see cref="WriteDictionary"/>.</para>
    /// </summary>
    IToTextBuilderBase dictionary (IDictionary dictionary);
  }
}