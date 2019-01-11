using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcMusicStore.Models;

namespace MvcMusicStore.ViewModels
{
    public class ShoppingCartViewModel
    {
        //车商品
        public List<Cart> CartItems { get; set; }
        //要在购物车中保存的所有项的总价格
        public decimal CartTotal { get; set; }
    }
}