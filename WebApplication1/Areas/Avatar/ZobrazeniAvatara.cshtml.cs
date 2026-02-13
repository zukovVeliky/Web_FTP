using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using System.Net;

namespace Avatar
{
    public class ZobrazeniAvataraModel : PageModel
    {

        private string imgPath = "";

        private string imgUrl = "";
        public string ImgURL
        {
            get
            {
                return this.imgUrl;
            }
            set
            {
                this.imgUrl = value;
            }
        }
        public string Width { get; set; } = "300";
        public string WidthPX { get { return WidthEditor + "px"; } }
        public string Height { get; set; } = "300";
        public string typZobrazeni { get; set; }= "circle";
        public string StyleCircle { get; set; } = "";

        public string Transform { get { return poleHodnotZobrazeni[0].Replace(',', '.'); } }
        public string TopPX { get { return poleHodnotZobrazeni[2].Replace(',', '.') + "px"; } }
        public string LeftPX { get { return poleHodnotZobrazeni[1].Replace(',', '.') + "px"; } }



        private string WidthEditor { get; set; } = "50";
        private string HeightEditor { get; set; } = "50";



        /// <summary>
        /// pole vraci nastaveni nahledu uyivatele
        /// 0 - url obrazku
        /// 1 - X0 parvy horni roh
        /// 2 - Y0 pravy horni roh
        /// 3 - X1 pravy spodni roh
        /// 4 - Y1 pravy spodni roh
        /// 5 - zoom
        /// 6 - width editoru
        /// 7 - heigth editoru
        /// </summary>
        private string[] poleHodnotNastaveni { get; set; } = { "", "", "", "", "", "", "","" };
        /// <summary>
        ///       
        /// pøepoèítané hodnoty pro zobrazení
        /// 
        /// 0 - transform
        /// 1 - left
        /// 2 - top
        /// </summary>
        private string[] poleHodnotZobrazeni { get; set; } = { "", "", "" };



        /// <summary>
        /// 
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="velikost">celikost zobrazeni</param>
        /// <param name="Nastaveni"></param>
        public ZobrazeniAvataraModel(string ServerRooth, string URL,string velikost, string Nastaveni)
        {
            if (Nastaveni != null)
            {
                poleHodnotNastaveni = Nastaveni.Split('~');

                string AvatarIMG = poleHodnotNastaveni[0];
                ////  kontrola existence souboru na servru
                string CestaAvatara = Path.Combine(ServerRooth, AvatarIMG);
                if (System.IO.File.Exists(CestaAvatara))
                {

                }
                else
                {
                    AvatarIMG = Path.Combine("Images", "Foto_Avatar", "1000.jpg");
                    AvatarIMG = AvatarIMG.Replace("\\", "/");
                    CestaAvatara = Path.Combine(ServerRooth, AvatarIMG);
                }
                ////
                this.typZobrazeni = "circle";
                this.StyleCircle = "border-radius:50%;";
                WidthEditor = velikost;
                Height = velikost;
                ImgURL = URL + "/" + AvatarIMG;  //"/UzivatelskeSoubory/AvatarFoto/"+ Path.GetFileName(CestaAvatara);
                imgPath = CestaAvatara.Replace("/", "\\");
                Width = poleHodnotNastaveni[6];


                poleHodnotZobrazeni = this.NastaveniAvatara().Split('~');
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="velikost">celikost zobrazeni</param>
        /// <param name="Nastaveni"></param>
        public ZobrazeniAvataraModel(string ServerRooth, string URL, string velikost_W,bool ZobrazeniCtverce, string Nastaveni)
        {

            poleHodnotNastaveni = Nastaveni.Split('~');

            string AvatarIMG = poleHodnotNastaveni[0];
            ////  kontrola existence souboru na servru
            string CestaAvatara = Path.Combine(ServerRooth, AvatarIMG);
            if (System.IO.File.Exists(CestaAvatara))
            {

            }
            else
            {
                AvatarIMG = Path.Combine("Images", "Foto_Avatar", "1000.jpg");
                AvatarIMG = AvatarIMG.Replace("\\", "/");
                CestaAvatara = Path.Combine(ServerRooth, AvatarIMG);
            }
            ////

            WidthEditor = velikost_W;
          
            ImgURL = URL + "/" + AvatarIMG;  //"/UzivatelskeSoubory/AvatarFoto/"+ Path.GetFileName(CestaAvatara);
            imgPath = CestaAvatara.Replace("/", "\\");
            Width = poleHodnotNastaveni[6];


            if (ZobrazeniCtverce)
            {
                this.typZobrazeni = "square";
                double transform = (double.Parse(this.WidthEditor)/double.Parse(this.Width) );
                int x = (int)(int.Parse(poleHodnotNastaveni[7]) * transform);
                this.Height = x.ToString();
                
            }
            else
            {

                this.typZobrazeni = "circle";
                this.Height = this.Width;
                this.StyleCircle = "border-radius:50%;";
            }
            
 
            poleHodnotNastaveni[0] = CestaAvatara;

           // poleHodnotNastaveni[6] = this.Width;
            //oleHodnotNastaveni[7] = this.Height;




            poleHodnotZobrazeni = this.NastaveniAvatara().Split('~');

        }
        public string PixelValue(string arg) { return arg + "px"; }

        private string NastaveniAvatara()
        {

 
            double zoom = double.Parse(this.poleHodnotNastaveni[5].Replace('.', ','));

            double transform = (zoom / (double.Parse(this.Width) / double.Parse(this.WidthEditor)));
            System.Drawing.Image img = System.Drawing.Image.FromFile(imgPath);
            double left = ((-1) * (((double.Parse(img.Width.ToString()) - (double.Parse(img.Width.ToString()) / (1 / transform))) / 2) + (double.Parse(this.poleHodnotNastaveni[1]) / (1 / transform))));
            double top = ((-1) * (((double.Parse(img.Height.ToString()) - (double.Parse(img.Height.ToString()) / (1 / transform))) / 2) + (double.Parse(this.poleHodnotNastaveni[2]) / (1 / transform))));


            string vystup =transform.ToString() + "~" + left.ToString() + "~" + top.ToString();

            string Test = test();

            return transform.ToString() + "~" + left.ToString() + "~" + top.ToString();

        }        

        public string test ()
        {
            double zoom = double.Parse(this.poleHodnotNastaveni[5].Replace('.', ','));

            double transform = zoom; // Transformace je pøímo hodnota zoomu

            System.Drawing.Image img = System.Drawing.Image.FromFile(imgPath);

            double left, top;

            if (img.Width >= img.Height)
            {
                left = ((-1) * (((img.Width - (img.Width / transform)) / 2) + (double.Parse(this.poleHodnotNastaveni[1]) / transform)));
                top = ((-1) * (((img.Height - (img.Height / transform)) / 2) + (double.Parse(this.poleHodnotNastaveni[2]) / transform)));
            }
            else
            {
                left = ((-1) * (((img.Width - (img.Width / transform)) / 2) + (double.Parse(this.poleHodnotNastaveni[1]) / transform)));
                top = ((-1) * (((img.Height - (img.Height / transform)) / 2) + (double.Parse(this.poleHodnotNastaveni[2]) / transform)));
            }

            return transform.ToString() + "~" + left.ToString() + "~" + top.ToString();
        }
        
        public void OnGet()
        {
        }
    }
}
