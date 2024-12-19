using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
    /// Логика взаимодействия для AddPage.xaml
    /// </summary>
    public partial class AddPage : Page
    {
        private Agent currentAgent = new Agent();
        private List<Product> Items;
        private ProductSale currentProductSale = new ProductSale();

        public AddPage(Agent SelectedAgent)
        {
            InitializeComponent();
            if (SelectedAgent != null)
                currentAgent = SelectedAgent;

            var allProducts =ShennikovaGlazkiSaveEntities.GetContext().Product.ToList();

            var currentProductSales = ShennikovaGlazkiSaveEntities.GetContext().ProductSale.ToList();
            currentProductSales = currentProductSales.Where(p => p.AgentID == currentAgent.ID).ToList();

            History.ItemsSource = currentProductSales;


            DataContext = currentProductSale;


            DataContext = currentAgent;
            LoadItems();
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                currentAgent.Logo = myOpenFileDialog.FileName;
                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            currentAgent.AgentTypeID = ComboType1.SelectedIndex + 1;
            if (string.IsNullOrWhiteSpace(currentAgent.Title))
                errors.AppendLine("Укажите наименование агента"); 
            if (string.IsNullOrWhiteSpace(currentAgent.Address))
                errors.AppendLine("Укажите адрес агента"); 
            if (string.IsNullOrWhiteSpace(currentAgent.DirectorName))
                errors.AppendLine("Укажите ФИО директора");
            if (ComboType1.SelectedItem == null)
                errors.AppendLine("Укажите тип агента"); 
            if (string.IsNullOrWhiteSpace(currentAgent.Priority.ToString()))
                errors.AppendLine("Укажите приоритет агента"); 
            if (currentAgent.Priority <= 0)
                errors.AppendLine("Укажите положительный приоритет агента"); 
            if (string.IsNullOrWhiteSpace(currentAgent.INN))
                errors.AppendLine("Укажите ИНН агента");
            if (string.IsNullOrWhiteSpace(currentAgent.KPP))
                errors.AppendLine("Укажите КПП агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Phone))
                errors.AppendLine("Укажите телефон агента");
            else
            {
                string ph = currentAgent.Phone.Replace("(", " ").Replace("-", "").Replace("+", "");
                if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11)
                    || (ph[1] == '3' && ph.Length != 12) )
                    errors.AppendLine("Укажите правильно телефон агента");

                string kpp = currentAgent.KPP;
                if ((kpp.Length != 9))
                    errors.AppendLine("Укажите корректный КПП агента. В КПП должно быть 9 цифр.");

                string inn = currentAgent.INN;
                if ((inn.Length != 10))
                    errors.AppendLine("Укажите корректный ИНН агента. В ИНН должно быть 10 цифр");

            }

            if (string.IsNullOrWhiteSpace(currentAgent.Email))
                errors.AppendLine("Укажите почту агента");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            
            if (currentAgent.ID == 0)
                ShennikovaGlazkiSaveEntities.GetContext().Agent.Add(currentAgent);
            try
            {
                ShennikovaGlazkiSaveEntities.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager1.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {


            var currentProductSale = ShennikovaGlazkiSaveEntities.GetContext().ProductSale.ToList();
            currentProductSale = currentProductSale.Where(p => p.AgentID == currentAgent.ID).ToList();

            if (currentProductSale.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление, так как существует история реализации продуктов");
            else
            {
                var currentAgentPriorityHistory = ShennikovaGlazkiSaveEntities.GetContext().AgentPriorityHistory.ToList();
                var currentShop = ShennikovaGlazkiSaveEntities.GetContext().Shop.ToList();
                currentAgentPriorityHistory = currentAgentPriorityHistory.Where(p => p.AgentID == currentAgent.ID).ToList();
                currentShop = currentShop.Where(p => p.AgentID == currentAgent.ID).ToList();


                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        ShennikovaGlazkiSaveEntities.GetContext().Agent.Remove(currentAgent);

                        if (currentAgentPriorityHistory.Count != 0)
                        {
                            for (int i = 0; currentAgentPriorityHistory.Count == i; i++)
                                ShennikovaGlazkiSaveEntities.GetContext().AgentPriorityHistory.Remove(currentAgentPriorityHistory[i]);
                        }
                        if (currentShop.Count != 0)
                        {
                            for (int i = 0; currentShop.Count == i; i++)
                                ShennikovaGlazkiSaveEntities.GetContext().Shop.Remove(currentShop[i]);
                        }
                        ShennikovaGlazkiSaveEntities.GetContext().SaveChanges();

                        MessageBox.Show("Информация удалена!");
                        Manager1.MainFrame.GoBack();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void LoadItems()
        {
            // Здесь вы загружаете элементы из базы данных
            Items = ShennikovaGlazkiSaveEntities.GetContext().Product.ToList();
            ComboTitle.ItemsSource = Items; // Устанавливаем источник данных для ComboBox
        }


        private void SearchBoxForComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBoxForComboBox.Text.ToLower();
            var filteredItems = Items.Where(p => p.Title.ToLower().Contains(searchText)).ToList();
            ComboTitle.ItemsSource = filteredItems;

            // Если ничего не найдено, показываем все элементы
            if (string.IsNullOrEmpty(searchText))
            {
                ComboTitle.ItemsSource = Items;
            }
        }

        private void ComboTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboTitle.SelectedItem is Product selectedProduct)
            {
                // Подставляем значение Title в TextBox
                SearchBoxForComboBox.Text = selectedProduct.Title; // Убедитесь, что у вас есть TextBox с именем SearchBoxForComboBox
            }
        }


        private void AddHistory_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ComboTitle.SelectedItem as Product;

            // Получаем количество из TextBox (например, для количества продукта)
            int productCount;
            if (!int.TryParse(CountProductTB.Text, out productCount) || productCount <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректное количество.");
                return;
            }

            if (selectedProduct != null)
            {
                // Создаем новый объект ProductSale
                var newSale = new ProductSale
                {
                    AgentID = currentAgent.ID, // Указываем ID агента
                    ProductID = selectedProduct.ID, // Указываем ID выбранного продукта
                    SaleDate = StartDate.SelectedDate ?? DateTime.Now, // Указываем дату продажи (если выбрана)
                    ProductCount = productCount // Указываем количество, введенное пользователем
                };

                // Добавляем новый объект в контекст и сохраняем изменения
                ShennikovaGlazkiSaveEntities.GetContext().ProductSale.Add(newSale);
                ShennikovaGlazkiSaveEntities.GetContext().SaveChanges();

                MessageBox.Show("информация сохранена");

                // Обновляем список продаж
                LoadProductSalesForCurrentAgent();

                // Очистка полей ввода (по желанию)
                ComboTitle.SelectedItem = null;
                CountProductTB.Clear();
                StartDate.SelectedDate = null;
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите продукт для добавления.");
            }
        }

        private void LoadProductSalesForCurrentAgent()
        {
            var currentProductSales = ShennikovaGlazkiSaveEntities.GetContext().ProductSale
                .Where(ps => ps.AgentID == currentAgent.ID) // Предполагается, что у ProductSale есть поле AgentID
                .ToList();

            // Устанавливаем источник данных для списка продаж
            History.ItemsSource = currentProductSales;
        }

        private void DelHistory_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = History.SelectedItems.Cast<ProductSale>().ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите хотя бы один элемент для удаления.");
                return;
            }
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление? ", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var context = ShennikovaGlazkiSaveEntities.GetContext();

                        // Удаляем каждый выбранный элемент из контекста
                        foreach (var item in selectedItems)
                        {
                            context.ProductSale.Remove(item);
                        }

                        // Сохраняем изменения в базе данных
                        context.SaveChanges();

                        LoadProductSalesForCurrentAgent();

                        MessageBox.Show("Элементы успешно удалены.");
                    }


                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

}

