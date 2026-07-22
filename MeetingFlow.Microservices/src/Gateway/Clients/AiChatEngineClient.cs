namespace Gateway.Clients;

public class AiChatEngineClient(HttpClient http)
{
    public async Task<HttpResponseMessage> ChatAsync(HttpContent body)
        => await http.PostAsync("/chat", body);
}
