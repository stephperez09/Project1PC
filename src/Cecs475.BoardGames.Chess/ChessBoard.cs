using System;
using System.Collections.Generic;
using System.Linq;


namespace Cecs475.BoardGames.Chess
{

    public class ChessBoard : IGameBoard
    {
        /// <summary>
        /// The number of rows and columns on the chess board.
        /// </summary>
        public const int BOARD_SIZE = 8;

        // Reminder: there are 3 different types of rooks
        private sbyte[,] mBoard = new sbyte[8, 8] {
            {-2, -4, -5, -6, -7, -5, -4, -3 },
            {-1, -1, -1, -1, -1, -1, -1, -1 },
            {0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0 },
            {1, 1, 1, 1, 1, 1, 1, 1 },
            {2, 4, 5, 6, 7, 5, 4, 3 }
        };

        private int mCurrentPlayer;

        // TODO:
        // You need a way of keeping track of certain game state flags. For example, a rook cannot perform a castling move
        // if either the rook or its king has moved in the game, so you need a way of determining whether those things have 
        // happened. There are several ways to do it and I leave it up to you.



        /// <summary>
        /// Constructs a new chess board with the default starting arrangement.
        /// </summary>
        public ChessBoard()
        {
            MoveHistory = new List<IGameMove>();
            mCurrentPlayer = 1;
            mBoard[0, 0] = (sbyte)ChessPieceType.RookQueen;
            mBoard[0, 1] = (sbyte)ChessPieceType.Knight;
            mBoard[0, 2] = (sbyte)ChessPieceType.Bishop;
            mBoard[0, 3] = (sbyte)ChessPieceType.Queen;
            mBoard[0, 4] = (sbyte)ChessPieceType.King;
            mBoard[0, 5] = (sbyte)ChessPieceType.Bishop;
            mBoard[0, 6] = (sbyte)ChessPieceType.Knight;
            mBoard[0, 7] = (sbyte)ChessPieceType.RookKing;
            mBoard[7, 0] = (sbyte)ChessPieceType.RookQueen;
            mBoard[7, 1] = (sbyte)ChessPieceType.Knight;
            mBoard[7, 2] = (sbyte)ChessPieceType.Bishop;
            mBoard[7, 3] = (sbyte)ChessPieceType.Queen;
            mBoard[7, 4] = (sbyte)ChessPieceType.King;
            mBoard[7, 5] = (sbyte)ChessPieceType.Bishop;
            mBoard[7, 6] = (sbyte)ChessPieceType.Knight;
            mBoard[7, 7] = (sbyte)ChessPieceType.RookKing;
            /* 
			mBoard[0,0] = -2;
			mBoard[0,1] = -4;
			mBoard[0,2] = -5;
			mBoard[0,3] = -6;
			mBoard[0,4] = -7;
			mBoard[0,5] = -5;
			mBoard[0,6] = -4;
			mBoard[0,7] = -3;
			mBoard[7,0] = 2;
			mBoard[7,1] = 4;
			mBoard[7,2] = 5;
			mBoard[7,3] = 6;
			mBoard[7,4] = 7;
			mBoard[7,5] = 5;
			mBoard[7,6] = 4;
			mBoard[7,7] = 3;
			*/
            for (int i = 0; i < 8; i++)
            {
                mBoard[6, i] = (sbyte)ChessPieceType.Pawn;
                mBoard[6, i] = 1;
            }
            for (int i = 0; i < 8; i++)
            {
                mBoard[1, i] = (sbyte)ChessPieceType.Pawn;
                mBoard[1, i] = -1;
            }
            Value = 0;

        }

        /// <summary>
        /// Constructs a new chess board by only placing pieces as specified.
        /// </summary>
        /// <param name="startingPositions">a sequence of tuple pairs, where each pair specifies the starting
        /// position of a particular piece to place on the board</param>
        public ChessBoard(IEnumerable<Tuple<BoardPosition, ChessPiecePosition>> startingPositions)

            : this()
        { // NOTE THAT THIS CONSTRUCTOR CALLS YOUR DEFAULT CONSTRUCTOR FIRST


            foreach (int i in Enumerable.Range(0, 8))
            { // another way of doing for i = 0 to < 8
                foreach (int j in Enumerable.Range(0, 8))
                {
                    mBoard[i, j] = 0;
                }
            }
            foreach (var pos in startingPositions)
            {
                SetPosition(pos.Item1, pos.Item2);
            }
        }

        /// <summary>
        /// A difference in piece values for the pieces still controlled by white vs. black, where
        /// a pawn is value 1, a knight and bishop are value 3, a rook is value 5, and a queen is value 9.
        /// </summary>
        public int Value { get; private set; }

        //Did this TODO
        public int CurrentPlayer
        {
            get
            {
                return mCurrentPlayer == 1 ? 1 : 2;
            }
        }


        // An auto-property suffices here.
        public IList<IGameMove> MoveHistory
        {
            get; private set;
        }

        /// <summary>
        /// Returns the piece and player at the given position on the board.
        /// </summary>
        public ChessPiecePosition GetPieceAtPosition(BoardPosition position)
        {
            var boardVal = mBoard[position.Row, position.Col];
            return new ChessPiecePosition((ChessPieceType)Math.Abs(mBoard[position.Row, position.Col]),
                boardVal > 0 ? 1 : boardVal < 0 ? 2 : 0);
        }


        public void ApplyMove(IGameMove move)
        {
            // TODO: implement this method.
            var player = CurrentPlayer;
            var enemy = player * -1;
            ChessMove cMove = move as ChessMove;
            //mBoard[cMove.StartPosition.Row, cMove.StartPosition.Col] = (sbyte)mCurrentPlayer;
            mBoard[cMove.EndPosition.Row, cMove.EndPosition.Col] = (sbyte)CurrentPlayer;

        }

