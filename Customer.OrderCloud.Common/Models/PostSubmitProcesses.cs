using Flurl.Http;
using Newtonsoft.Json;
using OrderCloud.Catalyst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.OrderCloud.Common.Models
{
	public class PostSubmitProcesses
	{
		public List<PostSubmitProcessResult> Results { get; }
        public bool AnyErrors => Results.Any(r => !r.Success);


        public async Task<T> Run<T>(string description, Task<T> func) 
		{
            try
            {
                var result = await func;
                Results.Add(new PostSubmitProcessResult()
                {
                    Description = description,
                    Success = true
                });
                return result;
            }
            catch (CatalystBaseException integrationEx)
            {
                Results.Add(new PostSubmitProcessResult()
                {
                    Description = description,
                    Success = false,
                    ErrorDetails = new PostSubmitProcessError(integrationEx)
                });
                return default;
            }
            catch (FlurlHttpException flurlEx)
            {
                Results.Add(new PostSubmitProcessResult()
                {
                    Description = description,
                    Success = false,
                    ErrorDetails = new PostSubmitProcessError(flurlEx)
                });
                return default;
            }
            catch (Exception ex)
            {
                Results.Add(new PostSubmitProcessResult()
                {
                    Description = description,
                    Success = false,
                    ErrorDetails = new PostSubmitProcessError(ex)
                });
                return default;
            }
        }
    }

	public class PostSubmitProcessResult
    {
		public string Description { get; set; }
		public bool Success { get; set; }
		public PostSubmitProcessError ErrorDetails { get; set; }
	}

    public class PostSubmitProcessError
    {
        public string Message { get; set; }
        public dynamic ResponseBody { get; set; }

        public PostSubmitProcessError(Exception ex)
        {
            Message = ex.Message;
            ResponseBody = "";
        }

        public PostSubmitProcessError(CatalystBaseException ex)
        {
            Message = ex.Errors[0].Message;
            try
            {
                ResponseBody = JsonConvert.SerializeObject(ex.Errors);
            }
            catch (Exception)
            {
                ResponseBody = "Error while trying to parse response body";
            }
        }

        public PostSubmitProcessError(FlurlHttpException ex)
        {
            Message = ex.Message;
            try
            {
                ResponseBody = ex.GetResponseJsonAsync().Result;
            }
            catch (Exception)
            {
                ResponseBody = "Error while trying to parse response body";
            }

        }
    }
}
