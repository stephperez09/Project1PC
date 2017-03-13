using System.Linq;
using System.Collections.Generic;
using System;
using Xunit;
using FluentAssertions;

namespace Cecs475.BoardGames.Chess.Test
{
    /// <summary>
    /// This partial class implementation includes many utility methods to make writing tests easier. 
    /// </summary>
    public partial class ChessTests
    {
        // For parsing moves and positions.
        private static ChessView BlankView = new ChessView();

        /// <summary>
        /// Creates a ChessMove from a string in the same format as ChessView.ParseMove expects.
        /// </summary>
        private static ChessMove Move(string moveString)
        {
            return BlankView.ParseMove(moveString) as ChessMove;
        }

        /// <summary>
        /// Creates a ChessMove from two strings reprenting the start and end chess coordinates.
        /// </summary>
        private static ChessMove Move(string start, string end)
        {
            return new ChessMove(Pos(start), Pos(end));
        }

        /// <summary>
        /// Creates a BoardPosition from a row integer and column integer, where (0,0) is chess square a8.
        /// </summary>
        private static BoardPosition Pos(int row, int col)
        {
            return new BoardPosition(row, col);
        }

        /// <summary>
        /// Creates a BoardPosition from a string representing a chess coordinate in algebraic notation.
        /// </summary>
        private static BoardPosition Pos(string position)
        {
            return BlankView.ParsePosition(position);
        }

        /// <summary>
        /// Creates a new ChessBoard by applying the given sequence of moves to a starting board.
        /// </summary>
        private static ChessBoard CreateBoardFromMoves(IEnumerable<ChessMove> moves)
        {
            ChessBoard b = new ChessBoard();
            ApplyMovesToBoard(b, moves);
            return b;
        }

        /// <summary>
        /// Creates a new ChessBoard by manually specifying which pieces are at which positions. 
        /// </summary>
        /// <param name="positions">must provide 3 values for each piece: a BoardPosition, a ChessPieceType, and an 
        /// int player, in that order.</param>
        /// <returns></returns>
        private static ChessBoard CreateBoardWithPositions(params object[] positions)
        {
            var p = new List<Tuple<BoardPosition, ChessPiecePosition>>();
            for (int i = 0; i < positions.Length; i += 3)
            {
                p.Add(Tuple.Create((BoardPosition)positions[i],
                    new ChessPiecePosition(
                        (ChessPieceType)positions[i + 1],
                        (int)positions[i + 2])));
            }
            return new ChessBoard(p);
        }

        /// <summary>
        /// Applies the given move to the given board.
        /// </summary>
        private static void ApplyMove(ChessBoard board, ChessMove move)
        {
            int currentValue = board.Value;
            var possMoves = board.GetPossibleMoves();
            board.Value.Should().Be(currentValue, "the board's value should not change after calling GetPossibleMoves");
            var toApply = possMoves.FirstOrDefault(m => m.Equals(move));
            if (toApply == null)
            {
                throw new InvalidOperationException("Could not apply the move " + move);
            }
            else
            {
                board.ApplyMove(toApply);
            }
        }

        /// <summary>
        /// Applies the given sequence of moves to the given board.
        /// </summary>
        private static void ApplyMovesToBoard(ChessBoard board, IEnumerable<ChessMove> moves)
        {
            foreach (var move in moves)
            {
                ApplyMove(board, move);
            }
        }

        /// <summary>
        /// Returns all moves from the given sequence that start at the given position.
        /// </summary>
        private static IEnumerable<ChessMove> GetMovesAtPosition(IEnumerable<ChessMove> moves, BoardPosition pos)
        {
            return moves.Where(m => m.StartPosition.Equals(pos));
        }

        /// <summary>
        /// Returns all chess piece positions controlled by the given player
        /// </summary>
        private static IEnumerable<ChessPiecePosition> GetAllPiecesForPlayer(ChessBoard b, int player)
        {
            return
                from pos in (
                    from row in Enumerable.Range(0, 8)
                    from col in Enumerable.Range(0, 8)
                    select b.GetPieceAtPosition(new BoardPosition(row, col))
                )
                where pos.Player == player
                select pos;

        }

