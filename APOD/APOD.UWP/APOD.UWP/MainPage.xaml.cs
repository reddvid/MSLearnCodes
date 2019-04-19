using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace APOD.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string endPointUrl = "https://api.nasa.gov/planetary/apod";

        DateTime launchDate = new DateTime(1995, 6, 16);

        int imageCountToday;

        const string settingDateToday = "date today";
        const string settingShowOnStartup = "show on startup";
        const string settingImageCountToday = "image count today";
        const string settingLimitRange = "limit range";

        ApplicationDataContainer localSettings;

        public MainPage()
        {
            this.InitializeComponent();

            localSettings = ApplicationData.Current.LocalSettings;

            MonthCalendar.MinDate = launchDate;
            MonthCalendar.MaxDate = DateTime.Today;

            ReadSettings();
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            LimitRangeCheckBox.IsChecked = false;

            MonthCalendar.Date = launchDate;
        }

        private void LimitRangeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var firstDayOfThisYear = new DateTime(DateTime.Today.Year, 1, 1);
            MonthCalendar.MinDate = firstDayOfThisYear;
        }

        private void LimitRangeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            MonthCalendar.MinDate = launchDate;
        }

        private async void MonthCalendar_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            await RetrievePhoto();
        }

        private async Task RetrievePhoto()
        {
            var client = new HttpClient();
            JObject jResult = null;
            string responseContent = null;
            string description = null;
            string photoUrl = null;
            String copyright = null;

            ImageCopyrightTextBox.Text = "NASA";
            DescriptionTextBox.Text = "";

            DateTimeOffset dto = (DateTimeOffset)MonthCalendar.Date;

            string dateSelected = $"{dto.Year.ToString()}-{dto.Month.ToString("00")}-{dto.Day.ToString("00")}";
            string urlParams = $"?date={dateSelected}&api_key=DEMO_KEY";

            client.BaseAddress = new Uri(endPointUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync(urlParams).Result;

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    responseContent = await response.Content.ReadAsStringAsync();

                    jResult = JObject.Parse(responseContent);

                    photoUrl = (string)jResult["url"];
                    var photoUri = new Uri(photoUrl);
                    var bitmapImage = new BitmapImage(photoUri);

                    ImagePictureBox.Source = bitmapImage;

                    if (IsSupoortedFormat(photoUrl))
                    {
                        copyright = (string)jResult["copyright"];
                        if (copyright != null && copyright.Length > 0)
                        {
                            ImageCopyrightTextBox.Text = copyright;
                        }

                        description = (string)jResult["explanation"];
                        DescriptionTextBox.Text = description;
                    }
                    else
                    {
                        DescriptionTextBox.Text = $"Image type is not supported. URL is {photoUrl}";
                    }
                }
                catch (Exception ex)
                {
                    DescriptionTextBox.Text = $"Image data is not supported. {ex.Message}";
                }

                ++imageCountToday;
                ImagesTodayTextBox.Text = imageCountToday.ToString();
            }
            else
            {
                DescriptionTextBox.Text = "We ere unable to retrieve the NASA picture for that day: " + $"{response.StatusCode.ToString()} {response.ReasonPhrase}";
            }
        }

        private bool IsSupoortedFormat(string photoUrl)
        {
            string ext = Path.GetExtension(photoUrl).ToLower();

            return (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext == ".tif" || ext == ".bmp" || ext == ".ico" || ext == ".svg");
        }

        private void Grid_LostFocus(object sender, RoutedEventArgs e)
        {
            WriteSettings();
        }

        private void WriteSettings()
        {
            localSettings.Values[settingDateToday] = DateTime.Today.ToString();
            localSettings.Values[settingShowOnStartup] = ShowTodaysImageCheckBox.IsChecked.ToString();
            localSettings.Values[settingLimitRange] = LimitRangeCheckBox.IsChecked.ToString();
            localSettings.Values[settingImageCountToday] = imageCountToday.ToString();
        }

        private void ReadSettings()
        {
            bool isToday = false;
            var todayObject = localSettings.Values[settingDateToday];

            if (todayObject != null)
            {
                var dateTime = DateTime.Parse((string)todayObject);
                if (dateTime.Equals(DateTime.Today))
                {
                    isToday = true;
                }
            }

            imageCountToday = 0;

            if (isToday)
            {
                var value = localSettings.Values[settingImageCountToday];
                if (value != null)
                {
                    imageCountToday = int.Parse((string)value);
                }
            }
            ImagesTodayTextBox.Text = imageCountToday.ToString();

            var showTodayObject = localSettings.Values[settingShowOnStartup];
            if (showTodayObject != null)
            {
                ShowTodaysImageCheckBox.IsChecked = bool.Parse((string)showTodayObject);
            }
            else
            {
                ShowTodaysImageCheckBox.IsChecked = true;
            }

            var limitRangeObject = localSettings.Values[settingLimitRange];
            if (limitRangeObject != null)
            {
                LimitRangeCheckBox.IsChecked = bool.Parse((string)limitRangeObject);
            }
            else
            {
                LimitRangeCheckBox.IsChecked = false;
            }

            if (ShowTodaysImageCheckBox.IsChecked == true)
            {
                MonthCalendar.Date = DateTime.Today;
            }
        }
    }
}
