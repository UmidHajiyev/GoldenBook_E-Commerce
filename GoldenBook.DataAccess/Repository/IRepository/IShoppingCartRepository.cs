
using GoldenBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBook.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        public int IncrementCount(ShoppingCart shoppingCart, int Count);
        public int DecrementCount(ShoppingCart shoppingCart, int Count);

    }

    
}
