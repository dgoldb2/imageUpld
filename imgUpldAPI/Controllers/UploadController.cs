using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace imgUpldAPI.Controllers
{
    [RoutePrefix("api/Upload")]
    public class UploadController : ApiController
    {
        [Route("user/PostUserImage")]
        //[BasicAuthentication]
        public async Task<HttpResponseMessage> PostUserImage()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
                    string newFileName = "";
                    string curFile = "";
                    //Change to final path on test and prod servers.
                    string path = @"C:\Users\davidtreering\source\Workspaces\ImageAPI\imgUpldAPI\imgUpldAPI\UserImage\";
                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 10; //Size = 10 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            var message = string.Format("Please Upload image of type .jpg,.png.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {
                            var message = string.Format("Please Upload a file upto 10 mb.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {
                            var filePath = HttpContext.Current.Server.MapPath("~/UserImage/" + postedFile.FileName);

                            //File is placed in the directory by the SaveAs function.
                            postedFile.SaveAs(filePath);
                            await Task.Delay(3000);
                            //Enter logic to handle scanned file
                            //File will be present, if scan succeeds. 
                            curFile = path + postedFile.FileName;
                            Debug.WriteLine(File.Exists(curFile) ? "File exists." : "File does not exist.");
                            if (File.Exists(curFile))
                            {
                                DateTime moment = DateTime.Now;
                                string day = "";
                                string month = "";
                                if (moment.Day < 10)
                                {
                                    day = moment.Day.ToString();
                                    day = "0" + day;
                                }
                                else
                                {
                                    day = moment.Day.ToString();
                                }

                                if (moment.Month < 10)
                                {
                                    month = moment.Month.ToString();
                                    month = "0" + month;
                                }
                                else
                                {
                                    month = moment.Month.ToString();
                                }
                                string year = moment.Year.ToString();
                                string yearMonthDay = year + month + day;
                                Debug.WriteLine(yearMonthDay);

                                Random rnd = new Random();
                                string rndnumber1 = rnd.Next(1000, 9999).ToString();
                                string rndnumber2 = rnd.Next(1000, 9999).ToString();
                                Debug.WriteLine(rndnumber1 + ", " + rndnumber2);

                                newFileName = yearMonthDay + "_" + rndnumber1 + "_" + rndnumber2 + extension;
                                Debug.WriteLine(newFileName);
                            }
                            else
                            //McAfee scan failed and file has not been quarantined.
                                {
                                var message = string.Format("Scan failed: file has been removed or is not found.");

                                dict.Add("error", message);
                                return Request.CreateResponse(HttpStatusCode.OK, dict);
                            }
                        }
                    }

                    var message1 = string.Format(newFileName);
                    //Rename uploaded image file
                    File.Move(curFile, path + newFileName);
                    
                    //Send http response with newFileName in header, Name: New-File-Name, value: newFileName.
                    return Request.CreateErrorResponse(HttpStatusCode.Created, message1); ;
                }
                var res = string.Format("Please upload an image.");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception ex)
            {
                var res = string.Format("some Message");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }
    }
}
