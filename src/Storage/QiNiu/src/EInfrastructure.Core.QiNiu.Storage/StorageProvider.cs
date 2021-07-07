// Copyright (c) zhenlei520 All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EInfrastructure.Core.Configuration.Enumerations;
using EInfrastructure.Core.Configuration.Ioc.Plugs.Storage;
using EInfrastructure.Core.Configuration.Ioc.Plugs.Storage.Config;
using EInfrastructure.Core.Configuration.Ioc.Plugs.Storage.Dto;
using EInfrastructure.Core.Configuration.Ioc.Plugs.Storage.Dto.Storage;
using EInfrastructure.Core.Configuration.Ioc.Plugs.Storage.Params.Storage;
using EInfrastructure.Core.QiNiu.Storage.Config;
using EInfrastructure.Core.QiNiu.Storage.Validator.Storage;
using EInfrastructure.Core.Tools;
using EInfrastructure.Core.Validation.Common;
using Qiniu.Http;
using Qiniu.Storage;

namespace EInfrastructure.Core.QiNiu.Storage
{
    /// <summary>
    /// 文件实现类
    /// </summary>
    public class StorageProvider : BaseStorageProvider, IStorageProvider
    {
        /// <summary>
        /// 文件实现类
        /// </summary>
        public StorageProvider(QiNiuStorageConfig qiNiuConfig = null) : base(qiNiuConfig)
        {
        }

        #region 得到实现类唯一标示

        /// <summary>
        /// 得到实现类唯一标示
        /// </summary>
        /// <returns></returns>
        public string GetIdentify()
        {
            MethodBase method = MethodBase.GetCurrentMethod();
            return method.ReflectedType.Namespace;
        }

        #endregion

        #region 根据文件流上传

        /// <summary>
        /// 根据文件流上传
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isResume">是否允许续传（续传采用非表单提交方式）</param>
        /// <returns></returns>
        public UploadResultDto UploadStream(UploadByStreamParam param, bool isResume = false)
        {
            var uploadPersistentOps = GetUploadPersistentOps(param.UploadPersistentOps);
            string token = GetUploadCredentials(QiNiuConfig,
                new UploadPersistentOpsParam(param.Key, uploadPersistentOps));
            if (isResume)
            {
                ResumableUploader target =
                    new ResumableUploader(Core.Tools.GetConfig(this.QiNiuConfig, uploadPersistentOps));
                HttpResult result =
                    target.UploadStream(param.Stream, param.Key, token, GetPutExtra(uploadPersistentOps));
                bool res = result.Code == (int) HttpCode.OK;
                return new UploadResultDto(res, res ? "成功" : result.ToString());
            }
            else
            {
                FormUploader target = new FormUploader(Core.Tools.GetConfig(this.QiNiuConfig, uploadPersistentOps));
                HttpResult result =
                    target.UploadStream(param.Stream, param.Key, token, GetPutExtra(uploadPersistentOps));
                bool res = result.Code == (int) HttpCode.OK;
                return new UploadResultDto(res, res ? "成功" : result.ToString());
            }
        }

        #endregion

        #region 根据文件字节数组上传

        /// <summary>
        /// 根据文件字节数组上传
        /// </summary>
        /// <param name="param">文件流上传配置</param>
        /// <param name="isResume">是否允许续传（续传采用非表单提交方式）</param>
        /// <returns></returns>
        public UploadResultDto UploadByteArray(UploadByByteArrayParam param, bool isResume = false)
        {
            var uploadPersistentOps = GetUploadPersistentOps(param.UploadPersistentOps);
            string token = GetUploadCredentials(QiNiuConfig,
                new UploadPersistentOpsParam(param.Key, uploadPersistentOps));
            if (isResume)
            {
                ResumableUploader target =
                    new ResumableUploader(Core.Tools.GetConfig(this.QiNiuConfig, uploadPersistentOps));
                HttpResult result =
                    target.UploadStream(param.ByteArray.ConvertToStream(), param.Key, token,
                        GetPutExtra(uploadPersistentOps));
                bool res = result.Code == (int) HttpCode.OK;
                return new UploadResultDto(res, res ? "成功" : result.ToString());
            }
            else
            {
                FormUploader target = new FormUploader(Core.Tools.GetConfig(this.QiNiuConfig, uploadPersistentOps));
                HttpResult result =
                    target.UploadData(param.ByteArray, param.Key, token, GetPutExtra(uploadPersistentOps));
                bool res = result.Code == (int) HttpCode.OK;
                return new UploadResultDto(res, res ? "成功" : result.ToString());
            }
        }

