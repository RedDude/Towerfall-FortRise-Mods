using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.Utils;
using TowerFall;

namespace VariantsPickupMod
{
    public class VariantPickup : Pickup
    {
        private Image image;
        
        public string Title;

        public string Description;

        public Image Image;

        private Wiggler scaleWiggler;

        private Wiggler rotateWiggler;

        private SineWave selectionSine;

        private float bubbleShow;

        private Canvas bubbleCanvas;

        private SineWave bubbleSine;

        protected bool explain;

        private float explainShow;

        private Canvas explainCanvas;
        
        public Variant Variant;

        public bool IsLoseOnDeath;
        
        public bool IsLoseOnEndRound;

        public VariantPickup(Variant variant, Vector2 position, Vector2 targetPosition)
            : base(position, targetPosition)
        {
            Variant = variant;
            // this.canActivate = canActivate;
            
            Collider = new Hitbox(16f, 16f, -8f, -8f);
            Tag(GameTags.PlayerCollectible);
            image = new Image(variant.Icon);
            image.CenterOrigin();
            Add(image);
        }

        public override void Update()
        {
            base.Update();
            if (Collidable)
            {
                image.Scale.X = 1f + 0.05f * sine.ValueOverTwo;
                image.Scale.Y = 1f + 0.05f * sine.Value;
            }

            image.Position = DrawOffset;
        }

        public override void Render()
        {
            DrawGlow();
            image.Render();
            image.DrawOutline();
            // int num = (int)Math.Ceiling(TFGame.Font.MeasureString(Title).X / 10f) + 1;
            // if (bubbleCanvas != null)
            // {
            //     bubbleCanvas.RenderTarget2D.Dispose();
            // }
            // bubbleCanvas = new Canvas(num * 10, 14);
            // Draw.BeginCanvas(bubbleCanvas);
            // Draw.Texture(TFGame.MenuAtlas["variants/bubbleEdge"], Vector2.Zero, Color.White, Vector2.Zero, Vector2.One);
            // for (int i = 1; i < num - 1; i++)
            // {
            //     Draw.Texture(TFGame.MenuAtlas["variants/bubbleMiddle"], new Vector2(i * 10, 0f), Color.White, Vector2.Zero, Vector2.One);
            // }
            // Draw.Texture(TFGame.MenuAtlas["variants/bubbleEdge"], new Vector2(bubbleCanvas.Width - 10, 0f), Color.White, Vector2.Zero, 1f, 0f, SpriteEffects.FlipHorizontally);
            // Draw.TextCentered(TFGame.Font, Title, new Vector2(bubbleCanvas.Width / 2, 7f), Color.Black);
            // Draw.SpriteBatch.End();
            // if (!string.IsNullOrEmpty(Description))
            // {
            //     int num2 = (int)Math.Ceiling(TFGame.Font.MeasureString(Description).X / 10f) + 1;
            //     if (explainCanvas == null)
            //     {
            //         explainCanvas = new Canvas(num2 * 10, 14);
            //     }
            //     Draw.BeginCanvas(explainCanvas);
            //     Draw.Texture(TFGame.MenuAtlas["variants/bubbleEdge"], Vector2.Zero, Color.White, Vector2.Zero, Vector2.One);
            //     for (int j = 1; j < num2 - 1; j++)
            //     {
            //         Draw.Texture(TFGame.MenuAtlas["variants/bubbleMiddle"], new Vector2(j * 10, 0f), Color.White, Vector2.Zero, Vector2.One);
            //     }
            //     Draw.Texture(TFGame.MenuAtlas["variants/bubbleEdge"], new Vector2(explainCanvas.Width - 10, 0f), Color.White, Vector2.Zero, 1f, 0f, SpriteEffects.FlipHorizontally);
            //     Draw.TextCentered(TFGame.Font, Description, new Vector2(explainCanvas.Width / 2, 7f), Color.Black);
            //     Draw.SpriteBatch.End();
            // }
            base.Render();
        }

        public override void OnPlayerCollide(Player player)
        {
            var variants = player.Level.Session.MatchSettings.Variants;
            var matchVariant = variants.Variants.FirstOrDefault(v => v.Title == Variant.Title);
            
            if (matchVariant == null)
            {
                matchVariant = variants.GetCustomVariant(Variant.Title);
                if (matchVariant == null)
                    return;
            }

            if(matchVariant[player.PlayerIndex])
                return;
            
            matchVariant[player.PlayerIndex] = true;

            VariantPickupManager.HandleBigHead(player, matchVariant);
            
            Level.Add(Cache.Create<LightFade>().Init(this));
            for (int i = 0; i < 30; i++)
            {
                if (IsLoseOnDeath && IsLoseOnEndRound)
                {
                    Level.Particles.Emit(Particles.PickupSparkle, 1, Position, Vector2.One * 3f,
                        (float) Math.PI * 2f * (float) i / 30f);
                }else if (IsLoseOnDeath)
                {
                    Level.Particles.Emit(Particles.PurpleAmbience, 1, Position, Vector2.One * 3f,
                        (float) Math.PI * 2f * (float) i / 30f);
                }else if (IsLoseOnEndRound)
                {
                    Level.Particles.Emit(Particles.PurpleAmbience, 1, Position, Vector2.One * 3f,
                        (float) Math.PI * 2f * (float) i / 30f);
                }
                Level.Particles.Emit(Particles.SpeedBootsPickup, 1, Position, Vector2.One * 3f,
                    (float) Math.PI * 2f * (float) i / 30f);
            
            }

            Level.Particles.Emit(Particles.SpeedBootsPickup2, 18, Position, Vector2.One * 4f);
            
            VariantPickupManager.AddVariantPickup(player.PlayerIndex, this);
            // DoCollectStats(player.PlayerIndex);
            RemoveSelf();
        }

     

        public override void TweenUpdate(float t)
        {
            base.TweenUpdate(t);
            image.Scale = Vector2.One * t;
        }

        public override void DoPlayerCollect(Player player)
        {
            Sounds.pu_speedBoots.Play(X);
            player.HasSpeedBoots = true;
        }
    }
}