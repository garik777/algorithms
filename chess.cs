using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgorithmsPractice
{
	class Program2
	{
		static void Main(string[] args){
			ChessBoard board = new ChessBoard ();




		}

		public struct TradeNAK
		{   public string BatchId;
			public string TradeId;
			public bool AckNakSuccess;
			public string ErrorText; 
		}

		//The func takes 3 params, the first one is a List and is used loop over TradeNAK stucks and updates the tSWIFT_History_Trades table for each TradeNAK in the list. 
		// The 2 other variables strFileNameRecieved and strDirectoryArchivedRecieved are applied per update. The update statement updates 4 colunms in sSWIFT_History_Trades table: MachineIDRecieved from an enviroment variable,
		//FileNameRecieved from func param, DirectoryArchivedReceieved also a func param, DateReceived from an ancillary func and AckNak which is hardcoded to equals 'ACK'. The updates is filtered based on the BatchID and TradeID, instance variable of the TradeNAC struct. 

		public int UpdateTradeACKNAKDB(List<TradeNAK> _lANT, string strFileNameReceived, string strDirectoryArchivedReceived)
		{
			try
			{
				int j = 0;
				var success = new Tuple<StringBuilder, StringBuilder>(new StringBuilder(), new StringBuilder());
				var failed  = new Tuple<StringBuilder, StringBuilder>(new StringBuilder(), new StringBuilder());

				for (int i = 0; i < _lANT.Count; i++){
					//if error exist then run update
					if(!string.IsNullOrEmpty(_lANT[i].ErrorText)) {
						j += Convert.ToInt16(dba.ExecuteSQL(string.Format(
								@"UPDATE tSWIFT_History_Trades 
						  		  SET MachineIdReceived = '{0},
						  	  		  FileNameRecieved = '{1}', 
						  	  		  DirectoryArchivedReceived = '{2}', 
						      		  DateRecieved = GetDate(), 
						              AckNak = '{3}',
									  Error = '{4}'
						  			  WHERE (BatchId = {5} AND TradeID = {6})", 
							System.Environment.UserName.ToUpper(),
							strFileNameReceived,
							strDirectoryArchivedReceived,
							_lANT[i].AckNakSuccess ? "ACK" : "NAK",
							_lANT[i].ErrorText,
							_lANT[i].BatchId),
							_lANT[i].TradeId));
					}
					// else store failed and successful TradeNAK's with no ErrorText for batching updates out of loop
					else { 
						if (_lANT[i].AckNakSuccess){
							success.Item1.Append(string.Format("{0},", _lANT[i].BatchId));
							success.Item2.Append(string.Format("{0},", _lANT[i].TradeId));
						}
						else {
							failed.Item1.Append(string.Format("{0},", _lANT[i].BatchId));
							failed.Item2.Append(string.Format("{0},", _lANT[i].TradeId));
						}
					}
				}

				if (failed.Item1.Length>0){
					j += Convert.ToInt16(dba.ExecuteSQL(string.Format(
						@"UPDATE tSWIFT_History_Trades 
						  SET MachineIdReceived = '{0},
						  	  FileNameRecieved = '{1}', 
						  	  DirectoryArchivedReceived = '{2}', 
						      DateRecieved = GetDate(), 
						      AckNak = 'NAK'
						  WHERE (BatchId in ({3}) AND TradeID in ({4}))", 
						System.Environment.UserName.ToUpper(),
						strFileNameReceived,
						strDirectoryArchivedReceived,
						failed.Item1.Remove(failed.Item1.Length - 1,1).ToString(),
						failed.Item2.Remove(failed.Item2.Length - 1,1).ToString())));
				}

				if (success.Item1.Length>0){
					j += Convert.ToInt16(dba.ExecuteSQL(string.Format(
						@"UPDATE tSWIFT_History_Trades 
						  SET MachineIdReceived = '{0},
						  	  FileNameRecieved = '{1}', 
						  	  DirectoryArchivedReceived = '{2}', 
						      DateRecieved = GetDate(), 
						      AckNak = 'ACK'
						  WHERE (BatchId in ({3}) AND TradeID in ({4}))", 
						System.Environment.UserName.ToUpper(),
						strFileNameReceived,
						strDirectoryArchivedReceived,
						success.Item1.Remove(success.Item1.Length - 1,1).ToString(),
						success.Item2.Remove(success.Item2.Length - 1,1).ToString())));
				}
			
				return j;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
	}
	public enum Color { White=0, Black=1};
	public enum PieceType { Pond=1, Rook=2, Knight = 3, Bishop = 4, Queen = 5, King=6};

 	public abstract class Piece {

		abstract public  Color Color { get; }
		abstract public PieceType PieceType { get; }
		abstract public int Row { get; set;}
		abstract public int Col { get; set; }

		public abstract void MovePiece (int column, int row, ChessBoard board);
	}


	internal class ChessPiece : Piece
	{
		int _row = 0;
		int _col = 0;
		Color _color;
		PieceType _type;

		public override int Col {
			get {
				return _col;
			}
			set { _col = value; }
		}
		public override int Row {
			get {
				return _row;
			}
			set{ _row = value; }

		}
		public override Color Color
		{
			get { return _color; }
		}
		public override PieceType PieceType {
			get {
				return _type;
			}
		}

		public ChessPiece(int row, int col, Color color, PieceType type){
			_col = col;
			_row = row;
			_color = color;
			_type = type;
		}

		public override void MovePiece (int column, int row, ChessBoard board)
		{

		}

		 
	}
	

	public class ChessBoard
	{
		private const int SIZE = 8;
		private int[,] _board = new int[8,8];
		private IDictionary<int,ChessPiece> _piecesCache = new Dictionary<int,ChessPiece>();

		public ChessBoard ()
		{
			SetupBoard ();
		}

		public void MovePiece( int hashcode, int row, int col){
			ChessPiece piece = _piecesCache [hashcode] as ChessPiece;

			if (CanMove (piece, row, col)) {

			}

		}
		private bool CanMove(ChessPiece piece, int row, int col){
			bool move = false;
			switch (piece.PieceType) {
			case PieceType.Pond:

				//test starting positions for all ponds
				if ((piece.Row == 1 && piece.Col == col && (row == piece.Row + 1 || row == piece.Row + 2 )) ||
					(piece.Row == SIZE - 2 && piece.Col == col && (row == piece.Row - 1 || row == piece.Row -2))){
					move = true;
					break;
				}
				//test advance any position
				if (piece.Col == col && row == piece.Row + 1){
					move = true;
					break;
				}

				//test attach position
				//if (piece.Color.Equals(Color.Black) _board[piece.Col - 1,piece - 1 

			}
			return true;
		}
		private void SetupBoard(){
			ChessPiece piece;


			for (int i = 0; i < SIZE; i++) {
				for (int j = 0; j < SIZE; j++) {
					Color color = i == 0 || i == 1 ? Color.Black : Color.White;

					if (i == 0 || i == SIZE - 1) {
						switch (j) {
						case 0:
							piece = new ChessPiece (i, j, color, PieceType.Rook);
							break;
						case 1:
							piece = new ChessPiece (i, j, color, PieceType.Knight);
							break;
						case 2:
							piece = new ChessPiece (i, j, color, PieceType.Bishop);
							break;
						case 3:
							piece = new ChessPiece (i, j, color, PieceType.Queen);
							break;
						case 4:
							piece = new ChessPiece (i, j, color, PieceType.King);	
							break;
						case 5:
							piece = new ChessPiece (i, j, color, PieceType.Bishop);
							break;
						case 6:
							piece = new ChessPiece (i, j, color, PieceType.Rook);
							break;
						case 7:
							piece = new ChessPiece (i, j, color, PieceType.Rook);
							break;
						default:
							break;
						}

						_board [i, j] = piece.GetHashCode ();
						_piecesCache.Add (piece.GetHashCode (), piece);	
					}
				
					if (i == 1 || i == SIZE - 2) {
						piece = new ChessPiece (i, j, color, PieceType.Pond);
						_board [i, j] = piece.GetHashCode ();						

					}
				}
			}
		}
	}



}