        #endregion

        #region 根据文件token上传

        /// <summary>
        /// 根据文件流以及文件字节数组上传
        /// </summary>
        /// <param name="param">文件流上传配置</param>
        /// <returns></returns>
        public UploadResultDto UploadByToken(UploadByTokenParam param)
        {
            var uploadPersistentOps = GetUploadPersistentOps(param.UploadPersistentOps);
            FormUploader target = new FormUploader(Core.Tools.GetConfig(this.QiNiuConfig, uploadPersistentOps));
            HttpResult result;
            if (param.Stream != null)
            {
                result =
                    target.UploadStream(param.Stream, param.Key, param.Token, GetPutExtra(uploadPersistentOps));

                bool res = result.Code == (int) HttpCode.OK;
                return new UploadResultDto(res, res ? "成功" : result.ToString());
            }

            if (param.ByteArray != null)
            {
                result =
                    target.UploadData(param.ByteArray, param.Key, param.Token, GetPutExtra(uploadPersistentOps));

                bool res = result.Code == (int) HttpCode.OK;
                return new UploadResultDto(res, res ? "成功" : result.ToString());
            }

            return new UploadResultDto(false, "不支持的上传方式");
        }

        #endregion

        #region 得到凭证

        #region 得到上传文件凭证

        /// <summary>
        /// 得到上传文件凭证
        /// </summary>
        /// <param name="opsParam">上传信息</param>
        public string GetUploadCredentials(UploadPersistentOpsParam opsParam)
        {
            new UploadPersistentOpsParamValidator().Validate(opsParam).Check(HttpStatus.Err.Name);
            var uploadPersistentOps = GetUploadPersistentOps(opsParam.UploadPersistentOps);
            return base.GetUploadCredentials(QiNiuConfig,
                new UploadPersistentOpsParam(opsParam.Key, uploadPersistentOps));
        }

        #endregion

        #region 得到管理凭证

        /// <summary>
        /// 得到管理凭证
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string GetManageToken(GetManageTokenParam request)
        {
            new GetManageTokenParamValidator().Validate(request).Check(HttpStatus.Err.Name);
            return GetAuth().CreateManageToken(request.Url);
        }

        #endregion

        #region 得到下载凭证

        /// <summary>
        /// 得到下载凭证
        /// </summary>
        /// <param name="url">url地址</param>
        /// <returns></returns>
        public string GetDownloadToken(string url)
        {
            return base.GetAuth().CreateDownloadToken(url);
        }

        #endregion

        #endregion

        #region 检查文件是否存在

        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OperateResultDto Exist(ExistParam request)
        {
            new ExistParamValidator().Validate(request).Check(HttpStatus.Err.Name);
            var res = Get(new GetFileParam(request.Key, request.PersistentOps));
            return new OperateResultDto(res.Success, res.Msg);
        }

        #endregion

        #region 获取指定前缀的文件列表

