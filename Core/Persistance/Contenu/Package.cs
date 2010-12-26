namespace EphemereGames.Core.Persistance
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Media;
    
    class Package
    {
        private Dictionary<String, object> ContenuCharge { get; set; }
        public bool Charge { get; private set; }

        public int Niveau { get; set; }
        public string Nom { get; set; }
        public bool Temporaire { get; set; }
        public ContentManager Contenu { get; set; }
        public List<DescriptionContenu> Assets { get; set; }

        public Package()
        {
            ContenuCharge = new Dictionary<string, object>();
            Charge = false;
        }

        public object recuperer(String nom)
        {
            if (ContenuCharge.ContainsKey(nom))
                return ContenuCharge[nom];

            return null;
        }

        public void charger()
        {
            foreach (var asset in Assets)
            {
                if (asset.Type == "Texture2D")
                    ContenuCharge.Add(asset.Nom, Contenu.Load<Texture2D>(asset.Chemin));
                else if (asset.Type == "Video")
                    ContenuCharge.Add(asset.Nom, Contenu.Load<Video>(asset.Chemin));
                else if (asset.Type == "EffetFX")
                    ContenuCharge.Add(asset.Nom, Contenu.Load<Effect>(asset.Chemin));
                else if (asset.Type == "Police")
                    ContenuCharge.Add(asset.Nom, Contenu.Load<SpriteFont>(asset.Chemin));
                else
                {
                    var objet = GestionnaireContenu.Instance.getTypeAsset(asset.Type).charger(
                        asset.Nom,
                        asset.Chemin,
                        asset.Parametres,
                        this.Contenu);

                    ContenuCharge.Add(asset.Nom, objet);
                }
            }

            Charge = true;
        }

        public void decharger()
        {
            Contenu.Unload();
            ContenuCharge.Clear();
            Charge = false;
        }
    }
}
