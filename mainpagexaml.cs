using System.Xml.Xsl;

namespace LAB_2._2
{
    public partial class MainPage : ContentPage
    {
        private IXmlStrategy _strategy;
        private string _localXmlPath = Path.Combine(FileSystem.CacheDirectory, "dormitory.xml");
        private string _localXslPath = Path.Combine(FileSystem.CacheDirectory, "dormitory.xsl");

        public MainPage()
        {
            InitializeComponent();
            PrepareFiles();
        }

        private async void PrepareFiles()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("dormitory.xml");
                using var fileStream = File.Create(_localXmlPath);
                await stream.CopyToAsync(fileStream);

                using var xslStream = await FileSystem.OpenAppPackageFileAsync("dormitory.xsl");
                using var xslFileStream = File.Create(_localXslPath);
                await xslStream.CopyToAsync(xslFileStream);

                LoadAttributes(); 
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Не вдалося завантажити ресурси: {ex.Message}", "OK");
            }
        }

        private void LoadAttributes()
        {
            if (File.Exists(_localXmlPath))
            {
                var doc = System.Xml.Linq.XDocument.Load(_localXmlPath);
                var faculties = doc.Descendants("Student")
                                   .Select(x => x.Attribute("Faculty")?.Value)
                                   .Where(f => !string.IsNullOrEmpty(f))
                                   .Distinct()
                                   .ToList();
                FacultyPicker.ItemsSource = faculties;
            }
        }

        private void OnSearchClicked(object sender, EventArgs e)
        {
            if (DomRadio.IsChecked)
                _strategy = new DomStrategy();
            else if (SaxRadio.IsChecked)
                _strategy = new SaxStrategy();
            else
                _strategy = new LinqToXmlStrategy();

            var filter = new Student
            {
                Faculty = FacultyPicker.SelectedItem?.ToString()
            };

            var results = _strategy.Search(_localXmlPath, filter);
            OutputListView.ItemsSource = results;
        }

        private async void OnTransformClicked(object sender, EventArgs e)
        {
            try
            {
                string htmlPath = Path.Combine(FileSystem.CacheDirectory, "result.html");

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(_localXslPath);
                transform.Transform(_localXmlPath, htmlPath);

                await DisplayAlert("Успіх", $"HTML файл створено у: {htmlPath}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            FacultyPicker.SelectedItem = null;
            OutputListView.ItemsSource = null;
            DomRadio.IsChecked = true;
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                bool exit = await DisplayAlert("Вихід", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
                if (exit)
                {
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            });
            return true;
        }
    }
}
