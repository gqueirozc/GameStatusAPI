namespace GameStatusAPI.Interfaces
{
    public interface IRegularHttpClient
    {
        /// <summary>
        /// Sends a GET request to the specified URL and returns the response body as a string.
        /// </summary>
        /// <param name="url">The URL to send the GET request to.</param>
        /// <returns>A task representing the asynchronous operation. The result of the task is the response body as a string.</returns>
        Task<string> GetStringAsync(string url);

        /// <summary>
        /// Sends a POST request to the specified URL with the provided payload and returns the response message.
        /// </summary>
        /// <param name="url">The URL to send the POST request to.</param>
        /// <param name="payload">The content to include in the request body.</param>
        /// <returns>A task representing the asynchronous operation. The result of the task is the response message.</returns>
        Task<HttpResponseMessage> PostAsync(string url, StringContent payload);
    }
}