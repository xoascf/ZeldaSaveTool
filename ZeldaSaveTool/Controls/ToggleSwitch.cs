/* Licensed under the Open Software License version 3.0 */

using System.Drawing;

namespace ZeldaSaveTool.Controls;

internal sealed class ToggleSwitch : Control {
	private bool _checked;
	public bool Checked {
		get => _checked;
		set { _checked = value; Invalidate(); }
	}

	private string _leftLabelText;
	public string LeftLabelText {
		get => _leftLabelText;
		set { _leftLabelText = value; Invalidate(); }
	}

	private string _rightLabelText;
	public string RightLabelText {
		get => _rightLabelText;
		set { _rightLabelText = value; Invalidate(); }
	}

	private int _toggleX;
	private int _targetToggleX;
	private readonly Timer _animationTimer;

	public ToggleSwitch() {
		DoubleBuffered = true;
		Cursor = Cursors.Hand;
		Checked = false;
		LeftLabelText = "OFF";
		RightLabelText = "ON";
		_toggleX = 1;
		_targetToggleX = Checked ? Width / 2 : 1;
		_animationTimer = new();
		_animationTimer.Interval = 10;
		_animationTimer.Tick += AnimationTimer_Tick;
	}

	protected override void OnPaint(PaintEventArgs e) {
		base.OnPaint(e);

		Graphics g = e.Graphics;
		g.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
		int toggleWidth = Width / 2;
		int toggleHeight = Height - 2;
		const int toggleY = 1;
		g.FillRectangle(new SolidBrush(ForeColor), _toggleX, toggleY, toggleWidth, toggleHeight);
		using StringFormat format = new();
		format.Alignment = StringAlignment.Center;
		format.LineAlignment = StringAlignment.Center;
		Font labelFont = new(Font.FontFamily, Font.Size);
		int labelHeight = (int)g.MeasureString("TEST", labelFont).Height;
		RectangleF leftLabelRect = new(0, Height / 2 - labelHeight / 2, Width / 2, labelHeight);
		RectangleF rightLabelRect = new(Width / 2, Height / 2 - labelHeight / 2, Width / 2, labelHeight);
		g.DrawString(LeftLabelText, labelFont, Brushes.Black, leftLabelRect, format);
		g.DrawString(RightLabelText, labelFont, Brushes.Black, rightLabelRect, format);
	}

	protected override void OnClick(EventArgs e) {
		base.OnClick(e);

		Checked = !Checked;
		_targetToggleX = Checked ? Width / 2 : 1;
		_animationTimer.Start();
	}

	private void AnimationTimer_Tick(object sender, EventArgs e) {
		if (_toggleX < _targetToggleX) {
			_toggleX += 2;
			if (_toggleX >= _targetToggleX) {
				_toggleX = _targetToggleX;
				_animationTimer.Stop();
			}
			Invalidate();
		} else if (_toggleX > _targetToggleX) {
			_toggleX -= 2;
			if (_toggleX <= _targetToggleX) {
				_toggleX = _targetToggleX;
				_animationTimer.Stop();
			}
			Invalidate();
		}
	}
}