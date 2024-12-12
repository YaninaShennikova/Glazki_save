using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Щенникова_ГлазкиСейв
{
    /// <summary>
    /// Логика взаимодействия для AgentPage.xaml
    /// </summary>
    public partial class AgentPage : Page
    {
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;

        List<Agent> CurrentPageList = new List<Agent>();
        List<Agent> TableList;

        public AgentPage()
        {
            InitializeComponent();
            var currentAgent=ShennikovaGlazkiSaveEntities.GetContext().Agent.ToList();
            AgentListView.ItemsSource = currentAgent;

            ComboType.SelectedIndex = 0;
            ComboSort.SelectedIndex = 0;

            UpdateAgent();
        }

        private void UpdateAgent()
        {
            var currentAgent = ShennikovaGlazkiSaveEntities.GetContext().Agent.ToList();

            currentAgent = currentAgent.Where(p => p.Email.ToLower().Contains(TBoxSearch.Text.ToLower()) ||
            p.Title.ToLower().Contains(TBoxSearch.Text.ToLower()) ||
            p.Phone.ToLower().Replace(" (", "").Replace(")", "").Replace(" ", "").Replace("-", "").Contains(TBoxSearch.Text.ToLower())).ToList();



            

            if (ComboSort.SelectedIndex == 2)
            {
                currentAgent = currentAgent.OrderBy(p => p.Title).ToList();
            }
            if (ComboSort.SelectedIndex == 1)
            {
                currentAgent = currentAgent.OrderByDescending(p => p.Title).ToList();
            }
            if (ComboSort.SelectedIndex == 5)
            {
                currentAgent = currentAgent.OrderBy(p => p.Priority).ToList();
            }
            if (ComboSort.SelectedIndex == 6)
            {
                currentAgent = currentAgent.OrderByDescending(p => p.Priority).ToList();
            }


            if (ComboType.SelectedIndex == 0)
            {
                currentAgent = currentAgent.Where(p =>
                (p.AgentType.Title.Contains("МФО") ||
                p.AgentType.Title.Contains("ООО") ||
                p.AgentType.Title.Contains("ЗАО") ||
                p.AgentType.Title.Contains("МКК") ||
                p.AgentType.Title.Contains("ОАО") ||
                p.AgentType.Title.Contains("ПАО"))).ToList();
            }
           
            if (ComboType.SelectedIndex == 1)
            {
                currentAgent = currentAgent.Where(p => (p.AgentType.Title.Contains("МФО"))).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentAgent = currentAgent.Where(p => (p.AgentType.Title.Contains("ООО"))).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentAgent = currentAgent.Where(p => (p.AgentType.Title.Contains("ЗАО"))).ToList();
            }
            if (ComboType.SelectedIndex == 4)
            {
                currentAgent = currentAgent.Where(p => (p.AgentType.Title.Contains("МКК"))).ToList();
            }
            if (ComboType.SelectedIndex == 5)
            {
                currentAgent = currentAgent.Where(p => (p.AgentType.Title.Contains("ОАО"))).ToList();
            }
            if (ComboType.SelectedIndex == 6)
            {
                currentAgent = currentAgent.Where(p => (p.AgentType.Title.Contains("ПАО"))).ToList();
            }



            // Обновляем источник данных для ListView
            AgentListView.ItemsSource = currentAgent;
            TableList = currentAgent;
            ChangePage(0, 0);

        }

        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;

            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }

            Boolean Ifupdate = true;

            int min;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else
            {
                switch (direction)
                {


                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;

                    case 2:
                        if (CurrentPage < CountPage - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;

                }

            }
            if (Ifupdate)
            {
                PageListBox.Items.Clear();
                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
               
                /*
                TBCount.Text = min.ToString();
                TBAllRecords.Text = " из " + CountRecords.ToString();
                */

                AgentListView.ItemsSource = CurrentPageList;
                AgentListView.Items.Refresh();


            }


        }

        


        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgent();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgent();
        }

   

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            UpdateAgent();
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager1.MainFrame.Navigate(new AddPage((sender as Button).DataContext as Agent));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                ShennikovaGlazkiSaveEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                AgentListView.ItemsSource = ShennikovaGlazkiSaveEntities.GetContext().Agent.ToList();
                UpdateAgent();
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager1.MainFrame.Navigate(new AddPage(null));
        }
    }
}
