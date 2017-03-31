using System;
using System.Collections;

namespace ChessSharp
{
	public class ChessLogic
	{
		public State curState;

		public ChessLogic()
		{
			curState = new State();

			curState.entries.Add(new StateEntry(Color.White, ChessType.Rook, 1, 1));
			curState.entries.Add(new StateEntry(Color.White, ChessType.Knight, 2, 1));
			curState.entries.Add(new StateEntry(Color.White, ChessType.Bishop, 3, 1));
			curState.entries.Add(new StateEntry(Color.White, ChessType.Queen, 4, 1));
			curState.entries.Add(new StateEntry(Color.White, ChessType.King, 5, 1));
			curState.entries.Add(new StateEntry(Color.White, ChessType.Bishop, 6, 1));
			curState.entries.Add(new StateEntry(Color.White, ChessType.Knight, 7, 1));
			curState.entries.Add(new StateEntry(Color.White, ChessType.Rook, 8, 1));
			curState.entries.Add(new StateEntry(Color.White, ChessType.Pawn, 1, 2));
			curState.entries.Add(new StateEntry(Color.White, ChessType.Pawn, 2, 2));
			curState.entries.Add(new StateEntry(Color.White, ChessType.Pawn, 3, 2));
			curState.entries.Add(new StateEntry(Color.White, ChessType.Pawn, 4, 2));
			curState.entries.Add(new StateEntry(Color.White, ChessType.Pawn, 5, 2));
			curState.entries.Add(new StateEntry(Color.White, ChessType.Pawn, 6, 2));
			curState.entries.Add(new StateEntry(Color.White, ChessType.Pawn, 7, 2));
			curState.entries.Add(new StateEntry(Color.White, ChessType.Pawn, 8, 2));

			curState.entries.Add(new StateEntry(Color.Black, ChessType.Rook, 1, 8));
			curState.entries.Add(new StateEntry(Color.Black, ChessType.Knight, 2, 8));
			curState.entries.Add(new StateEntry(Color.Black, ChessType.Bishop, 3, 8));
			curState.entries.Add(new StateEntry(Color.Black, ChessType.Queen, 4, 8));
			curState.entries.Add(new StateEntry(Color.Black, ChessType.King, 5, 8));
			curState.entries.Add(new StateEntry(Color.Black, ChessType.Bishop, 6, 8));
			curState.entries.Add(new StateEntry(Color.Black, ChessType.Knight, 7, 8));
			curState.entries.Add(new StateEntry(Color.Black, ChessType.Rook, 8, 8));
			curState.entries.Add(new StateEntry(Color.Black, ChessType.Pawn, 1, 7));
			curState.entries.Add(new StateEntry(Color.Black, ChessType.Pawn, 2, 7));
			curState.entries.Add(new StateEntry(Color.Black, ChessType.Pawn, 3, 7));
			curState.entries.Add(new StateEntry(Color.Black, ChessType.Pawn, 4, 7));
			curState.entries.Add(new StateEntry(Color.Black, ChessType.Pawn, 5, 7));
			curState.entries.Add(new StateEntry(Color.Black, ChessType.Pawn, 6, 7));
			curState.entries.Add(new StateEntry(Color.Black, ChessType.Pawn, 7, 7));
			curState.entries.Add(new StateEntry(Color.Black, ChessType.Pawn, 8, 7));
		}

		public void MakeMove(Move move)
		{
			curState.MakeMove(move);
		}

		public Move GenMove()
		{
			return AlphaBetaGenMove();
		}

		#region Min-Max search
		public Move MinMaxGenMove()
		{
			int best = 0;
			if(curState.turn == ChessLogic.Color.White)
				best = int.MinValue;
			else
				best = int.MaxValue;
			Move bestMove = new Move();

			State oldState = curState.Clone();
			foreach(StateEntry entry in oldState.entries)
			{				
				if(entry.color == oldState.turn)
				{
					Move move;
					move.from = entry;
					move.to = entry;
					for(int i = 1 ; i <= 8 ; i++)
						for(int j = 1 ; j <= 8 ; j++)
						{
							move.to.x = j;
							move.to.y = i;

							if(curState.LegalMove(move))
							{
								curState.MakeMove(move);
								int val;
								if(entry.color == Color.White)
									val = MinMax(3);
								else
									val = MinMax(3);
								curState = oldState.Clone();
								if((val > best && entry.color == Color.White) || (val < best && entry.color == Color.Black))
								{
									best = val;
									bestMove = move;
								}
							}
						}
				}
			}

			return bestMove;
		}

