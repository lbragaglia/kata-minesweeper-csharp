using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly string[] _minesField;

        public Game(int lines, int columns, string initialField)
        {
            var emptyLine = new[] { string.Join("", Enumerable.Repeat('.', columns + 2)) };
            var splittedLines = SplitLines(initialField, columns);
            _minesField = emptyLine
                .Concat(splittedLines)
                .Concat(emptyLine)
                .ToArray();
        }

        private static IEnumerable<string> SplitLines(string field, int lineSize)
        {
            for (int i = 0; i < field.Length; i += lineSize)
            {
                yield return '.' + field.Substring(i, Math.Min(lineSize, field.Length - i)) + '.';
            }
        }

        internal string AdjacencyField()
        {
            var adjacencyField = new int[_minesField.Length][];

            for (int i = 0; i < _minesField.Length; i++)
            {
                adjacencyField[i] = new int[_minesField[i].Length];
            }

            for (int i = 1; i < _minesField.Length - 1; i++)
            {
                for (int j = 1; j < _minesField[i].Length - 1; j++)
                {
                    if (_minesField[i][j] == '*')
                    {
                        adjacencyField[i - 1][j - 1] ++;
                        adjacencyField[i - 1][j + 0] ++;
                        adjacencyField[i - 1][j + 1] ++;
                        adjacencyField[i + 0][j - 1] ++;
                        adjacencyField[i + 0][j + 1] ++;
                        adjacencyField[i + 1][j - 1] ++;
                        adjacencyField[i + 1][j + 0] ++;
                        adjacencyField[i + 1][j + 1] ++;
                    }
                }
            }
            
            return string.Join("", adjacencyField.Skip(1).Take(_minesField.Length - 2)
                .SelectMany((line, i) => line.Zip(_minesField[i + 1], CellToString).Skip(1).Take(_minesField[i + 1].Length - 2)));
        }

        private string CellToString(int adjacentMines, char originalCell) => originalCell == '*' ? "*" : adjacentMines.ToString();
    }
}
