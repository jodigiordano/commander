namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Panel : PanelWidget
    {
        public virtual bool Visible { get; set; }
        public bool ShowFrame;
        public bool ShowBackground;
        public bool ShowCloseButton;
        public override Vector3 Dimension { get; set; }
        public override double VisualPriority { get; set; }
        public PanelWidget LastClickedWidget;
        public PanelWidget LastHoverWidget;

        public List<KeyValuePair<string, PanelWidget>> Widgets { get; private set; }

        private Image Background;
        private List<Image> Corners;
        private List<Image> Edges;
        protected CloseButton CloseButton;
        private Text Title;
        private Image TitleSeparator;
        private byte backgroundAlpha;
        private byte alpha;
        private Vector2 padding;
        private Vector3 position;
        private List<PanelWidget> TitleBarWidgets;
        private Vector2 size;

        protected bool RecomputePositions;


        public Panel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
        {
            Position = position - new Vector3(size / 2f, 0);
            VisualPriority = visualPriority;
            this.size = size;

            RecomputePositions = true;

            Background = new Image("PixelBlanc", position)
            {
                Size = size,
                Color = Color.Black,
                VisualPriority = visualPriority + 0.03,
                Origin = Vector2.Zero
            };
            BackgroundAlpha = 200;

            Corners = new List<Image>();
            Edges = new List<Image>();

            for (int i = 0; i < 4; i++)
            {
                Corners.Add(new Image("bulleCoin") { VisualPriority = visualPriority + 0.0002, Color = color });
                Edges.Add(new Image("PixelBlanc") { VisualPriority = visualPriority + 0.0002, Origin = Vector2.Zero, Color = color });
            }

            Dimension = new Vector3(size, 0);

            TitleSeparator = new Image("PixelBlanc")
            {
                Color = color,
                VisualPriority = visualPriority + 0.0002,
                Origin = Vector2.Zero,
                Size = new Vector2(size.X, 5)
            };

            CloseButton = new CloseButton(scene, new Vector3(Position.X + Dimension.X - 20, Position.Y + 15, 0), visualPriority)
            {
                Sticky = true
            };

            Visible = true;

            Widgets = new List<KeyValuePair<string, PanelWidget>>();
            TitleBarWidgets = new List<PanelWidget>();

            ShowFrame = true;
            ShowBackground = true;
            ShowCloseButton = true;

            Padding = Vector2.Zero;
            Alpha = 255;

            Scene = scene; //after created CloseButton
        }


        public override void Initialize()
        {
            CloseButton.Initialize();
            CloseButton.VisualPriority = VisualPriority;
        }


        public override Scene Scene
        {
            get { return base.Scene; }
            set
            {
                base.Scene = value;
                CloseButton.Scene = value;
            }
        }


        public new Vector2 Size
        {
            get { return size; }
            set { }
        }


        public byte BackgroundAlpha
        {
            get { return backgroundAlpha; }
            set
            {
                backgroundAlpha = value;
                Background.Alpha = value;
            }
        }


        public override byte Alpha
        {
            get
            {
                return alpha;
            }

            set
            {
                Background.Alpha = Math.Min(value, BackgroundAlpha);

                foreach (var w in Widgets)
                    w.Value.Alpha = value;

                foreach (var w in TitleBarWidgets)
                    w.Alpha = value;

                foreach (var corner in Corners)
                    corner.Alpha = value;

                foreach (var edge in Edges)
                    edge.Alpha = value;

                if (Title != null)
                {
                    Title.Alpha = value;
                    TitleSeparator.Alpha = value;
                }

                CloseButton.Alpha = value;

                alpha = value;
            }
        }


        public Vector2 Padding
        {
            get { return padding; }
            set
            {
                padding = value;
                RecomputePositions = true;
            }
        }


        public override Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                RecomputePositions = true;
            }
        }


        public virtual void AddWidget(string name, PanelWidget widget)
        {
            widget.Name = name;
            widget.Scene = Scene;
            widget.Initialize();
            widget.VisualPriority = VisualPriority - 0.001;
            widget.Alpha = Alpha;

            Widgets.Add(new KeyValuePair<string, PanelWidget>(name, widget));

            RecomputePositions = true;
        }


        public void AddTitleBarWidget(PanelWidget widget)
        {
            widget.Scene = Scene;
            widget.Initialize();
            widget.VisualPriority = VisualPriority - 0.001;
            widget.Alpha = Alpha;

            TitleBarWidgets.Add(widget);

            RecomputePositions = true;
        }


        public virtual void RemoveWidget(string name)
        {
            for (int i = Widgets.Count - 1; i > -1; i--)
                if (Widgets[i].Key == name)
                {
                    if (Widgets[i].Value == LastClickedWidget)
                        LastClickedWidget = null;

                    if (Widgets[i].Value == LastHoverWidget)
                        LastHoverWidget = null;

                    Widgets.RemoveAt(i);
                    break;
                }

            RecomputePositions = true;
        }


        public PanelWidget GetWidgetByName(string name)
        {
            foreach (var w in Widgets)
                if (w.Key == name)
                    return w.Value;

            return null;
        }


        public virtual void ClearWidgets()
        {
            Widgets.Clear();

            LastClickedWidget = null;
            LastHoverWidget = null;

            RecomputePositions = true;
        }


        public virtual void SetClickHandler(string widgetName, PanelWidgetHandler handler)
        {
            foreach (var w in Widgets)
                if (w.Key == widgetName)
                {
                    w.Value.ClickHandler = handler;
                    break;
                }
        }


        public virtual void SetClickHandler(PanelWidgetHandler handler)
        {
            foreach (var w in Widgets)
                w.Value.ClickHandler = handler;
        }


        public void SetHoverHandler(string widgetName, PanelWidgetHandler handler)
        {
            foreach (var w in Widgets)
                if (w.Key == widgetName)
                {
                    w.Value.HoverHandler = handler;
                    break;
                }
        }


        public void SetHoverHandler(PanelWidgetHandler handler)
        {
            foreach (var w in Widgets)
                w.Value.HoverHandler = handler;
        }


        public PanelWidgetHandler CloseButtonHandler
        {
            set { CloseButton.ClickHandler = value; }
        }


        protected override bool Click(Circle circle)
        {
            if (ShowCloseButton && CloseButton.DoClick(circle))
            {
                LastClickedWidget = CloseButton;
                return true;
            }

            if (ClickTitleWidgets(circle))
                return true;

            if (ClickWidgets(circle))
                return true;

            return false;
        }


        protected override bool Hover(Circle circle)
        {
            if (ShowCloseButton && CloseButton.DoHover(circle))
            {
                LastHoverWidget = CloseButton;
                return true;
            }

            if (HoverTitleWidgets(circle))
                return true;

            if (HoverWidgets(circle))
                return true;

            return false;
        }


        public void SetTitle(string title)
        {
            bool adjustDimension = Title == null;

            Title = new Text(title, @"Pixelite")
            {
                SizeX = 2,
                VisualPriority = VisualPriority,
                Position = new Vector3(Position.X, Position.Y + 5, 0)
            };

            TitleSeparator.Position = new Vector3(Position.X, Position.Y + Title.AbsoluteSize.Y + 5, 0);

            if (adjustDimension)
            {
                Padding += new Vector2(0, 30);
                Dimension += new Vector3(0, Title.AbsoluteSize.Y + 10, 0);
            }
        }


        public bool OnlyShowWidgets
        {
            get { return !ShowBackground && !ShowCloseButton && !ShowFrame; }
            set
            {
                ShowFrame = !value;
                ShowBackground = !value;
                ShowCloseButton = !value;
            }
        }


        public Color Color
        {
            set
            {
                for (int i = 0; i < 4; i++)
                {
                    Edges[i].Color = value;
                    Corners[i].Color = value;
                }
            }
        }


        public override void Draw()
        {
            if (!Visible)
                return;

            if (RecomputePositions)
            {
                ComputePositions();
                ComputeTitleBarPositions();
                RecomputePositions = false;
            }

            if (ShowBackground)
            {
                Background.Size = new Vector2(Dimension.X + 8, Dimension.Y + 8);
                Background.Position = new Vector3(Position.X - 4, Position.Y - 4, 0);

                Scene.Add(Background);
            }

            if (ShowFrame)
            {
                Corners[0].Position = new Vector3(Position.X - 4, Position.Y - 4, 0); //Haut droite
                Corners[0].Rotation = 0;

                Corners[1].Position = new Vector3(Position.X - 4, Position.Y + Dimension.Y + 4, 0); //Bas droite
                Corners[1].Rotation = -MathHelper.PiOver2;

                Corners[2].Position = new Vector3(Position.X + Dimension.X + 4, Position.Y - 4, 0); //Haut gauche
                Corners[2].Rotation = MathHelper.PiOver2;

                Corners[3].Position = new Vector3(Position.X + Dimension.X + 4, Position.Y + Dimension.Y + 4, 0); //Bas gauche
                Corners[3].Rotation = MathHelper.Pi;


                Edges[0].Size = new Vector2(Dimension.X, 4); //Haut
                Edges[0].Position = new Vector3(Position.X, Position.Y - 8, 0);

                Edges[1].Size = new Vector2(Dimension.X, 4); //Bas
                Edges[1].Position = new Vector3(Position.X, Position.Y + Dimension.Y + 4, 0);

                Edges[2].Size = new Vector2(4, Dimension.Y); //Gauche
                Edges[2].Position = new Vector3(Position.X - 8, Position.Y, 0);

                Edges[3].Size = new Vector2(4, Dimension.Y); //Droite
                Edges[3].Position = new Vector3(Position.X + Dimension.X + 4, Position.Y, 0);


                for (int i = 0; i < 4; i++)
                {
                    Scene.Add(Edges[i]);
                    Scene.Add(Corners[i]);
                }
            }

            if (ShowCloseButton)
            {
                CloseButton.Draw();
            }

            if (Title != null)
            {
                Title.Position = new Vector3(Position.X, Position.Y + 5, 0);
                TitleSeparator.Position = new Vector3(Position.X, Position.Y + Title.AbsoluteSize.Y + 5, 0);

                Scene.Add(TitleSeparator);
                Scene.Add(Title);
            }

            DrawWidgets();
            DrawTitleBarWidgets();
        }


        public override void Fade(int from, int to, double length)
        {
            Visible = true;

            Scene.VisualEffects.Add(this, VisualEffects.Fade(from, to, 0, length), FadeTerminated);
        }


        protected virtual void DrawWidgets()
        {
            foreach (var w in Widgets)
                w.Value.Draw();
        }


        private void DrawTitleBarWidgets()
        {
            foreach (var w in TitleBarWidgets)
                w.Draw();
        }


        protected virtual bool ClickWidgets(Circle circle)
        {
            foreach (var w in Widgets)
                if (w.Value.DoClick(circle))
                {
                    LastClickedWidget = w.Value;
                    return true;
                }

            return false;
        }


        protected virtual bool HoverWidgets(Circle circle)
        {
            foreach (var w in Widgets)
                if (w.Value.DoHover(circle))
                {
                    LastHoverWidget = w.Value;
                    return true;
                }

            return false;
        }


        private bool ClickTitleWidgets(Circle circle)
        {
            foreach (var w in TitleBarWidgets)
                if (w.DoClick(circle))
                {
                    LastClickedWidget = w;
                    return true;
                }

            return false;
        }


        private bool HoverTitleWidgets(Circle circle)
        {
            foreach (var w in TitleBarWidgets)
                if (w.DoHover(circle))
                {
                    LastHoverWidget = w;
                    return true;
                }

            return false;
        }


        protected virtual Vector3 GetUpperLeftUsableSpace()
        {
            if (Title != null)
                return TitleSeparator.Position + new Vector3(Padding.X, Padding.Y, 0);
            else
                return new Vector3(Position.X + Padding.X, Position.Y + Padding.Y, 0);
        }


        protected virtual void ComputePositions()
        {

        }


        private void ComputeTitleBarPositions()
        {
            Vector3 upperLeft = new Vector3(CloseButton.Position.X, Position.Y - 15, 0);

            foreach (var w in TitleBarWidgets)
            {
                upperLeft.X -= w.Dimension.X + 150;

                w.Position = upperLeft;
            }
        }


        private void FadeTerminated(int id)
        {
            Visible = Background.Alpha != 0;
        }
    }
}
