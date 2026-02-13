using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Drawing;
using Microsoft.AspNetCore.Hosting;

namespace Avatar
{
    public class EditaceAvataraModel : PageModel
    {

        public string ScriptButton1 { get; private set; }
        private string UrlWEB { get; set; }

        public bool ZobrazCtverec { get; set; } = false;
        public string typZobrazeni { get; set; } = "circle"; //square or circle

        public string ImgURL { get; set; } = "Images/Foto_Avatar/1000.jpg";
        public string Width { get; set; } = "300";
        public string Height { get; set; }
        /// <summary>
        /// pole vraci nastaveni nahledu uyivatele
        /// 0 - url obrazku
        /// 1 - X0 parvy horni roh
        /// 2 - Y0 pravy horni roh
        /// 3 - X1 pravy spodni roh
        /// 4 - Y1 pravy spodni roh
        /// 5 - zoom
        /// 6 - width editoru
        /// 7 - height editoru
        /// </summary>
        public string[] poleHodnotNastaveni { get; 
            private set; } = { "", "", "", "", "", "", "","" };
        public string stringHodnotNastaveni = "";
        public string _ServerRooth { get; set; }
        //vlstnost generujici nahodne cislo
        public string Modifikator { get; set; } = "";

        public string PixelValue(string arg) { return arg + "px"; }


        //modifikator je nepovinny parametr, ktery se použije pro rozlišení vpøi více instancích na jedné stránce
        public EditaceAvataraModel(string ServerRooth, Microsoft.AspNetCore.Http.HttpRequest Req, string Nastaveni, string URL, bool ZobrazeniCtverce, string width, string height, string modifikator ="")
        {
            this.Modifikator = modifikator;
            _ServerRooth = ServerRooth;
            if (ZobrazeniCtverce)
            {
                this.typZobrazeni = "square";
                this.Width = width;
                this.Height = height;
            }
            else
            {
                this.typZobrazeni = "circle";
                this.Height = this.Width;
            }
            stringHodnotNastaveni = Nastaveni;

            poleHodnotNastaveni = Nastaveni.Split('~');



            this.INIT(Req, URL);

        }
        public EditaceAvataraModel(string ServerRooth, Microsoft.AspNetCore.Http.HttpRequest Req, string Nastaveni, string URL, string modifikator = "")
        {
            this.Modifikator = modifikator;
            _ServerRooth = ServerRooth;
            stringHodnotNastaveni = Nastaveni;

            poleHodnotNastaveni = Nastaveni.Split('~');
            this.Height = this.Width;

            this.INIT(Req, URL);

        }
        public EditaceAvataraModel(string ServerRooth, Microsoft.AspNetCore.Http.HttpRequest Req, bool ZobrazeniCtverce, string width, string height, string modifikator = "")
        {
            this.Modifikator = modifikator;
            _ServerRooth = ServerRooth;
            UrlWEB = Req.Scheme + "://" + Req.Host;
            if (ZobrazeniCtverce)
            {
                this.typZobrazeni = "square";
                this.Width = width;
                this.Height = height;
            }
            else
            {

                this.typZobrazeni = "circle";
                this.Height = this.Width;
            }

            AvatarUmisteniIMG obrazek = new AvatarUmisteniIMG(_ServerRooth, Req, "Images/Foto_Avatar/1000.jpg");

            poleHodnotNastaveni[0] = obrazek.BasicIMG;

            poleHodnotNastaveni[6] = this.Width;
            poleHodnotNastaveni[7] = this.Height;

            //HiddenParametry.Value = "";


            foreach (string arr in poleHodnotNastaveni)
            {

                if (stringHodnotNastaveni != "") { stringHodnotNastaveni += "~"; }
                if (arr != "")
                {
                    stringHodnotNastaveni += arr;
                }
                else
                {
                    stringHodnotNastaveni += "0";
                }
            }
            // Contejner.Style.Add(HtmlTextWriterStyle.Width, this.Width.ToString() + "px");
            //Contejner.Style.Add(HtmlTextWriterStyle.Height, this.Height.ToString() + "px");

            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "avatarupdatescript", "$(document).ready(function () {EditaceAvatara('" + this.Contejner.ClientID + "', '" + this.HiddenParametry.ClientID + "'); });", true);

        }
        public EditaceAvataraModel(string ServerRooth, Microsoft.AspNetCore.Http.HttpRequest Req, string modifikator = "")
        {

            this.Modifikator = modifikator;
            UrlWEB = Req.Scheme + "://" + Req.Host;
            _ServerRooth = ServerRooth;

            AvatarUmisteniIMG obrazek = new AvatarUmisteniIMG(_ServerRooth, Req, "Images/Foto_Avatar/1000.jpg");

            poleHodnotNastaveni[0] = obrazek.BasicIMG;

            poleHodnotNastaveni[6] = this.Width;
            poleHodnotNastaveni[7] = this.Width;

            //HiddenParametry.Value = "";


            foreach (string arr in poleHodnotNastaveni)
            {

                if (stringHodnotNastaveni != "") { stringHodnotNastaveni += "~"; }
                if (arr != "")
                {
                    stringHodnotNastaveni += arr;
                }
                else
                {
                    stringHodnotNastaveni += "0";
                }
            }
            // Contejner.Style.Add(HtmlTextWriterStyle.Width, this.Width.ToString() + "px");
            //Contejner.Style.Add(HtmlTextWriterStyle.Height, this.Height.ToString() + "px");

            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "avatarupdatescript", "$(document).ready(function () {EditaceAvatara('" + this.Contejner.ClientID + "', '" + this.HiddenParametry.ClientID + "'); });", true);

        }



        public void OnGet()
        {
        }
        private void INIT(Microsoft.AspNetCore.Http.HttpRequest Req, string URL)
        {
            Random rnd = new Random();


            UrlWEB = Req.Scheme + "://" + Req.Host;


            if (poleHodnotNastaveni[0] == "")
            {
                poleHodnotNastaveni[0] = "Images/Foto_Avatar/1000.jpg";
            }   

            AvatarUmisteniIMG obrazek = new AvatarUmisteniIMG(_ServerRooth,Req,poleHodnotNastaveni[0]);

            ////  kontrola existence souboru na servru

            if (System.IO.File.Exists(obrazek.IMG_Server))
            {
                poleHodnotNastaveni[0] = obrazek.BasicIMG;
            }
            else
            {

                poleHodnotNastaveni[0] = "Images/Foto_Avatar/1000.jpg";
            }
            ////

            stringHodnotNastaveni = "";
            foreach (string arr in poleHodnotNastaveni)
            {

                if (stringHodnotNastaveni != "") { stringHodnotNastaveni += "~"; }
                if (arr != "")
                {
                    stringHodnotNastaveni += arr;
                }
                else
                {
                    stringHodnotNastaveni += "0";
                }
            }
        }


    }
}