		int MinMax(int depth)
		{
			if(curState.turn == Color.White)
				return Max(depth);
			else
				return Min(depth);
		}

		int Max(int depth)
		{
			int best = int.MinValue;

			if(depth <= 0)
				return curState.Estimate();

			State oldState = curState.Clone();
			foreach(StateEntry entry in oldState.entries)
			{				
				if(entry.color == Color.White)
				{
					Move move;
					move.from = entry;
					move.to = entry;
					for(int i = 1 ; i <= 8 ; i++)
						for(int j = 1 ; j <= 8 ; j++)
						{
							move.to.x = j;
							move.to.y = i;

							if(curState.LegalMove(move))
							{
								curState.MakeMove(move);
								int val = Min(depth - 1);
								curState = oldState.Clone();
								if(val > best)
									best = val;
							}
						}
				}
			}

			return best;
		}

		int Min(int depth)
		{
			int best = int.MaxValue;

			if(depth <= 0)
				return curState.Estimate();

			State oldState = curState.Clone();
			foreach(StateEntry entry in oldState.entries)
			{				
				if(entry.color == Color.Black)
				{
					Move move;
					move.from = entry;
					move.to = entry;
					for(int i = 1 ; i <= 8 ; i++)
						for(int j = 1 ; j <= 8 ; j++)
						{
							move.to.x = j;
							move.to.y = i;

							if(curState.LegalMove(move))
							{
								curState.MakeMove(move);
								int val = Max(depth - 1);
								curState = oldState.Clone();
								if(val < best)
									best = val;
							}
						}
				}
			}

			return best;
		}
		#endregion

		#region Alpha-Beta search
		public Move AlphaBetaGenMove()
		{
			Random rand = new Random();

			int best = int.MinValue;
			ArrayList bestMoves = new ArrayList();

			ArrayList possibleMoves = curState.GeneratePossibleMoves();
			ArrayList legalMoves = new ArrayList();
			foreach(Move move in possibleMoves)
				if(curState.FastLegalMove(move))
					legalMoves.Add(move);
			if(legalMoves.Count == 0)
				throw new Exception("No moves left!");

			State oldState = curState.Clone();
			foreach(Move move in legalMoves)
			{
				curState.MakeMove(move);
				int val;
				val = -AlphaBeta(3, -20000, 20000);
				curState = oldState.Clone();
				if(val > best)
				{
					bestMoves.Clear();
					bestMoves.Add(move);
					best = val;
				}
				else if(val == best)
					bestMoves.Add(move);
			}
			return (Move)bestMoves[rand.Next(bestMoves.Count - 1)];
		}
		int AlphaBeta(int depth, int alpha, int beta)
		{
			if(depth <= 0)
			{
				int val = curState.Estimate();
				if(curState.turn == Color.White)
					return val;
				else
					return -val;
			}
			ArrayList possibleMoves = curState.GeneratePossibleMoves();
			ArrayList legalMoves = new ArrayList();
			foreach(Move move in possibleMoves)
				if(curState.FastLegalMove(move))
					legalMoves.Add(move);
			if(legalMoves.Count == 0)
			{
				int val = curState.Estimate();
				if(curState.turn == Color.White)
					return val;
				else
					return -val;
			}

			State oldState = curState.Clone();
			foreach(Move move in legalMoves)
			{
				curState.MakeMove(move);
				int val = -AlphaBeta(depth - 1, -beta, -alpha);
				curState = oldState.Clone();
				if(val >= beta)
					return beta;
				if(val > alpha)
					alpha = val;
			}
			return alpha;
		}
		/*int AlphaBeta(int depth, int alpha, int beta)
		{
			if(depth <= 0)
			{
				int val = curState.Estimate();
				if(curState.turn == Color.White)
					return val;
				else
					return -val;
			}

			State oldState = curState.Clone();
			foreach(StateEntry entry in oldState.entries)
			{				
				if(entry.color == oldState.turn)
				{
					Move move;
					move.from = entry;
					move.to = entry;
					for(int i = 1 ; i <= 8 ; i++)
						for(int j = 1 ; j <= 8 ; j++)
						{
							move.to.x = j;
							move.to.y = i;

							if(curState.LegalMove(move))
							{
								curState.MakeMove(move);
								int val = -AlphaBeta(depth - 1, -beta, -alpha);
								curState = oldState.Clone();
								if(val >= beta)
									return beta;
								if(val > alpha)
									alpha = val;
							}
						}
				}
			}

			return alpha;
		}*/
		#endregion

