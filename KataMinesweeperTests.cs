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
                .Select(MineLayer)
                .Where(LayersWithMines)
                .Select(ToBinaryString)
                .Select(ToIntArray)
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

        private static int[] ToIntArray(string layerMines)
        {
            return layerMines.Select(_ => Convert.ToInt32(_.ToString())).ToArray();
        }

        private string ToBinaryString(long layerMines)
        {
            return Convert.ToString(layerMines, 2).PadLeft(_extFieldSize, '0');
        }

        private static bool LayersWithMines(long layerMines)
        {
            return layerMines > 0;
        }

        private long MineLayer(char cell, int index)
        {
            double pos = Math.Pow(2, _fieldSize - index - 1);
            return cell == '*' ? 5 * (long)pos : 0;
        }

        internal string AdjacencyField()
        {
            var extMinesField = "." + _minesField + ".";
            return string.Join("", _adjacencyField.Zip(extMinesField, CellToString).Skip(1).Take(_fieldSize));
        }

        private string CellToString(int adjacentMines, char originalCell) => originalCell == '*' ? "*" : adjacentMines.ToString();
    }
}
