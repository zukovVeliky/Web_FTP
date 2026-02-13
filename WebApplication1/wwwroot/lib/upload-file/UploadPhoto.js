    var holder = document.getElementById('holder'),
        tests = {
            filereader: typeof FileReader != 'undefined',
            dnd: 'draggable' in document.createElement('span'),
            formdata: !!window.FormData,
            progress: "upload" in new XMLHttpRequest
        },
        support = {
            filereader: document.getElementById('filereader'),
            formdata: document.getElementById('formdata'),
            progress: document.getElementById('progress')
        },
        acceptedTypes = {
            'image/png': true,
            'image/jpeg': true,
            'image/gif': true
        },
        progress = document.getElementById('uploadprogress'),
        fileupload = document.getElementById('upload');

    "filereader formdata progress".split(' ').forEach(function (api) {
        if (tests[api] === false) {
            support[api].className = 'fail';
        } else {
            // FFS. I could have done el.hidden = true, but IE doesn't support
            // hidden, so I tried to create a polyfill that would extend the
            // Element.prototype, but then IE10 doesn't even give me access
            // to the Element object. Brilliant.
            support[api].className = 'hidden';
        }
    });



    if (tests.dnd) {
        holder.ondragover = function () { this.className = 'hover'; return false; };
        holder.ondragend = function () { this.className = ''; return false; };
        holder.ondrop = function (e) {
            this.className = '';
            e.preventDefault();
            readfiles(e.dataTransfer.files);
        }
    } else {
        fileupload.className = 'hidden';
        fileupload.querySelector('input').onchange = function () {
            readfiles(this.files);
        };
    }


function previewfile(file) {
    if (tests.filereader === true && acceptedTypes[file.type] === true) {
        var reader = new FileReader();
        reader.onload = function (event) {
            var image = new Image();
            image.src = event.target.result;
            image.width = 250; // a fake resize
            holder.appendChild(image);
        };

        reader.readAsDataURL(file);
    } else {
        holder.innerHTML += '<p>Uploaded ' + file.name + ' ' + (file.size ? (file.size / 1024 | 0) + 'K' : '');
        console.log(file);
    }
}

function readfiles(files) {

    var formData = tests.formdata ? new FormData() : null;
    for (var i = 0; i < files.length; i++) {
        
        if (tests.formdata) formData.append('file', files[i]);
        previewfile(files[i]);
    }
debugger;
    // now post a new XHR request
    if (tests.formdata) {debugger;
        var xhr = new XMLHttpRequest();
        //'Komponenty/Galerie/DragAndDropFiles/Upload.ashx?Autor=' + Autor + '&Datum=' + Datum + '&Title=' + Title + '&Clanek=' + Clanek);
        xhr.open('POST', UploadURL);
        xhr.onload = function () {
            progress.value = progress.innerHTML = 100;
        };

        if (tests.progress) {debugger;
            xhr.upload.onprogress = function (event) {
                if (event.lengthComputable) {debugger;
                    var complete = (event.loaded / event.total * 100 | 0);
                    progress.value = progress.innerHTML = complete;
                }
            }
        }

        xhr.send(formData);
    }
}