		#region Nega-Max search
		public Move NegaMaxGenMove()
		{
			int best = int.MinValue;
			Move bestMove = new Move();

			State oldState = curState.Clone();
			foreach(StateEntry entry in oldState.entries)
			{				
				if(entry.color == oldState.turn)
				{
					Move move;
					move.from = entry;
					move.to = entry;
					for(int i = 1 ; i <= 8 ; i++)
						for(int j = 1 ; j <= 8 ; j++)
						{
							move.to.x = j;
							move.to.y = i;

							if(curState.LegalMove(move))
							{
								curState.MakeMove(move);
								int val = -NegaMax(3);
								curState = oldState.Clone();
								if(val > best)
								{
									best = val;
									bestMove = move;
								}
							}
						}
				}
			}

			return bestMove;
		}

		int NegaMax(int depth)
		{
			int best = int.MinValue;

			if(depth <= 0)
			{
				int val = curState.Estimate();
				if(curState.turn == Color.White)
					return val;
				else
					return -val;
			}

			State oldState = curState.Clone();
			foreach(StateEntry entry in oldState.entries)
			{				
				if(entry.color == oldState.turn)
				{
					Move move;
					move.from = entry;
					move.to = entry;
					for(int i = 1 ; i <= 8 ; i++)
						for(int j = 1 ; j <= 8 ; j++)
						{
							move.to.x = j;
							move.to.y = i;

							if(curState.LegalMove(move))
							{
								curState.MakeMove(move);
								int val = -NegaMax(depth - 1);
								curState = oldState.Clone();
								if(val > best)
									best = val;
							}
						}
				}
			}

			return best;
		}
		#endregion

		
		public class StateEntryList
		{
			ArrayList list;

			public StateEntryList()
			{
				list = new ArrayList();
			}

			public void Add(StateEntry entry)
			{
				list.Add(entry);
			}

			public void Remove(StateEntry entry)
			{
				list.Remove(entry);
			}

			public StateEntry this[int i]
			{
				get
				{
					return (StateEntry)list[i];
				}
				set
				{
					list[i] = value;
				}
			}

			public StateEntry this[int x, int y]
			{
				get
				{
					foreach(StateEntry entry in list)
						if(entry.x == x && entry.y == y)
							return entry;

					StateEntry nothing;
					nothing.x = x;
					nothing.y = y;
					nothing.type = (ChessType)(-1);
					nothing.color = (Color)(-1);

					return nothing;
				}
				set
				{
					for(int i = 0 ; i < list.Count ; i++)
					{
						StateEntry entry = (StateEntry)list[i];
						if(entry.x == x && entry.y == y)
						{
							list[i] = value;
						}
					}
				}
			}

			public int Count
			{
				get
				{
					return list.Count;
				}
			}

			public IEnumerator GetEnumerator()
			{
				return list.GetEnumerator();
			}
		}

		public enum Color : int
		{
			White,
			Black
		}
		public enum ChessType : int
		{
			Pawn,
			Knight,
			Bishop,
			Rook,
			Queen,
			King
		}
		
		public struct StateEntry
		{
			public Color color;
			public ChessType type;
			public int x;
			public int y;

			public StateEntry(Color color, ChessType type, int x, int y)
			{
				this.color = color;
				this.type = type;
				this.x = x;
				this.y = y;
			}
		}

		public struct Move
		{
			public StateEntry from;
			public StateEntry to;
		}
        
		public class State
		{
			public StateEntryList entries;
			bool check;
			public Color turn;

			public State()
			{
				entries = new StateEntryList();
				turn = Color.White;
			}
			
			public State Clone()
			{
				State result = new State();
				result.turn = turn;
				foreach(StateEntry entry in entries)
				{
					result.entries.Add(entry);
				}

				return result;
			}

			public void MakeMove(Move move)
			{
				StateEntry entry = entries[move.to.x, move.to.y];
				if(entry.color >= 0)
					entries.Remove(entry);
				entries[move.from.x, move.from.y] = move.to;
				turn = (Color)(1 - (int)turn);
				check = IsCheck();
			}
            
