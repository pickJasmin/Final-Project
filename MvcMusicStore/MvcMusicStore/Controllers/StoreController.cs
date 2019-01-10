using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcMusicStore.Models;

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
            var genres = new List<Genre>
    {
        new Genre { Name = "迪斯科"},
        new Genre { Name = "爵士乐"},
        new Genre { Name = "摇滚乐"}
    };
            return View(genres);
        }
        // GET:存储/浏览
        public ActionResult Browse(string genre)
        {
            var genreModel = new Genre { Name = genre };
            return View(genreModel);
        }
        //为下一次更改要读取并显示输入的参数的详细信息操作名为 id
        // GET:存储/详细信息
        public ActionResult Details(int id)
        {
            var album = new Album { Title = "Album " + id };
            return View(album);
        }
    }
}