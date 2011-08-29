namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    abstract class SequenceCheat : Cheat, InputListener
    {
        private enum Status
        {
            None,
            Trying,
            Failed,
            Succeed
        }

        protected double TimeToExecute;
        protected List<Keys> SequenceMouse;
        protected List<Buttons> SequenceGamepad;


        private Status CurrentStatus;
        private double ElapsedTime;
        private int CurrentSequenceId;


        public SequenceCheat(string name)
            : base(name)
        {
            TimeToExecute = 0;
            SequenceMouse = new List<Keys>();
            SequenceGamepad = new List<Buttons>();
            CurrentSequenceId = 0;
            CurrentStatus = Status.None;
        }


        public override void Initialize()
        {
            Inputs.AddListener(this);
        }


        public override bool Active
        {
            get
            {
                return CurrentStatus == Status.Succeed;
            }
        }


        public override void Update()
        {
            base.Update();

            if (Active)
                return;

            if (CurrentStatus == Status.None)
                return;

            ElapsedTime += Preferences.TargetElapsedTimeMs;

            if (ElapsedTime > TimeToExecute)
                CurrentStatus = Status.Failed;

            if (CurrentStatus == Status.Failed)
            {
                if (ElapsedTime > TimeToExecute)
                    CurrentStatus = Status.None;

                return;
            }
        }


        bool InputListener.EnableInputs
        {
            get { return true; }
        }


        void InputListener.DoKeyPressed(Core.Input.Player player, Keys key)
        {
            
        }


        void InputListener.DoKeyPressedOnce(Core.Input.Player player, Keys key)
        {
            if (CurrentStatus == Status.Failed || CurrentStatus == Status.Succeed)
                return;

            if (CurrentStatus == Status.None)
            {
                CurrentStatus = Status.Trying;
                ElapsedTime = 0;
                CurrentSequenceId = 0;
            }

            if (key == SequenceMouse[CurrentSequenceId])
            {
                CurrentSequenceId++;

                if (CurrentSequenceId == SequenceMouse.Count)
                {
                    CurrentStatus = Recurrent ? Status.None : Status.Succeed;
                    SetActivatedThisTick();
                }
            }

            else
                CurrentStatus = Status.Failed;
        }


        void InputListener.DoKeyReleased(Core.Input.Player player, Keys key)
        {
            
        }


        void InputListener.DoMouseButtonPressed(Core.Input.Player player, MouseButton button)
        {
            
        }


        void InputListener.DoMouseButtonPressedOnce(Core.Input.Player player, MouseButton button)
        {
            
        }


        void InputListener.DoMouseButtonReleased(Core.Input.Player player, MouseButton button)
        {
            
        }


        void InputListener.DoMouseScrolled(Core.Input.Player player, int delta)
        {
            
        }


        void InputListener.DoMouseMoved(Core.Input.Player player, Vector3 delta)
        {
            
        }


        void InputListener.DoGamePadButtonPressedOnce(Core.Input.Player player, Buttons button)
        {
            if (CurrentStatus == Status.Failed || CurrentStatus == Status.Succeed)
                return;

            if (CurrentStatus == Status.None)
            {
                CurrentStatus = Status.Trying;
                ElapsedTime = 0;
                CurrentSequenceId = 0;
            }

            if (button == SequenceGamepad[CurrentSequenceId])
            {
                CurrentSequenceId++;

                if (CurrentSequenceId == SequenceGamepad.Count)
                {
                    CurrentStatus = Status.Succeed;
                    SetActivatedThisTick();
                }
            }

            else
                CurrentStatus = Status.Failed;
        }


        void InputListener.DoGamePadButtonReleased(Core.Input.Player player, Buttons button)
        {
            
        }


        void InputListener.DoGamePadJoystickMoved(Core.Input.Player player, Buttons button, Vector3 delta)
        {
            
        }


        void InputListener.PlayerKeyboardConnectionRequested(Core.Input.Player Player, Keys key)
        {
            
        }


        void InputListener.PlayerMouseConnectionRequested(Core.Input.Player Player, MouseButton button)
        {
            
        }


        void InputListener.PlayerGamePadConnectionRequested(Core.Input.Player Player, Buttons button)
        {
            
        }


        void InputListener.DoPlayerConnected(Core.Input.Player player)
        {
            
        }


        void InputListener.DoPlayerDisconnected(Core.Input.Player player)
        {
            
        }
    }
}
