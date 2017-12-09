using Xamarin.Forms;

namespace ReactiveSearch.Extensions
{
    internal static class StyleExtensions
    {
        public static Style Extend(this Style style)
        {
            return new Style(style.TargetType)
            {
                BasedOn = style
            };
        }

        public static Style Set<T>(this Style style, BindableProperty property, T value)
        {
            style.Setters.Add(new Setter() { Property = property, Value = value });
            return style;
        }
    }
}
