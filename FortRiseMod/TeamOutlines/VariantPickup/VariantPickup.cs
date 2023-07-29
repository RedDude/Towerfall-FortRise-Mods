// using Microsoft.Xna.Framework;
// using Monocle;
//
// namespace TowerFall
// {
// 	public class VariantPickup : Pickup
// 	{
// 		private Variant variant;
//
// 		private GraphicsComponent graphic;
// 		private MatchVariants _matchVariants;
//
// 		public VariantPickup(Vector2 position, Vector2 targetPosition, Variant variant)
// 			: base(position, targetPosition)
// 		{
// 			_matchVariants = VariantPickupController.GetInstance().matchVariants;
// 			_matchVariants.
// 			base.Collider = new Hitbox(16f, 16f, -8f, -8f);
// 			Tag(GameTags.PlayerCollectible);
// 			switch (arrowType)
// 			{
// 				case ArrowTypes.Normal:
// 					graphic = new Image(TFGame.Atlas["pickups/arrowPickup"]);
// 					graphic.CenterOrigin();
// 					Add(graphic);
// 					break;
// 				case ArrowTypes.Bomb:
// 				{
// 					Sprite<int> spriteInt = new Sprite<int>(TFGame.Atlas["pickups/bombArrows"], 12, 12);
// 					spriteInt.Add(0, 0.3f, 0, 1);
// 					spriteInt.Play(0);
// 					spriteInt.CenterOrigin();
// 					Add(spriteInt);
// 					graphic = spriteInt;
// 					break;
// 				}
// 				case ArrowTypes.SuperBomb:
// 				{
// 					Sprite<int> spriteInt = new Sprite<int>(TFGame.Atlas["pickups/superBombArrows"], 12, 12);
// 					spriteInt.Add(0, 0.3f, 0, 1);
// 					spriteInt.Play(0);
// 					spriteInt.CenterOrigin();
// 					Add(spriteInt);
// 					graphic = spriteInt;
// 					break;
// 				}
// 				case ArrowTypes.Laser:
// 				{
// 					Sprite<int> spriteInt = new Sprite<int>(TFGame.Atlas["pickups/laserArrows"], 12, 12);
// 					spriteInt.Add(0, 0.3f, 0, 1);
// 					spriteInt.Play(0);
// 					spriteInt.CenterOrigin();
// 					Add(spriteInt);
// 					graphic = spriteInt;
// 					break;
// 				}
// 				case ArrowTypes.Bramble:
// 					graphic = new Image(TFGame.Atlas["pickups/brambleArrows"]);
// 					graphic.CenterOrigin();
// 					Add(graphic);
// 					break;
// 				case ArrowTypes.Drill:
// 					graphic = new Image(TFGame.Atlas["pickups/drillArrows"]);
// 					graphic.CenterOrigin();
// 					Add(graphic);
// 					break;
// 				case ArrowTypes.Bolt:
// 				{
// 					Sprite<int> spriteInt = new Sprite<int>(TFGame.Atlas["pickups/boltArrows"], 12, 12);
// 					spriteInt.Add(0, 0.05f, 0, 1, 2);
// 					spriteInt.Play(0);
// 					spriteInt.CenterOrigin();
// 					Add(spriteInt);
// 					graphic = spriteInt;
// 					break;
// 				}
// 				case ArrowTypes.Feather:
// 					graphic = new Image(TFGame.Atlas["pickups/featherArrows"]);
// 					graphic.CenterOrigin();
// 					Add(graphic);
// 					break;
// 				case ArrowTypes.Trigger:
// 				{
// 					Sprite<int> spriteInt = TFGame.SpriteData.GetSpriteInt("TriggerArrowsPickup");
// 					spriteInt.Play(0);
// 					Add(spriteInt);
// 					graphic = spriteInt;
// 					break;
// 				}
// 				case ArrowTypes.Prism:
// 				{
// 					Sprite<int> spriteInt = TFGame.SpriteData.GetSpriteInt("PrismArrowsPickup");
// 					spriteInt.Play(0);
// 					Add(spriteInt);
// 					graphic = spriteInt;
// 					break;
// 				}
// 				case ArrowTypes.Toy:
// 					break;
// 			}
// 		}
//
// 		public override void Update()
// 		{
// 			base.Update();
// 			graphic.Position = base.DrawOffset;
// 		}
//
// 		public override void Render()
// 		{
// 			DrawGlow();
// 			graphic.DrawOutline();
// 			base.Render();
// 		}
//
// 		public override void DoPlayerCollect(Player player)
// 		{
// 			player.CollectArrows(arrowType, arrowType);
// 			PlaySound();
// 		}
//
// 		public override void OnPlayerCollide(Player player)
// 		{
// 			// if (player.CollectArrows(arrowType, arrowType))
// 			// {
// 			// 	DoCollectStats(player.PlayerIndex);
// 			// 	RemoveSelf();
// 			// 	Color color = Arrow.Colors[(int) arrowType];
// 			// 	Color color2 = Arrow.ColorsB[(int) arrowType];
// 			// 	base.Level.Add(new FloatText(Position + new Vector2(0f, -10f), Arrow.Names[(int) arrowType], color,
// 			// 		color2, 1f, 1f, move: false));
// 			// 	base.Level.Add(new FloatText(Position + new Vector2(0f, -3f), "ARROWS", color, color2, 1f, 1f,
// 			// 		move: false));
// 			// 	base.Level.Add(Cache.Create<LightFade>().Init(this));
// 			// 	PlaySound();
// 			// }
// 		}
//
// 		private void PlaySound()
// 		{
// 			switch (arrowType)
// 			{
// 				default:
// 					Sounds.pu_plus2Arrows.Play(base.X);
// 					break;
// 				// case ArrowTypes.Bomb:
// 				// 	Sounds.pu_bombArrow.Play(base.X);
// 				// 	break;
// 				// case ArrowTypes.Laser:
// 				// 	Sounds.pu_laserArrow.Play(base.X);
// 				// 	break;
// 				// case ArrowTypes.Bramble:
// 				// 	Sounds.pu_brambleArrow.Play(base.X);
// 				// 	break;
// 				// case ArrowTypes.Drill:
// 				// 	Sounds.pu_drill.Play(base.X);
// 				// 	break;
// 				// case ArrowTypes.Bolt:
// 				// 	Sounds.pu_boltArrow.Play(base.X);
// 				// 	break;
// 				// case ArrowTypes.SuperBomb:
// 				// 	Sounds.pu_superBomb.Play(base.X);
// 				// 	break;
// 				// case ArrowTypes.Feather:
// 				// 	Sounds.pu_feather.Play(base.X);
// 				// 	break;
// 				// case ArrowTypes.Trigger:
// 				// 	Sounds.pu_triggerArrow.Play(base.X);
// 				// 	break;
// 				// case ArrowTypes.Prism:
// 				// 	Sounds.pu_prismArrow.Play(base.X);
// 				// 	break;
// 			}
// 		}
//
// 		public override void TweenUpdate(float t)
// 		{
// 			base.TweenUpdate(t);
// 			graphic.Scale = Vector2.One * t;
// 		}
// 	}
// }
