using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Areas.FileManager.Models
{
    public class fileUpload
    {

        public string  CkEditorFile = "";

        public fileUpload(HttpContext httpContext)
        {
            string vystup = "";
            string file1 = "";
            if (httpContext.Request.Method == "POST" && httpContext.Request.Path == "/")
            {
                if (httpContext.Request.Form.Files.Count > 0)
                {

                    
                    try
                    {

                        for (int i = 0; i < httpContext.Request.Form.Files.Count; i++)
                        {
                            string koncovka = httpContext.Request.Form.Files[i].FileName.Split('.')[httpContext.Request.Form.Files[i].FileName.Split('.').Length - 1];
                            string file = httpContext.Request.Form.Files[i].FileName.Substring(0, httpContext.Request.Form.Files[i].FileName.Length - 1 - koncovka.Length);
                            koncovka = koncovka.ToLower();
                            file = file + '.' + koncovka;
                            file = file.Replace(' ', '_');
                            int index = 1;
                            //string localAdresar = ;
                            string root = System.IO.Path.Combine("wwwroot",httpContext.Request.Query["uploadRoot"].ToString(),file);

                            while (File.Exists(root))
                            {
                                file = httpContext.Request.Form.Files[i].FileName.Substring(0, httpContext.Request.Form.Files[i].FileName.Length - 1 - koncovka.Length);

                                file = file + " (" + index.ToString() + ")." + koncovka;

                                root = System.IO.Path.Combine("wwwroot", httpContext.Request.Query["uploadRoot"].ToString(), file);

                                index = index + 1;


                            }
                            { }

                            CkEditorFile = file;

                            if (!File.Exists(root))
                            {

                                

                                if (httpContext.Request.Form.Files[i].Length > 0)
                                {

                                    using (var stream = System.IO.File.Create(root))
                                    {
                                        httpContext.Request.Form.Files[i].CopyTo(stream);
                                    }
                                }
                            }




                        }
                    }
                    catch (Exception ex)
                    {
                        vystup += "<br/><span style=\"color:red;\">soubor :  \"" + file1 + "\" se nepodařilo nahrát { chyba : " + ex.Message + " }</span>";
                    }
                    finally
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes(vystup);
                        //httpContext.Response.Body.WriteAsync(data, 0, data.Length);
                    }
                }
            }

        }

    }
}
