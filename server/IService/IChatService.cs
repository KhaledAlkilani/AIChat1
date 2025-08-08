namespace AIChat1.IService
{
    public interface IChatService
    {
        /// <summary>
        /// Sends the user's message to the AI engine and returns its reply.
        /// </summary>
        /// <param name="userName">The authenticated user's name or ID.</param>
        /// <param name="message">The user's input text.</param>
        /// <returns>The AI-generated response.</returns>
        Task<string> GetAiResponseAsync(string userName, string message);
    }
}
