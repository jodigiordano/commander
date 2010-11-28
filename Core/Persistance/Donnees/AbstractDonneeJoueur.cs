namespace Core.Persistance
{
    using System;
    using System.IO;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;
    using Core.Utilities;
    using Microsoft.Xna.Framework.Content;

    public abstract class AbstractDonneeJoueur : AbstractDonnee
    {
        [ContentSerializerIgnore]
        public PlayerIndex IndexJoueur { get; set; }

        public AbstractDonneeJoueur()
            : base() { }

        public void Initialize(PlayerIndex indexJoueur)
        {
#if XBOX
            if (SignedInGamer.SignedInGamers[indexJoueur] == null)
                throw new Exception("Le joueur n'est pas connecté.");
#endif
            base.Initialize();

            //
            IndexJoueur = indexJoueur;

            // création du save device
#if XBOX
            PeripheriqueStockage = new PlayerSaveDevice(NomDossier, indexJoueur);

            // quand le storage device sera sélectionné, on chargera les données (si elles n'avaient pas déjà été chargées)
            PeripheriqueStockage.DeviceSelected += new EventHandler(saveDevice_DeviceSelected);

            // si la sélection du storage device a été annulée, ou si celui-ci a été déconnecté,
            // on demandera au joueur s'il souhaite en sélectioner un autre ou retourner au SplashScreen
            PeripheriqueStockage.DeviceSelectorCanceled += (s, e) => e.Response = SaveDeviceEventResponse.Force;
            PeripheriqueStockage.DeviceDisconnected += (s, e) => e.Response = SaveDeviceEventResponse.Force;

            // s'il choisit de retourner au SplashScreen, let's do it ;)
            PeripheriqueStockage.DeviceReselectPromptClosed += new EventHandler<SaveDevicePromptEventArgs>(saveDevice_DeviceReselectPromptClosed);

#else
            PeripheriqueStockage = new PCSaveDevice(NomDossier);
#endif
        }
    }
}
