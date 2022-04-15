mergeInto(LibraryManager.library, {
    Export: function (base64, fileName) {
        // Reference: https://stackoverflow.com/questions/34339593/open-base64-in-new-tab
        const url = 'data:application/octet-stream;base64,' + Pointer_stringify(base64);

        const dl = document.createElement('a');
        dl.href = url;
        dl.download = Pointer_stringify(fileName);
        dl.click();
        dl.remove();
    },

    ImportEnabled: function (enabled) {
        var e = document.getElementById('import');
        if (enabled) {
            e.classList.add("enabled");
        }
        else {
            e.classList.remove("enabled");
        }
    }
});