			public ArrayList GeneratePossibleMoves()
			{
				ArrayList moves = new ArrayList();
				Move move;
				int start, finish, step; //for pawns only
				if(turn == Color.White)
				{
					start = 2;
					finish = 8;
					step = 1;
				}
				else
				{
					start = 7;
					finish = 1;
					step = -1;
				}
				foreach(StateEntry entry in entries)
				{
					if(entry.color == turn)
					{
						move.to = move.from = entry;
						if(entry.type == ChessType.Pawn)
						{
							if(move.from.y == start)
							{
								move.to.y = start + 2 * step;
								moves.Add(move);
							}
							move.to.y = move.from.y + step;
							if(move.to.y == finish)
								move.to.type = ChessType.Queen;
							moves.Add(move);
							move.to.x -= 1;
						moves.Add(move);
						move.to.x += 2;
						moves.Add(move);
					}
					else if(entry.type == ChessType.King)
					{
						for(int dx = -1 ; dx < 2 ; dx++)
						{
							if(move.from.x + dx > 0 && move.from.x + dx < 9)
							{
								move.to.x = move.from.x + dx;
								for(int dy = -1 ; dy < 2 ; dy++)
								{
									if(move.from.y + dy > 0 && move.from.y + dy < 9)
									{
										move.to.y = move.from.y + dy;
										moves.Add(move);
									}
								}
							}
						}
					}
					else if(entry.type == ChessType.Knight)
					{
						for(int dx = -2 ; dx < 3 ; dx += 4)
						{
							if(move.from.x + dx > 0 && move.from.x + dx < 9)
							{
								move.to.x = move.from.x + dx;
								for(int dy = -1 ; dy < 2 ; dy += 2)
								{
									if(move.from.y + dy > 0 && move.from.y + dy < 9)
									{
										move.to.y = move.from.y + dy;
										moves.Add(move);
									}
								}
							}
						}
						for(int dy = -2 ; dy < 3 ; dy += 4)
						{
							if(move.from.y + dy > 0 && move.from.y + dy < 9)
							{
								move.to.y = move.from.y + dy;
								for(int dx = -1 ; dx < 2 ; dx += 2)
								{
									if(move.from.x + dx > 0 && move.from.x + dx < 9)
									{
										move.to.x = move.from.x + dx;
										moves.Add(move);
									}
								}
							}
						}
					}
					else
					{
						if(entry.type != ChessType.Bishop)
						{
							for(move.to.x = 1 ; move.to.x != move.from.x ; move.to.x++)
								moves.Add(move);
							for(move.to.x = move.from.x + 1 ; move.to.x < 9 ; move.to.x++)
								moves.Add(move);
							move.to.x = move.from.x;
							for(move.to.y = 1 ; move.to.y != move.from.y ; move.to.y++)
								moves.Add(move);
							for(move.to.y = move.from.y + 1 ; move.to.y < 9 ; move.to.y++)
								moves.Add(move);
						}
						if(entry.type != ChessType.Rook)
						{
							for(move.to.x = move.from.x - 1, move.to.y = move.from.y - 1 ; move.to.x > 0 && move.to.y > 0 ; move.to.x--, move.to.y--)
								moves.Add(move);
							for(move.to.x = move.from.x + 1, move.to.y = move.from.y - 1 ; move.to.x < 9 && move.to.y > 0 ; move.to.x++, move.to.y--)
								moves.Add(move);
							for(move.to.x = move.from.x - 1, move.to.y = move.from.y + 1 ; move.to.x > 0 && move.to.y < 9 ; move.to.x--, move.to.y++)
								moves.Add(move);
							for(move.to.x = move.from.x + 1, move.to.y = move.from.y + 1 ; move.to.x < 9 && move.to.y < 9 ; move.to.x++, move.to.y++)
								moves.Add(move);
						}
					}
				}
			}
			return moves;
		}

			public bool SlowLegalMove(Move move)
			{
				ArrayList moves = GeneratePossibleMoves();
				return moves.Contains(move) && FastLegalMove(move);
			}
			public bool FastLegalMove(Move move)
			{
				bool result = FastLegalMoveWithoutCheck(move);
				if(result && check)
				{
				}
				return result;
			}
			public bool FastLegalMoveWithoutCheck(Move move)
			{
				if(entries[move.to.x, move.to.y].color == move.from.color)
					return false;

				int dx = move.to.x - move.from.x;
				int dy = move.to.y - move.from.y;
				if(move.from.type == ChessType.Pawn)
				{
					if(dx != 0)
						return entries[move.to.x, move.to.y].color >= 0;
					if(Math.Abs(dy) == 2 && entries[move.to.x, move.from.y + dy / 2].color >= 0)
						return false;
					return entries[move.to.x, move.to.y].color < 0;
				}
				else if(move.from.type != ChessType.King && move.from.type != ChessType.Knight)
				{
					int stepx = Math.Sign(dx);
					int stepy = Math.Sign(dy);
					int n = Math.Max(Math.Abs(dx), Math.Abs(dy)) - 1;
					int x, y;

					for(x = move.from.x + stepx, y = move.from.y + stepy ; n != 0 ; x += stepx, y += stepy, n--)
						if(entries[x, y].color >= 0)
							return false;
				}
				return true;
			}

