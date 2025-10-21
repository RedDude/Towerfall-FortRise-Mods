using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle;

public class SpeedrunTimerText : GraphicsComponent
{
	private const float DELTA_TIME = 1f / 60f;

	private SpriteFont font;

	private int frames;

	private Vector2 justify;

	public string Text { get; private set; }

    public bool Paused;

	public SpriteFont Font
	{
		get
		{
			return font;
		}
		set
		{
			font = value;
			CalculateOrigin();
		}
	}

	public int Frames
	{
		get
		{
			return frames;
		}
		set
		{
			if (frames != value)
			{
				frames = value;
				UpdateText();
				CalculateOrigin();
			}
		}
	}

	public Vector2 Justify
	{
		get
		{
			return justify;
		}
		set
		{
			justify = value;
			CalculateOrigin();
		}
	}

	public override float Width => font.MeasureString(Text).X;

	public override float Height => font.MeasureString(Text).Y;

	public SpeedrunTimerText(SpriteFont font, Vector2 justify)
		: base(active: true)
	{
		this.font = font;
		this.justify = justify;
		UpdateText();
		CalculateOrigin();
	}

	private void UpdateText()
	{
        Text = ((frames / 60) + (float)(frames % 60) * (1f / 60f)).ToString("0.00");
	}

	private void CalculateOrigin()
	{
		Origin = (font.MeasureString(Text) * justify).Floor();
	}

	public override void Update()
	{
		base.Update();
	
        if (Paused)
            return;
        frames++;
        UpdateText();
        CalculateOrigin();
	}

	public override void Render()
	{
		Draw.SpriteBatch.DrawString(font, Text, base.RenderPosition, Color, Rotation, Origin, Scale * Zoom, Effects, 0f);
	}
}
