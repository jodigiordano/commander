namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Content;
    using Core.Persistance;
    using Core.Utilities;

    [Serializable()]
    public class Transition : IContenu
    {
        [ContentSerializer(Optional = false)]
        public String NomTransition { get; set; }

        [ContentSerializer(Optional = false)]
        public float Duree { get; set; }

        [ContentSerializer(Optional = false)]
        public List<DescriptionTransition> Descriptions { get; set; }

        //[ContentSerializer(Optional = true)]
        //public List<AbstractEffet> Effets { get; set; }

        [ContentSerializerIgnore]
        public String TypeAsset { get { return "Transition"; } }

        public object charger(String nom, String chemin, Dictionary<String, String> parametres, ContentManager contenu)
        {
            Transition transition = contenu.Load<Transition>(chemin);
            GestionnaireTransitions.Instance.ajouter(nom, transition);

            return transition;
        }

        public object Clone()
        {
            return this;
        }
    }

    [Serializable()]
    public class DescriptionTransition
    {
        [ContentSerializer(Optional = false)]
        public String NomScene { get; set; }

        [ContentSerializer(Optional = true)]
        public Boolean FocusApres { get; set; }

        [ContentSerializer(Optional = true)]
        public Boolean EnPausePendant { get; set; }

        [ContentSerializer(Optional = true)]
        public Boolean EnPauseApres { get; set; }

        [ContentSerializer(Optional = true)]
        public Boolean VisibleApres { get; set; }

        [ContentSerializer(Optional = true)]
        public float PrioriteAffichagePendant { get; set; }

        [ContentSerializer(Optional = true)]
        public float PrioriteAffichageApres { get; set; }

        [ContentSerializer(Optional = true)]
        public List<AbstractEffet> Effets { get; set; }

        [ContentSerializer(Optional = true)]
        public List<Animation> Animations { get; set; }

        public DescriptionTransition()
        {
            FocusApres = true;
            EnPausePendant = false;
            EnPauseApres = false;
            VisibleApres = true;
            PrioriteAffichagePendant = 0.0f;
            PrioriteAffichageApres = 0.0f;
            Effets = new List<AbstractEffet>();
            Animations = new List<Animation>();
        }
    }
}
