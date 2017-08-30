// Write your JavaScript code.
var baseUrl
if (window.location.hostname.indexOf('localhost') === -1) {
    baseUrl = '';
} else {
    baseUrl = window.location.hostname;
}
$().ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});