        /// <summary>
        /// 获取指定前缀的文件列表
        /// </summary>
        /// <param name="filter">筛选</param>
        /// <returns></returns>
        public ListFileItemResultDto ListFiles(ListFileFilter filter)
        {
            new ListFileFilterValidator().Validate(filter).Check(HttpStatus.Err.Name);
            var listRet = base.GetBucketManager().ListFiles(
                Core.Tools.GetBucket(this.QiNiuConfig, filter.PersistentOps.Bucket), filter.Prefix, filter.LastMark,
                filter.PageSize,
                filter.Delimiter);
            if (listRet.Code == (int) HttpCode.OK)
            {
                return new ListFileItemResultDto(true, "success")
                {
                    CommonPrefixes = listRet.Result.CommonPrefixes,
                    Marker = listRet.Result.Marker,
                    Items = listRet.Result.Items.Select(x => new FileInfoDto()
                    {
                        Host = Core.Tools.GetHost(this.QiNiuConfig, filter.PersistentOps.Host),
                        Path = x.Key,
                        Msg = "success",
                        Hash = x.Hash,
                        Size = x.Fsize,
                        PutTime = x.PutTime,
                        MimeType = x.MimeType,
                        FileType = x.FileType,
                        Success = true,
                    }).ToList()
                };
            }

            return new ListFileItemResultDto(false, "lose");
        }

        #endregion

        #region 获取文件信息

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FileInfoDto Get(GetFileParam request)
        {
            new GetFileParamValidator().Validate(request).Check(HttpStatus.Err.Name);
            StatResult statRet = GetBucketManager()
                .Stat(Core.Tools.GetBucket(this.QiNiuConfig, request.PersistentOps.Bucket), request.Key);
            if (statRet.Code != (int) HttpCode.OK)
            {
                return new FileInfoDto()
                {
                    Success = false,
                    Msg = statRet.ToString()
                };
            }

            return new FileInfoDto()
            {
                Size = statRet.Result.Fsize,
                Hash = statRet.Result.Hash,
                MimeType = statRet.Result.MimeType,
                PutTime = statRet.Result.PutTime,
                FileType = statRet.Result.FileType,
                Success = true,
                Host = Core.Tools.GetHost(this.QiNiuConfig, request.PersistentOps.Host),
                Path = request.Key,
                Msg = "success"
            };
        }

        /// <summary>
        /// 获取文件信息集合
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<FileInfoDto> GetList(GetFileRangeParam request)
        {
            new GetFileRangeParamValidator().Validate(request).Check(HttpStatus.Err.Name);
            List<FileInfoDto> res = new List<FileInfoDto>();
            request.Keys.ToList()
                .ListPager((list) => { res.AddRange(GetMulti(list.ToArray(), request.PersistentOps)); }, 1000, 1);
            return res;
        }

        /// <summary>
        /// 获取文件信息集合
        /// </summary>
        /// <param name="keyList">文件key集合</param>
        /// <param name="persistentOps">策略</param>
        /// <returns></returns>
        private IEnumerable<FileInfoDto> GetMulti(string[] keyList, BasePersistentOps persistentOps)
        {
            List<string> ops = keyList.Select(key =>
                GetBucketManager().StatOp(Core.Tools.GetBucket(this.QiNiuConfig, persistentOps.Bucket), key)).ToList();
            BatchResult ret = GetBucketManager().Batch(ops);

            var index = 0;
            foreach (var item in ret.Result)
            {
                index++;
                if (item.Code == (int) HttpCode.OK)
                {
                    yield return new FileInfoDto()
                    {
                        Size = item.Data.Fsize,
                        Hash = item.Data.Hash,
                        MimeType = item.Data.MimeType,
                        PutTime = item.Data.PutTime,
                        FileType = item.Data.FileType,
                        Success = true,
                        Host = Core.Tools.GetHost(this.QiNiuConfig, persistentOps.Host),
                        Path = keyList[index - 1],
                        Msg = "success"
                    };
                }
                else
                {
                    yield return new FileInfoDto()
                    {
                        Success = false,
                        Msg = item.Data.Error,
                        Host = Core.Tools.GetHost(this.QiNiuConfig, persistentOps.Host),
                        Path = keyList[index - 1]
                    };
                }
            }
        }

        #endregion

        #region 删除文件

