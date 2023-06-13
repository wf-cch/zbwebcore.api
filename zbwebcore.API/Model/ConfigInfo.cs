using BeetleX.FastHttpApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zbwebcore.API.BllDal;
using zbwebcore.API.Config;

namespace zbwebcore.API.Model
{
    public class ConfigInfo
    {
        public static ConfigInfo Info
        {
            get { return new ConfigInfo(); }
        }
        public string redisIp
        {
            get
            {
                return AppConfigurtaionServices.Configuration["redis:ip"];
            }
            
        }
        public int dbNum
        {
            get
            {
                return Convert.ToInt32(AppConfigurtaionServices.Configuration["redis:dbnum"]);
            }

        }
        public int timeout
        {
            get
            {
                return Convert.ToInt32(AppConfigurtaionServices.Configuration["redis:timeout"]); 
            }

        }
        public string DBConnstr
        {
            get
            {
                return AppConfigurtaionServices.Configuration["connectionString"];
            }

        }
        public string DBConnstrFd
        {
            get
            {
                return AppConfigurtaionServices.Configuration["connectionString_fd"];
            }

        }
        public string csredis
        {
            get
            {
                return AppConfigurtaionServices.Configuration["csredis"];
            }

        }
        public string MongoDBIp
        {
            get
            {
                return AppConfigurtaionServices.Configuration["MongoDB:ip"];
            }

        }
        public string MongoDBDbName
        {
            get
            {
                return AppConfigurtaionServices.Configuration["MongoDB:DbName"];
            }

        }
        public string SqlFilterWord
        {
            get
            {
                return AppConfigurtaionServices.Configuration["SqlFilterWord"];
            }
        }
        public class ImageSize
        {
            public string width { get; set; }
            public string height { get; set; }
            public string quality { get; set; }
        }
        public ImageSize imageSize
        {
            get
            {
                var image = new ImageSize();
                image.width= AppConfigurtaionServices.Configuration["ImageSize:width"];
                image.height = AppConfigurtaionServices.Configuration["ImageSize:height"];
                image.quality = AppConfigurtaionServices.Configuration["ImageSize:quality"];
                return image;
            }

        }

        //public RedisDB rdb
        //{
        //    get
        //    {
        //        return new RedisDB(ConfigInfo.Info.dbNum, new JsonFormater());
        //    }
        //}
        public UserInfo getUser(string token)
        {
            try
            {
                if (RedisHelper.Instance.Exists(token))
                {
                    var userinfo = RedisHelper.Instance.Get<UserInfo>(token);
                    return userinfo;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
       
       
    }
}
