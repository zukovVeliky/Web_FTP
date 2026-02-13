using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;


namespace FileManager.Pages
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class FileManagerModel : PageModel
    {

        //public IHostingEnvironment _env;
        public IWebHostEnvironment _env;
        public string LocalDirectory { get; set; }
        

        private string[] ImgType = new string[] { "jpg", "jpeg", "png","tif","svg" };
        private bool OnlyIMG = false;

        public string VievRoot { get { return LocalRoot; } }
        //public string Settings;


        private bool Back { get; set; } = false;
        public string LocalRoot { get; set; } = "";
        public string URLDirectory { get; set; }

        public string UploadRoot { get { return Path.Combine(BasicDirectory,LocalRoot); } }

        /// <summary>
        /// nastaveni zakladniho adresare 
        /// </summary>
        public string BasicDirectory { get; set; } = "UzivatelskeSoubory"; 
        /// <summary>
        /// nastaveni javascriptov0 funkce na rodi4ovske strance kter8 je volana pri vraceni url Z managera
        /// </summary>
        public string PageFunction { get; set; } = "ZachyceniURL";
        public List<string> Directories { get; private set; }
        public List<string> Files { get; private set; }


        public string VybranyAdresar { get; set; } = "";
        public string Cesta { get; private set; } = "";

        public byte[] NahledIMG = null;



        private readonly ILogger<FileManagerModel> _logger;

        
        public FileManagerModel(IWebHostEnvironment env, ILogger<FileManagerModel> logger) { _env = env; _logger = logger; }


        public void OnGet(
            string SD, string LD,string SET, string back, string Copy,string Presun, string RenameF, string RenameD,
            string Delete,string CreateD,string ZIP,string UNZIP, string onlyIMG,string Nahled,string p,string f)
        {
            if (f != null && f != "")
            {
                
                var cookieOptions = new CookieOptions
                {
                    // Set the secure flag, which Chrome's changes will require for SameSite none.
                    // Note this will also require you to be running on HTTPS
                    Secure = true,

                    // Set the cookie to HTTP only which is good practice unless you really do need
                    // to access it client side in scripts.
                    HttpOnly = true,

                    // Add the SameSite attribute, this will emit the attribute with a value of none.
                    // To not emit the attribute at all set the SameSite property to SameSiteMode.Unspecified.
                    SameSite = SameSiteMode.None
                };
                Response.Cookies.Delete("f");
                Response.Cookies.Append("f", f, cookieOptions);
                this.PageFunction = f;

            }
            else
            {
                if (Request.Cookies["f"] != null)
                {
                    this.PageFunction = Request.Cookies["f"];
                }
            }


            if (p != null && p != "")
            {
                var cookieOptions = new CookieOptions
                {
                    // Set the secure flag, which Chrome's changes will require for SameSite none.
                    // Note this will also require you to be running on HTTPS
                    Secure = true,

                    // Set the cookie to HTTP only which is good practice unless you really do need
                    // to access it client side in scripts.
                    HttpOnly = true,

                    // Add the SameSite attribute, this will emit the attribute with a value of none.
                    // To not emit the attribute at all set the SameSite property to SameSiteMode.Unspecified.
                    SameSite = SameSiteMode.None
                };
                Response.Cookies.Delete("p");
                Response.Cookies.Append("p", p, cookieOptions);
                BasicDirectory = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(p));

            }
            else
            {
                if (Request.Cookies["p"] != null)
                {
                    BasicDirectory = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(Request.Cookies["p"]));
                }
            }



            if(SET != null && SET != "") { Nastaveni(SET); }


            //var vystup = System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Unicode.GetBytes("\u0010d"));
            if (onlyIMG != null && onlyIMG != "") { OnlyIMG = true; }
            if (SD != null && SD != "") { VybranyAdresar = SD; }
            if (LD != null && LD != "") { 
                
                
                LocalRoot = LD;


                string[] ld = LocalRoot.Split("\\");
                string[] bd = BasicDirectory.Split("\\");
                for (int i = 0; i < ld.Length && i < bd.Length; i++)
                {
                    if (ld[i] == bd[i]) { ld[i] = ""; }
                    else
                    {
                        break;
                    }
                }

                LocalRoot = Path.Combine(ld);


            }
            if (Nahled != null && Nahled != "")
            {
               //Models.ThumnailImage test = new Models.ThumnailImage(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, Nahled));
               //this.NahledIMG = test.ImageNahled;
                
                using (var ms = new MemoryStream())
                {
                    Bitmap imageIn = (Bitmap)ImageVelikost.resizeImage(
                        Bitmap.FromFile(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, Nahled)),
                        new Size(300,300));

                    imageIn.Save(ms, ImageFormat.Jpeg);
                    this.NahledIMG = ms.ToArray();
                }
                
            }
            if (back == "true" && LocalRoot != "")
            {
                Back = true;
            }
            if (Copy != null && Copy != "")
            {
                if (Presun == "Copy") { Kopirovani(Copy,false); }
                else {Kopirovani(Copy); }
            }
            if (RenameF != null && RenameF != "") { PrejmenovaniFile(RenameF); }
            if (RenameD != null && RenameD != "") { PrejmenovaniDirectory(RenameD); }
            if (Delete != null && Delete != "") { DeleteFD(Delete); }
            if (CreateD != null && CreateD != "") { CreateDirectory(CreateD); }
            if (ZIP != null && ZIP != "") { CreateZIP(ZIP); }
            if (UNZIP != null && UNZIP != "") { CreateUNZIP(UNZIP); }


            ReadSlozka();

            string testxxx = Request.Path.Value;
        }


        private void ReadSlozka()
        {
            //// uprava 17.1.2021

            if (Back)
            {
                string[] dire = LocalRoot.Split("\\").SkipLast(1).ToArray();
                LocalRoot = Path.Combine(dire);
                LocalDirectory = Path.Combine(BasicDirectory, LocalRoot);
            }
            else
            {
                LocalDirectory = Path.Combine(BasicDirectory, LocalRoot, VybranyAdresar);
                LocalRoot = Path.Combine(LocalRoot, VybranyAdresar);
            }




            URLDirectory = Request.Scheme + "://" + Request.Host.Value;

            Cesta = Path.Combine(_env.WebRootPath, LocalDirectory);



            Directories = Directory.GetDirectories(Cesta).ToList();

            Files = Directory.GetFiles(Cesta).ToList();

            
            if (OnlyIMG)
            {
                for (int i=0; Files.Count > i; i++)
                {
                    int lastItem = Files[i].Split('.').Length - 1;
                    bool shoda = false;
                    foreach (string arg in ImgType )
                    {
                        if (arg == Files[i].Split('.')[lastItem])
                        {
                            shoda = true;
                        }
                    }
                    if (!shoda) { Files.RemoveAt(i);i = 0; }

                }
            }


        }

 
        private void Kopirovani(string data) 
        { Kopirovani(data, true); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="delete">true odstrani zdrojove soubory a slozky</param>
        private void Kopirovani(string data,bool delete)
        {

            List<string> kolekceSouboru = data.Split('|')[1].Split('%')[0].Split('~').OfType<string>().ToList();
            List<string> kolekceAdresaru = data.Split('|')[1].Split('%')[1].Split('~').OfType<string>().ToList();

            /// kopirovani o uroven vyse
            if (data.Split('|')[0] == "")
            {
                string LocalRootBack = "";
                string[] dire = LocalRoot.Split("\\").SkipLast(1).ToArray();
                LocalRootBack = Path.Combine(dire);

                foreach (string adr in kolekceAdresaru)
                {
                    if (adr != "")
                    {
                        KopirovaniAdresaru(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, adr), Path.Combine(_env.WebRootPath, BasicDirectory, LocalRootBack));
                        if (delete) { Directory.Delete(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, adr), true); }
                    }
                }

                foreach (string soubor in kolekceSouboru)
                {
                    if (soubor != "")
                    {

                        System.IO.File.Copy(
                            Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, soubor),
                            this.KontrolaJmenaSoboru(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRootBack, soubor))
                            );
                        if (delete) { System.IO.File.Delete(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, soubor)); }
                    }

                }
            }
            else
            {



                ///kopirovami adresaru
                foreach (string adr in kolekceAdresaru)
                {
                    if (adr != data.Split('|')[0] && adr != "")
                    {
                        //string test = Path.Combine(Server.MapPath("~/"), UserPath.Replace("/","\\"), adr);
                        KopirovaniAdresaru(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, adr), Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, data.Split('|')[0]));
                        if (delete) { Directory.Delete(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, adr), true); }
                        /*
                         dodelat kopirovani o uroven vyse a kopirovani souboru v hlavnim vyberu
                     
                     */
                    }

                }
                ///kopirovani souboru
                foreach (string soubor in kolekceSouboru)
                {
                    if (soubor != "")
                    {

                        System.IO.File.Copy(
                            Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, soubor),
                            this.KontrolaJmenaSoboru(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, data.Split('|')[0], soubor))
                            );
                        if (delete) { System.IO.File.Delete(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, soubor)); }
                    }

                }
            }




        }
        private void KopirovaniAdresaru(string zdrojovyAdresar, string cilovyAdresar)
        {
            if (!Directory.Exists(Path.Combine(cilovyAdresar, Path.GetFileName(zdrojovyAdresar))))
            {
                Directory.CreateDirectory(Path.Combine(cilovyAdresar, Path.GetFileName(zdrojovyAdresar)));
            }
            foreach (string soubor in Directory.GetFiles(zdrojovyAdresar))
            {
                System.IO.File.Copy(soubor, this.KontrolaJmenaSoboru(Path.Combine(cilovyAdresar, Path.GetFileName(zdrojovyAdresar), Path.GetFileName(soubor))));
            }


            foreach (string subDirectory in Directory.GetDirectories(zdrojovyAdresar))
            {
                KopirovaniAdresaru(subDirectory, Path.Combine(cilovyAdresar, Path.GetFileName(zdrojovyAdresar)));
            }

        }


        private void PrejmenovaniDirectory (string rename)
        {
            
            if (System.IO.Directory.Exists(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, rename.Split('|')[0])))
            {
                System.IO.Directory.Move(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, rename.Split('|')[0]), KontrolaJmenaAdresare(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, rename.Split('|')[1])));
            }
        }


        private void PrejmenovaniFile (string rename)
        {
            if (System.IO.File.Exists(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, rename.Split('|')[0])))
            {
                System.IO.File.Move(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, rename.Split('|')[0]), KontrolaJmenaSoboru(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, rename.Split('|')[1])));
            }
            
        }

        private void DeleteFD (string Delete)
        {
            string[] deleteSoubory = Delete.Split('|')[0].Split('%');
            string[] deleteDirectory = Delete.Split('|')[1].Split('%');
            foreach(string arg in deleteSoubory)
            {
                if (arg != "")
                {
                    if (System.IO.File.Exists(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, arg)))
                    {
                        System.IO.File.Delete(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, arg));
                    }
                }
            }
            foreach(string arg in deleteDirectory)
            {
                if (arg != "")
                {
                    if (System.IO.Directory.Exists(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, arg))
                        && Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, arg) != Path.Combine(_env.WebRootPath, BasicDirectory)
                        )
                    {
                        System.IO.Directory.Delete(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, arg), true);
                    }
                }
            }
            
        }
        private void CreateUNZIP(string unzip)
        {

            string nameAdresare = Path.GetFileName(unzip).Split('.')[0];
            System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot,unzip),KontrolaJmenaAdresare( Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot,nameAdresare)));
        }
        private void CreateZIP(string zip)
        {

            string[] Soubory = zip.Split('|')[0].Split('%');
            string[] Directory = zip.Split('|')[1].Split('%');

            string vystupSoubory = "";
            string vystupDirectory = "";
            foreach(string arg in Soubory) { if (vystupSoubory == "" ) { vystupSoubory += arg; } else { vystupSoubory += "~" + arg; } }

            foreach (string arg in Directory) { if (vystupDirectory == "") { vystupDirectory += arg; } else { vystupDirectory += "~" + arg; } }


            string zipPath = KontrolaJmenaAdresare( Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, "Zip_" + DateTime.Now.Date.ToShortDateString().Replace('.','_')));
            CreateDirectory(Path.GetFileName(zipPath));

            Kopirovani(Path.GetFileName(zipPath) + "|" + vystupSoubory + "%" + vystupDirectory, false);


            System.IO.Compression.ZipFile.CreateFromDirectory(zipPath, KontrolaJmenaSoboru(zipPath + ".zip"));
            System.IO.Directory.Delete(zipPath, true);


        }

        private void CreateDirectory (string nameNewDirectory)
        {
            System.IO.Directory.CreateDirectory(KontrolaJmenaAdresare(Path.Combine(_env.WebRootPath, BasicDirectory, LocalRoot, nameNewDirectory)));
        }


        /// <summary>
        /// metoda kontroluje zdav v dane lokalite ji6 existuje stejn0 jmeno souboru, pokud ano prida ke jmenu ciselny index
        /// </summary>
        /// <param name="soubor"></param>
        /// <returns></returns>
        private string KontrolaJmenaSoboru(string soubor)
        {
            int index = 0;
            string jmenoSouboru = Path.GetFileName(soubor);
            while (System.IO.File.Exists(soubor.Replace(Path.GetFileName(soubor), jmenoSouboru)))
            {
                string koncovka = Path.GetFileName(soubor).Split('.')[Path.GetFileName(soubor).Split('.').Length - 1]; //   files[i].FileName.Split('.')[files[i].FileName.Split('.').Length - 1];
                string jmenoBezKoncovky = Path.GetFileName(soubor).Substring(0, Path.GetFileName(soubor).Length - 1 - koncovka.Length);

                jmenoSouboru = jmenoBezKoncovky + " (" + index.ToString() + ")." + koncovka;
                jmenoSouboru = jmenoSouboru.Replace(' ', '_');

                index = index + 1;

            }

            return soubor.Replace(Path.GetFileName(soubor), jmenoSouboru);
        }

        /// <summary>
        /// metoda kontroluje zdav v dane lokalite ji6 existuje stejn0 jmeno souboru, pokud ano prida ke jmenu ciselny index
        /// </summary>
        /// <param name="Adresar"></param>
        /// <returns></returns>
        private string KontrolaJmenaAdresare(string adresar)
        {
            int index = 0;
            string jmenoAdresare = Path.GetFileName(adresar);
            while (System.IO.Directory.Exists(adresar.Replace(Path.GetFileName(adresar), jmenoAdresare)))
            {
   
                string jmeno = Path.GetFileName(adresar);

                jmenoAdresare = jmeno + "_(" + index.ToString() + ").";
                jmenoAdresare = jmenoAdresare.Replace(' ', '_');

                index = index + 1;

            }

            return adresar.Replace(Path.GetFileName(adresar), jmenoAdresare);
        }

        private void Nastaveni (string set)
        {
            foreach (string arg in set.Split('&'))
            {
                if (arg == "onlyIMG") { OnlyIMG = true; }
            }
        }
        public string SestavSestaveni ()
        {
            string set = "&";
            if (OnlyIMG) { set += "onlyIMG&"; }
            return set;
        }


    }
}
