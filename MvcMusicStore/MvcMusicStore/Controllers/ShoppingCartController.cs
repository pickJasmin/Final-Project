using System.Linq;
using System.Web.Mvc;
using MvcMusicStore.Models;
using MvcMusicStore.ViewModels;

namespace MvcMusicStore.Controllers
{
    /// <summary>
    /// 购物车
    /// </summary>
    public class ShoppingCartController : Controller
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();
        //
        // GET: /ShoppingCart
        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            //设置我们的视图模型
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            //返回视图
            return View(viewModel);
        }
        //
        // GET: /Store/添加到购物车/5
        public ActionResult AddToCart(int id)
        {
            //从数据库中检索相册
            var addedAlbum = storeDB.Albums
                .Single(album => album.AlbumId == id);

            //将其添加到购物车中
            var cart = ShoppingCart.GetCart(this.HttpContext);

            cart.AddToCart(addedAlbum);

            //回到主商店页面进行更多的购物
            return RedirectToAction("Index");
        }
        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            // 将物品从购物车中移除
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // 获取要显示确认的相册名称
            string albumName = storeDB.Carts
                .Single(item => item.RecordId == id).Album.Title;

            // 从购物车删除
            int itemCount = cart.RemoveFromCart(id);

            //显示确认消息
            var results = new ShoppingCartRemoveViewModel
            {
                Message = Server.HtmlEncode(albumName) +
                    " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };
            return Json(results);
        }
        // GET: /ShoppingCart/CartSummary
        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            ViewData["CartCount"] = cart.GetCount();
            return PartialView("CartSummary");
        }
    }
}