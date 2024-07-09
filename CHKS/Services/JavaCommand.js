function ScrollToTop() {
    try {
        console.log('ScrollToTop() rz-data-grid-data')
        var elem = document.getElementsByClassName("rz-data-grid-data")[0];
        elem.scrollTop = 0;
    }
    catch (ex) {
        console.log(ex);
    }
}