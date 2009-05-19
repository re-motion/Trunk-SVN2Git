using System;
using System.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Infrastructure
{
  public interface IClientScriptManager
  {
    /// <summary>
    /// Gets the concrete instance wrapped by this <see cref="ClientScriptManager"/> wrapper.
    /// </summary>
    /// <exception cref="NotSupportedException">This is a stub implementation which does not contain an <see cref="ClientScriptManager"/>. </exception>
    ClientScriptManager WrappedInstance { get; }

    /// <summary>
    /// Registers an event reference for validation with <see cref="T:System.Web.UI.PostBackOptions"/>.
    /// </summary>
    /// <param name="options">A <see cref="T:System.Web.UI.PostBackOptions"/> object that specifies how client JavaScript is generated to initiate a postback event.
    /// </param>
    void RegisterForEventValidation (PostBackOptions options);

    /// <summary>
    /// Registers an event reference for validation with a unique control ID representing the client control generating the event.
    /// </summary>
    /// <param name="uniqueId">A unique ID representing the client control generating the event.
    /// </param>
    void RegisterForEventValidation (string uniqueId);

    /// <summary>
    /// Registers an event reference for validation with a unique control ID and event arguments representing the client control generating the event.
    /// </summary>
    /// <param name="uniqueId">A unique ID representing the client control generating the event.
    /// </param><param name="argument">Event arguments passed with the client event.
    /// </param><exception cref="T:System.InvalidOperationException">The method is called prior to the <see cref="M:System.Web.UI.Page.Render(System.Web.UI.HtmlTextWriter)"/> method.
    /// </exception>
    void RegisterForEventValidation (string uniqueId, string argument);

    /// <summary>
    /// Validates a client event that was registered for event validation using the <see cref="M:System.Web.UI.ClientScriptManager.RegisterForEventValidation(System.String)"/> method.
    /// </summary>
    /// <param name="uniqueId">A unique ID representing the client control generating the event.
    /// </param>
    void ValidateEvent (string uniqueId);

    /// <summary>
    /// Validates a client event that was registered for event validation using the <see cref="M:System.Web.UI.ClientScriptManager.RegisterForEventValidation(System.String,System.String)"/> method.
    /// </summary>
    /// <param name="uniqueId">A unique ID representing the client control generating the event.
    /// </param><param name="argument">Event arguments passed with the client event.
    /// </param><exception cref="T:System.ArgumentException"><paramref name="uniqueId"/> is null or an empty string ("").
    /// </exception>
    void ValidateEvent (string uniqueId, string argument);

    /// <summary>
    /// Obtains a reference to a client function that, when invoked, initiates a client call back to a server event. The client function for this overloaded method includes a specified control, argument, client script, and context.
    /// </summary>
    /// <returns>
    /// The name of a client function that invokes the client callback. 
    /// </returns>
    /// <param name="control">The server <see cref="T:System.Web.UI.Control"/> that handles the client callback. The control must implement the <see cref="T:System.Web.UI.ICallbackEventHandler"/> interface and provide a <see cref="M:System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent(System.String)"/> method. 
    /// </param><param name="argument">An argument passed from the client script to the server 
    /// <see cref="M:System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent(System.String)"/>  method. 
    /// </param><param name="clientCallback">The name of the client event handler that receives the result of the successful server event. 
    /// </param><param name="context">Client script that is evaluated on the client prior to initiating the callback. The result of the script is passed back to the client event handler. 
    /// </param><exception cref="T:System.ArgumentNullException">The <see cref="T:System.Web.UI.Control"/> specified is null. 
    /// </exception><exception cref="T:System.InvalidOperationException">The <see cref="T:System.Web.UI.Control"/> specified does not implement the <see cref="T:System.Web.UI.ICallbackEventHandler"/> interface.
    /// </exception>
    string GetCallbackEventReference (IControl control, string argument, string clientCallback, string context);

    /// <summary>
    /// Obtains a reference to a client function that, when invoked, initiates a client call back to server events. The client function for this overloaded method includes a specified control, argument, client script, context, and Boolean value.
    /// </summary>
    /// <returns>
    /// The name of a client function that invokes the client callback. 
    /// </returns>
    /// <param name="control">The server <see cref="T:System.Web.UI.Control"/> that handles the client callback. The control must implement the <see cref="T:System.Web.UI.ICallbackEventHandler"/> interface and provide a <see cref="M:System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent(System.String)"/> method. 
    /// </param><param name="argument">An argument passed from the client script to the server 
    /// <see cref="M:System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent(System.String)"/>  method. 
    /// </param><param name="clientCallback">The name of the client event handler that receives the result of the successful server event. 
    /// </param><param name="context">Client script that is evaluated on the client prior to initiating the callback. The result of the script is passed back to the client event handler. 
    /// </param><param name="useAsync">true to perform the callback asynchronously; false to perform the callback synchronously.
    /// </param><exception cref="T:System.ArgumentNullException">The <see cref="T:System.Web.UI.Control"/> specified is null. 
    /// </exception><exception cref="T:System.InvalidOperationException">The <see cref="T:System.Web.UI.Control"/> specified does not implement the <see cref="T:System.Web.UI.ICallbackEventHandler"/> interface.
    /// </exception>
    string GetCallbackEventReference (IControl control, string argument, string clientCallback, string context, bool useAsync);

    /// <summary>
    /// Obtains a reference to a client function that, when invoked, initiates a client call back to server events. The client function for this overloaded method includes a specified control, argument, client script, context, error handler, and Boolean value.
    /// </summary>
    /// <returns>
    /// The name of a client function that invokes the client callback. 
    /// </returns>
    /// <param name="control">The server <see cref="T:System.Web.UI.Control"/> that handles the client callback. The control must implement the <see cref="T:System.Web.UI.ICallbackEventHandler"/> interface and provide a <see cref="M:System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent(System.String)"/> method. 
    /// </param><param name="argument">An argument passed from the client script to the server <see cref="M:System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent(System.String)"/>  method. 
    /// </param><param name="clientCallback">The name of the client event handler that receives the result of the successful server event. 
    /// </param><param name="context">Client script that is evaluated on the client prior to initiating the callback. The result of the script is passed back to the client event handler. 
    /// </param><param name="clientErrorCallback">The name of the client event handler that receives the result when an error occurs in the server event handler. 
    /// </param><param name="useAsync">true to perform the callback asynchronously; false to perform the callback synchronously. 
    /// </param><exception cref="T:System.ArgumentNullException">The <see cref="T:System.Web.UI.Control"/> specified is null. 
    /// </exception><exception cref="T:System.InvalidOperationException">The <see cref="T:System.Web.UI.Control"/> specified does not implement the <see cref="T:System.Web.UI.ICallbackEventHandler"/> interface.
    /// </exception>
    string GetCallbackEventReference (IControl control, string argument, string clientCallback, string context, string clientErrorCallback, bool useAsync);

    /// <summary>
    /// Obtains a reference to a client function that, when invoked, initiates a client call back to server events. The client function for this overloaded method includes a specified target, argument, client script, context, error handler, and Boolean value.
    /// </summary>
    /// <returns>
    /// The name of a client function that invokes the client callback. 
    /// </returns>
    /// <param name="target">The name of a server <see cref="T:System.Web.UI.Control"/> that handles the client callback. The control must implement the <see cref="T:System.Web.UI.ICallbackEventHandler"/> interface and provide a <see cref="M:System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent(System.String)"/> method.
    /// </param><param name="argument">An argument passed from the client script to the server 
    /// <see cref="M:System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent(System.String)"/>  method. 
    /// </param><param name="clientCallback">The name of the client event handler that receives the result of the successful server event. 
    /// </param><param name="context">Client script that is evaluated on the client prior to initiating the callback. The result of the script is passed back to the client event handler.
    /// </param><param name="clientErrorCallback">The name of the client event handler that receives the result when an error occurs in the server event handler. 
    /// </param><param name="useAsync">true to perform the callback asynchronously; false to perform the callback synchronously.
    /// </param>
    string GetCallbackEventReference (string target, string argument, string clientCallback, string context, string clientErrorCallback, bool useAsync);

    /// <summary>
    /// Gets a reference, with javascript: appended to the beginning of it, that can be used in a client event to post back to the server for the specified control and with the specified event arguments.
    /// </summary>
    /// <returns>
    /// A string representing a JavaScript call to the postback function that includes the target control's ID and event arguments.
    /// </returns>
    /// <param name="control">The server control to process the postback .
    /// </param><param name="argument">The parameter passed to the server control. 
    /// </param>
    string GetPostBackClientHyperlink (IControl control, string argument);

    /// <summary>
    /// Gets a reference, with javascript: appended to the beginning of it, that can be used in a client event to post back to the server for the specified control with the specified event arguments and Boolean indication whether to register the post back for event validation.
    /// </summary>
    /// <returns>
    /// A string representing a JavaScript call to the postback function that includes the target control's ID and event arguments.
    /// </returns>
    /// <param name="control">The server control to process the postback
    /// </param><param name="argument">The parameter passed to the server control.
    /// </param><param name="registerForEventValidation">true to register the post back event for validation; false to not register the post back event for validation.
    /// </param>
    string GetPostBackClientHyperlink (IControl control, string argument, bool registerForEventValidation);

    /// <summary>
    /// Returns a string that can be used in a client event to cause postback to the server. The reference string is defined by the specified control that handles the postback and a string argument of additional event information.
    /// </summary>
    /// <returns>
    /// A string that, when treated as script on the client, initiates the postback.
    /// </returns>
    /// <param name="control">The server <see cref="T:System.Web.UI.Control"/> that processes the postback on the server.
    /// </param><param name="argument">A string of optional arguments to pass to the control that processes the postback.
    /// </param><exception cref="T:System.ArgumentNullException">The specified <see cref="T:System.Web.UI.Control"/> is null.
    /// </exception>
    string GetPostBackEventReference (IControl control, string argument);

    /// <summary>
    /// Returns a string to use in a client event to cause postback to the server. The reference string is defined by the specified control that handles the postback and a string argument of additional event information. Optionally, registers the event reference for validation.
    /// </summary>
    /// <returns>
    /// A string that, when treated as script on the client, initiates the postback.
    /// </returns>
    /// <param name="control">The server <see cref="T:System.Web.UI.Control"/> that processes the postback on the server.
    /// </param><param name="argument">A string of optional arguments to pass to <paramref name="control"/>.
    /// </param><param name="registerForEventValidation">true to register the event reference for validation; otherwise, false.
    /// </param><exception cref="T:System.ArgumentNullException">The specified <see cref="T:System.Web.UI.Control"/> is null.
    /// </exception>
    string GetPostBackEventReference (IControl control, string argument, bool registerForEventValidation);

    /// <summary>
    /// Returns a string that can be used in a client event to cause postback to the server. The reference string is defined by the specified <see cref="T:System.Web.UI.PostBackOptions"/> instance.
    /// </summary>
    /// <returns>
    /// A string that, when treated as script on the client, initiates the client postback.
    /// </returns>
    /// <param name="options">A <see cref="T:System.Web.UI.PostBackOptions"/> that defines the postback.
    /// </param><exception cref="T:System.ArgumentNullException">The <see cref="T:System.Web.UI.PostBackOptions"/> parameter is null</exception>
    string GetPostBackEventReference (PostBackOptions options);

    /// <summary>
    /// Returns a string that can be used in a client event to cause postback to the server. The reference string is defined by the specified <see cref="T:System.Web.UI.PostBackOptions"/> object. Optionally, registers the event reference for validation.
    /// </summary>
    /// <returns>
    /// A string that, when treated as script on the client, initiates the client postback.
    /// </returns>
    /// <param name="options">A <see cref="T:System.Web.UI.PostBackOptions"/> that defines the postback.
    /// </param><param name="registerForEventValidation">true to register the event reference for validation; otherwise, false.
    /// </param><exception cref="T:System.ArgumentNullException">The <see cref="T:System.Web.UI.PostBackOptions"/> is null.
    /// </exception>
    string GetPostBackEventReference (PostBackOptions options, bool registerForEventValidation);

    /// <summary>
    /// Gets a URL reference to a resource in an assembly.
    /// </summary>
    /// <returns>
    /// The URL reference to the resource.
    /// </returns>
    /// <param name="type">The type of the resource. 
    /// </param><param name="resourceName">The fully qualified name of the resource in the assembly. 
    /// </param><exception cref="T:System.ArgumentNullException">The web resource type is null.
    /// </exception><exception cref="T:System.ArgumentNullException">The web resource name is null.
    ///     - or -
    ///     The web resource name has a length of zero.
    /// </exception>
    string GetWebResourceUrl (Type type, string resourceName);

    /// <summary>
    /// Determines whether the client script block is registered with the <see cref="T:System.Web.UI.Page"/> object using the specified key. 
    /// </summary>
    /// <returns>
    /// true if the client script block is registered; otherwise, false.
    /// </returns>
    /// <param name="key">The key of the client script block to search for.
    /// </param>
    bool IsClientScriptBlockRegistered (string key);

    /// <summary>
    /// Determines whether the client script block is registered with the <see cref="T:System.Web.UI.Page"/> object using a key and type.
    /// </summary>
    /// <returns>
    /// true if the client script block is registered; otherwise, false.
    /// </returns>
    /// <param name="type">The type of the client script block to search for.  
    /// </param><param name="key">The key of the client script block to search for. 
    /// </param><exception cref="T:System.ArgumentNullException">The client script type is null.
    /// </exception>
    bool IsClientScriptBlockRegistered (Type type, string key);

    /// <summary>
    /// Determines whether the client script include is registered with the <see cref="T:System.Web.UI.Page"/> object using the specified key. 
    /// </summary>
    /// <returns>
    /// true if the client script include is registered; otherwise, false.
    /// </returns>
    /// <param name="key">The key of the client script include to search for. 
    /// </param>
    bool IsClientScriptIncludeRegistered (string key);

    /// <summary>
    /// Determines whether the client script include is registered with the <see cref="T:System.Web.UI.Page"/> object using a key and type.
    /// </summary>
    /// <returns>
    /// true if the client script include is registered; otherwise, false.
    /// </returns>
    /// <param name="type">The type of the client script include to search for. 
    /// </param><param name="key">The key of the client script include to search for. 
    /// </param><exception cref="T:System.ArgumentNullException">The client script include type is null.
    /// </exception>
    bool IsClientScriptIncludeRegistered (Type type, string key);

    /// <summary>
    /// Determines whether the startup script is registered with the <see cref="T:System.Web.UI.Page"/> object using the specified key.
    /// </summary>
    /// <returns>
    /// true if the startup script is registered; otherwise, false.
    /// </returns>
    /// <param name="key">The key of the startup script to search for.
    /// </param>
    bool IsStartupScriptRegistered (string key);

    /// <summary>
    /// Determines whether the startup script is registered with the <see cref="T:System.Web.UI.Page"/> object using the specified key and type.
    /// </summary>
    /// <returns>
    /// true if the startup script is registered; otherwise, false.
    /// </returns>
    /// <param name="type">The type of the startup script to search for. 
    /// </param><param name="key">The key of the startup script to search for.
    /// </param><exception cref="T:System.ArgumentNullException">The startup script type is null.
    /// </exception>
    bool IsStartupScriptRegistered (Type type, string key);

    /// <summary>
    /// Determines whether the OnSubmit statement is registered with the <see cref="T:System.Web.UI.Page"/> object using the specified key. 
    /// </summary>
    /// <returns>
    /// true if the OnSubmit statement is registered; otherwise, false.
    /// </returns>
    /// <param name="key">The key of the OnSubmit statement to search for.
    /// </param>
    bool IsOnSubmitStatementRegistered (string key);

    /// <summary>
    /// Determines whether the OnSubmit statement is registered with the <see cref="T:System.Web.UI.Page"/> object using the specified key and type.
    /// </summary>
    /// <returns>
    /// true if the OnSubmit statement is registered; otherwise, false.
    /// </returns>
    /// <param name="type">The type of the OnSubmit statement to search for. 
    /// </param><param name="key">The key of the OnSubmit statement to search for. 
    /// </param><exception cref="T:System.ArgumentNullException">The OnSubmit statement type is null.
    /// </exception>
    bool IsOnSubmitStatementRegistered (Type type, string key);

    /// <summary>
    /// Registers a JavaScript array declaration with the <see cref="T:System.Web.UI.Page"/> object using an array name and array value.
    /// </summary>
    /// <param name="arrayName">The array name to register.
    /// </param><param name="arrayValue">The array value or values to register.
    /// </param><exception cref="T:System.ArgumentNullException"><paramref name="arrayName"/> is null.
    /// </exception>
    void RegisterArrayDeclaration (string arrayName, string arrayValue);

    /// <summary>
    /// Registers a name/value pair as a custom (expando) attribute of the specified control given a control ID, attribute name, and attribute value.
    /// </summary>
    /// <param name="controlId">The <see cref="T:System.Web.UI.Control"/> on the page that contains the custom attribute. 
    /// </param><param name="attributeName">The name of the custom attribute to register. 
    /// </param><param name="attributeValue">The value of the custom attribute. 
    /// </param>
    void RegisterExpandoAttribute (string controlId, string attributeName, string attributeValue);

    /// <summary>
    /// Registers a name/value pair as a custom (expando) attribute of the specified control given a control ID, an attribute name, an attribute value, and a Boolean value indicating whether to encode the attribute value.
    /// </summary>
    /// <param name="controlId">The <see cref="T:System.Web.UI.Control"/> on the page that contains the custom attribute.
    /// </param><param name="attributeName">The name of the custom attribute to register.
    /// </param><param name="attributeValue">The value of the custom attribute.
    /// </param><param name="encode">A Boolean value indicating whether to encode the custom attribute to register.
    /// </param>
    void RegisterExpandoAttribute (string controlId, string attributeName, string attributeValue, bool encode);

    /// <summary>
    /// Registers a hidden value with the <see cref="T:System.Web.UI.Page"/> object.
    /// </summary>
    /// <param name="hiddenFieldName">The name of the hidden field to register.
    /// </param><param name="hiddenFieldInitialValue">The initial value of the field to register.
    /// </param><exception cref="T:System.ArgumentNullException"><paramref name="hiddenFieldName"/> is null.
    /// </exception>
    void RegisterHiddenField (string hiddenFieldName, string hiddenFieldInitialValue);

    /// <summary>
    /// Registers the client script with the <see cref="T:System.Web.UI.Page"/> object using a type, key, and script literal.
    /// </summary>
    /// <param name="type">The type of the client script to register. 
    /// </param><param name="key">The key of the client script to register. 
    /// </param><param name="script">The client script literal to register. 
    /// </param>
    void RegisterClientScriptBlock (Type type, string key, string script);

    /// <summary>
    /// Registers the client script with the <see cref="T:System.Web.UI.Page"/> object using a type, key, script literal, and Boolean value indicating whether to add script tags.
    /// </summary>
    /// <param name="type">The type of the client script to register. 
    /// </param><param name="key">The key of the client script to register. 
    /// </param><param name="script">The client script literal to register.  
    /// </param><param name="addScriptTags">A Boolean value indicating whether to add script tags.
    /// </param><exception cref="T:System.ArgumentNullException">The client script block type is null.
    /// </exception>
    void RegisterClientScriptBlock (Type type, string key, string script, bool addScriptTags);

    /// <summary>
    /// Registers the client script with the <see cref="T:System.Web.UI.Page"/> object using a key and a URL.
    /// </summary>
    /// <param name="key">The key of the client script include to register. 
    /// </param><param name="url">The URL of the client script include to register. 
    /// </param>
    void RegisterClientScriptInclude (string key, string url);

    /// <summary>
    /// Registers the client script include with the <see cref="T:System.Web.UI.Page"/> object using a type, a key, and a URL.
    /// </summary>
    /// <param name="type">The type of the client script include to register. 
    /// </param><param name="key">The key of the client script include to register. 
    /// </param><param name="url">The URL of the client script include to register. 
    /// </param><exception cref="T:System.ArgumentNullException">The client script include type is null.
    /// </exception><exception cref="T:System.ArgumentException">The URL is null. 
    ///     - or -
    ///     The URL is empty.
    /// </exception>
    void RegisterClientScriptInclude (Type type, string key, string url);

    /// <summary>
    /// Registers the client script resource with the <see cref="T:System.Web.UI.Page"/> object using a type and a resource name.
    /// </summary>
    /// <param name="type">The type of the client script resource to register. 
    /// </param><param name="resourceName">The name of the client script resource to register. 
    /// </param><exception cref="T:System.ArgumentNullException">The client resource type is null.
    /// </exception><exception cref="T:System.ArgumentNullException">The client resource name is null.
    ///     - or -
    ///     The client resource name has a length of zero.
    /// </exception>
    void RegisterClientScriptResource (Type type, string resourceName);

    /// <summary>
    /// Registers an OnSubmit statement with the <see cref="T:System.Web.UI.Page"/> object using a type, a key, and a script literal. The statement executes when the <see cref="T:System.Web.UI.HtmlControls.HtmlForm"/> is submitted.
    /// </summary>
    /// <param name="type">The type of the OnSubmit statement to register. 
    /// </param><param name="key">The key of the OnSubmit statement to register. 
    /// </param><param name="script">The script literal of the OnSubmit statement to register. 
    /// </param><exception cref="T:System.ArgumentNullException"><paramref name="type"/> is null.
    /// </exception>
    void RegisterOnSubmitStatement (Type type, string key, string script);

    /// <summary>
    /// Registers the startup script with the <see cref="T:System.Web.UI.Page"/> object using a type, a key, and a script literal.
    /// </summary>
    /// <param name="type">The type of the startup script to register. 
    /// </param><param name="key">The key of the startup script to register. 
    /// </param><param name="script">The startup script literal to register. 
    /// </param>
    void RegisterStartupScript (Type type, string key, string script);

    /// <summary>
    /// Registers the startup script with the <see cref="T:System.Web.UI.Page"/> object using a type, a key, a script literal, and a Boolean value indicating whether to add script tags.
    /// </summary>
    /// <param name="type">The type of the startup script to register. 
    /// </param><param name="key">The key of the startup script to register. 
    /// </param><param name="script">The startup script literal to register. 
    /// </param><param name="addScriptTags">A Boolean value indicating whether to add script tags. 
    /// </param><exception cref="T:System.ArgumentNullException"><paramref name="type"/> is null.
    /// </exception>
    void RegisterStartupScript (Type type, string key, string script, bool addScriptTags);
  }
}