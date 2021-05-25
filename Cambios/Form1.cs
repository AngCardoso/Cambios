using Cambios.Modelos;
using Cambios.Servicos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cambios
{
    public partial class Form1 : Form
    {
        #region atributos
        private NetworkService networkService;

        private ApiService apiService;

        private List<Rate> Rates;

        private DialogService DialogService;

        private DataService DataService;

        #endregion


        public Form1()
        {
            InitializeComponent();
            networkService = new NetworkService();
            apiService = new ApiService();
            LoadRates();
            DialogService = new DialogService();
            DataService = new DataService();
        }

        private async void LoadRates()
        {
            bool load;

            labelresultado.Text = "A Atualizar taxas";

            var connection = networkService.CheckConnection();

            if (!connection.IsSucess)
            {
                LoadLocalRates();
                load = false;
            }
            else
            {
                await LoadApiRates();
                load = true;
            }

            if(Rates.Count == 0)
            {
                labelresultado.Text = "não há ligação à internet" + Environment.NewLine +
                                        "e não foram carregadas taxas préviamente" + Environment.NewLine +
                                        "Tente mais tarde";
                labelstatus.Text = "Primeira ligação requer ligação à internet";

                return;
            }

            comboBoxOrigem.DataSource = Rates;
            comboBoxOrigem.DisplayMember = "Name";

            comboBoxDestino.BindingContext = new BindingContext();

            comboBoxDestino.DataSource = Rates;
            comboBoxDestino.DisplayMember = "Name";

            buttonConverter.Enabled = true;
            buttonTroca.Enabled = true;

            labelresultado.Text = "taxas atualizadas";

            if (load)
            {
                labelstatus.Text = string.Format("Taxas carregadas da inteernet em {0:F}", DateTime.Now);
            }
            else
            {
                labelstatus.Text = string.Format("Taxas carregadas da base de Dados");
            }

            progressBar1.Value = 100;
            
        }

        private void LoadLocalRates()
        {
           Rates = DataService.GetData();
        }

        private async Task LoadApiRates()
        {
            progressBar1.Value = 0;

            var response = await apiService.GetRates("http://cambios.somee.com", "/api/rates");

            Rates = (List<Rate>) response.Result;

            DataService.DeleteData();

            DataService.Savedata(Rates);
        }

        private void buttonConverter_Click(object sender, EventArgs e)
        {
            Converter();
        }

        private void Converter()
        {
            if (string.IsNullOrEmpty(TextBoxValor.Text))
            {
                DialogService.ShowMessage("Erro", "insira um valor a converter");
            }

            decimal valor;
            if (!decimal.TryParse(TextBoxValor.Text, out valor)) 
            {
                DialogService.ShowMessage("Erro de conversão", "insira um valor numérico"); 
            }

            if (comboBoxOrigem.SelectedItem == null)
            {
                DialogService.ShowMessage("Erro", "escolha uma moeda a converter");
                return;
            }

            if (comboBoxDestino.SelectedItem == null)
            {
                DialogService.ShowMessage("Erro", "escolha uma moeda de destino");
                return;
            }

            var taxaOrigem =(Rate)comboBoxOrigem.SelectedItem;
            var taxaDestino = (Rate)comboBoxDestino.SelectedItem;
            var valorConvertido = valor / (decimal)taxaOrigem.TaxRate * (decimal)taxaDestino.TaxRate;

            labelresultado.Text = string.Format("{0} {1:C2} = {2} {3:C2}", taxaOrigem.Code, valor, taxaDestino.Code, valorConvertido);
        }

        private void buttonTroca_Click(object sender, EventArgs e)
        {
          Troca();
        }

        private void Troca()
        {

            
            var aux = comboBoxOrigem.SelectedItem;
            comboBoxOrigem.SelectedItem = comboBoxDestino.SelectedItem;
            comboBoxDestino.SelectedItem = aux;
            
            Converter();
        }
    }
}
 