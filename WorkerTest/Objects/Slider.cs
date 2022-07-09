﻿using Newtonsoft.Json;
using System.Text;
namespace WorkerTest.Objects
{
    public class Slider
    {
        public string Base64 { get; set; }
        public string Name { get; set; } = "Default";
        public List<string> Links { get; set; } = new List<string>();

        public Slider(){}

        public static Slider DecodeSlider(string base64Str)
        {
            byte[] data = Convert.FromBase64String(base64Str);
            string decodedString = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<Slider>(decodedString);
        }        
    }
}