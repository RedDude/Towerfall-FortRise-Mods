using System;
using System.Xml;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using TowerFall;

namespace TeamOutlineVariant
{
    public class TeamOutlineComponent : Component
    {
      
        private ParticleType fireHair;
        
        private Color _color;
        private int _offset = 2;
        private static string _outline = "_outline";

        private Subtexture bodyOutline;
        private Subtexture bowOutline;
        private Subtexture headOutline;
        private Subtexture headOutlineNotHat;
        private Subtexture headOutlineCrown;
        
        private Subtexture originalBow;
        private Subtexture originalBody;
        private Subtexture originalHead;
        private Subtexture originalNotHat;
        private Subtexture originalHeadCrown;
        
        private Sprite<string> bodySprite;
        private Sprite<string> headSprite;
        private Sprite<string> bowSprite;

        public TeamOutlineComponent(Color color, int offset, bool active, bool visible) : base(active, visible)
        {
            _color = color;
            _offset = offset;
        }

        public override void Added()
        {
            var player = ((Player) Parent);
            if(player.Allegiance == Allegiance.Neutral)
            {
                Visible = false;
                Parent.Remove(this);
                return;
            }
            
            var spriteDataBody = TFGame.SpriteData.GetXML(player.ArcherData.Sprites.Body);
            var spriteDataBow = TFGame.SpriteData.GetXML(player.ArcherData.Sprites.Bow);
            var spriteDataHeadNoHat = TFGame.SpriteData.GetXML(player.ArcherData.Sprites.HeadNoHat);
            var spriteDataHeadCrown = TFGame.SpriteData.GetXML(player.ArcherData.Sprites.HeadCrown);
            var spriteDataHeadNormal = TFGame.SpriteData.GetXML(player.ArcherData.Sprites.HeadNormal);

            CheckSprite(spriteDataBody, ref bodyOutline, ref originalBody);
            CheckSprite(spriteDataHeadNormal,ref headOutline, ref originalHead);
            CheckSprite(spriteDataHeadNoHat, ref headOutlineNotHat, ref originalNotHat);
            CheckSprite(spriteDataHeadCrown, ref headOutlineCrown, ref originalHeadCrown);
            CheckSprite(spriteDataBow, ref bowOutline, ref originalBow);
            
            bodySprite = DynamicData.For(player).Get<Sprite<string>>("bodySprite");
            headSprite = DynamicData.For(player).Get<Sprite<string>>("headSprite");
            bowSprite = DynamicData.For(player).Get<Sprite<string>>("bowSprite");
        }

        public void CheckSprite(XmlElement element, ref Subtexture outline, ref Subtexture original)
        {
            var texture = element.ChildText("Texture");
            outline = TeamOutlineVariant.outlines[texture+_outline] 
                      ?? TFGame.Atlas[texture + _outline];

            if (outline == null)
            {
                original = null;
                return;
            }

            original = TFGame.Atlas[texture];
        }
        
        public void SwapSprite(Sprite<string> sprite, Subtexture outline, Subtexture original)
        {
            if(outline == null || original == null)
                return;
            sprite.SwapSubtexture(outline);
            sprite.DrawOutline(_color, _offset);
            sprite.SwapSubtexture(original);
            // sprite.DrawOutline(Color.Black);
        }

        public override void Render()
        {
            var player = ((Player) Parent);
            if(player.Allegiance == Allegiance.Neutral)
                return;
            
  
            foreach (var component in player.Components)
            {
                if (component is not Sprite<string> sprite) continue;
                if (!sprite.Visible) continue;
                if (bodySprite == component)
                {
                    SwapSprite(sprite, bodyOutline, originalBody);
                    continue;
                }
                if (headSprite == component)
                {
                    if(player.HatState == Player.HatStates.Normal)
                    {
                        SwapSprite(sprite, headOutline, originalHead);
                        continue;
                    }
                    if(player.HatState == Player.HatStates.NoHat)
                    {
                        SwapSprite(sprite, headOutlineNotHat, originalNotHat);
                        continue;
                    }
                    if(player.HatState == Player.HatStates.Crown)
                    {
                        SwapSprite(sprite, headOutlineCrown, originalHeadCrown);
                        continue;
                    }
                }
                if (bowSprite == component)
                {
                    SwapSprite(sprite, bowOutline, originalBow);
                    continue;
                }
            }

            base.Render();
        }
    }
}


// fireHair = new ParticleType
// {
//     Source = TFGame.Atlas["fireParticle"],
//     ColorSwitch = 12,
//     ColorSwitchLoop = false,
//     Size = 0.5f,
//     SizeRange = 0.25f,
//     Speed = 0.14f,
//     SpeedRange = 0.1f,
//     Direction = -(float)Math.PI / 2f,
//     DirectionRange = (float)Math.PI / 6f,
//     Life = 28,
//     LifeRange = 10,
//     ScaleOut = true,
//     Color = (Particles.Fire.Color = Calc.HexToColor("FCF353")),
//     Color2 = (Particles.Fire.Color2 = Calc.HexToColor("F83800"))
// };
// public override void Update()
// {
// var player = ((Player) Parent);
// var bodySprite = DynamicData.For(player).Get<Sprite<string>>("bodySprite");
// var headSprite = DynamicData.For(player).Get<Sprite<string>>("headSprite");
// var bowSprite = DynamicData.For(player).Get<Sprite<string>>("bowSprite");
//
// headSprite.Visible = false;
// // bodySprite.Visible = false;
// bowSprite.Visible = false;
// if(player.Level.OnInterval(3))
// {
//     player.Level.Particles.Emit(fireHair, 2, player.Position + headSprite.Position + new Vector2(0f, -8f), new Vector2(2f, 0f));
//     // base.Level.ParticlesBG.Emit(Particles.BGTorch, 1, Position + new Vector2(0f, 8f), new Vector2(1f, 0f));
// }
            
// base.Update();
// }
// player.Level.Particles.Emit(fireHair, 1, headSprite.Position + new Vector2(0f, 8f), new Vector2(1f, 0f));
// if (player.Hair)
// {
//     player.Hair.RenderOutline()(Color.Black, 2);
// }