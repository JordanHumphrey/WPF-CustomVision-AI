using LandmarkAi.Classes;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LandmarkAi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png; *.jpg)|*.jpg;*.png;*.jpeg|All file(*.*)|*.*";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if(dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;
                selectedImage.Source = new BitmapImage(new Uri(fileName));

                MakePredictionAsync(fileName);
            }
        }

        /// <summary>
        /// Send Image file for prediction to Microsoft
        /// Custom Vision for Prediction Analysis.
        /// var file holds array of bytes from image data
        /// to send through http request to service.
        /// </summary>
        /// <param name="filename"></param>
        private async void MakePredictionAsync(string filename)
        {
            // URL for CustomVision AI Project, Prediction Key and Content type REQUIRED for processing
            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/e5c45d02-57f5-43d6-86d7-fc57e30e2f9a/image?iterationId=f0325650-cf29-48a3-8fe6-c4aa9cf82ef0";
            string prediction_Key = "a1ed9ffbf54c4a73962de335dd7d5bac";
            string content_Type = "application/octet-stream";
            var file = File.ReadAllBytes(filename);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Prediction-Key", prediction_Key);

                using (var content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(content_Type);

                    // Send elements to CustomVision and retrieve response
                    var response = await client.PostAsync(url, content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    List<Prediction> predictions = (JsonConvert.DeserializeObject<CustomVision>(responseString)).Predictions;
                    predictionsListView.ItemsSource = predictions;
                }
            }
        }
    }
}
