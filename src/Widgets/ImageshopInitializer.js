define([
    "dojo",
    "dojo/_base/declare",
    "epi/_Module",
    "epi-cms/plugin-area/assets-pane",
    "imageshop-optimizely-plugin/AssetPane",
    "epi/dependency",
    "epi/routes"
], function (
    dojo,
    declare,
    _Module,
    assetsPanePluginArea,
    ImportCommand,
    dependency,
    routes
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
                    importCommand1.enableDocuments = this._settings.pluginenabledocuments;
                    importCommand1.label = "Import from Imageshop";// resources.importTitle;
                    assetsPanePluginArea.add(importCommand1);
                }

            } catch (error) {
                console.log("Error ImageshopInitializer.js initialize(): " + error);
            }

        }
    });
});