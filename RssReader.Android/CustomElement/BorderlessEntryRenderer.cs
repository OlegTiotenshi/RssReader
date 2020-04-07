using Android.Content;
using RssReader.Droid.CustomElement;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(BorderlessEntryRenderer))]

namespace RssReader.Droid.CustomElement
{
    public class BorderlessEntryRenderer : EntryRenderer
    {
        public BorderlessEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e == null)
                return;

            Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            using (var layoutParams = new MarginLayoutParams(Control.LayoutParameters))
            {
                layoutParams.SetMargins(0, 0, 0, 0);
                Control.LayoutParameters = layoutParams;
                Control.SetPadding(Control.Left, 0, Control.Right, 0);
            }
        }
    }
}