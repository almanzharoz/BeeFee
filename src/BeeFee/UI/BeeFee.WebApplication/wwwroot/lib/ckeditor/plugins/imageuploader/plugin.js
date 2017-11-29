CKEDITOR.plugins.add('imageuploader',
    {
        init: function(editor) {
            editor.addCommand('imageuploader',
                new CKEDITOR.dialogCommand('imageuploader',
                    {
                    }));
            editor.ui.addButton('imageuploader',
                {
                    command: 'imageuploder',
                    label: 'Загрузить изображение',
                    toolbar: 'tools'
                });
            CKEDITOR.dialog.add('imageuploader', add_dialog);

            //editor.on('');

            var add_dialog = function(editor) {
                var dialog = {
                    title: 'Загрузить изображение',
                    width: 300,
                    height: 100,
                    contents: [
                        {
                            id: 'general',
                            label: 'upload',
                            elements:
                            [
                                {
                                    type: 'button',
                                    id: 'upload_btn',
                                    label: 'Загрузить'
                                }
                            ]
                        }
                    ],

                    onOk: function() {

                    }
                }
                return dialog;
            };
        }
    });