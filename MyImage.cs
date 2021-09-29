using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;

namespace MyImage
{
    class MyImage
    {
        private int tailleFichier;
        private int tailleOffset;
        private int largeur;
        private int hauteur;
        private int NbBitCouleur;
        private string TypeFichier;
        private MatriceRGB[,] image;
        
        public int TailleFichier
        {
            get { return tailleFichier; }
            set { tailleFichier = value ; }
        }
        public int TailleOffset
        {
            get { return tailleOffset; }
            set { tailleOffset = value; }
        }
        public int Largeur
        {
            get { return largeur; }
            set { largeur = value; }
        }
        public int Hauteur
        {
            get { return hauteur; }
            set { hauteur = value; }
        }
        public int NBbitCouleur
        {
            get { return NbBitCouleur; }
            set { NbBitCouleur = value; }
        }
        public MatriceRGB[,] Image
        {
            get { return image; }
            set { image = value; }
        }

        /// <summary>
        /// Constructeur de notre classe, appelle la méthode qui donne à chaque attribue sa valeur et qui affiche le menu
        /// </summary>
        /// <param name="monfichier">Nom du fichier bmp sur lequel nous allons travailler</param>
        public MyImage(string monfichier)
        {
            MonImage(monfichier);         
        }


        /// <summary>
        /// Transforme une instance de MyImage en fichier
        /// </summary>
        public void From_Image_To_File()
        {
            byte[] Fichier = new byte[(hauteur * largeur * 3) + 54];

            Fichier[0] = 66;
            Fichier[1] = 77;
            int Compteur3 = 0;

            //Conversion de la taille de la photo en Little Endian

            for(int i = 2; i<=5; i++)
            {
                Fichier[i] = (Convertir_Int_To_Endian(tailleFichier, 4))[Compteur3];
                Compteur3++;
            }

            for (int i = 6; i <= 13; i++)
            {
                if (i == 10)
                {
                    Fichier[i] = 54;
                }
                else
                {
                    Fichier[i] = 0;
                }

            }
            Compteur3 = 0;

            //Conversion de la taille offset, de la largeur, de la hauteur et du nombre de bits par couleurs de la photo en Little Endian

            for (int i = 14; i<=17;i++)
            {
                Fichier[i] = (Convertir_Int_To_Endian(tailleOffset, 4))[Compteur3];
                Compteur3++;
            }
            Compteur3 = 0;
           
            for (int i =18;i<=21;i++)
            {
                Fichier[i] = (Convertir_Int_To_Endian(largeur, 4))[Compteur3];
                Compteur3++;
            }
            Compteur3 = 0;
            for (int i = 22; i <= 25; i++)
            {
                Fichier[i] = (Convertir_Int_To_Endian(hauteur, 4))[Compteur3];
                Compteur3++;
            }

            Fichier[26] = 1;
            Fichier[27] = 0;
            Compteur3 = 0;
            for (int i = 28; i<=29;i++)
            {
                Fichier[i] = (Convertir_Int_To_Endian(NbBitCouleur, 2))[Compteur3];
                Compteur3++;
            }

            for (int i = 30; i <= 53; i++)
            {
                Fichier[i] = 0;

                if (i == 34)
                {
                    Fichier[i] = 176;

                }

                if (i == 35)
                {
                    Fichier[i] = 4;
                }

            }
            

            int Compteur4 = 54;

            //Ecriture des valeurs des pixels dans notre fichier

            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    
                    Fichier[Compteur4] = image[x, y].Red;
                    Fichier[Compteur4+1] = image[x, y].Green;
                    Fichier[Compteur4+2] = image[x, y].Blue;
                    Compteur4 += 3;
                }
            }
            for (int i = 0; i <Fichier.Length; i++)
            {
                Console.Write(Fichier[i] + " ");
            }
            
        }

        /// <summary>
        /// Méthode qui donne à chaque attribue sa valeur et qui affiche le menu
        /// </summary>
        /// <param name="monfichier">Nom du fichier bmp sur lequel nous allons travailler</param>
        public void MonImage(string monfichier)
        {
            //Lecture du fichier

            byte[] MyFile = File.ReadAllBytes(monfichier);

            //Lecture de l'extension du fichier (Type du fichier)

            int Compteur1 = 0;
            while (monfichier[Compteur1] != '.')
            {
                Compteur1++;
            }
            for (int Compteur2 = Compteur1 + 1; Compteur2 < monfichier.Length; Compteur2++)
            {
                TypeFichier += monfichier[Compteur2];
            }


            tailleFichier = Convertir_Endian_To_Int(MyFile, 2, 5);

            largeur = Convertir_Endian_To_Int(MyFile, 18, 21);

            hauteur = Convertir_Endian_To_Int(MyFile, 22, 25);

            NbBitCouleur = Convertir_Endian_To_Int(MyFile, 28, 29);

            tailleOffset = Convertir_Endian_To_Int(MyFile, 14, 17);

            //Création d'une matrice de pixels correspondant à la photo grâce aux informations du fichier

            image = new MatriceRGB[hauteur, largeur];
            int compteur = 54;
            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    image[x, y] = new MatriceRGB(MyFile[compteur], MyFile[compteur + 1], MyFile[compteur + 2]);

                    compteur += 3;
                }
            }


        }
        
        /// <summary>
        /// Méthode qui affiche les informations de l'image
        /// </summary>
        public void InfoImage()
        {
            Console.WriteLine("Le type de fichier est : " + TypeFichier);
            Console.WriteLine("Taille de l'image : " + tailleFichier + " octets");
            Console.WriteLine("Hauteur de l'image : " + hauteur);
            Console.WriteLine("Largeur de l'image : " + largeur);
            Console.WriteLine("Nombre de bits par couleur : " + NbBitCouleur);
            Console.WriteLine("La taille offset est de : " + tailleOffset);
        }

        /// <summary>
        /// Méthode qui convertit un nombre en little endian vers la base décimale
        /// </summary>
        /// <param name="tab">Tableau de byte à convertir</param>
        /// <param name="ValeurBasse">Borne basse du tableau de byte à convertir</param>
        /// <param name="ValeurHaute">Borne haute du tableau de byte à convertir</param>
        /// <returns>L'entier convertit en base décimale</returns>
        public int Convertir_Endian_To_Int(byte[] tab, int ValeurBasse, int ValeurHaute) //Les entiers ValeurBasse et ValeurHaute nous permettent de traiter un ficher dans son intégralité et de séléctionner les parties de ce fichier qui nous intéressent
        {
            double entier = 0;
            double a = 0;

            for (int i = ValeurBasse; i <= ValeurHaute; i++)
            {
                entier += Convert.ToDouble(tab[i]) * (Math.Pow(256, a));
                a++;
            }

            int Conversion = Convert.ToInt32(entier);

            return Conversion;

        }

        /// <summary>
        /// Méthode qui convertit un entier vers la base little endian
        /// </summary>
        /// <param name="val">Entier à convertir</param>
        /// <param name="TailleTableau">Taille du tableau qui sera retourné (Certains tableaux devait être de 2 ou de 4 bytes)</param>
        /// <returns>Tableau de byte correspondant à l'entier</returns>
        public byte[] Convertir_Int_To_Endian(int val, int TailleTableau)
        {
            byte[] Tableau = new byte[TailleTableau];
            double tampon = val / (Math.Pow(256, TailleTableau - 1));

            while (tampon < 0)
            {
                Tableau[TailleTableau - 1] = 0;
                TailleTableau--;
            }

            for (int i = TailleTableau - 1; i >= 0; i--)
            {
                int Tampon1 = Convert.ToInt32(Math.Truncate(val / (Math.Pow(256, i))));
                Tableau[i] = Convert.ToByte(Tampon1);
                val = Convert.ToInt32(val % (Math.Pow(256, i)));
            }
            return Tableau;
        }

        /// <summary>
        /// Méthode qui affiche une image dont la matrice de pixel a été modifié
        /// </summary>
        /// <param name="Image2">Matrice de pixel correspondant à notre nouvelle image créée </param>
        /// <param name="MonNouveauFichier">Nom de fichier pour notre nouvelle image créée </param>
        public void AfficherImage(MatriceRGB[,] Image2, string MonNouveauFichier)
        {
            byte[] Fichier = new byte[(hauteur * largeur * 3) + 54];

            Fichier[0] = 66;
            Fichier[1] = 77;
            int Compteur3 = 0;

            for (int i = 2; i <= 5; i++)
            {
                Fichier[i] = (Convertir_Int_To_Endian(tailleFichier, 4))[Compteur3];
                Compteur3++;
            }
            for (int i = 6; i <= 13; i++)
            {
                if (i == 10)
                {
                    Fichier[i] = 54;
                }
                else
                {
                    Fichier[i] = 0;
                }

            }
            Compteur3 = 0;
            for (int i = 14; i <= 17; i++)
            {
                Fichier[i] = (Convertir_Int_To_Endian(tailleOffset, 4))[Compteur3];
                Compteur3++;
            }
            Compteur3 = 0;
            for (int i = 18; i <= 21; i++)
            {
                Fichier[i] = (Convertir_Int_To_Endian(largeur, 4))[Compteur3];
                Compteur3++;
            }
            Compteur3 = 0;
            for (int i = 22; i <= 25; i++)
            {
                Fichier[i] = (Convertir_Int_To_Endian(hauteur, 4))[Compteur3];
                Compteur3++;
            }

            Fichier[26] = 1;
            Fichier[27] = 0;
            Compteur3 = 0;
            for (int i = 28; i <= 29; i++)
            {
                Fichier[i] = (Convertir_Int_To_Endian(NbBitCouleur, 2))[Compteur3];
                Compteur3++;
            }

            for (int i = 30; i <= 53; i++)
            {
                Fichier[i] = 0;

                if (i == 34)
                {
                    Fichier[i] = 176;

                }

                if (i == 35)
                {
                    Fichier[i] = 4;
                }

            }


            int Compteur4 = 54;

            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    Fichier[Compteur4] = Image2[x, y].Red;
                    Fichier[Compteur4 + 1] = Image2[x, y].Green;
                    Fichier[Compteur4 + 2] = Image2[x, y].Blue;
                    Compteur4 += 3;
                }
            }


            File.WriteAllBytes(MonNouveauFichier, Fichier);
            Process.Start(MonNouveauFichier);
        }

        /// <summary>
        /// Seconde méthode pour afficher une image dont les dimensions ont été modifiées (Prend en compte la nouvelle hauteur et largeur de l'image)
        /// </summary>
        /// <param name="Image2">Matrice de pixel correspondant à notre nouvelle image créée</param>
        /// <param name="MonNouveauFichier">Nom de fichier pour notre nouvelle image créée</param>
        /// <param name="NewHauteur">Nouvelle hauteur de l'image modifiée</param>
        /// <param name="NewLargeur">Nouvelle largeur de l'image modifiée</param>
        public void AfficherImageBis(MatriceRGB[,] Image2, string MonNouveauFichier, int NewHauteur, int NewLargeur)
        {
            byte[] Fichier = new byte[(NewHauteur * NewLargeur * 3) + 54];

            Fichier[0] = 66;
            Fichier[1] = 77;
            int Compteur3 = 0;

            for (int i = 2; i <= 5; i++)
            {
                Fichier[i] = (Convertir_Int_To_Endian(tailleFichier, 4))[Compteur3];
                Compteur3++;
            }
            for (int i = 6; i <= 13; i++)
            {
                if (i == 10)
                {
                    Fichier[i] = 54;
                }
                else
                {
                    Fichier[i] = 0;
                }

            }
            Compteur3 = 0;
            for (int i = 14; i <= 17; i++)
            {
                Fichier[i] = (Convertir_Int_To_Endian(tailleOffset, 4))[Compteur3];
                Compteur3++;
            }
            Compteur3 = 0;
            for (int i = 18; i <= 21; i++)
            {
                Fichier[i] = (Convertir_Int_To_Endian(NewLargeur, 4))[Compteur3];
                Compteur3++;
            }
            Compteur3 = 0;
            for (int i = 22; i <= 25; i++)
            {
                Fichier[i] = (Convertir_Int_To_Endian(NewHauteur, 4))[Compteur3];
                Compteur3++;
            }

            Fichier[26] = 1;
            Fichier[27] = 0;
            Compteur3 = 0;
            for (int i = 28; i <= 29; i++)
            {
                Fichier[i] = (Convertir_Int_To_Endian(NbBitCouleur, 2))[Compteur3];
                Compteur3++;
            }

            for (int i = 30; i <= 53; i++)
            {
                Fichier[i] = 0;

                if (i == 34)
                {
                    Fichier[i] = 176;

                }

                if (i == 35)
                {
                    Fichier[i] = 4;
                }

            }


            int Compteur4 = 54;

            for (int x = 0; x < NewHauteur; x++)
            {
                for (int y = 0; y < NewLargeur; y++)
                {
                    Fichier[Compteur4] = Image2[x, y].Red;
                    Fichier[Compteur4 + 1] = Image2[x, y].Green;
                    Fichier[Compteur4 + 2] = Image2[x, y].Blue;
                    Compteur4 += 3;
                }
            }


            File.WriteAllBytes(MonNouveauFichier, Fichier);
            Process.Start(MonNouveauFichier);
        }

        /// <summary>
        /// Méthode qui inverse une image suivant l'effet miroir
        /// </summary>
        public void Miroir()
        {
            MatriceRGB[,] ImageMiroir = new MatriceRGB[hauteur, largeur];

            int cpt = largeur - 1;

            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    ImageMiroir[x, y] = image[x, cpt];
                    cpt--;
                }
                cpt = largeur - 1;
            }

            AfficherImage(ImageMiroir, "Miroir.bmp");
        }

        /// <summary>
        /// Méthode qui aggrandit une image en fonction du facteur rentré par l'utilisateur
        /// </summary>
        public void Agrandissement()
        {
            Console.WriteLine("Veuillez saisir le facteur d'agrandissement :");
            int Facteur = Convert.ToInt32(Console.ReadLine());
            MatriceRGB[,] ImageAgrandie = new MatriceRGB[hauteur * Facteur, largeur * Facteur];

            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    for (int i = 0; i < Facteur; i++)
                    {
                        for (int j = 0; j < Facteur; j++)
                        {
                            ImageAgrandie[x * Facteur + i, y * Facteur + j] = image[x, y];
                        }
                    }
                }

            }
            AfficherImageBis(ImageAgrandie, "Agrandissement.bmp", ImageAgrandie.GetLength(0), ImageAgrandie.GetLength(1));
        }

        /// <summary>
        /// Méthode qui rétrécit une image en fonction du facteur rentré par l'utilisateur
        /// </summary>
        public void Rétrécissement()
        {
            Console.WriteLine("Veuillez saisir le facteur de rétrécissement :");
            int Facteur = Convert.ToInt32(Console.ReadLine());
            int NouvelleHauteur = hauteur / Facteur;
            int NouvelleLargeur = largeur / Facteur;

            MatriceRGB[,] ImageRétrécie = new MatriceRGB[NouvelleHauteur, NouvelleLargeur];

            for (int i = 0; i < NouvelleHauteur; i++)
            {
                for (int j = 0; j < NouvelleLargeur; j++)
                {
                    ImageRétrécie[i, j] = new MatriceRGB(0, 0, 0);
                }
            }

            for (int i = 0; i < NouvelleHauteur; i++)
            {
                for (int j = 0; j < NouvelleLargeur; j++)
                {
                    ImageRétrécie[i, j].Red = image[i * Facteur, j * Facteur].Red;
                    ImageRétrécie[i, j].Green = image[i * Facteur, j * Facteur].Green;
                    ImageRétrécie[i, j].Blue = image[i * Facteur, j * Facteur].Blue;
                }

            }

            AfficherImageBis(ImageRétrécie, "Retrecissement.bmp", ImageRétrécie.GetLength(0), ImageRétrécie.GetLength(1));
        }

        /// <summary>
        /// Applique la couleur moyenne des 3 bits -> (Red+Green+Blue)/3 
        /// </summary>
        public void NuancesDeGris()
        {
            MatriceRGB[,] MatriceNB = new MatriceRGB[hauteur, largeur];
            byte byte1;

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    byte1 = Convert.ToByte((image[i, j].Red + image[i, j].Green + image[i, j].Blue) / 3);
                    MatriceNB[i, j] = new MatriceRGB(byte1, byte1, byte1);
                }
            }
            AfficherImage(MatriceNB, "NuancesDeGris.bmp");
        }

        /// <summary>
        /// Applique la couleur noire (0,0,0) ou blanche (255,255,255) aux pixels de l'image suivant la valeur moyenne des 3 bits 
        /// </summary>
        public void NoirEtBlanc()
        {
            MatriceRGB[,] MatriceNB = new MatriceRGB[hauteur, largeur];
            byte byte1;

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    int BoW = (image[i, j].Blue + image[i, j].Green + image[i, j].Blue) / 3;
                    byte1 = Convert.ToByte(BoW);
                    if (BoW > 128)
                    {
                        MatriceNB[i, j] = new MatriceRGB(255, 255, 255);
                    }
                    else
                    {
                        MatriceNB[i, j] = new MatriceRGB(0, 0, 0);
                    }
                }
            }

            AfficherImage(MatriceNB, "NoirEtBlanc.bmp");
        }

        /// <summary>
        /// Méthode qui demande de choisir un angle et effectue une rotation de l'image suivant cet angle
        /// </summary>
        public void RotationAngleDonne()
        {
            Console.WriteLine("Voici les différents angles disponibles :");
            Console.WriteLine();
            Console.WriteLine("1 - 90°");
            Console.WriteLine();
            Console.WriteLine("2 - 180°");
            Console.WriteLine();
            Console.WriteLine("3 - 270°");
            Console.WriteLine();

            int ValeurMaximaleDeCas = SaisirNbre();
            switch (ValeurMaximaleDeCas)
            {
                case 1:
                    Console.WriteLine();
                    Console.WriteLine("Vous avez choisi 90°");
                    AfficherImageBis(Rotation90(image), "Test90.bmp", Rotation90(image).GetLength(0), Rotation90(image).GetLength(1));
                    break;

                case 2:
                    Console.WriteLine();
                    Console.WriteLine("Vous avez choisi 180°");
                    MatriceRGB[,] Image180 = Rotation90(image);
                    Image180 = Rotation90(Image180);
                    AfficherImageBis(Image180, "Test180.bmp", Image180.GetLength(0), Image180.GetLength(1));
                    break;

                case 3:
                    Console.WriteLine();
                    Console.WriteLine("Vous avez choisi 270°");
                    MatriceRGB[,] Image270 = Rotation90(image);
                    Image270 = Rotation90(Image270);
                    Image270 = Rotation90(Image270);
                    AfficherImageBis(Image270, "Test270.bmp", Image270.GetLength(0), Image270.GetLength(1));
                    break;

                default:
                    Console.WriteLine();
                    Console.WriteLine("L'angle que vous avez choisi n'existe pas");
                    Console.WriteLine("Veuillez en choisir un autre parmi ceux existants");
                    break;
            }
        }

        /// <summary>
        /// Méthode qui fait subir une rotation de 90° à une image
        /// </summary>
        /// /// <param name="Image">Image qui va subir la rotation</param>
        public static MatriceRGB[,] Rotation90(MatriceRGB[,] Image)
        {
            MatriceRGB[,] Image90 = new MatriceRGB[Image.GetLength(1), Image.GetLength(0)];

            for (int x = 0; x < Image.GetLength(0); x++)
            {
                for (int y = 0; y < Image.GetLength(1); y++)
                {
                    Image90[y, Image.GetLength(0) - 1 - x] = Image[x, y];
                }
            }
            return Image90;
        }

        /// <summary>
        /// Méthode créée en année 1 qui permet de boucler dans un menu
        /// </summary>
        public static int SaisirNbre()
        {
            int n = 0;
            bool parseOk;
            do
            {
                Console.WriteLine("Quel traitement d'image souhaitez-vous ?");
                parseOk = int.TryParse(Console.ReadLine(), out n);
            }
            while (n < 0 || parseOk == false);

            return n;
        }

        /// <summary>
        /// Méthode pour la matrice de convolution (Création d'une image plus grande que celle que l'on va traiter afin de ne pas sortir de la matrice)
        /// </summary>
        /// <param name="MatriceConvolution">Noyau (Matrice générique obtenue sur Internet)</param>
        public void Convolution(double[,] MatriceConvolution)
        {
            //Création d'un cadre blanc autour de la photo pour ne pas sortir au moment de faire la convolution

            int NouvelleHauteur = hauteur + 4;
            int NouvelleLargeur = largeur + 4;

            MatriceRGB[,] ImageConvolution = new MatriceRGB[NouvelleHauteur, NouvelleLargeur];

            for (int i = 0; i < NouvelleHauteur; i++)
            {
                for (int j = 0; j < NouvelleLargeur; j++)
                {
                    ImageConvolution[i, j] = new MatriceRGB(255, 255, 255);
                }
            }

            for (int i = 4; i < hauteur; i++)
            {
                for (int j = 4; j < largeur; j++)
                {
                    ImageConvolution[i, j].Red = image[i - 1, j - 1].Red;
                    ImageConvolution[i, j].Green = image[i - 1, j - 1].Green;
                    ImageConvolution[i, j].Blue = image[i - 1, j - 1].Blue;
                }
            }

            //Application de la matrice de convolution sur notre image

            double Bleu = 0;
            double Vert = 0;
            double Rouge = 0;

            for (int x = 4; x < hauteur - 1; x++)
            {
                for (int y = 4; y < largeur - 1; y++)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            Bleu += image[x + i, y + j].Blue * MatriceConvolution[1 + i, 1 + j];
                            Vert += image[x + i, y + j].Green * MatriceConvolution[1 + i, 1 + j];
                            Rouge += image[x + i, y + j].Red * MatriceConvolution[1 + i, 1 + j];
                        }
                    }

                    // Les cas où la valeur serait inférieure à 0 ou supérieure à 255 sont gérés
                    if (Bleu < 0)
                    {
                        Bleu = 0;
                    }
                    else if (Bleu > 255)
                    {
                        Bleu = 255;
                    }
                    if (Vert < 0)
                    {
                        Vert = 0;
                    }
                    else if (Vert > 255)
                    {
                        Vert = 255;
                    }
                    if (Rouge < 0)
                    {
                        Rouge = 0;
                    }
                    else if (Rouge > 255)
                    {
                        Rouge = 255;
                    }
                    ImageConvolution[x, y].Blue = Convert.ToByte(Math.Truncate(Bleu));
                    ImageConvolution[x, y].Green = Convert.ToByte(Math.Truncate(Vert));
                    ImageConvolution[x, y].Red = Convert.ToByte(Math.Truncate(Rouge));
                    Bleu = 0;
                    Vert = 0;
                    Rouge = 0;
                }
            }
            AfficherImageBis(ImageConvolution, "Convolution.bmp", NouvelleHauteur, NouvelleLargeur);
        }

        #region Noyaux
        public void DetectionContour()
        {
        Choix:
            Console.WriteLine("Quelle détéction de contour voulez-vous ?");
            Console.WriteLine(" ");
            Console.WriteLine("1 - Détection de contour standard ");
            Console.WriteLine(" ");
            Console.WriteLine("2 - Détection de contour améliorée ");
            Console.WriteLine(" ");
            Console.WriteLine("3 - Détection de contour optimale");

            int Choix = Convert.ToInt32(Console.ReadLine());

            if (Choix == 1)
            {
                double[,] Matrice = { { 1, 0, -1 }, { 0, 0, 0 }, { -1, 0, 1 } };
                Convolution(Matrice);
            }
            else if (Choix == 2)
            {
                double[,] Matrice = { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
                Convolution(Matrice);
            }
            else if (Choix == 3)
            {
                double[,] Matrice = { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
                Convolution(Matrice);
            }
            else
            {
                Console.WriteLine("Le choix que vous avez fait n'est pas disponible");
                goto Choix;
            }


        }

        public void RenforcementBords()
        {
            double[,] Matrice = { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } };
            Convolution(Matrice);
        }

        public void Flou()
        {
            double[,] Matrice = { { 0.0625, 0.125, 0.0625 }, { 0.125, 0.25, 0.125 }, { 0.0625, 0.125, 0.0625 } };
            Convolution(Matrice);
        }

        public void AmeliorationDeLaNettete()
        {
            double[,] Matrice = { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
            Convolution(Matrice);
        }

        public void Repoussage()
        {
            double[,] Matrice = { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
            Convolution(Matrice);
        }

        public void FiltreDeSobel()
        {
            //double[,] Matrice = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
            double[,] Matrice = { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } }; // Prewit
            Convolution(Matrice);
        }
        #endregion Noyaux
        //Région regroupant les méthodes de traitement d'image

        /// <summary>
        /// Méthode nécessaire à CacherImage() qui convertit deux entiers en octet et renvoie un tableau avec les 4 premiers octets de Pixel1 puis les 4 premiers de Pixel2
        /// </summary>
        /// <param name="Pixel1">Entier correspondant à la couleur d'un pixel de l'image qui va contenir l'image cachée</param>
        /// <param name="Pixel2">Entier correspondant à la couleur d'un pixel de l'image qui va être cachée</param>
        /// <returns>Tableau d'entier correspondant à un octet de la nouvelle image (Image+Image cachée)</returns>
        public int[] Convert_Int_To_Binary(int Pixel1, int Pixel2)
        {
            int[] tab1 = new int[4];
            int[] tab2 = new int[4];
            int[] tab = new int[8];

            for (int i = 0; i < 4; i++)
            {
                Pixel1 = Pixel1 / 2;
            }

            for (int i = 3; i > 0; i--)
            {
                tab1[i] = Pixel1 % 2;
                Pixel1 = Pixel1 / 2;               
            }

            for (int i = 0; i < 4; i++)
            {
                Pixel2 = Pixel2 / 2;
            }
            for (int i = 3; i > 0; i--)
            {
                tab2[i] = Pixel2 % 2;
                Pixel2 = Pixel2 / 2;                
            }
           
            for (int i = 0; i<8; i++)
            {
                if(i<4)
                {
                    tab[i] = tab1[i];
                }
                else
                {
                    tab[i] = tab2[i-4];
                }
            }
           

            return tab;
        }

        /// <summary>
        /// Méthode qui convertit un chiffre binaire en héxadécimal
        /// </summary>
        /// <param name="tab">Tableau du chiffre en binaire</param>
        /// <returns>Entier convertit</returns>
        public int Convert_Binary_To_Int(int[] tab)
        {
            int Pixel = 0;
            

            for(int i = 0; i<tab.Length; i++)
            {
               Pixel += Convert.ToInt32( tab[i] * (Math.Pow(2, (tab.Length - 1) - i)));
            }
            
            return Pixel;
        }

        /// <summary>
        /// Méthode convertissant un entier en binaire
        /// </summary>
        /// <param name="Pixel">Entier à convertir</param>
        /// <returns>Entier converti</returns>
        public int[] Convert_Int_To_Binary_Bis(int Pixel)
        {
            int[] tabFINAL = new int[8];
            int[] tableau = new int[8];

            for (int i = 7; i >= 0; i--)
            {
                tableau[i] = Pixel % 2;
                Pixel = Pixel / 2;
            }


            for (int i = 0; i < 8; i++)
            {
                if (i>=4)
                {
                    tabFINAL[i] = 0;
                }
                else
                {
                    tabFINAL[i] = tableau[i+4];
                }
            }
                       

            return tabFINAL;
        }

        /// <summary>
        /// Méthode qui permet de cacher une image à l'intérieur d'une autre
        /// </summary>
        /// <param name="Fichier">Nom de l'image bmp qui va être cachée</param>
        public void CacherImage(string Fichier)
        {
            MatriceRGB[,] newImage = new MatriceRGB[hauteur, largeur];

            for(int i = 0; i < hauteur; i++)
            {
                for(int j = 0; j < largeur; j++)
                {
                    newImage[i, j] = new MatriceRGB(100, 100, 100);
                }
            }
           
            byte[] MonNouveauFichier = File.ReadAllBytes(Fichier);

            int NewLargeur = Convertir_Endian_To_Int(MonNouveauFichier, 18, 21);
            int NewHauteur = Convertir_Endian_To_Int(MonNouveauFichier, 22, 25);

            MatriceRGB[,] ImageCachée = new MatriceRGB[NewHauteur, NewLargeur];
            int compteur = 54;
            for (int x = 0; x < NewHauteur; x++)
            {
                for (int y = 0; y < NewLargeur; y++)
                {
                    ImageCachée[x, y] = new MatriceRGB(MonNouveauFichier[compteur], MonNouveauFichier[compteur + 1], MonNouveauFichier[compteur + 2]);

                    compteur += 3;
                }
            }
            

            for (int i = 0; i < hauteur; i++)
            {
                for(int j = 0; j < largeur; j++)
                {
                    if(i<ImageCachée.GetLength(0) && j<ImageCachée.GetLength(1))
                    {
                        newImage[i, j].Red = Convert.ToByte(Convert_Binary_To_Int(Convert_Int_To_Binary(image[i, j].Red, ImageCachée[i, j].Red)));
                        newImage[i, j].Green = Convert.ToByte(Convert_Binary_To_Int(Convert_Int_To_Binary(image[i, j].Green, ImageCachée[i, j].Green)));
                        newImage[i, j].Blue = Convert.ToByte(Convert_Binary_To_Int(Convert_Int_To_Binary(image[i, j].Blue, ImageCachée[i, j].Blue)));
                    }
                    else
                    {
                        newImage[i, j].Red = image[i, j].Red;
                        newImage[i, j].Green = image[i, j].Green;
                        newImage[i, j].Blue = image[i, j].Blue;
                    }
                    
                }
            }

            AfficherImageBis(newImage, "Image_Cachée.bmp", hauteur, largeur);
        }

        /// <summary>
        /// Méthode qui décode une image et retrouve l'image cachée
        /// </summary>
        /// <param name="ImageCachée">Image contenant une image cachée</param>
        public void DecoderImage(string ImageCachée)
        {
            MatriceRGB[,] newImage = new MatriceRGB[hauteur, largeur];

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    newImage[i, j] = new MatriceRGB(100, 100, 100);
                }
            }

            byte[] MonNouveauFichier = File.ReadAllBytes(ImageCachée);
                      
            MatriceRGB[,] ImageHide = new MatriceRGB[hauteur, largeur];
            int compteur = 54;
            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    ImageHide[x, y] = new MatriceRGB(MonNouveauFichier[compteur], MonNouveauFichier[compteur + 1], MonNouveauFichier[compteur + 2]);

                    compteur += 3;
                }
            }

            Console.WriteLine("Connaissez-vous les dimensions de l'image cachée ?");
            Console.WriteLine("0 - Oui");
            Console.WriteLine("1 - Non");
            int Choix = Convert.ToInt32(Console.ReadLine());
            int LargeurImageCachée = 0;
            int HauteurImageCachée = 0;
            if (Choix == 0)
            {
                Console.WriteLine("Quelle est la largeur de l'image cachée ?");
                LargeurImageCachée = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Quelle est la hauteur de l'image cachée ?");
                HauteurImageCachée = Convert.ToInt32(Console.ReadLine());
            }
            else
            {
                LargeurImageCachée = largeur;
                HauteurImageCachée = hauteur;
            }
            

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    if (i < HauteurImageCachée && j <LargeurImageCachée)
                    {
                        newImage[i, j].Red = Convert.ToByte(Convert_Binary_To_Int(Convert_Int_To_Binary_Bis(ImageHide[i,j].Red)));
                        newImage[i, j].Green = Convert.ToByte(Convert_Binary_To_Int(Convert_Int_To_Binary_Bis(ImageHide[i, j].Green)));
                        newImage[i, j].Blue = Convert.ToByte(Convert_Binary_To_Int(Convert_Int_To_Binary_Bis(ImageHide[i, j].Blue)));
                    }
                    else
                    {
                        newImage[i, j].Red = 255;
                        newImage[i, j].Green = 255;
                        newImage[i, j].Blue = 255;
                    }

                }
            }
            AfficherImageBis(newImage, "Image_Décodée.bmp", hauteur, largeur);
        }

        /// <summary>
        /// Méthode qui créer un histogramme de la photo traitée
        /// </summary>
        public void Histogramme()
        {

            Console.WriteLine("Quel histogramme voulez-vous ?"); //(Bleu,Vert,Rouge)
            Console.WriteLine("1 - Blue");
            Console.WriteLine("2 - Vert");
            Console.WriteLine("3 - Rouge");
            Console.WriteLine("4 - Moyenne des couleurs (Gris)");
            int Choix = Convert.ToInt32(Console.ReadLine());

            MatriceRGB Bleu = new MatriceRGB(255, 0, 0);
            MatriceRGB Vert = new MatriceRGB(0, 255, 0);
            MatriceRGB Rouge = new MatriceRGB(0, 0, 255);
            MatriceRGB Gris = new MatriceRGB(192, 192, 192);
            MatriceRGB Couleur = new MatriceRGB(0, 0, 0);


            int[] TableauHistogramme = new int[256];
            int MoyenneDuPixel = 0;

            if (Choix == 1)
            {
                for (int x = 0; x < hauteur; x++)
                {
                    for (int y = 0; y < largeur; y++)
                    {
                        MoyenneDuPixel = image[x, y].Blue;
                        TableauHistogramme[MoyenneDuPixel]++;
                    }
                }
                Couleur = Bleu;
            }
            if (Choix == 2)
            {
                for (int x = 0; x < hauteur; x++)
                {
                    for (int y = 0; y < largeur; y++)
                    {
                        MoyenneDuPixel = image[x, y].Green;
                        TableauHistogramme[MoyenneDuPixel]++;
                    }
                }
                Couleur = Vert;
            }
            if (Choix == 3)
            {
                for (int x = 0; x < hauteur; x++)
                {
                    for (int y = 0; y < largeur; y++)
                    {
                        MoyenneDuPixel = image[x, y].Red;
                        TableauHistogramme[MoyenneDuPixel]++;
                    }
                }
                Couleur = Rouge;
            }
            if (Choix == 4)
            {
                for (int x = 0; x < hauteur; x++)
                {
                    for (int y = 0; y < largeur; y++)
                    {
                        MoyenneDuPixel = (image[x, y].Blue + image[x, y].Green + image[x, y].Blue) / 3;
                        TableauHistogramme[MoyenneDuPixel]++;
                    }
                }
                Couleur = Gris;
            }

            int Max = 0;
            for (int i = 0; i < 256; i++)
            {
                int Valeur = TableauHistogramme[i];
                if (Valeur > Max)
                {
                    Max = Valeur;
                }
            }
            while (Max % 4 != 0)
            {
                Max++;
            }


            int HauteurHistogramme = Max + 16;
            int LargeurHistogramme = 256 * 8 + 32;

            MatriceRGB[,] Histogramme = new MatriceRGB[HauteurHistogramme, LargeurHistogramme];


            for (int i = 0; i < HauteurHistogramme; i++)
            {
                for (int j = 0; j < LargeurHistogramme; j++)
                {
                    Histogramme[i, j] = new MatriceRGB(255, 255, 255);
                }
            }

            int Compteur = 0;
            for (int y = 16; y < 256 * 8; y = y + 10)
            {
                for (int x = 0; x < TableauHistogramme[Compteur]; x++)
                {
                    Histogramme[x, y] = Couleur;
                    Histogramme[x, y + 1] = Couleur;
                    Histogramme[x, y + 2] = Couleur;
                    Histogramme[x, y + 3] = Couleur;
                    Histogramme[x, y + 4] = Couleur;
                    Histogramme[x, y + 5] = Couleur;
                    Histogramme[x, y + 6] = Couleur;
                    Histogramme[x, y + 7] = Couleur;
                }
                Compteur++;
            }
            AfficherImageBis(Histogramme, "Histogramme.bmp", HauteurHistogramme, LargeurHistogramme);
        }

        /// <summary>
        /// Méthode qui permet de rogner une photo
        /// </summary>
        public void Rogner()
        {
            MatriceRGB[,] ImageRognée = new MatriceRGB[hauteur, largeur];

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    ImageRognée[i, j] = new MatriceRGB(255, 255, 255);
                }
            }
            Console.WriteLine("INFORMATION IMPORTANTE : Le point (0,0) est le point du coin bas-gauche");
            Console.WriteLine("Quel est le point bas-gauche du rognage ?");
            Console.WriteLine(" X = ");
            int X1 = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Y = ");
            int Y1 = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Le point bas-gauche est (" + X1 + ";" + Y1 + ")");
            Console.WriteLine("Quel est le point haut-droit du rognage ?");
            Console.WriteLine(" X = ");
            int X2 = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Y = ");
            int Y2 = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Le point haut-droite est (" + X2 + ";" + Y2 + ")");

            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    if (x > X1 && x < X2 && y > Y1 && y < Y2)
                    {
                        ImageRognée[x, y].Blue = image[x, y].Blue;
                        ImageRognée[x, y].Green = image[x, y].Green;
                        ImageRognée[x, y].Red = image[x, y].Red;
                    }
                }
            }

            int NewLargeur = X2 - X1;
            int NewHauteur = Y2 - Y1;

            while (NewLargeur % 4 != 0)
            {
                NewLargeur = NewLargeur + 1;
            }
            while (NewHauteur % 4 != 0)
            {
                NewHauteur = NewHauteur + 1;
            }


            AfficherImageBis(ImageRognée, "ImageRognée.bmp", NewHauteur, NewLargeur);        }

        /// <summary>
        /// Méthode pour séparer partie réelle et imaginaire d'un nombre complexe
        /// </summary>
        /// <param name="Complexe">Nombre complexe</param>
        public double[] Carré(double[] Complexe)
        {
            double Tampon = (Complexe[0] * Complexe[0]) - (Complexe[1] * Complexe[1]);
            Complexe[1] = 2.0 * Complexe[0] * Complexe[1];
            Complexe[0] = Tampon;
            return Complexe;
        }

        /// <summary>
        /// Calcul de la norme d'un nombre complexe (Condition d'arrêt)
        /// </summary>
        /// <param name="Complexe">Nombre complexe</param>
        public double Norme(double[] Complexe)
        {
            return Math.Sqrt((Complexe[0] * Complexe[0]) + (Complexe[1] * Complexe[1]));
        }

        /// <summary>
        /// Méthode qui fait la somme de deux nombres complexes
        /// </summary>
        /// <param name="Complexe1">Nombre complexe 1</param>
        /// <param name="Complexe2">Nombre complexe 2</param>
        public double[] SommeComplexe(double[] Complexe1, double[] Complexe2)
        {
            double[] Resultat = new double[2];
            Resultat[0] = Complexe1[0] + Complexe2[0];
            Resultat[1] = Complexe1[1] + Complexe2[1];
            return Resultat;
        }

        /// <summary>
        /// Méthode qui créé et affiche la fractale de Mandelbrot
        /// </summary>
        public void Fractale()
        {
            Console.WriteLine("Hauteur de l'image (3200 conseillé)");
            int HauteurFractale = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Largeur de l'image (3200 conseillé)");
            int LargeurFractale = Convert.ToInt32(Console.ReadLine());

            MatriceRGB[,] ImageFractale = new MatriceRGB[HauteurFractale, LargeurFractale];

            for (int i = 0; i < HauteurFractale; i++)
            {
                for (int j = 0; j < LargeurFractale; j++)
                {
                    ImageFractale[i, j] = new MatriceRGB(255, 255, 255);
                }
            }

            for (int x = 0; x < HauteurFractale; x++)
            {
                for (int y = 0; y < LargeurFractale; y++)
                {
                    double a = (double)(x - (HauteurFractale / 2)) / (double)(HauteurFractale / 4);
                    double b = (double)(y - (LargeurFractale / 2)) / (double)(LargeurFractale / 4);
                    double[] c = { a, b };
                    double[] z = { 0, 0 };
                    int Compteur = 0;
                    while (Compteur < 100)
                    {
                        Compteur++;
                        z = Carré(z);
                        z = SommeComplexe(z, c);
                        if (Norme(z) > 2.0) break;
                    }
                    if (Compteur < 100)
                    {
                        ImageFractale[x, y] = new MatriceRGB(0, 0, 0);
                    }
                    else
                    {
                        ImageFractale[x, y] = new MatriceRGB(255, 255, 255);
                    }
                }
            }
            AfficherImageBis(ImageFractale, "Fractale.bmp", HauteurFractale, LargeurFractale);
        }

        /// <summary>
        /// Méthode convertissant un entier en binaire selon la taille donnée
        /// </summary>
        /// <param name="val">Entier à convertir</param>
        /// <param name="val">Taille de l'entier converti (8bits, 9bits...)</param>
        /// <returns>Entier converti</returns>
        public byte[] Convertir_Int_Binaire(int val, int taille)
        {
            byte[] bit = new byte[taille];
            int cal = 0;
            for (int i = taille - 1; i > -1; i--)
            {
                cal = Convert.ToInt32(Math.Truncate(val / (Math.Pow(2, i))));
                bit[taille - 1 - i] = Convert.ToByte(cal);
                val = Convert.ToInt32(val % (Math.Pow(2, i)));
            }
            return bit;
        }

        /// <summary>
        /// Méthode qui transforme une phrase en une suite de 0 et de 1 afin de créer le QR Code
        /// </summary>
        /// <param name="Phrase">Phrase qui va être codée dans le QR Code</param>
        /// <returns>Les données d'informations du QR Code (sans les bytes de correction)</returns>
        public byte[] DonnéesQRCode(string Phrase)
        {
            int TaillePhrase = Phrase.Length;
            byte[] TaillePhrase9bits = Convertir_Int_Binaire(TaillePhrase, 9);
            int[] TableauPhrase = new int[Phrase.Length];
            string TableauAlphanumerique = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";

            //Traduction de la phrase en alphanumérique

            for (int i = 0; i < Phrase.Length; i++)
            {
                char Lettre = Phrase[i];
                for (int j = 0; j < TableauAlphanumerique.Length; j++)
                {
                    if (Lettre == TableauAlphanumerique[j])
                    {
                        TableauPhrase[i] = j;
                    }
                }
            }

            //Création de la chaine de données binaire

            int Entier2Lettres = 0;
            string Données = "0010";

            for (int i = 0; i < 9; i++)
            {
                Données = Données + TaillePhrase9bits[i];
            }
            if (Phrase.Length % 2 == 0)
            {
                for (int i = 0; i < Phrase.Length; i = i + 2)
                {
                    Entier2Lettres = Convert.ToInt32((Math.Pow(45, 1) * TableauPhrase[i]) + (Math.Pow(45, 0) * TableauPhrase[i + 1]));
                    byte[] Tampon = Convertir_Int_Binaire(Entier2Lettres, 11);
                    for (int j = 0; j < 11; j++)
                    {
                        Données = Données + Tampon[j];
                    }
                }
            }
            else
            {
                for (int i = 0; i < Phrase.Length; i = i + 2)
                {
                    if (i == Phrase.Length - 1)
                    {
                        Entier2Lettres = Convert.ToInt32(Math.Pow(45, 0) * TableauPhrase[i]);
                        byte[] Tampon2 = Convertir_Int_Binaire(Entier2Lettres, 6);
                        for (int j = 0; j < 6; j++)
                        {
                            Données = Données + Tampon2[j];
                        }
                    }
                    else
                    {
                        Entier2Lettres = Convert.ToInt32((Math.Pow(45, 1) * TableauPhrase[i]) + (Math.Pow(45, 0) * TableauPhrase[i + 1]));
                        byte[] tampon = Convertir_Int_Binaire(Entier2Lettres, 11);
                        for (int j = 0; j < 11; j++)
                        {
                            Données = Données + tampon[j];
                        }
                    }
                }
            }

            //Ajout de la terminaison et incrémentation pour obtenir une longueur multiple de 8

            Données = Données + "0000";

            while (Données.Length % 8 != 0)
            {
                Données = Données + "0";
            }

            //Ajout des entiers 236 et 17 en binaire

            int Iteration = (152 - Données.Length) / 8;
            int Compteur = 0;

            for (int i = 0; i < Iteration; i++)
            {
                if (Compteur % 2 == 0)
                {
                    Données = Données + "11101100";
                }
                else
                {
                    Données = Données + "00010001";
                }
                Compteur++;
            }

            //Conversion de la chaine de donnée de string à byte

            byte[] DonnéesFinales = new byte[Données.Length];

            for (int i = 0; i < Données.Length; i++)
            {
                if (Données[i] == '0')
                {
                    DonnéesFinales[i] = 0;
                }
                else
                {
                    DonnéesFinales[i] = 1;
                }
            }
            return DonnéesFinales;
        }

        /// <summary>
        /// Méthode qui créé les parties invariables d'un QR Code (motifs de recherche, d'alignement...)
        /// </summary>
        /// <returns>Image correspondant au fond d'un QR Code</returns>
        public MatriceRGB[,] FondQRCode()
        {
            MatriceRGB[,] Fond = new MatriceRGB[21, 21];
            MatriceRGB Noir = new MatriceRGB(0, 0, 0);
            for (int i = 0; i < 21; i++)
            {
                for (int j = 0; j < 21; j++)
                {
                    Fond[i, j] = new MatriceRGB(255, 255, 255);
                }
            }
            for (int i = 0; i < 7; i++)
            {
                Fond[0, i] = Noir;
                Fond[6, i] = Noir;
                Fond[i, 0] = Noir;
                Fond[i, 6] = Noir;

                Fond[14, i] = Noir;
                Fond[14 + i, 6] = Noir;
                Fond[14 + i, 0] = Noir;
                Fond[20, i] = Noir;

                Fond[14, 14 + i] = Noir;
                Fond[14 + i, 14] = Noir;
                Fond[20, 14 + i] = Noir;
                Fond[i + 14, 20] = Noir;
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Fond[i + 2, j + 2] = Noir;
                    Fond[i + 16, j + 2] = Noir;
                    Fond[i + 16, j + 16] = Noir;
                }
            }
            Fond[8, 6] = Noir;
            Fond[10, 6] = Noir;
            Fond[12, 6] = Noir;
            Fond[14, 8] = Noir;
            Fond[14, 10] = Noir;
            Fond[14, 12] = Noir;
            Fond[7, 8] = Noir;

            Fond[12, 0] = Noir;
            Fond[12, 1] = Noir;
            Fond[12, 2] = Noir;
            Fond[12, 4] = Noir;
            Fond[12, 5] = Noir;
            Fond[12, 7] = Noir;
            Fond[12, 8] = Noir;
            Fond[13, 8] = Noir;
            Fond[18, 8] = Noir;

            Fond[0, 8] = Noir;
            Fond[1, 8] = Noir;
            Fond[2, 8] = Noir;
            Fond[4, 8] = Noir;
            Fond[5, 8] = Noir;
            Fond[6, 8] = Noir;
            Fond[12, 13] = Noir;
            Fond[12, 14] = Noir;
            Fond[12, 18] = Noir;

            return Fond;
        }

        /// <summary>
        /// Méthode qui applique le masque 1 et qui écrit les données dans le QR Code
        /// </summary>
        /// <param name="Info">Tableau de byte correspondant aux données qui vont être convertit en QR Code</param>
        /// <param name="FondQRCode">Image qui correspond aux parties invariables d'un QR Code</param>
        /// <returns>QR Code</returns>
        public MatriceRGB[,] Masque(byte[] Info, MatriceRGB[,] FondQRCode)
        {
            MatriceRGB Noir = new MatriceRGB(0, 0, 0);
            int Compteur = 0;

            //Colonne 1
            for (int i = 0; i < 12; i++)
            {
                if ((i + 20) % 2 == 1) //Formule du masque 001 : (x+y)%2
                {
                    if (Info[Compteur] == 1)
                    {
                        FondQRCode[i, 20] = Noir;
                    }
                }
                if ((i + 20) % 2 == 0)
                {
                    if (Info[Compteur] == 0)
                    {
                        FondQRCode[i, 20] = Noir;
                    }
                }

                if ((i + 19) % 2 == 1)
                {
                    if (Info[Compteur + 1] == 1)
                    {
                        FondQRCode[i, 19] = Noir;
                    }
                }
                if ((i + 19) % 2 == 0)
                {
                    if (Info[Compteur + 1] == 0)
                    {
                        FondQRCode[i, 19] = Noir;
                    }
                }
                Compteur += 2;
            }
            //Colonne 2
            for (int i = 11; i > -1; i--)
            {
                if ((i + 18) % 2 == 1)
                {
                    if (Info[Compteur] == 1)
                    {
                        FondQRCode[i, 18] = Noir;
                    }
                }
                if ((i + 18) % 2 == 0)
                {
                    if (Info[Compteur] == 0)
                    {
                        FondQRCode[i, 18] = Noir;
                    }
                }

                if ((i + 17) % 2 == 1)
                {
                    if (Info[Compteur + 1] == 1)
                    {
                        FondQRCode[i, 17] = Noir;
                    }
                }
                if ((i + 17) % 2 == 0)
                {
                    if (Info[Compteur + 1] == 0)
                    {
                        FondQRCode[i, 17] = Noir;
                    }
                }
                Compteur += 2;
            }
            //Colonne 3
            for (int i = 0; i < 12; i++)
            {
                if ((i + 16) % 2 == 1)
                {
                    if (Info[Compteur] == 1)
                    {
                        FondQRCode[i, 16] = Noir;
                    }
                }
                if ((i + 16) % 2 == 0)
                {
                    if (Info[Compteur] == 0)
                    {
                        FondQRCode[i, 16] = Noir;
                    }
                }

                if ((i + 15) % 2 == 1)
                {
                    if (Info[Compteur + 1] == 1)
                    {
                        FondQRCode[i, 15] = Noir;
                    }
                }
                if ((i + 15) % 2 == 0)
                {
                    if (Info[Compteur + 1] == 0)
                    {
                        FondQRCode[i, 15] = Noir;
                    }
                }
                Compteur += 2;
            }
            //Colonne 4
            for (int i = 11; i > -1; i--)
            {
                if ((i + 14) % 2 == 1)
                {
                    if (Info[Compteur] == 1)
                    {
                        FondQRCode[i, 14] = Noir;
                    }
                }
                if ((i + 14) % 2 == 0)
                {
                    if (Info[Compteur] == 0)
                    {
                        FondQRCode[i, 14] = Noir;
                    }
                }

                if ((i + 13) % 2 == 1)
                {
                    if (Info[Compteur + 1] == 1)
                    {
                        FondQRCode[i, 13] = Noir;
                    }
                }
                if ((i + 13) % 2 == 0)
                {
                    if (Info[Compteur + 1] == 0)
                    {
                        FondQRCode[i, 13] = Noir;
                    }
                }
                Compteur += 2;
            }
            //Colonne 5
            for (int i = 0; i < 14; i++)
            {
                if ((i + 12) % 2 == 1)
                {
                    if (Info[Compteur] == 1)
                    {
                        FondQRCode[i, 12] = Noir;
                    }
                }
                if ((i + 12) % 2 == 0)
                {
                    if (Info[Compteur] == 0)
                    {
                        FondQRCode[i, 12] = Noir;
                    }
                }

                if ((i + 11) % 2 == 1)
                {
                    if (Info[Compteur + 1] == 1)
                    {
                        FondQRCode[i, 11] = Noir;
                    }
                }
                if ((i + 11) % 2 == 0)
                {
                    if (Info[Compteur + 1] == 0)
                    {
                        FondQRCode[i, 11] = Noir;
                    }
                }
                Compteur += 2;
            }
            for (int i = 15; i < 21; i++)
            {
                if ((i + 12) % 2 == 1)
                {
                    if (Info[Compteur] == 1)
                    {
                        FondQRCode[i, 12] = Noir;
                    }
                }
                if ((i + 12) % 2 == 0)
                {
                    if (Info[Compteur] == 0)
                    {
                        FondQRCode[i, 12] = Noir;
                    }
                }

                if ((i + 11) % 2 == 1)
                {
                    if (Info[Compteur + 1] == 1)
                    {
                        FondQRCode[i, 11] = Noir;
                    }
                }
                if ((i + 11) % 2 == 0)
                {
                    if (Info[Compteur + 1] == 0)
                    {
                        FondQRCode[i, 11] = Noir;
                    }
                }
                Compteur += 2;
            }
            //Colonne 6
            for (int i = 20; i > 14; i--)
            {
                if ((i + 10) % 2 == 1)
                {
                    if (Info[Compteur] == 1)
                    {
                        FondQRCode[i, 10] = Noir;
                    }
                }
                if ((i + 10) % 2 == 0)
                {
                    if (Info[Compteur] == 0)
                    {
                        FondQRCode[i, 10] = Noir;
                    }
                }

                if ((i + 9) % 2 == 1)
                {
                    if (Info[Compteur + 1] == 1)
                    {
                        FondQRCode[i, 9] = Noir;
                    }
                }
                if ((i + 9) % 2 == 0)
                {
                    if (Info[Compteur + 1] == 0)
                    {
                        FondQRCode[i, 9] = Noir;
                    }
                }
                Compteur += 2;
            }
            for (int i = 13; i > -1; i--)
            {
                if ((i + 10) % 2 == 1)
                {
                    if (Info[Compteur] == 1)
                    {
                        FondQRCode[i, 10] = Noir; ;
                    }
                }
                if ((i + 10) % 2 == 0)
                {
                    if (Info[Compteur] == 0)
                    {
                        FondQRCode[i, 10] = Noir;
                    }
                }

                if ((i + 9) % 2 == 1)
                {
                    if (Info[Compteur + 1] == 1)
                    {
                        FondQRCode[i, 9] = Noir;
                    }
                }
                if ((i + 9) % 2 == 0)
                {
                    if (Info[Compteur + 1] == 0)
                    {
                        FondQRCode[i, 9] = Noir;
                    }
                }
                Compteur += 2;
            }
            //Colonne 7
            for (int i = 8; i < 12; i++)
            {
                if ((i + 8) % 2 == 1)
                {
                    if (Info[Compteur] == 1)
                    {
                        FondQRCode[i, 8] = Noir;
                    }
                }
                if ((i + 8) % 2 == 0)
                {
                    if (Info[Compteur] == 0)
                    {
                        FondQRCode[i, 8] = Noir;
                    }
                }

                if ((i + 7) % 2 == 1)
                {
                    if (Info[Compteur + 1] == 1)
                    {
                        FondQRCode[i, 7] = Noir;
                    }
                }
                if ((i + 7) % 2 == 0)
                {
                    if (Info[Compteur + 1] == 0)
                    {
                        FondQRCode[i, 7] = Noir;
                    }
                }
                Compteur += 2;
            }
            //Colonne 8
            for (int i = 11; i > 7; i--)
            {
                if ((i + 5) % 2 == 1)
                {
                    if (Info[Compteur] == 1)
                    {
                        FondQRCode[i, 5] = Noir;
                    }
                }
                if ((i + 5) % 2 == 0)
                {
                    if (Info[Compteur] == 0)
                    {
                        FondQRCode[i, 5] = Noir;
                    }
                }

                if ((i + 4) % 2 == 1)
                {
                    if (Info[Compteur + 1] == 1)
                    {
                        FondQRCode[i, 4] = Noir;
                    }
                }
                if ((i + 4) % 2 == 0)
                {
                    if (Info[Compteur + 1] == 0)
                    {
                        FondQRCode[i, 4] = Noir;
                    }
                }
                Compteur += 2;
            }
            //Colonne 9
            for (int i = 8; i < 12; i++)
            {
                if ((i + 3) % 2 == 1)
                {
                    if (Info[Compteur] == 1)
                    {
                        FondQRCode[i, 3] = Noir;
                    }
                }
                if ((i + 3) % 2 == 0)
                {
                    if (Info[Compteur] == 0)
                    {
                        FondQRCode[i, 3] = Noir;
                    }
                }

                if ((i + 2) % 2 == 1)
                {
                    if (Info[Compteur + 1] == 1)
                    {
                        FondQRCode[i, 2] = Noir;
                    }
                }
                if ((i + 2) % 2 == 0)
                {
                    if (Info[Compteur + 1] == 0)
                    {
                        FondQRCode[i, 2] = Noir;
                    }
                }
                Compteur += 2;
            }
            //Colonne 10
            for (int i = 11; i > 7; i--)
            {
                if ((i + 1) % 2 == 1)
                {
                    if (Info[Compteur] == 1)
                    {
                        FondQRCode[i, 1] = Noir;
                    }
                }
                if ((i + 1) % 2 == 0)
                {
                    if (Info[Compteur] == 0)
                    {
                        FondQRCode[i, 1] = Noir;
                    }
                }

                if ((i + 0) % 2 == 1)
                {
                    if (Info[Compteur + 1] == 1)
                    {
                        FondQRCode[i, 0] = Noir;
                    }
                }
                if ((i + 0) % 2 == 0)
                {
                    if (Info[Compteur + 1] == 0)
                    {
                        FondQRCode[i, 0] = Noir;
                    }
                }
                Compteur += 2;
            }
            return FondQRCode;
        }

        /// <summary>
        /// Méthode qui aggrandit et affiche
        /// </summary>
        /// <param name="QRCode">QR Code à aggrandir </param>
        public void AgrandissementQRCode(MatriceRGB[,] QRCode)
        {
            int Facteur = 12;

            MatriceRGB[,] ImageAgrandie = new MatriceRGB[QRCode.GetLength(0) * Facteur, QRCode.GetLength(1) * Facteur];

            for (int x = 0; x < 21; x++)
            {
                for (int y = 0; y < 21; y++)
                {
                    for (int i = 0; i < Facteur; i++)
                    {
                        for (int j = 0; j < Facteur; j++)
                        {
                            ImageAgrandie[x * Facteur + i, y * Facteur + j] = QRCode[x, y];
                        }
                    }
                }
            }
            AfficherImageBis(ImageAgrandie, "QRCode.bmp", ImageAgrandie.GetLength(0), ImageAgrandie.GetLength(1));
        }
    }
}