        /// <summary>
        /// 根据文件key删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DeleteResultDto Remove(RemoveParam request)
        {
            new RemoveParamValidator().Validate(request).Check(HttpStatus.Err.Name);
            HttpResult deleteRet = GetBucketManager()
                .Delete(Core.Tools.GetBucket(this.QiNiuConfig, request.PersistentOps.Bucket), request.Key);
            var res = deleteRet.Code == (int) HttpCode.OK;
            return new DeleteResultDto(res, request.Key, res ? "删除成功" : deleteRet.ToString());
        }

        /// <summary>
        /// 根据文件key集合删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<DeleteResultDto> RemoveRange(RemoveRangeParam request)
        {
            new RemoveRangeParamValidator().Validate(request).Check(HttpStatus.Err.Name);
            List<DeleteResultDto> res = new List<DeleteResultDto>();
            request.Keys.ListPager((list) => { res.AddRange(DelMulti(list, request.PersistentOps)); }, 1000, 1);
            return res;
        }

        ///  <summary>
        /// 根据文件key集合删除
        ///  </summary>
        ///  <param name="keyList">文件key集合</param>
        ///  <param name="persistentOps">策略</param>
        ///  <returns></returns>
        private IEnumerable<DeleteResultDto> DelMulti(IEnumerable<string> keyList, BasePersistentOps persistentOps)
        {
            var enumerable = keyList as string[] ?? keyList.ToArray();
            List<string> ops = enumerable.Select(key =>
                    GetBucketManager().DeleteOp(Core.Tools.GetBucket(this.QiNiuConfig, persistentOps.Bucket), key))
                .ToList();
            BatchResult ret = GetBucketManager().Batch(ops);
            var index = 0;
            foreach (var item in ret.Result)
            {
                index++;
                if (item.Code == (int) HttpCode.OK)
                {
                    yield return new DeleteResultDto(true, enumerable.ToList()[index - 1], "删除成功");
                }
                else
                {
                    yield return new DeleteResultDto(false, enumerable.ToList()[index - 1], item.Data.Error);
                }
            }
        }

        #endregion

        #region 批量复制文件

        /// <summary>
        /// 复制文件（两个文件需要在同一账号下）
        /// </summary>
        /// <param name="copyFileParam">复制到新空间的参数</param>
        /// <returns></returns>
        public CopyFileResultDto CopyTo(CopyFileParam copyFileParam)
        {
            new CopyFileParamValidator().Validate(copyFileParam).Check(HttpStatus.Err.Name);
            HttpResult copyRet = GetBucketManager().Copy(
                Core.Tools.GetBucket(this.QiNiuConfig, copyFileParam.PersistentOps.Bucket), copyFileParam.SourceKey,
                copyFileParam.OptBucket, copyFileParam.OptKey, copyFileParam.IsForce);
            var res = copyRet.Code == (int) HttpCode.OK;
            return new CopyFileResultDto(res, copyFileParam.FileId, res ? "复制成功" : copyRet.Text);
        }

        /// <summary>
        /// 复制文件（两个文件需要在同一账号下）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<CopyFileResultDto> CopyRangeTo(CopyFileRangeParam request)
        {
            new CopyFileRangeParamValidator().Validate(request).Check(HttpStatus.Err.Name);
            List<CopyFileResultDto> res = new List<CopyFileResultDto>();
            request.CopyFiles.ToList()
                .ListPager(list => { res.AddRange(CopyToMulti(list, request.PersistentOps)); }, 1000, 1);
            return res;
        }

        ///  <summary>
        /// 复制到新空间的参数
        ///  </summary>
        ///  <param name="copyFileParam">复制到新空间的参数</param>
        ///  <param name="persistentOps">策略</param>
        ///  <returns></returns>
        private IEnumerable<CopyFileResultDto> CopyToMulti(ICollection<CopyFileRangeParam.CopyFileParam> copyFileParam,
            BasePersistentOps persistentOps)
        {
            List<string> ops = copyFileParam.Select(x =>
                GetBucketManager().CopyOp(Core.Tools.GetBucket(this.QiNiuConfig, persistentOps.Bucket), x.SourceKey,
                    x.OptBucket, x.OptKey, x.IsForce)).ToList();
            BatchResult ret = GetBucketManager().Batch(ops);
            var index = 0;
            foreach (BatchInfo info in ret.Result)
            {
                index++;
                if (info.Code == (int) HttpCode.OK)
                {
                    yield return new CopyFileResultDto(true, copyFileParam.ToList()[index - 1].FileId,
                        "复制成功");
                }
                else
                {
                    yield return new CopyFileResultDto(false, copyFileParam.ToList()[index - 1].FileId,
                        info.Data.Error);
                }
            }
        }

