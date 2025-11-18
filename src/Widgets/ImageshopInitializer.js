define([
    "dojo",
    "dojo/_base/declare",
    "epi/_Module",
    "epi-cms/plugin-area/assets-pane",
    "imageshop-optimizely-plugin/AssetPane",
    "epi/i18n!epi/cms/nls/episerver.cms.widget.thumbnailselector"
], function (
    dojo,
    declare,
    _Module,
    assetsPanePluginArea,
    ImportCommand,
    resources
) {
    return declare([_Module], {
        initialize: function () {
            try {
                this.inherited(arguments);

                var registry = this.resolveDependency("epi.storeregistry");
                //Register the store
                registry.create("imageshopstore", "/imageshopextended/imageshopstore/");
                //debugger
                if (this._settings.pluginenableassetpane == true) {

                    var importCommand1 = new ImportCommand();
                    importCommand1.baseUrl = this._settings.baseUrl;
                    importCommand1.enableDocuments = false;
                    importCommand1.label = resources.importimageshoptitle;
                    assetsPanePluginArea.add(importCommand1);

                    if (this._settings.pluginenabledocuments == true) {
                        var importCommand = new ImportCommand();
                        importCommand.baseUrl = this._settings.baseUrl;
                        importCommand.enableDocuments = this._settings.pluginenabledocuments;
                        importCommand.label = resources.importimageshopdocumenttitle;
                        assetsPanePluginArea.add(importCommand);
                    }

                }

            } catch (error) {
                console.log("Error ImageshopInitializer.js initialize(): " + error);
            }

        }
    });
});