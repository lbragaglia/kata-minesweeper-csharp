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
        public void Test1D(string inputField, string expectedField)
        {
            var game = new Game(1, inputField.Length, inputField);
            Assert.Equal(expectedField, game.AdjacencyField());
        }

        [Theory]
        [InlineData(2, 2, "....", "0000")]
        [InlineData(2, 2, "...*", "111*")]
        [InlineData(3, 3, "....*....", "1111*1111")]
        [InlineData(5, 5, "............*............", "000000111001*100111000000")]
        [InlineData(4, 4, "*........*......", "*10022101*101110")]
        [InlineData(3, 5, "**.........*...", "**100332001*100")]
        public void Test2D(int lines, int columns, string inputField, string expectedField)
        {
            var game = new Game(lines, columns, inputField);
            Assert.Equal(expectedField, game.AdjacencyField());
        }
    }

    public class Game
    {
        private readonly int _lines;
        private readonly int _lineSize;
        private readonly int _extLineSize;
        private readonly string _minesField;
        private readonly int[] _adjacencyField;

        private static readonly int[,] NeighborsMask = new int[3, 3] { { 1, 1, 1 }, { 1, 0, 1 }, { 1, 1, 1 } };
        private const int BorderSize = 2;
        private const char EmptySquare = '.';
        private const char MineSquare = '*';

        public Game(int lines, int columns, string initialField)
        {
            _lines = lines;
            _lineSize = columns;
            _extLineSize = columns + BorderSize;

            _minesField = ExtendMinesField(initialField);
            _adjacencyField = BuildAdjacencyField(_extLineSize * (lines + BorderSize));
        }

        private string ExtendMinesField(string initialField)
        {
            var emptyLine = Enumerable.Repeat(EmptySquare, _extLineSize);
            var extendedLines = SplitLines(initialField, _lineSize);
            return string.Concat(emptyLine.Concat(extendedLines.SelectMany(line => line)).Concat(emptyLine));
        }

        private static IEnumerable<string> SplitLines(string field, int lineSize)
        {
            for (int i = 0; i < field.Length; i += lineSize)
            {
                yield return EmptySquare + field.Substring(i, Math.Min(lineSize, field.Length - i)) + EmptySquare;
            }
        }

        private int[] BuildAdjacencyField(int extendedFieldSize)
        {
            var adjacencyField = new int[extendedFieldSize];

            _minesField
                .Select(Square)
                .Where(ThereIsAMine)
                .Aggregate(adjacencyField, SumOfLayers(_extLineSize));

            return adjacencyField;
        }

        private static (bool, int) Square(char square, int index) => (square == MineSquare, index);

        private static bool ThereIsAMine((bool isMinePresent, int) square) => square.isMinePresent;

        private static Func<int[], (bool, int), int[]> SumOfLayers(int lineSize) => (int[] sum, (bool, int offset) mine) =>
        {
            for (int i = 0; i < NeighborsMask.GetLength(0); i++)
            {
                for (int j = 0; j < NeighborsMask.GetLength(1); j++)
                {
                    sum[mine.offset + (i - 1) * lineSize + j - 1] += NeighborsMask[i, j];
                }
            }
            return sum;
        };

        internal string AdjacencyField() => StripBorder(_adjacencyField.Zip(_minesField, PrintSquare));

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

        private char PrintSquare(int adjacentMines, char originalSquare) => originalSquare == MineSquare ? MineSquare : adjacentMines.ToString()[0];
    }
}