        #endregion

        #region 批量移动文件（两个文件需要在同一账号下）

        /// <summary>
        /// 移动文件（两个文件需要在同一账号下）
        /// </summary>
        /// <param name="moveFileParam"></param>
        /// <returns></returns>
        public MoveFileResultDto Move(MoveFileParam moveFileParam)
        {
            new MoveFileParamValidator().Validate(moveFileParam).Check(HttpStatus.Err.Name);
            HttpResult copyRet = GetBucketManager().Move(
                Core.Tools.GetBucket(this.QiNiuConfig, moveFileParam.PersistentOps.Bucket), moveFileParam.SourceKey,
                moveFileParam.OptBucket, moveFileParam.OptKey, moveFileParam.IsForce);
            var res = copyRet.Code == (int) HttpCode.OK;
            return new MoveFileResultDto(res, moveFileParam.FileId, res ? "移动成功" : copyRet.Text);
        }

        /// <summary>
        /// 移动文件（两个文件需要在同一账号下）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<MoveFileResultDto> MoveRange(MoveFileRangeParam request)
        {
            new MoveFileParamRangeValidator().Validate(request).Check(HttpStatus.Err.Name);
            List<MoveFileResultDto> res = new List<MoveFileResultDto>();
            request.MoveFiles.ToList()
                .ListPager(list => { res.AddRange(MoveMulti(list, request.PersistentOps)); }, 1000, 1);
            return res;
        }

        /// <summary>
        /// 移动文件（两个文件需要在同一账号下）
        /// </summary>
        /// <param name="moveFileParamList"></param>
        /// <param name="persistentOps">策略</param>
        /// <returns></returns>
        private IEnumerable<MoveFileResultDto> MoveMulti(List<MoveFileRangeParam.MoveFileParam> moveFileParamList,
            BasePersistentOps persistentOps)
        {
            var bucketManager = GetBucketManager(persistentOps);
            List<string> ops = moveFileParamList.Select(x =>
                bucketManager.MoveOp(Core.Tools.GetBucket(this.QiNiuConfig, persistentOps.Bucket), x.SourceKey,
                    x.OptBucket, x.OptKey, x.IsForce)).ToList();
            BatchResult ret = bucketManager.Batch(ops);
            var index = 0;
            foreach (BatchInfo info in ret.Result)
            {
                index++;
                if (info.Code == (int) HttpCode.OK)
                {
                    yield return new MoveFileResultDto(true, moveFileParamList.ToList()[index - 1].FileId,
                        "复制成功");
                }
                else
                {
                    yield return new MoveFileResultDto(false, moveFileParamList.ToList()[index - 1].FileId,
                        info.Data.Error);
                }
            }
        }

        #endregion

        #region 得到地址

        /// <summary>
        /// 得到公开空间的访问地址
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string GetPublishUrl(GetPublishUrlParam request)
        {
            new GetPublishUrlParamValidator().Validate(request).Check(HttpStatus.Err.Name);
            return DownloadManager.CreatePublishUrl(Core.Tools.GetHost(this.QiNiuConfig, request.PersistentOps.Host),
                request.Key);
        }

        /// <summary>
        /// 得到私有空间的地址
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string GetPrivateUrl(GetPrivateUrlParam request)
        {
            new GetPrivateUrlParamValidator().Validate(request).Check(HttpStatus.Err.Name);
            return DownloadManager.CreatePrivateUrl(this.QiNiuConfig.GetMac(),
                Core.Tools.GetHost(this.QiNiuConfig, request.PersistentOps.Host), request.Key, request.Expire);
        }

