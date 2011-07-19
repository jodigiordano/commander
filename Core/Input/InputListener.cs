namespace EphemereGames.Core.Input
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    public interface InputListener
    {
        bool EnableInputs { get; }

        void DoKeyPressed(Player player, Keys key);
        void DoKeyPressedOnce(Player player, Keys key);
        void DoKeyReleased(Player player, Keys key);

        void DoMouseButtonPressed(Player player, MouseButton button);
        void DoMouseButtonPressedOnce(Player player, MouseButton button);
        void DoMouseButtonReleased(Player player, MouseButton button);
        void DoMouseScrolled(Player player, int delta);
        void DoMouseMoved(Player player, Vector3 delta);

        void DoGamePadButtonPressedOnce(Player player, Buttons button);
        void DoGamePadButtonReleased(Player player, Buttons button);
        void DoGamePadJoystickMoved(Player player, Buttons button, Vector3 delta);

        void PlayerKeyboardConnectionRequested(Player Player, Keys key);
        void PlayerMouseConnectionRequested(Player Player, MouseButton button);
        void PlayerGamePadConnectionRequested(Player Player, Buttons button);

        void DoPlayerConnected(Player player);
        void DoPlayerDisconnected(Player player);
    }
}
