using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public void Test1D(string minesField, string adjacencyField)
        {
            var game = new Game(1, minesField.Length, minesField);
            Assert.Equal(adjacencyField, game.AdjacencyField());
        }

        [Theory]
        [InlineData(2, 2, "....", "0000")]
        [InlineData(2, 2, "...*", "111*")]
        [InlineData(3, 3, "....*....", "1111*1111")]
        [InlineData(5, 5, "............*............", "000000111001*100111000000")]
        [InlineData(4, 4, "*........*......", "*10022101*101110")]
        [InlineData(3, 5, "**.........*...", "**100332001*100")]
        public void Test2D(int lines, int columns, string minesField, string adjacencyField)
        {
            var game = new Game(lines, columns, minesField);
            Assert.Equal(adjacencyField, game.AdjacencyField());
        }
    }
    public class Game
    {
        private readonly string _minesField;
        private readonly int _lines;
        private readonly int _lineSize;
        private readonly int _extLineSize;
        private readonly int _extFieldSize;
        private int[] _adjacencyField;
        private static readonly int[,] NeighborsMask = new int[3,3] {{1, 1, 1}, {1, 0, 1}, {1, 1, 1}};
        private const int BorderSize = 2;

        public Game(int lines, int columns, string initialField)
        {
            _lines = lines;
            _lineSize = columns;
            _extLineSize = columns + BorderSize;
            _extFieldSize = _extLineSize * (lines + BorderSize);
            
            var emptyLine = Enumerable.Repeat('.', _lineSize + BorderSize);
            var splittedLines = SplitLines(initialField, columns);
            _minesField = string.Concat(emptyLine.Concat(splittedLines.SelectMany(line => line)).Concat(emptyLine));

            BuildAdjacencyField();
        }

        private static IEnumerable<string> SplitLines(string field, int lineSize)
        {
            for (int i = 0; i < field.Length; i += lineSize)
            {
                yield return '.' + field.Substring(i, Math.Min(lineSize, field.Length - i)) + '.';
            }
        }

        private void BuildAdjacencyField()
        {
            _adjacencyField = new int[_extFieldSize];

            _minesField
                .Select(Square)
                .Where(ThereIsAMine)
                .Aggregate(_adjacencyField, SumOfLayers(_extLineSize));
        }

        private static (bool, int) Square(char square, int index) => (square == '*', index);
 
        private static bool ThereIsAMine((bool isMinePresent, int) square) => square.isMinePresent;

        private static Func<int[], (bool, int), int[]> SumOfLayers(int lineSize) => (int[] sum, (bool, int offset) mineLayer) =>
        {
            for (int i = 0; i < NeighborsMask.GetLength(0); i++)
            {
                for (int j = 0; j < NeighborsMask.GetLength(1); j++)
                {
                    sum[mineLayer.offset + (i - 1) * lineSize + j - 1] += NeighborsMask[i, j];
                }
            }
            return sum;
        };

        internal string AdjacencyField()
        {
            return StripBorder(_adjacencyField.Zip(_minesField, CellToString));
        }

        private string StripBorder(IEnumerable<char> extResultField)
        {
            var strippedResult = new StringBuilder();
            var extResultList = extResultField.ToList();
            for (int i = 0; i < _lines; i++)
            {
                strippedResult.Append(string.Concat(extResultList.GetRange((i + 1) * _extLineSize + 1, _lineSize)));
            }
            return strippedResult.ToString();
        }

        private char CellToString(int adjacentMines, char originalCell) => originalCell == '*' ? '*' : adjacentMines.ToString()[0];
    }
}
