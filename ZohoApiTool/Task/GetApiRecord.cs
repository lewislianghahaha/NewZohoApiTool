using System;
using RestSharp;

namespace ZohoApiTool.Task
{
    //与ZOHO API交互操作
    public class GetApiRecord
    {
        //api所需organizationid
        private string _organizationid = "678315528";

        /// <summary>
        /// 获取access_Token（刷新令牌）;在每次执行API获取数据时使用
        /// </summary>
        /// <returns></returns>
        public string GetAccessToken()
        {
            var result = "";
            var serviceUrl = "";

            var request = new RestRequest();

            try
            {
                LogHelper.WriteLog("获取刷新令牌");

                //todo:设置API 刷新令牌URL 地址
                serviceUrl = $@"https://accounts.zoho.com/oauth/v2/token?refresh_token=1000.de999e5653faa9642fbce6501a337516.bb785e602be2b63616c88eb0d3ce2d19
                                &client_id=1000.VCUSUC900CXB0UEOT18NV3XS1L3EQT
                                &client_secret=fa638b638a7ff82e42bdfb4a9b5271a4a816b8f027
                                &redirect_uri=http://www.zoho.com/Books&grant_type=refresh_token";


                //todo:执行调用API获取Access_Token(刷新令牌)
                var client = new RestClient(serviceUrl);
                //设置请求方式
                request.Method = Method.Post;
                //request.AddHeader("","");

                //执行调用API
                var response = client.Execute(request);

                if (Convert.ToString(response.StatusCode) != "OK") throw new Exception("返回刷新令牌出现异常");
                else
                {
                    //获取API返回JSON结果
                    var a = response.Content;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog("获取Access_Token令牌异常,原因:",ex);
            }
            return result;
        }



        /// <summary>
        /// 
        /// </summary>
        private void GetApiJsonRecord()
        {
            
        }

    }
}
