using System.Collections;
using System.Collections.Generic;

namespace Opera
{
    public class GameOption
    {
        public string optionName;
        public bool IsNewGame => string.IsNullOrEmpty(gameId);
        public string gameId;

        public static readonly GameOption newGameOption = new()
        {
            gameId = "",
            optionName = "[New Game...]",
        };
    }
}
