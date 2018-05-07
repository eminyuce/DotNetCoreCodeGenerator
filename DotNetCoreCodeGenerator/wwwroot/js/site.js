function ajaxMethodCall(postData, ajaxUrl, successFunction) {

    $.ajax({
        type: "POST",
       // contentType: "application/x-www-form-urlencoded; charset=UTF-8", //this could be left out as it is the default content-type of an AJAX request
        url: ajaxUrl,
        data: postData,
        success: successFunction,
        error: function (jqXHR, exception) {
            if (jqXHR.status === 0) {
                console.error('Not connect.\n Verify Network.');
            } else if (jqXHR.status === 404) {
                console.error('Requested page not found. [404]');
            } else if (jqXHR.status ===500) {
                console.error('Internal Server Error [500].');
            } else if (exception === 'parsererror') {
                console.error('Requested JSON parse failed.');
            } else if (exception === 'timeout') {
                console.error('Time out error.');
            } else if (exception === 'abort') {
                console.error('Ajax request aborted.');
            } else {
                console.error('Uncaught Error.\n' + jqXHR.responseText);
            }
        },
        contentType: 'application/json; charset=UTF-8',
        dataType: "json"
    });
}
function isEmpty(str) {
    return (!str || 0 === str.length);
}
$(document).ready(function () {


    var $TextBox_Filter = $('#TextBox_Filter');
    var $ddl = $('#SelectedTable');
    var $items = $('select[id$=SelectedTable] option');

    $("#TextBox_Filter")
        .blur(filter)
        .keyup(filter)
        .change(filter);


    function filter() {
        var keyword = $TextBox_Filter.val();
        if (isEmpty(keyword)) {
            $('#SelectedTable').find('option').remove();
            $('#SelectedTable').append($('#TableCopied').val());
            return;
        }


        var select = $ddl[0];
        for (var i = 0; i < select.length; i++) {
            var txt = select.options[i].text;
            if (txt.toLowerCase().indexOf(keyword.toLowerCase()) === -1) {
                $(select.options[i]).attr('disabled', 'disabled').hide();
            } else {
                $(select.options[i]).removeAttr('disabled').show();
            }
        }
    }




    $("#RetrieveTables").click(function () {
        fillTables();
    });

    fillTables();
    function fillTables() {
        if (!isEmpty($("#ConnectionString").val()) || !isEmpty($("#MySqlConnectionString").val())) {
            var postData = JSON.stringify({ "connectionString": $("#ConnectionString").val(), "MySqlConnectionString": $("#MySqlConnectionString").val() });
            ajaxMethodCall(postData, "/Ajax/GetTables", function (data) {
                var listitems = '';
                var SelectedTableValue = $("#SelectedTableValue").val();
                $("#SelectedTable").empty();
                data.forEach(function (element) {
                    console.log(element.TableNameWithSchema);

                    $("#SelectedTable").append("<option value=" + element.DatabaseTableName+">" + element.TableNameWithSchema + "</option>");

                });
                $("#SelectedTable option").each(function () {
                    console.log('OPTION:'+$(this).text());
                });

            });
        }
    }
    $('#SelectedTable').on('change', function () {
        $('#ModifiedTableName').val(this.value);
    });

});