using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcMusicStore.Models
{
    /// <summary>
    /// 购物车类
    /// </summary>
    public partial class ShoppingCart
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";
        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }
        //帮助器方法来简化购物车调用
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }
        /// <summary>
        /// AddToCart采用唱片集作为参数并将其添加到用户的购物车。
        /// </summary>
        /// <param name="album"></param>
        public void AddToCart(Album album)
        {
            // 获取匹配的购物车和相册实例
            var cartItem = storeDB.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.AlbumId == album.AlbumId);

            if (cartItem == null)
            {
                //如果不存在购物车项，则创建一个新的购物车项
                cartItem = new Cart
                {
                    AlbumId = album.AlbumId,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                storeDB.Carts.Add(cartItem);
            }
            else
            {
                //如果商品确实存在于购物车中，
                //然后在数量上加1
                cartItem.Count++;
            }
            // Save changes
            storeDB.SaveChanges();
        }
        /// <summary>
        /// RemoveFromCart采用唱片集 ID，并将其从用户的购物车中删除。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int RemoveFromCart(int id)
        {
            // Get the cart
            var cartItem = storeDB.Carts.Single(
                cart => cart.CartId == ShoppingCartId
                && cart.RecordId == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    storeDB.Carts.Remove(cartItem);
                }
                //保存更改
                storeDB.SaveChanges();
            }
            return itemCount;
        }
        /// <summary>
        /// EmptyCart从用户的购物车中移除所有项。
        /// </summary>
        public void EmptyCart()
        {
            var cartItems = storeDB.Carts.Where(
                cart => cart.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                storeDB.Carts.Remove(cartItem);
            }
            //保存更改
            storeDB.SaveChanges();
        }
        /// <summary>
        /// GetCartItems检索以便进行显示或处理 CartItems 的列表。
        /// </summary>
        /// <returns></returns>
        public List<Cart> GetCartItems()
        {
            return storeDB.Carts.Where(
                cart => cart.CartId == ShoppingCartId).ToList();
        }
        /// <summary>
        /// GetCount检索用户具有在其购物车中的唱片集的总数。
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            //获取购物车中每一项的数量，并将它们相加
            int? count = (from cartItems in storeDB.Carts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Count).Sum();
            // Return 0 if all entries are null
            return count ?? 0;
        }
        /// <summary>
        /// GetTotal计算购物车中的所有项的总成本。
        /// </summary>
        /// <returns></returns>
        public decimal GetTotal()
        {
            //用专辑价格乘以要得到的专辑数量
            //购物车中每个相册的当前价格
            //将所有专辑的价格加起来得到购物车的总价格
            decimal? total = (from cartItems in storeDB.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Count *
                              cartItems.Album.Price).Sum();

            return total ?? decimal.Zero;
        }
        /// <summary>
        /// CreateOrder签出阶段将购物车转换为订单。
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int CreateOrder(Order order)
        {
            decimal orderTotal = 0;

            var cartItems = GetCartItems();
            //遍历购物车中的项目，
            //添加每个订单的详细信息
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    AlbumId = item.AlbumId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Album.Price,
                    Quantity = item.Count
                };
                //设置购物车的订单总数
                orderTotal += (item.Count * item.Album.Price);

                storeDB.OrderDetails.Add(orderDetail);

            }
            //将订单的总数设置为orderTotal count
            order.Total = orderTotal;

            //保存订单
            storeDB.SaveChanges();
            //清空购物车
            EmptyCart();
            //返回OrderId作为确认号
            return order.OrderId;
        }
        //我们使用HttpContextBase来访问cookie。
        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] =
                        context.User.Identity.Name;
                }
                else
                {
                    //使用系统生成一个新的随机GUID。Guid类
                    Guid tempCartId = Guid.NewGuid();
                    //将tempCartId作为cookie发送回客户端
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return context.Session[CartSessionKey].ToString();
        }
        //当用户登录后，将购物车迁移到
        //与他们的用户名相关联
        public void MigrateCart(string userName)
        {
            var shoppingCart = storeDB.Carts.Where(
                c => c.CartId == ShoppingCartId);

            foreach (Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
            storeDB.SaveChanges();
        }
    }
}