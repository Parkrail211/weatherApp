using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;


namespace weatherApp
{
    public partial class MainScreen : UserControl
    {

        Image rain = Properties.Resources.clouds;
        Image clear = Properties.Resources.clearSky;
        Image snow = Properties.Resources.snowy;
        public MainScreen()
        {
            InitializeComponent();
        }

        private void MainScreen_Load(object sender, EventArgs e)
        {
            LabelFill();
        }

        private decimal Rounder(string number)
        {
            decimal outputNumber = decimal.Round(Convert.ToDecimal(number), 0);

            return outputNumber;
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
           
                Form1.days.Clear();
           
            ExtractForecast(cityBox.Text);
            ExtractCurrent(cityBox.Text);
            LabelFill();

        }

        private void LabelFill()
        {
            // changes background based of current weather
            if (Convert.ToInt32(Form1.days[0].outdoorCondition) >= 200
                && Convert.ToInt32(Form1.days[0].outdoorCondition) < 600)
            {
                this.BackgroundImage = rain;
            }
            else if (Convert.ToInt32(Form1.days[0].outdoorCondition) >= 600
                 && Convert.ToInt32(Form1.days[0].outdoorCondition) < 623)
            {
                this.BackgroundImage = snow;
            }
            else
            {
                this.BackgroundImage = clear;
            }

            // displayes current days info
            cWeatherOutput.Text = $"Current Temp: {Rounder(Form1.days[0].currentTemp)}°C";
            maxOutput.Text = $"High: {Rounder(Form1.days[0].tempHigh)}°C";
            minOutput.Text = $"Low: {Rounder(Form1.days[0].tempLow)}°C";
            locationOutput.Text = Form1.days[0].location;
            todayDateOutput.Text = Form1.days[0].date;

            // cleares forecast
            foreDateOutput.Text = "";
            foreMaxOutput.Text = "";
            foreMinOutput.Text = "";
            WeatherIconOutput.Text = "";

            // adds data to screen
            for (int i = 1; i < Form1.days.Count() - 1; i++)
            {
                foreDateOutput.Text += Form1.days[i].date;
                foreDateOutput.Text += "\n \n";

                foreMaxOutput.Text += $"{Rounder(Form1.days[i].tempHigh)}°C";
                foreMaxOutput.Text += "\n \n";

                foreMinOutput.Text += $"{Rounder(Form1.days[i].tempLow)}°C";
                foreMinOutput.Text += "\n \n";

                // displayes weather of day as a symbol
                if (Convert.ToInt32(Form1.days[i].outdoorCondition) >= 200
                 && Convert.ToInt32(Form1.days[i].outdoorCondition) < 600)
                {
                    WeatherIconOutput.Text += $"🌧";
                }
                else if (Convert.ToInt32(Form1.days[i].outdoorCondition) >= 600
                 && Convert.ToInt32(Form1.days[i].outdoorCondition) < 623)
                {
                    WeatherIconOutput.Text += "❄"; ;
                }
                else
                {
                    WeatherIconOutput.Text += "🌣";
                }
                WeatherIconOutput.Text += "\n \n";

            }
        }
        // gets the forcast for the city entered
        private void ExtractForecast(string city)
        {
            XmlReader reader = XmlReader.Create($"http://api.openweathermap.org/data/2.5/forecast/daily?q={city}&mode=xml&units=metric&cnt=7&appid=3f2e224b815c0ed45524322e145149f0");

            while (reader.Read())
            {
                //create a day object
                Day newDay = new Day();

                //fill day object with required data
                reader.ReadToFollowing("time");
                newDay.date = reader.GetAttribute("day");

                reader.ReadToFollowing("symbol");
                newDay.outdoorCondition = reader.GetAttribute("number");

                reader.ReadToFollowing("temperature");
                newDay.tempLow = reader.GetAttribute("min");
                newDay.tempHigh = reader.GetAttribute("max");

                //TODO: if day object not null add to the days list
                Form1.days.Add(newDay);
            }
        }
        // gets current info from the city entered
        private void ExtractCurrent(string city)
        {
            // current info is not included in forecast file so we need to use this file to get it


            XmlReader reader = XmlReader.Create($"http://api.openweathermap.org/data/2.5/weather?q={city}&mode=xml&units=metric&appid=3f2e224b815c0ed45524322e145149f0");

            //find the city and current temperature and add to appropriate item in days list
            reader.ReadToFollowing("city");
            Form1.days[0].location = reader.GetAttribute("name");

            reader.ReadToFollowing("temperature");
            Form1.days[0].currentTemp = reader.GetAttribute("value");

        }
    }
}
