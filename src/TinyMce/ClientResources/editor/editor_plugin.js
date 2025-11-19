"use strict";
var tinymce = tinymce || {};

var currentUrl = location.href;

tinymce.PluginManager.add("imageshopoptimizelyplugin", function (ed, url) {
    var imageNode;

    console.log("Tinymce version: " + tinymce.majorVersion + "." + tinymce.minorVersion);

    var hostUrl = location.protocol + '//' + location.host;

    if (hostUrl.includes('undefined')) {
        var urlParts = ed.documentBaseUrl.match(/^(\w+:)\/\/([^:/?]+)(?::(\d+))?/);
        if (urlParts) {
            var protocol = urlParts[1];
            var hostname = urlParts[2];
            var port = urlParts[3] ? urlParts[3] : "";

            hostUrl = protocol + '//' + hostname + (port ? ':' + port : '');
        }
    }

    function generateRandomFiveDigitNumber() {
        return Math.floor(10000 + Math.random() * 90000);
    }

    function getLocalizedButtonTitles() {
        var localizedTitles = {
            en: {
                buttonTitle: "Insert/Upload Imageshop Image",
                buttonTitleVideo: "Insert/Upload Imageshop Video"
            },
            nb: {
                buttonTitle: "Sett inn/Last opp Imageshop-bilde",
                buttonTitleVideo: "Sett inn/Last opp Imageshop-video"
            },
            da: {
                buttonTitle: "Indsæt/Upload Imageshop Billede",
                buttonTitleVideo: "Indsæt/Upload Imageshop Video"
            },
            sv: {
                buttonTitle: "Infoga/Ladda upp Imageshop Bild",
                buttonTitleVideo: "Infoga/Ladda upp Imageshop Video"
            },
        };

        var currentLocale = dojo.locale.split('-')[0]; // Get the language part
        return localizedTitles[currentLocale] || localizedTitles['en']; // Default to English if locale not found
    }

    // Usage
    var { buttonTitle, buttonTitleVideo } = getLocalizedButtonTitles();
    var buttonText = "";
    var buttonIcon = url + "/images/icon.png";
    var controllerPath = "/imageshoptinymce/insertimage/";
    //var pluginpath = dojoConfig.paths["imageshop-optimizely-plugin"];
    var dialogUrl = hostUrl + controllerPath + "?TINYMCE=true&TINYMCEEXTENDED=true&CULTURE=" + dojo.locale + "&IMAGESHOPLANGUAGE=" + epi.dependency.resolve("epi.shell.ContextService").currentContext.language + "&rnd=" + generateRandomFiveDigitNumber();//or epi.dependency.resolve("epi.cms.contentEditing.command.Editing").model.languageContext.language; 

    if (tinymce.majorVersion >= 5) {
        // Register icon for Imageshop
        ed.ui.registry.addIcon('imageshopIcon', '<svg width="24" height="24" viewBox="0 0 24 24"><defs><style>.cls-1{fill:#6eb1ff;}.cls-2{fill:#175aff;}.cls-3{display:none;}</style></defs><g><g id="Layer_1" class="cls-3" focusable="false"><path d="M5,15.6999998l3.3000002-3.1999998c.3000002-.3000002.6999998-.3000002,1,0l2.6999998,2.5,4.1000004-4c.2999992-.3999996.7999992-.3999996,1,0l2,1.8999996v-7.8999996H5v10.6999998ZM5,18v1h3l2.8000002-2.8999996-2-2-3.8000002,3.7999992v.1000004ZM19,15l-2.5-2.3999996-6.3999996,6.5h8.8999996v-4.1000004ZM4,3h16c.6000004,0,1,.4000001,1,1v16c0,.6000004-.3999996,1-1,1H4c-.5522847,0-1-.4477158-1-1V4c0-.5999999.4000001-1,1-1ZM10,11c1.1045694,0,2-.8954306,2-2s-.8954306-2-2-2-2,.8954306-2,2,.8954306,2,2,2Z"/></g><g id="Layer_2"><polygon class="cls-1" points="4.8170081 18.0914969 4.8170081 19.1545977 8.0063105 19.1545977 10.9829797 16.0715795 8.8567781 13.9453779 4.8170081 17.9852128 4.8170081 18.0914969"/><g><circle class="cls-2" cx="10" cy="9" r="2"/><path class="cls-2" d="M20,3H4c-.5999756,0-1,.4000244-1,1v16c0,.5523071.4476929,1,1,1h16c.5999756,0,1-.4000244,1-1V4c0-.5999756-.4000244-1-1-1ZM5,5h14.0999756v7.9000244l-2-1.9000244c-.1999512-.4000244-.6999512-.4000244-1,0l-4.0999756,4-2.7000122-2.5c-.2999878-.2999878-.7000122-.2999878-1,0l-3.2999878,3.2000122V5ZM5,18v-.0999756l3.7999878-3.8000488,2,2-2.7999878,2.9000244h-3v-1Z"/></g></g></g></svg>');
        ed.ui.registry.addIcon('imageshopIconVideo', '<svg width="24" height="24" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">  <defs>    <style>      .cls-1 { fill: #6eb1ff; }      .cls-2 { fill: #175aff; }    </style>  </defs>  <rect x="3" y="3" width="18" height="18" class="cls-1" />  <polygon points="10,8 16,12 10,16" class="cls-2" /></svg>');

        // Register buttons
        ed.ui.registry.addButton('imageshopoptimizelyplugin', {
            title: buttonTitle,
            tooltip: buttonTitle,
            icon: 'imageshopIcon',
            onAction: function () {
                try {
                    var currentDialogUrl = dialogUrl;

                    if (ed.selection !== null && ed.selection.getNode() && ed.selection.getNode().src)
                        currentDialogUrl += "&image=" + encodeURIComponent(ed.selection.getNode().src);

                    tinymce.activeEditor.windowManager.openUrl({
                        title: 'ImageShop',
                        url: currentDialogUrl,
                        buttons: [{
                            type: 'cancel',
                            name: 'close',
                            text: 'Close'
                        }]
                    });

                } catch (error) {
                    console.log("Error setting up button action: " + error);
                }
            },
            onSetup: function (api) {
                // Node change handler to toggle button active state
                const nodeChangeHandler = function(e) {
                    var isWebShopImage = e.element.tagName === "IMG" && (ed.dom.getAttrib(e.element, "class").indexOf("ScrImageshopImage") > -1 || ed.dom.getAttrib(e.element, "class").indexOf("mceItem") === -1);

                    //Set or reset imageNode to the node cause Image Editor command enabled
                    imageNode = isWebShopImage ? e.element : null;
                    api.setEnabled(true);
                }; 

                // Register node change handler
                ed.on('NodeChange', nodeChangeHandler);

                // Return function to unregister node change handler
                return function() {
                    ed.off('NodeChange', nodeChangeHandler);
                };
            }
        });

        ed.ui.registry.addButton('imageshopvideo', {
            title: buttonTitleVideo,
            tooltip: buttonTitleVideo,
            icon: 'imageshopIconVideo',
            onAction: function () {
                try {
                    var currentDialogUrl = dialogUrl + "&SHOWVIDEO=true";

                    if (ed.selection !== null && ed.selection.getNode() && ed.selection.getNode().src)
                        currentDialogUrl += "&video=" + encodeURIComponent(ed.selection.getNode().src);

                    tinymce.activeEditor.windowManager.openUrl({
                        title: 'ImageShop',
                        url: currentDialogUrl,
                        buttons: [{
                            type: 'cancel',
                            name: 'close',
                            text: 'Close'
                        }]
                    });

                } catch (error) {
                    console.log("Error setting up button action imageshopvideo: " + error);
                }
            },
            onSetup: function (api) {
                // Node change handler to toggle button active state
                const nodeChangeHandler = function (e) {
                    var isWebShopImage = e.element.tagName === "IMG" && (ed.dom.getAttrib(e.element, "class").indexOf("ScrImageshopImage") > -1 || ed.dom.getAttrib(e.element, "class").indexOf("mceItem") === -1);

                    //Set or reset imageNode to the node cause Image Editor command enabled
                    imageNode = isWebShopImage ? e.element : null;
                    api.setEnabled(true);
                };

                // Register node change handler
                ed.on('NodeChange', nodeChangeHandler);

                // Return function to unregister node change handler
                return function () {
                    ed.off('NodeChange', nodeChangeHandler);
                };
            }
        });
    }
    else {
        console.log("running tinymce setup for versions older than version 5");

        // Register commands
        ed.addCommand('mceimageshopoptimizelyplugin', function () {

            try {
                var currentDialogUrl = dialogUrl;

                if (ed.selection !== null && ed.selection.getNode() !== null && ed.selection.getNode().src !== null)
                    dialogUrl += "&image=" + encodeURIComponent(ed.selection.getNode().src);

                ed.windowManager.open({
                    file: currentDialogUrl,
                    width: 950,
                    height: 650,
                    scrollbars: 1,
                    inline: 1
                }, {
                    plugin_url: url
                });
            } catch (error) {
                console.log("Error running command mceimageshopoptimizelyplugin: " + error);
            }
        });

        // Register buttons
        ed.addButton("imageshopoptimizelyplugin", {
            title: buttonTitle,
            cmd: "mceimageshopoptimizelyplugin",
            image: buttonIcon,
            onPostRender: function () {
                // Add a node change handler, selects the button in the UI when a image is selected
                ed.on("NodeChange", function (e) {
                    //Prevent tool from being activated as objects represented as images.
                    var isWebShopImage = e.element.tagName === "IMG" && (ed.dom.getAttrib(e.element, "class").indexOf("ScrImageshopImage") > -1 || ed.dom.getAttrib(e.element, "class").indexOf("mceItem") === -1);
                    this.active(isWebShopImage);

                    //Set or reset imageNode to the node cause Image Editor command enabled
                    imageNode = isWebShopImage ? e.element : null;
                }.bind(this));
            }
        });
    }

    return {
        getMetadata: function () {
            return {
                name: "Imageshop image plugin",
                url: "https://github.com/imageshoporg/Optimizely",
                author: 'Geta AS, Bouvet ASA, Epinova AB, Imageshop, tinymce ' + tinymce.majorVersion + '.' + tinymce.minorVersion,
                authorurl: 'https://github.com/imageshoporg/Optimizely',
                infourl: 'https://github.com/imageshoporg/Optimizely',
                version: tinymce.majorVersion + "." + tinymce.minorVersion
            };
        }
    };
});