        [Fact]
        public void InitialStartingBoard()
        {
            ChessBoard b = new ChessBoard();
            for (int i = 0; i < 8; i++)
            {
                b.GetPieceAtPosition(Pos(6, i)).PieceType.Should().Be(ChessPieceType.Pawn);
                b.GetPieceAtPosition(Pos(1, i)).PieceType.Should().Be(ChessPieceType.Pawn);
                switch (i)
                {
                    case 0:
                        b.GetPieceAtPosition(Pos(7, i)).PieceType.Should().Be(ChessPieceType.RookQueen);
                        b.GetPieceAtPosition(Pos(0, i)).PieceType.Should().Be(ChessPieceType.RookQueen);
                        break;
                    case 1:
                        b.GetPieceAtPosition(Pos(7, i)).PieceType.Should().Be(ChessPieceType.Knight);
                        b.GetPieceAtPosition(Pos(0, i)).PieceType.Should().Be(ChessPieceType.Knight);
                        break;
                    case 2:
                        b.GetPieceAtPosition(Pos(7, i)).PieceType.Should().Be(ChessPieceType.Bishop);
                        b.GetPieceAtPosition(Pos(0, i)).PieceType.Should().Be(ChessPieceType.Bishop);
                        break;
                    case 3:
                        b.GetPieceAtPosition(Pos(7, i)).PieceType.Should().Be(ChessPieceType.Queen);
                        b.GetPieceAtPosition(Pos(0, i)).PieceType.Should().Be(ChessPieceType.Queen);
                        break;
                    case 4:
                        b.GetPieceAtPosition(Pos(7, i)).PieceType.Should().Be(ChessPieceType.King);
                        b.GetPieceAtPosition(Pos(0, i)).PieceType.Should().Be(ChessPieceType.King);
                        break;
                    case 5:
                        b.GetPieceAtPosition(Pos(7, i)).PieceType.Should().Be(ChessPieceType.Bishop);
                        b.GetPieceAtPosition(Pos(0, i)).PieceType.Should().Be(ChessPieceType.Bishop);
                        break;
                    case 6:
                        b.GetPieceAtPosition(Pos(7, i)).PieceType.Should().Be(ChessPieceType.Knight);
                        b.GetPieceAtPosition(Pos(0, i)).PieceType.Should().Be(ChessPieceType.Knight);
                        break;
                    case 7:
                        b.GetPieceAtPosition(Pos(7, i)).PieceType.Should().Be(ChessPieceType.RookKing);
                        b.GetPieceAtPosition(Pos(0, i)).PieceType.Should().Be(ChessPieceType.RookKing);
                        break;
                }
            }


        }

        [Fact]
        public void PawnRowCheck()
        {
            ChessBoard b = new ChessBoard();
            for (int i = 0; i < 8; i++)
            {
                b.GetPieceAtPosition(Pos(6, i)).PieceType.Should().Be(ChessPieceType.Pawn, "Each piece on row 2 should be a pawn");
                b.GetPlayerAtPosition(Pos(6, i)).Should().Be(1, "Player 1 should have possession of these pawns");
            }

            for (int i = 0; i < 8; i++)
            {
                b.GetPieceAtPosition(Pos(1, i)).PieceType.Should().Be(ChessPieceType.Pawn, "Each piece on row 7 should be a pawn");
                b.GetPlayerAtPosition(Pos(1, i)).Should().Be(2, "Player 2 should have possession of these pawns");
            }
            	var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
              var whitePawn = GetMovesAtPosition(possMoves, Pos("g1"));
            	whitePawn.Should().HaveCount(3, "Pawn has 2 possible move").And.Contain(Move("g1","f3")).And.Contain(Move("g1","h3")).And.Contain(Move("g1","g4"));
        }


    }
}