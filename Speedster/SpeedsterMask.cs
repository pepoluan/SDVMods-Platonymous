﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomElementHandler;
using System.Runtime.CompilerServices;

namespace Speedster
{
    internal class SpeedsterMask : Hat, ISaveElement
    {

        private static bool wearing;
        private static int oldHair;
        private static int oldShirt;
        private static int oldShoes;
        private static Microsoft.Xna.Framework.Color oldPants;
        private static Texture2D oldShirtTexture;
        private static Texture2D shirts;
        public int index;
        public static int baseIndex;
        public static bool setup = false;
        public static bool hyperdrive = false;


        public SpeedsterMask()

        {

        }


        public SpeedsterMask(int index)
 
        {
            build(index);
        }

        private void build(int index)
        {
            this.index = index;
            shirts = loadTexture("shirts.png");
            oldPants = Game1.player.pantsColor;
            oldShirt = Game1.player.shirt;
            oldHair = Game1.player.hair;
            if (Game1.player.boots != null)
            {
                oldShoes = Game1.player.boots.indexInColorSheet;
            }
            else
            {
                oldShoes = 12;
            }

            string path = Path.Combine(SpeedsterMod.ModHelper.DirectoryPath, "Assets", "masks.png");
            if (!setup)
            {
                baseIndex = setTextures(path, 20, 20 * 4);
                setup = true;
            }

            which = baseIndex + index;
            skipHairDraw = true;
            
            name = "Speedster Mask";
            if(index == 1)
            {
                name = "Reverse Speedster Mask";
            }

            description = "Powerful Disguise";
            ignoreHairstyleOffset = true;
            category = -95;
        }

        public static void putOnCostume(int index)
        {
            if (!wearing)
            {
                Game1.player.hair = -1;
                oldShirtTexture = FarmerRenderer.shirtsTexture;
                FarmerRenderer.shirtsTexture = shirts;
                if (index == 0)
                {
                    Game1.player.changePants(Microsoft.Xna.Framework.Color.Red);
                }

                if (index == 1)
                {
                    Game1.player.changePants(Microsoft.Xna.Framework.Color.Yellow);
                }

                Game1.player.FarmerRenderer.changeShirt(index);
                Game1.player.shirt = index;
                Game1.player.FarmerRenderer.recolorShoes(0);
                wearing = true;
            }

            if (wearing)
            {
                if (hyperdrive)
                {
                    Game1.player.addedSpeed = Math.Max(24, Game1.player.addedSpeed);
                }
                else
                {
                    Game1.player.addedSpeed = Math.Max(6, Game1.player.addedSpeed);
                }
               
            }

        }

        public static void takeOffCostume()
        {
        
                if (wearing)
            {
                Game1.player.hair = oldHair;
                FarmerRenderer.shirtsTexture = oldShirtTexture;
                Game1.player.changePants(oldPants);
                Game1.player.FarmerRenderer.changeShirt(oldShirt);
                Game1.player.shirt = oldShirt;
                Game1.player.FarmerRenderer.recolorShoes(oldShoes);
                wearing = false;
                Game1.player.addedSpeed = 0;
            }
        }


        private Texture2D loadTexture(string file)
        {
            string path = Path.Combine(SpeedsterMod.ModHelper.DirectoryPath, "Assets", file);
            Image textureImage = Image.FromFile(path);
            Texture2D texture = Bitmap2Texture(new Bitmap(textureImage));
            return texture;
        }


        public int setTextures(string path, int width, int height)
        {

            Bitmap orgSpriteFirstLoad = this.Texture2Image(FarmerRenderer.hatsTexture);
            Texture2D orgSpriteSecondLoad = this.Bitmap2Texture(orgSpriteFirstLoad);
            Image orgSprite = this.Texture2Image(orgSpriteSecondLoad);
            int osH = orgSprite.Height;
            int osW = orgSprite.Width;
            
            Image newSprite = Image.FromFile(path);
            int nsH = newSprite.Height;

            Bitmap combSprite = this.combineImages(orgSprite, newSprite, 0);
            Texture2D fullTex = this.Bitmap2Texture(combSprite);
            Bitmap fullSprite = this.Texture2Image(fullTex);
            FarmerRenderer.hatsTexture = fullTex;

            combSprite.Dispose();
            fullSprite.Dispose();

            return (osW / width) * ((osH) / height);




        }

        private Texture2D Bitmap2Texture(Bitmap bmp)
        {
            MemoryStream s = new MemoryStream();

            bmp.Save(s, System.Drawing.Imaging.ImageFormat.Png);
            s.Seek(0, SeekOrigin.Begin);
            Texture2D tx = Texture2D.FromStream(Game1.graphics.GraphicsDevice, s);

            return tx;

        }

        private Bitmap Texture2Image(Texture2D tex)
        {
            Texture2D texture = tex;
            byte[] textureData = new byte[4 * texture.Width * texture.Height];
            texture.GetData<byte>(textureData);

            Bitmap bmp = new Bitmap(
                           texture.Width, texture.Height,
                           System.Drawing.Imaging.PixelFormat.Format32bppArgb
                         );

            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(
                           new System.Drawing.Rectangle(0, 0, texture.Width, texture.Height),
                           System.Drawing.Imaging.ImageLockMode.WriteOnly,
                           System.Drawing.Imaging.PixelFormat.Format32bppArgb
                         );

            IntPtr safePtr = bmpData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(textureData, 0, safePtr, textureData.Length);
            bmp.UnlockBits(bmpData);

            return bmp;

        }

        private Bitmap combineImages(Image img1, Image img2, int offset)
        {

            int height = img1.Height + img2.Height + offset;
            int width = Math.Max(img1.Width, img2.Width);

            Bitmap img3 = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(img3);

            g.Clear(System.Drawing.Color.Transparent);
            g.DrawImage(img1, new System.Drawing.Point(0, 0));
            g.DrawImage(img2, new System.Drawing.Point(0, img1.Height + offset));

            g.Dispose();
            img1.Dispose();
            img2.Dispose();

            return img3;

        }

        public override string getDescription()
        {
            return description;
        }

        public override string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public override void actionWhenBeingHeld(Farmer who)
        {
            base.actionWhenBeingHeld(who);
        }
 
        public override Item getOne()
        {
            return new SpeedsterMask(index);
        }

        
        public dynamic getReplacement()
        {
            return new Hat(index);
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string,string> savedata = new Dictionary<string, string>();
            savedata.Add("index", index.ToString());
            return savedata;
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            build(int.Parse(additionalSaveData["index"]));
        }
    }
}