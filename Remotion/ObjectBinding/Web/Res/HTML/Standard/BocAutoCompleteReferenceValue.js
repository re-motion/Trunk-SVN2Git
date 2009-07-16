function BocAutoCompleteReferenceValue()
{
}

BocAutoCompleteReferenceValue.Bind = function(textbox, hiddenField, button, webServiceUrl, boClass, boProperty, boID, args) {
    textbox.autocomplete(webServiceUrl,
        {
            extraParams: { 'businessObjectClass': boClass, 'businessObjectProperty': boProperty, 'businessObjectID': boID, 'args': args },
            minChars: 0,
            max: 30, // Set query limit
            mustMatch: false, //set true if should clear input on no results
            matchContains: true,
            scrollHeight: 220,
            width: 200, //Define width of results
            extraBind: button.attr('id'),
            dataType: 'json',
            parse: function(data) {
                return $.map(data.d, function(row) {
                    return {
                        data: row,
                        value: row.UniqueIdentifier,
                        result: row.DisplayName
                    }
                });
            },
            formatItem: function(item) {
                return item.DisplayName; //What we display on input box
            }
        }
    ).result(function(e, item) {
        var dummy = "";
        hiddenField.val(item.UniqueIdentifier); //What we polulate on hidden box
        textbox.trigger('change');
    });
};
