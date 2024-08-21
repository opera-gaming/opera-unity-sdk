var HandleIO = {
     SyncFiles : function()
     {
         FS.syncfs(false,function (err) {
             // handle callback
         });
     }
};
mergeInto(LibraryManager.library, HandleIO);
