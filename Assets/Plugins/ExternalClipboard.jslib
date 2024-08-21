var CopyPastePlugin =
{
  CopyPasteReader: function(gObj, vName)
  {
      var gameObjectName = UTF8ToString(gObj);
      var voidName = UTF8ToString(vName);
      navigator.clipboard.readText().then(function(data) {
        unityInstance.SendMessage(gameObjectName, voidName, data);
      }, function() {
        unityInstance.SendMessage(gameObjectName, voidName, "no text aviable in clipboard");
      })
  }
};
mergeInto(LibraryManager.library, CopyPastePlugin);
