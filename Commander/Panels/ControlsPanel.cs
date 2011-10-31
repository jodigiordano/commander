namespace EphemereGames.Commander
{
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class ControlsPanel : SlideshowPanel
    {
        private HBMessageConstructor MessageConstructor;


        public ControlsPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            SetTitle("Controls");

            Slider.SpaceForValue = 400;

            Alpha = 0;

            MessageConstructor = new HBMessageConstructor();

            AddWidget("keyboardAndMouse", CreateSubPanel(InputType.MouseAndKeyboard));
            AddWidget("gamepad", CreateSubPanel(InputType.Gamepad));

            Slider.AddAlias(0, "Keyboard and Mouse");
            Slider.AddAlias(1, "Gamepad");
        }


        private VerticalPanel CreateSubPanel(InputType inputType)
        {
            var v = CreateEmptySubPanel();

            var spaceship = new Label(new Text("Spaceship", "Pixelite") { SizeX = 2, Alpha = 0 });

            var move = new ImageLabel(
                MessageConstructor.CreateMoveImage(inputType, Main.InputsFactory.GetDefaultConfiguration()),
                new Text("Move", "Pixelite") { SizeX = 2, Alpha = 0 });
            var fire = new ImageLabel(
                MessageConstructor.CreateFireImage(inputType, Main.InputsFactory.GetDefaultConfiguration()),
                new Text("Fire (hold)", "Pixelite") { SizeX = 2, Alpha = 0 });
            
            var menus = new Label(new Text("Menus", "Pixelite") { SizeX = 2, Alpha = 0 });

            var toggle = new ImageLabel(
                MessageConstructor.CreateToggleImage(inputType, Main.InputsFactory.GetDefaultConfiguration()),
                new Text("Toggle choices", "Pixelite") { SizeX = 2, Alpha = 0 });
            var accept = new ImageLabel(
                MessageConstructor.CreateSelectImage(inputType, Main.InputsFactory.GetDefaultConfiguration()),
                new Text("Accept", "Pixelite") { SizeX = 2, Alpha = 0 });
            var cancel = new ImageLabel(
                MessageConstructor.CreateCancelImage(inputType, Main.InputsFactory.GetDefaultConfiguration()),
                new Text("Cancel", "Pixelite") { SizeX = 2, Alpha = 0 });

            
            var other = new Label(new Text("Other", "Pixelite") { SizeX = 2, Alpha = 0 });

            var zoom = new ImageLabel(
                MessageConstructor.CreateZoomImage(inputType, Main.InputsFactory.GetDefaultConfiguration()),
                new Text("Zoom in / Zoom out", "Pixelite") { SizeX = 2, Alpha = 0 });
            var back = new ImageLabel(
                MessageConstructor.CreateBackImage(inputType, Main.InputsFactory.GetDefaultConfiguration()),
                new Text("Go back / Pause game", "Pixelite") { SizeX = 2, Alpha = 0 });
            var disconnect = new ImageLabel(
                MessageConstructor.CreateDisconnectImage(inputType, Main.InputsFactory.GetDefaultConfiguration()),
                new Text("Disconnect", "Pixelite") { SizeX = 2, Alpha = 0 });

            move.SpaceForImage =
            fire.SpaceForImage =
            toggle.SpaceForImage =
            accept.SpaceForImage =
            cancel.SpaceForImage =
            back.SpaceForImage =
            zoom.SpaceForImage =
            disconnect.SpaceForImage = 200;

            v.AddWidget("spaceship", spaceship);
            v.AddWidget("move", move);
            v.AddWidget("fire", fire);
            v.AddWidget("Seperator1", new VerticalSeparatorWidget() { MaxAlpha = 0 });
            v.AddWidget("menus", menus);
            v.AddWidget("toggle", toggle);
            v.AddWidget("accept", accept);
            v.AddWidget("cancel", cancel);
            v.AddWidget("Seperator2", new VerticalSeparatorWidget() { MaxAlpha = 0 });
            v.AddWidget("other", other);
            v.AddWidget("zoom", zoom);
            v.AddWidget("back", back);
            v.AddWidget("disconnect", disconnect);

            return v;
        }


        private VerticalPanel CreateEmptySubPanel()
        {
            var v = new VerticalPanel(Scene, Vector3.Zero, Size, VisualPriority, Color);

            v.OnlyShowWidgets = true;
            v.DistanceBetweenTwoChoices = 15;
            v.Padding += new Vector2(30, 0);

            return v;
        }
    }
}
