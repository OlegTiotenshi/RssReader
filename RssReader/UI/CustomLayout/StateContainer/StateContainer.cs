using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RssReader.UI.CustomLayout.StateContainer
{
    [ContentProperty("Conditions")]
    public class StateContainer : ContentView
    {
        public List<StateCondition> Conditions { get; set; } = new List<StateCondition>();

        public static readonly BindableProperty StateProperty = BindableProperty.Create(nameof(State), typeof(object), typeof(StateContainer), null, BindingMode.Default, null, propertyChanged: StateChanged);

        public static void Init()
        {
        }

        private static async void StateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var parent = bindable as StateContainer;
            if (parent != null)
                await parent.ChooseStateProperty(newValue);
        }

        public object State
        {
            get { return this.GetValue(StateProperty); }
            set { this.SetValue(StateProperty, value); }
        }

        private async Task ChooseStateProperty(object newValue)
        {
            if (Conditions == null && Conditions?.Count == 0) return;

            try
            {
                foreach (var stateCondition in Conditions.Where(stateCondition => stateCondition.State != null && stateCondition.State.ToString().Equals(newValue.ToString())))
                {
                    if (Content != null)
                    {
                        await Content.FadeTo(0, 250U);
                        await Task.Delay(30);
                    }

                    Content = null;
                    Content = stateCondition.Content;
                    await Content.FadeTo(1, 250U);
                    break;
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
