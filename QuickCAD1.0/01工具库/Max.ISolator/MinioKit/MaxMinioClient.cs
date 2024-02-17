﻿using Max.BaseKit;
using Minio;
using Minio.DataModel;
using Minio.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Max.ISolator.MinioKit
{
    /// <summary>
    /// Minio文件服务器客户端
    /// </summary>
    public class MaxMinioClient
    {
        public MimioClientMod Mod { get; private set; }
        public MinioClient minioClient { get; private set; }
        public MaxMinioClient(MimioClientMod mod)
        {
            this.Mod = mod;
            try
            {
                #region 样例
                //MinioClient minioClient = new MinioClient("play.min.io",accessKey: "Q3AM3UQ867SPQQA43P2F",secretKey: "zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG").WithSSL();
                //MinioClient s3Client = new MinioClient("s3.amazonaws.com:80",accessKey: "YOUR-ACCESSKEYID",secretKey: "YOUR-SECRETACCESSKEY").WithSSL();
                #endregion
                //minioClient = new MinioClient(endpoint, accessKey: accessKey, secretKey: secretKey).WithSSL();
                minioClient = new MinioClient(mod.Endpoint, accessKey: mod.AccessKey, secretKey: mod.SecretKey);
            }
            catch (MinioException ex)
            {
                NLogger.Warn($"创建MinioClient异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 通过Stream上传对象
        /// 单个对象的最大大小限制在5TB
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <param name="data">要上传的Stream对象</param>
        /// <returns></returns>
        public async Task PutObjectAsync(string bucketName, string objectName, Stream data)
        {
            try
            {
                if (string.IsNullOrEmpty(objectName)) return;
                string contentType = "application/octet-stream";//文件的Content type，默认是"application/octet-stream"
                await minioClient?.PutObjectAsync(bucketName, objectName, data, data.Length, contentType: contentType, metaData: null);
            }
            catch (MinioException ex)
            {
                NLogger.Error($"通过Stream上传对象：{ex.Message}");
            }
        }

        /// <summary>
        /// 创建一个存储桶
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="location">对象存储的region,可选参数。默认是us-east-1</param>
        /// <returns></returns>
        public async Task MakeBucketAsync(string bucketName, string location = "us-east-1")
        {
            try
            {
                bool found = await minioClient?.BucketExistsAsync(bucketName);//检查桶是否存在
                if (found) return;
                await minioClient?.MakeBucketAsync(bucketName, location);
            }
            catch (MinioException ex)
            {
                NLogger.Error($"创建桶{bucketName}异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 列出所有的存储桶
        /// </summary>
        /// <returns></returns>
        public async Task<ListAllMyBucketsResult> ListBucketsAsync()
        {
            try
            {
                return await minioClient?.ListBucketsAsync();
            }
            catch (MinioException ex)
            {
                NLogger.Error($"列出所有的存储桶异常：{ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// 检查存储桶是否存在
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <returns></returns>
        public async Task<bool> BucketExistsAsync(string bucketName)
        {
            try
            {
                return await minioClient?.BucketExistsAsync(bucketName);
            }
            catch (MinioException ex)
            {
                NLogger.Error($"检查存储桶是否存在：{ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// 删除一个存储桶
        /// 注意：- removeBucket不会删除存储桶中的对象，你需要调用removeObject API清空存储桶内的对象
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <returns></returns>
        public async Task RemoveBucketAsync(string bucketName)
        {
            try
            {
                bool found = await minioClient?.BucketExistsAsync(bucketName);//检查桶是否存在
                if (!found)
                {
                    NLogger.Warn($"{bucketName}存储桶不存在");
                    return;
                }
                await minioClient?.RemoveBucketAsync(bucketName);
            }
            catch (MinioException ex)
            {
                NLogger.Error($"删除一个{bucketName}存储桶异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 列出存储桶里的对象
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="prefix">对象的前缀</param>
        /// <param name="recursive">rue代表递归查找，false代表类似文件夹查找，以'/'分隔，不查子文件夹</param>
        /// <returns></returns>
        public async Task<IObservable<Item>> ListObjectsAsync(string bucketName, string prefix = null, bool recursive = true)
        {
            try
            {
                bool found = await minioClient?.BucketExistsAsync(bucketName);//检查桶是否存在
                if (!found)
                {
                    NLogger.Warn($"{bucketName}存储桶不存在");
                    return default;
                }
                IObservable<Item> observable = minioClient?.ListObjectsAsync(bucketName, prefix, true);
                //IDisposable subscription = observable.Subscribe(
                //        item => Console.WriteLine("OnNext: {0}", item.Key),
                //        ex => Console.WriteLine("OnError: {0}", ex.Message),
                //        () => Console.WriteLine("OnComplete: {0}"));
                return observable;
            }
            catch (MinioException ex)
            {
                NLogger.Error($"列出存储桶里的对象异常：{ex.Message}");
                return default;
            }
        }


        /// <summary>
        /// 列出存储桶中未完整上传的对象
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="prefix">对象的前缀</param>
        /// <param name="recursive">true代表递归查找，false代表类似文件夹查找，以'/'分隔，不查子文件夹</param>
        /// <returns></returns>
        public async Task<IObservable<Upload>> ListIncompleteUploads(string bucketName, string prefix, bool recursive)
        {
            try
            {
                bool found = await minioClient?.BucketExistsAsync(bucketName);
                if (!found)
                {
                    NLogger.Warn($"{bucketName}存储桶不存在");
                    return default;
                }

                IObservable<Upload> observable = minioClient?.ListIncompleteUploads(bucketName, prefix, recursive);
                //IDisposable subscription = observable.Subscribe(
                //                    item => Console.WriteLine("OnNext: {0}", item.Key),
                //                    ex => Console.WriteLine("OnError: {0}", ex.Message),
                //                    () => Console.WriteLine("OnComplete: {0}"));
                return observable;
            }
            catch (MinioException ex)
            {
                NLogger.Error($"列出存储桶中未完整上传的对象异常：{ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// 通过文件上传到对象中
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <param name="filePath">要上传的本地文件名</param>
        /// <param name="contentType">文件的Content type，默认是"application/octet-stream"</param>
        /// <param name="metaData">元数据头信息的Dictionary对象，默认是null</param>
        /// <returns></returns>
        public async Task PutObjectAsync(string bucketName, string objectName, string filePath, string contentType = "application/octet-stream", Dictionary<string, string> metaData = null)
        {
            try
            {
                if (string.IsNullOrEmpty(objectName)) return;
                await minioClient?.PutObjectAsync(bucketName, objectName, filePath, contentType: contentType, metaData: metaData);
            }
            catch (MinioException ex)
            {
                NLogger.Error($"通过Stream上传对象：{ex.Message}");
            }
        }

        /// <summary>
        /// 下载对象数据的流
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <param name="callback">处理流的回调函数</param>
        /// <returns></returns>
        public async Task GetObjectAsync(string bucketName, string objectName, Action<Stream> callback)
        {
            try
            {
                await minioClient?.GetObjectAsync(bucketName, objectName, callback);
            }
            catch (MinioException ex)
            {
                NLogger.Error($"下载对象数据的流异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 下载对象指定区域的字节数组(流)
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <param name="offset">起始字节的位置</param>
        /// <param name="length">要读取的长度</param>
        /// <param name="callback">处理流的回调函数</param>
        /// <returns></returns>
        public async Task GetObjectAsync(string bucketName, string objectName, long offset, long length, Action<Stream> callback)
        {
            try
            {
                await minioClient?.GetObjectAsync(bucketName, objectName, offset, length, callback);
            }
            catch (MinioException ex)
            {
                NLogger.Error($"下载对象指定区域的字节数组(流)异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 下载并将文件保存到本地
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <param name="fileName">本地文件路径(photo.jpg)</param>
        /// <returns></returns>
        public async Task GetObjectAsync(string bucketName, string objectName, string fileName)
        {
            try
            {
                await minioClient?.GetObjectAsync(bucketName, objectName, fileName);
            }
            catch (MinioException ex)
            {
                NLogger.Error($"下载并将文件保存到本地异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取对象的元数据
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <returns></returns>
        public async Task<ObjectStat> StatObjectAsync(string bucketName, string objectName)
        {
            try
            {
                return await minioClient?.StatObjectAsync(bucketName, objectName);
            }
            catch (MinioException ex)
            {
                NLogger.Error($"获取对象的元数据异常：{ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// 从objectName指定的对象中将数据拷贝到destObjectName指定的对象
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">源存储桶中的源对象名称("island.jpg")</param>
        /// <param name="destBucketName">目标存储桶名称</param>
        /// <param name="destObjectName">要创建的目标对象名称,如果为空，默认为源对象名称("processed.png")</param>
        /// <param name="copyConditions">拷贝操作的一些条件Map</param>
        /// <returns></returns>
        public async Task CopyObjectAsync(string bucketName, string objectName, string destBucketName, string destObjectName = null, CopyConditions copyConditions = null)
        {
            try
            {
                //CopyConditions copyConditions = new CopyConditions();
                //copyConditions.setMatchETagNone("TestETag");

                await minioClient?.CopyObjectAsync(bucketName, objectName, destBucketName, destObjectName, copyConditions);
            }
            catch (MinioException ex)
            {
                NLogger.Error($"从objectName指定的对象中将数据拷贝到destObjectName指定的对象异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 删除一个对象
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <returns></returns>
        public async Task RemoveObjectAsync(string bucketName, string objectName)
        {
            try
            {
                await minioClient?.RemoveObjectAsync(bucketName, objectName);
            }
            catch (MinioException ex)
            {
                NLogger.Error($"删除一个对象异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 删除多个对象
        /// </summary>
        /// <param name="bucketName">存储桶里的对象名称</param>
        /// <param name="objectsList">含有多个对象名称的IEnumerable</param>
        /// <returns></returns>
        public async Task<IObservable<DeleteError>> RemoveObjectAsync(string bucketName, IEnumerable<string> objectsList)
        {
            try
            {
                IObservable<DeleteError> observable = await minioClient?.RemoveObjectAsync(bucketName, objectsList);
                //IDisposable subscription = observable.Subscribe(
                //    deleteError => Console.WriteLine("Object: {0}", deleteError.Key),
                //    ex => Console.WriteLine("OnError: {0}", ex),
                //    () =>
                //    {
                //        Console.WriteLine("Listed all delete errors for remove objects on  " + bucketName + "\n");
                //    });
                return observable;
            }
            catch (MinioException ex)
            {
                NLogger.Error($"删除多个对象异常：{ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// 删除一个未完整上传的对象
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <returns></returns>
        public async Task RemoveIncompleteUploadAsync(string bucketName, string objectName)
        {
            try
            {
                await minioClient?.RemoveIncompleteUploadAsync(bucketName, objectName);
            }
            catch (MinioException ex)
            {
                NLogger.Error($"删除一个未完整上传的对象异常：{ex.Message}");
            }
        }
    }
}