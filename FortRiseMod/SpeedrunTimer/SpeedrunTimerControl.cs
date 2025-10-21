using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using SpeedrunTimer;
using static TowerFall.Player;

namespace TowerFall;

public class SpeedrunTimerControl : HUD
{
	public long time;

	public bool canEnd = true;

	private Text text;

	private Action<Tween> onTweenOut;

	private float flashAlpha;

	public bool Started { get; private set; }

	RoundLogic e;
	private Vector2 positionValue;

	public SpeedrunTimerControl()
	{
		base.Depth = -10000;
	}

	public SpeedrunTimerControl(RoundLogic e)
	{
		base.Depth = -10000;
		this.e = e;
	}


	public override void Added()
	{
		base.Added();

		var position = SpeedrunTimerModule.Settings.PinPosition;

		positionValue = new Vector2(Engine.Instance.Screen.Width - 50, 20f);

		if (!string.IsNullOrEmpty(position))
		{
			if (position.StartsWith("BOTTOM "))
			{
				positionValue.Y = Engine.Instance.Screen.Height - 10f;
			}
			if (position.EndsWith(" LEFT"))
			{
				positionValue.X = 3;
			}
			if (position.EndsWith(" CENTER"))
			{
				positionValue.X = (Engine.Instance.Screen.Width / 2) - 25;
			}
		}

		text = new Text(TFGame.Font, "0.000", positionValue, Text.HorizontalAlign.Left, Text.VerticalAlign.Top);
		text.Scale = Vector2.One * 2f;
		Add(text);
		// Tween tween2 = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 20, start: true);
		// tween2.OnUpdate = delegate (Tween t)
		// {
		// 	text.X = MathHelper.Lerp(positionValue.X, 5f, t.Eased);
		// };
		// Add(tween2);
		// onTweenOut = delegate (Tween t)
		// {
		// 	text.X = MathHelper.Lerp(5f, positionValue.X, t.Eased);
		// };
	}

	public override void SceneBegin()
	{
		base.SceneBegin();
		canEnd = true;
	}


	public override void Update()
	{
		base.Update();

		if (flashAlpha > 0f)
		{
			flashAlpha -= 0.02f * Engine.TimeMult;
		}

		if (canEnd)
		{
			if (e.Session.RoundLogic is QuestRoundLogic questRoundLogic)
			{
				time = questRoundLogic.Time;
			}

			if (e.Session.RoundLogic is DarkWorldRoundLogic)
			{
				time = e.Session.DarkWorldState.Time;
			}
		}


		text.DrawText = GetTimeString(time);
	}

	public static string GetTimeString(TimeSpan time)
	{
		if ((int)time.TotalMinutes > 0)
		{
			return (int)time.TotalMinutes + ":" + time.Seconds.ToString(2) + "." + time.Milliseconds.ToString(3);
		}

		return time.Seconds + "." + time.Milliseconds.ToString(3);
	}

	public static string GetTimeString(long ticks)
	{
		return GetTimeString(TimeSpan.FromTicks(ticks));
	}

	public override void Render()
	{
		base.Render();
		if (flashAlpha > 0f && !SaveData.Instance.Options.RemoveScreenFlashEffects)
		{
			Draw.Rect(0f, 0f, 320f, 240f, Color.White * flashAlpha);
		}
	}

	private void KillAllEnemies()
	{
		if (base.Level[GameTags.Enemy].Count != 0)
			foreach (Enemy item in base.Level[GameTags.Enemy])
			{
				if (item is DarkWorldBoss)
					continue;

				if (item)
					item.Die(0);
			}
	}


    private void DebugPosition()
    {
        if (MInput.Keyboard.Check(Keys.Up))
        {
            positionValue.Y = positionValue.Y + 0.1f;
        }
        if (MInput.Keyboard.Check(Keys.Down))
        {
            positionValue.Y = positionValue.Y - 0.1f;
        }
        if (MInput.Keyboard.Check(Keys.Left))
        {
            positionValue.X = positionValue.X + 0.1f;
        }
        if (MInput.Keyboard.Check(Keys.Right))
        {
            positionValue.X = positionValue.X - 0.1f;
        }

        text.Position = positionValue;
        Console.WriteLine(positionValue);
    }

}
