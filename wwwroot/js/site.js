// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.


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
})
const bodyContent = document.querySelector('#BodyContent');
const saveBtn = document.querySelector('#cdx-save');
saveBtn.addEventListener('click', () => {
    editor.save().then((outputData) => {
        console.log('Article data: ', outputData)
        bodyContent.value = outputData;
        console.log('body: ', bodyContent.value)

    }).catch((error) => {
        console.log('Saving failed: ', error)
    });
})