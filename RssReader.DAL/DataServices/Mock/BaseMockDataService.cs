using Newtonsoft.Json;
using RssReader.DAL.Helpers;
using System;
using System.Threading.Tasks;

namespace RssReader.DAL.DataServices.Mock
{
    public class BaseMockDataService
    {
		protected async Task<RequestResult<T>> GetMockData<T>(string fileName) where T : class
		{
			try
			{
				var data = JsonConvert.DeserializeObject<T>(DataTools.GetFileContent(fileName));
				return new RequestResult<T>(data, RequestStatus.Ok);
			}
			catch (Exception e)
			{
				return new RequestResult<T>(default(T), RequestStatus.InternalServerError, e.Message);
			}
		}
	}
}
