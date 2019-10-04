using System;
using Android.Content;

namespace ReactiveSearch.Droid
{
    internal class AndroidConfigProvider : IConfigProvider
    {
        private readonly Context context;

        public AndroidConfigProvider(Context context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public string AppCenterApiToken => $"android={context.GetString(Resource.String.app_center_api_token)};";
    }
}