using Castle.DynamicProxy;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;

namespace WcfRefactoring.NetFrameworkClient2
{
    public class HttpDotNetCoreInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var client = new RestClient(GetEndpointUrl());

            var request = new RestRequest(GetPath(invocation), DataFormat.Json);

            dynamic dynamicObject = new ExpandoObject();
            IDictionary<string, object> dic = dynamicObject;

            int i = 0;
            foreach(var parameters in invocation.Method.GetParameters())
            {
                request.AddParameter(parameters.Name, invocation.GetArgumentValue(i++));
            }

            var response = client.Post(request);

            if(invocation.Method.ReturnType != typeof(void))
            {
                invocation.ReturnValue = JsonConvert.DeserializeObject(response.Content, invocation.Method.ReturnType);
            }

        }

        private string GetPath(IInvocation invocation)
        {
            Type proxType = invocation.Method.DeclaringType;

            if(proxType.IsInterface && proxType.Name.StartsWith("I"))
                return proxType.Name.Substring(1).ToLower();
            
            return proxType.Name.ToLower();
        }

        private static string GetEndpointUrl()
        {
            return ConfigurationManager.AppSettings["dotnet-core-endpoint"] ?? 
                throw new InvalidOperationException("The configuration dotnet-core-endpoint is not set on App.Config/Web.Config");
        }
    }
}