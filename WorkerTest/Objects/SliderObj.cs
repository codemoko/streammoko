using MokoPages;
using MokoPages.Data.CloudflareImages;
using Newtonsoft.Json;
using System.Text;
namespace WorkerTest.Objects
{
    public class SliderObj
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

        public SliderObj(){}

        public SliderObj(CloudflareImagePage[] resultsPages)
        {
            Pages = resultsPages;
        }

        public static SliderObj DecodeSlider(string base64Str)
        {
            byte[] data = Convert.FromBase64String(base64Str);
            string decodedString = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<SliderObj>(decodedString);
        }        
    }
}
