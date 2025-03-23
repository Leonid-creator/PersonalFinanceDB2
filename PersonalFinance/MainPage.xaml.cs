
using Microsoft.Maui.Controls;

namespace PersonalFinance;

public partial class MainPage : ContentPage
{
    private List<View> addedProducts = new();
    public MainPage()
    {
        InitializeComponent();

        storePicker.ItemsSource = new List<string> { "Lidl", "Tesco", "Dunnes" };
    }

    private void OnStoreSelected(object sender, EventArgs e)
    {
        string selectedStore = storePicker.SelectedItem as string;
        //DisplayAlert("Selected", $"You select: {selectedStore}", "OK");
    }
    private void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        DateTime selectedDate = e.NewDate;
        DisplayAlert("Выбрано", $"Выбрана дата: {selectedDate.ToShortDateString()}", "OK");
    }

    private void OnProcessReceiptClicked(object sender, EventArgs e)
    {

    }
    private void AddRowToGrid()
    {
        Grid dynamicGrid1 = new Grid {Margin = new Thickness(0)};
        Grid dynamicGrid2 = new Grid {Margin = new Thickness(0, 0, 0, 15)};

        dynamicGrid1.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        dynamicGrid1.ColumnDefinitions.Add(new ColumnDefinition { Width = 40 });
        dynamicGrid1.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        dynamicGrid1.ColumnDefinitions.Add(new ColumnDefinition { Width = 60 });
        dynamicGrid1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        dynamicGrid2.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        dynamicGrid2.ColumnDefinitions.Add(new ColumnDefinition { Width = 50 });
        dynamicGrid2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        dynamicGrid2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        Entry quantity = new Entry { Placeholder = "Q-ty" };
        Grid.SetRow(quantity, 0);
        Grid.SetColumn(quantity, 0);
        dynamicGrid1.Children.Add(quantity);

        Entry productName = new Entry { Placeholder = "Prod. name" };
        Grid.SetRow(productName, 0);
        Grid.SetColumn(productName, 1);
        dynamicGrid1.Children.Add(productName);

        Entry amount = new Entry { Placeholder = "Amount" };
        Grid.SetRow(amount, 0);
        Grid.SetColumn(amount, 2);
        dynamicGrid1.Children.Add(amount);

        Button removeProductBTN = new Button { Text = "X", WidthRequest = 40, HeightRequest = 40, Margin = new Thickness(10, 0, 0, 0) };
        Grid.SetRow(removeProductBTN, 0);
        Grid.SetColumn(removeProductBTN, 3);
        removeProductBTN.Clicked += RemoveRow;
        dynamicGrid1.Children.Add(removeProductBTN);

        Entry discount = new Entry { Placeholder = "Disc" };
        Grid.SetRow(discount, 1);
        Grid.SetColumn(discount, 0);
        dynamicGrid2.Children.Add(discount);

        Entry category = new Entry { Placeholder = "Category" };
        Grid.SetRow(category, 1);
        Grid.SetColumn(category, 1);
        dynamicGrid2.Children.Add(category);

        Entry subcategory = new Entry { Placeholder = "Subcategory" };
        Grid.SetRow(subcategory, 1);
        Grid.SetColumn(subcategory, 2);
        Grid.SetColumnSpan(subcategory, 2);
        dynamicGrid2.Children.Add(subcategory);

        VerticalStackLayout stackLayout = this.FindByName<VerticalStackLayout>("MyStackLayout");
        productsLayout.Children.Add(dynamicGrid1);
        productsLayout.Children.Add(dynamicGrid2);
    }

    private void OnAddProductClicked(object sender, EventArgs e)
    {
        AddRowToGrid();
    }

    private void RemoveRow()
    {

    }
    private void RemoveRow(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            // Получаем родительский Grid (dynamicGrid1)
            if (button.Parent is Grid dynamicGrid1)
            {
                // Получаем индекс dynamicGrid1 в productsLayout
                int index = productsLayout.Children.IndexOf(dynamicGrid1);

                if (index != -1 && index + 1 < productsLayout.Children.Count)
                {
                    // Удаляем оба Grid (dynamicGrid1 и dynamicGrid2)
                    productsLayout.Children.RemoveAt(index);     // Удаляем первый Grid
                    productsLayout.Children.RemoveAt(index);     // Удаляем второй Grid (он сдвинется на место первого)
                }
            }
        }
    }
}