        #endregion

        #region 下载文件

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">文件访问地址(绝对地址，非文件key)</param>
        /// <param name="savePath">保存路径</param>
        /// <returns></returns>
        public DownloadResultDto Download(string url, string savePath)
        {
            var ret = DownloadManager.Download(url, savePath);
            var res = ret.Code == (int) HttpCode.OK;
            return new DownloadResultDto(res, ret.Text, ret);
        }

        #endregion

        #region 设置生存时间

        /// <summary>
        /// 设置生存时间（超时会自动删除）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ExpireResultDto SetExpire(SetExpireParam request)
        {
            new SetExpireParamValidator().Validate(request).Check(HttpStatus.Err.Name);
            var expireRet = base.GetBucketManager()
                .DeleteAfterDays(Core.Tools.GetBucket(this.QiNiuConfig, request.PersistentOps.Bucket), request.Key,
                    request.Expire);
            if (expireRet.Code != (int) HttpCode.OK)
            {
                return new ExpireResultDto(false, request.Key, "lose");
            }

            return new ExpireResultDto(true, request.Key, "success");
        }

        /// <summary>
        /// 批量设置生存时间（超时会自动删除）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<ExpireResultDto> SetExpireRange(SetExpireRangeParam request)
        {
            new SetExpireRangeParamValidator().Validate(request).Check(HttpStatus.Err.Name);
            List<ExpireResultDto> expireResult = new List<ExpireResultDto>();
            request.Keys.Distinct().ToList()
                .ListPager(
                    (list) =>
                    {
                        expireResult.AddRange(SetExpireMulti(list.ToArray(), request.Expire, request.PersistentOps));
                    },
                    1000,
                    1);
            return expireResult;
        }

        /// <summary>
        /// 设置生存时间
        /// </summary>
        /// <param name="keys">文件key集合</param>
        /// <param name="expire">过期时间 单位：day</param>
        /// <param name="persistentOps">策略</param>
        /// <returns></returns>
        private IEnumerable<ExpireResultDto> SetExpireMulti(string[] keys, int expire, BasePersistentOps persistentOps)
        {
            var bucketManager = base.GetBucketManager();
            List<string> ops = new List<string>();
            foreach (string key in keys)
            {
                string op = bucketManager.DeleteAfterDaysOp(
                    Core.Tools.GetBucket(this.QiNiuConfig, persistentOps.Bucket), key, expire);
                ops.Add(op);
            }

            BatchResult ret = bucketManager.Batch(ops);
            var index = 0;
            foreach (BatchInfo info in ret.Result)
            {
                index++;
                if (info.Code == (int) HttpCode.OK)
                {
                    yield return new ExpireResultDto(true, keys.ToList()[index - 1], "success");
                }
                else
                {
                    yield return new ExpireResultDto(false, keys.ToList()[index - 1], "lose");
                }
            }
        }

        #endregion

        #region 修改文件MimeType

        /// <summary>
        /// 修改文件MimeType
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ChangeMimeResultDto ChangeMime(ChangeMimeParam request)
        {
            new ChangeMimeParamValidator().Validate(request).Check(HttpStatus.Err.Name);
            var ret = base.GetBucketManager()
                .ChangeMime(Core.Tools.GetBucket(this.QiNiuConfig, request.PersistentOps.Bucket), request.Key,
                    request.MimeType);
            if (ret.Code != (int) HttpCode.OK)
            {
                return new ChangeMimeResultDto(false, request.Key, ret.Text);
            }

            return new ChangeMimeResultDto(true, request.Key, "success");
        }

