using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeetleX.Buffers;
using BeetleX.FastHttpApi;
using BeetleX.FastHttpApi.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using zbwebcore.API.Config;
using zbwebcore.API.Model;

namespace zbwebcore.API.Controllers
{
    [Options(AllowOrigin = "*", AllowHeaders = "*,token")]
    [Controller(BaseUrl = "upload/")]
    //[SkipFilter(typeof(GlobalFilter))]
    public class UploadController
    {
        [Post]
        public object UploadFile(IHttpContext context)
        {
            string msg = "上传失败!", src = "";
            bool flag = false;
            try
            {
                var files = context.Request.Files[0];
                var bytes1 = ImageHelper.ImageScalingToRange(files.Data,files.FileName);
                var retid = MongoHelper.UploadFileBytes("UploadFile", files.FileName, bytes1);
                var bytesqr = ImageHelper.QRCoder("http://www.baidu.com");
                var retidqr = MongoHelper.UploadFileBytes("UploadFile", files.FileName, bytesqr);
                //var bytes2 = ImageHelper.ImageMaxCutByCenter(files.Data, files.FileName);
                //var bytes3 = ImageHelper.ImageScalingByOversized(files.Data, files.FileName);
                //var retid = MongoHelper.UploadFile("UploadFile", files.FileName, files.Data);
                
                //var retid2 = MongoHelper.UploadFileBytes("UploadFile", files.FileName, bytes2);
                //var retid3 = MongoHelper.UploadFileBytes("UploadFile", files.FileName, bytes3);
                if (!string.IsNullOrEmpty(retid.ToString()))
                {
                    src = "/upload/DownloadFile?id=" + retid.ToString();
                    flag = true;
                    msg = "上传成功!";
                }
                
            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg, src = src }); ;
        }
        [Get]
        //[SkipFilter(typeof(GlobalFilter))]
        public object DownloadFile(string id)
        {
            try
            {
                var bytes = MongoHelper.GetFileBytes("UploadFile", id);
                return new DownloadImg(bytes);

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        [SkipFilter(typeof(GlobalFilter))]
        [Get]
        public object DownloadFilePDF(string id)
        {
            try
            {
                var bytes = MongoHelper.GetFileBytes("UploadFile", id);
                //var stream = MongoHelper.GetFileStream("UploadFile", id); 
                //return stream;
                return new DownLoadPdf(bytes,"testpdf");

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public object downloadFileTest()
        {
            return new FileResult(@"/files/httpd.zip");
        }
        //上传excel并转换为json返回前端table
        [Post]
        public object UploadExcel(IHttpContext context)
        {
            string msg = "上传失败!";
            bool flag = false;
            DataTable dt = null;
            int count = 0;
            try
            {
                var files = context.Request.Files[0];
                dt = NpoiHelper.ExcelToDT(files.Data, files.FileName);
                if (dt!=null)
                {
                    flag = true;
                    msg = "上传成功!";
                    count = dt.Rows.Count;
                }

            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { code = flag ? 0 : 200, count = count,flag=flag, msg = msg, data = dt }); ;
        }
        //[SkipFilter(typeof(GlobalFilter))]
        //[Get]
       
    }
}