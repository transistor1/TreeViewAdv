using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Quimron.WinUI.Controls.TreeListView
{
	internal class ReorderColumnState : ColumnState
	{
		#region Properties

		private Point _location;
		public Point Location
		{
			get { return _location; }
		}

		private Bitmap _ghostImage;
		public Bitmap GhostImage
		{
			get { return _ghostImage; }
		}

		private TreeColumn _dropColumn;
		public TreeColumn DropColumn
		{
			get { return _dropColumn; }
		}

		#endregion

		public ReorderColumnState(TreeListView tree, TreeColumn column)
			: base(tree, column)
		{
			_ghostImage = column.CreateGhostImage(new Rectangle(0, 0, column.Width, tree.ColumnHeaderHeight), tree.Font);
		}

		public override void KeyDown(KeyEventArgs args)
		{
			args.Handled = true;
			if (args.KeyCode == Keys.Escape)
				FinishResize();
		}

		public override void MouseDown(TreeListNodeMouseEventArgs args)
		{
		}

		public override void MouseUp(TreeListNodeMouseEventArgs args)
		{
			FinishResize();
		}

		public override bool MouseMove(MouseEventArgs args)
		{
			_dropColumn = null;
			_location = new Point(args.X + Tree.OffsetX, 0);
			int x = 0;
			foreach (TreeColumn c in Tree.Columns)
			{
				if (c.IsVisible)
				{
					if (_location.X < x + c.Width / 2)
					{
						_dropColumn = c;
						break;
					}
					x += c.Width;
				}
			}
			Tree.UpdateHeaders();
			return true;
		}

		private void FinishResize()
		{
			Tree.ChangeInput();
			if (Column == DropColumn)
				Tree.UpdateView();
			else
			{
				Tree.Columns.Remove(Column);
				if (DropColumn == null)
					Tree.Columns.Add(Column);
				else
					Tree.Columns.Insert(Tree.Columns.IndexOf(DropColumn), Column);

				Tree.OnColumnReordered(Column);
			}
		}
	}
}