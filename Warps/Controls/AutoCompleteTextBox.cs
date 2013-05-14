using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Warps.Controls
{
	public class AutoCompleteTextBox : TextBox
	{
		private ListBox _listBox;
		private bool _isAdded;
		private object[] _values;
		private String _formerValue = String.Empty;
		private ImageList _images = new ImageList();

		public AutoCompleteTextBox()
		{
			InitializeComponent();
			ResetListBox();
			_listBox.DrawMode = DrawMode.OwnerDrawFixed;
			_listBox.DrawItem += _listBox_DrawItem;

			_images.Images.Add("String", Warps.Properties.Resources.equation);
			_images.Images.Add("MouldCurve", Warps.Properties.Resources.glyphicons_098_vector_path_curve);
		}

		void _listBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index == -1)
				return;

			if (e.Bounds.Top < 0)
				return;

			if (e.Bounds.Bottom > (_listBox.Bottom + _listBox.ItemHeight))
				return;

			e.DrawBackground();

			Graphics gr = e.Graphics;

			string Text = _listBox.Items[e.Index].ToString();

			Image img = null;

			if (_images != null)
			{
				img = _images.Images[_listBox.Items[e.Index].GetType().Name];
				Rectangle imgRect = new Rectangle(e.Bounds.Left,
								e.Bounds.Top, img.Width, img.Height);
				gr.DrawImage(img, imgRect, 0, 0, img.Width,
								img.Height, GraphicsUnit.Pixel);
			}

			using (Brush b = new SolidBrush(Color.Black))
			{
				gr.DrawString(Text, this.Font, b,
				    new Point(e.Bounds.Left + img.Width, e.Bounds.Top + 2));
			}

			e.DrawFocusRectangle();
		}

		private void InitializeComponent()
		{
			_listBox = new ListBox();
			KeyDown += this_KeyDown;
			KeyUp += this_KeyUp;
		}

		private void ShowListBox()
		{
			if (!_isAdded)
			{
				Parent.Controls.Add(_listBox);
				_listBox.Left = Left;
				_listBox.Top = Top + Height;
				_isAdded = true;
				//_listBox.Location = this.Cursor.HotSpot;
			}
			_listBox.Visible = true;
			
			_listBox.BringToFront();
			//Parent.SendToBack();
		}

		private void ResetListBox()
		{
			_listBox.Visible = false;
		}

		private void this_KeyUp(object sender, KeyEventArgs e)
		{
			UpdateListBox();
		}

		private void this_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Tab:
					{
						if (_listBox.Visible)
						{
							InsertWord(_listBox.SelectedItem.ToString());
							ResetListBox();
							_formerValue = Text;
						}
						break;
					}
				case Keys.Down:
					{
						if ((_listBox.Visible) && (_listBox.SelectedIndex < _listBox.Items.Count - 1))
							_listBox.SelectedIndex++;

						break;
					}
				case Keys.Up:
					{
						if ((_listBox.Visible) && (_listBox.SelectedIndex > 0))
							_listBox.SelectedIndex--;

						break;
					}
				case Keys.Enter:
					{
						if (_listBox.Visible)
						{
							InsertWord(_listBox.SelectedItem.ToString());
							ResetListBox();
							_formerValue = Text;
						}
						break;
					}
			}
		}

		protected override bool IsInputKey(Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Tab:
					return true;
				default:
					return base.IsInputKey(keyData);
			}
		}

		private void UpdateListBox()
		{
			if (Text == _formerValue) return;
			_formerValue = Text;
			String word = GetWord();

			if (_values != null && word.Length > 0)
			{
				object[] matches = Array.FindAll(_values,
										   x => (x.ToString().StartsWith(word, StringComparison.OrdinalIgnoreCase) && !SelectedValues.Contains(x)));
				if (matches.Length > 0)
				{
					ShowListBox();
					_listBox.Items.Clear();
					Array.ForEach(matches, x => _listBox.Items.Add(x));
					_listBox.SelectedIndex = 0;
					_listBox.Height = 0;
					_listBox.Width = 0;
					Focus();
					using (Graphics graphics = _listBox.CreateGraphics())
					{
						for (int i = 0; i < _listBox.Items.Count; i++)
						{
							_listBox.Height += _listBox.GetItemHeight(i);
							// it item width is larger than the current one
							// set it to the new max item width
							// GetItemRectangle does not work for me
							// we add a little extra space by using '_'
							int itemWidth = (int)graphics.MeasureString((_listBox.Items[i].ToString()) + "_", _listBox.Font).Width;
							_listBox.Width = (_listBox.Width < itemWidth) ? itemWidth : _listBox.Width;

						}
						_listBox.Height += 15;
						_listBox.Width += 15;
						//_listBox.Location = new Point(_listBox.Location.X + 100, _listBox.Location.Y - 30);
						//_listBox.Location.X -= 100;
						//_listBox.Location.Y += 30;
					}
				}
				else
				{
					ResetListBox();
				}
			}
			else
			{
				ResetListBox();
			}
		}

		private String GetWord()
		{
			String text = Text;
			int pos = SelectionStart;

			int posStart = text.LastIndexOf(' ', (pos < 1) ? 0 : pos - 1);
			posStart = (posStart == -1) ? 0 : posStart + 1;
			int posEnd = text.IndexOf(' ', pos);
			posEnd = (posEnd == -1) ? text.Length : posEnd;

			int length = ((posEnd - posStart) < 0) ? 0 : posEnd - posStart;

			return text.Substring(posStart, length);
		}

		private void InsertWord(String newTag)
		{
			String text = Text;
			int pos = SelectionStart;

			int posStart = text.LastIndexOf(' ', (pos < 1) ? 0 : pos - 1);
			posStart = (posStart == -1) ? 0 : posStart + 1;
			int posEnd = text.IndexOf(' ', pos);

			String firstPart = text.Substring(0, posStart) + newTag;
			String updatedText = firstPart + ((posEnd == -1) ? "" : text.Substring(posEnd, text.Length - posEnd));

			Text = updatedText;
			SelectionStart = firstPart.Length;
		}

		public object[] Values
		{
			get
			{
				return _values;
			}
			set
			{
				_values = value;
			}
		}

		public List<String> SelectedValues
		{
			get
			{
				String[] result = Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				return new List<String>(result);
			}
		}

	}

}
