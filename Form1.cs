using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace ChessSharp
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panel1;

		private PictureBox[,] squares;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Panel panel2;
		private ChessLogic logic;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            squares = new PictureBox[8, 8];

			logic = new ChessLogic();

			int squareHeight = panel1.Height / 8;
			int squareWidth = panel1.Width / 8;

			bool squareWillBeBlack = true;
			for(int i = 0 ; i < 8 ; i++)
			{
				for(int j = 0 ; j < 8 ; j++)
				{
					squares[i, j] = new PictureBox();
					squares[i, j].Location = new Point(squareWidth * j, squareHeight * (7-i));
					squares[i, j].Size = new Size(squareWidth, squareHeight);
					squares[i, j].Parent = panel1;

					squares[i, j].Click += new EventHandler(Form1_Click);

					if(squareWillBeBlack)
						squares[i, j].BackColor = Color.RosyBrown;
					else
						squares[i, j].BackColor = Color.Pink;
					squareWillBeBlack = !squareWillBeBlack;
				}
				squareWillBeBlack = !squareWillBeBlack;
			}

			Redraw();
			
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panel1 = new System.Windows.Forms.Panel();
			this.button1 = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(336, 336);
			this.panel1.TabIndex = 0;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(344, 8);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(80, 32);
			this.button1.TabIndex = 1;
			this.button1.Text = "GenMove";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.White;
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Location = new System.Drawing.Point(368, 48);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(32, 32);
			this.panel2.TabIndex = 2;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(426, 343);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		/*private void Form1_Resize(object sender, EventArgs e)
		{
			if(Width != Height)
				Width = Height;

			int squareHeight = panel1.Height / 8;
			int squareWidth = panel1.Width / 8;

			for(int i = 0 ; i < 8 ; i++)
			{
				for(int j = 0 ; j < 8 ; j++)
				{
					squares[i, j].Location = new Point(squareWidth * j, squareHeight * i);
					squares[i, j].Size = new Size(squareWidth, squareHeight);
				}
			}
		}*/

		private void button1_Click(object sender, System.EventArgs e)
		{
			if(selected != null)
			{
				selected.BorderStyle = BorderStyle.None;
				selected = null;
			}
			ChessLogic.Move move = logic.GenMove();
			logic.MakeMove(move);
			Redraw();
		}

		private void Redraw()
		{
			panel2.BackColor = logic.curState.turn == ChessLogic.Color.White ? Color.White : Color.Black;

			foreach(PictureBox box in squares)
				box.Image = null;
			foreach(ChessLogic.StateEntry entry in logic.curState.entries)
			{
				string filename;
				if(entry.color == ChessLogic.Color.White)
					filename = "w";
				else
					filename = "b";
				if(entry.type == ChessLogic.ChessType.Pawn)
					filename += "pawn.gif";
				else if(entry.type == ChessLogic.ChessType.Knight)
					filename += "knight.gif";
				else if(entry.type == ChessLogic.ChessType.Rook)
					filename += "rook.gif";
				else if(entry.type == ChessLogic.ChessType.Bishop)
					filename += "bishop.gif";
				else if(entry.type == ChessLogic.ChessType.Queen)
					filename += "queen.gif";
				else
					filename += "king.gif";
				squares[entry.y - 1, entry.x - 1].Image = Image.FromFile(filename);
			}
		}

        PictureBox selected;
		int xFrom, yFrom;
		private void Form1_Click(object sender, EventArgs e)
		{
			PictureBox box = (PictureBox)sender;
			int x, y;
			x = y = 0;
			for(int i = 0 ; i < 8 ; i++)
				for(int j = 0 ; j < 8 ; j++)
					if(squares[i, j] == box)
					{
						x = j + 1;
						y = i + 1;
						goto breakboth;
					}
			breakboth:
				if(logic.curState.entries[x, y].color == logic.curState.turn)
				{
					if(selected != null)
					{
						selected.BorderStyle = BorderStyle.None;
					}
					selected = box;
					selected.BorderStyle = BorderStyle.FixedSingle;
					xFrom = x;
					yFrom = y;
				}
				else
				{
					ChessLogic.Move move;
					move.from = logic.curState.entries[xFrom, yFrom];
					move.to = move.from;
					move.to.x = x;
					move.to.y = y;
					if(logic.curState.SlowLegalMove(move))
					{
						selected.BorderStyle = BorderStyle.None;
						selected = null;
						logic.curState.MakeMove(move);
						Redraw();
					}
				}
		}
	}
}
