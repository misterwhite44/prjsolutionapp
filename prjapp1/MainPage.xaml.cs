using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Newtonsoft.Json.Linq;

namespace prjapp1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            string name = NameEntry.Text;
            int year = (int)YearPicker.SelectedItem;
            string result = await GetBirthDataAsync(name, year);
            ResultLabel.Text = result;
        }

        private async Task<string> GetBirthDataAsync(string name, int year)
        {
            string apiUrl = $"https://data.nantesmetropole.fr/api/records/1.0/search/?dataset=244400404_prenoms-enfants-nes-nantes&q={name}&refine.annee={year}";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(responseBody);
                    int count = json["nhits"].Value<int>();

                    if (count > 0)
                    {
                        var records = json["records"];
                        int total = 0;

                        foreach (var record in records)
                        {
                            total += record["fields"]["nombre"].Value<int>();
                        }

                        return $"Nombre d'enfants nés avec le prénom {name} en {year} à Nantes : {total}";
                    }
                    else
                    {
                        return $"Aucun enfant né avec le prénom {name} en {year} à Nantes.";
                    }
                }
                catch (Exception ex)
                {
                    return $"Erreur lors de la récupération des données : {ex.Message}";
                }
            }
        }
    }
}
