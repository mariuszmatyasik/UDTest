using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UDTest
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var getData = FetchData();
        }

        public static IList<Testlist> testlists = new List<Testlist>();
        public static JArray testListModelArray = new JArray();

        public static async Task FetchData()
        {
            using (var db = new TestlistDBEnt())
            {
                testlists = await (from name in db.Testlists select name).ToListAsync();
                db.Dispose();
            }
        }

        public void PopulateData()
        {
            listBox.Items.Clear();
            
            foreach (Testlist item in testlists)
            {
                listBox.Items.Add(item);
            }
        }

        public async Task<IList<Testlist>> UpdateList()
        {
            IList<Testlist> lModel = new List<Testlist>();

            using (var httpClient = new HttpClient())
                try
                {
                    String json = await httpClient.GetStringAsync("http://testyudt.com/UDT_APP/API/index.php?api_key=cdad5e6b5ab66cd4e10b6ace30fee27c&api_tests_list=show");
                    //  Now parse with JSON.Net
                    testListModelArray = JArray.Parse(json);

                    IList<Testlist> listModel = testListModelArray.Select(p => new Testlist
                    {
                        url = (string)p["url"],
                        name = (string)p["name"],
                        description = (string)p["description"],
                        question_pool = (int)p["question-pool"],
                        answers_limit = (int)p["answers-limit"],
                        threshold = (int)p["threshold"],
                        time = (int)p["time"]
                    }).ToList();

                    lModel = listModel;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Connection Error", MessageBoxButton.OK);
                }

            return lModel;
        }

        private async void load_Testlist_Click(object sender, RoutedEventArgs e)
        {
            var testList = await UpdateList();

                Testlist newItem = new Testlist
                {
                    Id = testList[0].Id,
                    name = testList[0].name,
                    description = testList[0].description,
                    question_pool = testList[0].question_pool,
                    answers_limit = testList[0].answers_limit,
                    threshold = testList[0].threshold,
                    time = testList[0].time,
                    url = testList[0].url
                };
            
            // Deleting entities from actual database
            using (var db = new TestlistDBEnt())
            {
                var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)db).ObjectContext;
                objCtx.ExecuteStoreCommand("DELETE FROM [Testlist]");
                await db.SaveChangesAsync();
                db.Dispose();
            }

            // Adding newly parsed entities to database
            using (var db = new TestlistDBEnt())
            {
                db.Testlists.Add(newItem);
                await db.SaveChangesAsync();
                db.Dispose();
            }

            var newData = FetchData();
            PopulateData();
        }

        private void btn_start_test_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem == null)
            {
                MessageBox.Show("Proszę wybrać test!");
                return;
            }

            Testlist selectedUrl = listBox.SelectedItem as Testlist;
            string url = selectedUrl.url.ToString();

            var newFrame = new TestFrame(this, url);
            newFrame.Show();
            this.Hide();
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Testlist selected = listBox.SelectedItem as Testlist;
            
            if (selected != null)
            {
                description_label.Text = selected.description.ToString();
                q_pool.Content = selected.question_pool.ToString();
                a_limit.Content = selected.answers_limit.ToString();
                threshold_nr.Content = selected.threshold.ToString() + "/" + selected.question_pool;
                time_nr.Content = (selected.time / 60).ToString() + " minut";
            }
            else if (selected == null)
            {
                description_label.Text = "";
                q_pool.Content = "";
                a_limit.Content = "";
                threshold_nr.Content = "";
                time_nr.Content = "";
            }
            else return;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown(1);
        }

    }
}
