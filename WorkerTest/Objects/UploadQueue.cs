namespace WorkerTest.Objects
{
    public class UploadQueue
    {
        public UploadQueue()
        {
        }

        public async Task<bool> Upload(Stream stream, HttpClient client)
        {
            ByteArrayContent byteContent = null;
                //new ByteArrayContent(QueuedBytes.Dequeue());

            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                var memoryBytes = memoryStream.ToArray();
                byteContent = new ByteArrayContent(memoryBytes);
            }
            MultipartFormDataContent multipartContent = new MultipartFormDataContent();
            multipartContent.Add(byteContent);
            StringContent stringContent = new StringContent("blah blah",System.Text.Encoding.UTF8);
            multipartContent.Add(stringContent);
            HttpResponseMessage reponse = await client.PostAsync("http://localhost:7071/api/MokoWebHooks", multipartContent);
            var test = false;
            return test;
        }
    }
}
