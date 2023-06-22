/* Licensed under the Open Software License version 3.0 */

using System.Drawing;
using static ZeldaSaveTool.ID.Colors;
using static ZeldaSaveTool.Utility.Forms;

namespace ZeldaSaveTool.Controls;

internal sealed class Hearts : Control {
	private static readonly Image[] HeartImg = {
		R.Empty, R.OneQuarter, R.Half, R.ThreeQuarter, R.Full
	};
	private static readonly Image[] HeartDdImg = {
		R.DDEmpty, R.DDOneQuarter, R.DDHalf, R.DDThreeQuarter, R.DDFull
	};

	/* Initial health values (16 per full heart) */
	private short _curHealth = 48;
	private short _totalHealth = 48;
	private bool _isDd;
	private System.Drawing.Imaging.ImageAttributes _ia = new();
	private Image[] _hImages = HeartImg;

	private void SetProps() {
		Tint c = new();
		c.Replace(Color.FromArgb(68, 68, 68), NoHeartEmpty);
		c.Replace(from: Color.White, to: _isDd ? DDHeartInner : SDHeartInner);
		c.Replace(from: Color.Black, to: _isDd ? DDHeartOuter : SDHeartOuter);
		_ia = c.GetImageAttributes();
		_hImages = _isDd ? HeartDdImg : HeartImg;
	}

	public Hearts() {
		DoubleBuffered = true;
		SetProps();
	}

	public short CurrentHealth {
		get => _curHealth; set {
			if (value < 0) _curHealth = 0;
			else if (value > _totalHealth) _curHealth = _totalHealth;
			else _curHealth = value;
			Invalidate();
		}
	}

	public short TotalHealth {
		get => _totalHealth; set {
			_totalHealth = value < 0 ? (short)0 : value;
			if (_totalHealth < _curHealth) _curHealth = _totalHealth;
			if (value > 320) _totalHealth = 320;
			Invalidate();
		}
	}

	public bool IsDoubleDefense {
		get => _isDd; set { _isDd = value; SetProps(); Invalidate(); }
	}

	protected override void OnPaint(PaintEventArgs e) {
		base.OnPaint(e);

		int totalHearts = _totalHealth / 16;
		int fullHearts = _curHealth / 16;
		int heartQuarter = _curHealth % 16 / 4;
		int perRow = Math.Min(totalHearts, 10); /* 10 hearts per row */
		int numRows = (int)Math.Ceiling((double)fullHearts / perRow);

		for (int row = 0; row < numRows; row++) {
			int startHeartIndex = row * perRow;
			int endHeartIndex = Math.Min(startHeartIndex + perRow, fullHearts);

			for (int i = startHeartIndex; i < endHeartIndex; i++)
				DrawHeart(e, 4, i % perRow, row);
		}

		if (heartQuarter > 0)
			DrawHeart(e, heartQuarter, fullHearts % perRow, fullHearts++ / perRow);

		for (int i = fullHearts; i < totalHearts; i++)
			DrawHeart(e, 0, i % perRow, i / perRow);
	}

	private void DrawHeart(PaintEventArgs e, int index, int x, int y) {
		Rectangle rect = new(x * 14, y * 14, 16, 16);
		e.Graphics.DrawImage(_hImages[index], rect, 0, 0, 16, 16, GraphicsUnit.Pixel, _ia);
	}
}