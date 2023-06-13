using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using zbwebcore.API.Model;

namespace zbwebcore.API.Config
{

    public class MongoHelper
    {
        private static IMongoDatabase mongoContext;

        static MongoHelper()
        {
            GetMongoContext();
        }

        public static void GetMongoContext()
        {
            var client = new MongoClient(ConfigInfo.Info.MongoDBIp);
            if (client != null)
            {
                //通过数据库名获得上下文 名称可以配置到配置文件里
                mongoContext = client.GetDatabase(ConfigInfo.Info.MongoDBDbName);
            }
        }

        /// <summary>
        /// 上传文件（流）
        /// </summary>
        /// <param name="bucketName">相当于文件夹名</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fs">文件流</param>
        /// <returns></returns>
        public static ObjectId UploadFile(string bucketName, string fileName, Stream fs)
        {
            GridFSBucketOptions options = new GridFSBucketOptions();
            options.BucketName = bucketName;
            var bucket = new GridFSBucket(mongoContext, options);
            var oid = bucket.UploadFromStream(fileName, fs);
            return oid;
        }
        // 上传文件（字节）
        public static ObjectId UploadFileBytes(string bucketName, string fileName, byte[] _byte)
        {
            GridFSBucketOptions options = new GridFSBucketOptions();
            options.BucketName = bucketName;
            var bucket = new GridFSBucket(mongoContext, options);
            var oid = bucket.UploadFromBytes(fileName, _byte);
            return oid;
        }
        /// <summary>
        /// 获得文件字节数组
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public static Byte[] GetFileBytes(string bucketName, string fileId)
        {
            GridFSBucketOptions options = new GridFSBucketOptions();
            options.BucketName = bucketName;
            var bucket = new GridFSBucket(mongoContext, options);
            return bucket.DownloadAsBytes(new ObjectId(fileId));
        }
       
        /// <summary>
        /// 获得文件流
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public static Stream GetFileStream(string bucketName, string fileId)
        {
            MemoryStream stream = new MemoryStream();
            GridFSBucketOptions options = new GridFSBucketOptions();
            options.BucketName = bucketName;
            var bucket = new GridFSBucket(mongoContext, options);
            bucket.DownloadToStream(new ObjectId(fileId), stream);
            return stream;
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public static void DelFile(string bucketName, string fileId)
        {
            try
            {
                GridFSBucketOptions options = new GridFSBucketOptions();
                options.BucketName = bucketName;
                var bucket = new GridFSBucket(mongoContext, options);
                bucket.Delete(new ObjectId(fileId));
            }
            catch (Exception)
            {

            }
           
            
            
        }
    }
}
