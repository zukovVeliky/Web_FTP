using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Avatar
{
    public class AvatarUmisteniIMG
    {
        string UrlWEB { get; set; }
        string _ServerRooth { get; set; }

        public string BasicIMG { get; private set; }
        public string IMG_Web { get; private set; }
        public string IMG_Server { get; private set; }
        public AvatarUmisteniIMG(string ServerRooth,Microsoft.AspNetCore.Http.HttpRequest Req,string IMG)
        {
            _ServerRooth = ServerRooth;
            UrlWEB = Req.Scheme + "://" + Req.Host;

            BasicIMG = NormalizeBasicImg(IMG);
            if (string.IsNullOrWhiteSpace(BasicIMG))
            {
                BasicIMG = "Images/Foto_Avatar/1000.jpg";
            }


            KontrolaUmisteniSouboru();


            IMG_Web = UrlWEB + "/" + BasicIMG;
        }

        private string NormalizeBasicImg(string img)
        {
            if (string.IsNullOrWhiteSpace(img))
            {
                return "";
            }

            var normalized = img.Trim().Replace("\\", "/");

            if (Uri.TryCreate(normalized, UriKind.Absolute, out var uri))
            {
                normalized = uri.AbsolutePath;
            }
            else if (!string.IsNullOrEmpty(UrlWEB) &&
                     normalized.StartsWith(UrlWEB, StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(UrlWEB.Length);
            }

            if (!string.IsNullOrEmpty(_ServerRooth))
            {
                var serverRootNormalized = _ServerRooth.Replace("\\", "/").TrimEnd('/');
                if (normalized.StartsWith(serverRootNormalized, StringComparison.OrdinalIgnoreCase))
                {
                    normalized = normalized.Substring(serverRootNormalized.Length);
                }
            }

            return normalized.TrimStart('/');
        }


        private void KontrolaUmisteniSouboru()
        {






            var parts = BasicIMG.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && parts[0] == "Images" && parts[1] == "Foto_Avatar")
            {
                // existuje

                IMG_Server = Path.Combine(_ServerRooth, BasicIMG.Replace("/", "\\"));
            }
            else
            {
                // neexistuje

                string Aktualni_IMG_adresar = Path.Combine("Images", "Foto_Avatar", DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString());

                if (!Directory.Exists(Path.Combine(_ServerRooth, Aktualni_IMG_adresar)))
                {
                    Directory.CreateDirectory(Path.Combine(_ServerRooth, Aktualni_IMG_adresar));
                }

                // vytvoření nového jedinečného jména souboru
                string new_File = Guid.NewGuid() + ".png";

                // uprava rozměrů a uložení náhledu obrázku


                string old_IMG_Server = Path.Combine(_ServerRooth, BasicIMG.Replace("/", "\\"));
                if (!File.Exists(old_IMG_Server))
                {
                    BasicIMG = "Images/Foto_Avatar/1000.jpg";
                    IMG_Server = Path.Combine(_ServerRooth, BasicIMG.Replace("/", "\\"));
                    return;
                }

                IMG_Server = Path.Combine(Path.Combine(_ServerRooth, Aktualni_IMG_adresar), new_File);
                File.Copy(old_IMG_Server, IMG_Server);
                BasicIMG = IMG_Server.Replace(UrlWEB, "").Replace(_ServerRooth, "").Replace("\\", "/");
                if (BasicIMG[0] == '/') {BasicIMG = BasicIMG.Remove(0, 1); }
            }

        }
    }
}
