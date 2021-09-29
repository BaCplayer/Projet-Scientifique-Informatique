using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyImage
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Quelle image voulez-vous ?");
            Console.WriteLine("1 - Coco");
            Console.WriteLine("2 - Createurs");
            Console.WriteLine("3 - Votre photo présente dans le debug");
            int Choix = Convert.ToInt32(Console.ReadLine());
            MyImage image = new MyImage("Coco.bmp");
            
            if (Choix == 1 )
            {
                image = new MyImage("Coco.bmp");
            }
            if (Choix == 2)
            {
                image = new MyImage("Createurs.bmp");
            }
            if (Choix == 3)
            {
                Console.WriteLine("Entrez le nom de votre photo (avec.bmp)");
                string NomPhoto = Console.ReadLine();
                image = new MyImage(NomPhoto);
            }


            #region MENU
            Console.WriteLine("Projet Scientique-Informatique");
            Console.WriteLine();
            Console.WriteLine("Bonjour,");
            Console.WriteLine();
            Console.WriteLine("Voici les différents traitements d'image disponibles :");
            Console.WriteLine();                                                                                                                            //Affichage ergonomique
            Console.WriteLine("1 - Afficher les informations relatives à l'image");
            Console.WriteLine();
            Console.WriteLine("2 - Photo en nuances de gris");
            Console.WriteLine();
            Console.WriteLine("3 - Photo en noir et blanc");
            Console.WriteLine();                                                                                                                            //Affichage ergonomique
            Console.WriteLine("4 - Agrandir l'image");
            Console.WriteLine();                                                                                                                            //Affichage ergonomique
            Console.WriteLine("5 - Rétrecir l'image");
            Console.WriteLine();                                                                                                                            //Affichage ergonomique
            Console.WriteLine("6 - Rotation");
            Console.WriteLine();
            Console.WriteLine("7 - Effet miroir");
            Console.WriteLine();
            Console.WriteLine("8 - Détection de contour");
            Console.WriteLine();
            Console.WriteLine("9 - Renforcement des bords");
            Console.WriteLine();
            Console.WriteLine("10 - Flou");
            Console.WriteLine();
            Console.WriteLine("11 - Repoussage");
            Console.WriteLine();
            Console.WriteLine("12 - Cacher une image");
            Console.WriteLine();
            Console.WriteLine("13 - Retrouver une image");
            Console.WriteLine();
            Console.WriteLine("14 - Histogramme");
            Console.WriteLine();
            Console.WriteLine("15 - Fractale");
            Console.WriteLine();
            Console.WriteLine("16 - QR Code");
            Console.WriteLine();
            Console.WriteLine(" Innovations ");
            Console.WriteLine();
            Console.WriteLine("17 - Rogner");
            Console.WriteLine();
            Console.WriteLine("18 - Amélioration de la netteté");
            Console.WriteLine();
            Console.WriteLine("0 - Quitter");
            Console.WriteLine();
            Console.WriteLine("Un projet présenté par Aurélien Damerval et Emmeran Darribère");
            Console.WriteLine();                                                                                                                            //Affichage ergonomique

            int ValeurMaximaleDeCas = 100; 

            while (ValeurMaximaleDeCas > 0)
            {
                ValeurMaximaleDeCas = SaisirNbre();
                switch (ValeurMaximaleDeCas)
                {
                    case 0:
                        Console.WriteLine("Fin de programme");
                        break;
                    case 1:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi d'afficher les informations de l'image");
                        Console.WriteLine();
                        image.InfoImage();
                        break;
                    case 2:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi d'afficher la photo en nuances de gris");
                        image.NuancesDeGris();
                        break;
                    case 3:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi d'afficher la photo en noir et blanc");
                        image.NoirEtBlanc();
                        break;
                    case 4:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi d'agrandir l'image");
                        image.Agrandissement();
                        break;
                    case 5:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi de rétrecir l'image");
                        image.Rétrécissement();
                        break;
                    case 6:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi la rotation");
                        image.RotationAngleDonne();
                        break;
                    case 7:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi l'effet miroir");
                        image.Miroir();
                        break;
                    case 8:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi la détection de contour");
                        image.DetectionContour();
                        break;
                    case 9:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi le renforcement des bords");
                        image.RenforcementBords();
                        break;
                    case 10:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi le traitement flou");
                        image.Flou();
                        break;
                    case 11:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi le repoussage");
                        image.Repoussage();
                        break;
                    case 18:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi l'amélioration de la netteté");
                        image.AmeliorationDeLaNettete();
                        break;
                    case 12:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi de cacher une image");
                        image.CacherImage("Createurs.bmp");
                        break;
                    case 13:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi de retrouver une image");
                        image.DecoderImage("Image_Cachée.bmp");
                        break;
                    case 14:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi l'histogramme");
                        image.Histogramme();
                        break;
                    case 17:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi de rogner");
                        image.Rogner();
                        break;
                    case 15:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi de faire une fractale");
                        image.Fractale();
                        break;
                    case 16:
                        Console.WriteLine();
                        Console.WriteLine("Vous avez choisi le QR Code");
                        Console.WriteLine("Ecrivez un mot");


                        string Phrase = Console.ReadLine();
                        byte[] Donnees = image.DonnéesQRCode(Phrase);

                        byte[] Correction = { 209, 239, 196, 207, 78, 195, 109 };
                        byte[] Correction2 = ReedSolomonAlgorithm.Encode(Donnees, 7, ErrorCorrectionCodeType.QRCode);

                        byte[] Info = new byte[Donnees.Length + 7 * 8];
                        for (int i = 0; i < Donnees.Length; i++)
                        {
                            Info[i] = Donnees[i];
                        }
                        string OctetsCorrection = "";
                        for (int i = 0; i < Correction.Length; i++)
                        {
                            byte[] Octet = image.Convertir_Int_Binaire(Correction[i], 8);
                            for (int j = 0; j < 8; j++)
                            {
                                OctetsCorrection += Octet[j];
                            }
                        }
                        string StringDonnées = "";
                        for (int i = 0; i < Donnees.Length; i++)
                        {
                            StringDonnées += Donnees[i];
                        }
                        StringDonnées += OctetsCorrection;
                        for (int i = 0; i < StringDonnées.Length; i++)
                        {
                            if (StringDonnées[i] == '0')
                            {
                                Info[i] = 0;
                            }
                            else
                            {
                                Info[i] = 1;
                            }
                        }
                        MatriceRGB[,] Fond = image.FondQRCode();
                        image.AgrandissementQRCode(image.Masque(Info, Fond));
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine();
                        Console.WriteLine("Le traitement que vous avez choisi n'existe pas");
                        Console.WriteLine("Veuillez en choisir un autre parmi ceux existants");
                        break;
                }
            }

            Console.ReadKey();
            #endregion                            

        }
        public static int SaisirNbre()
        {
            int n = 0;
            bool parseOk;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Quel traitement d'image souhaitez-vous ?");
                parseOk = int.TryParse(Console.ReadLine(), out n);
            }
            while (n < 0 || parseOk == false);

            return n;
        }

    }
}
