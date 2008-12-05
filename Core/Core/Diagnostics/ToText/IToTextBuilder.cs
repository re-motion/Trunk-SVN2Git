// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System;
using System.Collections;
using System.Linq.Expressions;
using Remotion.Diagnostics.ToText.Infrastructure;

namespace Remotion.Diagnostics.ToText
{
  public interface IToTextBuilder : IDisposable
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
    IToTextBuilder writeIfSkeletonOrHigher { get; }
    /// <summary>
    /// Write the following if the output complexity level is set to "basic" or higher.
    /// </summary>
    IToTextBuilder writeIfBasicOrHigher { get; }
    /// <summary>
    /// Write the following if the output complexity level is set to "medium" or higher.
    /// </summary>
    IToTextBuilder writeIfMediumOrHigher { get; }
    /// <summary>
    /// Write the following if the output complexity level is set to "complex" or higher.
    /// </summary>
    IToTextBuilder writeIfComplexOrHigher { get; }
    /// <summary>
    /// Write the following if the output complexity level is set to "full" or higher.
    /// </summary>
    IToTextBuilder writeIfFull { get; }
    /// <summary>
    /// Write the following if the output complexity level is to the passed <see cref="ToTextBuilderBase.ToTextBuilderOutputComplexityLevel"/>.
    /// </summary>
    IToTextBuilder WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderBase.ToTextBuilderOutputComplexityLevel complexityLevel);

    /// <summary>
    /// <para>Set the output complexity level to "disable", effectively disabling the <see cref="ToTextBuilder"/>.</para>
    /// </summary>
    IToTextBuilder SetOutputComplexityToDisable ();
    /// <summary>
    /// <para>Set the output complexity level to "skeleton", the lowest complexity level where output is still produced.</para>
    /// </summary>
    IToTextBuilder SetOutputComplexityToSkeleton ();
    /// <summary>
    /// <para>Set the output complexity level to "basic".</para>
    /// </summary>
    IToTextBuilder SetOutputComplexityToBasic ();
    /// <summary>
    /// <para>Set the output complexity level to "medium".</para>
    /// </summary>
    IToTextBuilder SetOutputComplexityToMedium ();
    /// <summary>
    /// <para>Set the output complexity level to "complex".</para>
    /// </summary>
    IToTextBuilder SetOutputComplexityToComplex ();
    /// <summary>
    /// <para>Set the output complexity level to "full".</para>
    /// </summary>
    IToTextBuilder SetOutputComplexityToFull ();



    /// <summary>
    /// <para>Check if e.g. all open sequences have been closed and return a string which represents the current <see cref="ToTextBuilder"/> text.</para> 
    /// </summary>
    string CheckAndConvertToString ();
    
    /// <summary>
    /// <para>Flushes the underlying stream writer.</para> 
    /// </summary>
    IToTextBuilder Flush ();


    /// <summary>
    /// <para>Starts raw element writing. Until <see cref="WriteRawElementEnd"/> is called, calls which write directly to the underlying
    /// stream writer (e.g. <see cref="WriteRawString"/>, <see cref="WriteRawChar"/>) are permitted.</para> 
    /// </summary>
    IToTextBuilder WriteRawElementBegin ();

    /// <summary>
    /// <para>Ends raw element writing started with a call to <see cref="WriteRawElementBegin"/>.</para> 
    /// </summary>
    IToTextBuilder WriteRawElementEnd ();

    
    /// <summary>
    /// <para>Writes the number of newline-tokens passed as an argument.</para>
    /// <para>Shorthand notation: <see cref="nl(int)"/></para>
    /// </summary>
    /// <remarks>
    /// <para>Note that for e.g. a <see cref="ToTextBuilder"/>
    /// writing an XML stream a newline will be expressed as an XML-tag, not as carriage return/linefeed characters.</para>
    /// </remarks>
    IToTextBuilder WriteNewLine (int numberNewlines);
    
    /// <summary>
    /// <para>Writes the passed number of newline-tokens. Shorthand notation for <see cref="WriteNewLine(int)"/></para>
    /// </summary>   
    IToTextBuilder nl (int numberNewlines);
    
    /// <summary>
    /// <para>Writes one newline-token. Equivalent to <see cref="WriteNewLine(int)"/> with an argument of 1.</para> 
    /// <para>Shorthand notation: <see cref="nl()"/></para>
    /// </summary>
    IToTextBuilder WriteNewLine ();
    
    /// <summary>
    /// <para>Writes one newline-token. Shorthand notation for <see cref="WriteNewLine()"/></para>
    /// </summary>   
    IToTextBuilder nl ();

    /// <summary>
    /// <para>Writes the given string directly to the output stream. The string is written directly to the output stream, 
    /// without considering sequences etc. A raw scope must be open for this call to be valid (see <see cref="WriteRawElementBegin"/>).</para>
    /// <para>Shorthand notation: <see cref="s"/></para>
    /// </summary>
    IToTextBuilder WriteRawString (string s);
    
    /// <summary>
    /// <para>Writes the given string directly to the output stream. Shorthand notation for <see cref="WriteRawString(string)"/>.</para>
    /// </summary>
    IToTextBuilder s (string s);
    
    /// <summary>
    /// <para>Writes the given string directly to the output stream, replacing newline, tabulator, etc characters with an escaped representation.
    /// The string is written directly to the output stream, 
    /// without considering sequences etc. A raw scope must be open for this call to be valid (see <see cref="WriteRawElementBegin"/>).</para>
    /// <para>Shorthand notation: <see cref="sEsc"/></para>
    /// </summary>
    IToTextBuilder WriteRawStringEscaped (string s);
    
    /// <summary>
    /// <para>Writes the given string directly to the output stream, replacing newline, tabulator, etc characters with 
    /// an escaped representation. Shorthand notation for <see cref="WriteRawStringEscaped(string)"/>.</para>
    /// </summary>
    IToTextBuilder sEsc (string s);

    /// <summary>
    /// <para>Writes the given character directly to the output stream. The string is written directly to the output stream, 
    /// without considering sequences etc. A raw scope must be open for this call to be valid (see <see cref="WriteRawElementBegin"/>).</para>
    /// </summary>
    IToTextBuilder WriteRawChar (char c);



    /// <summary>
    /// <para>Writes the object argument directly to the underlying stream. A raw scope must be open for this call to be valid (see <see cref="WriteRawElementBegin"/>).</para>
    /// </summary>
    IToTextBuilder WriteRaw (Object obj);



    /// <summary>
    /// <para>Begins a sequence with a given name, literal set of sequence-begin/end and element-prefix/postifx and -seperarator strings.</para>
    /// <para>Shorthand notation: <see cref="sbLiteral(string,string,string,string,string)"/>.</para>
    /// </summary>
    /// <remarks><para>
    /// Note that not all classes implementing <see cref="IToTextBuilder"/> are required to support this method.
    /// Use semantic sequence-begin-calls (e.g. <see cref="WriteSequenceBegin"/>, <see cref="WriteInstanceBegin"/>, <see cref="WriteSequenceArrayBegin"/>)
    /// to be compatible with all <see cref="IToTextBuilder"/> implementations.
    /// </para>
    /// </remarks>remarks>
    IToTextBuilder WriteSequenceLiteralBegin (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix);

    /// <summary>
    /// <para>Begins a sequence with a given literal pair of sequence-begin and -end strings.
    /// See <see cref="WriteSequenceLiteralBegin"/>.
    /// </para>
    /// <para>Shorthand notation: <see cref="sbLiteral(string,string)"/>.</para>
    /// </summary>
    IToTextBuilder sbLiteral (string sequencePrefix, string sequencePostfix);
    /// <summary>
    /// <para>Begins a sequence with a given literal pair of sequence-begin and -end strings, together with an element separator.
    /// See <see cref="WriteSequenceLiteralBegin"/>.
    /// </para>
    /// <para>Shorthand notation: <see cref="sbLiteral(string,string)"/>.</para>
    /// </summary>
    IToTextBuilder sbLiteral (string sequencePrefix, string separator, string sequencePostfix);
    /// <summary>
    /// <para>Begins a sequence with a given literal pair of sequence-begin and -end strings, together with an element separator.
    /// See <see cref="WriteSequenceLiteralBegin"/>.
    /// </para>
    /// </summary>
    IToTextBuilder sbLiteral (string sequencePrefix, string elementPostfix, string elementPrefix, string separator, string sequencePostfix);


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
    IToTextBuilder WriteSequenceBegin ();
    
    /// <summary>
    /// <para>Begins a new sequence. Shorthand notation for <see cref="WriteSequenceBegin"/>.</para>
    /// </summary>
    IToTextBuilder sb ();


    /// <summary>
    /// <para>Applies <see cref="ToText"/> to the passed argument and emits the result through the <see cref="ToTextBuilder"/>.</para>
    /// <para>Shorthand notation: <see cref="e(object)"/>.</para>
    /// </summary>
    IToTextBuilder WriteElement (object obj);
    
    /// <summary>
    /// <para>Applies <see cref="ToText"/> to the passed argument and emits the result through the <see cref="ToTextBuilder"/> as a name-value-pair.</para>
    /// <para>Shorthand notation: <see cref="e(string,object)"/>.</para>
    /// </summary>
    IToTextBuilder WriteElement (string name, Object obj);

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
    IToTextBuilder WriteElement<T> (Expression<Func<T>> expression);

    
    /// <summary>
    /// <para>Applies <see cref="ToText"/> to the passed argument and emits the result through the <see cref="ToTextBuilder"/>.</para>
    /// <para>Shorthand notation for <see cref="WriteElement(object)"/>.</para>
    /// </summary>
    IToTextBuilder e (Object obj);
    
    /// <summary>
    /// <para>Applies <see cref="ToText"/> to the passed argument and emits the result through the <see cref="ToTextBuilder"/> as a name-value-pair.</para>
    /// <para>Shorthand notation for <see cref="WriteElement{T}"/>.</para>
    /// </summary>
    IToTextBuilder e<T> (Expression<Func<T>> expression);
    
    /// <summary>
    /// <para>Applies <see cref="ToText"/> to the passed argument and emits the result through the <see cref="ToTextBuilder"/> as a name-value-pair.</para>
    /// <para>Shorthand notation for <see cref="WriteElement(string,object)"/>.</para>
    /// </summary>
    IToTextBuilder e (string name, Object obj);

    /// <summary>
    /// <para>Writes element using <see cref="e(string,object)"/> if the passed Object is not null.</para>
    /// </summary>
    IToTextBuilder eIfNotNull (string name, Object obj);

    /// <summary>
    /// <para>Writes element using <see cref="e(object)"/> if the passed Object is not null.</para>
    /// </summary>
    IToTextBuilder eIfNotNull (Object obj);

    /// <summary>
    /// <para>Writes element using <see cref="e(string,object)"/> if the passed Object is not null.</para>
    /// </summary>
    IToTextBuilder eIfNotEqualTo (string name, Object obj, Object notEqualObj);

    /// <summary>
    /// <para>Closes a sequence, activating the previously active sequence.</para>
    /// <para>Shorthand notation: <see cref="se"/>.</para>
    /// </summary>
    IToTextBuilder WriteSequenceEnd ();
    /// <summary>
    /// <para>Closes a sequence.</para>
    /// <para>Shorthand notation for <see cref="WriteSequenceEnd"/>.</para>
    /// </summary>
    IToTextBuilder se ();





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
    IToTextBuilder WriteSequenceElements (params object[] sequenceElements);


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
    IToTextBuilder elements (params object[] sequenceElements);
    
    /// <summary>
    /// <para>Writes out as sequence elements of the form <c>s+i0,s+i0+1,...,s+i1</c>.</para>
    /// </summary>
    /// <remarks>
    /// <example><code>
    /// toTextBuilder.sbLiteral ("", "|", ">", "", "").elementsNumbered ("a", 1, 5).se();
    /// var result = toTextBuilder.CheckAndConvertToString(); // returns: |a1>|a2>|a3>|a4>|a5>
    /// </code></example>
    /// </remarks>    
    IToTextBuilder elementsNumbered (string s, int i0, int i1);


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
    IToTextBuilder WriteSequence (params object[] sequenceElements);
    
    /// <summary>
    /// <para>Writes the passed parameters out as a standard sequence.</para>
    /// <para>Shorthand notation for <see cref="WriteSequence"/>.</para>
    /// </summary>
    /// <remarks>
    /// <example><code>
    /// toTextBuilder.sequence(x0,x1,x2,x3,x4); 
    /// // Is equivalent to:
    /// toTextBuilder.sb().e(x0).e(x1).e(x2).e(x3).e(x4).se();
    /// </code></example>
    /// </remarks>
    IToTextBuilder sequence (params object[] sequenceElements);


    /// <summary>
    /// <para>Begins a new sequence for writing an instance of the passed Type.</para>
    /// <para>Shorthand notation: <see cref="ib"/>.</para>
    /// </summary>
    IToTextBuilder WriteInstanceBegin (Type type, string shortTypeName);
    


    /// <summary>
    /// <para>Begins a new sequence for writing an instance of the passed Type.</para>
    /// <para>Shorthand notation for <see cref="WriteInstanceBegin"/>.</para>
    /// </summary>
    IToTextBuilder ib (Type type);
    
    /// <summary>
    /// <para>Begins a new sequence for writing an instance of the Type of the generic argument.</para>
    /// </summary>
    IToTextBuilder ib<T> ();

    /// <summary>
    /// <para>Begins a new sequence for writing an instance of the Type of the generic argument, using
    /// the passed shortTypeName in the output.</para>
    /// </summary>
    IToTextBuilder ib<T> (string shortTypeName);

    /// <summary>
    /// <para>Closes an instance sequence.</para>
    /// </summary>
    IToTextBuilder ie();

    /// <summary>
    /// <para>Begins a new sequence for writing out the elements of an array. Note that arrays are handled automatically 
    /// by the <see cref="WriteElement(object)"/>, etc methods, so normally there is no need to call this method explicitely.</para>
    /// </summary>
    IToTextBuilder WriteSequenceArrayBegin ();
    
    /// <summary>
    /// <para>Writes out the passed array. Note that arrays are also handled automatically by <see cref="WriteElement(object)"/> so
    /// normally there is no need to call this method explicitely.</para>
    /// <para>Shorthand notation: <see cref="array"/>.</para>
    /// </summary>
    IToTextBuilder WriteArray (Array array);
    
    /// <summary>
    /// <para>Writes out the passed array.</para>
    /// <para>Shorthand notation for <see cref="WriteArray"/>.</para>
    /// </summary>
    IToTextBuilder array (Array array);

    /// <summary>
    /// <para>Writes out the passed enumerable. Note that enumerables are handled automatically 
    /// by <see cref="WriteElement(object)"/>-methods, so normally there is no need to call this method explicitely.</para>
    /// </summary>
    IToTextBuilder WriteEnumerable (IEnumerable enumerable);
    
    /// <summary>
    /// <para>Writes out the passed enumerable.</para>
    /// <para>Shorthand notation for <see cref="WriteEnumerable"/>.</para>
    /// </summary>
    IToTextBuilder enumerable (IEnumerable enumerable);

    /// <summary>
    /// <para>Writes out the passed dictionary. Note that dictionaries are handled automatically 
    /// by <see cref="WriteElement(object)"/>-methods, so normally there is no need to call this method explicitely.</para>
    /// </summary>
    IToTextBuilder WriteDictionary (IDictionary dictionary);
    
    /// <summary>
    /// <para>Writes out the passed dictionary.</para>
    /// <para>Shorthand notation for <see cref="WriteDictionary"/>.</para>
    /// </summary>
    IToTextBuilder dictionary (IDictionary dictionary);



    IToTextBuilder indent ();
    IToTextBuilder unindent ();

    void Close ();
  }
}