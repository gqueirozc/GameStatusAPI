using System.Diagnostics.CodeAnalysis;
using GameStatusAPI.Interfaces;

namespace GameStatusAPI.Factories
{
    public class RegularHttpClient : BaseHttpClient, IRegularHttpClient
    {
        [ExcludeFromCodeCoverage]
        public RegularHttpClient(HttpClient httpClient) : base(httpClient)
        {
        }

        [ExcludeFromCodeCoverage]
        protected override void SetupHeaders()
        {
        }
    }
}
