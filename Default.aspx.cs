using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var encodedResponse = Request.Form["g-recaptcha-response"];
            var isCaptchaValid = ReCaptcha.Validate(encodedResponse); 
            lblResult.Visible = true;
            if (Page.IsValid && isCaptchaValid) {

                lblResult.Text = "You Got It!";
                lblResult.ForeColor = System.Drawing.Color.Green;
            }
          else
            {
                lblResult.Text = "Incorrect";
                lblResult.ForeColor = System.Drawing.Color.Red;
            }
           
        }
      
        protected bool IsValidCaptcha()
        {
            string resp = Request["g-recaptcha-response"];
            var req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LcNJdwZAAAAAA-kHnYAHR09xBv5ouVhlDDvo_bW" + "&response=" + resp);
            using (WebResponse wResponse = req.GetResponse())
            {
                using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                {
                    string jsonResponse = readStream.ReadToEnd();
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    // Deserialize Json
                    CaptchaResult data = js.Deserialize<CaptchaResult>(jsonResponse);
                    if (Convert.ToBoolean(data.success))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
    public class CaptchaResult
    {
        public string success { get; set; }
    }
    public class ReCaptcha
    {
        public bool Success { get; set; }
        public List<string> ErrorCodes { get; set; }

        public static bool Validate(string encodedResponse)
        {
            if (string.IsNullOrEmpty(encodedResponse)) return false;

            var client = new System.Net.WebClient();
            var secret = "6LcNJdwZAAAAAA-kHnYAHR09xBv5ouVhlDDvo_bW";// ConfigurationManager.AppSettings["Google.ReCaptcha.Secret"];

            if (string.IsNullOrEmpty(secret)) return false;

            var googleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, encodedResponse));

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            var reCaptcha = serializer.Deserialize<ReCaptcha>(googleReply);

            return reCaptcha.Success;
        }
    }
}