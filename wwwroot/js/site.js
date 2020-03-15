// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.
const pageName = document.querySelector('[data-page]').dataset.page;

// Show the Editor
if (pageName == 'project-create' || pageName == 'project-edit') {
    const editor = new EditorJS({
        /** 
         * Id of Element that should contain the Editor 
         */
        holderId: 'codex-editor',
        tools: {
            header: {
                class: Header,
                inlineToolbar: ['link']
            },
            list: {
                class: List,
                inlineToolbar: true
            }
        },
        onReady: function () {
            if (bodyContent.value !== "") {
                const data = JSON.parse(bodyContent.value);
                editor.render(data);
            }

        }
    })
    const bodyContent = document.querySelector('#BodyContent');
    const codexEditor = document.querySelector('#codex-editor')
    codexEditor.addEventListener('focusout', () => {
        editor.save().then((outputData) => {
            console.log('Article data: ', outputData)
            // convert outputData to string and inject to bodyContent.value
            bodyContent.value = JSON.stringify(outputData);
            console.log('body: ', bodyContent.value)

        }).catch((error) => {
            console.log('Saving failed: ', error)
        });
    })
}

// Output json to HTML
if (pageName == 'project-details') {
    console.log(pageName)
    const bodyContent = document.querySelector('#bodyContent-input').textContent;
    const outputContent = document.querySelector('#bodyContent-output')
    const bodyJson = JSON.parse(bodyContent).blocks;
    console.log(bodyJson)
    toHtml(bodyJson, outputContent);

}
if (pageName == 'project-index') {
    console.log(pageName)
    const bodyContents = [...document.querySelectorAll('.bodyContent-inputs')];
    const outputContents = [...document.querySelectorAll('.bodyContent-outputs')]
    const bodyJsons = bodyContents.map((bodyContent)=> JSON.parse(bodyContent.textContent).blocks)
    console.log(bodyJsons)
    for (let i = 0; i < outputContents.length; i++) {
        toHtml(bodyJsons[i], outputContents[i]);
    }
}

// convert input json to output's innerHtml
function toHtml(input, output) {
    const bodyHtml = input.map((e) => {
        if (e.type == 'paragraph') {
            return `<p>${e.data.text}</p>`
        } else if (e.type == 'header') {
            return `<h${e.data.level}>${e.data.text}</h${e.data.level}>`

        } else if (e.type == 'list' && e.data.style == 'ordered') {
            return `
                    <ol>
                    ${e.data.items.map(i => ` <li>${i}</li>`).join("")} 
                    </ol>`
        } else if (e.type == 'list' && e.data.style == 'unordered') {
            return `
                    <ul>
                    ${e.data.items.map(i => ` <li>${i}</li>`).join("")} 
                    </ul>`
        }
        else {
            return `<div>${e.data.text}</div>`
        }
    }).join("")
    output.innerHTML = bodyHtml
}