        /// <summary>
        /// 批量更改文件mime
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<ChangeMimeResultDto> ChangeMimeRange(ChangeMimeRangeParam request)
        {
            new ChangeMimeRangeParamValidator().Validate(request).Check(HttpStatus.Err.Name);
            List<ChangeMimeResultDto> ret = new List<ChangeMimeResultDto>();
            request.Keys.Distinct().ToList()
                .ListPager(
                    (list) =>
                    {
                        ret.AddRange(ChangeMimeMulti(list.ToArray(), request.MimeType, request.PersistentOps));
                    }, 1000, 1);
            return ret;
        }

        /// <summary>
        /// 批量更改mimeType
        /// </summary>
        /// <param name="keys">文件key</param>
        /// <param name="mime">文件mime</param>
        /// <param name="persistentOps">策略</param>
        /// <returns></returns>
        private IEnumerable<ChangeMimeResultDto> ChangeMimeMulti(string[] keys, string mime,
            BasePersistentOps persistentOps)
        {
            var bucketManager = base.GetBucketManager();
            List<string> ops = new List<string>();
            foreach (string key in keys)
            {
                string op = bucketManager.ChangeMimeOp(Core.Tools.GetBucket(this.QiNiuConfig, persistentOps.Bucket),
                    key, mime);
                ops.Add(op);
            }

            BatchResult ret = bucketManager.Batch(ops);
            var index = 0;
            foreach (BatchInfo info in ret.Result)
            {
                index++;
                if (info.Code == (int) HttpCode.OK)
                {
                    yield return new ChangeMimeResultDto(true, keys.ToList()[index - 1], "success");
                }
                else
                {
                    yield return new ChangeMimeResultDto(false, keys.ToList()[index - 1], "lose");
                }
            }
        }

        #endregion

        #region 修改文件的存储类型

        /// <summary>
        /// 修改文件存储类型
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ChangeTypeResultDto ChangeType(ChangeTypeParam request)
        {
            new ChangeTypeParamValidator().Validate(request).Check(HttpStatus.Err.Name);
            HttpResult ret = base.GetBucketManager()
                .ChangeType(Core.Tools.GetBucket(this.QiNiuConfig, request.PersistentOps.Bucket), request.Key,
                    request.Type);
            if (ret.Code == (int) HttpCode.OK)
            {
                return new ChangeTypeResultDto(true, request.Key, "success");
            }

            return new ChangeTypeResultDto(false, request.Key, ret.Text);
        }

        /// <summary>
        /// 批量更改文件类型
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<ChangeTypeResultDto> ChangeTypeRange(ChangeTypeRangeParam request)
        {
            new ChangeTypeRangeParamValidator().Validate(request).Check(HttpStatus.Err.Name);
            List<ChangeTypeResultDto> ret = new List<ChangeTypeResultDto>();
            request.Keys.Distinct().ToList()
                .ListPager(
                    (list) => { ret.AddRange(ChangeTypeMulti(list.ToArray(), request.Type, request.PersistentOps)); },
                    1000, 1);
            return ret;
        }

        /// <summary>
        /// 批量更改文件Type
        /// </summary>
        /// <param name="keys">文件key</param>
        /// <param name="type">0表示普通存储，1表示低频存储</param>
        /// <param name="persistentOps">策略</param>
        /// <returns></returns>
        private IEnumerable<ChangeTypeResultDto> ChangeTypeMulti(string[] keys, int type,
            BasePersistentOps persistentOps)
        {
            var bucketManager = base.GetBucketManager();
            List<string> ops = new List<string>();
            foreach (string key in keys)
            {
                string op = bucketManager.ChangeTypeOp(Core.Tools.GetBucket(this.QiNiuConfig, persistentOps.Bucket),
                    key, type);
                ops.Add(op);
            }

            BatchResult ret = bucketManager.Batch(ops);
            var index = 0;
            foreach (BatchInfo info in ret.Result)
            {
                index++;
                if (info.Code == (int) HttpCode.OK)
                {
                    yield return new ChangeTypeResultDto(true, keys.ToList()[index - 1], "success");
                }
                else
                {
                    yield return new ChangeTypeResultDto(false, keys.ToList()[index - 1], "lose");
                }
            }
        }

        #endregion
    }
}
