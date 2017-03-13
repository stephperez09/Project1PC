﻿using System.Collections.Generic;

namespace Cecs475.BoardGames.Othello {
	/// <summary>
	/// Implements the board model for a game of othello. Tracks which squares of the 8x8 grid are occupied
	/// by which player, as well as state for the current player and move history.
	/// </summary>
	public class OthelloBoard : IGameBoard {
		// "internal" allows other classes in this assembly to access the member.
		internal const int BOARD_SIZE = 8;

		// The board is represented by an 8x8 matrix of signed bytes. Each entry represents one square on the board.
		private sbyte[,] mBoard = new sbyte[BOARD_SIZE, BOARD_SIZE];

		// A property to access the Board, but only within this assembly.
		internal sbyte[,] Board { get { return mBoard; } }

		/// <summary>
		/// Constructs an othello board in the starting game state.
		/// </summary>
		public OthelloBoard() {
			CurrentPlayer = 1;
			MoveHistory = new List<IGameMove>();
			mBoard[BOARD_SIZE / 2 - 1, BOARD_SIZE / 2 - 1] = -1;
			mBoard[BOARD_SIZE / 2, BOARD_SIZE / 2] = -1;
			mBoard[BOARD_SIZE / 2, BOARD_SIZE / 2 - 1] = 1;
			mBoard[BOARD_SIZE / 2 - 1, BOARD_SIZE / 2] = 1;
		}

		/// <summary>
		/// An integer to represent the current player, where 1 == Black, -1 == White.
		/// </summary>
		public int CurrentPlayer {
			get; private set;
		}

		/// <summary>
		/// How many "pass" moves have been applied in a row.
		/// </summary>
		public int PassCount {
			get; private set;
		}

		/// <summary>
		/// Gets the current value of the board, as the difference between the number of Black pieces and White pieces.
		/// </summary>
		public int Value {
			get; private set;
		}

		/// <summary>
		/// Gets a list 
		/// </summary>
		public IList<IGameMove> MoveHistory {
			get; private set;
		}

		/// <summary>
		/// Applies the given move to the board state.
		/// </summary>
		/// <param name="m">a move that is assumed to be valid</param>
		public void ApplyMove(IGameMove move) {
			OthelloMove m = move as OthelloMove;

			// If the move is a pass, then we do very little.
			if (m.IsPass) {
				PassCount++;
			}
			else {
				PassCount = 0;
				// Otherwise update the board at the move's position with the current player.
				mBoard[m.Position.Row, m.Position.Col] = (sbyte)CurrentPlayer;
				Value += CurrentPlayer;

				// Iterate through all 8 directions radially from the move's position.
				for (int rDelta = -1; rDelta <= 1; rDelta++) {
					for (int cDelta = -1; cDelta <= 1; cDelta++) {
						if (rDelta == 0 && cDelta == 0)
							continue;

						// Repeatedly move in the selected direction, as long as we find "enemy" squares.
						BoardPosition newPos = m.Position;
						int steps = 0;
						do {
							newPos = newPos.Translate(rDelta, cDelta);
							steps++;
						} while (PositionInBounds(newPos) && PositionIsEnemy(newPos, CurrentPlayer));

						// This is a valid direction of flips if we moved at least 2 squares, and ended in bounds and on a
						// "friendly" square.
						if (steps > 1 && PositionInBounds(newPos) && mBoard[newPos.Row, newPos.Col] == CurrentPlayer) {
							// Record this direction in the move's flipsets so the move can be undone.
							m.FlipSets.Add(
								new OthelloMove.FlipSet() { RowDelta = rDelta, ColDetla = cDelta, Count = steps - 1 });

							// Repeatedly walk back the way we came, updating the board with the current player's piece.
							newPos = newPos.Translate(-rDelta, -cDelta);
							while (steps > 1) {
								mBoard[newPos.Row, newPos.Col] = (sbyte)CurrentPlayer;
								Value += 2 * CurrentPlayer;

								newPos = newPos.Translate(-rDelta, -cDelta);
								steps--;
							}
						}
					}
				}
			}
			// Update the rest of the board state.
			CurrentPlayer = -CurrentPlayer;
			MoveHistory.Add(m);
		}

