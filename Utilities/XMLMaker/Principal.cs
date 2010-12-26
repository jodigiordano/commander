namespace Utilities.XMLMaker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
    using EphemereGames.Commander;


    class Principal
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Asset a generer: ");
            Console.WriteLine("[0] Highscores");

            char choix = (char)Console.Read();
            Console.ReadLine();
            bool valide = true;

            switch (choix)
            {
                case '0':
                    Console.WriteLine("Generation en cours.");
                    writeXMLWithXmlSerializer<SerializableDictionaryProxy<int, HighScores>>("highscores.xml", genererHighScores());
                    break;

                default: valide = false;
                    break;
            }

            Console.WriteLine("Termine.");

            if (valide)
                Console.WriteLine("Le fichier a ete genere dans le dossier " + AppDomain.CurrentDomain.BaseDirectory);

            Console.Read();
        }


        private static SerializableDictionaryProxy<int, HighScores> genererHighScores()
        {
            var highscores = new SerializableDictionaryProxy<int, HighScores>();
            highscores.Add(new KeyValuePair<int,HighScores>(0, new HighScores(0)));
            highscores[0].Add("Bobby", 2001);
            highscores[0].Add("Bobby2", 5444);

            return highscores;
        }


        private static void ecrireXML<T>(String nomFichier, T objet)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(nomFichier, settings))
            {
                IntermediateSerializer.Serialize(writer, objet, null);
            }
        }


        private static void writeXMLWithXmlSerializer<T>(String filename, T obj)
        {
            XmlSerializer writer = new XmlSerializer(obj.GetType());
            StreamWriter stream = new StreamWriter(filename);

            writer.Serialize(stream, obj);
        }
    }
}
