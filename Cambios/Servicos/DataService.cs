using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using Cambios.Modelos;

namespace Cambios.Servicos
{
    class DataService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService;


        public DataService()
        {
            dialogService = new DialogService();
            if (!Directory.Exists("data"))
            {
                Directory.CreateDirectory("data");
            }

            var path = @"Data\Rates.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source =" + path);
                connection.Open();

                string sqlcommand = "create table if not exists rates(RateId int, Code varchar(5), TaxRate real, Name varchar(250))";
                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();

            }

            catch (Exception e)
            {
                dialogService.ShowMessage("erro", e.Message);
            }
        }

        public void Savedata(List<Rate> Rates)
        {
            try
            {
                foreach (var rate in Rates)
                {
                    string sql = string.Format("insert into Rates (RateId, Code, TaxRate, Name) values({0}, '{1}', {2}, '{3}')",
                        rate.RateId, rate.Code, rate.TaxRate, rate.Name);

                    command = new SQLiteCommand(sql, connection);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("erro", e.Message);
            }
        }

        public List<Rate> GetData()
        {
            List<Rate> rates = new List<Rate>();
                try 
                {
                    string sql = "select RateId, Code, TaxRate, Name from rates";

                    command = new SQLiteCommand(sql, connection);

                    SQLiteDataReader reader = command.ExecuteReader();
                    
                        while(reader.Read())
                        {
                            rates.Add(new Rate
                            {
                                RateId = (int)reader["RateID"],
                                Code = (string)reader["Code"],
                                Name = (string)reader["Taxrate"],
                                TaxRate = (double)reader["TaxRate"]
                            });
                        }

                connection.Close();

                return rates;
                }
                catch (Exception e)
                {
                    dialogService.ShowMessage("erro", e.Message);
                    return null;
                }
        }

        public void DeleteData()
        { 
            try 
            {
                string sql = "delete from Rates";

                command = new SQLiteCommand(sql, connection);

                command.ExecuteNonQuery();
            }

            catch(Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }
    }
}
