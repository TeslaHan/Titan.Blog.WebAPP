﻿/************************************************************************
 * 文件名：MainServices
 * 文件功能描述：xx控制层
 * 作    者：  韩俊俊
 * 创建日期：2019/1/22 17:25:45
 * 修 改 人：
 * 修改日期：
 * 修改原因：
 * Copyright (c) 2017 . All Rights Reserved. 
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Titan.Blog.AppService.Base;
using Titan.Blog.IAppService;
using Titan.Blog.IAppService.Base;
using Titan.Blog.Infrastructure.Utility;
using Titan.Blog.IRepository;
using Titan.Blog.Model.DataModel;

namespace Titan.Blog.AppService
{
    public class MainServices:BaseServices<Main,Guid>,IMainServices
    {
        private readonly IMainRepository _iMainRepository;
        private readonly IChildrenRepository _iChildrenRepository;

        public MainServices(IMainRepository iMainRepository, IChildrenRepository iChildrenRepository)
        {
            base.BaseRepository = iMainRepository;//如果要用基类封装的方法必须传值
            _iMainRepository = iMainRepository;
            _iChildrenRepository = iChildrenRepository;
        }

        public async Task AddModel(Main model)
        {
            await Add(model);//直接使用基类中的方法
            //await _iMainRepository.Add(model);//使用仓储中的方法
        }

        public async Task<Tuple<List<Main>,int>> GetList()
        {
            //ef 跟踪查询
            Expression<Func<Children, bool>> where1 = x => x.Main.Telphone.Contains("11");
            Expression<Func<Children, int>> orderby1 = x => x.Id;
            var dt = await _iChildrenRepository.Query(where1, orderby1, true, 1, 10);
            //更新数据
            var put = dt.Item1.FirstOrDefault();
            put.Name = "非跟踪更新";
            await _iChildrenRepository.Update(put);

            Expression<Func<Main, bool>> where = x => x.Telphone.Contains("11");
            Expression<Func<Main, string>> orderby = x => x.Name;
            var data=await Query(where, orderby, true, 1, 10);
            return data;
        }

        public async Task<Tuple<List<Children>, int>> QueryAsNotraking()
        {
            string redisConfiguration = Appsettings.app(new string[] { "AppSettings", "RedisCaching", "ConnectionString" });//获取连接字符串
            //ef非跟踪查询
            Expression<Func<Children, bool>> where1 = x => x.Main.Telphone.Contains("11");
            Expression<Func<Children, string>> orderby1 = x => x.Name;
            Expression<Func<Children, string>> orderby2 = x => x.Id.ToString();
            var data= await _iChildrenRepository.AsNoTracking<string>(where1, orderby1, orderby2, true, 1, 10);
            //更新数据
            var put = data.Item1.FirstOrDefault();
            put.Name = "非跟踪更新";
            await _iChildrenRepository.Update(put);
            return data;
        }
    }
}
