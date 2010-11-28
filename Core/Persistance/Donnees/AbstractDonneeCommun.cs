namespace Core.Persistance
{
    using System;
    using System.IO;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;
    using Core.Utilities;

    public abstract class AbstractDonneeCommun : AbstractDonnee
    {
        public enum Reponse
        {
            Prompt,
            ReselectionnerSD,
            RienFaire
        }

        public AbstractDonneeCommun()
            : base() { }

        public virtual Reponse doDemandeSelection() { return Reponse.RienFaire; }


        public void Initialize(PlayerIndex indexJoueur)
        {
            // création du save device
#if XBOX
            PeripheriqueStockage = new SharedSaveDevice(NomDossier);

            // quand le storage device sera sélectionné, on chargera les données (si elles n'avaient pas déjà été chargées)
            // sinon, on les enregistre sur le nouveau SD
            PeripheriqueStockage.DeviceSelected += new EventHandler(saveDevice_DeviceSelected);

            // si le storage device a été déconnecté ou que sa sélection a été annulée,
            // et que le menu high score ou le mode high est actif,
            // on demandera au joueur s'il souhaite en sélectionner un autre ou
            //                                         - quitter la partie en cours (si mode high score actif)
            //                                         - ne rien faire (sinon)
            // Si le menu High Score et le mode High Score sont inactifs, on réinitialise le save device.
            PeripheriqueStockage.DeviceSelectorCanceled += new EventHandler<SaveDeviceEventArgs>(demanderSelection);
            PeripheriqueStockage.DeviceDisconnected += new EventHandler<SaveDeviceEventArgs>(demanderSelection);

            // s'il choisit de quitter la partie en cours ou de fermer le menu High Score, let's do it ;)
            PeripheriqueStockage.DeviceReselectPromptClosed += new EventHandler<SaveDevicePromptEventArgs>(saveDevice_DeviceReselectPromptClosed);

            // On demande maintenant à afficher l'écran de sélection du storage device
            PeripheriqueStockage.PromptForDevice();
#else
            PeripheriqueStockage = new PCSaveDevice(NomDossier);
#endif
        }


        private void demanderSelection(object sender, SaveDeviceEventArgs e)
        {
            Reponse reponse = doDemandeSelection();

            if (SignedInGamer.SignedInGamers.Count > 0)
                e.PlayerToPrompt = SignedInGamer.SignedInGamers[0].PlayerIndex;

            switch (reponse)
            {
                case Reponse.Prompt:
                    e.Response = SaveDeviceEventResponse.Prompt;
                    break;
                case Reponse.ReselectionnerSD:
                    e.Response = SaveDeviceEventResponse.Prompt1Choice; // demandera seulement de resélectionner un SD
                    break;
                case Reponse.RienFaire:
                    e.Response = SaveDeviceEventResponse.Nothing;
                    break;
            }
        }
    }
}
