using MvcMusicStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcMusicStore.Controllers
{
    /// <summary>
    /// 音乐页面类
    /// </summary>
    public class StoreController : Controller
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();
        // GET:存储
        public ActionResult Index()
        {
            var genres = storeDB.Genres.ToList();
            return View(genres);
        }
        // GET:存储/浏览
        public ActionResult Browse(string genre)
        {
            // 从数据库中检索类型及其关联相册
            var genreModel = storeDB.Genres.Include("Albums")
                .Single(g => g.Name == genre);

            return View(genreModel);
        }
        //为下一次更改要读取并显示输入的参数的详细信息操作名为 id
        // GET:存储/详细信息
        public ActionResult Details(int id)
        {
            var album = storeDB.Albums.Find(id);

            return View(album);
        }
    }
}