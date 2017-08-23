function LiveFilter(phrase, data) {

    var filteredData = [];
    for (var i = 0; i < data.length; i++) {
        if (data[i].indexOf(phrase) > -1) {
            filteredData.push(data[i]);
        }
    }
    return filteredData;
}