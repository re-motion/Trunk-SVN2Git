function BocAutoCompleteReferenceValue()
{
}

BocAutoCompleteReferenceValue.Bind = function(textbox, hiddenField, button, webServiceUrl) 
{
    $(textbox).autocomplete(webServiceUrl,
        {
            extraParams: { 'example_parameter': 'value_of_example_parameter' }, //RB: add extra parameters for AJAX call if you need
            minChars: 0,
            max: 30, // Set query limit
            mustMatch: false, //set true if should clear input on no results
            matchContains: true,
            scrollHeight: 220,
            width: 200, //Define width of results
            extraBind: button.id,
            dataType: 'json',
            parse: function(data) {
                return $.map(data, function(row) {
                    return {
                        data: row,
                        value: row.displayName,
                        result: row.displayName
                    }
                });
            },
            formatItem: function(item) {
                return item.displayName + ' (' + item.UniqueIdentifier + ')'; //What we display on input box
            }
        }
    ).result(function(e, item) 
    {
        $(hiddenField).val(item.UniqueIdentifier); //What we polulate on hidden box
    });
};
