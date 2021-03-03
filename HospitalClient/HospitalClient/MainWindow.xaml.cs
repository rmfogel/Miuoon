using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
using System.Xml;

namespace HospitalClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int triels=0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private List<string> BindHospitalNames()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:51037/");
            client.DefaultRequestHeaders.Accept.Add(
             new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("api/Data").Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<List<string>>().Result;

            }

            return null;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> hospitals = BindHospitalNames();
            if (hospitals != null)
                if (triels < 3)
                {

                    if (hospitals.IndexOf(txtHOspitalName.Text) - 1 == hospitals.IndexOf(txtPassword.Password))
                    {
                        var window = new DepartmentWindow(txtPassword.Password, txtHOspitalName.Text);
                        //  window.ShowDialog();
                        this.Close();

                    }

                    else
                    {
                        triels++;
                        txtHOspitalName.Text = "";
                        txtPassword.Password = "";
                    }

                    }

                else
                {
                    MessageBox.Show("שם המשתמש או הסיסמא שגויים", "התוכנית תסגר", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();

                }
        }

    }


}

