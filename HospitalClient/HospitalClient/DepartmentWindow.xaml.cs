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
using System.Windows.Shapes;

namespace HospitalClient
{
    /// <summary>
    /// Interaction logic for Department.xaml
    /// </summary>
    public partial class DepartmentWindow : Window
    {
        public DepartmentWindow(string hospitalCode,string hospitalName)
        {

            InitializeComponent();
            lblHospitalName.Content = hospitalName;

            setDepartments(hospitalCode);
            ShowDialog();

        }

        private void setDepartments(string hospitalCode)
        {
           Dictionary<int,string> departments=getDepartmentNames(hospitalCode);
            int i = 3;
             foreach (var item in departments)
            {
                DepartmentUCtr uc = new DepartmentUCtr(item.Value, item.Key);
                this.grdDepartment.RowDefinitions.Add(new RowDefinition ());
                this.grdDepartment.Children.Add(uc);
                uc.SetValue(Grid.RowProperty, i);
                uc.SetValue(Grid.ColumnProperty, 4);
                uc.SetValue(Grid.ColumnSpanProperty , 22);

                i++;
            }
        }

        private Dictionary<int,string> getDepartmentNames(string hospitalCode)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:51037/");
            client.DefaultRequestHeaders.Accept.Add(
             new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("api/Data/" + hospitalCode).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<Dictionary<int,string>>().Result;

            }

            return null;

        }


    }
}