		/// <summary>
		/// Returns true if the given position is in bounds of the board.
		/// </summary>
		private static bool PositionInBounds(BoardPosition pos) {
			return pos.Row >= 0 && pos.Row < BOARD_SIZE && pos.Col >= 0 && pos.Col < BOARD_SIZE;
		}

		/// <summary>
		/// Returns true if the given in-bounds position is an enemy of the given player.
		/// </summary>
		/// <param name="pos">assumed to be in bounds</param>
		private bool PositionIsEnemy(BoardPosition pos, int player) {
			return mBoard[pos.Row, pos.Col] == -player;
		}

		/// <summary>
		/// Returns an enumeration of moves that would be valid to apply to the current board state.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IGameMove> GetPossibleMoves() {
			List<OthelloMove> moves = new List<OthelloMove>();

			// Iterate through all 64 squares on the board, looking for an empty position.
			for (int row = 0; row < BOARD_SIZE; row++) {
				for (int col = 0; col < BOARD_SIZE; col++) {
					if (mBoard[row, col] != 0)
						continue;

					bool validSquare = false;

					// Iterate through all 8 directions radially from the current position.
					for (int rDelta = -1; rDelta <= 1 && !validSquare; rDelta++) {
						for (int cDelta = -1; cDelta <= 1 && !validSquare; cDelta++) {
							if (rDelta == 0 && cDelta == 0)
								continue;

							// Repeatedly move in the selected direction, as long as we find "enemy" squares.
							BoardPosition newPos = new BoardPosition(row, col);
							int steps = 0;
							do {
								newPos = newPos.Translate(rDelta, cDelta);
								steps++;
							} while (PositionInBounds(newPos) && PositionIsEnemy(newPos, CurrentPlayer));

							// This is a valid direction of flips if we moved at least 2 squares, and ended in bounds and on a
							// "friendly" square.
							if (steps > 1 && PositionInBounds(newPos) && mBoard[newPos.Row, newPos.Col] == CurrentPlayer) {
								validSquare = true;
							}
						}
					}
					// If the current position is valid, yield a move at the position.
					if (validSquare) {
						moves.Add(new OthelloMove(new BoardPosition(row, col)));
					}
				}
			}
			// If no positions were valid, yield a "pass" move.
			if (moves.Count == 0) {
				moves.Add(new OthelloMove(new BoardPosition(-1, -1)));
			}

			return moves;
		}


		/// <summary>
		/// Undoes the last move, restoring the game to its state before the move was applied.
		/// </summary>
		public void UndoLastMove() {
			OthelloMove m = MoveHistory[MoveHistory.Count - 1] as OthelloMove;

			if (!m.IsPass) {
				// Reset the board at the move's position.
				mBoard[m.Position.Row, m.Position.Col] = 0;
				Value += CurrentPlayer;

				// Iterate through the move's recorded flipsets.
				foreach (var flipSet in m.FlipSets) {
					BoardPosition pos = m.Position;
					// For each flipset, walk along the flipset's direction resetting pieces.
					for (int i = 1; i <= flipSet.Count; i++) {
						pos = pos.Translate(flipSet.RowDelta, flipSet.ColDetla);
						mBoard[pos.Row, pos.Col] = (sbyte)CurrentPlayer;
						Value += 2 * CurrentPlayer;
					}
				}

				// Check to see if the second-to-last move was a pass; if so, set PassCount.
				if (MoveHistory.Count > 1 && ((OthelloMove)MoveHistory[MoveHistory.Count - 2]).IsPass) {
					PassCount = 1;
				}
			}
			else {
				PassCount--;
			}
			// Reset the remaining game state.
			CurrentPlayer = -CurrentPlayer;
			MoveHistory.RemoveAt(MoveHistory.Count - 1);
		}
	}
}
