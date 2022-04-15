# schwer-scripts: WebGL save handler
[![Root](https://img.shields.io/badge/Root-schwer--scripts-0366D6.svg)](/../../) [![Donate](https://img.shields.io/badge/Donate-PayPal-brightgreen.svg)](https://www.paypal.com/donate?hosted_button_id=NYFKAS24D4MJS)

<!-- one-line description -->

<!-- .jslib folder and import settings -->

## Contents
* [`WebGLSaveHelper.jslib`](#WebGLSaveHelper.jslib)

# `WebGLSaveHelper.jslib`

## Required WebGL Template `html`
```html
<head>
    <script>
        // instantiate unity player, etc

        function inputPrompt(button) {
            if (button.classList.contains("enabled")) {
                document.getElementById('input').click()
            }
        }

        function readFile(files) {
            if (files.length === 0) { return; }

            var file = files[0];
            var fileReader = new FileReader();
            fileReader.readAsArrayBuffer(file);
            fileReader.onload = function () {
                // Reference: https://stackoverflow.com/questions/17845032/net-mvc-deserialize-byte-array-from-json-uint8array
                var str = String.fromCharCode.apply(null, new Uint8Array(fileReader.result));
                unityInstance.SendMessage("%UNITY_CUSTOM_IMPORT_DATA_GAME_OBJECT_NAME%", "%UNITY_CUSTOM_IMPORT_DATA_FUNCTION_NAME%", window.btoa(str));

                document.querySelector('form').reset();
            };
        }
    </script>
    <style>
        /* unityContainer styling, etc */

        body { margin: 0; }

        div#footer {
            font-family: sans-serif;
            text-align: center;
            display: flex;
            width: %UNITY_WIDTH%px;
            height: %UNITY_CUSTOM_FOOTER_HEIGHT%px;
        }

        div#footer a {
            flex: 1;
            text-decoration: none;
            background-color: #2F3136;
            color: #363636;
        }

        div#footer a.enabled {
            background-color: #2F3136;
            color: #FFFFFF;
        }

        div#footer a.enabled:hover {
            cursor: pointer;
            background-color: #36393F;
        }

        div#footer a.enabled:active {
            background-color: #FFFFFF;
            color: #36393F;
        }
    </style>
</head>

<body>
    <!-- unityContainer, etc -->
    <div id="footer">
        <a onclick="unityInstance.SetFullscreen(1)" class="enabled">Fullscreen</a>
        <a id="download">Download save file (--:--)</a>
        <a id="import" onclick="inputPrompt(this)">Import local save file</a>
        <form>
            <input id="input" type="file" accept=".%UNITY_CUSTOM_SAVE_FILE_EXTENSION%" oninput="readFile(this.files)" style="display:none;">
        </form>
    </div>
</body>
```
