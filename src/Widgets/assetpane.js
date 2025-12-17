define([
    "dojo/_base/declare",
    "dojo/topic",
    "dojo/when",
    "dojo/on",
    "epi/shell/request/xhr",
    "dojo/_base/lang",
    "epi/shell/command/_Command",
    "epi/dependency",
    "imageshop-optimizely-plugin/widgets/_ImageSelector",
    "xstyle/css!./widgets/templates/ImageSelector.css"
], function (
    declare,
    topic,
    when,
    on,
    xhr,
    lang,
    _Command,
    dependency,
    _ImageSelector
) {
    return declare([_Command], {
        baseUrl: null,
        browserClass: "imageshop-browser",
        closeBtnId: "imageshop-frame-container-close",
        containerId: "imageshop-frame-container",
        enableDocuments: false,
        cropName: "",
        currentImage: null,
        innerClass: "imageshop-frame-wrapper",
        iframeId: "imageshop-frame",
        messageReceivedCallback: null,
        selectCurrentImage: true,
        store: null,
        //      List of file extensions that may be dropped onto the editor
        allowedExtensions: [],
        filesNotOpenInExportApi: null,
        importOriginalSelection: null,
        folder: null,
        iconClass: "epi-iconDownload",
        _onModelChange: function () {

            if (this.model === null || this.model.length === 0) {
                this.set("canExecute", false);
                this.set("isAvailable", false);
                return;
            }

            this.firstModel = this.model[0];

            if (!this._validateModels(this.model)) {
                this.set("isAvailable", false);
                return;
            }


            this.set("canExecute", true);
            this.set("isAvailable", true);
        },
        _isFolder(model) {
            return model.typeIdentifier === "episerver.core.contentfolder" ||
                model.typeIdentifier === "episerver.core.contentassetfolder";
        },

        _validateModels: function (models) {
            if (!(models instanceof Array)) {
                models = [models];
            }

            if (models.length > 1)
                return false;

            //const commonImageExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.svg', '.webp', '.tiff', '.ico'];
            var hasUnsupportedTypes = models.some(function (model) {

                //    if (model.publicUrl && model.publicUrl.length > 0) {
                //        model.publicUrl = model.publicUrl.toLowerCase();
                //        const urlEndsWithCommonExtension = commonImageExtensions.some(extension => model.publicUrl.endsWith(extension));
                //        if (urlEndsWithCommonExtension)
                //            return false; // hasUnsupportedTypes
                //    }

                return model.typeIdentifier !== "episerver.core.contentfolder" &&
                    model.typeIdentifier !== "episerver.core.contentassetfolder";
            });
            return !hasUnsupportedTypes;
        },
        _execute: function () {
            var ids = "";
            var name = "";

            if (this.model instanceof Array) {
                ids = this.model.map(function (model) {
                    return model.contentLink;
                }).join(",");
                name = "media";
            }

            this.folder = this.firstModel.contentLink;

            if (!this._isFolder(this.firstModel))//for images, not in use
                this.folder = this.firstModel.parentLink;

            //console.log(this.model)

            if (!this.model) {
                return;
            }

            this.openWindow();
        },
        createFrame: function () {
            try {
                var root = document.body;
                var div = document.createElement("div");
                div.setAttribute("id", this.containerId);
                div.setAttribute("class", this.containerId);

                var innerContainer = document.createElement("div");
                innerContainer.setAttribute("class", this.innerClass + " " + this.browserClass);

                var ifrm = document.createElement("iframe");
                var ifrmUrl = this.baseUrl;

                ifrmUrl += "&CULTURE=" + dojo.locale + "&IMAGESHOPLANGUAGE=" + epi.dependency.resolve("epi.shell.ContextService").currentContext.language + "&SHOWDOCUMENT=" + this.enableDocuments;

                if (this.selectCurrentImage && this.currentImage != null && this.currentImage.url) {
                    ifrmUrl += "&IMAGE=" + encodeURI(this.currentImage.url);
                }

                ifrm.setAttribute("src", ifrmUrl);
                ifrm.setAttribute("id", this.iframeId);
                ifrm.setAttribute("frameborder", "0");

                var closeBtn = document.createElement("a");
                closeBtn.setAttribute("class", this.closeBtnId);
                closeBtn.setAttribute("id", this.closeBtnId);
                closeBtn.setAttribute("title", "Close window");

                closeBtn.onclick = lang.hitch(this, function () {
                    this.closeWindow();
                });

                innerContainer.appendChild(ifrm);
                innerContainer.appendChild(closeBtn);

                div.appendChild(innerContainer);
                root.appendChild(div);
            } catch (error) {
                //console.log("Error _imageSelector createFrame: " + error);
            }
        },
        onImageSelected: function (image) {
        },

        onMessageReceived: function (event) {
            try {
                // Check if event.data is a string
                if (typeof event.data !== 'string') {
                    return;
                }

                var firstPart = event.data.split(";")[0];

                this.store = dependency.resolve('epi.storeregistry').get('imageshopstore');
                if (this.enableDocuments) {

                    xhr.get(this.store.target + 'importdocument?parent=' + this.folder + '&url=' + encodeURIComponent(firstPart) + '&adminUrl=' + encodeURIComponent(document.location.href), {
                        handleAs: 'json'
                    }).then(lang.hitch(this, function (data) {
                            topic.publish("/epi/cms/upload", data);
                        }), lang.hitch(this, function (err) {
                            alert("Couldn't import: " + err);
                            console.error("Couldn't import: " + err);
                        }));


                    this.closeWindow();
                    return;
                }


                if (firstPart.startsWith("[") == false) {

                    firstPart = "[" + firstPart + "]";
                }

                var imageData = JSON.parse(firstPart);


                xhr.post(this.store.target + 'import?parent=' + this.folder + '&adminUrl=' + encodeURIComponent(document.location.href), {
                    handleAs: 'json',
                    preventCache: true,
                    postData: JSON.stringify(imageData),
                    headers: { "Content-Type": "application/json" }
                }).then(lang.hitch(this, function (data) {
                    topic.publish("/epi/cms/upload", data);
                }), lang.hitch(this, function (err) {
                    alert("Couldn't import: " + err);
                    console.error("Couldn't import: " + err);
                }));


                this.closeWindow();

            } catch (error) {
                console.error("Error AssetPane ImageSelector onMessageReceived: " + error);
            }
        },              

        openWindow: function (evt) {
            try {
                this.isShowingChildDialog = true;

                this.messageReceivedCallback = lang.hitch(this, this.onMessageReceived);

                if (window.addEventListener) {
                    window.addEventListener("message", this.messageReceivedCallback, false);
                } else {
                    window.attachEvent("onmessage", this.messageReceivedCallback);
                }

                this.createFrame();
            } catch (error) {
                //console.log("Error _imageSelector openWindow: " + error);
            }
        },
        closeWindow: function (evt) {
            try {
                if (window.removeEventListener) {
                    removeEventListener('message', this.messageReceivedCallback, false);
                } else {
                    detachEvent("onmessage", this.messageReceivedCallback);
                }

                this.messageReceivedCallback = null;
                this.destroyFrame();
                this.isShowingChildDialog = false;
            } catch (error) {
                //console.log("Error _imageSelector closeWindow: " + error);
            }
        },
        destroyFrame: function () {
            try {
                var container = document.getElementById(this.containerId);
                container.parentNode.removeChild(container);
            } catch (error) {
                //console.log("Error _imageSelector destroyFrame: " + error);
            }
        }
    });
});