using System.IO;

namespace FileManager.Models
{

    public class ItemFM
    {

        public string FullName { get; private set; } = "";
        public string NameZobrazeni { get; private set; } = "";
        public string URL_ikony { get; private set; } = "";
        public string DatumVytvoreni { get; private set; } = "1.1.1900";
        public string Velikost { get; private set; } = "0Kb";
        public string OnClick { get; private set; } = "";
        public string CSS_context_menu { get; private set; } = "";
        public string DirectoryName { get; private set; } = "";
        public bool IsDirectory { get; private set; } = false;
        public string FileName { get; private set; } = "";
        public bool IsFile { get; private set; } = false;
        public string Koncovka { get; private set; } = "";
        public string Server_Path { get; private set; } = "";
        public string URL_Path { get; private set; } = "";
        public string User_Path { get; private set; } = "";
        public bool IsImage { get { if ((Koncovka == "png" || Koncovka == "jpg" || Koncovka == "jpeg" || Koncovka == "svg" || Koncovka == "bmp") && IsFile) { return true; } return false; } }

        /// <summary>
        /// file
        /// </summary>
        public ItemFM(string path, string urlPath, string serverPath)
        {
            //Server_Path = serverPath;
            FileName = Path.GetFileName(serverPath + path);
            FullName = Path.GetFileName(serverPath + path);
            Koncovka = FileName.Split('.')[FileName.Split('.').Length - 1];
            URL_Path = urlPath;
            User_Path = path;
            IsFile = true;

            string fileSelektor = Koncovka;
            if (IsImage) { fileSelektor = "IMG"; }

            CSS_context_menu = "context-menu-one"+" "+fileSelektor;
            //OnClick = "alert('ccc');";



            if (File.Exists(serverPath + "lib\\FileManager\\Icony\\" + Koncovka + ".png"))
            {
                URL_ikony = urlPath + "lib/FileManager/Icony/" + Koncovka + ".png";
            }

            FileInfo fInfo = new FileInfo(serverPath + path);
            long big = fInfo.Length / 1024;
            Velikost = big.ToString() + " Kb";
            DatumVytvoreni = File.GetCreationTime(serverPath + path).ToShortDateString();

            if (FileName.Length > 18) { NameZobrazeni = FileName.Substring(0, 10) + " ... " + Koncovka; } else { NameZobrazeni = FileName; }

        }

        /// <summary>
        /// directory
        /// </summary>
        /// 
        public ItemFM(string path, string urlPath, string serverPath, bool directory)
        {
            Server_Path = serverPath + path.Replace("/", "\\\\");
            DirectoryName = Path.GetFileName(path);
            FullName = Path.GetFileName(path);
            URL_Path = urlPath + "/" + path;
            User_Path = path;
            CSS_context_menu = "context-menu-two";
            IsDirectory = true;
            URL_ikony = "\"" + urlPath + "/lib/FileManager/Icony/folder.png\"";

            Velikost = "";
            DatumVytvoreni = "";



            if (DirectoryName.Length > 23) { NameZobrazeni = DirectoryName.Substring(0, 10) + " ... "; } else { NameZobrazeni = DirectoryName; }

        }

        /// <summary>
        /// item Back
        /// </summary>
        public ItemFM(string urlPath, bool back)
        {
            Server_Path = "";
            DirectoryName = "";
            FullName = "";
            URL_Path = "";
            User_Path = "";
            CSS_context_menu = "context-menu-three";
            IsDirectory = false;
            URL_ikony = "\"" + urlPath + "/lib/FileManager/Icony/back.png\"";

            Velikost = "";
            DatumVytvoreni = "";


        }
    }
}


