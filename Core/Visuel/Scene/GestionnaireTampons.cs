namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Core.Utilities;

    class GestionnaireTampons
    {
        private static GestionnaireTampons instance = null;

        private List<Tampon> Tampons;


        private GestionnaireTampons()
        {
            Tampons = new List<Tampon>();

            // Pour le gestionnaire de scènes
            Tampons.Add(new Tampon(1, 1));
        }

        public void ajouter(int largeur, int hauteur)
        {
            Tampons.Add(new Tampon(hauteur, largeur));
        }

        public Tampon recuperer(int largeur, int hauteur)
        {
            Tampon tampon = null;

            foreach (var tamponCandidat in Tampons)
            {
                if (tamponCandidat.Hauteur == hauteur && tamponCandidat.Largeur == largeur)
                {
                    tampon = tamponCandidat;
                    break;
                }
            }

            if (tampon == null)
                throw new Exception("Un tampon de dimension [" + largeur + "," + hauteur + "] n'est pas disponible. Vous devez fournir un ensemble suffisant via l'Initialize de la facade visuelle (Core.Visuel.Facade)");

            Tampons.Remove(tampon);

            return tampon;
        }

        public void remettre(Tampon tampon)
        {
            if (Tampons.Contains(tampon))
                return;

            Tampons.Add(tampon);
        }

        public static GestionnaireTampons Instance
        {
            get
            {
                if (instance == null)
                    instance = new GestionnaireTampons();

                return instance;
            }
        }
    }
}
