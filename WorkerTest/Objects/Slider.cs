using MokoPages;
using MokoPages.Data.CloudflareImages;
using Newtonsoft.Json;
using System.Text;
namespace WorkerTest.Objects
{
    public class Slider
    {
        public string Name { get; set; } = "Default";
        public List<string> Links {  get
            {
                if (Pages == null)
                {
                    return null;
                }
                return Pages[PagePosition].Images.Select(x => x.Variants.First()).ToList();
            }
        }
        public CloudflareImagePage[] Pages { get; set; }
        public int PagePosition { get; set; } = 0;

        public Slider(){}

        public Slider(CloudflareImagePage[] resultsPages)
        {
            Pages = resultsPages;
        }

        public static Slider DecodeSlider(string base64Str)
        {
            byte[] data = Convert.FromBase64String(base64Str);
            string decodedString = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<Slider>(decodedString);
        }        
    }
}
