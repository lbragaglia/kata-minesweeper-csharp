using System;
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
        private int[] _adjacencyField;
        private static readonly int[] MineNeighbors = {1, 0, 1};
        private const int BorderSize = 2;

        public Game(string initialField)
        {
            _minesField = initialField;
            _fieldSize = initialField.Length;
            _extFieldSize = _fieldSize + BorderSize;
            BuildAdjacencyField();
        }

        private void BuildAdjacencyField()
        {
            _adjacencyField = new int[_extFieldSize];

            _minesField
                .Select(Square)
                .Where(ThereIsAMine)
                .Select(MineAdjacencyLayerOfSize(_extFieldSize))
                .Aggregate(_adjacencyField, SumOfLayers);
        }

        private static int[] SumOfLayers(int[] sum, int[] mineLayer)
        {
            for (int i = 0; i < mineLayer.Length; i++)
            {
                sum[i] += mineLayer[i];
            }
            return sum;
        }

        private static (bool, int) Square(char square, int index) => (square == '*', index);
 
        private static bool ThereIsAMine((bool isMinePresent, int) square) => square.isMinePresent;
 
        private static Func<(bool, int), int[]> MineAdjacencyLayerOfSize(int size) => ((bool, int offset) square) =>
        {
            var layer = new int[size];
            layer[square.offset + 0] = MineNeighbors[0];
            layer[square.offset + 1] = MineNeighbors[1];
            layer[square.offset + 2] = MineNeighbors[2];
            return layer;
        };
 
        internal string AdjacencyField()
        {
            var extMinesField = "." + _minesField + ".";
            
            return string.Join("", _adjacencyField.Zip(extMinesField, CellToString).Skip(1).Take(_fieldSize));
        }

        private string CellToString(int adjacentMines, char originalCell) => originalCell == '*' ? "*" : adjacentMines.ToString();
    }
}
