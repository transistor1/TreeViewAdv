using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Quimron.WinUI.Controls.TreeListView.NodeControls
{
	[DesignTimeVisible(false), ToolboxItem(false)]
	public abstract class NodeControl: Component
	{
		#region Properties

		private TreeListView _parent;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeListView Parent
		{
			get { return _parent; }
			set 
			{
				if (value != _parent)
				{
					if (_parent != null)
						_parent.NodeControls.Remove(this);

					if (value != null)
						value.NodeControls.Add(this);
				}
			}
		}

		private IToolTipProvider _toolTipProvider;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IToolTipProvider ToolTipProvider
		{
			get { return _toolTipProvider; }
			set { _toolTipProvider = value; }
		}

		private TreeColumn _parentColumn;
		public TreeColumn ParentColumn
		{
			get { return _parentColumn; }
			set 
			{ 
				_parentColumn = value; 
				if (_parent != null)
					_parent.FullUpdate();
			}
		}

		private VerticalAlignment _verticalAlign = VerticalAlignment.Center;
		[DefaultValue(VerticalAlignment.Center)]
		public VerticalAlignment VerticalAlign
		{
			get { return _verticalAlign; }
			set 
			{ 
				_verticalAlign = value;
				if (_parent != null)
					_parent.FullUpdate();
			}
		}

		#endregion

		internal virtual void AssignParent(TreeListView parent)
		{
			_parent = parent;
		}

		protected virtual Rectangle GetBounds(TreeListNode node, DrawContext context)
		{
			Rectangle r = context.Bounds;
			Size s = MeasureSize(node, context);
			Size bs = new Size(r.Width, s.Height);
			switch (VerticalAlign)
			{
				case VerticalAlignment.Top:
					return new Rectangle(new Point(r.X, r.Y), bs);
				case VerticalAlignment.Bottom:
					return new Rectangle(new Point(r.X, r.Bottom - s.Height), bs);
				default:
					return new Rectangle(new Point(r.X, r.Y + (r.Height - s.Height) / 2), bs);
			}
		}

		protected void CheckThread()
		{
			if (Parent != null && Control.CheckForIllegalCrossThreadCalls)
				if (Parent.InvokeRequired)
					throw new InvalidOperationException("Cross-thread calls are not allowed");
		}

		public bool IsVisible(TreeListNode node)
		{
			NodeControlValueEventArgs args = new NodeControlValueEventArgs(node);
			args.Value = true;
			OnIsVisibleValueNeeded(args);
			return Convert.ToBoolean(args.Value);
		}

		internal Size GetActualSize(TreeListNode node, DrawContext context)
		{
			if (IsVisible(node))
				return MeasureSize(node, context);
			else
				return Size.Empty;
		}

		public abstract Size MeasureSize(TreeListNode node, DrawContext context);

		public abstract void Draw(TreeListNode node, DrawContext context);

		public virtual string GetToolTip(TreeListNode node)
		{
			if (ToolTipProvider != null)
				return ToolTipProvider.GetToolTip(node, this);
			else
				return string.Empty;
		}

		public virtual void MouseDown(TreeListNodeMouseEventArgs args)
		{
		}

		public virtual void MouseUp(TreeListNodeMouseEventArgs args)
		{
		}

		public virtual void MouseDoubleClick(TreeListNodeMouseEventArgs args)
		{
		}

		public virtual void KeyDown(KeyEventArgs args)
		{
		}

		public virtual void KeyUp(KeyEventArgs args)
		{
		}

		public event EventHandler<NodeControlValueEventArgs> IsVisibleValueNeeded;
		private void OnIsVisibleValueNeeded(NodeControlValueEventArgs args)
		{
			if (IsVisibleValueNeeded != null)
				IsVisibleValueNeeded(this, args);
		}
	}
}