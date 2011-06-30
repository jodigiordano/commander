namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class OptionsScene : Scene
    {
        private Text MusicTitle;
        private HorizontalSlider Music;
        private Text SoundEffectsTitle;
        private HorizontalSlider SoundEffects;
        private Dictionary<Player, Cursor> Cursors;

        private Image Title;
        private Image Background;
        private Image Bubble;
        private Image Commodore;

        private static string Credits = "Hello, my name is Jodi Giordano. I glued all this stuff together. Special thanks to my supporting friends and my family, the LATECE laboratory crew, UQAM, Mercury Project and SFXR. If you want more info about me and my games, please visit ephemeregames.com... Now get back to work, commander!";
        private TextTypeWriter TypeWriter;

        private double TimeBetweenTwoMusics;


        public OptionsScene()
            : base(Vector2.Zero, 1280, 720)
        {
            Name = "Options";

            Title = new Image("options", new Vector3(-550, -150, 0));
            Title.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;
            Title.Origin = Vector2.Zero;

            MusicTitle = new Text
                    (
                        "Music",
                        "Pixelite",
                        Color.White,
                        new Vector3(-420, 130, 0)
                    );
            MusicTitle.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;
            MusicTitle.SizeX = 2f;

            SoundEffectsTitle = new Text
                    (
                        "Sound Effects",
                        "Pixelite",
                        Color.White,
                        new Vector3(-420, 200, 0)
                    );
            SoundEffectsTitle.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;
            SoundEffectsTitle.SizeX = 2f;

            Cursors = new Dictionary<Player, Cursor>();

            foreach (var player in Inputs.ConnectedPlayers)
                Cursors.Add((Player) player, new Cursor(this, Vector3.Zero, 10, Preferences.PrioriteGUIMenuPrincipal));

            Music = new HorizontalSlider(this, new Vector3(-120, 140, 0), 0, 10, 5, 1, Preferences.PrioriteGUIMenuPrincipal + 0.01f);
            SoundEffects = new HorizontalSlider(this, new Vector3(-120, 210, 0), 0, 10, 5, 1, Preferences.PrioriteGUIMenuPrincipal + 0.01f);

            Commodore = new Image("lieutenant", new Vector3(-75, -220, 0));
            Commodore.SizeX = 8;
            Commodore.Rotation = MathHelper.Pi;
            Commodore.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            Bubble = new Image("bulleRenversee", new Vector3(80, -150, 0));
            Bubble.SizeX = 8;
            Bubble.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.02f;
            Bubble.Origin = Vector2.Zero;

            Background = new Image("fondecran12", Vector3.Zero);
            Background.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.03f;

            TypeWriter = new TextTypeWriter
            (
                Credits,
                Color.Black,
                new Vector3(170, -130, 0),
                "Pixelite",
                2.0f,
                new Vector2(370, 330),
                50,
                true,
                1000,
                true,
                new List<string>()
                {
                    "sfxLieutenantParle1",
                    "sfxLieutenantParle2",
                    "sfxLieutenantParle3",
                    "sfxLieutenantParle4"
                },
                this
            );
            TypeWriter.Text.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            TimeBetweenTwoMusics = 0;
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            Main.SaveGame.VolumeMusic = Music.Valeur;
            Main.SaveGame.VolumeSfx = SoundEffects.Valeur;

            Audio.MusicVolume = Music.Valeur / 10f;
            Audio.SfxVolume = SoundEffects.Valeur / 10f;

            TimeBetweenTwoMusics -= gameTime.ElapsedGameTime.TotalMilliseconds;
            TypeWriter.Update(gameTime);
        }


        protected override void UpdateVisual()
        {
            Add(MusicTitle);
            Add(SoundEffectsTitle);
            Add(Title);
            Add(Commodore);
            Add(Bubble);
            Add(Background);
            Add(TypeWriter.Text);

            foreach (var c in Cursors.Values)
                c.Draw();
            
            Music.Draw();
            SoundEffects.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            SyncPlayers();

            Music.Valeur = Main.SaveGame.VolumeMusic;
            SoundEffects.Valeur = Main.SaveGame.VolumeSfx;
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            Persistence.SaveData("savePlayer");
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (button == MouseConfiguration.Select)
            {
                Player player = (Player) p;

                Music.DoClick(player.Circle);
                SoundEffects.DoClick(player.Circle);
            }

            if (button == MouseConfiguration.Back)
                BeginTransition();
        }


        public override void DoMouseMoved(Core.Input.Player p, Vector3 delta)
        {
            Player player = (Player) p;

            player.Move(ref delta, MouseConfiguration.Speed);
            Cursors[player].Position = player.Position;
        }


        public override void DoGamePadJoystickMoved(Core.Input.Player p, Buttons button, Vector3 delta)
        {
            Player player = (Player) p;

            if (button == GamePadConfiguration.MoveCursor)
            {
                player.Move(ref delta, GamePadConfiguration.Speed);
                Cursors[player].Position = player.Position;
            }
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            if (key == KeyboardConfiguration.Disconnect)
            {
                Inputs.DisconnectPlayer(p);
                return;
            }

            if (key == KeyboardConfiguration.Back || key == KeyboardConfiguration.Cancel)
                BeginTransition();

            if (key == KeyboardConfiguration.ChangeMusic)
                Main.ChangeMusic();
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (button == GamePadConfiguration.Disconnect)
            {
                Inputs.DisconnectPlayer(p);
                return;
            }


            if (button == GamePadConfiguration.Select)
            {
                Player player = (Player) p;

                Music.DoClick(player.Circle);
                SoundEffects.DoClick(player.Circle);
            }

            if (button == GamePadConfiguration.Cancel)
                BeginTransition();

            if (button == GamePadConfiguration.ChangeMusic)
                Main.ChangeMusic();
        }


        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            Cursors.Remove((Player) player);

            if (Inputs.ConnectedPlayers.Count == 0)
                TransiteTo("Chargement");
        }


        public override void PlayerConnectionRequested(Core.Input.Player player)
        {
            player.Connect();
        }


        public override void DoPlayerConnected(Core.Input.Player player)
        {
            Cursors.Add((Player) player, new Cursor(this, Vector3.Zero, 10, Preferences.PrioriteGUIMenuPrincipal));
        }


        private void BeginTransition()
        {
            TransiteTo("Menu");
        }


        private void SyncPlayers()
        {
            foreach (var p in Inputs.Players)
            {
                Player player = (Player) p;

                if (p.State == PlayerState.Connected && !Cursors.ContainsKey(player))
                    DoPlayerConnected(player);
                else if (p.State == PlayerState.Disconnected && Cursors.ContainsKey(player))
                    DoPlayerDisconnected(player);
            }
        }
    }
}
