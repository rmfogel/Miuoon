using System;
using System.Collections.Generic;
using System.Linq;
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

namespace HospitalClient
{
    /// <summary>
    /// Interaction logic for DepartmentUCtr.xaml
    /// </summary>
    public partial class DepartmentUCtr : UserControl
    { const string url= @"C:\Users\This_User\Music\";
        private int departmentCode;
        HttpClient client = new HttpClient();
       
        public DepartmentUCtr(string departnemtName,int departmentCode)
        {
            client.BaseAddress = new Uri("http://localhost:51037/");
            client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));

            this.departmentCode = departmentCode;
            InitializeComponent();
            dptnameLbl.Content = departnemtName;
        }

        private void addWaiterBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckReturnStatus(client.GetAsync("api/AddOneWaiter/" + departmentCode).Result);
            }

            catch(Exception)
            {
                PlaySound("failure");
            }

        }
           
        private void ExitWaiterBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                CheckReturnStatus(client.GetAsync("api/ExitOneWaiter/" + departmentCode).Result);
            }

            catch(Exception)
            {
                PlaySound("failure");
            }


          


        }

        private void addStaffBtn_Click(object sender, RoutedEventArgs e)
        {
            CheckReturnStatus(client.GetAsync("api/AddOneStaff/" + departmentCode).Result);

        }

        private void ExitStaffBtn_Click(object sender, RoutedEventArgs e)
        {
            CheckReturnStatus(client.GetAsync("api/ExitOneStaff/" + departmentCode).Result);

        }

        public void CheckReturnStatus(HttpResponseMessage response)
            {
                if (response.IsSuccessStatusCode)
                {
                    if (response.Content.ReadAsAsync<Boolean>().Result)
                    {
                        PlaySound("success");
                    }

                    else
                    {
                        PlaySound("failure");

                    }

                }

                else
                {
                    PlaySound("failure");

                }

            }


        

        public static void PlaySound(string soundName)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer($"{url}{soundName}.wav");
            player.Play();
        }
    }
}
