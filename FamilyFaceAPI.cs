using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Family_Detection
{
    class recognizeResponse
    {

        public bool success { get; set; }
        public Face[] predictions { get; set; }

    }

    class registerResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string error { get; set; }
    }

    class deleteResponse
    {
        public bool success { get; set; }
    }

    class listResponse
    {
        public bool success { get; set; }
        public string[] faces { get; set; }
    }

    class Face
    {

        public string userid { get; set; }
        public float confidence { get; set; }
        public int y_min { get; set; }
        public int x_min { get; set; }
        public int y_max { get; set; }
        public int x_max { get; set; }

    }

    class FamilyFaceAPI
    {
        private HttpClient client;
        private string server;
        

        public FamilyFaceAPI(string server)
        {
            client = new HttpClient();
            this.server = server;
        }

        public async Task<registerResponse> registerFace(string family, string name, string[] images_path)
        {

            var request = new MultipartFormDataContent();
            for(int i = 0; i < images_path.Length; i++)
            {
                var image_data = File.OpenRead(images_path[i]);
                request.Add(new StreamContent(image_data), "image"+(i+1), Path.GetFileName(images_path[i]));
            }
            request.Add(new StringContent(family+"_"+name), "userid");
            var output = await client.PostAsync(server + "/v1/vision/face/register", request);
            var jsonString = await output.Content.ReadAsStringAsync();
            registerResponse response = JsonConvert.DeserializeObject<registerResponse>(jsonString);

            return response;
        }

        public async Task<recognizeResponse> recognizeFace(string image_path)
        {

            var request = new MultipartFormDataContent();
            var image_data = File.OpenRead(image_path);
            request.Add(new StreamContent(image_data), "image", Path.GetFileName(image_path));
            var output = await client.PostAsync(server + "/v1/vision/face/recognize", request);
            var jsonString = await output.Content.ReadAsStringAsync();
            recognizeResponse response = JsonConvert.DeserializeObject<recognizeResponse>(jsonString);

            return response;

        }

        public listResponse listFaces()
        {
            
            var output =  client.PostAsync(server + "/v1/vision/face/list", null).Result;
            var jsonString = output.Content.ReadAsStringAsync().Result;

            listResponse response = JsonConvert.DeserializeObject<listResponse>(jsonString);

            return response;

        }

        public async Task<deleteResponse> deleteFace(string userid)
        {

            var request = new MultipartFormDataContent();
            request.Add(new StringContent(userid), "userid");
            var output = await client.PostAsync(server + "/v1/vision/face/delete", request);
            var jsonString = await output.Content.ReadAsStringAsync();

            deleteResponse response = JsonConvert.DeserializeObject<deleteResponse>(jsonString);

            return response;

        }

    }
}
