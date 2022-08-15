using System.ComponentModel;
using Android.Graphics;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using counttimeforms.Effects;
using counttimeforms.Droid.Effects;

[assembly: ResolutionGroupName("counttimeforms")]
[assembly: ExportEffect(typeof(TintColorEffect), nameof(TintEffect))]
namespace counttimeforms.Droid.Effects {
    public class TintColorEffect : PlatformEffect {
        ImageView control;

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args) {
            if (args.PropertyName == TintEffect.TintColorProperty.PropertyName) {
                UpdateColor();
            }
        }

        protected override void OnAttached() {
            this.control = Control as ImageView;
            UpdateColor();
        }

        protected override void OnDetached() {
            this.control = null;
        }

        void UpdateColor() {
            try {
                var color = TintEffect.GetTintColor(Element);
                var filter = new PorterDuffColorFilter(color.ToAndroid(), PorterDuff.Mode.SrcIn);
                this.control.SetColorFilter(filter);
            } catch {
                //do nothing
            }
        }
    }
}
