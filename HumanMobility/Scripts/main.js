$(document).ready(function () {
    $("#fromDatePicker").datetimepicker();

    $("#toDatePicker").datetimepicker();

    $("#afromDatePicker").datetimepicker();

    $("#atoDatePicker").datetimepicker();

    $("#summaryDatePicker").datetimepicker();

    $("#bugReportEditor").on('input', updateCount);

    function updateCount() {
        if ($(this).val().length >= 300) {
            $(this).val($(this).val().substring(0, 299));
        }
        $('#characters').text(299 - $(this).val().length);
    }
});
