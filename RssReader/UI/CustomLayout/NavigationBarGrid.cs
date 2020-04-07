using Xamarin.Forms;

namespace RssReader.UI.CustomLayout
{
    public class NavigationBarGrid : Grid
    {
        public static readonly BindableProperty DegreeOfViewProperty = BindableProperty.Create(
            nameof(DegreeOfView),
            typeof(double),
            typeof(NavigationBarGrid),
            defaultValue: 2.0,
            propertyChanged: CountOfView_PropertyChanged);
        public double DegreeOfView
        {
            get => (double)GetValue(DegreeOfViewProperty);
            set => SetValue(DegreeOfViewProperty, value);
        }

        public NavigationBarGrid()
        {
            HeightRequest = 50;
            MinimumHeightRequest = 50;
            Padding = new Thickness(16, 0);
            ColumnSpacing = 0;
            RowSpacing = 0;
            BackgroundColor = Color.Black;

            CountOfView_PropertyChanged(this, null, null);
        }

        private static void CountOfView_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var layout = (bindable as NavigationBarGrid);
            layout.ColumnDefinitions.Clear();

            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(layout.DegreeOfView, GridUnitType.Star) });
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            layout.RowDefinitions.Add(new RowDefinition { Height = 49.5 });
            layout.RowDefinitions.Add(new RowDefinition { Height = 0.5 });


            var style = (Style)Application.Current.Resources["SeparatorLine"];
            BoxView separator = new BoxView()
            {
                Style = style,
                Margin = new Thickness(-16, 0),
                VerticalOptions = LayoutOptions.End
            };
            layout.Children.Add(separator, 0, 1);
            SetColumnSpan(separator, 3);
        }
    }
}
