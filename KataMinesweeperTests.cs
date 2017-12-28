using System.Linq;
using Xunit;

namespace KataMinesweeper
{
    public class KataMinesweeperTests
    {
        [Theory]
        [InlineData("*", "*")]
        [InlineData("*.", "*1")]
        [InlineData(".*", "1*")]
        [InlineData("..", "00")]
        [InlineData("**", "**")]
        [InlineData("...", "000")]
        [InlineData("*..", "*10")]
        [InlineData(".*.", "1*1")]
        [InlineData("**.", "**1")]
        [InlineData("*.*", "*2*")]
        [InlineData("*..*", "*11*")]
        [InlineData("*...*", "*101*")]
        public void Test1(string minesField, string adjacencyField)
        {
            var game = new Game(minesField);
            Assert.Equal(adjacencyField, game.AdjacencyField());
        }
    }
    public class Game
    {
        private readonly string _minesField;
        private readonly int _fieldSize;
        private readonly int _extFieldSize;
        private const int BorderSize = 2;

        public Game(string initialField)
        {
            _minesField = initialField;
            _fieldSize = initialField.Length;
            _extFieldSize = _fieldSize + BorderSize;
        }

        internal string AdjacencyField()
        {
            var extMinesField = "." + _minesField + ".";
            var adjacencyField = new int[_extFieldSize];
            for (int i = 1; i < extMinesField.Length - 1; i++)
            {
                if (extMinesField[i] == '*')
                {
                    adjacencyField[i - 1] ++;
                    adjacencyField[i + 1] ++;
                }
            }

            return string.Join("", adjacencyField.Zip(extMinesField, CellToString).Skip(1).Take(_minesField.Length));
        }

        private string CellToString(int adjacentMines, char originalCell) => originalCell == '*' ? "*" : adjacentMines.ToString();
    }
}
