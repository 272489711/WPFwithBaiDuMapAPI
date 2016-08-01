using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace WpfApplication1
{
    public class ReadCities
    {
        private string json="";
        public ReadCities(string path)
        {
            using (StreamReader sr = new StreamReader(path,Encoding.Default))
            {
                string line="";
                while ((line = sr.ReadLine())!=null)
                {
                    json += line;
                }

            }

        }

        public JObject getJObj()
        {
            return JObject.Parse(json);
        }




    }
}