        public IEnumerable<IGameMove> GetPossibleMoves()
        {
            // TODO: implement this method by returning a list of all possible moves.
            List<ChessMove> possibleMoves = new List<ChessMove>();
            var player = CurrentPlayer;
            var enemy = player * -1;

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    BoardPosition bp = new BoardPosition(i, j);
                    var boardVal = mBoard[i, j];
                    ChessPiecePosition piece = new ChessPiecePosition((ChessPieceType)Math.Abs(mBoard[bp.Row, bp.Col]),
                        boardVal > 0 ? 1 : boardVal < 0 ? 2 : 0);

                    if (piece.PieceType.Equals(ChessPieceType.Pawn))
                    {
                        List<IGameMove> pawns = new List<IGameMove>();
                        var pawnMoves = PawnPossibleMoves(bp) as IEnumerable<ChessMove>;
                        possibleMoves.AddRange(pawnMoves);
                    }
                    if (piece.PieceType.Equals(ChessPieceType.Knight))
                    {
                        List<IGameMove> knight = new List<IGameMove>();
                        var knightMoves = KnightPossibleMoves(bp) as IEnumerable<ChessMove>;
                        possibleMoves.AddRange(knightMoves);
                    }
                    if (piece.PieceType.Equals(ChessPieceType.Bishop))
                    {
                        List<IGameMove> bishop = new List<IGameMove>();
                        var bishopMoves = BishopPossibleMoves(bp) as IEnumerable<ChessMove>;
                        possibleMoves.AddRange(bishopMoves);
                    }
                    if (piece.PieceType.Equals(ChessPieceType.Queen))
                    {
                        List<IGameMove> queen = new List<IGameMove>();
                        var queenMoves = QueenPossibleMoves(bp) as IEnumerable<ChessMove>;
                        possibleMoves.AddRange(queenMoves);
                    }
                    if (piece.PieceType.Equals(ChessPieceType.King))
                    {
                        List<IGameMove> king = new List<IGameMove>();
                        var kingMoves = KingPossibleMoves(bp) as IEnumerable<ChessMove>;
                        possibleMoves.AddRange(kingMoves);
                    }
                    if (piece.PieceType.Equals(ChessPieceType.RookKing) || piece.PieceType.Equals(ChessPieceType.RookQueen))
                    {
                        List<IGameMove> knight = new List<IGameMove>();
                        var knightMoves = KnightPossibleMoves(bp) as IEnumerable<ChessMove>;
                        possibleMoves.AddRange(knightMoves);
                    }


                }

            }
            return possibleMoves;
        }

        public IEnumerable<IGameMove> PawnPossibleMoves(BoardPosition pawn)
        {
            List<ChessMove> pawnMoves = new List<ChessMove>();
            var player = CurrentPlayer;
            var enemy = player * -1;
            var position = mBoard[pawn.Row, pawn.Col];
            BoardPosition whiteDiagonalLeft = new BoardPosition(pawn.Row - 1, pawn.Col - 1);
            BoardPosition whiteDiagonalRight = new BoardPosition(pawn.Row - 1, pawn.Col + 1);
            BoardPosition blackDiagonalLeft = new BoardPosition(pawn.Row + 1, pawn.Col - 1);
            BoardPosition blackDiagonalRight = new BoardPosition(pawn.Row + 1, pawn.Col + 1);
            BoardPosition whitePawnFowardOne = new BoardPosition(pawn.Row - 1, pawn.Col);
            BoardPosition whitePawnFowardTwo = new BoardPosition(pawn.Row - 2, pawn.Col);
            BoardPosition blackPawnFowardOne = new BoardPosition(pawn.Row + 1, pawn.Col);
            BoardPosition blackPawnFowardTwo = new BoardPosition(pawn.Row + 2, pawn.Col);
            if (position > 0)
            {
                if (pawn.Row == 6)
                {
                    if (pawn.Col == 0 || pawn.Col == 1 || pawn.Col == 2 || pawn.Col == 3 || pawn.Col == 4 || pawn.Col == 5 || pawn.Col == 6 || pawn.Col == 7)
                    {
                        if (PositionIsEmpty(whitePawnFowardOne) == true)
                        {
                            pawnMoves.Add(new ChessMove(pawn, whitePawnFowardOne));
                        }
                        if (PositionIsEmpty(whitePawnFowardTwo))
                        {
                            pawnMoves.Add(new ChessMove(pawn, whitePawnFowardTwo));
                        }
                    }
                }
                else
                {
                    if (PositionIsEmpty(whitePawnFowardOne))
                    {
                        pawnMoves.Add(new ChessMove(pawn, whitePawnFowardOne));
                    }
                }
                if (PositionInBounds(whiteDiagonalLeft) == true)
                {
                    if (PositionIsEnemy(whiteDiagonalLeft, enemy))
                    {
                        pawnMoves.Add(new ChessMove(pawn, whiteDiagonalLeft));
                    }
                }
                if (PositionInBounds(whiteDiagonalRight) == true)
                {
                    if (PositionIsEnemy(whiteDiagonalRight, enemy))
                    {
                        pawnMoves.Add(new ChessMove(pawn, whiteDiagonalRight));
                    }
                }
            }
            else
            {
                if (pawn.Row == 1)
                {
                    if (pawn.Col == 0 || pawn.Col == 1 || pawn.Col == 2 || pawn.Col == 3 || pawn.Col == 4 || pawn.Col == 5 || pawn.Col == 6 || pawn.Col == 7)
                    {
                        if (PositionIsEmpty(blackPawnFowardOne) == true)
                        {
                            pawnMoves.Add(new ChessMove(pawn, blackPawnFowardOne));
                        }
                        if (PositionIsEmpty(blackPawnFowardTwo))
                        {
                            pawnMoves.Add(new ChessMove(pawn, blackPawnFowardTwo));
                        }
                    }
                }
                else
                {
                    if (PositionIsEmpty(blackPawnFowardOne))
                    {
                        pawnMoves.Add(new ChessMove(pawn, blackPawnFowardOne));
                    }
                }
                if (PositionInBounds(blackDiagonalLeft) == true)
                {
                    if (PositionIsEnemy(blackDiagonalLeft, enemy))
                    {
                        pawnMoves.Add(new ChessMove(pawn, blackDiagonalLeft));
                    }
                }
                if (PositionInBounds(blackDiagonalRight) == true)
                {
                    if (PositionIsEnemy(blackDiagonalRight, enemy))
                    {
                        pawnMoves.Add(new ChessMove(pawn, blackDiagonalRight));
                    }
                }
            }
            return pawnMoves;
        }

        public IEnumerable<IGameMove> RookPossibleMoves(BoardPosition rook)
        {
            List<ChessMove> rookMoves = new List<ChessMove>();
            var player = CurrentPlayer;
            var enemy = player * -1;

            for (int i = 1; i < BOARD_SIZE; i++)
            {
                if (PositionInBounds(new BoardPosition(rook.Row - i, rook.Col)) == true)
                {
                    if (PositionIsEmpty(new BoardPosition(rook.Row - i, rook.Col)) == true || PositionIsEnemy(new BoardPosition(rook.Row - i, rook.Col), enemy) == true)
                    {
                        rookMoves.Add(new ChessMove(rook, new BoardPosition(rook.Row - i, rook.Col)));
                    }
                }
            }
            for (int i = 1; i < BOARD_SIZE; i++)
            {
                if (PositionInBounds(new BoardPosition(rook.Row + i, rook.Col)) == true)
                {
                    if (PositionIsEmpty(new BoardPosition(rook.Row + i, rook.Col)) == true || PositionIsEnemy(new BoardPosition(rook.Row + i, rook.Col), enemy) == true)
                    {
                        rookMoves.Add(new ChessMove(rook, new BoardPosition(rook.Row + i, rook.Col)));
                    }
                }
            }
            for (int i = 1; i < BOARD_SIZE; i++)
            {
                if (PositionInBounds(new BoardPosition(rook.Row, rook.Col - i)) == true)
                {
                    if (PositionIsEmpty(new BoardPosition(rook.Row, rook.Col - i)) == true || PositionIsEnemy(new BoardPosition(rook.Row, rook.Col - i), enemy) == true)
                    {
                        rookMoves.Add(new ChessMove(rook, new BoardPosition(rook.Row, rook.Col - i)));
                    }
                }
            }
            for (int i = 1; i < BOARD_SIZE; i++)
            {
                if (PositionInBounds(new BoardPosition(rook.Row, rook.Col + i)) == true)
                {
                    if (PositionIsEmpty(new BoardPosition(rook.Row, rook.Col + i)) == true || PositionIsEnemy(new BoardPosition(rook.Row, rook.Col + i), enemy) == true)
                    {
                        rookMoves.Add(new ChessMove(rook, new BoardPosition(rook.Row, rook.Col + i)));
                    }
                }
            }
            return rookMoves;

        }

        public IEnumerable<IGameMove> BishopPossibleMoves(BoardPosition bishop)
        {
            List<ChessMove> bishopMoves = new List<ChessMove>();
            var player = CurrentPlayer;
            var enemy = player * -1;

            for (int i = 1; i < 7; i++)
            {
                if (PositionInBounds(new BoardPosition(bishop.Row - i, bishop.Col - i)) == true)
                {
                    if (PositionIsEmpty(new BoardPosition(bishop.Row - i, bishop.Col - i)) == true || PositionIsEnemy(new BoardPosition(bishop.Row - i, bishop.Col - i), enemy) == true)
                    {
                        bishopMoves.Add(new ChessMove(bishop, new BoardPosition(bishop.Row - i, bishop.Col - i)));
                    }
                }
            }
            for (int i = 1; i < 7; i++)
            {
                if (PositionInBounds(new BoardPosition(bishop.Row - i, bishop.Col + i)) == true)
                {
                    if (PositionIsEmpty(new BoardPosition(bishop.Row - i, bishop.Col + i)) == true || PositionIsEnemy(new BoardPosition(bishop.Row - i, bishop.Col + i), enemy) == true)
                    {
                        bishopMoves.Add(new ChessMove(bishop, new BoardPosition(bishop.Row - i, bishop.Col + i)));
                    }
                }
            }
            for (int i = 1; i < 7; i++)
            {
                if (PositionInBounds(new BoardPosition(bishop.Row + i, bishop.Col - i)) == true)
                {
                    if (PositionIsEmpty(new BoardPosition(bishop.Row + i, bishop.Col - i)) == true || PositionIsEnemy(new BoardPosition(bishop.Row + i, bishop.Col - i), enemy) == true)
                    {
                        bishopMoves.Add(new ChessMove(bishop, new BoardPosition(bishop.Row + i, bishop.Col - i)));
                    }
                }
            }
            for (int i = 1; i < 7; i++)
            {
                if (PositionInBounds(new BoardPosition(bishop.Row + i, bishop.Col + i)) == true)
                {
                    if (PositionIsEmpty(new BoardPosition(bishop.Row + i, bishop.Col + i)) == true || PositionIsEnemy(new BoardPosition(bishop.Row + i, bishop.Col + i), enemy) == true)
                    {
                        bishopMoves.Add(new ChessMove(bishop, new BoardPosition(bishop.Row + i, bishop.Col + i)));
                    }
                }
            }
            return bishopMoves;
        }

        public IEnumerable<IGameMove> KingPossibleMoves(BoardPosition king)
        {
            List<ChessMove> kingMoves = new List<ChessMove>();
            var player = CurrentPlayer;
            var enemy = player * -1;

            BoardPosition KingLeft = new BoardPosition(king.Row, king.Col - 1);
            BoardPosition KingRight = new BoardPosition(king.Row, king.Col + 1);
            BoardPosition KingUp = new BoardPosition(king.Row - 1, king.Col);
            BoardPosition KingDown = new BoardPosition(king.Row + 1, king.Col);
            BoardPosition KingDUL = new BoardPosition(king.Row - 1, king.Col - 1);
            BoardPosition KingDUR = new BoardPosition(king.Row - 1, king.Col + 1);
            BoardPosition KingDDL = new BoardPosition(king.Row + 1, king.Col - 1);
            BoardPosition KingDDR = new BoardPosition(king.Row + 1, king.Col + 1);

            if (PositionInBounds(KingLeft) == true)
            {
                if (PositionIsEmpty(KingLeft) == true || PositionIsEnemy(KingLeft, enemy) == true)
                {
                    kingMoves.Add(new ChessMove(king, KingLeft));
                }
            }
            if (PositionInBounds(KingRight) == true)
            {
                if (PositionIsEmpty(KingRight) == true || PositionIsEnemy(KingRight, enemy) == true)
                {
                    kingMoves.Add(new ChessMove(king, KingRight));
                }
            }
            if (PositionInBounds(KingUp) == true)
            {
                if (PositionIsEmpty(KingUp) == true || PositionIsEnemy(KingUp, enemy) == true)
                {
                    kingMoves.Add(new ChessMove(king, KingUp));
                }
            }
            if (PositionInBounds(KingDown) == true)
            {
                if (PositionIsEmpty(KingDown) == true || PositionIsEnemy(KingDown, enemy) == true)
                {
                    kingMoves.Add(new ChessMove(king, KingDown));
                }
            }
            if (PositionInBounds(KingDUL) == true)
            {
                if (PositionIsEmpty(KingDUL) == true || PositionIsEnemy(KingDUL, enemy) == true)
                {
                    kingMoves.Add(new ChessMove(king, KingDUL));
                }
            }
            if (PositionInBounds(KingDUR) == true)
            {
                if (PositionIsEmpty(KingDUR) == true || PositionIsEnemy(KingDUR, enemy) == true)
                {
                    kingMoves.Add(new ChessMove(king, KingDUR));
                }
            }
            if (PositionInBounds(KingDDL) == true)
            {
                if (PositionIsEmpty(KingDDL) == true || PositionIsEnemy(KingDDL, enemy) == true)
                {
                    kingMoves.Add(new ChessMove(king, KingDDL));
                }
            }
            if (PositionInBounds(KingDDR) == true)
            {
                if (PositionIsEmpty(KingDDR) == true || PositionIsEnemy(KingDDR, enemy) == true)
                {
                    kingMoves.Add(new ChessMove(king, KingDDR));
                }
            }
            return kingMoves;
        }

        public IEnumerable<IGameMove> QueenPossibleMoves(BoardPosition queen)
        {
            List<ChessMove> queenMoves = new List<ChessMove>();
            var player = CurrentPlayer;
            var enemy = player * -1;

            for (int i = 1; i < BOARD_SIZE; i++)
            {
                if (PositionInBounds(new BoardPosition(queen.Row - i, queen.Col)) == true)
                {
                    if (PositionIsEmpty(new BoardPosition(queen.Row - i, queen.Col)) == true || PositionIsEnemy(new BoardPosition(queen.Row - i, queen.Col), enemy) == true)
                    {
                        queenMoves.Add(new ChessMove(queen, new BoardPosition(queen.Row - i, queen.Col)));
                    }
                }
            }
            for (int i = 1; i < BOARD_SIZE; i++)
            {
                if (PositionInBounds(new BoardPosition(queen.Row + i, queen.Col)) == true)
                {
                    if (PositionIsEmpty(new BoardPosition(queen.Row + i, queen.Col)) == true || PositionIsEnemy(new BoardPosition(queen.Row + i, queen.Col), enemy) == true)
                    {
                        queenMoves.Add(new ChessMove(queen, new BoardPosition(queen.Row + i, queen.Col)));
                    }
                }
            }
            for (int i = 1; i < BOARD_SIZE; i++)
            {
                if (PositionInBounds(new BoardPosition(queen.Row, queen.Col - i)) == true)
                {
                    if (PositionIsEmpty(new BoardPosition(queen.Row, queen.Col - i)) == true || PositionIsEnemy(new BoardPosition(queen.Row, queen.Col - i), enemy) == true)
                    {
                        queenMoves.Add(new ChessMove(queen, new BoardPosition(queen.Row, queen.Col - i)));
                    }
                }
            }
            for (int i = 1; i < BOARD_SIZE; i++)
            {
                if (PositionInBounds(new BoardPosition(queen.Row, queen.Col + i)) == true)
                {
                    if (PositionIsEmpty(new BoardPosition(queen.Row, queen.Col + i)) == true || PositionIsEnemy(new BoardPosition(queen.Row, queen.Col + i), enemy) == true)
                    {
                        queenMoves.Add(new ChessMove(queen, new BoardPosition(queen.Row, queen.Col + i)));
                    }
                }
            }
            for (int i = 1; i < BOARD_SIZE; i++)
            {
                if (PositionInBounds(new BoardPosition(queen.Row - i, queen.Col - i)) == true)
                {
                    if (PositionIsEmpty(new BoardPosition(queen.Row - i, queen.Col - i)) == true || PositionIsEnemy(new BoardPosition(queen.Row - i, queen.Col - i), enemy) == true)
                    {
                        queenMoves.Add(new ChessMove(queen, new BoardPosition(queen.Row - i, queen.Col - i)));
                    }
                }
            }
            for (int i = 1; i < BOARD_SIZE; i++)
            {
                if (PositionInBounds(new BoardPosition(queen.Row - i, queen.Col + i)) == true)
                {
                    if (PositionIsEmpty(new BoardPosition(queen.Row - i, queen.Col + i)) == true || PositionIsEnemy(new BoardPosition(queen.Row - i, queen.Col + i), enemy) == true)
                    {
                        queenMoves.Add(new ChessMove(queen, new BoardPosition(queen.Row - i, queen.Col + i)));
                    }
                }
            }
            for (int i = 1; i < BOARD_SIZE; i++)
            {
                if (PositionInBounds(new BoardPosition(queen.Row + i, queen.Col - i)) == true)
                {
                    if (PositionIsEmpty(new BoardPosition(queen.Row + i, queen.Col - i)) == true || PositionIsEnemy(new BoardPosition(queen.Row + i, queen.Col - i), enemy) == true)
                    {
                        queenMoves.Add(new ChessMove(queen, new BoardPosition(queen.Row + i, queen.Col - i)));
                    }
                }
            }
            for (int i = 1; i < BOARD_SIZE; i++)
            {
                if (PositionInBounds(new BoardPosition(queen.Row + i, queen.Col + i)) == true)
                {
                    if (PositionIsEmpty(new BoardPosition(queen.Row + i, queen.Col + i)) == true || PositionIsEnemy(new BoardPosition(queen.Row + i, queen.Col + i), enemy) == true)
                    {
                        queenMoves.Add(new ChessMove(queen, new BoardPosition(queen.Row + i, queen.Col + i)));
                    }
                }
            }
            return queenMoves;
        }
        public IEnumerable<IGameMove> KnightPossibleMoves(BoardPosition knight) {
            List<ChessMove> knightMoves = new List<ChessMove>();
            var player = CurrentPlayer;
            var enemy = player * -1;

            BoardPosition upLeft = new BoardPosition(knight.Row - 2, knight.Col - 1);
            BoardPosition upRight = new BoardPosition(knight.Row - 2, knight.Col + 1);
            BoardPosition RightUp = new BoardPosition(knight.Row - 1, knight.Col + 2);
            BoardPosition RightDown = new BoardPosition(knight.Row + 1, knight.Col + 2);
            BoardPosition DownLeft = new BoardPosition(knight.Row + 2, knight.Col - 1);
            BoardPosition DownRight = new BoardPosition(knight.Row + 2, knight.Col + 1);
            BoardPosition LeftUp = new BoardPosition(knight.Row - 1, knight.Col - 2);
            BoardPosition LeftDown = new BoardPosition(knight.Row + 1, knight.Col - 2);

            if (PositionInBounds(upLeft) == true) {
                if (PositionIsEmpty(upLeft) == true || PositionIsEnemy(upLeft, enemy)==true){
                    knightMoves.Add(new ChessMove(knight, upLeft));
                }
            }
            if (PositionInBounds(upRight) == true)
            {
                if (PositionIsEmpty(upRight) == true || PositionIsEnemy(upRight, enemy) == true)
                {
                    knightMoves.Add(new ChessMove(knight, upRight));
                }
            }
            if (PositionInBounds(RightUp) == true)
            {
                if (PositionIsEmpty(RightUp) == true || PositionIsEnemy(RightUp, enemy) == true)
                {
                    knightMoves.Add(new ChessMove(knight, RightUp));
                }
            }
            if (PositionInBounds(RightDown) == true)
            {
                if (PositionIsEmpty(RightDown) == true || PositionIsEnemy(RightDown, enemy) == true)
                {
                    knightMoves.Add(new ChessMove(knight, RightDown));
                }
            }
            if (PositionInBounds(DownLeft) == true)
            {
                if (PositionIsEmpty(DownLeft) == true || PositionIsEnemy(DownLeft, enemy) == true)
                {
                    knightMoves.Add(new ChessMove(knight, DownLeft));
                }
            }
            if (PositionInBounds(DownRight) == true)
            {
                if (PositionIsEmpty(DownRight) == true || PositionIsEnemy(DownRight, enemy) == true)
                {
                    knightMoves.Add(new ChessMove(knight, DownRight));
                }
            }
            if (PositionInBounds(LeftUp) == true)
            {
                
                if (PositionIsEmpty(LeftUp) == true) {
                    knightMoves.Add(new ChessMove(knight, LeftUp));
                }
                if (PositionIsEnemy(LeftUp,enemy)==true) {
                    knightMoves.Add(new ChessMove(knight, LeftUp));
                }

            }
            if (PositionInBounds(LeftDown) == true)
            {
                if (PositionIsEmpty(LeftDown) == true || PositionIsEnemy(LeftDown, enemy) == true)
                {
                    knightMoves.Add(new ChessMove(knight, LeftDown));
                }
            }
            return knightMoves;
        }
        /*
        public IEnumerable<IGameMove> KnightPossibleMoves(BoardPosition knight)
        {
            List<ChessMove> knightMoves = new List<ChessMove>();
            var player = CurrentPlayer;
            var enemy = player * -1;
            //up then left
            if (PositionInBounds(new BoardPosition(knight.Row - 2, knight.Col - 1)) == true)
            {
                if (PositionIsEmpty(new BoardPosition(knight.Row - 2, knight.Col - 1)) == true || PositionIsEnemy(new BoardPosition(knight.Row - 2, knight.Col - 1), enemy) == true)
                {
                    knightMoves.Add(new ChessMove(knight, new BoardPosition(knight.Row - 2, knight.Col - 1)));
                }
            }
            //up then right
            if (PositionInBounds(new BoardPosition(knight.Row - 2, knight.Col + 1)) == true)
            {
                if (PositionIsEmpty(new BoardPosition(knight.Row - 2, knight.Col + 1)) == true || PositionIsEnemy(new BoardPosition(knight.Row - 2, knight.Col + 1), enemy) == true)
                {
                    knightMoves.Add(new ChessMove(knight, new BoardPosition(knight.Row - 2, knight.Col + 1)));
                }
            }
            //left then up
            if (PositionInBounds(new BoardPosition(knight.Row - 1, knight.Col - 2)) == true)
            {
                if (PositionIsEmpty(new BoardPosition(knight.Row - 1, knight.Col - 2)) == true || PositionIsEnemy(new BoardPosition(knight.Row - 1, knight.Col - 2), enemy) == true)
                {
                    knightMoves.Add(new ChessMove(knight, new BoardPosition(knight.Row - 1, knight.Col - 2)));
                }
            }
            //left then down
            if (PositionInBounds(new BoardPosition(knight.Row + 1, knight.Col - 2)) == true)
            {
                if (PositionIsEmpty(new BoardPosition(knight.Row + 1, knight.Col - 2)) == true || PositionIsEnemy(new BoardPosition(knight.Row + 1, knight.Col - 2), enemy) == true)
                {
                    knightMoves.Add(new ChessMove(knight, new BoardPosition(knight.Row + 1, knight.Col - 2)));
                }
            }
            //right then up
            if (PositionInBounds(new BoardPosition(knight.Row - 1, knight.Col + 2)) == true)
            {
                if (PositionIsEmpty(new BoardPosition(knight.Row - 1, knight.Col + 2)) == true || PositionIsEnemy(new BoardPosition(knight.Row - 1, knight.Col + 2), enemy) == true)
                {
                    knightMoves.Add(new ChessMove(knight, new BoardPosition(knight.Row - 1, knight.Col + 2)));
                }
            }
            //right then down
            if (PositionInBounds(new BoardPosition(knight.Row + 1, knight.Col + 2)) == true)
            {
                if (PositionIsEmpty(new BoardPosition(knight.Row + 1, knight.Col + 2)) == true || PositionIsEnemy(new BoardPosition(knight.Row + 1, knight.Col + 2), enemy) == true)
                {
                    knightMoves.Add(new ChessMove(knight, new BoardPosition(knight.Row + 1, knight.Col + 2)));
                }
            }
            //down left
            if (PositionInBounds(new BoardPosition(knight.Row + 2, knight.Col - 1)) == true)
            {
                if (PositionIsEmpty(new BoardPosition(knight.Row + 2, knight.Col - 1)) == true || PositionIsEnemy(new BoardPosition(knight.Row + 2, knight.Col - 1), enemy) == true)
                {
                    knightMoves.Add(new ChessMove(knight, new BoardPosition(knight.Row + 2, knight.Col - 1)));
                }
            }
            //down right
            if (PositionInBounds(new BoardPosition(knight.Row + 2, knight.Col + 1)) == true)
            {
                if (PositionIsEmpty(new BoardPosition(knight.Row + 2, knight.Col + 1)) == true || PositionIsEnemy(new BoardPosition(knight.Row + 2, knight.Col + 1), enemy) == true)
                {
                    knightMoves.Add(new ChessMove(knight, new BoardPosition(knight.Row + 2, knight.Col + 1)));
                }
            }

            return knightMoves;
        }
        */
        /// <summary>
        /// Gets a sequence of all positions on the board that are threatened by the given player. A king
        /// may not move to a square threatened by the opponent.
        /// </summary>
        public IEnumerable<BoardPosition> GetThreatenedPositions(int byPlayer)
        {
            // TODO: implement this method. Make sure to account for "special" moves.
            List<BoardPosition> tPositions = new List<BoardPosition>();

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; i++)
                {

                }

            }
            return tPositions;
        }
        public IEnumerable<BoardPosition> PawnThreats(BoardPosition pawn)
        {
            List<BoardPosition> pPositions = new List<BoardPosition>();
            var position = mBoard[pawn.Row, pawn.Col];
            BoardPosition whiteLeftPosition = new BoardPosition(pawn.Row - 1, pawn.Col - 1);
            BoardPosition whiteRightPosition = new BoardPosition(pawn.Row - 1, pawn.Col + 1);
            BoardPosition blackLeftPosition = new BoardPosition(pawn.Row + 1, pawn.Col + 1);
            BoardPosition blackRightPosition = new BoardPosition(pawn.Row + 1, pawn.Col + 1);
            if (PositionInBounds(pawn) == true)
            {
                if (position > 0)
                {
                    if (PositionInBounds(whiteLeftPosition) == true)
                    {
                        pPositions.Add(whiteLeftPosition);
                    }
                    if (PositionInBounds(whiteRightPosition) == true)
                    {
                        pPositions.Add(whiteRightPosition);
                    }
                }
                else
                {
                    if (PositionInBounds(blackLeftPosition) == true)
                    {
                        pPositions.Add(blackLeftPosition);
                    }
                    if (PositionInBounds(blackRightPosition) == true)
                    {
                        pPositions.Add(blackRightPosition);
                    }
                }
            }
            return pPositions;
        }
        public IEnumerable<BoardPosition> RookThreats(BoardPosition rook)
        {
            List<BoardPosition> rPositions = new List<BoardPosition>();
            if (PositionInBounds(rook) == true)
            {
                for (int i = 1; i < BOARD_SIZE; i++)
                {
                    if (PositionInBounds(new BoardPosition(rook.Row - i, rook.Col)) == true)
                    {
                        rPositions.Add(new BoardPosition(rook.Row - i, rook.Col));
                    }
                }
                for (int i = 1; i < BOARD_SIZE; i++)
                {
                    if (PositionInBounds(new BoardPosition(rook.Row + i, rook.Col)) == true)
                    {
                        rPositions.Add(new BoardPosition(rook.Row + i, rook.Col));
                    }
                }
                for (int i = 1; i < BOARD_SIZE; i++)
                {
                    if (PositionInBounds(new BoardPosition(rook.Row, rook.Col - i)) == true)
                    {
                        rPositions.Add(new BoardPosition(rook.Row, rook.Col - i));
                    }
                }
                for (int i = 1; i < BOARD_SIZE; i++)
                {
                    if (PositionInBounds(new BoardPosition(rook.Row, rook.Col + i)) == true)
                    {
                        rPositions.Add(new BoardPosition(rook.Row, rook.Col + i));
                    }
                }
            }
            return rPositions;
        }

        public IEnumerable<BoardPosition> BishopThreats(BoardPosition bishop)
        {
            List<BoardPosition> bPositions = new List<BoardPosition>();
            var position = mBoard[bishop.Row, bishop.Col];

            if (PositionInBounds(bishop) == true)
            {
                for (int i = 1; i < BOARD_SIZE; i++)
                {
                    if (PositionInBounds(new BoardPosition(bishop.Row - i, bishop.Col - i)) == true)
                    {
                        bPositions.Add(new BoardPosition(bishop.Row - i, bishop.Col - i));
                    }
                }
                for (int i = 1; i < BOARD_SIZE; i++)
                {
                    if (PositionInBounds(new BoardPosition(bishop.Row - i, bishop.Col + i)) == true)
                    {
                        bPositions.Add(new BoardPosition(bishop.Row - i, bishop.Col + i));
                    }
                }
                for (int i = 1; i < BOARD_SIZE; i++)
                {
                    if (PositionInBounds(new BoardPosition(bishop.Row + i, bishop.Col - i)) == true)
                    {
                        bPositions.Add(new BoardPosition(bishop.Row + i, bishop.Col - i));
                    }
                }
                for (int i = 1; i < BOARD_SIZE; i++)
                {
                    if (PositionInBounds(new BoardPosition(bishop.Row + i, bishop.Col + i)) == true)
                    {
                        bPositions.Add(new BoardPosition(bishop.Row + i, bishop.Col + i));
                    }
                }

            }

            return bPositions;
        }
        //remember to later add to check to see if king hasn't been moved with a global varible to check castling move
        public IEnumerable<BoardPosition> KingThreats(BoardPosition king)
        {
            List<BoardPosition> kPositions = new List<BoardPosition>();
            BoardPosition KingLeft = new BoardPosition(king.Row, king.Col - 1);
            BoardPosition KingRight = new BoardPosition(king.Row, king.Col + 1);
            BoardPosition KingUp = new BoardPosition(king.Row - 1, king.Col);
            BoardPosition KingDown = new BoardPosition(king.Row + 1, king.Col);
            BoardPosition KingDUL = new BoardPosition(king.Row - 1, king.Col - 1);
            BoardPosition KingDUR = new BoardPosition(king.Row - 1, king.Col + 1);
            BoardPosition KingDDL = new BoardPosition(king.Row + 1, king.Col - 1);
            BoardPosition KingDDR = new BoardPosition(king.Row + 1, king.Col + 1);

            if (PositionInBounds(king) == true)
            {
                if (PositionInBounds(KingLeft) == true)
                {
                    kPositions.Add(KingLeft);
                }
                if (PositionInBounds(KingRight) == true)
                {
                    kPositions.Add(KingRight);
                }
                if (PositionInBounds(KingUp) == true)
                {
                    kPositions.Add(KingUp);
                }
                if (PositionInBounds(KingDown) == true)
                {
                    kPositions.Add(KingDown);
                }
                if (PositionInBounds(KingDUL) == true)
                {
                    kPositions.Add(KingDUL);
                }
                if (PositionInBounds(KingDUR) == true)
                {
                    kPositions.Add(KingDUR);
                }
                if (PositionInBounds(KingDDL) == true)
                {
                    kPositions.Add(KingDDL);
                }
                if (PositionInBounds(KingDDR) == true)
                {
                    kPositions.Add(KingDDR);
                }
            }
            return kPositions;
        }

        public IEnumerable<BoardPosition> QueenThreats(BoardPosition queen)
        {
            List<BoardPosition> qPositions = new List<BoardPosition>();
            if (PositionInBounds(queen) == true)
            {
                for (int i = 1; i < BOARD_SIZE; i++)
                {
                    if (PositionInBounds(new BoardPosition(queen.Row - i, queen.Col)))
                    {
                        qPositions.Add(new BoardPosition(queen.Row - i, queen.Col));
                    }
                }
                for (int i = 1; i < BOARD_SIZE; i++)
                {
                    if (PositionInBounds(new BoardPosition(queen.Row + i, queen.Col)))
                    {
                        qPositions.Add(new BoardPosition(queen.Row + i, queen.Col));
                    }
                }
                for (int i = 1; i < BOARD_SIZE; i++)
                {
                    if (PositionInBounds(new BoardPosition(queen.Row, queen.Col - i)))
                    {
                        qPositions.Add(new BoardPosition(queen.Row, queen.Col - i));
                    }
                }
                for (int i = 1; i < BOARD_SIZE; i++)
                {
                    if (PositionInBounds(new BoardPosition(queen.Row, queen.Col + i)))
                    {
                        qPositions.Add(new BoardPosition(queen.Row, queen.Col + i));
                    }
                }
                for (int i = 1; i < BOARD_SIZE; i++)
                {
                    if (PositionInBounds(new BoardPosition(queen.Row - i, queen.Col - i)))
                    {
                        qPositions.Add(new BoardPosition(queen.Row - i, queen.Col - i));
                    }
                }
                for (int i = 1; i < BOARD_SIZE; i++)
                {
                    if (PositionInBounds(new BoardPosition(queen.Row - i, queen.Col + i)))
                    {
                        qPositions.Add(new BoardPosition(queen.Row - i, queen.Col + i));
                    }
                }
                for (int i = 1; i < BOARD_SIZE; i++)
                {
                    if (PositionInBounds(new BoardPosition(queen.Row + i, queen.Col - i)))
                    {
                        qPositions.Add(new BoardPosition(queen.Row + i, queen.Col - i));
                    }
                }
                for (int i = 1; i < BOARD_SIZE; i++)
                {
                    if (PositionInBounds(new BoardPosition(queen.Row + i, queen.Col + i)))
                    {
                        qPositions.Add(new BoardPosition(queen.Row + i, queen.Col + i));
                    }
                }
            }
            return qPositions;
        }
        public IEnumerable<BoardPosition> KnightThreats(BoardPosition knight)
        {
            List<BoardPosition> kPositions = new List<BoardPosition>();

            if (PositionInBounds(knight) == true)
            {
                if (PositionInBounds(new BoardPosition(knight.Row - 2, knight.Col - 1)))
                {
                    kPositions.Add(new BoardPosition(knight.Row - 2, knight.Col - 1));
                }
                if (PositionInBounds(new BoardPosition(knight.Row - 2, knight.Col + 1)))
                {
                    kPositions.Add(new BoardPosition(knight.Row - 2, knight.Col + 1));
                }
                if (PositionInBounds(new BoardPosition(knight.Row - 1, knight.Col - 2)))
                {
                    kPositions.Add(new BoardPosition(knight.Row - 1, knight.Col - 2));
                }
                if (PositionInBounds(new BoardPosition(knight.Row + 1, knight.Col - 2)))
                {
                    kPositions.Add(new BoardPosition(knight.Row + 1, knight.Col - 2));
                }
                if (PositionInBounds(new BoardPosition(knight.Row - 1, knight.Col + 2)))
                {
                    kPositions.Add(new BoardPosition(knight.Row - 1, knight.Col + 2));
                }
                if (PositionInBounds(new BoardPosition(knight.Row + 1, knight.Col + 2)))
                {
                    kPositions.Add(new BoardPosition(knight.Row + 1, knight.Col + 2));
                }
                if (PositionInBounds(new BoardPosition(knight.Row + 1, knight.Col - 1)))
                {
                    kPositions.Add(new BoardPosition(knight.Row + 1, knight.Col - 1));
                }
                if (PositionInBounds(new BoardPosition(knight.Row + 2, knight.Col + 1)))
                {
                    kPositions.Add(new BoardPosition(knight.Row + 2, knight.Col + 1));
                }
            }
            return kPositions;
        }

        public void UndoLastMove()
        {
            // TODO: implement this method. Make sure to account for "special" moves.
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns true if the given position on the board is empty.
        /// </summary>
        /// <remarks>returns false if the position is not in bounds</remarks>
        public bool PositionIsEmpty(BoardPosition pos)
        {
            var boardValue = mBoard[pos.Row, pos.Col];
            if (boardValue == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the given position contains a piece that is the enemy of the given player.
        /// </summary>
        /// <remarks>returns false if the position is not in bounds</remarks>
        private bool PositionIsEnemy(BoardPosition pos, int player)
        {
            return mBoard[pos.Row, pos.Col] == -player;
        }

        /// <summary>
        /// Returns true if the given position is in the bounds of the board.
        /// </summary>
        private static bool PositionInBounds(BoardPosition pos)
        {
            return pos.Row >= 0 && pos.Row < BOARD_SIZE && pos.Col >= 0 && pos.Col < BOARD_SIZE;
        }

        /// <summary>
        /// Returns which player has a piece at the given board position, or 0 if it is empty.
        /// </summary>
        public int GetPlayerAtPosition(BoardPosition pos)
        {
            var position = mBoard[pos.Row, pos.Col];
            if (position > 0)
            {
                return 1;
            }
            else if (position < 0)
            {
                return 2;
            }
            return 0;
        }

        /// <summary>
        /// Gets the value weight for a piece of the given type.
        /// </summary>
        /*
		 * VALUES:
		 * Pawn: 1
		 * Knight: 3
		 * Bishop: 3
		 * Rook: 5
		 * Queen: 9
		 * King: infinity (maximum integer value)
		 */
        public int GetPieceValue(ChessPieceType pieceType)
        {
            if (pieceType == ChessPieceType.Pawn)
            {
                return 1;
            }
            else if (pieceType == ChessPieceType.Bishop || pieceType == ChessPieceType.Knight)
            {
                return 3;
            }
            else if (pieceType == ChessPieceType.RookKing || pieceType == ChessPieceType.RookQueen || pieceType == ChessPieceType.RookPawn)
            {
                return 5;
            }
            else if (pieceType == ChessPieceType.Queen)
            {
                return 9;
            }
            return 0;

        }


        /// <summary>
        /// Manually places the given piece at the given position.
        /// </summary>
        // This is used in the constructor
        private void SetPosition(BoardPosition position, ChessPiecePosition piece)
        {
            mBoard[position.Row, position.Col] = (sbyte)((int)piece.PieceType * (piece.Player == 2 ? -1 :
                piece.Player));
        }
    }
}
