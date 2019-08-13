namespace Utilities.REST {

	public interface IJsonHttpClientSerializer
	{
		string Serialize(object content);

		TType Deserialize<TType>(string body);
	}

}