			public bool LegalMove(Move move)
			{
				if(move.to.x == move.from.x && move.to.y == move.from.y)
					return false;
				if(entries[move.to.x, move.to.y].color == move.from.color)
					return false;

				int dx = move.to.x - move.from.x;
				int dy = move.to.y - move.from.y;
				int stepx = Math.Sign(dx);
				int stepy = Math.Sign(dy);
				int n = Math.Max(Math.Abs(dx), Math.Abs(dy)) - 1;
				int x, y;

				switch(move.from.type)
				{
					case ChessType.Pawn:
						int start;
						int step;
						if(move.from.color == Color.White)
						{
							start = 2;
							step = 1;
						}
						else
						{
							start = 7;
							step = -1;
						}

						if(move.from.x == move.to.x)
						{
							if((move.to.y - move.from.y) / step == 1)
							{
								if(entries[move.to.x, move.to.y].color < 0)
									return true;
							}
							else if((move.from.y == start) && (move.to.y - move.from.y) / step == 2)
							{
								if(entries[move.to.x,move.to.y].color < 0 && entries[move.to.x,move.to.y - step].color < 0)
									return true;
							}
						}
						else if(Math.Abs(move.from.x - move.to.x) == 1 && (move.from.y + step == move.to.y))
						{
							StateEntry entry = entries[move.to.x, move.to.y];
							if(entry.color >= 0 && entry.color != move.from.color)
								return true;
						}
						return false;
					case ChessType.Rook:
						if(dx * dy != 0)
							return false;

						for(x = move.from.x + stepx, y = move.from.y + stepy ; n != 0 ; x += stepx, y += stepy, n--)
							if(entries[x, y].color >= 0)
								return false;
						return true;
					case ChessType.Bishop:
						if(Math.Abs(dx) != Math.Abs(dy))
							return false;

						for(x = move.from.x + stepx, y = move.from.y + stepy ; n != 0 ; x += stepx, y += stepy, n--)
							if(entries[x, y].color >= 0)
								return false;
						return true;
					case ChessType.Queen:
						if(dx * dy != 0 && Math.Abs(dx) != Math.Abs(dy))
							return false;

						for(x = move.from.x + stepx, y = move.from.y + stepy ; n != 0 ; x += stepx, y += stepy, n--)
							if(entries[x, y].color >= 0)
								return false;
						return true;
					case ChessType.Knight:
						dx = Math.Abs(dx);
						dy = Math.Abs(dy);
						if(dx == 1 && dy == 2 || dx == 2 && dy == 1)
							if(entries[move.to.x, move.to.y].color != move.from.color)
								return true;
						return false;
				}

				return false;
			}

			public bool IsCheck()
			{
				Color enemy = (Color)(1 - (int)turn);
				StateEntry king;
				king.x = king.y = -1;
				foreach(StateEntry entry in entries)
				{
					if(entry.color == turn && entry.type == ChessType.King)
					{
						king = entry;
						break;
					}
				}
				foreach(StateEntry entry in entries)
				{
					if(entry.color == enemy)
					{
						Move move;
						move.from = entry;
						move.to.x = king.x;
						move.to.y = king.y;
						move.to.color = move.from.color;
						move.to.type = move.from.type;

						if(LegalMove(move))
							return true;
					}
				}
				return false;
			}

			public int Estimate()
			{
				double result = 0;
				foreach(StateEntry entry in entries)
				{
					double step;
					if(entry.color == Color.White)
						step = 1;
					else
						step = -1;

					switch(entry.type)
					{
						case ChessType.Pawn:
							result += 100 * step;
							continue;
						case ChessType.Rook:
							result += 500 * step;
							continue;
						case ChessType.Knight:
							result += 300 * step;
							continue;
						case ChessType.Bishop:
							result += 325 * step;
							continue;
						case ChessType.Queen:
							result += 900 * step;
							continue;
						case ChessType.King:
							result += 10000 * step;
							continue;
					}
				}
				return (int)result;
			}
		}
	}
}
