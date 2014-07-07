$(function() {
    $('#searchForm').validate({
        rules: {
            searchText: {
                minlength: 3,
                required: true
            }
        }
    });
});


