﻿using System.Collections.Generic;
using System.Drawing;
using RAGE;
using RAGE.NUI;
using RPServerClient.Util;
using Player = RAGE.Elements.Player;

namespace RPServerClient
{
    class compvars
    {
        public int[] drawableid = new int[300];
        public int[] textureid = new int[300];
        public int[] paletteid = new int[300];

    }

    internal class Sandbox : Events.Script
    {
        public static int ScreenX = 0;
        public static int ScreenY = 0;

        public static int ScreenResX = 0;
        public static int ScreenResY = 0;

        private static string VERSION = null;

        private readonly MenuPool _clothespool = new MenuPool();
        UIMenu menu = new UIMenu("Clothes", "Clothes", new Point(1350, 200));
        private List<compvars> compvars = new List<compvars>();

        public Sandbox()
        {
            for (int i = 0; i < 12; i++)
            {
                compvars.Add(new compvars());
            }

            RAGE.Game.Graphics.GetScreenResolution(ref ScreenResX, ref ScreenResY);
            RAGE.Game.Graphics.GetActiveScreenResolution(ref ScreenX, ref ScreenY);


            Events.Tick += Tick;



            Events.Add("GetVersion", OnGetVersion);


            // FlyScript
            Events.Add("ToggleFlyMode", OnToggleFlyMode);

            // Chars
            Events.Add("headdata", SetHeadData);
            Events.Add("headoverlay", SetHeadOverlay);
            Events.Add("headoverlaycolor", SetHeadOverlayColor);
            Events.Add("headdata", SetHeadData);
            Events.Add("facefeautre", SetFaceFeature);
            Events.Add("compvar", SetCompVar);
            Events.Add("tpinfront", TeleportInFront);
            Events.Add("testpos", TestPos);
            Events.Add("testclothes", TestClothes);

            // Boost
            uint stamina = RAGE.Game.Misc.GetHashKey("SP0_STAMINA");
            RAGE.Game.Stats.StatSetInt(stamina, 100, true);
            uint flying = RAGE.Game.Misc.GetHashKey("SP0_FLYING");
            RAGE.Game.Stats.StatSetInt(flying, 100, true);
            uint driving = RAGE.Game.Misc.GetHashKey("SP0_DRIVING");
            RAGE.Game.Stats.StatSetInt(driving, 100, true);
            uint shooting = RAGE.Game.Misc.GetHashKey("SP0_SHOOTING");
            RAGE.Game.Stats.StatSetInt(shooting, 100, true);
            uint strength = RAGE.Game.Misc.GetHashKey("SP0_STRENGTH");
            RAGE.Game.Stats.StatSetInt(strength, 100, true);
            uint stealth = RAGE.Game.Misc.GetHashKey("SP0_STEALTH");
            RAGE.Game.Stats.StatSetInt(stealth, 100, true);
            uint lungCapacity = RAGE.Game.Misc.GetHashKey("SP0_LUNGCAPACITY");
            RAGE.Game.Stats.StatSetInt(lungCapacity, 100, true);

            _clothespool.Add(menu);

            var comps = new List<dynamic>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            var drawables = new List<dynamic>();
            var textures = new List<dynamic>();
            var palletes = new List<dynamic>();

            for (int i = 0; i < 300; i++)
            {
                drawables.Add(i);
                textures.Add(i);
                palletes.Add(i);
            }

            var compsMenu = new UIMenuListItem("CompID", comps, 0);
            menu.AddItem(compsMenu);

            var drawablesMenu = new UIMenuListItem("DrawableID", drawables, 0);
            menu.AddItem(drawablesMenu);

            var texturesMenu = new UIMenuListItem("TextureID", textures, 0);
            menu.AddItem(texturesMenu);

            var palletesMenu = new UIMenuListItem("PalleteID", palletes, 0);
            menu.AddItem(palletesMenu);


            menu.OnListChange += (sender, item, index) =>
            {
                if (item == compsMenu)
                {
                    drawablesMenu.Index = compvars[compsMenu.Index].drawableid[compsMenu.Index];
                    texturesMenu.Index = compvars[compsMenu.Index].textureid[compsMenu.Index];
                    palletesMenu.Index = compvars[compsMenu.Index].paletteid[compsMenu.Index];
                    return;
                }

                compvars[compsMenu.Index].drawableid[compsMenu.Index] = drawablesMenu.Index;
                compvars[compsMenu.Index].textureid[compsMenu.Index] = texturesMenu.Index;
                compvars[compsMenu.Index].paletteid[compsMenu.Index] = palletesMenu.Index;
                RAGE.Chat.Output($"{comps[compsMenu.Index]}, {drawables[drawablesMenu.Index]}, {textures[texturesMenu.Index]}, {palletes[palletesMenu.Index]}");
                Player.LocalPlayer.SetComponentVariation(comps[compsMenu.Index], drawables[drawablesMenu.Index], textures[texturesMenu.Index], palletes[palletesMenu.Index]);
            };

        }

        private void TestClothes(object[] args)
        {
            _clothespool.RefreshIndex();
            menu.Visible = true;
        }

        private void TestPos(object[] args)
        {
            var vect = Helper.GetPosInFrontOfVector3(new Vector3((float)args[0], (float)args[1], (float)args[2]), (float)args[3], 1);
            RAGE.Chat.Output(vect.ToString());
        }

        private void OnGetVersion(object[] args)
        {
            if (args == null || args.Length < 1) return;
            VERSION = args[0].ToString();
        }


        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (VERSION != null)
            {
                RAGE.Game.Ui.SetTextOutline();
                RAGE.Game.UIText.Draw(VERSION, new Point(ScreenResX / 2, ScreenResY - (int)(ScreenResY * 0.03)), 0.35f, Color.White, RAGE.Game.Font.ChaletLondon, true);
            }

            _clothespool?.ProcessMenus();
        }

        private void TeleportInFront(object[] args)
        {
            var pos = Helper.GetPosInFrontOfPlayer(Player.LocalPlayer, 2.0f);
            Player.LocalPlayer.Position = pos;
        }

        private void OnToggleFlyMode(object[] args) => Events.CallLocal("flyModeStart");


        private void SetCompVar(object[] args)
        {
            Player.LocalPlayer.SetComponentVariation((int)args[0], (int)args[1], (int)args[2], (int)args[3]);
        }

        private void SetFaceFeature(object[] args)
        {
            Player.LocalPlayer.SetFaceFeature((int)args[0], (float)args[1]);
        }

        private void SetHeadOverlay(object[] args)
        {
            Player.LocalPlayer.SetHeadOverlay((int)args[0], (int)args[1], (float)args[2]);
        }

        private void SetHeadOverlayColor(object[] args)
        {
            RAGE.Chat.Output("Triggert/crted");
            Player.LocalPlayer.SetHeadOverlayColor((int)args[0], (int)args[1], (int)args[2], (int)args[3]);
        }

        private void SetHeadData(object[] args)
        {
            if (args.Length < 9) return;

            Player.LocalPlayer.SetHeadBlendData((int)args[0], (int)args[1], (int)args[2], (int)args[3], (int)args[4], (int)args[5], (float)args[6], (float)args[7], (float)args[8], (bool)args[9]);
        }
    }
}
